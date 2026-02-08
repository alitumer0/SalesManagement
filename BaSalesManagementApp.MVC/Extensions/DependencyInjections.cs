using Elfie.Serialization;

namespace BaSalesManagementApp.MVC.Extentions
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddMVCServices(this IServiceCollection services)
        {
            services.AddControllersWithViews().AddMvcLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix,
                options => options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var assemblyName = new AssemblyName(typeof(Resource).GetTypeInfo().Assembly.FullName);
                    return factory.Create(nameof(Resource), assemblyName.Name);

                });

            // Localization servisini ekleme
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // HTTP Context erişimi için
            services.AddHttpContextAccessor();

            // UTF-8 kodlaması ve kültürel ayarlar ekleme
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("tr-TR")
                };

                options.DefaultRequestCulture = new RequestCulture("tr-TR");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.ApplyCurrentCultureToResponseHeaders = true;
            });

            return services;
        }

        public static IApplicationBuilder UseRequestLocalizationService(this IApplicationBuilder app)
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("tr-TR")
            };

            var localizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                DefaultRequestCulture = new RequestCulture("tr-TR"),
                ApplyCurrentCultureToResponseHeaders = true,
            };

            // Correctly configure the RequestCultureProviders
            localizationOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
            app.UseRequestLocalization(localizationOptions);

            return app;
        }
    }
}
