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
using BaSalesManagementApp.MVC.Models.StockTypeSizeVMs;
using BaSalesManagementApp.MVC.Models.StockVMs;
using BaSalesManagementApp.MVC.Models.StudentVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;

namespace BaSalesManagementApp.MVC.Models.SearchVM
{
	public class SearchVM
	{
		public List<AdminListVM> AdminListVM { get; set; }
		public List<BranchListVM> BranchListVM { get; set; }
		public List<CategoryListVM> CategoryListVM { get; set; }
		public List<CityListVM> CityListVM { get; set; }
		public List<CompanyListVM> CompanyListVM { get; set; }
		public List<CountryListVM> CountryListVM { get; set; }
		public List<CustomerListVM> CustomerListVM { get; set; }
		public List<EmployeeListVM> EmployeeListVM { get; set; }
		public List<OrderListVM> OrderListVM { get; set; }
		public List<ProductListVM> ProductListVM { get; set; }
		public List<StockTypeSizeListVM>StockTypeSizeListVM { get; set; }

		public List<ProductTypeListVM> productTypeListVM { get; set; }
        public List<PromotionListVM> PromotionListVM { get; set; }
		public List<StockListVM> StockListVM { get; set; }
		public List<StudentListVM> StudentListVM { get; set; }
		public List<WarehouseListVM> WarehouseListVM { get; set; }

	}
}
