using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Core.Utilities.Results;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.WarehouseDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.ProductVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Localization;
using System.Drawing.Printing;
using System.Formats.Tar;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICompanyService _companyService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly IEmployeeService _employeeService;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            ICompanyService companyService,
            IStringLocalizer<Resource> stringLocalizer,
            IEmployeeService employeeService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _companyService = companyService;
            _stringLocalizer = stringLocalizer;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Kategoriye göre filtrelenen ve belirli bir sıraya göre sıralanan ürün listesini asenkron olarak döndürür.
        /// Ayrıca kategori bilgilerini yükler ve sayfalama yapar.
        /// </summary>
        /// <param name="page">Görüntülenecek sayfa numarası. Varsayılan: 1.</param>
        /// <param name="sortOrder">
        /// Ürünlerin sıralama düzeni ("alphabetical", "alphabeticaldesc", "date", "datedesc", "priceascend", "pricedesc").
        /// </param>
        /// <param name="categoryId">Filtreleme için kategori Id'si. Null ise tüm ürünler getirilir.</param>
        /// <returns>Sıralanmış ve filtrelenmiş ürün listesi.</returns>


        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical", Guid? categoryId = null, Guid? companyId = null, int pageSize = 10, string? searchQuery = null)
        {
            int pageNumber = page ?? 1;

            // Kategorileri al ve ViewBag'e aktar
            var categoriesResult = await _categoryService.GetAllAsync();
            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var companyIdString = await _employeeService.GetCompanyIdByUserIdAsync(userIdClaim);
                    if (!string.IsNullOrEmpty(companyIdString) && Guid.TryParse(companyIdString, out Guid parsedCompanyId))
                    {
                        var productslist = await _productService.GetProductsByCompanyIdAsync(parsedCompanyId);
                        categoriesResult = await _categoryService.GetCategoriesByProductListAsyncs(productslist);
                    }
                }
            }
            ViewBag.Categories = categoriesResult.IsSuccess
                ? categoriesResult.Data.Adapt<List<CategoryDTO>>()
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,   // x.Name senin CategoryDTO içinde görünen metin olacak alan
                        Value = x.Id.ToString()  // x.Id ya da uygun olan başka bir ID alanı
                    })
                : new List<SelectListItem>();

            // Şirketleri al ve ViewBag'e aktar
            List<CompanyDTO> companies;
            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var companyIdString = await _employeeService.GetCompanyIdByUserIdAsync(userIdClaim);
                    if (!string.IsNullOrEmpty(companyIdString) && Guid.TryParse(companyIdString, out Guid parsedCompanyId))
                    {
                        var singleCompanyResult = await _companyService.GetByIdAsync(parsedCompanyId);
                        companies = singleCompanyResult.IsSuccess && singleCompanyResult.Data != null
                            ? new List<CompanyDTO> { singleCompanyResult.Data }
                            : new List<CompanyDTO>();
                    }
                    else
                    {
                        companies = new List<CompanyDTO>();
                    }
                }
                else
                {
                    companies = new List<CompanyDTO>();
                }
            }
            //else
            //{
            //    var companiesResult = await _companyService.GetAllAsync();
            //    companies = companiesResult.IsSuccess && companiesResult.Data != null
            //        ? companiesResult.Data.Adapt<List<CompanyDTO>>() ?? new List<CompanyDTO>()
            //        : new List<CompanyDTO>();
            //}

            //ViewBag.Companies = companies
            //    .Select(x => new SelectListItem
            //    {
            //        Text = x.Name ?? "Bilinmeyen Şirket",
            //        Value = x.Id.ToString()
            //    })
            //    .ToList();

            /* */

            // Seçilen kategori, şirket ve sıralama bilgisini ViewData'ya aktar
            
            //ViewData["SelectedCategoryId"] = categoryId;
            //ViewData["SelectedCompanyId"] = companyId;
            //ViewData["CurrentSort"] = sortOrder; // CurrentSortOrder yerine CurrentSort kullanıyoruz, çünkü View'da bu isimle erişiliyor
            //ViewData["CurrentPageSize"] = pageSize;

            // Service'ten sıralanmış, filtrelenmiş ve sayfalanmış ürünleri al
            var productsDTO = await _productService.GetProductsAsync(sortOrder, 1, int.MaxValue, categoryId, companyId, User); //buda kaldırılacak Hoca istedi
            //CompanySimpleDTO oluşturdum içinde sadece Hocanın istediği gibi Id ve Name var
            var filtered = productsDTO
                    .Where(p => string.IsNullOrEmpty(searchQuery) ||
                    p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            var pagedProducts = filtered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // productsDTO'nun yalnızca mevcut sayfasındaki verilerini ProductListVM'e dönüştür
            var productsVMList = pagedProducts.Adapt<List<ProductListVM>>();

            // Sayfalama bilgisini koruyarak IPagedList<ProductListVM> oluştur
            var productsVM = new StaticPagedList<ProductListVM>(
                productsVMList,
                pageNumber,
                pageSize,
                filtered.Count);

            // Kategori filtresi başarılıysa bildirim göster
            if (categoryId.HasValue && productsVM.Any())
            {
                // Kategori sözlüğünü oluştur
                var categoryDictionary = categoriesResult.IsSuccess
                    ? categoriesResult.Data.ToDictionary(c => c.Id, c => c.Name)
                    : new Dictionary<Guid, string>();

                // Seçilen kategorinin adını alma
                string categoryName = categoryDictionary.TryGetValue(categoryId.Value, out var name)
                    ? name
                    : " ";

                // Başarı mesajını göster
                NotifySuccess(string.Format(_stringLocalizer[Messages.PRODUCT_LISTED_SUCCESS], categoryName));
            }
            else if (categoryId.HasValue && !productsVM.Any())
            {
                // Kategori sözlüğünü oluştur
                var categoryDictionary = categoriesResult.IsSuccess
                    ? categoriesResult.Data.ToDictionary(c => c.Id, c => c.Name)
                    : new Dictionary<Guid, string>();

                // Seçilen kategorinin adını alma
                string categoryName = categoryDictionary.TryGetValue(categoryId.Value, out var name)
                    ? name
                    : " ";

                // Başarısız mesajını göster
                NotifyError(string.Format(_stringLocalizer[Messages.PRODUCT_LISTED_ERROR], categoryName));
            }

            ViewData["CurrentSearchQuery"] = searchQuery;

            // Ürünleri sayfalama için View'e gönder
            return View(productsVM);
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<JsonResult> ListCompaniesJson()
        {
            var res = await _companyService.GetAllAsync();
            var list = (res.IsSuccess && res.Data != null)
                ? res.Data.Select(c => new CompanySimpleDTO { Id = c.Id, Name = c.Name ?? "Bilinmeyen Şirket" }).ToList()
                : new List<CompanySimpleDTO>();
            return Json(list);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Employee"))
            {
                return View("~/Views/Shared/AccessDenied.cshtml");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var categoriesResult = await _categoryService.GetAllAsync();

            List<CompanyDTO> companies = new List<CompanyDTO>();

            if (userRole == "Manager")
            {
                var userResult = await _employeeService.GetByIdentityIdAsync(userId);
                if (userResult.IsSuccess && userResult.Data != null)
                {
                    var companyId = userResult.Data.CompanyId;
                    var companyResult = await _companyService.GetByIdAsync(companyId);
                    if (companyResult.IsSuccess && companyResult.Data != null)
                    {
                        companies.Add(companyResult.Data.Adapt<CompanyDTO>());
                    }
                }

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userIdClaim);
                    if (!string.IsNullOrEmpty(companyId))
                    {
                        categoriesResult = await _categoryService.GetCategoriesByCompanyIdAsync(Guid.Parse(companyId));
                    }
                }

            }
            else
            {
                var companyResult = await _companyService.GetAllAsync();
                companies = companyResult.Data?.Adapt<List<CompanyDTO>>() ?? new List<CompanyDTO>();
            }
            var categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();




            var productCreateVM = new ProductCreateVM
            {
                Categories = categories,
                Companies = companies,
            };

            return View(productCreateVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM productCreateVM)
        {
            if (User.IsInRole("Employee"))
            {
                return View("~/Views/Shared/AccessDenied.cshtml");
            }
            var categoriesResult = await _categoryService.GetAllAsync();
            productCreateVM.Categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            if (!ModelState.IsValid)
            {
                return View(productCreateVM);
            }

            // İsimdeki kelimelerin ilk harflerini büyük yapalım (örnek)
            productCreateVM.Name = StringUtilities.CapitalizeEachWord(productCreateVM.Name);

            Guid companyId;
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Manager")
            {
                // Eğer kullanıcı yönetici ise sadece kendi şirketi için kontrol yap
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                // Eğer metod string döndürüyorsa, önce nullable GUID'e çevirelim
                string companyIdString = await _employeeService.GetCompanyIdByUserIdAsync(userIdClaim);
                if (!Guid.TryParse(companyIdString, out Guid parsedCompanyId))
                {
                    NotifyError("Şirket ID geçersiz veya bulunamadı.");
                    return View(productCreateVM);
                }

                companyId = parsedCompanyId; // String GUID'i, normal GUID olarak kullanıyoruz.
            }
            else
            {
                // Admin farklı şirketlere ürün ekleyebilir, bu yüzden seçilen şirket ID'si kullanılıyor
                companyId = productCreateVM.CompanyId.GetValueOrDefault(Guid.Empty);
            }

            // Aynı isimde ve aynı şirkette ürün olup olmadığını kontrol et
            var existingProduct = await _productService.GetByNameAndCompanyAsync(productCreateVM.Name, companyId);
            if (existingProduct != null)
            {
                // Eğer aynı isimde bir ürün varsa hata mesajı göster
                NotifyError(_stringLocalizer["Bu şirkette bu isimde bir ürün zaten mevcut. Lütfen farklı bir isim seçin."]);
                return View(productCreateVM);
            }

            var result = await _productService.AddAsync(productCreateVM.Adapt<ProductCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCT_CREATED_ERROR]);
                return View(productCreateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCT_CREATED_SUCCESS]);
            return RedirectToAction("Index");
        }






        public async Task<IActionResult> Delete(Guid productId)
        {
            if (User.IsInRole("Employee"))
            {
                return View("~/Views/Shared/AccessDenied.cshtml");
            }
            var result = await _productService.DeleteAsync(productId);
            if (result.IsSuccess)
            {
                NotifySuccess(_stringLocalizer[result.Message]);
            }
            else
            {
                NotifyError(_stringLocalizer[Messages.PRODUCT_DELETED_ERROR]);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(Guid productId)
        {
            if (User.IsInRole("Employee"))
            {
                return View("~/Views/Shared/AccessDenied.cshtml");
            }
            var result = await _productService.GetByIdAsync(productId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCT_NOT_FOUND]);
                return RedirectToAction("Index");
            }

            var productsResult = await _productService.GetAllAsync();
            var productListVMs = result.Data.Adapt<List<ProductListVM>>() ?? new List<ProductListVM>();

            var categoriesResult = await _categoryService.GetAllAsync();
            var categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            var companyResult = await _companyService.GetAllAsync();
            var companies = companyResult.Data?.Adapt<List<CompanyDTO>>() ?? new List<CompanyDTO>();

            var productEditVM = result.Data.Adapt<ProductUpdateVM>();
            productEditVM.Categories = categories;
            productEditVM.Companies = companies;

            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                    var productList = await _productService.GetProductsByCompanyIdAsync(Guid.Parse(companyId));

                    if (!productList.Data.Any(w => w.Id == productId))
                    {
                        NotifyError(_stringLocalizer[Messages.PRODUCT_NOT_FOUND]);
                        return RedirectToAction("Index");
                    }
                    productListVMs = productList.Data.Adapt<List<ProductListVM>>() ?? new List<ProductListVM>();
                    productEditVM = result.Data.Adapt<ProductUpdateVM>();
                    productEditVM.Categories = categories;
                    productEditVM.Companies = companies;
                }
            }

            return View(productEditVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateVM productUpdateVM)
        {
            if (User.IsInRole("Employee"))
            {
                return View("~/Views/Shared/AccessDenied.cshtml");
            }
            // İsimdeki ilk harfi büyük yapalım (örnek)
            productUpdateVM.Name = StringUtilities.CapitalizeFirstLetter(productUpdateVM.Name);

            var productResult = await _productService.GetByIdAsync(productUpdateVM.Id);

            var productListVMs = productResult.Data.Adapt<List<ProductListVM>>() ?? new List<ProductListVM>();

            // Kategori ve Şirket listelerini tekrar dolduruyoruz

            var categoriesResult = await _categoryService.GetAllAsync();
            var categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            var companyResult = await _companyService.GetAllAsync();
            var companies = companyResult.Data?.Adapt<List<CompanyDTO>>() ?? new List<CompanyDTO>();

            var productEditVM = productResult.Data.Adapt<ProductUpdateVM>();
            productEditVM.Categories = categories;
            productEditVM.Companies = companies;

            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                    var productList = await _productService.GetProductsByCompanyIdAsync(Guid.Parse(companyId));

                    if (!productList.Data.Any(w => w.Id == productUpdateVM.Id))
                    {
                        NotifyError(_stringLocalizer[Messages.PRODUCT_NOT_FOUND]);
                        return RedirectToAction("Index");
                    }
                    productListVMs = productList.Data.Adapt<List<ProductListVM>>() ?? new List<ProductListVM>();
                    productEditVM = productResult.Data.Adapt<ProductUpdateVM>();
                    productEditVM.Categories = categories;
                    productEditVM.Companies = companies;
                }
            }

            var result = await _productService.UpdateAsync(productUpdateVM.Adapt<ProductUpdateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCT_UPDATED_ERROR]);
                return View(productUpdateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCT_UPDATED_SUCCESS]);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid productId)
        {
            var result = await _productService.GetByIdAsync(productId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCT_NOT_FOUND]);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.PRODUCT_GET_SUCCESS]);
            }
            return View(result.Data.Adapt<ProductDetailsVM>());
        }

        [HttpGet]
        public async Task<JsonResult> CheckProductInOrder(Guid productId)
        {
            var isInOrder = await _productService.IsProductInOrderAsync(productId);
            return Json(new { isInOrder });
        }

        [HttpGet]
        public async Task<JsonResult> GetCategoriesByCompany(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                return Json(new List<object>()); // Eğer geçerli bir ID gelmezse boş liste döndür
            }

            var categoriesResult = await _categoryService.GetCategoriesByCompanyIdAsync(companyId);
            if (!categoriesResult.IsSuccess)
            {
                return Json(new List<object>()); // Başarısız sonuçta boş liste döndür
            }

            var categories = categoriesResult.Data
                ?.Select(c => new { id = c.Id, name = c.Name })
                .ToList();

            return Json(categories);

        }

    }
}
