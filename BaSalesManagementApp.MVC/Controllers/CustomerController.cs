using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.CityDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using BaSalesManagementApp.Dtos.CustomerDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.AdminVMs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.CustomerVMs;
using BaSalesManagementApp.MVC.Models.OrderVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly ICompanyService _companyService;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;
        private readonly IAdminService _adminService;
        private readonly IEmployeeService _employeeService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IStringLocalizer<Resource> _stringLocalizer;
        /// <summary>
        /// Müşteri kontrolcüsü
        /// </summary>
        /// <param name="customerService">Müşteri işlemleri servisi</param>
        /// <param name="companyService">Şirket işlemleri servisi</param>
        /// /// <param name="stringLocalizer">Çeviri hizmeti.</param>
        /// <param name="countryService">Ülke işlemleri servisi.</param>
        /// <param name="cityService">Şehir işlemleri servisi.</param>
        public CustomerController(ICustomerService customerService, ICompanyService companyService, IStringLocalizer<Resource> stringLocalizer, ICountryService countryService, ICityService cityService, IAdminService adminService, IEmployeeService employeeService, UserManager<IdentityUser> userManager)
        {
            _customerService = customerService;
            _companyService = companyService;
            _stringLocalizer = stringLocalizer;
            _countryService = countryService;
            _cityService = cityService;
            _adminService = adminService;
            _employeeService = employeeService;
            _userManager = userManager;
        }


        /// <summary>
        /// Müşterileri gösteren ana sayfa görünümü
        /// </summary>
        /// <returns>Tüm müşterileri listeleyen ana sayfa görünümünü döndürür.</returns>
        //// EK: Arama parametresi eklendi
        public async Task<IActionResult> Index(int? page,string sortOrder = "alphabetical", Guid? company_Id = null, int pageSize = 10, string? q = null )
        {
            try
            {
                int pageNumber = page ?? 1;

                var companiesResult = await _companyService.GetAllAsync();
                ViewBag.Companies = companiesResult.IsSuccess
                    ? companiesResult.Data.Adapt<List<CompanyDTO>>()
                    : new List<CompanyDTO>();
                ViewData["SelectedCompanyId"] = company_Id;

                // TR-duyarlı contains
                static bool TurkishContains(string? haystack, string? needle)
                {
                    if (string.IsNullOrWhiteSpace(needle)) return true;     // arama boşsa geç
                    if (string.IsNullOrEmpty(haystack)) return false;
                    var ci = new System.Globalization.CultureInfo("tr-TR");
                    return ci.CompareInfo.IndexOf(haystack, needle, System.Globalization.CompareOptions.IgnoreCase) >= 0;
                }

                var result = await _customerService.GetAllAsync(sortOrder);

                if (company_Id.HasValue)
                {
                    result = await _customerService.GetCustomersByCompanyId(company_Id.Value, sortOrder);
                    var companyResult = await _companyService.GetByIdAsync(company_Id.Value);
                    var companyName = string.IsNullOrWhiteSpace(companyResult?.Data?.Name)
                        ? _stringLocalizer["Unknown Company"]
                        : companyResult!.Data!.Name;

                    if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                    {
                        var errorMessage = string.Format(_stringLocalizer[Messages.CUSTOMER_NOTFOUND_FOR_COMPANY], companyName);
                        NotifyError(errorMessage);
                        // Arama boş değilse bile boş liste döndür
                        ViewBag.CurrentSort = sortOrder;
                        ViewBag.CurrentSearch = q; 
                        return View(Enumerable.Empty<CustomerListVM>().ToPagedList(pageNumber, pageSize));
                    }
                    var successMessage = string.Format(_stringLocalizer[Messages.CUSTOMER_LISTED_FOR_COMPANY], companyName);
                    NotifySuccess(successMessage);

                    // DTO -> VM
                    var customerListVMs = result.Data.Adapt<List<CustomerListVM>>() ?? new List<CustomerListVM>();

                    //Sunucu tarafı ARAMA (Ad/Soyad Name içinde ise yine tutar)
                    if (!string.IsNullOrWhiteSpace(q))
                        customerListVMs = customerListVMs
                            .Where(c => TurkishContains(c.Name, q))
                            .ToList();
                    ViewBag.CurrentSort = sortOrder;
                    ViewBag.CurrentSearch = q; 
                    return View(customerListVMs.ToPagedList(pageNumber, pageSize));
                }

                // Şirket filtresi yoksa tüm liste
                var customerListAll = result.Data?.Adapt<List<CustomerListVM>>() ?? new List<CustomerListVM>();

                if (User.IsInRole("Manager"))
                {
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userIdClaim))
                    {
                        var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userIdClaim);
                        if (!string.IsNullOrEmpty(companyId))
                        {
                            var resByCompany = await _customerService.GetCustomersByCompanyId(Guid.Parse(companyId), sortOrder);
                            if (!resByCompany.IsSuccess)
                            {
                                NotifyError(_stringLocalizer[Messages.CUSTOMER_LIST_FAILED]);
                                ViewBag.CurrentSort = sortOrder;
                                ViewBag.CurrentSearch = q; 
                                return View(Enumerable.Empty<CustomerListVM>().ToPagedList(pageNumber, pageSize));
                            }
                            customerListAll = resByCompany.Data?.Adapt<List<CustomerListVM>>() ?? new List<CustomerListVM>();
                        }
                        else
                        {
                            NotifyError(_stringLocalizer[Messages.COMPANY_LISTED_NOTFOUND]);
                            ViewBag.CurrentSort = sortOrder;
                            ViewBag.CurrentSearch = q; 
                            return View(Enumerable.Empty<CustomerListVM>().ToPagedList(pageNumber, pageSize));
                        }
                    }
                    else
                    {
                        NotifyError(_stringLocalizer[Messages.CUSTOMER_LIST_FAILED]);
                        ViewBag.CurrentSort = sortOrder;
                        ViewBag.CurrentSearch = q; 
                        return View(Enumerable.Empty<CustomerListVM>().ToPagedList(pageNumber, pageSize));
                    }
                }

                // Sunucu tarafı ARAMA 
                if (!string.IsNullOrWhiteSpace(q))
                    customerListAll = customerListAll
                        .Where(c => TurkishContains(c.Name, q))
                        .ToList();
          
                // Sıralama flag'lerini View'a taşı
                ViewData["CurrentSortOrder"] = sortOrder;
                ViewBag.CurrentSort = sortOrder;//diğer sayfalara sıralama devaam etmesi için
                ViewData["CurrentPage"] = pageNumber;
                ViewData["CurrentPageSize"] = pageSize;
                ViewBag.CurrentSearch = q;                     

                // PagedList
                var paginatedList = customerListAll.ToPagedList(pageNumber, pageSize);
                return View(paginatedList);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_GET_FAILED] + ": " + ex.Message);
                return View("Error");
            }
        }
        /// <summary>
        /// Yeni bir müşteri oluşturma sayfası
        /// /// Şirket, şehir ve ülke bilgilerini doldurarak sayfa modelini hazırlar.
        /// </summary>
        /// <returns>Yeni bir müşteri oluşturma sayfasını döndürür.</returns>
        public async Task<IActionResult> Create()
        {
			var user = await _userManager.GetUserAsync(User);
			bool isManager = user != null && await _userManager.IsInRoleAsync(user, "Manager");

			ViewBag.IsManager = isManager;

			List<Company> companies = new();

			if (isManager)
			{
				var companyId = await _employeeService.GetCompanyIdByUserIdAsync(user.Id);
				if (!string.IsNullOrEmpty(companyId))
				{
					var companyResult = await _companyService.GetByIdAsync(Guid.Parse(companyId));
					if (companyResult != null && companyResult.IsSuccess)
					{
						companies.Add(companyResult.Data.Adapt<Company>());
					}
				}
			}
			else
			{
				var companiesRes = await _companyService.GetAllAsync();
				if (companiesRes != null && companiesRes.IsSuccess)
				{
					companies = companiesRes.Data.Adapt<List<Company>>();
				}
			}

			var citiesRes = await _cityService.GetAllAsync();
			var countriesRes = await _countryService.GetAllAsync();

			var model = new CustomerCreateVM
			{
				Companies = companies,
				Cities = citiesRes.Data.Adapt<List<City>>(),
				Countries = countriesRes.Data.Adapt<List<Country>>()
			};

			return View(model);
		}

        /// <summary>
        /// Yeni bir müşteri oluşturma işlemi
        /// Form doğrulaması başarılı ise müşteri oluşturulur ve başarılı sonuç mesajıyla müşteri listesine yönlendirilir.
        /// </summary>
        /// <param name="model">Müşteri oluşturmak için gerekli bilgileri içeren model</param>
        /// <returns>İşlem başarılı ise müşteri listesine yönlendirir. Başarısız ise hata mesajı ile birlikte aynı sayfayı tekrar döndürür.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                var companiesRes = await _companyService.GetAllAsync();
                if (companiesRes.IsSuccess && companiesRes.Data != null)
                {
                    model.Companies = companiesRes.Data.Adapt<List<Company>>();
                }
                var citiesRes = await _cityService.GetAllAsync();
                if (citiesRes.IsSuccess && citiesRes.Data != null)
                {
                    model.Cities = citiesRes.Data.Adapt<List<City>>();
                }
                var countriesRes = await _countryService.GetAllAsync();
                if (countriesRes.IsSuccess && countriesRes.Data != null)
                {
                    model.Countries = countriesRes.Data.Adapt<List<Country>>();
                }
                return View(model);
            }

            
            model.Name=StringUtilities.CapitalizeEachWord(model.Name);
            model.Address=StringUtilities.CapitalizeEachWord(model.Address);

            var result = await _customerService.AddAsync(model.Adapt<CustomerCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_ADD_ERROR]);
                //NotifyError(result.Message);
                return View(model);
            }

            NotifySuccess(_stringLocalizer[Messages.CUSTOMER_ADD_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }




        /// <summary>
        /// Müşteri detaylarını gösteren sayfa
        /// </summary>
        /// <param name="customerId">Müşteri Id</param>
        /// <returns>Müşteri detaylarını gösteren sayfayı döndürür.</returns>
        public async Task<IActionResult> Details(Guid customerId)
        {
            try
            {

                var result = await _customerService.GetByIdAsync(customerId);
                
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CUSTOMER_NOT_FOUND]);
                    //NotifyError(result.Message);
                    return RedirectToAction("Index");
                }

                // Service'ten gelen DTO -> VM
                var customerDetailsVM = result.Data.Adapt<CustomerDetailsVM>();

                // Burada CountryId ile ülke bilgisi çekiyoruz
                var countryResult = await _countryService.GetByIdAsync(result.Data.CountryId);
                if (countryResult.IsSuccess && countryResult.Data != null)
                {
                    var currentCulture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                    customerDetailsVM.CountryName = currentCulture switch
                    {
                        "tr" => countryResult.Data.NameTr,
                        "en" => countryResult.Data.NameEn,
                        _ => countryResult.Data.NameEn
                    };
                }


                var adminResult = await _adminService.GetByIdentityIdAsync(result.Data.CreatedBy);
				var employeeResult = await _employeeService.GetByIdentityIdAsync(result.Data.CreatedBy.ToString());


				//var adminDetailsVM = adminResult?.Data.Adapt<AdminDetailsVM>() ?? new AdminDetailsVM { FirstName = "Bilinmeyen", LastName = "Bilinmeyen" , Email = "Bilinmeyen"};

				if (adminResult?.Data != null)
				{
					var adminDetailsVM = adminResult.Data.Adapt<AdminDetailsVM>();
					customerDetailsVM.Admin = adminDetailsVM;
					NotifySuccess(_stringLocalizer["CUSTOMER_FOUND_SUCCESS"]);

					return View(customerDetailsVM);
				}
				else
				{
					var employeeDetailsVM = employeeResult?.Data.Adapt<AdminDetailsVM>() ?? new AdminDetailsVM { FirstName = "Bilinmeyen", LastName = "Bilinmeyen", Email = "Bilinmeyen" };	
					customerDetailsVM.Admin = employeeDetailsVM;
					NotifySuccess(_stringLocalizer["CUSTOMER_FOUND_SUCCESS"]);

					return View(customerDetailsVM);
				}

            }
            catch (Exception ex)
            {
                var detailedMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                NotifyError($"An error occurred: {detailedMessage}");
                return View("Error");
            }
        }

        /// <summary>
        /// Müşteri bilgilerini güncellemek için sayfa
        /// Müşterinin şirket, şehir ve ülke bilgilerini de günceller.
        /// </summary>
        /// <param name="customerId">Müşteri Id</param>
        /// <returns>Müşteri bilgilerini güncellemek için sayfayı döndürür.</returns>
        public async Task<IActionResult> Update(Guid customerId)
        {
            var result = await _customerService.GetByIdAsync(customerId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_GET_FAILED]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var companiesResult = await _companyService.GetAllAsync();
            var citiesResult = await _cityService.GetAllAsync();
            var countriesResult = await _countryService.GetAllAsync();

            var customerUpdateVM = result.Data.Adapt<CustomerUpdateVM>();
            //CustomerUpdateVM customerUpdateVM = new CustomerUpdateVM();
            //customerUpdateVM.Phone = result.Data.Phone;
            //customerUpdateVM.CityId = result.Data.CityId;
            //customerUpdateVM.CompanyId = result.Data.CompanyId;

            customerUpdateVM.Companies = companiesResult.Data.Adapt<List<CompanyDTO>>();
            customerUpdateVM.Cities = citiesResult.Data.Adapt<List<CityDTO>>();
            customerUpdateVM.Countries = countriesResult.Data.Adapt<List<CountryDTO>>();

            return View(customerUpdateVM);
        }

        /// <summary>
        /// Müşteri bilgilerini güncellemek için işlem
        /// </summary>
        /// <param name="customerUpdateVM">Müşteri bilgilerini güncellemek için gerekli bilgileri içeren model</param>
        /// <returns>İşlem başarılı ise müşteri listesine yönlendirir. Başarısız ise hata mesajı ile birlikte aynı sayfayı tekrar döndürür.</returns>
        [HttpPost]
        public async Task<IActionResult> Update(CustomerUpdateVM customerUpdateVM)
        {
            if (!ModelState.IsValid)
            {
                var companiesRes = await _companyService.GetAllAsync();
                var citiesRes = await _cityService.GetAllAsync();
                var countriesRes = await _countryService.GetAllAsync();


                if (companiesRes.IsSuccess && companiesRes.Data != null)
                {
                    customerUpdateVM.Companies = companiesRes.Data.Adapt<List<CompanyDTO>>();
                }

                if (citiesRes.IsSuccess && citiesRes.Data != null)
                {
                    customerUpdateVM.Cities = citiesRes.Data.Adapt<List<CityDTO>>();
                }

                if (countriesRes.IsSuccess && countriesRes.Data != null)
                {
                    customerUpdateVM.Countries = countriesRes.Data.Adapt<List<CountryDTO>>();
                }
                return View(customerUpdateVM);
            }

            
            customerUpdateVM.Name=StringUtilities.CapitalizeEachWord(customerUpdateVM.Name);
            customerUpdateVM.Address=StringUtilities.CapitalizeEachWord(customerUpdateVM.Address);

            var result = await _customerService.UpdateAsync(customerUpdateVM.Adapt<CustomerUpdateDTO>());

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_UPDATED_FAILED]);
                //NotifyError(result.Message);
                return View(customerUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.CUSTOMER_UPDATED_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index"); ;
        }

        /// <summary>
        /// Müşteri silme işlemi
        /// </summary>
        /// <param name="customerId">Müşteri Id</param>
        /// <returns>İşlem başarılı ise müşteri listesine yönlendirir. Başarısız ise hata mesajı ile birlikte aynı sayfayı tekrar döndürür.</returns>
        public async Task<IActionResult> Delete(Guid customerId)
        {
            var result = await _customerService.DeleteAsync(customerId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_DELETE_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            NotifySuccess(_stringLocalizer[Messages.CUSTOMER_DELETE_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen ülke ID'sine göre şehir listesini getirir.
        /// </summary>
        /// <param name="countryId">Şehirlerin getirileceği ülkenin benzersiz ID'si.</param>
        /// <returns>Belirtilen ülkeye ait şehirlerin listesini içeren bir JSON nesnesi.Eğer bir hata oluşursa, hata mesajı ile birlikte bad request döner.</returns>
        [HttpGet]
        public async Task<IActionResult> GetCitiesByCountryId(Guid countryId)
        {
            try
            {
                var citiesResult = await _cityService.GetByCountryIdAsync(countryId);
                var cities = citiesResult.Data?.Adapt<List<CityDTO>>();
                return Json(cities);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Error fetching cities.");
            }
        }

        /// <summary>
        /// Müşteri Id bilgisine göre tüm siparişlerini gösterir
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="sortOrder"></param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCustomerOrdersHistory(Guid customerId, string sortOrder = "date", string searchQuery = "")
        {

            try
            {
                var result = await _customerService.GetCustomerOrdersHistoryAsync(customerId, sortOrder, searchQuery);
                if (!result.IsSuccess || result.Data == null || result.Data.Count == 0)
                {
                    NotifyError(_stringLocalizer[Messages.ORDER_LIST_EMPTY]);
                    return RedirectToAction("Index");
                }

                var customerOrdersHistoryVM = new CustomerOrdersHistoryVM
                {
                    customerId = customerId,
                    Orders = result.Data.Select(order => new OrderListVM
                    {
                        Id = order.Id,
                        TotalPrice = order.TotalPrice,
                        OrderDate = order.OrderDate,
                        IsActive = order.IsActive,
                        AdminName = order.AdminName,

                        OrderDetails = order.OrderDetails.Select(detail => new OrderDetailListDTO
                        {
                            Id = detail.Id,
                            ProductName = detail.ProductName,
                            Quantity = detail.Quantity,
                            TotalPrice = detail.TotalPrice,
                            Discount = detail.Discount,
                            UnitPrice = detail.UnitPrice
                        }).ToList()
                    }).ToList(),
                    CustomerName = result.Data.FirstOrDefault().CustomerName


                };

                NotifySuccess(_stringLocalizer["ORDER_LIST_FOUND_SUCCESS"]);

                return View(customerOrdersHistoryVM);
            }
            catch (Exception ex)
            {

                var detailedMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                NotifyError($"An error occurred: {detailedMessage}");
                return View("Error");
            }
        }





    }
}
