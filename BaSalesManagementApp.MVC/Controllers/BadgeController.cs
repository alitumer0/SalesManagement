using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Core.Utilities.Results;
using BaSalesManagementApp.Dtos.BadgeDTOs;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.BadgeVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    [Authorize(Roles ="Admin,Manager,Employee")]
    public class BadgeController : BaseController
    {
        private readonly IBadgeService _badgeService;
        private readonly ICompanyService _companyService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly IEmployeeService _employeeService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BadgeController(IBadgeService badgeService, ICompanyService companyService, IStringLocalizer<Resource> stringLocalizer, IEmployeeService employeeService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _badgeService = badgeService;
            _companyService = companyService;
            _stringLocalizer = stringLocalizer;
            _employeeService = employeeService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int? page, string sortOrder, int pageSize = 10, Guid? companyIdWithDropDown = null)
        {
            int pageNumber = page ?? 1;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentPageSize = pageSize;

            //kullanıcının bilgilerini bir kere çek hep çekme (performans iyileştirmesi)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isEmployee = User.IsInRole("Employee");
            var isManager = User.IsInRole("Manager");

            // employee veya manager ise company idyi bir kere çek
            Guid? userCompanyId = null;
            if ((isEmployee || isManager) && !string.IsNullOrEmpty(userId))
            {
                var companyIdStr = await _employeeService.GetCompanyIdByUserIdAsync(userId);
                userCompanyId = Guid.Parse(companyIdStr);
            }

            //manager için gereksiz company listesi çekme (performans iyileştirmesi)
            if (!isManager)
            {
                var companiesResult = await _companyService.GetAllAsync();
                ViewBag.Companies = companiesResult.IsSuccess
                     ? companiesResult.Data.Adapt<List<CompanyDTO>>()
                     : new List<CompanyDTO>();
            }
            ViewData["SelectedCompanyId"] = companyIdWithDropDown;

            // badgeleri önce filtreleyip sonra çektirdik (performans iyileştirmesi)
            List<BadgeListVM> badgeListVMs;

            //Eğer rol Employee ise kullanıcının şirketine ait rozetleri listeleyecek
            if (isEmployee && userCompanyId.HasValue)
            {
                var badgeResult = await _badgeService.GetBadgesByCompanyIdAsync(userCompanyId.Value);
                badgeListVMs = badgeResult.Adapt<List<BadgeListVM>>();
            }
            //Sistemeki kullanıcının rolünün manager olup olmadığını kontrol eder
            else if (isManager && userCompanyId.HasValue)
            {
                var badgeResult = await _badgeService.GetBadgesByCompanyIdAsync(userCompanyId.Value);
                badgeListVMs = badgeResult.Adapt<List<BadgeListVM>>();
            }
            else
            {
                IDataResult<List<BadgeListDTO>> result;

                if (companyIdWithDropDown.HasValue)
                {
                    result = await _badgeService.GetBadgesByCompanyIdAsynca(companyIdWithDropDown.Value);
                }
                else
                {
                    result = await _badgeService.GetAllAsync();
                }

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.BADGE_LISTED_ERROR]);
                    return View(Enumerable.Empty<BadgeListVM>().ToPagedList(pageNumber, pageSize));
                }

                badgeListVMs = result.Data.Adapt<List<BadgeListVM>>();
            }

            // Eğer seçilen şirketin hiç rozeti yoksa hata ver
            if (badgeListVMs == null || !badgeListVMs.Any())
            {
                NotifyError(_stringLocalizer[Messages.BADGE_LISTED_NOTFOUND]);
                return View(Enumerable.Empty<BadgeListVM>().ToPagedList(pageNumber, pageSize));
            }

            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["CurrentPageSize"] = pageSize;

            switch (sortOrder)
            {
                case "name_asc":
                    badgeListVMs = badgeListVMs.OrderBy(x => x.Name).ToList();
                    break;
                case "name_desc":
                    badgeListVMs = badgeListVMs.OrderByDescending(x => x.Name).ToList();
                    break;
                case "date_asc":
                    badgeListVMs = badgeListVMs.OrderBy(x => x.CreatedDate).ToList();
                    break;
                case "date_desc":
                    badgeListVMs = badgeListVMs.OrderByDescending(x => x.CreatedDate).ToList();
                    break;
                default:
                    badgeListVMs = badgeListVMs.OrderBy(x => x.Name).ToList();
                    break;
            }

            var paginatedList = badgeListVMs.ToPagedList(pageNumber, pageSize);

            return View(paginatedList);
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            List<Company> companies;

            //Giriş yapan kişinin rolü Manager ise if bloğu içerisinde ilgili işlemler:
            if (user != null && await _userManager.IsInRoleAsync(user, "Manager"))
            {
                var companyId = await _employeeService.GetCompanyIdByUserIdAsync(user.Id);
                var companyResult = await _companyService.GetByIdAsync(Guid.Parse(companyId));
                companies = new List<Company> { companyResult.Data.Adapt<Company>() };
            }
            else
            {
                var badgeResult = await _companyService.GetAllAsync();
                companies = badgeResult.Data.Adapt<List<Company>>();
            }

            var model = new BadgeCreateVM()
            {
                Companies = companies
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(BadgeCreateVM badgeCreateVM)
        {

            //badgeCreateVM.Name=StringUtilities.CapitalizeFirstLetter(badgeCreateVM.Name);
            badgeCreateVM.Name = StringUtilities.CapitalizeEachWord(badgeCreateVM.Name);
            

            var result = await _badgeService.AddAsync(badgeCreateVM.Adapt<BadgeCreateDTO>());

            if (!result.IsSuccess)
            {
                //NotifyError(_stringLocalizer[Messages.BADGE_ADD_ERROR]);
                NotifyError(result.Message);
                return View(badgeCreateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.BADGE_ADD_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Details(Guid badgeId)
        {
            var result = await _badgeService.GetByIdAsync(badgeId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BADGE_GETBYID_ERROR]);
                // NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            NotifySuccess(_stringLocalizer[Messages.BADGE_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            var badgeDetailsVM = result.Data.Adapt<BadgeDetailsVM>();

            return View(badgeDetailsVM);
        }


        public async Task<IActionResult> Update(Guid badgeId)
        {
            var user = await _userManager.GetUserAsync(User);
            Badge badge = null;
            List<Company> company;

            //Sistemdeki kullanıcı rolü Manager ise blok içerisindeki işlemler:
            if (user != null && await _userManager.IsInRoleAsync(user, "Manager"))
            {
                var companyId = await _employeeService.GetCompanyIdByUserIdAsync(user.Id);
                var companyResult = await _companyService.GetByIdAsync(Guid.Parse(companyId));
                var badgeResult = await _badgeService.GetByIdAsync(badgeId);

                company = new List<Company> { companyResult.Data.Adapt<Company>() };
                badge = badgeResult.Data.Adapt<Badge>();
            }
            else
            {
                var badgeResult = await _badgeService.GetByIdAsync(badgeId);
                var companyResult = await _companyService.GetAllAsync();

                if (!badgeResult.IsSuccess || !companyResult.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.BADGE_UPDATE_ERROR]);
                    return RedirectToAction("Index");
                }
                badge = badgeResult.Data.Adapt<Badge>();
                company = companyResult.Data.Adapt<List<Company>>();
            }
            var badgeUpdateVM = badge.Adapt<BadgeUpdateVM>();
            badgeUpdateVM.Companies = company;

            NotifySuccess(_stringLocalizer[Messages.BADGE_UPDATE_SUCCESS]);
            return View(badgeUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(BadgeUpdateVM badgeUpdateVM)
        {
            badgeUpdateVM.Name = StringUtilities.CapitalizeFirstLetter(badgeUpdateVM.Name);
           

            var result = await _badgeService.UpdateAsync(badgeUpdateVM.Adapt<BadgeUpdateDTO>());

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BADGE_UPDATE_ERROR]);
                // NotifyError(result.Message);
                return View(badgeUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.BADGE_UPDATE_SUCCESS]);
            // NotifySuccess(result.Message);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid badgeId)
        {
            //bool hasWarehouse = await _branchService.HasWarehouseAsync(branchId);
            //if (hasWarehouse)
            //{
            //    NotifyError("Bu şubenin deposu var, silme işlemi yapılamaz.");
            //    return RedirectToAction("Index");
            //}

            var result = await _badgeService.DeleteAsync(badgeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BADGE_DELETE_ERROR]);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.BADGE_DELETE_SUCCESS]);
            }

            return RedirectToAction("Index");
        }
    }
}
