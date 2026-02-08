using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CustomerDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.OrderDTOs;
using BaSalesManagementApp.Dtos.PaymentTypeDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.MVC.Models.OrderVMs;
using BaSalesManagementApp.MVC.Models.PaymentTypeVMs;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly IAdminService _adminService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly ICustomerService _customerService;
        private readonly IStockService _stockService;
        private readonly IEmployeeService _employeeService;
        private readonly IPaymentTypeService _paymentTypeService;

        /// <summary>
        /// OrderController kurucusu, IOrderService bağımlılığını alır.
        /// </summary>
        /// <param name="orderService">Sipariş hizmeti</param>
        public OrderController(IOrderService orderService, IProductService productService, ICompanyService companyService,
            IAdminService adminService, IStringLocalizer<Resource> stringLocalizer, ICustomerService customerService,
            IStockService stockService, IEmployeeService employeeService, IPaymentTypeService paymentTypeService)
        {
            _orderService = orderService;
            _productService = productService;
            _companyService = companyService;
            _adminService = adminService;
            _stringLocalizer = stringLocalizer;
            _customerService = customerService;
            _stockService = stockService;
            _employeeService = employeeService;
            _paymentTypeService = paymentTypeService;

        }

        /// <summary>
        /// Tüm siparişleri listeleyen ana sayfa görünümünü döndürür.
        /// </summary>
        /// <returns>Sipariş listesini gösteren ana sayfa görünümü</returns>
        //public async Task<IActionResult> Index(int? page, string sortOrder = "date", int pageSize = 10, Guid? company_Id = null, Guid? customer_Id = null, Guid? selectedCustomerId = null)
        //{
        //    try
        //    {
        //        int pageNumber = page ?? 1;

        //        ViewBag.CurrentSort = sortOrder;
        //        ViewBag.DateSortParm = sortOrder == "date" ? "datedesc" : "date";
        //        ViewBag.CurrentPageSize = pageSize;

        //        // Yönetici (Manager) kontrolü
        //        if (User.IsInRole("Manager"))
        //        {
        //            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        //            {
        //                NotifyError("User ID is missing or invalid.");
        //                return View("Error");
        //            }

        //            var companyResult = await _employeeService.GetCompanyIdByUserIdAsync(userGuid);
        //            if (!companyResult.IsSuccess || companyResult.Data == null)
        //            {
        //                NotifyError("Failed to retrieve company ID. Please ensure your profile is linked to a company.");
        //                return View("Error");
        //            }

        //            company_Id = companyResult.Data;

        //            if (company_Id == null || company_Id == Guid.Empty)
        //            {
        //                NotifyError("Your company ID is missing. Please contact the administrator.");
        //                return View("Error");
        //            }

        //            // Yönetici için şirket bazlı siparişleri getir
        //            var result = await _orderService.GetOrdersListByCompanyIdAsync(company_Id.Value);
        //            if (!result.IsSuccess)
        //            {
        //                NotifyError(_stringLocalizer["ORDER_LIST_FAILED"]);
        //                return View(Enumerable.Empty<OrderListVM>().ToPagedList(pageNumber, pageSize));
        //            }

        //            var orderListVM = result.Data.Select(order => order.Adapt<OrderListVM>()).ToList();

        //            // Sıralama işlemi
        //            switch (sortOrder.ToLower())
        //            {
        //                case "date":
        //                    orderListVM = orderListVM.OrderByDescending(s => s.OrderDate).ToList();
        //                    break;
        //                case "datedesc":
        //                    orderListVM = orderListVM.OrderBy(s => s.OrderDate).ToList();
        //                    break;
        //                case "active":
        //                    orderListVM = orderListVM.Where(s => s.IsActive == true).ToList();
        //                    break;
        //                case "inactive":
        //                    orderListVM = orderListVM.Where(s => s.IsActive == false).ToList();
        //                    break;
        //            }

        //            return View(orderListVM.ToPagedList(pageNumber, pageSize));
        //        }
        //        else // Admin için
        //        {
        //            var companiesResult = await _companyService.GetAllAsync();
        //            ViewBag.Companies = companiesResult.IsSuccess
        //                ? companiesResult.Data.Adapt<List<CompanyDTO>>()
        //                : new List<CompanyDTO>();

        //            var customersResult = await _customerService.GetAllAsync();
        //            var customerList = customersResult.IsSuccess
        //                ? customersResult.Data.Adapt<List<CustomerDTO>>()
        //                : new List<CustomerDTO>();

        //            ViewBag.Customers = customerList;
        //            ViewBag.SelectedCustomerId = customer_Id;

        //            // Müşteri bazlı filtreleme
        //            if (customer_Id.HasValue && customer_Id.Value != Guid.Empty)
        //            {
        //                var result = await _orderService.GetOrdersListByCustomerIdAsync(customer_Id.Value);
        //                var customerResult = await _customerService.GetByIdAsync(customer_Id.Value);

        //                var customerName = string.IsNullOrWhiteSpace(customerResult?.Data?.Name)
        //                    ? _stringLocalizer["Unknown Customer"]
        //                    : customerResult.Data.Name;

        //                if (!result.IsSuccess || !result.Data?.Any() == true)
        //                {
        //                    var errorMessage = string.Format(_stringLocalizer[Messages.ORDER_CUSTOMER_LIST_FAILED], customerName);
        //                    NotifyError(errorMessage);
        //                    return View(Enumerable.Empty<OrderListVM>().ToPagedList(pageNumber, pageSize));
        //                }

        //                var orderListVM = result.Data.Select(order => order.Adapt<OrderListVM>()).ToList();
        //                return View(orderListVM.ToPagedList(pageNumber, pageSize));
        //            }

        //            // Şirket bazlı filtreleme
        //            if (company_Id.HasValue)
        //            {
        //                var result = await _orderService.GetOrdersListByCompanyIdAsync(company_Id.Value);
        //                var companyResult = await _companyService.GetByIdAsync(company_Id.Value);

        //                var companyName = string.IsNullOrWhiteSpace(companyResult?.Data?.Name)
        //                    ? _stringLocalizer["Unknown Company"]
        //                    : companyResult.Data.Name;

        //                if (!result.IsSuccess || !result.Data?.Any() == true)
        //                {
        //                    var errorMessage = string.Format(_stringLocalizer[Messages.ORDER_COMPANY_LIST_FAILED], companyName);
        //                    NotifyError(errorMessage);
        //                    return View(Enumerable.Empty<OrderListVM>().ToPagedList(pageNumber, pageSize));
        //                }

        //                var orderListVM = result.Data.Select(order => order.Adapt<OrderListVM>()).ToList();
        //                return View(orderListVM.ToPagedList(pageNumber, pageSize));
        //            }

        //            // Genel sipariş listesi
        //            var allOrdersResult = await _orderService.GetAllAsync(sortOrder);
        //            if (!allOrdersResult.IsSuccess)
        //            {
        //                NotifyError(_stringLocalizer["ORDER_LIST_FAILED"]);
        //                return View(Enumerable.Empty<OrderListVM>().ToPagedList(pageNumber, pageSize));
        //            }

        //            var allOrderListVM = allOrdersResult.Data.Select(order => order.Adapt<OrderListVM>()).ToList();
        //            return View(allOrderListVM.ToPagedList(pageNumber, pageSize));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        NotifyError("An unexpected error occurred. Please try again later.");
        //        return View("Error");
        //    }
        //}


        public async Task<IActionResult> Index(int? page, string sortOrder = "date", int pageSize = 10, Guid? company_Id = null, Guid? customer_Id = null, Guid? selectedCustomerId = null,string searchQuery=null)
        {
            try
            {
                int pageNumber = page ?? 1;

                ViewBag.CurrentSort = sortOrder;
                ViewBag.DateSortParm = sortOrder == "date" ? "datedesc" : "date";
                ViewBag.CurrentPageSize = pageSize;
                ViewData["CurrentFilter"] = searchQuery;

                // Yönetici (Manager) kontrolü
                if (User.IsInRole("Manager"))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                    {
                        NotifyError("User ID is missing or invalid.");
                        return View("Error");
                    }

                    var companyResult = await _employeeService.GetCompanyIdByUserIdAsync(userGuid);
                    if (!companyResult.IsSuccess || companyResult.Data == null)
                    {
                        NotifyError("Failed to retrieve company ID. Please ensure your profile is linked to a company.");
                        return View("Error");
                    }

                    company_Id = companyResult.Data;

                    if (company_Id == null || company_Id == Guid.Empty)
                    {
                        NotifyError("Your company ID is missing. Please contact the administrator.");
                        return View("Error");
                    }

                    // Yönetici için şirket bazlı siparişleri getir
                    var result = await _orderService.GetOrdersListByCompanyIdAsync(company_Id.Value);
                    if (!result.IsSuccess)
                    {
                        NotifyError(_stringLocalizer["ORDER_LIST_FAILED"]);
                        return View(Enumerable.Empty<OrderListVM>().ToPagedList(pageNumber, pageSize));
                    }

                    var orderListVM = result.Data.Select(order => order.Adapt<OrderListVM>()).ToList();

                    // Sıralama işlemi
                    switch (sortOrder.ToLower())
                    {
                        case "date":
                            orderListVM = orderListVM.OrderByDescending(s => s.OrderDate).ToList();
                            break;
                        case "datedesc":
                            orderListVM = orderListVM.OrderBy(s => s.OrderDate).ToList();
                            break;
                        case "active":
                            orderListVM = orderListVM.Where(s => s.IsActive == true).ToList();
                            break;
                        case "inactive":
                            orderListVM = orderListVM.Where(s => s.IsActive == false).ToList();
                            break;
                    }

                    return View(orderListVM.ToPagedList(pageNumber, pageSize));
                }
                else // Admin için
                {
                    var companiesResult = await _companyService.GetAllAsync();
                    ViewBag.Companies = companiesResult.IsSuccess
                        ? companiesResult.Data.Adapt<List<CompanyDTO>>()
                        : new List<CompanyDTO>();

                    var customersResult = await _customerService.GetAllAsync();
                    var customerList = customersResult.IsSuccess
                        ? customersResult.Data.Adapt<List<CustomerDTO>>()
                        : new List<CustomerDTO>();

                    ViewBag.Customers = customerList.OrderBy(c => c.Name).ToList(); ;
                    ViewBag.SelectedCustomerId = customer_Id;

                    // Müşteri bazlı filtreleme
                    if (customer_Id.HasValue && customer_Id.Value != Guid.Empty)
                    {
                        var result = await _orderService.GetOrdersListByCustomerIdAsync(customer_Id.Value);
                        var customerResult = await _customerService.GetByIdAsync(customer_Id.Value);

                        var customerName = string.IsNullOrWhiteSpace(customerResult?.Data?.Name)
                            ? _stringLocalizer["Unknown Customer"]
                            : customerResult.Data.Name;

                        if (!result.IsSuccess || !result.Data?.Any() == true)
                        {
                            var errorMessage = string.Format(_stringLocalizer[Messages.ORDER_CUSTOMER_LIST_FAILED], customerName);
                            NotifyError(errorMessage);
                            return View(Enumerable.Empty<OrderListVM>().ToPagedList(pageNumber, pageSize));
                        }

                        var orderListVM = result.Data.Select(order => order.Adapt<OrderListVM>()).ToList();
                        return View(orderListVM.ToPagedList(pageNumber, pageSize));
                    }

                    // Şirket bazlı filtreleme
                    if (company_Id.HasValue)
                    {
                        var result = await _orderService.GetOrdersListByCompanyIdAsync(company_Id.Value);
                        var companyResult = await _companyService.GetByIdAsync(company_Id.Value);

                        var companyName = string.IsNullOrWhiteSpace(companyResult?.Data?.Name)
                            ? _stringLocalizer["Unknown Company"]
                            : companyResult.Data.Name;

                        if (!result.IsSuccess || !result.Data?.Any() == true)
                        {
                            var errorMessage = string.Format(_stringLocalizer[Messages.ORDER_COMPANY_LIST_FAILED], companyName);
                            NotifyError(errorMessage);
                            return View(Enumerable.Empty<OrderListVM>().ToPagedList(pageNumber, pageSize));
                        }

                        var orderListVM = result.Data.Select(order => order.Adapt<OrderListVM>()).ToList();
                        return View(orderListVM.ToPagedList(pageNumber, pageSize));
                    }

                    // Genel sipariş listesi
                    var allOrdersResult = await _orderService.GetAllAsync(sortOrder);
                    if (!allOrdersResult.IsSuccess)
                    {
                        NotifyError(_stringLocalizer["ORDER_LIST_FAILED"]);
                        return View(Enumerable.Empty<OrderListVM>().ToPagedList(pageNumber, pageSize));
                    }

                    var allOrderListVM = allOrdersResult.Data.Select(order => order.Adapt<OrderListVM>()).ToList();
                    if (!string.IsNullOrWhiteSpace(searchQuery))
                    {
                        allOrderListVM = allOrderListVM
                            .Where(o =>
                                (o.CompanyName != null && o.CompanyName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                                 || (o.CustomerName != null && o.CustomerName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) 
                                
                            )
                            .ToList();
                    }
                    return View(allOrderListVM.ToPagedList(pageNumber, pageSize));
                }
            }
            catch (Exception ex)
            {
                NotifyError("An unexpected error occurred. Please try again later.");
                return View("Error");
            }
        }

        /// <summary>
        /// Yeni bir sipariş oluşturma sayfasını döndürür.
        /// </summary>
        /// <returns>Yeni bir sipariş oluşturma sayfası</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                NotifyError("Kullanıcı kimliği bulunamadı.");
                return RedirectToAction("Index");
            }

            var paymentType = await _paymentTypeService.GetAllAsync();
            if (!paymentType.IsSuccess)
            {
                NotifyError("Ödeme türleri getirilemedi.");
                return RedirectToAction("Index");
            }

            List<CompanyDTO> companies = new();

            if (User.IsInRole("Manager"))
            {
                var userResult = await _employeeService.GetByIdentityIdAsync(userId);
                if (!userResult.IsSuccess || userResult.Data == null || userResult.Data.CompanyId == null)
                {
                    NotifyError("Şirket ID bulunamadı. Lütfen sistem yöneticinize başvurun.");
                    return RedirectToAction("Index");
                }

                var companyId = userResult.Data.CompanyId;
                var orderCreateVM = new OrderCreateVM
                {
                    Products = (await _productService.GetProductsByCompanyIdAsync(companyId)).Data,
                    Employees =(await  _employeeService.GetByCompanyIdAsync(companyId)).Data,
                    Customers = (await _customerService.GetCustomersByCompanyId(companyId, "date")).Data,
                    Companies = new List<CompanyDTO> { new CompanyDTO { Id = companyId } },
                    PaymentTypes = paymentType.Data
                };

                return View(orderCreateVM);
            }
            else
            {
                var companyResult = await _companyService.GetAllAsync();
                if (companyResult.IsSuccess)
                {
                    companies = companyResult.Data?.Adapt<List<CompanyDTO>>() ?? new List<CompanyDTO>();
                }

                var orderCreateVM = new OrderCreateVM
                {
                    Products = (await _productService.GetAllAsync()).Data,
                    Customers = (await _customerService.GetAllAsync()).Data,
                    Employees = (await _employeeService.GetAllAsync()).Data, // Tüm çalışanları ekleyelim
                    Companies = companies,
                    PaymentTypes = paymentType.Data
                };

                return View(orderCreateVM);
            }
        }


        /// <summary>
        /// Siparişi oluşturan adminin bilgileriyle birlikte bir siparişi oluşturur ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="orderCreateVM">Oluşturulacak siparişin verileri</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateVM orderCreateVM)
        {
            if (!ModelState.IsValid)
            {
                NotifyError("Formda eksik veya hatalı bilgiler var.");
                return View(orderCreateVM);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                NotifyError("Kullanıcı kimliği bulunamadı.");
                return RedirectToAction("Index");
            }

            Guid? companyId = null;

            if (User.IsInRole("Manager"))
            {
                var userResult = await _employeeService.GetByIdentityIdAsync(userId);
                if (!userResult.IsSuccess || userResult.Data == null || userResult.Data.CompanyId == null)
                {
                    NotifyError("Şirket ID bulunamadı. Lütfen sistem yöneticinize başvurun.");
                    return RedirectToAction("Create");
                }

                companyId = userResult.Data.CompanyId;
            }
            else
            {
                companyId = orderCreateVM.CompanyId;
            }

            if (companyId == null)
            {
                NotifyError("Şirket ID eksik. Lütfen tekrar deneyin.");
                return RedirectToAction("Create");
            }

            try
            {
                var orderCreateDTO = orderCreateVM.Adapt<OrderCreateDTO>();
                orderCreateDTO.Id = Guid.NewGuid();
                orderCreateDTO.CompanyId = companyId.Value;

                var result = await _orderService.AddAsync(orderCreateDTO, userId);

                if (result.IsSuccess)
                {
                    NotifySuccess("Sipariş başarıyla oluşturuldu.");
                    return RedirectToAction("Index");
                }
                else
                {
                    NotifyError("Sipariş oluşturulamadı: " + result.Message);
                    return View(orderCreateVM);
                }
            }
            catch (Exception ex)
            {
                NotifyError("Sipariş oluşturulurken bir hata meydana geldi: " + ex.ToString());
                return View(orderCreateVM);
            }
        }

        /// <summary>
        /// Ürün silinmiş olsa bile belirtilen siparişin detaylarını gösterir. 
        /// </summary>
        /// <param name="orderId">Gösterilecek siparişin ID'si</param>
        /// <returns>Sipariş detaylarının görüntülendiği sayfa</returns>
        public async Task<IActionResult> Details(Guid orderId)
        {
            try
            {
                // Kullanıcı bilgilerini ClaimsPrincipal üzerinden alıyoruz.
                var currentUser = User;

                // Kullanıcının "Manager" rolünde olup olmadığını kontrol ediyoruz.
                bool isManager = currentUser.IsInRole("Manager");

                // Eğer kullanıcı bir manager ise, sadece kendi şirketine ait siparişin detaylarını alıyoruz.
                if (isManager)
                {
                    // Kullanıcının ID'sini alıyoruz (Claim üzerinden).
                    var userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
                    // Eğer userId geçerli değilse hata mesajı veriyoruz ve hata sayfasına yönlendiriyoruz.
                    if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                    {
                        NotifyError("User ID is missing or invalid.");
                        return View("Error");
                    }

                    // EmployeeService üzerinden şirket bilgilerini alıyoruz.
                    var companyResult = await _employeeService.GetCompanyIdByUserIdAsync(userGuid);
                    // Eğer şirket bilgisi alınamazsa, hata mesajı gösteriyoruz.
                    if (!companyResult.IsSuccess || companyResult.Data == null)
                    {
                        NotifyError("Failed to retrieve company ID. Please ensure your profile is linked to a company.");
                        return View("Error");
                    }

                    // Şirket ID'sini alıyoruz.
                    var companyId = companyResult.Data;

                    // Eğer geçerli bir companyId varsa, sadece bu şirkete ait siparişleri getiriyoruz.
                    if (companyId.HasValue)
                    {
                        // Sipariş listelerini şirket ID'sine göre alıyoruz.
                        var result = await _orderService.GetOrdersListByCompanyIdAsync(companyId.Value);
                        // Belirtilen orderId'ye sahip siparişi buluyoruz.
                        var order = result.Data?.FirstOrDefault(o => o.Id == orderId);

                        // Sipariş bulunamazsa, kullanıcıyı hata sayfasına yönlendiriyoruz.
                        if (order == null)
                        {
                            NotifyError(_stringLocalizer["ORDER_NOT_FOUND"]);
                            return RedirectToAction("Index");
                        }

                        // Siparişin detaylarını OrderDetailsVM modeline dönüştürüyoruz.
                        var orderDetailsVM = order.Adapt<OrderDetailsVM>();

                        // Deleted olmayan sipariş detaylarını filtreliyoruz.
                        orderDetailsVM.OrderDetails = orderDetailsVM.OrderDetails
                            .Where(od => od.Status != Status.Deleted).ToList();

                        // Kullanıcıya başarı mesajı gösteriyoruz ve siparişin detay sayfasını döndürüyoruz.
                        NotifySuccess(_stringLocalizer["ORDER_FOUND_SUCCESS"]);
                        return View(orderDetailsVM);
                    }
                    else
                    {
                        // Eğer companyId geçerli değilse, hata mesajı gösteriyoruz.
                        NotifyError("Company ID is not available.");
                        return View("Error");
                    }
                }

                // Eğer kullanıcı "Manager" değilse, genel sipariş detaylarını alıyoruz.
                var resultDetails = await _orderService.GetOrderWithDetailsByIdAsync(orderId);
                // Eğer sipariş bulunamazsa, kullanıcıyı hata sayfasına yönlendiriyoruz.
                if (!resultDetails.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_NOT_FOUND"]);
                    return RedirectToAction("Index");
                }

                // Siparişin admin bilgilerini alıyoruz.
                var adminDetailsResult = await _adminService.GetByIdAsync(resultDetails.Data.AdminId);
                // Eğer admin bilgisi mevcut değilse, "Bilinmeyen Admin" olarak varsayıyoruz.
                var adminDTODetails = adminDetailsResult?.Data ?? new AdminDTO { FirstName = "Bilinmeyen", LastName = "Admin" };

                // Siparişin detaylarını OrderDetailsVM modeline dönüştürüyoruz.
                var orderDetailsVMGeneral = resultDetails.Data.Adapt<OrderDetailsVM>();
                orderDetailsVMGeneral.Admin = adminDTODetails;

                // Deleted olmayan sipariş detaylarını filtreliyoruz.
                orderDetailsVMGeneral.OrderDetails = orderDetailsVMGeneral.OrderDetails
                    .Where(od => od.Status != Status.Deleted).ToList();

                // Kullanıcıya başarı mesajı gösteriyoruz ve genel siparişin detay sayfasını döndürüyoruz.
                NotifySuccess(_stringLocalizer["ORDER_FOUND_SUCCESS"]);
                return View(orderDetailsVMGeneral);
            }
            catch (Exception ex)
            {
                // Eğer bir hata oluşursa, hata mesajını detaylı şekilde kullanıcıya gösteriyoruz.
                var detailedMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                NotifyError($"An error occurred: {detailedMessage}");
                return View("Error");
            }
        }

        /// <summary>
        /// Belirtilen siparişin güncelleme sayfasını gösterir.
        /// </summary>
        /// <param name="orderId">Güncellenecek siparişin ID'si</param>
        /// <returns>Sipariş güncelleme sayfası</returns>
        public async Task<IActionResult> Update(Guid orderId)
        {
            // Sisteme giriş yapan kullanıcıyı alıyoruz.
            var currentUser = User;
            Guid? managerCompanyId = null;


            if (currentUser.IsInRole("Manager"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userResult = await _employeeService.GetByIdentityIdAsync(userId);

                if (userResult.IsSuccess && userResult.Data != null)
                {
                    managerCompanyId = userResult.Data.CompanyId;
                }
            }

            // Güncellenmek istenen siparişi getiriyoruz.
            var result = await _orderService.GetByIdAsync(orderId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_GETBYID_ERROR]);
                return RedirectToAction("Index");
            }

            // Eğer kullanıcı Manager rolündeyse ve kendi şirketindeki siparişi güncelleyemiyorsa yetkisiz erişim hatası veriyoruz.
            if (currentUser.IsInRole("Manager") && result.Data.CompanyId != managerCompanyId)
            {
                NotifyError(_stringLocalizer["Başka bir şirketin siparişini güncelleyemezsiniz!"]);
                return RedirectToAction("Index");
            }

            // Admin kullanıcı içinse tüm siparişleri getiriyoruz.
            var companiesResult = await _companyService.GetAllAsync();
            if (!companiesResult.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_LISTED_ERROR]);
                return RedirectToAction("Index");
            }

            var orderUpdateVM = result.Data.Adapt<OrderUpdateVM>();

            // Deleted olmayan order detail'leri filtrele
            orderUpdateVM.OrderDetails = orderUpdateVM.OrderDetails
                .Where(od => od.Status != Core.Enums.Status.Deleted).ToList();

            foreach (var orderDetail in orderUpdateVM.OrderDetails)
            {
                var product = (await _productService.GetByIdAsync(orderDetail.ProductId)).Data;
                orderDetail.ProductName = product.Name;
            }

            orderUpdateVM.Products = (await _productService.GetAllAsync()).Data;

            return View(orderUpdateVM);
        }

        /// <summary>
        ///  Sipariş bilgilerini günceller.
        /// </summary>
        /// <param name="orderUpdateVM"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(OrderUpdateVM orderUpdateVM)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(_stringLocalizer["ORDER_UPDATE_FAILED"]);
                orderUpdateVM.Products = (await _productService.GetAllAsync()).Data ?? new List<ProductListDTO>();
                return View(orderUpdateVM);
            }

            // Sipariş detaylarını birleştir
            orderUpdateVM.OrderDetails = orderUpdateVM.OrderDetails
                .GroupBy(od => (od.ProductId, od.Discount))
                .Select(g => new OrderDetailUpdateDTO
                {
                    Id = g.First().Id,
                    ProductId = g.Key.ProductId,
                    Quantity = g.Sum(od => od.Quantity),
                    UnitPrice = g.First().UnitPrice,
                    Discount = g.First().Discount,
                    TotalPrice = g.Sum(od => od.Quantity * g.First().UnitPrice * ((100 - g.First().Discount) / 100))
                })
                .ToList();

            //Ürünlerin stok kontrolleri
            var stockCheckResult = await _stockService.CheckStockAvailabilityAsync(orderUpdateVM.OrderDetails, orderUpdateVM.Id);
            if (!stockCheckResult.IsSuccess)
            {
                NotifyError(_stringLocalizer["STOCK_NOT_FOUND"]);
                return View(orderUpdateVM);
            }

            //Stok güncelleme
            var stockUpdateResult = await _stockService.UpdateStockAsync(orderUpdateVM.OrderDetails, orderUpdateVM.Id);
            if (!stockUpdateResult.IsSuccess)
            {
                NotifyError(_stringLocalizer["STOCK_UPDATE_FAILED"]);
            }

            var orderUpdateDTO = orderUpdateVM.Adapt<OrderUpdateDTO>();

            var result = await _orderService.UpdateAsync(orderUpdateDTO);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer["ORDER_UPDATE_FAILED"]);
                orderUpdateVM.Products = (await _productService.GetAllAsync()).Data ?? new List<ProductListDTO>();
                return View(orderUpdateVM);
            }

            NotifySuccess(_stringLocalizer["ORDER_UPDATE_SUCCESS"]);
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Belirtilen ID'li siparişi siler ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="orderId">Silinecek siparişin ID'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        public async Task<IActionResult> Delete(Guid orderId)
        {
            try
            {
                var stockUpdateResult = await _stockService.DeleteStockAsync(orderId);
                if (!stockUpdateResult.IsSuccess)
                {
                    NotifyError(_stringLocalizer["STOCK_UPDATE_FAILED"]);
                }
                var result = await _orderService.DeleteAsync(orderId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_DELETE_FAILED"]);
                    //NotifyError(result.Message);
                }
                else
                {
                    NotifySuccess(_stringLocalizer["ORDER_DELETED_SUCCESS"]);
                    //NotifySuccess(result.Message);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer["ORDER_DELETE_FAILED"] + ": " + ex.Message);
                //NotifyError(ex.Message);
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetProductsByCompanyId(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                return Json(new List<object>()); // Eğer geçerli bir ID gelmezse boş liste döndür
            }

            var result = await _productService.GetProductsByCompanyIdAsync(companyId);
            if (!result.IsSuccess)
            {
                return Json(new List<object>());
            }

            var products = result.Data
                .Select(p => new
                {
                    p.Id,
                    p.Name
                })
                .ToList();

            return Json(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesByCompanyId(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                return Json(new List<object>()); // Eğer geçerli bir ID gelmezse boş liste döndür
            }

            var result = await _employeeService.GetByCompanyIdAsync(companyId);
            if (!result.IsSuccess)
            {
                return Json(new List<object>());
            }

            var employees = result.Data
                .Select(e => new
                {
                    e.Id,
                    e.FirstName,
                    e.LastName
                })
                .ToList();

            return Json(employees);
        }


    }
}
