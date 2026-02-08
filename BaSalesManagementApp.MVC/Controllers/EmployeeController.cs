using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Core.Utilities.Results;
using BaSalesManagementApp.Dtos.EmployeeDTOs;
using BaSalesManagementApp.Dtos.MailDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.CountryVMs;
using BaSalesManagementApp.MVC.Models.EmployeeVMs;
using BaSalesManagementApp.MVC.Models.OrderVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    [Authorize(Roles = "Manager , Admin, Employee")]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICompanyService _companyService;
        private readonly RoleManager<IdentityRole> _roleManager;
        public List<SelectListItem> roleOptions;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly IMailService _mailService;
        private readonly IPasswordGeneratorService _passwordGeneratorService;
        private readonly IOrderService _orderService;
        private readonly UserManager<IdentityUser> _userManager;
        protected ClaimsPrincipal currentUser => User;

        public EmployeeController(IEmployeeService employeeService, IWebHostEnvironment webHostEnvironment, ICompanyService companyService, RoleManager<IdentityRole> roleManager, IStringLocalizer<Resource> stringLocalizer, IMailService mailService = null, IPasswordGeneratorService passwordGeneratorService = null, IOrderService orderService = null, UserManager<IdentityUser> userManager = null)
        {
            _employeeService = employeeService;
            _webHostEnvironment = webHostEnvironment;
            _companyService = companyService;
            _roleManager = roleManager;
            _stringLocalizer = stringLocalizer;
            _mailService = mailService;
            _passwordGeneratorService = passwordGeneratorService;
            _orderService = orderService;
            _userManager = userManager;
        }


        //sortEmployee parametresi kaldırıldı hep A-Z sıralaması olacagı icin, name gereksiz hale geldi
        public async Task<IActionResult> Index(int? page, int pageSize = 10, Guid? companyId = null, string sortOrder = "name_asc")
        {
            int pageNumber = page ?? 1;

            //sisteme giriş yapan kişinin kendi profil bilglerini görmesi için.
            // Giriş yapan kullanıcı Employee rolünde mi kontrol et
            if (User.IsInRole("Employee"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var employeeResult = await _employeeService.GetByIdentityIdAsync(userId);

                if (employeeResult is not { IsSuccess: true, Data: not null })
                {
                    NotifyError(_stringLocalizer[Messages.EMPLOYEE_LISTED_NOTFOUND]);
                    return View(Enumerable.Empty<EmployeeListVM>().ToPagedList(1, 1));
                }
                //mapster
                var employeeVM = employeeResult.Data.Adapt<EmployeeListVM>();

                var companyResult = await _companyService.GetByIdAsync(employeeVM.CompanyId);
                employeeVM.CompanyName = companyResult.Data?.Name ?? "N/A";

                return View(new List<EmployeeListVM> { employeeVM }.ToPagedList(1, 1));
            }

            var companiesResult = await _companyService.GetAllAsync();
            if (companiesResult.IsSuccess && companiesResult.Data != null)
            {
                ViewData["Companies"] = companiesResult.Data
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();

                // Log for debugging
                foreach (var company in (List<SelectListItem>)ViewData["Companies"])
                {
                    Console.WriteLine($"Company: {company.Text}, ID: {company.Value}");
                }
            }
            else
            {
                ViewData["Companies"] = new List<SelectListItem>();
                Console.WriteLine("No companies found.");
            }

            Guid? managerCompanyId = null;

            if (currentUser.IsInRole("Manager"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Kullanıcı kimliğini al
                var userResult = await _employeeService.GetByIdentityIdAsync(userId); // Kullanıcı bilgilerini al

                if (userResult.IsSuccess && userResult.Data != null)
                {
                    managerCompanyId = userResult.Data.CompanyId; // Şirket kimliğini al
                }

                if (managerCompanyId.HasValue)
                {
                    // Eğer manager ise sadece kendi şirketine ait çalışanları göster
                    companyId = managerCompanyId;
                }
            }

            ViewData["CurrentCompanyId"] = companyId?.ToString();

            IDataResult<List<EmployeeListDTO>> result;
            if (companyId.HasValue)
            {
                result = await _employeeService.GetByCompanyIdAsync(companyId.Value);
            }
            else
            {
                result = await _employeeService.GetAllAsync();
            }

            if (!result.IsSuccess)
            {
              
                    // Şirket sözlüğünü oluştur
                    var companyDictionary = companiesResult.IsSuccess
                        ? companiesResult.Data.ToDictionary(c => c.Id, c => c.Name)
                        : new Dictionary<Guid, string>();

                    // Seçilen şirketin adını alma
                    string companyName = companyDictionary.TryGetValue(companyId.Value, out var name)
                        ? name
                        : "Bilinmeyen Şirket";

                    // Başarısız mesajını göster
                    NotifyError(string.Format(_stringLocalizer[Messages.EMPLOYEE_LISTED_NOTFOUND], companyName));
               

            }


            var employeeList = result.Data?.Adapt<List<EmployeeListVM>>() ?? new List<EmployeeListVM>();

            // Şirket Filtreleme işleminden sonra başarılı işlemi
            if (companyId.HasValue && employeeList.Any())
            {
                // Şirket sözlüğünü oluştur
                var companyDictionary = companiesResult.IsSuccess
                    ? companiesResult.Data.ToDictionary(c => c.Id, c => c.Name)
                    : new Dictionary<Guid, string>();

                // Seçilen şirketin adını alma
                string companyName = companyDictionary.TryGetValue(companyId.Value, out var name)
                    ? name
                    : "Bilinmeyen Şirket";

                // Başarı mesajını göster
                NotifySuccess(string.Format(_stringLocalizer[Messages.EMPLOYEE_LISTED_SUCCESS], companyName));
            }



            // Şirket isimlerini tek seferde ayarla  
            var companyNames = companiesResult.IsSuccess
                ? companiesResult.Data.ToDictionary(c => c.Id, c => c.Name)
                : new Dictionary<Guid, string>();

            foreach (var employee in employeeList)
            {
                employee.CompanyName = companyNames.TryGetValue(employee.CompanyId, out var name) ? name : "N/A";
            }

            // Sıralama işlemi - Geliştirilmiş versiyon
            employeeList = sortOrder switch
            {
                "name_asc" => employeeList.OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList(),
                "name_desc" => employeeList.OrderByDescending(e => e.FirstName).ThenByDescending(e => e.LastName).ToList(),
                "date_asc" => employeeList.OrderBy(e => e.CreatedDate).ToList(),
                "date_desc" => employeeList.OrderByDescending(e => e.CreatedDate).ToList(),
                _ => employeeList.OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList()
            };

            var paginatedList = employeeList.ToPagedList(pageNumber, pageSize);

            // ViewData ile sıralama bilgilerini aktarıyoruz
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["CurrentPageSize"] = pageSize;

            return View(paginatedList);
        }



        // Aynı işlemleri create get ve postunda kullandığımız için buraya action yazıp buradan ilgili actionlara uygun model gönderiyoruz.
        private async Task<EmployeeCreateVM> PrepareEmployeeCreateVMAsync(EmployeeCreateVM model = null)
        {
            var roleOptions = new List<SelectListItem>();
            foreach (var role in _roleManager.Roles)
            {
                if (role.Name.ToLower() != "admin")
                {
                    roleOptions.Add(new SelectListItem
                    {
                        Value = role.Name,  // Rol adını kullanıyoruz
                        Text = role.Name
                    });
                }
            }

            var companiesResult = await _companyService.GetAllAsync();
            var companyOptions = new List<SelectListItem>();
            if (companiesResult.IsSuccess)
            {
                var companies = companiesResult.Data.Adapt<List<CompanyListVM>>();
                companyOptions = companies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
            }

            if (model == null)
            {
                model = new EmployeeCreateVM();
            }

            model.RoleOptions = roleOptions;
            model.CompanyOptions = companyOptions;

            return model;
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // ViewModel hazırlanıyor
            var model = await PrepareEmployeeCreateVMAsync();

            // Mevcut kullanıcıyı alıyoruz
            var currentUser = await _userManager.GetUserAsync(User);

            // Kullanıcının rollerini alıyoruz
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            // Kullanıcının rolünü frontend'e gönder
            ViewData["UserRole"] = userRoles.Contains("Manager") ? "Manager" : "Other";
            // Eğer kullanıcının rolü 'Manager' ise 'Admin' ve 'Manager' rollerini hariç tut
            if (userRoles.Contains("Manager"))
            {
                var currentCompany = await _employeeService.GetCompanyIdByUserIdAsync(currentUser.Id);
                model.CompanyId = Guid.Parse(currentCompany);

                model.RoleOptions = _roleManager.Roles
                    .Where(r => r.Name.ToLower() != "admin" && r.Name.ToLower() != "manager") // 'Admin' ve 'Manager' hariç
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name,  // Rol adını değer olarak kullanıyoruz
                        Text = _stringLocalizer[r.Name]  // Çevrilmiş rol adını gösteriyoruz
                    })
                    .ToList();
            }
            else
            {
                model.RoleOptions = _roleManager.Roles
                    .Where(r => r.Name.ToLower() != "admin")  // 'Admin' rolünü hariç tutuyoruz
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name,  // Rol adını değer olarak kullanıyoruz
                        Text = _stringLocalizer[r.Name]  // Çevrilmiş rol adını gösteriyoruz
                    })
                    .ToList();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateVM model)
        {
            // Model geçerli değilse
            if (!ModelState.IsValid)
            {
                NotifyError(_stringLocalizer["The_information_entered_is_not_valid."]);
                model = await PrepareEmployeeCreateVMAsync(model);
                return View(model);
            }

            // Eğer çalışan bir "Manager" ise ve zaten o şirkette bir Manager varsa işlem iptal edilir
            if (model.Title == "Manager")
            {
                bool managerExists = await _employeeService.IsManagerExistsAsync(model.CompanyId);
                if (managerExists)
                {
                    NotifyError(_stringLocalizer["Her şirkette sadece bir yönetici bulunabilir."]);
                    model = await PrepareEmployeeCreateVMAsync(model);
                    return View(model);
                }
            }

            if (model.Photo != null)
            {
                var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Photo", _stringLocalizer["Invalid file type. Please upload an image file."]);
                    model = await PrepareEmployeeCreateVMAsync(model);
                    return View(model);
                }
            }

            // İsimleri kültürel bilgiyle düzenleme
            var turkishCulture = new CultureInfo("tr-TR");
            model.FirstName = StringUtilities.CapitalizeEachWord(model.FirstName);
            model.LastName = StringUtilities.CapitalizeFirstLetter(model.LastName);

            // DTO oluşturma
            var employeeDto = new EmployeeCreateDTO
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhotoData = null,
                Title = model.Title,
                CompanyId = model.CompanyId
            };

            // Fotoğrafı DTO'ya ekleme
            if (model.Photo != null && model.Photo.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.Photo.CopyToAsync(memoryStream);
                    employeeDto.PhotoData = memoryStream.ToArray();
                }
            }

            // Çalışanı ekleme
            var result = await _employeeService.AddAsync(employeeDto);
            if (result.IsSuccess)
            {
                // Kullanıcı adı ve şifre oluştur
                var username = model.Email.Split('@')[0];

                // Burada şifreyi 8 karakter uzunluğunda oluşturuyoruz
                var password = await _passwordGeneratorService.GenerateRandomPasswordAsync(8);

                // E-posta için DTO
                var mailDto = new MailCreateDto
                {
                    Title = "Kullanıcı Bilgileri",
                    ReceiverMailAddress = model.Email,
                    Subject = _stringLocalizer["Your Account Information"],
                    Body = $@"
                            <p>Merhaba {model.FirstName},</p>
                            <p>Kullanıcı adınız: <b>{username}</b></p>
                            <p>Şifreniz: <b>{password}</b></p>
                            <p>Lütfen sisteme giriş yaptıktan sonra şifrenizi değiştirin.</p>"
                };

                try
                {
                    await _mailService.SendMailAsync(mailDto);
                    NotifySuccess(_stringLocalizer[Messages.EMPLOYEE_ADD_SUCCESS]);
                }
                catch (Exception ex)
                {
                    NotifyError(_stringLocalizer["Mail gönderilemedi. Hata:"] + ex.Message);
                }

                return RedirectToAction("Index");
            }

            // İşlem başarısızsa
            NotifyError(_stringLocalizer[Messages.EMPLOYEE_ADD_ERROR]);
            model = await PrepareEmployeeCreateVMAsync(model);
            return View(model);
        }

        public async Task<IActionResult> Details(Guid employeeId)
        {
            var result = await _employeeService.GetByIdAsync(employeeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_GETBYID_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var employeeDetails = result.Data.Adapt<EmployeeDetailsVM>();
            var companyResult = await _companyService.GetByIdAsync(employeeDetails.CompanyId);
            if (companyResult.IsSuccess)
            {
                employeeDetails.CompanyName = companyResult.Data.Name;
            }

            return View(employeeDetails);
        }

        public async Task<IActionResult> Update(Guid employeeId)
        {
            // Manager rolündeki kullanıcıların sadece kendi şirketlerine ait verileri güncelleyebilmeleri için şirket kimliği belirleniyoruz.
            Guid? managerCompanyId = null;

            var result = await _employeeService.GetByIdAsync(employeeId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_GETBYID_ERROR]);
                return RedirectToAction("Index");
            }

            var employeeUpdateVM = result.Data.Adapt<EmployeeUpdateVM>();

            // Kullanıcı Manager rolündeyse, sadece kendi şirketine ait çalışanı güncelleyebilsin
            if (currentUser.IsInRole("Manager"))
            {
                // Kullanıcının kimliği alınarak şirket bilgileri getiriliyor
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Kullanıcı kimliğini alıyoruz.
                var userResult = await _employeeService.GetByIdentityIdAsync(userId); // Kullanıcı bilgilerini alıyoruz.

                // Kullanıcı bilgileri başarıyla alındıysa, Manager'ın şirket kimliğini belirleniyoruz.
                if (userResult.IsSuccess && userResult.Data != null)
                {
                    managerCompanyId = userResult.Data.CompanyId;
                }

                // Manager sadece kendi şirketine ait çalışanı güncelleyebilir
                if (managerCompanyId.HasValue && employeeUpdateVM.CompanyId != managerCompanyId.Value)
                {
                    NotifyError("Sadece kendi şirketinize ait çalışanları güncelleyebilirsiniz.");
                    return RedirectToAction("Index");
                }

                // Manager için şirket bilgisini, sadece kendi şirketine ayarlıyoruz
                employeeUpdateVM.CompanyId = managerCompanyId.Value;
            }

            // Eğer kullanıcı Admin rolündeyse, tüm şirketlerin listesiin göstetiyoruz.
            if (!currentUser.IsInRole("Manager"))
            {
                var companiesResult = await _companyService.GetAllAsync();
                if (!companiesResult.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.EMPLOYEE_LISTED_ERROR]);
                    return RedirectToAction("Index");
                }

                var companies = companiesResult.Data.Adapt<List<CompanyListVM>>();
                ViewBag.Companies = companies;
            }

            if (employeeUpdateVM.PhotoData != null)
            {
                string base64 = Convert.ToBase64String(employeeUpdateVM.PhotoData);
                employeeUpdateVM.PhotoUrl = $"data:image/png;base64,{base64}";
            }

            // Güncellenmiş çalışan bilgilerini View'a gönderiyruz.
            return View(employeeUpdateVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EmployeeUpdateVM employeeUpdateVM)
        {
            if (ModelState.IsValid)
            {
                var employeeDto = employeeUpdateVM.Adapt<EmployeeUpdateDTO>();

                // Eğer çalışan Manager olarak güncelleniyorsa, şirket içinde başka bir Manager var mı kontrol et
                if (employeeUpdateVM.Title == "Manager")
                {
                    bool isAnotherManagerExists = await _employeeService.IsAnotherManagerExistsAsync(employeeUpdateVM.CompanyId, employeeUpdateVM.Id);
                    if (isAnotherManagerExists)
                    {
                        NotifyError("Her şirkette yalnızca bir manager olabilir.");
                        return View(employeeUpdateVM);
                    }
                }

                var result = await _employeeService.UpdateAsync(employeeDto);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.EMPLOYEE_UPDATE_ERROR]);
                    return View(employeeUpdateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.EMPLOYEE_UPDATE_SUCCESS]);
                return RedirectToAction("Index");
            }

            return View(employeeUpdateVM);
        }

        public async Task<IActionResult> Delete(Guid employeeId)
        {
            var result = await _employeeService.DeleteAsync(employeeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_DELETE_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            NotifySuccess(_stringLocalizer[Messages.EMPLOYEE_DELETE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }
        // personellerin geçmiş sparişleri
        [HttpGet]
        public async Task<IActionResult> GetOrdersByEmployee(Guid employeeId)
        {


            if (!ModelState.IsValid)
            {
                NotifyError(_stringLocalizer[Messages.ORDER_LIST_EMPTY]);
            }

            var result = await _orderService.GetOrdersByEmployeeIdAsync(employeeId);
            try
            {
                // Servisten veri çek
                var serviceResult = await _orderService.GetOrdersByEmployeeIdAsync(employeeId);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.ORDER_LIST_EMPTY]);
                    return View(new EmployeeOrderVM());
                }

                // İlk siparişten çalışana ait temel bilgileri al
                var firstOrder = result.Data.FirstOrDefault();

                var employeeOrdersVM = new EmployeeOrderVM()
                {
                    EmployeeId = employeeId,
                    FirstName = firstOrder?.EmployeeName,
                    LastName = firstOrder?.EmployeeLastName,
                    Orders = result.Data.Select(order => new OrderListVM
                    {
                        Id = order.Id,
                        OrderDate = order.OrderDate,
                        IsActive = order.IsActive,
                        TotalPrice = order.OrderDetails.Sum(detail =>
                            detail.UnitPrice * detail.Quantity * (1 - detail.Discount / 100)), // İndirimli toplam
                        OrderDetails = order.OrderDetails.Select(detail => new OrderDetailListDTO
                        {
                            Id = detail.Id,
                            ProductName = detail.ProductName,
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice,
                            Discount = detail.Discount,
                            TotalPrice = detail.UnitPrice * detail.Quantity * (1 - detail.Discount / 100) // İndirimli toplam
                        }).ToList()
                    }).ToList(),
                    DailyOrderSummary = result.Data
                        .GroupBy(o => o.OrderDate.Date)
                        .Select(group => new DailyOrderSummaryVM
                        {
                            OrderDate = group.Key,
                            TotalOrders = group.Count(),
                            TotalDailyPrice = group.Sum(order =>
                                order.OrderDetails.Sum(detail =>
                                    detail.UnitPrice * detail.Quantity * (1 - detail.Discount / 100))) // Günlük indirimli toplam
                        }).ToList()
                };

                NotifySuccess(_stringLocalizer[Messages.ORDER_LISTED_SUCCESS]);
                return View(employeeOrdersVM);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.ORDER_LIST_FAILED] + ": " + ex.Message);
                return View("Error");
            }
        }
    }
}
