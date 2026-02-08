using BaSalesManagementApp.Business.Interfaces;
using BaSalesManagementApp.Business.Services;
using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.WarehouseDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.BranchVMs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.OrderVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    public class WarehouseController : BaseController
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;
        private readonly IEmployeeService _employeeService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public WarehouseController(IWarehouseService warehouseService, ICompanyService companyService, IBranchService branchService, IStringLocalizer<Resource> stringLocalizer, IEmployeeService employeeService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _warehouseService = warehouseService;
            _branchService = branchService;
            _companyService = companyService;
            _stringLocalizer = stringLocalizer;
            _employeeService = employeeService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Depo listesini getirir ve sayfalama (pagination) uygular.
        /// Kullanıcı "Manager" rolündeyse, sadece kendi şirketine ait depoları görüntüler.
        /// </summary>
        /// <param name="page">Sayfa numarası (varsayılan: 1)</param>
        /// <param name="sortOrder">Sıralama kriteri (varsayılan: "alphabetical")</param>
        /// <param name="pageSize">Sayfa başına gösterilecek öğe sayısı (varsayılan: 10)</param>
        /// <returns>Sayfalanmış ve filtrelenmiş depo listesi içeren bir View</returns>
        /// <remarks>
        /// - Tüm depoları getirir ve belirlenen sıralama düzenine göre sıralar.
        /// - Eğer kullanıcı "Manager" rolündeyse, yalnızca kendi şirketine bağlı şubelere ait depoları filtreler.
        /// - Sonuçları sayfalayarak View'e döndürür.
        /// - Başarılı işlemlerde kullanıcıya bilgilendirme mesajı gösterir.
        /// - Hata durumunda bir hata mesajı göstererek "Error" View'ini döndürür.
        /// </remarks>
        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical", Guid? companyId = null, int pageSize = 10,
            string? searchQuery = null)
        {

            try
            {
                int pageNumber = page ?? 1;
                var term = string.IsNullOrWhiteSpace(searchQuery) ? null : searchQuery.Trim();

                // Şirket listesi (filtre dropdown’ı için)
                var companiesResult = await _companyService.GetAllAsync();
                ViewBag.Companies = companiesResult.IsSuccess
                    ? companiesResult.Data.Adapt<List<CompanyDTO>>()
                    : new List<CompanyDTO>();

                // View tarafında korunacak state’ler
                ViewData["SelectedCompanyId"] = companyId;
                ViewData["CurrentSortOrder"] = sortOrder;
                ViewData["CurrentPageSize"] = pageSize;
                ViewData["CurrentSearchQuery"] = term ?? string.Empty; // navbar input’unu doldurur

                List<WarehouseListVM> warehouseListVMs;

                // Şirket filtresi varsa önce buna göre çek
                if (companyId is not null && companyId != Guid.Empty)
                {
                    var byCompany = await _warehouseService.GetAllAsyncByCompanyId(companyId);
                    if (!byCompany.IsSuccess)
                    {
                        NotifyError(_stringLocalizer[Messages.Warehouse_LIST_FAILED]);
                        return View(Enumerable.Empty<WarehouseListVM>().ToPagedList(pageNumber, pageSize));
                    }
                    warehouseListVMs = byCompany.Data.Adapt<List<WarehouseListVM>>();
                }
                else
                {
                    // Şirket filtresi yoksa servis tarafında aramayı kullanalım
                    var all = await _warehouseService.GetAllAsync(sortOrder, term);
                    if (!all.IsSuccess)
                    {
                        NotifyError(_stringLocalizer[Messages.Warehouse_LIST_FAILED]);
                        return View(Enumerable.Empty<WarehouseListVM>().ToPagedList(pageNumber, pageSize));
                    }
                    warehouseListVMs = all.Data.Adapt<List<WarehouseListVM>>();
                }

                // Manager ise sadece kendi şirketine ait depolar kalsın
                if (User.IsInRole("Manager"))
                {
                    var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var companyIdOfUser = await _employeeService.GetCompanyIdByUserIdAsync(userId);
                        if (!string.IsNullOrEmpty(companyIdOfUser))
                        {
                            var branchList = await _branchService.GetBranchesByCompanyIdAsync(Guid.Parse(companyIdOfUser));
                            var allowedWarehouses = await _warehouseService.GetWarehousesByBranchIdAsync(branchList);
                            var allowedIds = allowedWarehouses.Select(w => w.Id).ToHashSet();
                            warehouseListVMs = warehouseListVMs.Where(w => allowedIds.Contains(w.Id)).ToList();
                        }
                    }
                }

                // Şirket filtresi açıkken servis arama almıyorsa (GetAllAsyncByCompanyId),
                // listedekilere elde arama uygula (İSİM/ADRES/ŞUBE)
                if (!string.IsNullOrWhiteSpace(term) && companyId is not null && companyId != Guid.Empty)
                {
                    warehouseListVMs = warehouseListVMs.Where(w =>
                        (!string.IsNullOrEmpty(w.Name) && w.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(w.Address) && w.Address.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(w.BranchName) && w.BranchName.Contains(term, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                var paginatedList = warehouseListVMs.ToPagedList(pageNumber, pageSize);
                return View(paginatedList);
            }
            catch (Exception ex)
            {
                NotifyError("Depoları getirirken bir hata meydana geldi: " + ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Depo oluşturma işlemi için GET isteğini işler.
        /// </summary>
        /// <returns>
        /// Kullanıcının rolüne göre önceden doldurulmuş bir <see cref="WarehouseCreateVM"/> modeli ile 
        /// Create görünümünü döndürür.
        /// </returns>
        ///
        /// <remarks>
        /// - Kullanıcı "Manager" rolündeyse, bağlı olduğu şirketin kimliği alınır 
        ///   ve sadece o şirkete ait şubeler listeye eklenir.
        /// - Kullanıcı farklı bir role sahipse, tüm şirketler getirilerek seçim yapmasına olanak tanınır.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var warehouseCreateVM = new WarehouseCreateVM();

            if (userRole == "Manager")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userId, out var userIdGuid))
                {
                    var result = await _employeeService.GetCompanyIdByUserIdAsync(userIdGuid);
                    if (result.Data != null)
                    {
                        var userCompanyId = result.Data.Value;
                        warehouseCreateVM.SelectedCompanyId = userCompanyId;
                        warehouseCreateVM.Branches = (await _branchService.GetBranchesByCompanyIdAsync(userCompanyId))?.Adapt<List<BranchDTO>>() ?? new List<BranchDTO>();
                    }
                }
            }
            else
            {
                warehouseCreateVM.Companies = (await _companyService.GetAllAsync()).Data?.Adapt<List<CompanyDTO>>() ?? new List<CompanyDTO>();
            }

            return View(warehouseCreateVM);
        }

        /// <summary>
        /// Belirtilen şirket kimliğine göre şubeleri getirir.
        /// </summary>
        /// <param name="companyId">Şubeleri alınacak şirketin kimliği.</param>
        /// <returns>
        /// - Geçerli bir şirket kimliği girilmezse <see cref="BadRequest"/> döner.
        /// - Şirkete ait şube bulunamazsa <see cref="NotFound"/> döner.
        /// - Şubeler başarıyla bulunursa JSON formatında <see cref="List{BranchDTO}"/> döner.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetBranchesByCompany(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                return BadRequest("Geçerli bir şirket seçilmelidir.");
            }

            var branches = await _branchService.GetBranchesByCompanyIdAsync(companyId);

            if (branches == null || !branches.Any())
            {
                return NotFound("Bu şirkete ait şube bulunamadı.");
            }

            return Json(branches.Adapt<List<BranchDTO>>());
        }


        /// <summary>
        /// Yeni bir depo oluşturur ve ana sayfaya yönlendirir.      
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(WarehouseCreateVM warehouseCreateVM)
        {
            //var branchesResult = await _branchService.GetAllAsync();
            //warehouseCreateVM.Branches = branchesResult.Data?.Adapt<List<BranchDTO>>() ?? new List<BranchDTO>();
            if (!ModelState.IsValid)
            {
                return View(warehouseCreateVM);
            }

            warehouseCreateVM.Name = StringUtilities.CapitalizeEachWord(warehouseCreateVM.Name);
            warehouseCreateVM.Address = StringUtilities.CapitalizeEachWord(warehouseCreateVM.Address);

            var result = await _warehouseService.AddAsync(warehouseCreateVM.Adapt<WarehouseCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_CREATE_FAILED]);
                //NotifyError(result.Message);
                return View(warehouseCreateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.Warehouse_CREATED_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen ID'li depoyu siler ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="warehouseId">Silinecek siparişin ID'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid warehouseId)
        {
            var result = await _warehouseService.DeleteAsync(warehouseId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_DELETE_FAILED]);
                //NotifyError(result.Message);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.Warehouse_DELETED_SUCCESS]);
                // NotifySuccess(result.Message);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen depo bilgilerini alıp güncelleme sayfası oluşturur
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> Update(Guid warehouseId)
        {

            var result = await _warehouseService.GetByIdAsync(warehouseId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_GET_FAILED]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var branchesResult = await _branchService.GetAllAsync();
            var branches = branchesResult.Data?.Adapt<List<BranchDTO>>() ?? new List<BranchDTO>();

            var warehouseEditVM = result.Data.Adapt<WarehouseUpdateVM>();
            warehouseEditVM.Branches = branches;

            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                    var branchlist = await _branchService.GetBranchesByCompanyIdAsync(Guid.Parse(companyId));
                    var warehouseList = await _warehouseService.GetWarehousesByBranchIdAsync(branchlist);
                    if (!warehouseList.Any(w => w.Id == warehouseId))  // warehouseId ile Warehouse nesnesinin Id'sini karşılaştırıyoruz
                    {
                        NotifyError(_stringLocalizer[Messages.Warehouse_GET_FAILED]);
                        return RedirectToAction("Index");
                    }
                    branches = branchlist.Adapt<List<BranchDTO>>() ?? new List<BranchDTO>();
                    warehouseEditVM = result.Data.Adapt<WarehouseUpdateVM>();
                    warehouseEditVM.Branches = branches;

                }
            }


            return View(warehouseEditVM);
        }
        /// <summary>
        /// Belirtilen deponun bilgilerini günceller ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="warehouseUpdateVM "></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update(WarehouseUpdateVM warehouseUpdateVM)
        {
            var updateResult = await _warehouseService.GetByIdAsync(warehouseUpdateVM.Id);
            var branchesResult = await _branchService.GetAllAsync();
            var branches = branchesResult.Data?.Adapt<List<BranchDTO>>() ?? new List<BranchDTO>();

            var warehouseEditVM = updateResult.Data.Adapt<WarehouseUpdateVM>();
            warehouseEditVM.Branches = branches;

            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                    var branchlist = await _branchService.GetBranchesByCompanyIdAsync(Guid.Parse(companyId));
                    var warehouseList = await _warehouseService.GetWarehousesByBranchIdAsync(branchlist);
                    if (!warehouseList.Any(w => w.Id == warehouseUpdateVM.Id))  // warehouseId ile Warehouse nesnesinin Id'sini karşılaştırıyoruz
                    {
                        NotifyError(_stringLocalizer[Messages.Warehouse_GET_FAILED]);
                        return RedirectToAction("Index");
                    }
                    branches = branchlist.Adapt<List<BranchDTO>>() ?? new List<BranchDTO>();

                    warehouseEditVM = updateResult.Data.Adapt<WarehouseUpdateVM>();
                    warehouseEditVM.Branches = branches;

                }
            }

            if (!ModelState.IsValid)
            {
                return View(warehouseUpdateVM);
            }

            warehouseUpdateVM.Name = StringUtilities.CapitalizeEachWord(warehouseUpdateVM.Name);
            warehouseUpdateVM.Address = StringUtilities.CapitalizeEachWord(warehouseUpdateVM.Address);

            var result = await _warehouseService.UpdateAsync(warehouseUpdateVM.Adapt<WarehouseUpdateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_UPDATE_FAILED]);
                //NotifyError(result.Message);
                return View(warehouseUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.Warehouse_UPDATED_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Girilen stok detaylarını gösterir.
        /// </summary>
        /// <param name="warehouseId">Girilen stok ID'si</param>
        /// <returns>Stok detaylarının görüntülendiği sayfa</returns>
        public async Task<IActionResult> Details(Guid warehouseId)
        {

            var result = await _warehouseService.GetByIdAsync(warehouseId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_GET_FAILED]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var warehouseDetailVM = result.Data.Adapt<WarehouseDetailVM>();

            var warehouseName = string.IsNullOrWhiteSpace(warehouseDetailVM.Name)
    ? _stringLocalizer["Unknown Warehouse"] // Depo ismini mesajda göstermesi için isim olup olmadığını  kontrol ediyor
    : warehouseDetailVM.Name;

            var successMessage = string.Format(_stringLocalizer[Messages.Warehouse_DETAILS_LISTED_SUCCESS], warehouseName);
            NotifySuccess(successMessage);
            return View(warehouseDetailVM);
        }

    }


}
