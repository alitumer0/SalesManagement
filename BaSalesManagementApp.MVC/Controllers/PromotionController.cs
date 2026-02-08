using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.PromotionDTOs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.PromotionVMs;
using X.PagedList;
using Microsoft.Extensions.Localization;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Core.Utilities.Results;
using BaSalesManagementApp.Dtos.EmployeeDTOs;
using System.ComponentModel.Design;

namespace BaSalesManagementApp.MVC.Controllers
{
    /// <summary>
    /// PromotionController, IPromotionService bağımlılığını alır ve CRUD işlemlerini gerçekleştirir.
    /// </summary>
    public class PromotionController : BaseController
    {
        private readonly IPromotionService _promotionService;
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly ICategoryService _categoryService;
        private readonly IEmployeeService _employeeService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;

        /// <summary>
        /// PromotionController kurucusu, IPromotionService bağımlılığını alır.
        /// </summary>
        /// <param name="promotionService">Promosyon hizmetini alıyoruz</param>
        public PromotionController(IPromotionService promotionService, IProductService productService, ICompanyService companyService, IStringLocalizer<Resource> stringLocalizer, IEmployeeService employeeService, ICategoryService categoryService)
        {
            _promotionService = promotionService;
            _productService = productService;
            _companyService = companyService;
            _stringLocalizer = stringLocalizer;
            _employeeService = employeeService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// Index action,  Promosyon listesi sayfasını döndürür ve belirli filtreleme ve sıralama işlemlerini gerçekleştirir.
        /// </summary>
        /// <returns>Promosyon Listesini getiren sayfa</returns>

        public async Task<IActionResult> Index(string filterStatus = "all", int? page = 1, string sortOrder = "date", int pageSize = 10, Guid? companyIdWithDropDown = null, string? searchQuery = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            int pageNumber = page ?? 1;

            // Promosyonları getirme
            IDataResult<List<PromotionListDTO>> result;
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                result = await _promotionService.GetAllAsync(sortOrder, searchQuery);
            }
            else
            {
                result = await _promotionService.GetAllAsync(sortOrder);
            }

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_LISTED_ERROR]);
                return View(Enumerable.Empty<PromotionListVM>().ToPagedList(pageNumber, pageSize));
            }

            var promotions = result.Data;

            // Tarih aralığı filtreleme
            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                promotions = promotions.Where(p => p.StartDate.Date >= start).ToList();
            }
            if (endDate.HasValue)
            {
                var end = endDate.Value.Date;
                promotions = promotions.Where(p => p.EndDate.Date <= end).ToList();
            }

            //Aktif kolonuna tıklanınca önce aktif-pasif gösterme
            if (sortOrder == "isactive_asc")
            {
                promotions = promotions.OrderByDescending(p => p.IsActive).ToList(); // Aktif üstte
            }
            else if (sortOrder == "isactive_desc")
            {
                promotions = promotions.OrderBy(p => p.IsActive).ToList(); // Pasif üstte
            }

            // Filtreleme işlemi
            if (filterStatus == "active")
            {
                promotions = promotions.Where(p => p.IsActive).ToList();
            }
            else if (filterStatus == "inactive")
            {
                promotions = promotions.Where(p => !p.IsActive).ToList();
            }

            // Şirket filtrelemesi
            if (companyIdWithDropDown != null)
            {
                promotions = promotions.Where(p => p.CompanyId == companyIdWithDropDown.Value).ToList();
            }

            // ViewModel'e dönüştür
            var promotionListVM = promotions.Adapt<List<PromotionListVM>>();
            var paginatedList = promotionListVM.ToPagedList(pageNumber, pageSize);

            // Manager rolü için filtreleme
            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userIdClaim);
                    if (!string.IsNullOrEmpty(companyId))
                    {
                        var promotionList = await _promotionService.GetPromotionsListByCompanyIdAsync(Guid.Parse(companyId));
                        paginatedList = promotionList.Adapt<List<PromotionListVM>>().ToPagedList(pageNumber, pageSize);
                    }
                }
            }

            // Şirketlerin listelenmesi
            var companiesResult = await _companyService.GetAllAsync();
            ViewBag.Companies = companiesResult.IsSuccess
                 ? companiesResult.Data.Adapt<List<CompanyDTO>>()
                 : new List<CompanyDTO>();

            // Seçilen şirket ID'sinin ViewData'ya aktarılması
            ViewData["SelectedCompanyId"] = companyIdWithDropDown;

            // ViewData ile seçili parametreleri taşıma
            ViewData["CurrentFilterStatus"] = filterStatus;
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["CurrentPageSize"] = pageSize;
            ViewData["CurrentSearchQuery"] = searchQuery;
            ViewData["CurrentStartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["CurrentEndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(paginatedList);
        }


        /// <summary>
        /// Details action, Promosyon detaylarını getirme işlemini gerçekleştirir.
        /// </summary>
        /// <param name="promotionId">Getirilecek promosyonun ID 'si</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(Guid promotionId)
        {
            var result = await _promotionService.GetByIdAsync(promotionId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_GETBYID_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            NotifySuccess(_stringLocalizer[Messages.PROMOTION_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            var promotionDetailsVM = result.Data.Adapt<PromotionDetailsVM>();
            return View(promotionDetailsVM);
        }

        /// <summary>
        /// Create action (HttpGet), yeni bir promosyon oluşturma sayfasını döndürür.
        /// </summary>
        /// <returns>Yeni bir promosyon oluşturma sayfası</returns>
        [HttpGet]
        public async Task<IActionResult> Create(Guid? companyId)
        {
            var companyResult = await _companyService.GetAllAsync();

            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var managerCompanyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                    if (Guid.TryParse(managerCompanyId, out var parsedCompanyId))
                    {
                        var productResult = await _productService.GetProductsByCompanyIdAsync(parsedCompanyId);

                        var model = new PromotionCreateVM
                        {
                            Products = productResult.Data.Adapt<List<ProductDTO>>(),
                            CompanyId = parsedCompanyId
                        };
                        return View(model);
                    }
                }
                return RedirectToAction("Index");
            }
            else // Admin için
            {
                List<ProductDTO> productList = new List<ProductDTO>();
                List<CategoryDTO> categoryList = new List<CategoryDTO>();
                if (companyId.HasValue)
                {
                    var productResult = await _productService.GetProductsByCompanyIdAsync(companyId.Value);
                    productList = productResult.Data.Adapt<List<ProductDTO>>();

                    var categoryResult = await _categoryService.GetCategoriesByCompanyIdAsync(companyId.Value);
                    categoryList = categoryResult.Data.Adapt<List<CategoryDTO>>();
                }

                var model = new PromotionCreateVM
                {
                    Products = productList,
                    Companies = companyResult.Data.Adapt<List<CompanyDTO>>(),
                    Categories = categoryList,
                    CompanyId = companyId,
                };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid promotionId, bool isActive)
        {

            var promotionResult = await _promotionService.GetByIdAsync(promotionId);
            if (!promotionResult.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_GETBYID_ERROR]);
                return RedirectToAction("Index", "Promotion");
            }

            var promotionUpdateDTO = promotionResult.Data.Adapt<PromotionUpdateDTO>();
            promotionUpdateDTO.IsActive = isActive;

            var updateResult = await _promotionService.UpdateAsync(promotionUpdateDTO);
            if (!updateResult.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_UPDATE_ERROR]);
                return RedirectToAction("Index", "Promotion");
            }

            NotifySuccess(_stringLocalizer[Messages.PROMOTION_UPDATE_SUCCESS]);
            return RedirectToAction("Index", "Promotion");

        }


        /// <summary>
        /// Create action, yeni bir promosyon oluşturma işlemini gerçekleştirir.
        /// </summary>
        /// <param name="promotionCreateVM">Oluşturulacak promosyonun verileri</param>
        /// <returns>Promosyon Oluşturma işleminin sonucu</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromotionCreateVM promotionCreateVM)
        {
            try
            {
                //Fiyatları kontrol etmek için log yazılıyor
                Console.WriteLine($"Gelen Fiyat (String): {promotionCreateVM.Price}");
                Console.WriteLine($"Gelen Toplam Fiyat (String): {promotionCreateVM.TotalPrice}");

                // Modelden gelen fiyatların formatı yanlış olduğundan burada düzeltme yapılıyor
                promotionCreateVM.Price = promotionCreateVM.Price / 100;
                promotionCreateVM.TotalPrice = promotionCreateVM.TotalPrice / 100;

                var result = await _promotionService.AddAsync(promotionCreateVM.Adapt<PromotionCreateDTO>());
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.PROMOTION_CREATE_ERROR]);
                    return View(promotionCreateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.PROMOTION_CREATE_SUCCESS]);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                NotifyError("Bir hata oluştu. Lütfen tekrar deneyin.");
                return View(promotionCreateVM);
            }
        }


        /// <summary>
        /// Update action (HttpGet), belirli bir promosyonu güncelleme sayfasını döndürür.
        /// </summary>
        /// <param name="promotionId">Güncellenecek promosyonun ID 'si</param>
        /// <returns>Güncellenecek promosyonu Getirme işleminin sonucu</returns>
        [HttpGet]
        public async Task<IActionResult> Update(Guid promotionId)
        {
            var result = await _promotionService.GetByIdAsync(promotionId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_GETBYID_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            NotifySuccess(_stringLocalizer[Messages.PROMOTION_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);

            var productResult = await _productService.GetAllAsync();
            var comnpanyResult = await _companyService.GetAllAsync();
            //var products = productResult.Data?.Adapt<List<ProductDTO>>() ?? new List<ProductDTO>();

            var promotionUpdateVM = result.Data.Adapt<PromotionUpdateVM>();
            promotionUpdateVM.Products = productResult.Data.Adapt<List<ProductDTO>>();
            promotionUpdateVM.Companies = comnpanyResult.Data.Adapt<List<CompanyDTO>>();

            return View(promotionUpdateVM);
        }//

        /// <summary>
        /// Update action, belirli bir promosyonu güncelleme işlemini gerçekleştirir.
        /// </summary>
        /// <param name="promotionUpdateVM">Güncellenecek promosyonun verileri</param>
        /// <returns>Promosyon Güncelleme işleminin sonucu</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PromotionUpdateVM promotionUpdateVM)
        {
            var result = await _promotionService.UpdateAsync(promotionUpdateVM.Adapt<PromotionUpdateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_UPDATE_ERROR]);
                //NotifyError(result.Message);
                return View(promotionUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.PROMOTION_UPDATE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete action, Id 'si belirtilen bir promosyonu silme işlemini gerçekleştirir.
        /// </summary>
        /// <param name="promotionId">silinecek promosyonun ID 'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        public async Task<IActionResult> Delete(Guid promotionId)
        {
            var result = await _promotionService.DeleteAsync(promotionId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_DELETE_ERROR]);
                //NotifyError(result.Message);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.PROMOTION_DELETE_SUCCESS]);
                //NotifySuccess(result.Message);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GetProductsByCompanyId action, Id 'si belirtilen bir şirketin ürünlerini getirme işlemini gerçekleştirir.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProductsByCompanyId(Guid companyId)
        {
            var result = await _productService.GetProductsByCompanyIdAsync(companyId);
            if (!result.IsSuccess)
            {
                return Json(new List<ProductDTO>());
            }
            return Json(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesByCompanyId(Guid companyId)
        {
            var result = await _categoryService.GetAllAsync();
            if (!result.IsSuccess)
            {
                return Json(new List<CategoryDTO>());
            }

            var categories = result.Data
                .Where(c => c.CompanyId == companyId)
                .ToList();

            return Json(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByCategoryId(Guid categoryId)
        {
            var result = await _productService.GetAllAsync();
            if (!result.IsSuccess)
            {
                return Json(new List<ProductDTO>());
            }

            var products = result.Data
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    price = p.Price
                })
                .ToList();

            return Json(products);
        }

        
    }
}