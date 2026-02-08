using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.ProductTypeDtos;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.PaymentTypeVMs;
using BaSalesManagementApp.MVC.Models.ProductTypeVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    [Authorize(Roles ="Manager,Admin")]
    public class ProductTypeController : BaseController
    {
        private readonly IStockTypeService productTypeService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly ICategoryService categoryService;
        private readonly IEmployeeService _employeeService;
        private readonly IProductService _productService;

        public ProductTypeController(IStockTypeService productTypeService, IStringLocalizer<Resource> stringLocalizer, ICategoryService categoryService, IEmployeeService employeeService, IProductService productService)
        {
            this.productTypeService = productTypeService;
            _stringLocalizer = stringLocalizer;
            this.categoryService = categoryService;
            _employeeService = employeeService;
            _productService = productService;
        }
        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical", Guid? categoryId = null, int pageSize = 10, string searchQuery = null)
        {

            int pageNumber = page ?? 1;

            var categoryResult = await categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categoryResult.Data, "Id", "Name", categoryId);

            ViewData["CurrentPageSize"] = pageSize;
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentCategoryId"] = categoryId;

            // Kategori ismini al
            string selectedCategoryName = categoryResult.Data.FirstOrDefault(c => c.Id == categoryId)?.Name;

            if (User.IsInRole("Manager"))
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);
                var products = await _productService.GetProductsByCompanyIdAsync(Guid.Parse(companyId));
                var categoryListResult = await categoryService.GetCategoriesByProductListAsyncs(products);

                var categoryEntities = categoryListResult.Data?.Adapt<List<Category>>() ?? new List<Category>();
                var filteredCategories = categoryEntities;

                if (categoryId.HasValue)
                {
                    filteredCategories = categoryEntities.Where(c => c.Id == categoryId.Value).ToList();
                }

                var result = await productTypeService.GetProductTypeListByCategoryListAsyncs(filteredCategories);
                var dtolist = result.Data;
                var vmList = dtolist.Adapt<List<ProductTypeListVM>>();

                if (categoryId.HasValue && !string.IsNullOrEmpty(selectedCategoryName))
                {
                    vmList = vmList.Where(x => x.CategoryName == selectedCategoryName).ToList();
                }

                return View(vmList.ToPagedList(pageNumber, pageSize));
            }

            
            var allResult = await productTypeService.GetAllAsync(sortOrder);
            var allVmList = allResult.Data.Adapt<List<ProductTypeListVM>>();

            
            allVmList = ApplySorting(allVmList, sortOrder);

            if (categoryId.HasValue && !string.IsNullOrEmpty(selectedCategoryName))
            {
                allVmList = allVmList.Where(x => x.CategoryName == selectedCategoryName).ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                allVmList = allVmList
                    .Where(x => x.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewData["CurrentSearchQuery"] = searchQuery;

            return View(allVmList.ToPagedList(pageNumber, pageSize));
        }


        // Sıralama işlemini yapan metod
        private List<ProductTypeListVM> ApplySorting(List<ProductTypeListVM> list, string sortOrder)
        {
            return sortOrder switch
            {
                "alphabetical" => list.OrderBy(x => x.Name).ToList(),
                "alphabeticaldesc" => list.OrderByDescending(x => x.Name).ToList(),
                "category" => list.OrderBy(x => x.CategoryName).ToList(),
                "categorydesc" => list.OrderByDescending(x => x.CategoryName).ToList(),
                "date" => list.OrderByDescending(x => x.CreatedDate).ToList(), // EN YENİLER ÖNCE
                "datedesc" => list.OrderBy(x => x.CreatedDate).ToList(),       // EN ESKİLER ÖNCE
                _ => list
            };
        }






        [HttpGet("ProductType/Detail/{productTypeId}")]
        public async Task<IActionResult> Detail(Guid productTypeId)
        {

            var result = await productTypeService.GetByAsync(productTypeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_UNSUCCESS]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            var productTypeDetailVM = result.Data.Adapt<ProductTypeDetailVM>();
            return View(productTypeDetailVM);
        }

        public async Task<IActionResult> Add()
        {

            if (User.IsInRole("Manager"))
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;

                    var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);

                    var productList = await _productService.GetProductsByCompanyIdAsync(Guid.Parse(companyId));

                    var categoryList = await categoryService.GetCategoriesByProductListAsync(productList);

                    var categories = categoryList.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

                    var productTypeCreateVM = new ProductTypeAddVM
                    {
                        Categories = categories
                    };

                    return View(productTypeCreateVM);
                }
                else
                {
                    NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_ADD_UNSUCCESS]);
                    return RedirectToAction("Index");
                }

            }
            else
            {
                var categoryResult = await categoryService.GetAllAsync();

                var categories = categoryResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

                var productTypeCreateVM = new ProductTypeAddVM
                {
                    Categories = categories
                };

                return View(productTypeCreateVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductTypeAddVM productTypeAddVM)
        {

            var categoryResult = await categoryService.GetAllAsync();

            productTypeAddVM.Categories = categoryResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            if (!ModelState.IsValid) return View(productTypeAddVM);

            var currentCultureCode = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            CultureInfo currentCulture = CultureInfo.GetCultureInfo(currentCultureCode);

            string productName = productTypeAddVM.Name?
                .Trim()
                .Replace(" ", "")
                .ToUpper(currentCulture);

            string categoryName = categoryResult.Data
                .FirstOrDefault(c => c.Id == productTypeAddVM.CategoryId)?
                .Name?
                .Trim()
                .Replace(" ", "")
                .ToUpper(currentCulture);

            if (productName == categoryName)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_CATEGORY_NAME_CONFLICT]);
                return View(productTypeAddVM);
            }

            productTypeAddVM.Name = StringUtilities.CapitalizeEachWord(productTypeAddVM.Name);

            // Aynı isimde ürün tipi var mı kontrol et
            if (!string.IsNullOrWhiteSpace(productTypeAddVM.Name) && await productTypeService.IsStockTypeExistsAsync(productTypeAddVM.Name))
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_EXISTS]);
                return View(productTypeAddVM);
            }

            var result = await productTypeService.AddAsync(productTypeAddVM.Adapt<StockTypeAddDto>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_ADD_UNSUCCESS]);
                //NotifyError(result.Message);
                return View(productTypeAddVM);
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_ADD_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        [HttpGet("ProductType/Delete/{productTypeId}")]
        public async Task<IActionResult> Delete(Guid productTypeId)
        {

            var result = await productTypeService.DeleteAsync(productTypeId);

            if (result.IsSuccess)
            {
                NotifySuccess(_stringLocalizer[result.Message]);

            }
            else
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_DELETE_UNSUCCESS]);
            }


            return RedirectToAction("Index");
        }

        [HttpGet("ProductType/Update/{productTypeId}")]
        public async Task<IActionResult> Update(Guid productTypeId)
        {

            var result = await productTypeService.GetByAsync(productTypeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_UNSUCCESS]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var categoriesResult = await categoryService.GetAllAsync();
            var categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            var productTypeEditVM = result.Data?.Adapt<ProductTypeUpdateVM>();

            productTypeEditVM.Categories = categories;

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            return View(productTypeEditVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductTypeUpdateVM productTypeUpdateVM)
        {

            var categoriesResult = await categoryService.GetAllAsync();

            productTypeUpdateVM.Categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            //if (!ModelState.IsValid) 
            //{

            //    return View(productTypeUpdateVM);

            //}

            productTypeUpdateVM.Name = StringUtilities.CapitalizeEachWord(productTypeUpdateVM.Name);

            var result = await productTypeService.UpdateAsync(productTypeUpdateVM.Adapt<StockTypeUpdateDto>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_UPDATE_UNSUCCESS]);
                //NotifyError(result.Message);
                return View(productTypeUpdateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_UPDATE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }
    }
}