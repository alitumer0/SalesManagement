using BaSalesManagementApp.MVC.Models.AdminVMs;
using BaSalesManagementApp.MVC.Models.BranchVMs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.CityVMs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.CountryVMs;
using BaSalesManagementApp.MVC.Models.CustomerVMs;
using BaSalesManagementApp.MVC.Models.EmployeeVMs;
using BaSalesManagementApp.MVC.Models.OrderVMs;
using BaSalesManagementApp.MVC.Models.ProductTypeVMs;
using BaSalesManagementApp.MVC.Models.ProductVMs;
using BaSalesManagementApp.MVC.Models.PromotionVMs;
using BaSalesManagementApp.MVC.Models.SearchVM;
using BaSalesManagementApp.MVC.Models.StockTypeSizeVMs;
using BaSalesManagementApp.MVC.Models.StockVMs;
using BaSalesManagementApp.MVC.Models.StudentVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.AspNetCore.Mvc;

namespace BaSalesManagementApp.MVC.Controllers
{
	public class SearchController : BaseController
	{
		private readonly IAdminService _adminService;
        private readonly IBranchService _branchService;
        private readonly ICategoryService _categoryService;
		private readonly ICityService _cityService;
		private readonly ICompanyService _companyService;
		private readonly ICountryService _countryService;
		private readonly ICustomerService _customerService;
		private readonly IEmployeeService _employeeService;
		private readonly IOrderService _orderService;
		private readonly IProductService _productService;
		private readonly IStockTypeService _stockTypeService;
		private readonly IPromotionService _promotionService;
		private readonly IStockService _stockService;
		private readonly IWarehouseService _warehouseService;
		private readonly IStockTypeSizeService _stockTypeSizeService;

        public SearchController(IAdminService adminService, IBranchService branchService, ICategoryService categoryService, ICityService cityService, ICompanyService companyService, ICountryService countryService, ICustomerService customerService, IEmployeeService employeeService, IOrderService orderService, IProductService productService, IStockTypeService stockTypeService, IPromotionService promotionService, IStockService stockService, IWarehouseService warehouseService,IStockTypeSizeService stockTypeSizeService)
		{
            _stockTypeSizeService = stockTypeSizeService;
            _adminService = adminService;
			_branchService = branchService;
			_categoryService = categoryService;
			_cityService = cityService;
			_companyService = companyService;
			_countryService = countryService;
			_customerService = customerService;
			_employeeService = employeeService;
			_orderService = orderService;
			_productService = productService;
			_stockTypeService = stockTypeService;
			_promotionService = promotionService;
			_stockService = stockService;
			_warehouseService = warehouseService;
		}

		public IActionResult Index(string searchQuery, string sortOrder = "alphabetical")
		{

			SearchVM searchVM = new SearchVM();

			// Filtrelenmiş verileri alalım
			searchVM.AdminListVM = _adminService.GetAllAsync(sortOrder, searchQuery).Result.Data.Adapt<List<AdminListVM>>();
			searchVM.BranchListVM = _branchService.GetAllAsync(searchQuery).Result.Data.Adapt<List<BranchListVM>>();
			searchVM.CategoryListVM = _categoryService.GetAllAsync(sortOrder, searchQuery).Result.Data.Adapt<List<CategoryListVM>>();
			searchVM.CityListVM = _cityService.GetAllAsync(sortOrder, searchQuery).Result.Data.Adapt<List<CityListVM>>();
			searchVM.CompanyListVM = _companyService.GetAllAsync(searchQuery).Result.Data.Adapt<List<CompanyListVM>>();
			searchVM.CountryListVM = _countryService.GetAllAsync(searchQuery).Result.Data.Adapt<List<CountryListVM>>();
			searchVM.CustomerListVM = _customerService.GetAllAsync(sortOrder, searchQuery).Result.Data.Adapt<List<CustomerListVM>>();
			searchVM.EmployeeListVM = _employeeService.GetAllAsync(sortOrder,searchQuery).Result.Data.Adapt<List<EmployeeListVM>>();
			searchVM.OrderListVM = _orderService.GetAllAsync(sortOrder, searchQuery).Result.Data.Adapt<List<OrderListVM>>();
			searchVM.ProductListVM = _productService.GetAllAsync(sortOrder,searchQuery).Result.Data.Adapt<List<ProductListVM>>();
			searchVM.StockTypeSizeListVM = _stockTypeSizeService.GetAllAsync(searchQuery).Result.Data.Adapt<List<StockTypeSizeListVM>>();
			searchVM.productTypeListVM = _stockTypeService.GetAllAsync(sortOrder,searchQuery).Result.Data.Adapt<List<ProductTypeListVM>>();
            searchVM.PromotionListVM = _promotionService.GetAllAsync(sortOrder, searchQuery).Result.Data.Adapt<List<PromotionListVM>>();
			searchVM.StockListVM = _stockService.GetAllAsync(sortOrder, searchQuery).Result.Data.Adapt<List<StockListVM>>();
			searchVM.WarehouseListVM = _warehouseService.GetAllAsync(sortOrder, searchQuery).Result.Data.Adapt<List<WarehouseListVM>>();

			// Filtrelenmiş verileri view'e gönderelim
			return View(searchVM);
		}
	}
}
