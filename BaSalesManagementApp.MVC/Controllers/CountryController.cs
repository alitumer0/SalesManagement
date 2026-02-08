using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.CountryVMs;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class CountryController : BaseController
    {
        private readonly ICountryService _countryService;
        private readonly IStringLocalizer<CountryController> stringLocalizers;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public CountryController(ICountryService countryService, IStringLocalizer<CountryController> stringLocalizers, IStringLocalizer<Resource> stringLocalizer)
        {
            _countryService = countryService;
            this.stringLocalizers = stringLocalizers;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<IActionResult> Index(int? page, int pageSize = 10, string sortOrder = "name_asc", string searchQuery="")
        {
            try
            {
                int pageNumber = page ?? 1;

                // Tüm ülkeleri getirir
                var result = await _countryService.GetAllAsync();
                var countryListVMs = result.Data.Adapt<List<CountryListVM>>();

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_LISTED_ERROR]);
                    return View(Enumerable.Empty<CountryListVM>().ToPagedList(pageNumber, pageSize));
                }

                // Dil kontrolü ve Name alanına atama
                var currentCulture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                foreach (var country in countryListVMs)
                {
                    country.Name = currentCulture switch
                    {
                        "tr" => country.NameTr,
                        "en" => country.NameEn,
                        _ => country.NameEn // Default İngilizce
                    };
                }

                // Arama işlemi
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    countryListVMs = countryListVMs
                        .Where(c =>
                            c.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                            c.CreatedDate.ToString("dd.MM.yyyy").Contains(searchQuery) ||   // 21.09.2025 gibi
                            c.CreatedDate.ToString("yyyy-MM-dd").Contains(searchQuery)      // 2025-09-21 gibi
                        )
                        .ToList();
                }

                // Sıralama işlemi
                countryListVMs = sortOrder switch
                {
                    "name_asc" => countryListVMs.OrderBy(c => c.Name).ToList(),
                    "name_desc" => countryListVMs.OrderByDescending(c => c.Name).ToList(),
                    "date_asc" => countryListVMs.OrderBy(c => c.CreatedDate).ToList(),
                    "date_desc" => countryListVMs.OrderByDescending(c => c.CreatedDate).ToList(),
                    _ => countryListVMs.OrderBy(c => c.Name).ToList()
                };


                // Sayfalamayı uygular ve ViewData'ya bilgileri ekler
                var paginatedList = countryListVMs.ToPagedList(pageNumber, pageSize);
                ViewData["CurrentSortOrder"] = sortOrder;
                ViewData["CurrentPage"] = pageNumber;
                ViewData["CurrentPageSize"] = pageSize;
                ViewData["CurrentFilter"] = searchQuery;

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                var errorMessage = "Ülkeleri getirirken bir hata meydana geldi: " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }


        public async Task<IActionResult> Details(Guid countryId)
        {
            try
            {
                var result = await _countryService.GetByIdAsync(countryId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_GETBYID_ERROR]);
                    return RedirectToAction("Index");
                }
                NotifySuccess(_stringLocalizer[Messages.COUNTRY_GETBYID_SUCCESS]);

                var countryDetailsVM = result.Data.Adapt<CountryDetailsVM>();

                // Örnek dil kontrolü: cookie veya kültürden çekebilirsin
                var culture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                countryDetailsVM.Name = (culture == "tr") ? result.Data.NameTr : result.Data.NameEn;

                return View(countryDetailsVM);

            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_GETBYID_ERROR]);
                return View("Error");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_CREATE_ERROR]);
                return View("Error");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryCreateVM countryCreateVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(countryCreateVM);
                }
           
                countryCreateVM.NameTr = StringUtilities.CapitalizeEachWord(countryCreateVM.NameTr);
                countryCreateVM.NameEn = StringUtilities.CapitalizeEachWord(countryCreateVM.NameEn);

                var result = await _countryService.AddAsync(countryCreateVM.Adapt<CountryCreateDTO>());

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_CREATE_ERROR]);
                    return View(countryCreateVM);

                }
                NotifySuccess(_stringLocalizer[Messages.COUNTRY_CREATE_SUCCESS]);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_CREATE_ERROR]);
                return View("Error");
            }
        }

        public async Task<IActionResult> Update(Guid countryId)
        {
            try
            {
                var result = await _countryService.GetByIdAsync(countryId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_UPDATE_ERROR]);
                    return RedirectToAction("Index");
                }

                NotifySuccess(_stringLocalizer[Messages.COUNTRY_UPDATE_SUCCESS]);

                var countryUpdateVM = result.Data.Adapt<CountryUpdateVM>();

                return View(countryUpdateVM);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_UPDATE_ERROR]);
                return View("Error");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CountryUpdateVM countryUpdateVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(countryUpdateVM);
                }
                 
                countryUpdateVM.NameTr = StringUtilities.CapitalizeEachWord(countryUpdateVM.NameTr);
                countryUpdateVM.NameEn = StringUtilities.CapitalizeEachWord(countryUpdateVM.NameEn);

                var result = await _countryService.UpdateAsync(countryUpdateVM.Adapt<CountryUpdateDTO>());

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_UPDATE_ERROR]);
                    return View(countryUpdateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.COUNTRY_UPDATE_SUCCESS]);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_UPDATE_ERROR]);
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(Guid countryId)
        {
            try
            {
                var result = await _countryService.DeleteAsync(countryId);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_DELETE_ERROR]);
                }

                NotifySuccess(_stringLocalizer[Messages.COUNTRY_DELETE_SUCCESS]);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_DELETE_ERROR]);
                return View("Error");
            }
        }
    }
}
