namespace BaSalesManagementApp.DataAccess.Extentions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddDbContext<BaSalesManagementAppDbContext>(option =>
            {
                option.UseLazyLoadingProxies();
                option.UseSqlServer(configuration.GetConnectionString(BaSalesManagementAppDbContext.ConnectionName));
                
            });
            services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<BaSalesManagementAppDbContext>().AddDefaultTokenProviders();


            return services;
        }
    }
}
