using BaSalesManagementApp.Business.Services;
using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    [Authorize(Roles = "Admin,Manager")]   //Kategori sayfasına çalışan erişimi olmasın
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly IEmployeeService _employeeService;
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        public CategoryController(ICategoryService categoryService, IStringLocalizer<Resource> stringLocalizer, IEmployeeService employeeService, IProductService productService, ICompanyService companyService)
        {
            _categoryService = categoryService;
            _stringLocalizer = stringLocalizer;
            _employeeService = employeeService;
            _productService = productService;
            _companyService = companyService;
        }




        /// <summary>
        /// Kategori listesini görüntüleyen action metodu.
        /// Sayfalama, sıralama ve kullanıcı rolüne göre filtreleme içerir.
        /// </summary>
        /// <param name="page">Sayfa numarası (varsayılan: 1)</param>
        /// <param name="pageSize">Sayfa başına gösterilecek kategori sayısı (varsayılan: 10)</param>
        /// <param name="sortOrder">Kategorilerin sıralama düzeni (varsayılan: alfabetik)</param>
        /// <returns>Kategori listesini içeren bir View döndürür.</returns>
        public async Task<IActionResult> Index(
       int? page,
       int pageSize = 10,
       string sortOrder = "alphabetical",
       Guid? companyId = null,
       string? searchQuery = null)  
        {
            try
            {
                var result = await _categoryService.GetAllAsync(sortOrder);
                if (!result.IsSuccess || result.Data == null)
                {
                    NotifyError(_stringLocalizer["Kategori listesi bulunamadı."]);
                    return View(new List<CategoryListVM>().ToPagedList(page ?? 1, pageSize));
                }

                var categories = result.Data.Adapt<List<CategoryListVM>>();

                // Şirket filtresi
                if (companyId.HasValue)
                    categories = categories.Where(c => c.CompanyId == companyId.Value).ToList();

                //  Arama filtresi (sadece kategori adı içinde)
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    categories = categories
                        .Where(c => c.Name != null &&
                                    c.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var paginatedList = categories.ToPagedList(page ?? 1, pageSize);

                //  ViewData içine CurrentQuery eklendi
                ViewData["CurrentSortOrder"] = sortOrder;
                ViewData["CurrentPage"] = page ?? 1;
                ViewData["CurrentPageSize"] = pageSize;
                ViewData["CurrentCompanyId"] = companyId;
                ViewData["CurrentSearchQuery"] = searchQuery;
                //ViewData["DisableGlobalSearch"] = true;

                var companiesResult = await _companyService.GetAllAsync();
                ViewBag.Companies = new SelectList(companiesResult.Data, "Id", "Name", companyId);

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }







        private async Task<List<CategoryListVM>> GetFilteredCategoriesAsync(string sortOrder)
        {
            var result = await _categoryService.GetAllAsync(sortOrder);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CATEGORY_LIST_EMPTY]);
                return new List<CategoryListVM>();
            }

            var categoryListVMs = result.Data.Adapt<List<CategoryListVM>>();

            if (User.IsInRole("Manager"))
            {
                return await FilterCategoriesForManagerAsync(categoryListVMs);
            }

            return categoryListVMs;
        }

        private async Task<List<CategoryListVM>> FilterCategoriesForManagerAsync(List<CategoryListVM> categoryListVMs)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return categoryListVMs;

            var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);
            if (string.IsNullOrEmpty(companyId)) return categoryListVMs;

            var companyGuid = Guid.Parse(companyId);

            // Şirket filtresi olmadan tüm kategorileri al
            var allCategories = categoryListVMs.ToList();

            // Şirket filtresi ile yalnızca o şirkete ait kategorileri al
            var filteredByCompany = allCategories.Where(c => c.CompanyId == companyGuid).ToList();

            // Şirketin ürünlerine bağlı kategorileri al
            var productsList = await _productService.GetProductsByCompanyIdAsync(companyGuid);
            var categoryList = await _categoryService.GetCategoriesByProductListAsync(productsList);

            // Hem eski (tüm kategoriler) hem de yeni eklenen kategoriler (şirketin ürünleriyle ilişkili kategoriler) birleştir
            var combinedCategories = allCategories
                .Where(c => filteredByCompany.Any(fc => fc.Id == c.Id) || categoryList.Any(cl => cl.Id == c.Id))
                .ToList();

            // Kombine kategorileri döndür
            return combinedCategories;
        }

        private IActionResult HandleException(Exception ex)
        {
            NotifyError($"Kategorileri getirirken bir hata meydana geldi: {ex.Message}");
            return View("Error");
        }

        // Yeni bir kategori oluşturur ve ana sayfaya yönlendirir.
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                var errorMessage = "Sayfa yüklenirken bir hata meydana geldi: " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVM categoryCreateVM)
        {
            try
            {
                categoryCreateVM.Name = StringUtilities.CapitalizeEachWord(categoryCreateVM.Name);

                // Kullanıcı ID'yi al
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                string companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                // Eğer Manager ise ve companyId boş veya geçersizse hata döndür
                if (User.IsInRole("Manager"))
                {
                    if (string.IsNullOrEmpty(companyId) || !Guid.TryParse(companyId, out Guid companyGuid))
                    {
                        NotifyError("Şirket ID'niz bulunamadı veya geçersiz, kategori eklenemiyor.");
                        return View(categoryCreateVM);
                    }

                    categoryCreateVM.CompanyId = companyGuid;  // Artık modelde var
                }

                // Aynı isimde kategori var mı kontrol et
                var categoriesResult = await _categoryService.GetAllAsync();
                if (categoriesResult.IsSuccess)
                {
                    var categories = categoriesResult.Data;
                    if (categories.Any(c => c.Name.Equals(categoryCreateVM.Name, StringComparison.OrdinalIgnoreCase) &&
                                            c.CompanyId == categoryCreateVM.CompanyId)) // Güvenli karşılaştırma
                    {
                        ModelState.AddModelError("Name", _stringLocalizer["Bu isimde kategori zaten mevcut, farklı bir kategori giriniz"]);
                        return View(categoryCreateVM);
                    }
                }

                // Yeni kategoriyi DTO'ya çevir
                var newCategory = categoryCreateVM.Adapt<CategoryCreateDTO>();

                // Yeni kategori oluşturma işlemi
                var result = await _categoryService.AddAsync(newCategory);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CATEGORY_CREATE_FAILED]);
                    return View(categoryCreateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.CATEGORY_CREATED_SUCCESS]);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var errorMessage = _stringLocalizer["Kategori oluşturulurken hata oluştu: "] + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        // Belirtilen ID'li kategoriyi siler ve ana sayfaya yönlendirir.
        public async Task<IActionResult> Delete(Guid categoryId)
        {
            try
            {
                bool isCategoryInUse = (await _categoryService.HasProductsAsync(categoryId)).Data;

                if (isCategoryInUse == true)
                {
                    NotifyError(_stringLocalizer[Messages.CATEGORY_DELETE_FAILED_HAS_PRODUCTS]);
                    return RedirectToAction("Index");
                }
                var result = await _categoryService.DeleteAsync(categoryId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CATEGORY_DELETE_FAILED]);
                    // NotifyError(result.Message);
                    return View();
                }

                NotifySuccess(_stringLocalizer[Messages.CATEGORY_DELETED_SUCCESS]);
                // NotifySuccess(result.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var errorMessage = "Kategori silinirken bir hata meydana geldi: " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        // Belirtilen ID'li kategorinin detaylarını gösterir.
        public async Task<IActionResult> Details(Guid categoryId)
        {
            var result = await _categoryService.GetByIdAsync(categoryId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CATEGORY_NOT_FOUND]);
                // NotifyError(result.Message);
                return View();
            }

            NotifySuccess(_stringLocalizer[Messages.CATEGORY_FOUND_SUCCESS]);
            // NotifySuccess(result.Message);
            return View(result.Data.Adapt<CategoryDetailsVM>());
        }

        // Belirtilen ID'li kategoriyi günceller.
        public async Task<IActionResult> Update(Guid categoryId)
        {

            var result = await _categoryService.GetByIdAsync(categoryId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CATEGORY_UPDATED_FAILED]);
                // NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.CATEGORY_UPDATED_SUCCESS]);
                // NotifySuccess(result.Message);
            }

            var categoryUpdateVM = result.Data.Adapt<CategoryUpdateVM>();
            return View(categoryUpdateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryUpdateVM categoryUpdateVM)
        {

            if (!ModelState.IsValid)
            {
                return View(categoryUpdateVM);
            }

            categoryUpdateVM.Name = StringUtilities.CapitalizeFirstLetter(categoryUpdateVM.Name);

            var result = await _categoryService.UpdateAsync(categoryUpdateVM.Adapt<CategoryUpdateDTO>());

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CATEGORY_UPDATED_FAILED]);
                // NotifyError(result.Message);
                return View(categoryUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.CATEGORY_UPDATED_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");

        }
    }
}
