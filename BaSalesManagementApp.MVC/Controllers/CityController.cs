using BaSalesManagementApp.Dtos.CityDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using BaSalesManagementApp.MVC.Models.CityVMs;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using System;
using BaSalesManagementApp.Business.Utilities;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class CityController : BaseController
    {
        private readonly ICityService _cityService;
        private readonly ICountryService _countryService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public CityController(ICityService cityService, ICountryService countryService, IStringLocalizer<Resource> stringLocalizer)
        {
            _cityService = cityService;
            _countryService = countryService;
            _stringLocalizer = stringLocalizer;
        }

        //4
        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical", int pageSize = 10,
     string? searchQuery = null)
        {
            try
            {
                int pageNumber = page ?? 1;
                var term = searchQuery?.Trim();

                var result = await _cityService.GetAllAsync(sortOrder, term);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CITY_LISTED_ERROR]);
                    return View(Enumerable.Empty<CityListVM>().ToPagedList(pageNumber, pageSize));
                }

                var cityListVMs = result.Data.Adapt<List<CityListVM>>();

                var firstThree = string.Join(", ", cityListVMs.Take(3).Select(x => x.Name));
                //Console.WriteLine($"DEBUG - ilk 3: {firstThree}");

                var totalCount = cityListVMs.Count;
                var pagedData = cityListVMs.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                var paginatedList = new StaticPagedList<CityListVM>(pagedData, pageNumber, pageSize, totalCount);

                var pagedNames = string.Join(", ", pagedData.Select(x => x.Name));
                //Console.WriteLine($"DEBUG - anual pag sonrası: {pagedNames}");

                ViewData["CurrentSortOrder"] = sortOrder;
                ViewData["CurrentPage"] = pageNumber;
                ViewData["CurrentPageSize"] = pageSize;
                ViewData["CurrentSearchQuery"] = term ?? string.Empty;
                ViewBag.CurrentSearch = term ?? string.Empty;
                ViewBag.CurrentSort = sortOrder;

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                var errorMessage = _stringLocalizer[Messages.CITY_LISTED_ERROR] + ": " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(Guid cityId)
        {
            try
            {
                var result = await _cityService.GetByIdAsync(cityId);

                if (!result.IsSuccess)
                {

                    NotifyError(_stringLocalizer[Messages.CITY_NOT_FOUND]);
                    //NotifyError(result.Message);
                    return RedirectToAction("Index");
                }
                NotifySuccess(_stringLocalizer[Messages.CITY_GET_SUCCESS]);
                // NotifySuccess(result.Message);

                var cityDetailsVM = result.Data.Adapt<CityDetailsVM>();

                return View(cityDetailsVM);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var countriesResult = await _countryService.GetAllAsync();
                var countries = countriesResult.Data?.Adapt<List<CountryDTO>>() ?? new List<CountryDTO>();

                var cityCreateVM = new CityCreateVM()
                {
                    Countries = countries,
                };
                return View(cityCreateVM);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CityCreateVM cityCreateVM)
        {
            try
            {
                cityCreateVM.Name = StringUtilities.CapitalizeEachWord(cityCreateVM.Name);

                var countriesResult = await _countryService.GetAllAsync();
                cityCreateVM.Countries = countriesResult.Data?.Adapt<List<CountryDTO>>() ?? new List<CountryDTO>();

                if (!ModelState.IsValid)
                {
                    return View(cityCreateVM);
                }

                var result = await _cityService.AddAsync(cityCreateVM.Adapt<CityCreateDTO>());

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CITY_CREATED_ERROR]);
                    // NotifyError(result.Message);
                    return View(cityCreateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.CITY_CREATED_SUCCESS]);
                // NotifySuccess(result.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.CITY_CREATED_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Update(Guid cityId)
        {
            try
            {

                var result = await _cityService.GetByIdAsync(cityId);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CITY_NOT_FOUND]);
                    return RedirectToAction("Index");
                }


                var countriesResult = await _countryService.GetAllAsync();
                var countries = countriesResult.Data?.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    //Text = c.Name,
                    Text = CultureInfo.CurrentCulture.Name == "tr-TR" ? c.NameTr : c.NameEn,
                    Selected = c.Id == result.Data.CountryId 
                }).ToList();

                
                var cityUpdateVM = result.Data.Adapt<CityUpdateVM>();
                cityUpdateVM.Countries = countries;

                return View(cityUpdateVM);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.CITY_UPDATED_ERROR]);
                return View("Error");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CityUpdateVM cityUpdateVM)
        {
            try
            {

                cityUpdateVM.Name = StringUtilities.CapitalizeEachWord(cityUpdateVM.Name);

                var countriesResult = await _countryService.GetAllAsync();

                cityUpdateVM.Countries = countriesResult.Data?.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    //Text = c.Name,
                    Text = CultureInfo.CurrentCulture.Name == "tr-TR" ? c.NameTr : c.NameEn,
                    Selected = c.Id == cityUpdateVM.CountryId
                }).ToList();

                if (!ModelState.IsValid)
                {
                    return View(cityUpdateVM);
                }

                var result = await _cityService.UpdateAsync(cityUpdateVM.Adapt<CityUpdateDTO>());

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CITY_UPDATED_ERROR]);
                    //NotifyError(result.Message);
                    return View(cityUpdateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.CITY_UPDATED_SUCCESS]);
                // NotifySuccess(result.Message);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.CITY_UPDATED_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(Guid cityId)
        {
            try
            {
                var result = await _cityService.DeleteAsync(cityId);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CITY_DELETED_ERROR]);
                    //NotifyError(result.Message);
                }

                NotifySuccess(_stringLocalizer[Messages.CITY_DELETED_SUCCESS]);

                // NotifySuccess(result.Message);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.CITY_DELETED_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }
    }
}

