using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.PaymentTypeDTOs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.PaymentTypeVMs;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Globalization;
using System.Security.Claims;
using X.PagedList;
using System.Globalization;


namespace BaSalesManagementApp.MVC.Controllers
{
    public class PaymentTypeController : BaseController
    {
        private readonly IPaymentTypeService _paymentTypeService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly IEmployeeService _employeeService;

        public PaymentTypeController(IPaymentTypeService paymentTypeService, IStringLocalizer<Resource> stringLocalizer, IEmployeeService employeeService)
        {
            _paymentTypeService = paymentTypeService;
            _stringLocalizer = stringLocalizer;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index(int? page, int pageSize = 10, string sortOrder = "alphabetical", [FromQuery(Name = "searchQuery")] string? searchQuery = null)
        {
            try
            {
                if (User.IsInRole("Manager"))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userRole = User.FindFirstValue(ClaimTypes.Role);
                    var userResult = await _employeeService.GetByIdentityIdAsync(userId);
                    var companyId = userResult.Data.CompanyId;
                    int pageNumber = page ?? 1;
                    var term = (searchQuery ?? string.Empty).Trim();
                    var paymentTypeList = await _paymentTypeService.GetPaymentTypesByCompanyIdAsync(companyId);

                    var paymentTypeListVMs = paymentTypeList.Data.Adapt<List<PaymentTypeListVM>>();
                    if (!paymentTypeList.IsSuccess)
                    {
                        NotifyError(_stringLocalizer[Messages.PAYMENT_TYPE_LIST_EMPTY]);
                        return View(Enumerable.Empty<PaymentTypeListVM>().ToPagedList(pageNumber, pageSize));
                    }
                   
                    var paginatedList = paymentTypeListVMs.ToPagedList(pageNumber, pageSize);
                    // ARAMA 
                    if (!string.IsNullOrWhiteSpace(term))
                    {
                        paymentTypeListVMs = paymentTypeListVMs
                            .Where(x =>
                                (!string.IsNullOrEmpty(x.Name) &&
                                 x.Name.Contains(term, StringComparison.CurrentCultureIgnoreCase)) ||
                                x.Rate.ToString(CultureInfo.InvariantCulture)
                                      .Contains(term, StringComparison.InvariantCultureIgnoreCase))
                            .ToList();
                    }
                    ViewData["CurrentSortOrder"] = sortOrder;
                    ViewData["CurrentPage"] = pageNumber;
                    ViewData["CurrentPageSize"] = pageSize;
                    ViewData["CurrentSearchQuery"] = term;
                    ViewBag.searchQuery = term;
                    return View(paginatedList);
                }
                else
                {
                    int pageNumber = page ?? 1;
                    string term = (searchQuery ?? string.Empty).Trim();
                    var result = await _paymentTypeService.GetAllAsync(sortOrder);
                    var paymentTypeListVMs = result.Data.Adapt<List<PaymentTypeListVM>>();

                    if (!result.IsSuccess)
                    {
                        NotifyError(_stringLocalizer[Messages.PAYMENT_TYPE_LIST_EMPTY]);
                        return View(Enumerable.Empty<PaymentTypeListVM>().ToPagedList(pageNumber, pageSize));
                    }
                    if (!string.IsNullOrWhiteSpace(term))
                    {
                        paymentTypeListVMs = paymentTypeListVMs
                            .Where(x =>
                                (!string.IsNullOrEmpty(x.Name) &&
                                 x.Name.Contains(term, StringComparison.CurrentCultureIgnoreCase)) ||
                                x.Rate.ToString(CultureInfo.InvariantCulture)
                                      .Contains(term, StringComparison.InvariantCultureIgnoreCase))
                            .ToList();
                    }
                    var paginatedList = paymentTypeListVMs.ToPagedList(pageNumber, pageSize);
                    ViewData["CurrentSortOrder"] = sortOrder;
                    ViewData["CurrentPage"] = pageNumber;
                    ViewData["CurrentSearchQuery"] = term;
                    ViewBag.searchQuery = term;
                    ViewData["CurrentPageSize"] = pageSize;
                    return View(paginatedList);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Ödeme türlerini getirirken bir hata meydana geldi: " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                if (User.IsInRole("Manager"))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userResult = await _employeeService.GetByIdentityIdAsync(userId);

                    if (userResult.IsSuccess)
                    {
                        ViewBag.CompanyId = userResult.Data.CompanyId;
                    }
                    else
                    {
                        NotifyError(_stringLocalizer["Kullanıcı bilgileri alınamadı."]);
                        return View("Error");
                    }
                }

                return View();
            }
            catch (Exception)
            {
                NotifyError(_stringLocalizer["Sayfa yüklenirken bir hata meydana geldi."]);
                return View("Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentTypeCreateVM paymentTypeCreateVM)
        {
            try
            {
                paymentTypeCreateVM.Name = StringUtilities.CapitalizeEachWord(paymentTypeCreateVM.Name);

                if (User.IsInRole("Manager"))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userResult = await _employeeService.GetByIdentityIdAsync(userId);
                    paymentTypeCreateVM.CompanyId = userResult.Data.CompanyId;
                }

                var paymentTypeResult = await _paymentTypeService.GetAllAsync();
                if (paymentTypeResult.IsSuccess)
                {
                    var paymentTypes = paymentTypeResult.Data;
                    if (paymentTypes.Any(py => py.Name.Equals(paymentTypeCreateVM.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError("Name", _stringLocalizer["Bu isimde ödeme tipi zaten mevcut, farklı bir ödeme tipi giriniz"]);
                        return View(paymentTypeCreateVM);
                    }
                }

                var result = await _paymentTypeService.AddAsync(paymentTypeCreateVM.Adapt<PaymentTypeCreateDTO>());

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.PAYMENT_TYPE_CREATE_FAILED]);
                    return View(paymentTypeCreateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.PAYMENT_TYPE_CREATED_SUCCESS]);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var errorMessage = "Ödeme türü oluşturulurken bir hata meydana geldi: " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(Guid paymentTypeId)
        {
            try
            {
                var result = await _paymentTypeService.DeleteAsync(paymentTypeId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.PAYMENT_TYPE_DELETE_FAILED]);
                    return View();
                }

                NotifySuccess(_stringLocalizer[Messages.PAYMENT_TYPE_DELETED_SUCCESS]);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var errorMessage = "Ödeme türü silinirken bir hata meydana geldi: " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(Guid paymentTypeId)
        {
            var result = await _paymentTypeService.GetByIdAsync(paymentTypeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PAYMENT_TYPE_NOT_FOUND]);
                return View();
            }

            NotifySuccess(_stringLocalizer[Messages.PAYMENT_TYPE_FOUND_SUCCESS]);
            return View(result.Data.Adapt<PaymentTypeDetailsVM>());
        }

        public async Task<IActionResult> Update(Guid paymentTypeId)
        {
            var result = await _paymentTypeService.GetByIdAsync(paymentTypeId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PAYMENT_TYPE_UPDATED_FAILED]);
                return RedirectToAction("Index");
            }

            var paymentTypeUpdateVM = result.Data.Adapt<PaymentTypeUpdateVM>();
            return View(paymentTypeUpdateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PaymentTypeUpdateVM paymentTypeUpdateVM)
        {
            if (!ModelState.IsValid)
            {
                return View(paymentTypeUpdateVM);
            }

            paymentTypeUpdateVM.Name = StringUtilities.CapitalizeFirstLetter(paymentTypeUpdateVM.Name);

            var result = await _paymentTypeService.UpdateAsync(paymentTypeUpdateVM.Adapt<PaymentTypeUpdateDTO>());

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PAYMENT_TYPE_UPDATED_FAILED]);
                return View(paymentTypeUpdateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.PAYMENT_TYPE_UPDATED_SUCCESS]);
            return RedirectToAction("Index");
        }
    }
}



