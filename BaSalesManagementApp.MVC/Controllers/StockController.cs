using BaSalesManagementApp.Business.Interfaces;
using BaSalesManagementApp.Business.Services;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.StockDTOs;
using BaSalesManagementApp.MVC.Models.StockVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{

    public class StockController : BaseController
    {
        private readonly IStockService _stockService;
        private readonly IProductService _productService;
        private readonly IStringLocalizer<StockController> _localizer;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly IWarehouseService _warehouseService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmployeeService _employeeService;
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;

        public StockController(IStockService stockService, IProductService productService, IStringLocalizer<StockController> localizer, IStringLocalizer<Resource> stringLocalizer, IWarehouseService warehouseService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IEmployeeService employeeService, IBranchService branchService, ICompanyService companyService)
        {
            _stockService = stockService;
            _productService = productService;
            _localizer = localizer;
            _stringLocalizer = stringLocalizer;
            _warehouseService = warehouseService;
            _userManager = userManager;
            _roleManager = roleManager;
            _employeeService = employeeService;
            _branchService = branchService;
            _companyService = companyService;
        }


        /// <summary>
        /// Eldeki tüm stokları listeleyen ana sayfayı döndürür.
        /// </summary>
        /// <returns>Stok listesini gösteren ana sayfa görünümü</returns>
        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical", Guid? companyId = null, int pageSize = 10)
        {
            int pageNumber = page ?? 1;

            var companiesResult = await _companyService.GetAllAsync();
            ViewBag.Companies = companiesResult.IsSuccess
                 ? companiesResult.Data.Adapt<List<CompanyDTO>>()
                 : new List<CompanyDTO>();

            ViewData["SelectedCompanyId"] = companyId;

            // Verileri al
            var stock = await _stockService.GetAllAsync(sortOrder);

            string companyName = string.Empty;
            if (companyId != null)
            {
                stock = await _stockService.GetStocksListByCompanyIdAsync(companyId.Value);

                var selectedCompany = companiesResult.Data.FirstOrDefault(c => c.Id == companyId.Value);
                if (selectedCompany != null) { companyName = selectedCompany.Name; } 
            }

            // companyId var mı yok mu kontrolü ile bildirimler ayrı yazılır
            if (companyId != null)
            {
                if (!stock.IsSuccess || stock.Data == null || !stock.Data.Any())
                {
                    NotifyError($"{companyName} {_stringLocalizer[Messages.STOCK_COMPANY_NOT_FOUND]}");
                    return View(Enumerable.Empty<StockListVM>().ToPagedList(pageNumber, pageSize));
                }
                NotifySuccess($"{companyName} {_stringLocalizer[Messages.STOCK_COMPANY_FOUND]}");
            }
            else
            {
                if (!stock.IsSuccess || stock.Data == null || !stock.Data.Any())
                {
                    NotifyError(_stringLocalizer[Messages.STOCK_LIST_EMPTY]);
                    return View(Enumerable.Empty<StockListVM>().ToPagedList(pageNumber, pageSize));
                }
            }

            var stockListVM = stock.Data.Adapt<List<StockListVM>>();

            // Giriş yapan kişinin rolü Manager ise if bloğu içerisinde ilgili stok işlemleri:
            var user = await _userManager.GetUserAsync(User);
            if (user != null && await _userManager.IsInRoleAsync(user, "Manager"))
            {
                var stockListDTOs = await _stockService.GetStockListForManagerAsync(user.Id);
                stockListVM = stockListDTOs.Adapt<List<StockListVM>>();
            }

            // Aynı ürünleri ve depoları birleştirme işlemi
            var mergedStockListVM = stockListVM
                .GroupBy(s => new { s.ProductName, s.WarehouseName })
                .Select(g => new StockListVM
                {
                    Id = g.OrderByDescending(s => s.ModifiedDate ?? s.CreatedDate).First().Id,
                    ProductName = g.Key.ProductName,
                    WarehouseName = g.Key.WarehouseName,
                    Count = g.Sum(s => s.Count),  // Toplam miktar
                    CreatedDate = g.Min(s => s.CreatedDate),
                    ModifiedDate = g.Max(s => s.ModifiedDate)
                })
                .ToList();

            // Sıralama işlemi
            switch (sortOrder)
            {
                case "alphabetical":
                    mergedStockListVM = mergedStockListVM.OrderBy(s => s.ProductName).ToList(); // A-Z
                    break;
                case "alphabeticaldesc":
                    mergedStockListVM = mergedStockListVM.OrderByDescending(s => s.ProductName).ToList(); // Z-A
                    break;
                case "date": // En yeni önce
                    mergedStockListVM = mergedStockListVM
                        .OrderByDescending(s => s.ModifiedDate ?? s.CreatedDate)
                        .ToList();
                    break;
                case "datedesc": // En eski önce
                    mergedStockListVM = mergedStockListVM
                        .OrderBy(s => s.ModifiedDate ?? s.CreatedDate)
                        .ToList();
                    break;
                default:
                    mergedStockListVM = mergedStockListVM.OrderBy(s => s.ProductName).ToList(); // Varsayılan sıralama
                    break;
            }

            // Sayfalama işlemi
            var paginatedList = mergedStockListVM.ToPagedList(pageNumber, pageSize);
            // ViewData üzerinden sıralama bilgisi
            ViewData["CurrentSortOrder"] = sortOrder;  // Sıralama bilgisi
            ViewData["CurrentPageSize"] = pageSize;    // Sayfa boyutu

            return View(paginatedList);
        }

        /// <summary>
        /// Yeni bir stok girme sayfasını döndürür.
        /// </summary>
        /// <returns>Yeni bir stok girme sayfası</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Employee"))
            {
                return View("~/Views/Shared/AccessDenied.cshtml");
            }
            List<ProductDTO> products = new();

            // Eğer kullanıcı Manager rolündeyse, kendi şirketine ait ürünleri getir
            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    NotifyError("Kullanıcı bulunamadı.");
                    return RedirectToAction("Index");
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var companyIdResult = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                if (!companyIdResult.IsSuccess || companyIdResult.Data == null)
                {
                    NotifyError(companyIdResult.Message);
                    return RedirectToAction("Index");
                }

                var productResult = await _productService.GetProductsByCompanyIdAsync(companyIdResult.Data.Value);
                if (productResult.IsSuccess && productResult.Data != null)
                {
                    products = productResult.Data
                        .Select(p => new ProductDTO
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            CategoryId = p.CategoryId
                            // Diğer alanları ekleyin
                        }).ToList();
                }
                else
                {
                    NotifyError(productResult.Message);
                }
            }
            else
            {
                // Eğer kullanıcı Manager değilse, tüm ürünleri getir
                var productResult = await _productService.GetAllAsync();
                if (productResult.IsSuccess && productResult.Data != null)
                {
                    products = productResult.Data
                        .Select(p => new ProductDTO
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            CategoryId = p.CategoryId
                            // Diğer alanları ekleyin
                        }).ToList();
                }
                else
                {
                    NotifyError(productResult.Message);
                }
            }

            // Depoları al
            var warehouseResult = await _warehouseService.GetAllAsync();
            var warehouses = warehouseResult.Data?.Adapt<List<WarehouseListVM>>() ?? new List<WarehouseListVM>();

			if (User.IsInRole("Manager"))
			{
				var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
				if (!string.IsNullOrEmpty(userIdClaim))
				{
					var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userIdClaim);
					if (!string.IsNullOrEmpty(companyId))
					{
						var branchlist = await _branchService.GetBranchesByCompanyIdAsync(Guid.Parse(companyId));
						var warehouseList = await _warehouseService.GetWarehousesByBranchIdAsync(branchlist);
						warehouses = warehouseList.Adapt<List<WarehouseListVM>>() ?? new List<WarehouseListVM>();

					}
				}
			}

			// ViewModel'i doldur
			var stockCreateVM = new StockCreateVM
            {
                Products = products,
                Warehouses = warehouses
            };

            return View(stockCreateVM);
        }





        /// <summary>
        /// Yeni bir stok girer ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="stockCreateVM">Girilen stok verileri</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockCreateVM stockCreateVM)
        {
            if (User.IsInRole("Employee"))
            {
                return View("~/Views/Shared/AccessDenied.cshtml");
            }
            if (!ModelState.IsValid)
            {
                // Depoları tekrar yükle
                var warehouseResult = await _warehouseService.GetAllAsync();
                stockCreateVM.Warehouses = warehouseResult.Data?.Adapt<List<WarehouseListVM>>() ?? new List<WarehouseListVM>();

                // Ürünleri tekrar yükle
                var productResult = await _productService.GetAllAsync();
                stockCreateVM.Products = productResult.Data
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        CategoryId = p.CategoryId
                        // Diğer alanları ekleyin
                    }).ToList();

                return View(stockCreateVM);
            }

            var stockCreateDTO = new StockCreateDTO
            {
                ProductId = stockCreateVM.ProductId,
                WarehouseId = stockCreateVM.WarehouseId,
                Count = (int)stockCreateVM.Count
                // Diğer alanları ekleyin
            };

            var result = await _stockService.AddAsync(stockCreateDTO);

            if (!result.IsSuccess)
            {
                NotifyError(result.Message);
                return View(stockCreateVM);
            }

            NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Girilen stok detaylarını gösterir.
        /// </summary>
        /// <param name="stockId">Girilen stok ID'si</param>
        /// <returns>Stok detaylarının görüntülendiği sayfa</returns>
        public async Task<IActionResult> Details(Guid stockId)
        {
            
            var stock = await _stockService.GetByIdAsync(stockId);

            if (!stock.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_NOT_FOUND]);
                //NotifyError(_localizer[stock.Message].Value);
                return RedirectToAction("Index");
            }

            var stockDetailsVM = stock.Data.Adapt<StockDetailVM>();
            NotifySuccess(_stringLocalizer[Messages.STOCK_FOUND_SUCCESS]);
            // NotifySuccess(stock.Message);

            return View(stockDetailsVM);
        }

        /// <summary>
        /// Belirtilen stok güncelleme sayfasını gösterir.
        /// </summary>
        /// <param name="stockId">Güncellenecek stok ID'si</param>
        /// <returns>Stok güncelleme sayfası</returns>
        public async Task<IActionResult> Update(Guid stockId)
        {
            if (User.IsInRole("Employee"))
            {
                return View("~/Views/Shared/AccessDenied.cshtml");
            }
            var stockResult = await _stockService.GetByIdAsync(stockId);
            if (!stockResult.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_UPDATE_FAILED]);
                return RedirectToAction("Index");
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.STOCK_UPDATE_SUCCESS]);
                // NotifySuccess(_localizer[stockResult.Message].Value);
            }

            var stock = stockResult.Data;

            var productResult = await _productService.GetAllAsync();
            var products = productResult.Data?.Adapt<List<ProductDTO>>() ?? new List<ProductDTO>();

            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;

                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                    var productList = await _productService.GetProductsByCompanyIdAsync(Guid.Parse(companyId));

                    products = productList.Data?.Adapt<List<ProductDTO>>() ?? new List<ProductDTO>();
                }
            }

            var stockListResult = await _stockService.GetAllAsync();
            var stocks = stockListResult.Data?.Adapt<List<StockDTO>>() ?? new List<StockDTO>();

            var productsWithoutStock = products.Where(p => !stocks.Any(s => s.ProductId == p.Id && s.Id != stockId)).ToList();

            // Depoları al
            var warehouseResult = await _warehouseService.GetAllAsync();
            var warehouses = warehouseResult.Data?.Adapt<List<WarehouseListVM>>() ?? new List<WarehouseListVM>();

            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userIdClaim);
                    if (!string.IsNullOrEmpty(companyId))
                    {
                        var branchlist = await _branchService.GetBranchesByCompanyIdAsync(Guid.Parse(companyId));
                        var warehouseList = await _warehouseService.GetWarehousesByBranchIdAsync(branchlist);
                        warehouses = warehouseList.Adapt<List<WarehouseListVM>>() ?? new List<WarehouseListVM>();

                    }
                }
            }

            var stockUpdateVM = stock.Adapt<StockUpdateVM>();
            stockUpdateVM.Products = products;
            stockUpdateVM.Warehouses = warehouses; // Depoları ekle
            stockUpdateVM.WarehouseId = stock.WarehouseId;
            stockUpdateVM.ProductId = stock.ProductId;

            return View(stockUpdateVM);
        }


        /// <summary>
        ///  Stok bilgilerini günceller.
        /// </summary>
        /// <param name="stockUpdateVM"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(StockUpdateVM stockUpdateVM)
        {
            if (User.IsInRole("Employee"))
            {
                return View("~/Views/Shared/AccessDenied.cshtml");
            }
            var productResult = await _productService.GetAllAsync();
            stockUpdateVM.Products = productResult.Data?.Adapt<List<ProductDTO>>() ?? new List<ProductDTO>();
            if (!ModelState.IsValid)
            {
                return View(stockUpdateVM);
            }

            var stock = await _stockService.UpdateAsync(stockUpdateVM.Adapt<StockUpdateDTO>());

            if (!stock.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_UPDATE_FAILED]);
                //NotifyError(_localizer[stock.Message].Value);
                return View(stockUpdateVM);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.STOCK_UPDATE_SUCCESS]);
                //NotifySuccess(_localizer[stock.Message].Value);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen ID'li stoğu siler ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="stockId">Silinecek stok ID'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid stockId)
        {
            if (User.IsInRole("Employee"))
            {
                return View("~/Views/Shared/AccessDenied.cshtml");
            }
            var stock = await _stockService.DeleteAsync(stockId);

            if (!stock.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_DELETE_FAILED]);
                //NotifyError(_localizer[stock.Message].Value);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.STOCK_DELETED_SUCCESS]);
                //NotifySuccess(_localizer[stock.Message].Value);
            }
            return RedirectToAction("Index");

        }
    }
}
