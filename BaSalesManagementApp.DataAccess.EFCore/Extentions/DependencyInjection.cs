
namespace BaSalesManagementApp.DataAccess.EFCore.Extentions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessEfCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IAdminRepository, AdminRespository>();
            services.AddScoped<IBadgeRepository, BadgeRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();            
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICurrentExchangeRateRepository, CurrentExchangeRateRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();            
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IStockTypeRepository, StockTypeRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();            
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IStockTypeSizeRepository, StockTypeSizeRepository>();
            services.AddScoped<IPaymentTypeRepository,PaymentTypeRepository>();
            return services;
        }
    }
}