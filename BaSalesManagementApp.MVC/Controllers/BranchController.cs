using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.BranchVMs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.ComponentModel.Design;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly IEmployeeService _employeeService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public BranchController(IBranchService branchService, ICompanyService companyService, IStringLocalizer<Resource> stringLocalizer, IEmployeeService employeeService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _branchService = branchService;
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

            var companiesResult = await _companyService.GetAllAsync();
            ViewBag.Companies = companiesResult.IsSuccess
                 ? companiesResult.Data.Adapt<List<CompanyDTO>>()
                 : new List<CompanyDTO>();
            ViewData["SelectedCompanyId"] = companyIdWithDropDown;

            var result = await _branchService.GetAllAsync();

            if (companyIdWithDropDown != null)
            {
                result = await _branchService.GetBranchesByCompanyIdAsynca(companyIdWithDropDown.Value);
            }

            var branchListVMs = result.Data.Adapt<List<BranchListVM>>();

            // Eğer seçilen şirketin hiç şubesi yoksa hata ver
            if (companyIdWithDropDown != null && !branchListVMs.Any())
            {
                NotifyError(_stringLocalizer[Messages.BRANCH_LISTED_NOTFOUND]);
                return View(Enumerable.Empty<BranchListVM>().ToPagedList(pageNumber, pageSize));
            }

            //Sistemeki kullanıcının rolünün manager olup olmadığını kontrol eder
            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                    var branchlist = await _branchService.GetBranchesByCompanyIdAsync(Guid.Parse(companyId));
                    // Şirket şubelerini branchListVMs'e aktar
                    branchListVMs = branchlist.Adapt<List<BranchListVM>>();
                }

                // Eğer Manager'a ait hiç şube yoksa hata ver
                if (!branchListVMs.Any())
                {
                    NotifyError(_stringLocalizer[Messages.BRANCH_LISTED_NOTFOUND]);
                    return View(Enumerable.Empty<BranchListVM>().ToPagedList(pageNumber, pageSize));
                }
            }
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["CurrentPageSize"] = pageSize; // Seçilen pageSize'ı sakla
            switch (sortOrder)
            {
                case "company_asc":
                    branchListVMs = branchListVMs.OrderBy(x => x.CompanyName).ToList();
                    break;
                case "company_desc":
                    branchListVMs = branchListVMs.OrderByDescending(x => x.CompanyName).ToList();
                    break;
                case "name_asc":
                    branchListVMs = branchListVMs.OrderBy(x => x.Name).ToList();
                    break;
                case "name_desc":
                    branchListVMs = branchListVMs.OrderByDescending(x => x.Name).ToList();
                    break;
                case "date_asc":
                    branchListVMs = branchListVMs.OrderBy(x => x.CreatedDate).ToList();
                    break;
                case "date_desc":
                    branchListVMs = branchListVMs.OrderByDescending(x => x.CreatedDate).ToList();
                    break;
                default:
                    branchListVMs = branchListVMs.OrderBy(x => x.Name).ToList();
                    break;
            }

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BRANCH_LISTED_ERROR]);
                return View(Enumerable.Empty<BranchListVM>().ToPagedList(pageNumber, pageSize));
            }
            var paginatedList = branchListVMs.Adapt<List<BranchListVM>>().ToPagedList(pageNumber, pageSize);

            return View(paginatedList);
        }


        public async Task<IActionResult> Details(Guid branchId)
        {
            var result = await _branchService.GetByIdAsync(branchId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BRANCH_GETBYID_ERROR]);
                // NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            NotifySuccess(_stringLocalizer[Messages.BRANCH_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            var branchDetailsVM = result.Data.Adapt<BranchDetailsVM>();

            return View(branchDetailsVM);
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
                var branchResult = await _companyService.GetAllAsync();
                companies = branchResult.Data.Adapt<List<Company>>();
            }

            var model = new BranchCreateVM()
            {
                Companies = companies
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BranchCreateVM branchCreateVM)
        {

            //branchCreateVM.Name=StringUtilities.CapitalizeFirstLetter(branchCreateVM.Name);
            branchCreateVM.Name = StringUtilities.CapitalizeEachWord(branchCreateVM.Name);
            branchCreateVM.Address=StringUtilities.CapitalizeEachWord(branchCreateVM.Address);

            var result = await _branchService.AddAsync(branchCreateVM.Adapt<BranchCreateDTO>());

            if (!result.IsSuccess)
            {
                //NotifyError(_stringLocalizer[Messages.BRANCH_ADD_ERROR]);
                NotifyError(result.Message);
                return View(branchCreateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.BRANCH_ADD_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(Guid branchId)
        {
            var user = await _userManager.GetUserAsync(User);
            Branch branch = null;
            List<Company> company;

            //Sistemdeki kullanıcı rolü Manager ise blok içerisindeki işlemler:
            if (user != null && await _userManager.IsInRoleAsync(user, "Manager"))
            {
                var companyId = await _employeeService.GetCompanyIdByUserIdAsync(user.Id);
                var companyResult = await _companyService.GetByIdAsync(Guid.Parse(companyId));
                var branchResult = await _branchService.GetByIdAsync(branchId);

                company = new List<Company> { companyResult.Data.Adapt<Company>() };
                branch = branchResult.Data.Adapt<Branch>();
            }
            else
            {
                var branchResult = await _branchService.GetByIdAsync(branchId);
                var companyResult = await _companyService.GetAllAsync();

                if (!branchResult.IsSuccess || !companyResult.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.BRANCH_UPDATE_ERROR]);
                    return RedirectToAction("Index");
                }
                branch = branchResult.Data.Adapt<Branch>();
                company = companyResult.Data.Adapt<List<Company>>();
            }
            var branchUpdateVM = branch.Adapt<BranchUpdateVM>();
            branchUpdateVM.Companies = company;

            NotifySuccess(_stringLocalizer[Messages.BRANCH_UPDATE_SUCCESS]);
            return View(branchUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(BranchUpdateVM branchUpdateVM)
        {
            branchUpdateVM.Name = StringUtilities.CapitalizeFirstLetter(branchUpdateVM.Name);
            branchUpdateVM.Address = StringUtilities.CapitalizeEachWord(branchUpdateVM.Address);

            var result = await _branchService.UpdateAsync(branchUpdateVM.Adapt<BranchUpdateDTO>());

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BRANCH_UPDATE_ERROR]);
                // NotifyError(result.Message);
                return View(branchUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.BRANCH_UPDATE_SUCCESS]);
            // NotifySuccess(result.Message);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid branchId)
        {
            bool hasWarehouse = await _branchService.HasWarehouseAsync(branchId);
            if (hasWarehouse)
            {
                NotifyError("Bu şubenin deposu var, silme işlemi yapılamaz.");
                return RedirectToAction("Index");
            }

            var result = await _branchService.DeleteAsync(branchId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BRANCH_DELETE_ERROR]);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.BRANCH_DELETE_SUCCESS]);
            }

            return RedirectToAction("Index");
        }



    }
}
