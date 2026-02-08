using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.MVC.Models.AdminVMs;
using BaSalesManagementApp.Business.Utilities;
using X.PagedList;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Mapster;

namespace BaSalesManagementApp.MVC.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmployeeService _employeeService;
        private readonly IOrderService _orderService;

        public AdminController(
            IAdminService adminService,
            IWebHostEnvironment webHostEnvironment,
            IStringLocalizer<Resource> stringLocalizer,
            UserManager<IdentityUser> userManager,
            IEmployeeService employeeService,
            IOrderService orderService)
        {
            _adminService = adminService;
            _webHostEnvironment = webHostEnvironment;
            _stringLocalizer = stringLocalizer;
            _userManager = userManager;
            _employeeService = employeeService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(
            int? page,
            string sortAdmin = "name",
            int pageSize = 10,
            string searchString = null)
        {
            try
            {
                // 👤 Giriş yapan kullanıcıyı al
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                    return RedirectToAction("Login", "Account");

                // 👮 Manager ise Home/Index'e yönlendir
                if (await _userManager.IsInRoleAsync(currentUser, "Manager"))
                    return RedirectToAction("Index", "Home");

                // 👔 Çalışan rolünde mi kontrol et
                bool isEmployee = await _userManager.IsInRoleAsync(currentUser, "Employee");
                ViewBag.IsEmployee = isEmployee;

                // 📄 Sayfa numarası
                int pageNumber = page ?? 1;

                // ⚙️ Server-side sayfalama, filtreleme ve sıralama (performanslı)
                var (items, total) = await _adminService.GetPagedAsync(
                    search: searchString,
                    sort: sortAdmin,
                    page: pageNumber,
                    pageSize: pageSize
                );

                // DTO → VM dönüştür
                var adminListVM = items.Adapt<List<AdminListVM>>();

                // 📊 Dashboard verilerini hazırla (servis üzerinden)
                var adminDashboard = new AdminDashboardVM
                {
                    TotalAdmins = await _adminService.CountAsync(),
                    NewAdminsThisMonth = await _adminService.CountNewThisMonthAsync(),
                    TotalEmployees = (await _employeeService.GetAllAsync())?.Data?.Count ?? 0,
                    TotalOrders = (await _orderService.GetAllAsync())?.Data?.Count ?? 0,
                    TotalOrderAmount = (await _orderService.GetAllAsync())?.Data?.Sum(o => o.TotalPrice) ?? 0m
                };

                ViewBag.AdminDashboard = adminDashboard;

                // 📦 StaticPagedList → yalnızca o sayfa belleğe alınır
                var paginatedList = new StaticPagedList<AdminListVM>(adminListVM, pageNumber, pageSize, total);

                // Görünüm için ViewData’lar
                ViewData["CurrentSortAdmin"] = sortAdmin;
                ViewData["CurrentPage"] = pageNumber;
                ViewData["CurrentPageSize"] = pageSize;
                ViewData["CurrentFilter"] = searchString;

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.ADMIN_LISTED_ERROR] + ": " + ex.Message);
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
                NotifyError(_stringLocalizer[Messages.ADMIN_CREATE_ERROR] + ": " + ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateVM adminCreateVM)
        {
            if (ModelState.IsValid)
            {
                adminCreateVM.FirstName = StringUtilities.CapitalizeEachWord(adminCreateVM.FirstName);
                adminCreateVM.LastName = StringUtilities.CapitalizeFirstLetter(adminCreateVM.LastName);

                var adminDto = adminCreateVM.Adapt<AdminCreateDTO>();

                byte[] photoBytes = null;

                if (adminCreateVM.Photo != null && adminCreateVM.Photo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await adminCreateVM.Photo.CopyToAsync(memoryStream);
                        photoBytes = memoryStream.ToArray();
                    }
                }
                else
                {
                    adminDto.PhotoData = null;
                }

                if (adminCreateVM.Photo != null)
                {
                    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(adminCreateVM.Photo.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Photo", _stringLocalizer["Invalid file type. Please upload an image file."]);
                        return View(adminCreateVM);
                    }
                }

                adminDto.PhotoData = photoBytes;

                var result = await _adminService.AddAsync(adminDto);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.ADMIN_CREATE_ERROR]);
                    return View(adminCreateVM);
                }
                else
                {
                    NotifySuccess(_stringLocalizer[Messages.ADMIN_CREATED_SUCCESS]);
                    return RedirectToAction("Index", "Admin");
                }
            }
            else
                return View(adminCreateVM);
        }

        public async Task<IActionResult> Delete(Guid adminId)
        {
            try
            {
                var result = await _adminService.DeleteAsync(adminId);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.ADMIN_DELETE_ERROR]);
                    // NotifyError(result.Message);
                }
                else
                {
                    NotifySuccess(_stringLocalizer[Messages.ADMIN_DELETED_SUCCESS]);
                    // NotifySuccess(result.Message);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.ADMIN_DELETE_ERROR] + ": " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Update(Guid adminId)
        {
            var result = await _adminService.GetByIdAsync(adminId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.ADMIN_GETBYID_ERROR]);
                return RedirectToAction("Index");
            }

            var adminUpdateVM = result.Data.Adapt<AdminUpdateVM>();

            if (adminUpdateVM.PhotoData != null)
            {
                string base64 = Convert.ToBase64String(adminUpdateVM.PhotoData);
                adminUpdateVM.PhotoUrl = $"data:image/png;base64,{base64}";
            }

            return View(adminUpdateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AdminUpdateVM adminUpdateVM)
        {
            if (ModelState.IsValid)
            {
                var adminUpdateDto = adminUpdateVM.Adapt<AdminUpdateDTO>();

                // Yeni fotoğraf yükleme kontrolü
                if (adminUpdateVM.Photo != null && adminUpdateVM.Photo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await adminUpdateVM.Photo.CopyToAsync(memoryStream);
                        adminUpdateDto.PhotoData = memoryStream.ToArray();
                    }
                }

                // Admin güncelleme işlemi
                var result = await _adminService.UpdateAsync(adminUpdateDto);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.ADMIN_UPDATE_ERROR]);
                    return View(adminUpdateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.ADMIN_UPDATE_SUCCESS]);
                return RedirectToAction("Index");
            }

            return View(adminUpdateVM);
        }

        public async Task<IActionResult> Details(Guid adminId)
        {
            var result = await _adminService.GetByIdAsync(adminId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.ADMIN_GETBYID_ERROR]);
                return RedirectToAction("Index");
            }

            NotifySuccess(_stringLocalizer[Messages.ADMIN_GETBYID_SUCCESS]);
            return View(result.Data.Adapt<AdminDetailsVM>());
        }
    }
}