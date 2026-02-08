using BaSalesManagementApp.Business;
using BaSalesManagementApp.Business.Services;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.MVC.Mapster;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


//mapster ayarlar�n� y�kledim
Mapsterconfig.Mapping();


// Add services to the container.
builder.Services.AddControllersWithViews();

//Authorize olmadan yap�lacak giri�lerdeki y�nlendirmeler
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/LogOut";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddDataAccessEfCoreServices();
builder.Services.AddBusinessService();
builder.Services.AddBackgroundJobs(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddMVCServices();

builder.Services.Configure<EMailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));

builder.Services.Configure<RecaptchaSettings>(builder.Configuration.GetSection("Recaptcha"));

// Hangfire configuration
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnectionString"));
});
builder.Services.AddHangfireServer();

// Configure localization services for language
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("tr-TR")
    };
    options.DefaultRequestCulture = new RequestCulture("tr-TR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});



var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler("/Home/Error");

app.UseNotyf();

// Hangfire Dashboard
app.UseHangfireDashboardWithPath("/hangfire");

//app.UseHttpsRedirection();

app.UseStaticFiles(); app.UseRequestLocalization(); // Apply localization middleware

app.UseRouting();
app.Use(async (context, next) =>
{
    await next.Invoke();
    if (context.Request.Path == "/Account/Login")
    {
        //Proje istemsiz bir �ekilde yetkisiz giri�lerde Account/Login'e y�nlendirdi�i i�in burada Home/Logine y�nlendirme yapt�k
        context.Response.Redirect("/Home/Login");
    }
    if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
        context.Response.Redirect("/Account/AccessDenied");
    }
    else if (context.Response.StatusCode == StatusCodes.Status404NotFound)
    {
        //Ge�ersiz bir url girildi�inde Home/Logine y�nlendirsin bizi istedik
        context.Response.Redirect("/Home/Login");
    }
});
app.UseAuthentication();
app.UseAuthorization();
//Language
app.UseRequestLocalizationService();
app.MapControllerRoute
       (
           name: "default",
           pattern: "{controller=Home}/{action=Login}/{id?}"
       );

app.MapDefaultControllerRoute();

app.Run();
