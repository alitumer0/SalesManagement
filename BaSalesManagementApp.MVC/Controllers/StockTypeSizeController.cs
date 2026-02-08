using BaSalesManagementApp.Business.Services;
using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.ProductTypeDtos;
using BaSalesManagementApp.Dtos.StockTypeSizeDTOs;
using BaSalesManagementApp.MVC.Models.StockTypeSizeVMs;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    public class StockTypeSizeController : BaseController
    {
        private readonly IStockTypeSizeService _stockTypeSizeService;
        private readonly IStockTypeService _productTypeService;
        private readonly IEmployeeService _employeeService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;

        public StockTypeSizeController(IStockTypeSizeService stockTypeSizeService, IStockTypeService productTypeService, IStringLocalizer<Resource> stringLocalizer, IEmployeeService employeeService, IProductService productService, ICategoryService categoryService)
        {
            _stockTypeSizeService = stockTypeSizeService;
            _stringLocalizer = stringLocalizer;
            _productTypeService = productTypeService;
            _employeeService = employeeService;
            _productService = productService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// Bu metot, StockTypeSize verilerini sayfalayarak ve sıralayarak kullanıcıya gösterir. 
        /// Eğer kullanıcı "Manager" rolündeyse, yalnızca kendi şirketine ait veriler gösterilir.
        /// Sayfalama ve sıralama parametrelerine göre veri filtrelenir ve uygun başarı veya hata mesajı kullanıcıya iletilir.
        /// </summary>
        /// <param name="page">Sayfa numarası</param>
        /// <param name="sortOrder">Sıralama düzeni (örn. 'sizeName_asc', 'date_desc')</param>
        /// <param name="pageSize">Sayfa başına gösterilecek veri sayısı (varsayılan: 10)</param>
        /// <returns>Sayfalı ve sıralı StockTypeSize verisi içeren bir View döner.</returns>
        public async Task<IActionResult> Index(int? page, string sortOrder, int pageSize = 10, string searchQuery = null)
        {
            int pageNumber = page ?? 1;
            ViewBag.CurrentSort = sortOrder;
            ViewData["CurrentPageSize"] = pageSize;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Guid? userCompanyId = null;

            // Kullanıcı Manager ise kendi şirketine ait verileri listele
            if (userRole == "Manager" && Guid.TryParse(userId, out var userIdGuid))
            {
                var companyResult = await _employeeService.GetCompanyIdByUserIdAsync(userIdGuid);
                if (companyResult.IsSuccess && companyResult.Data.HasValue)
                {
                    userCompanyId = companyResult.Data.Value;
                }
                else
                {
                    NotifyError(_stringLocalizer[Messages.COMPANY_LISTED_NOTFOUND]);
                    return View(Enumerable.Empty<StockTypeSizeListVM>().ToPagedList(pageNumber, pageSize));
                }
            }

            // StockTypeSize listesini al
            var result = await _stockTypeSizeService.GetAllAsync();
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_LIST_FAILED]);
                return View(Enumerable.Empty<StockTypeSizeListVM>().ToPagedList(pageNumber, pageSize));
            }

            var stockTypeSizeVM = result.Data?.Adapt<List<StockTypeSizeListVM>>() ?? new List<StockTypeSizeListVM>();

            // Eğer kullanıcı Manager ise sadece kendi şirketine ait verileri filtrele
            if (userCompanyId.HasValue)
            {
                stockTypeSizeVM = stockTypeSizeVM
                    .Where(x => x.CompanyId == userCompanyId.Value)  // Yeni filtreleme
                    .ToList();
            }

            // Sıralama işlemi
            stockTypeSizeVM = sortOrder switch
            {
                "sizeName_asc" => stockTypeSizeVM.OrderBy(x => x.Size).ToList(),
                "sizeName_desc" => stockTypeSizeVM.OrderByDescending(x => x.Size).ToList(),
                "date_asc" => stockTypeSizeVM.OrderBy(x => x.CreatedDate).ToList(),
                "date_desc" => stockTypeSizeVM.OrderByDescending(x => x.CreatedDate).ToList(),
                _ => stockTypeSizeVM.OrderBy(x => x.Size).ToList()
            };

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                stockTypeSizeVM = stockTypeSizeVM
                    .Where(x => x.Size.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            ViewData["CurrentSearchQuery"] = searchQuery;

            return View(stockTypeSizeVM.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var productTypeResult = await _productTypeService.GetAllAsync();
            var producTypes = productTypeResult.Data?.Adapt<List<StockTypeDto>>() ?? new List<StockTypeDto>();

            var stockTypeSizeCreateVM = new StockTypeSizeCreateVM()
            {
                StockTypes = producTypes,
            };
            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;

                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                    var productList = await _productService.GetProductsByCompanyIdAsync(Guid.Parse(companyId));

                    var categoryList = await _categoryService.GetCategoriesByProductListAsync(productList);

                    var productTypeList = await _productTypeService.GetProductTypeListByCategoryListAsyncs(categoryList);

                    //stockTypeSizeCreateVM = productTypeList.Adapt<List<StockTypeSizeCreateVM>>();

                    stockTypeSizeCreateVM = new StockTypeSizeCreateVM()
                    {
                        StockTypes = productTypeList.Data,
                    };
                }
            }
            return View(stockTypeSizeCreateVM);
        }

        /// <summary>
        /// Yeni bir Stok Tipi Boyutu oluşturur ve ana sayfaya yönlendirir.      
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(StockTypeSizeCreateVM stockTypeSizeCreateVM)
        {
            // Ürün tiplerini al
            var stockTypes = await _productTypeService.GetAllAsync();
            stockTypeSizeCreateVM.StockTypes = stockTypes.Data?.Adapt<List<StockTypeDto>>() ?? new List<StockTypeDto>();

            // Model doğrulama
            if (!ModelState.IsValid)
            {
                return View(stockTypeSizeCreateVM);
            }

            // Aynı boyut, ürün ve ürün tipi olup olmadığını kontrol et
            var existingStockTypeSizes = await _stockTypeSizeService.GetAllAsync();
            bool isDuplicate = existingStockTypeSizes.Data?.Any(x =>
                x.Size.Equals(stockTypeSizeCreateVM.Size, StringComparison.OrdinalIgnoreCase) &&
                x.Description == stockTypeSizeCreateVM.Description &&
                x.StockTypeId == stockTypeSizeCreateVM.StockTypeId) ?? false;

            if (isDuplicate)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_ALREADY_EXISTS]);
                return View(stockTypeSizeCreateVM);
            }

            // Verileri doğru şekilde düzenle
            stockTypeSizeCreateVM.Size = StringUtilities.CapitalizeEachWord(stockTypeSizeCreateVM.Size);
            stockTypeSizeCreateVM.Description = StringUtilities.CapitalizeEachWord(stockTypeSizeCreateVM.Description);

            // Stok birim miktarını ekle
            var result = await _stockTypeSizeService.AddAsync(stockTypeSizeCreateVM.Adapt<StockTypeSizeCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_CREATE_FAILED]);
                return View(stockTypeSizeCreateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_CREATED_SUCCESS]);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen ID'li Stok Tipi Boyutunu siler ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="stockTypeSizeId">Silinecek Stok Tipi Boyutunun ID'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid stockTypeSizeId)
        {
            var result = await _stockTypeSizeService.DeleteAsync(stockTypeSizeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_DELETE_FAILED]);
                //NotifyError(result.Message);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_DELETED_SUCCESS]);
                //NotifySuccess(result.Message);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen Stok Tipi Boyutu bilgilerini alıp güncelleme sayfası oluşturur
        /// </summary>
        /// <param name="stockTypeSizeId"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> Update(Guid stockTypeSizeId)
        {
            var result = await _stockTypeSizeService.GetByIdAsync(stockTypeSizeId);
            if (!result.IsSuccess)
            {

                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_NOT_FOUND]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            var productTypeResult = await _productTypeService.GetAllAsync();
            var productTypes = productTypeResult.Data?.Adapt<List<StockTypeDto>>() ?? new List<StockTypeDto>();


            var stockTypeSizeEditVM = result.Data.Adapt<StockTypeSizeUpdateVM>();
            stockTypeSizeEditVM.StockTypes = productTypes;

            return View(stockTypeSizeEditVM);
        }

        /// <summary>
        /// Belirtilen Stok Tipi Boyutunun bilgilerini günceller ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="stockTypeSizeUpdateVM"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update(StockTypeSizeUpdateVM stockTypeSizeUpdateVM)
        {
            var productTypeResult = await _productTypeService.GetAllAsync();
            stockTypeSizeUpdateVM.StockTypes = productTypeResult.Data?.Adapt<List<StockTypeDto>>() ?? new List<StockTypeDto>();

            if (!ModelState.IsValid)
            {
                return View(stockTypeSizeUpdateVM);
            }

            stockTypeSizeUpdateVM.Size=StringUtilities.CapitalizeEachWord(stockTypeSizeUpdateVM.Size);
            stockTypeSizeUpdateVM.Description=StringUtilities.CapitalizeFirstLetter(stockTypeSizeUpdateVM.Description);

            var result = await _stockTypeSizeService.UpdateAsync(stockTypeSizeUpdateVM.Adapt<StockTypeSizeUpdateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_UPDATE_FAILED]);
                //NotifyError(result.Message);
                return View(stockTypeSizeUpdateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_UPDATE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Girilen Stok Tipi Boyutu detaylarını gösterir.
        /// </summary>
        /// <param name="stockTypeSizeId">Girilen Stok Tipi Boyutu ID'si</param>
        /// <returns>Stok Tipi Boyutu detaylarının görüntülendiği sayfa</returns>
        public async Task<IActionResult> Details(Guid stockTypeSizeId)
        {
            var result = await _stockTypeSizeService.GetByIdAsync(stockTypeSizeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_GET_FAILED]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var stockTypeSizeDetailVM = result.Data.Adapt<StockTypeSizeDetailVM>();
            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_FOUND_SUCCESS]);
            return View(stockTypeSizeDetailVM);
        }

    }
}

