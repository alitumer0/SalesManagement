using BaSalesManagementApp.Business.Interfaces;
using BaSalesManagementApp.Business.Services;
using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.DataAccess.Interfaces.Repositories;
using BaSalesManagementApp.Dtos.OrderDTOs;
using BaSalesManagementApp.Dtos.PaymentTypeDTOs;
using BaSalesManagementApp.Dtos.ReportingDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models;
using BaSalesManagementApp.MVC.Models.AppUserVMs;
using BaSalesManagementApp.MVC.Models.LoginVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Security.Claims;

namespace BaSalesManagementApp.MVC.Controllers
{

    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAdminRepository _adminRepository;
        private readonly IAccountService _accountService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IPaymentTypeService _paymentTypeService;


        public HomeController(
            ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IAdminRepository adminRepository,
            IAccountService accountService,
            IStringLocalizer<Resource> stringLocalizer,
            IOrderService orderService,
            IProductService productService,
            IPaymentTypeService paymentTypeService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _adminRepository = adminRepository;
            _accountService = accountService;
            _stringLocalizer = stringLocalizer;
            _orderService = orderService;
            _productService = productService;
            _paymentTypeService = paymentTypeService;
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var appUser = await _accountService.FindByEmailAsync(vm.Email);


            if (appUser is null)
            {
                NotifyError(_stringLocalizer[Messages.ACCOUNT_NOT_FOUND]);
                return View(vm);
            }

            // Kullanıcı ID'sini claim'lere ekle sonra order oluştururken kullanmak için
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                new Claim(ClaimTypes.Name, appUser.UserName),
                new Claim(ClaimTypes.Email, appUser.Email),


            };

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _accountService.SignInAsync(appUser, vm.Password, false);
            if (!signInResult.Succeeded)
            {
                NotifyError(_stringLocalizer[Messages.ACCOUNT_NOT_FOUND]);
                return View(vm);
            }

            var roles = await _accountService.GetRolesAsync(appUser);
            if (roles is null)
            {
                NotifyError(_stringLocalizer[Messages.ACCOUNT_ROLE_NOT_FOUND_FOR_USER]);
                //NotifyError(_localizer[Messages.ACCOUNT_ROLE_NOT_FOUND_FOR_USER]);
                return View(vm);
            }

            if (appUser.EmailConfirmed == false)
            {
                // Bir token üreten bölüm
                var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                // token ile birlikte Accountcontroller'ın ChangePassword Action'una giden bölüm
                return RedirectToAction("ChangePassword", "Account", new { token });
            }

            return RedirectToAction("Index", "Home");

        }


        [HttpGet]
        public async Task<IActionResult> GetTotalPayments(int? year)
        {
            int selectedYear = year ?? DateTime.Now.Year;

            // Siparişleri servisten al
            var ordersResult = await _orderService.GetAllAsync("date");

            if (!ordersResult.IsSuccess || ordersResult.Data == null)
            {
                return Json(new { totalPayments = "0.00" });
            }

            // Filtrele ve toplamı hesapla
            var filteredOrders = ordersResult.Data
                .Where(o => o.OrderDate.Year == selectedYear)
                .ToList();

            var totalPayments = filteredOrders.Sum(o => o.TotalPrice);

            return Json(new { totalPayments = totalPayments.ToString("N2") });
        }


        public async Task<IActionResult> LogOut()
        {
            await _accountService.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesCardData(int year)
        {
            try
            {
                if (year <= 0) year = DateTime.Now.Year;

                // SIRAYLA ÇEK (aynı DbContext üzerinde tek seferde)
                var sales = await _orderService.GetSalesMetricAsync(year, null);
                var profit = await _orderService.GetProfitMetricAsync(year, null, true);




                // Siparişleri servisten al
                var ordersResult = await _orderService.GetAllAsync("date");

                if (!ordersResult.IsSuccess || ordersResult.Data == null)
                {
                    return Json(new { totalPayments = "0.00" });
                }

                // Filtrele ve toplamı hesapla
                var filteredOrders = ordersResult.Data
                    .Where(o => o.OrderDate.Year == year)
                    .ToList();

                var totalPayments = filteredOrders.Sum(o => o.TotalPrice);

                return Json(new

                {
                    // SALES kartı
                    salesTotal = Math.Round(sales.CurrentTL, 2),
                    salesPrevious = Math.Round(sales.PreviousTL, 2),
                    salesChangePct = Math.Round(sales.ChangePct, 2),
                    salesIsUp = sales.ChangePct >= 0m,
                    totalPayments = totalPayments.ToString("N2"),

                    // Sağdaki toplam (eski "totalBalance") — KÂR
                    totalBalance = Math.Round(profit.CurrentTL, 2), 
                //Yıla göre bakiye hesaplaması.
                //ViewBag.TotalBalance = await _orderService.GetTotalProfitAsync(selectedYear);

                    // PROFIT kartı
                    profitTotal = Math.Round(profit.CurrentTL, 2),
                    profitPrevious = Math.Round(profit.PreviousTL, 2),
                    profitChangePct = Math.Round(profit.ChangePct, 2),
                    profitIsUp = profit.ChangePct >= 0m
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSalesCardData failed for year {Year}", year);
                Response.StatusCode = 500;
                return Json(new { error = ex.GetBaseException().Message });
            }
        }
        // Ödemeler: Seçilen yıla ait toplam tahsilat
        //ViewBag.TotalPayments = filteredOrders?.Sum(o => o.TotalPrice) ?? 0m;

        //Kullanıcı Adını ve soyadını al 

        [HttpGet]
        public async Task<IActionResult> GetHomeInit(int? year = null)
        {
            var selectedYear = year ?? DateTime.Now.Year;
            var currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(currentYear - 4, 5).OrderByDescending(y => y).ToList();

            // Kullanıcı bilgisi
            var user = await _accountService.GetProfileAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userName = user != null ? $"{user.FirstName} {user.LastName}" : "";
            var greeting = _stringLocalizer["Congratulations", userName].Value;

            // Gelir/Kâr/Gider
            var sales = await _orderService.GetSalesMetricAsync(selectedYear, null);
            var profit = await _orderService.GetProfitMetricAsync(selectedYear, null, true);
            var expense = await _orderService.GetExpenseMetricAsync(selectedYear, null, true);

            // Tüm siparişler
            var ordersResult = await _orderService.GetAllAsync("date");
            var filteredOrders = ordersResult.IsSuccess && ordersResult.Data != null
                ? ordersResult.Data
                    .Where(o => o.OrderDate != null && o.OrderDate.Year == selectedYear) // null-safe
                    .ToList()
                : new List<OrderListDTO>();

            var orderCount = filteredOrders.Count;

            // PaymentType listesi (DTO ile)
            var paymentTypesResult = await _paymentTypeService.GetAllAsync();
            var paymentTypes = paymentTypesResult.IsSuccess && paymentTypesResult.Data != null
                ? paymentTypesResult.Data
                : new List<PaymentTypeListDTO>();

            // Ödeme tiplerine göre gruplama
            var paymentStats = filteredOrders
                .Where(o => o.PaymentTypeId.HasValue)
                .GroupBy(o => paymentTypes.FirstOrDefault(p => p.Id == o.PaymentTypeId)?.Name ?? "Bilinmiyor")
                .Select(g => new
                {
                    PaymentTypeName = g.Key,
                    UsageCount = g.Count(),
                    TotalAmount = g.Sum(o => o.TotalPrice)
                })
                .ToList();

            // JSON ile döndür
            return Json(new
            {
                selectedYear,
                years,
                userName,
                greeting,
                income = Math.Round(sales.CurrentTL, 2),
                profit = Math.Round(profit.CurrentTL, 2),
                expense = Math.Round(expense.CurrentTL, 2),
                transactionCount = orderCount,
                paymentStats
            });
        }

        //Language
        public IActionResult ChangeLanguage(string culture)
        {
            var cookieValue = $"c={culture}|uic={culture}";
            Response.Cookies.Append(".AspNetCore.Culture", cookieValue, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                Path = "/"
            });
            return Redirect(Request.Headers["Referer"].ToString());

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
