using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.CurrencyVMs;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class CurrencyController : BaseController
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStringLocalizer<Resource> _localizer;

        public CurrencyController(ICurrencyService currencyService, IStringLocalizer<Resource> localizer)
        {
            _currencyService = currencyService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index(int? page, string sortOrder = "date_desc", int pageSize = 10, string searchQuery = null)
        {
            try
            {
                int pageNumber = page ?? 1;
                ViewBag.CurrentSort = sortOrder;
                ViewBag.CurrentPageSize = pageSize;

                var result = await _currencyService.GetAllCurrentExchangeRatesAsync();

                if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                {
                    NotifyError(_localizer[Messages.CURRENCY_LIST_ERROR]);
                    return View(Enumerable.Empty<CurrencyListVM>().ToPagedList(pageNumber, pageSize));
                }

                // DTO - View Model dönüşümü
                var currencyListVM = result.Data.Adapt<List<CurrencyListVM>>();

                // Sıralama işlemi
                currencyListVM = sortOrder switch
                {
                    "date_asc" => currencyListVM.OrderBy(x => x.CreatedDate).ToList(),
                    _ => currencyListVM.OrderByDescending(x => x.CreatedDate).ToList()
                };

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    currencyListVM = currencyListVM
                        .Where(x => x.CreatedDate.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                ViewData["CurrentSearchQuery"] = searchQuery;

                ViewBag.NewSortOrder = sortOrder == "date_asc" ? "date_desc" : "date_asc";

                return View(currencyListVM.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                NotifyError($"{_localizer[Messages.CURRENCY_LIST_ERROR]}: {ex.Message}");
                return View("Error");
            }
        }
    }
}