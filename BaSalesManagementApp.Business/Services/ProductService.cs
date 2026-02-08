using BaSalesManagementApp.Business.Interfaces;
using BaSalesManagementApp.DataAccess.EFCore.Repositories;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.StockDTOs;
using BaSalesManagementApp.Dtos.WarehouseDTOs;
using BaSalesManagementApp.Entites.DbSets;
using Mapster;
using Microsoft.Identity.Client;
using System.Security.Claims;
using X.PagedList;
using System.Linq;

namespace BaSalesManagementApp.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IQrService _qrService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IStockService _stockService;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmployeeService _employeeService; 

        public ProductService(IProductRepository productRepository, IQrService qrService, ICategoryRepository categoryRepository, ICompanyRepository companyRepository, IStockService stockService, IOrderRepository orderRepository,IEmployeeService employeeService)
        {
            _productRepository = productRepository;
            _qrService = qrService;
            _categoryRepository = categoryRepository;
            _companyRepository = companyRepository;
            _stockService = stockService;
            _orderRepository = orderRepository;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Ürünleri sıralama, sayfalama ve filtreleme yaparak getirir. Filtreleme ve sıralama işlemleri veritabanı seviyesinde gerçekleştirilir.
        /// </summary>
        /// <param name="sortOrder">Sıralama türü (örneğin, "alphabetical", "pricedesc").</param>
        /// <param name="page">Görüntülenecek sayfa numarası.</param>
        /// <param name="pageSize">Sayfa başına gösterilecek ürün sayısı.</param>
        /// <param name="categoryId">Filtreleme için kategori ID'si (opsiyonel).</param>
        /// <param name="companyId">Filtreleme için şirket ID'si (opsiyonel).</param>
        /// <param name="user">Kullanıcı bilgileri (örneğin, Manager rolü kontrolü için).</param>
        /// <returns>Sayfalanmış ürün listesini (IPagedList<ProductListDTO>) döndürür.</returns>
        public async Task<IPagedList<ProductListDTO>> GetProductsAsync(string sortOrder,int page,int pageSize,Guid? categoryId,Guid? companyId,ClaimsPrincipal user = null)
        {
            try
            {
                 var products = await _productRepository.GetAllAsync();

                // Filtreler
                if (categoryId.HasValue && categoryId.Value != Guid.Empty)
                    products = products.Where(p => p.CategoryId == categoryId.Value).ToList();

                if (companyId.HasValue && companyId.Value != Guid.Empty)
                {
                    products = products.Where(p => p.CompanyId == companyId.Value).ToList();
                }
                else if (user != null && user.IsInRole("Manager"))
                {
                    var uid = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(uid))
                    {
                        var companyIdString = await _employeeService.GetCompanyIdByUserIdAsync(uid);
                        if (!string.IsNullOrEmpty(companyIdString) && Guid.TryParse(companyIdString, out Guid managerCompanyId))
                            products = products.Where(p => p.CompanyId == managerCompanyId).ToList();
                    }
                }

                // var totalCount = products.Count; // HATA: IEnumerable için property yok
                var totalCount = products.Count(); // LINQ Count()

                // Sıralama
                switch ((sortOrder ?? "alphabetical").ToLowerInvariant())
                {
                    case "alphabetical":
                        products = products.OrderBy(p => p.Name).ToList();
                        break;
                    case "alphabeticaldesc":
                        products = products.OrderByDescending(p => p.Name).ToList();
                        break;
                    case "date":
                        products = products.OrderByDescending(p => p.CreatedDate).ToList();
                        break;
                    case "datedesc":
                        products = products.OrderBy(p => p.CreatedDate).ToList();
                        break;
                    case "priceascend":
                        products = products.OrderBy(p => p.Price).ToList();
                        break;
                    case "pricedesc":
                        products = products.OrderByDescending(p => p.Price).ToList();
                        break;
                    default:
                        products = products.OrderBy(p => p.Name).ToList();
                        break;
                }

                // Sayfalama (yalnızca sayfa verisi)
                var pageIndex = page < 1 ? 1 : page;
                var skip = (pageIndex - 1) * pageSize;
                var pageItems = products.Skip(skip).Take(pageSize).ToList();

                // Sadece sayfadaki ürünler için sözlükler
                var categoryDict = (await _categoryRepository.GetAllAsync())
                    .ToDictionary(x => x.Id, x => x.Name);

                var companyDict = (await _companyRepository.GetAllAsync())
                    .ToDictionary(x => x.Id, x => x.Name);

                // DTO map (yalnız pageItems)
                var dtoList = pageItems.Adapt<List<ProductListDTO>>();
                foreach (var dto in dtoList)
                {
                    dto.CategoryName = categoryDict.TryGetValue(dto.CategoryId, out var catName)
                        ? catName
                        : "Bilinmeyen Kategori";

                    dto.CompanyName = companyDict.TryGetValue(dto.CompanyId, out var compName)
                        ? compName
                        : Messages.PRODUCT_COMPANY_DELETED;
                }

                return new StaticPagedList<ProductListDTO>(dtoList, pageIndex, pageSize, totalCount);
            }
            catch
            {
                return new List<ProductListDTO>().ToPagedList(page, pageSize);
            }
        }



        /// <summary>
        /// Belirtilen ürün adı ve şirket kimliğine göre ürünü asenkron olarak getirir.
        /// </summary>
        /// <param name="name">Aranacak ürünün adı.</param>
        /// <param name="companyId">Ürünün ait olduğu şirketin kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucu, ürün bulunursa <see cref="ProductDTO"/> döndürür; aksi takdirde null döner.</returns>        
        public async Task<ProductDTO> GetByNameAndCompanyAsync(string name, Guid companyId)
        {
            var product = (await _productRepository.GetAllAsync())
                .Where(p => p.Name == name && p.CompanyId == companyId)
                .FirstOrDefault();

            return product?.Adapt<ProductDTO>(); // Product'u ProductDTO'ya mapliyoruz.
        }

        public async Task<bool> IsProductNameExistAsync(string productName)
        {
            var product = await _productRepository.GetByNameAsync(productName);
            return product != null;
        }

        public async Task<Product> GetByNameAsync(string productName)
        {
            return await _productRepository.GetByNameAsync(productName);
        }

        /// <summary>
        /// Yeni bir ürün oluşturur.
        /// </summary>
        /// <param name="productCreateDTO">Oluşturulacak ürünün özelliklerini içerir.</param>
        /// <returns></returns>
        public async Task<IDataResult<ProductDTO>> AddAsync(ProductCreateDTO productCreateDTO)
        {
            try
            {
                //var category = await _categoryRepository.GetByIdAsync(productCreateDTO.CategoryId);
                //if (category == null)
                //{
                //    return new ErrorDataResult<ProductDTO>(_localizer[Messages.CATEGORY_LIST_FAILED]);
                //}

                var company = await _companyRepository.GetByIdAsync(productCreateDTO.CompanyId);

                if (company == null)
                {
                    return new ErrorDataResult<ProductDTO>(Messages.PRODUCT_CREATED_ERROR);
                }

                var product = productCreateDTO.Adapt<Product>();
                product.QRCode = _qrService.GenerateQrCode(product.Id.ToString());
                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangeAsync();
                return new SuccessDataResult<ProductDTO>(product.Adapt<ProductDTO>(), Messages.PRODUCT_CREATED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<ProductDTO>(productCreateDTO.Adapt<ProductDTO>(), Messages.PRODUCT_CREATED_ERROR + " - " + ex.Message);
            }
        }

        /// <summary>
        /// Checks if the specified product is associated with any orders.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        /// <returns>A boolean value indicating whether the product is present in any order (true) or not (false).</returns>
        public async Task<bool> IsProductInOrderAsync(Guid productId)
        {
            return await _orderRepository.AnyAsync(o => o.OrderDetails.Any(od => od.ProductId == productId));
        }

        /// <summary>
        /// Id'si verilen ürünü siler ve bu ürünle ilişkili stok kayıtlarındaki miktarı sıfırlar.
        /// Eğer ürün bir siparişte mevcutsa, ürünü silmek yerine pasif hale getirir.
        /// </summary>
        /// <param name="productId">Silinecek olan ürünün Id değeri</param>
        /// <returns></returns>
        public async Task<IResult> DeleteAsync(Guid productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new ErrorResult(Messages.PRODUCT_NOT_FOUND);
                }

                var productInOrders = await _orderRepository.AnyAsync(o => o.OrderDetails.Any(od => od.ProductId == productId));

                if (productInOrders)
                {
                    // Ürün bir siparişte mevcut, bu yüzden silmek yerine pasif hale getiriyoruz
                    var result = await _productRepository.SetProductToPassiveAsync(productId);
                    if (!result.IsSuccess)
                    {
                        return result;
                    }
                    await _productRepository.SaveChangeAsync();
                    return new SuccessResult(Messages.PRODUCT_PASSIVED_SUCCESS);
                }
                else
                {
                    // Ürünü sil ve stok miktarlarını sıfırla
                    var stocks = await _stockService.GetAllAsync();
                    foreach (var stock in stocks.Data)
                    {
                        if (stock.ProductId == productId)
                        {
                            stock.Count = 0;
                            stock.ProductName = product.Name;
                            await _stockService.UpdateAsync(stock.Adapt<StockUpdateDTO>());
                        }
                    }

                    await _productRepository.DeleteAsync(product);
                }

                await _productRepository.SaveChangeAsync();
                return new SuccessResult(Messages.PRODUCT_DELETED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorResult(Messages.PRODUCT_DELETED_ERROR + " - " + ex.Message);
            }
        }

        /// <summary>
        /// Tüm ürünleri liste olarak döndürür
        /// </summary>
        /// <returns></returns>
        public async Task<IDataResult<List<ProductListDTO>>> GetAllAsync()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                var categories = await _categoryRepository.GetAllAsync();
                if (products == null)
                {
                    return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), Messages.PRODUCT_LISTED_ERROR);
                }
                else if (products.Count() == 0)
                {
                    return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), Messages.PRODUCT_LISTED_EMPTY);
                }

                var productListDTOs = products.Adapt<List<ProductListDTO>>();
                var companies = await _companyRepository.GetAllAsync();

                foreach (var productDTO in productListDTOs)
                {
                    var category = categories.FirstOrDefault(b => b.Id == productDTO.CategoryId);
                    productDTO.CategoryName = category?.Name;
                    var company = companies.FirstOrDefault(x => x.Id == productDTO.CompanyId);

                    if (company != null)
                    {
                        productDTO.CompanyName = company.Name;
                    }
                    else
                    {
                        productDTO.CompanyName = Messages.PRODUCT_COMPANY_DELETED;
                    }
                }

                return new SuccessDataResult<List<ProductListDTO>>(productListDTOs, Messages.PRODUCT_LISTED_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), Messages.PRODUCT_LISTED_ERROR);
            }
        }

        /// <summary>
        /// Verilen Id değerine sahip ürünü döndürür.
        /// </summary>
        /// <param name="productId">İstenen ürünün Id değeri</param>
        /// <returns></returns>
        public async Task<IDataResult<ProductDTO>> GetByIdAsync(Guid productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new ErrorDataResult<ProductDTO>(Messages.PRODUCT_NOT_FOUND);
                }

                var productDTO = product.Adapt<ProductDTO>();
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                var company = await _companyRepository.GetByIdAsync(product.CompanyId);

                if (category != null && company != null)
                {
                    productDTO.CategoryName = category.Name;
                    productDTO.CompanyName = company.Name;
                }

                return new SuccessDataResult<ProductDTO>(product.Adapt<ProductDTO>(), Messages.PRODUCT_GET_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<ProductDTO>(Messages.PRODUCT_NOT_FOUND);
            }
        }

        /// <summary>
        /// ID'sine karşılık gelen ürünü verilen özellikler ile günceller
        /// </summary>
        /// <param name="productUpdateDTO"></param>
        /// <returns></returns>
        public async Task<IResult> UpdateAsync(ProductUpdateDTO productUpdateDTO)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productUpdateDTO.Id);
                if (product == null)
                {
                    return new ErrorResult(Messages.PRODUCT_NOT_FOUND);
                }

                var category = await _categoryRepository.GetByIdAsync(productUpdateDTO.CategoryId);
                if (category == null)
                {
                    return new ErrorResult(Messages.CATEGORY_LIST_FAILED);
                }

                var company = await _companyRepository.GetByIdAsync(productUpdateDTO.CompanyId);
                if (company == null)
                {
                    return new ErrorDataResult<ProductDTO>(Messages.COMPANY_GETBYID_ERROR);
                }

                product.Name = productUpdateDTO.Name;
                product.Price = productUpdateDTO.Price;
                product.CategoryId = productUpdateDTO.CategoryId;
                product.CompanyId = productUpdateDTO.CompanyId;

                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangeAsync();

                return new SuccessResult(Messages.PRODUCT_UPDATED_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.Warehouse_UPDATE_FAILED);
            }
        }

        /// <summary>
        /// Tüm ürünleri günceller
        /// </summary>
        /// <returns></returns>
        public async Task<IResult> UpdateAllProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                foreach (var product in products)
                {
                    var updateDto = new ProductUpdateDTO
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        CategoryId = product.CategoryId,
                        CompanyId = product.CompanyId
                    };
                    await UpdateAsync(updateDto);
                }

                return new SuccessResult("Tüm ürünler başarıyla güncellendi." + " " + DateTime.Now);
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Tüm ürünler güncellenemedi: {ex.Message}" + " " + DateTime.Now);
            }
        }

        public async Task<IDataResult<List<ProductListDTO>>> GetAllAsync(string sortOrder, string searchQuery = "")
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                var categories = await _categoryRepository.GetAllAsync();

                if (products == null)
                {
                    return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), Messages.PRODUCT_LISTED_ERROR);
                }
                else if (products.Count() == 0)
                {
                    return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), Messages.PRODUCT_LISTED_EMPTY);
                }

                // Filter products based on search query
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    products = products
                        .Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var productListDTOs = products.Adapt<List<ProductListDTO>>();
                var companies = await _companyRepository.GetAllAsync();

                foreach (var productDTO in productListDTOs)
                {
                    var category = categories.FirstOrDefault(b => b.Id == productDTO.CategoryId);
                    productDTO.CategoryName = category?.Name;

                    var company = companies.FirstOrDefault(x => x.Id == productDTO.CompanyId);

                    if (company != null)
                    {
                        productDTO.CompanyName = company.Name;
                    }
                    else
                    {
                        productDTO.CompanyName = Messages.PRODUCT_COMPANY_DELETED;
                    }
                }

                return new SuccessDataResult<List<ProductListDTO>>(productListDTOs, Messages.PRODUCT_LISTED_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), Messages.PRODUCT_LISTED_ERROR);
            }
        }

        /// <summary>
        /// Tüm ürünleri asenkron olarak getirir ve sağlanan sıralama türüne göre listeyi sıralar.
        /// </summary>
        /// <param name="sortOrder">
        /// Ürün listesinin sıralama düzeni. Geçerli değerler:
        /// "date" - Oluşturulma tarihine göre azalan sırayla sıralar (en yeni ilk).
        /// "datedesc" - Oluşturulma tarihine göre artan sırayla sıralar (en eski ilk).
        /// "alphabetical" - Ürün adına göre artan sırayla sıralar (A-Z).
        /// "alphabeticaldesc" - Ürün adına göre azalan sırayla sıralar (Z-A).
        /// </param>
        /// <returns>
        /// Sıralanmış ürün listesini içeren List<ProductListDTO> döner.
        /// Başarılı olursa, sonuç sıralanmış ürün listesini ve bir başarı mesajını içerir.
        /// Eğer ürün bulunamazsa veya bir hata oluşursa, sonuç bir hata mesajını içerir.
        /// </returns>
        public async Task<IDataResult<List<ProductListDTO>>> GetAllAsyncProduct(string sortOrder)
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                var productListDTOs = products.Adapt<List<ProductListDTO>>();
                if (productListDTOs == null || productListDTOs.Count == 0)
                {
                    return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), Messages.PRODUCT_LISTED_EMPTY);
                }
                switch (sortOrder.ToLower())
                {
                    case "date":
                        productListDTOs = productListDTOs.OrderByDescending(c => c.CreatedDate).ToList();
                        break;
                    case "datedesc":
                        productListDTOs = productListDTOs.OrderBy(c => c.CreatedDate).ToList();
                        break;
                    case "alphabetical":
                        productListDTOs = productListDTOs.OrderBy(c => c.Name).ToList();
                        break;
                    case "alphabeticaldesc":
                        productListDTOs = productListDTOs.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "priceascend":
                        productListDTOs = productListDTOs.OrderBy(c => c.Price).ToList();
                        break;
                    case "pricedesc":
                        productListDTOs = productListDTOs.OrderByDescending(c => c.Price).ToList();
                        break;
                }
                return new SuccessDataResult<List<ProductListDTO>>(productListDTOs, Messages.PRODUCT_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), Messages.PRODUCT_LISTED_ERROR);
            }
        }

        /// <summary>
        /// Belirtilen kategoriye ait ürünleri asenkron olarak getirir.
        /// Eğer kategori Id belirtilmezse tüm ürünleri döndürür.
        /// </summary>
        /// <param name="categoryId">
        /// Filtreleme yapmak için kullanılan kategori Id'si.
        /// Eğer null verilirse, tüm ürünler döndürülür.
        /// </param>
        /// <returns>
        /// Belirtilen kategoriye göre filtrelenmiş ürün listesini içeren List<ProductListDTO>.
        /// Başarılı olursa, sonuç ürün listesini ve bir başarı mesajını içerir.
        /// Eğer ürün bulunamazsa veya bir hata oluşursa, sonuç bir hata mesajını içerir.
        /// </returns>
        public async Task<IDataResult<List<ProductListDTO>>> GetAllAsyncByCategory(Guid? categoryId)
        {
            try
            {
                var products = await _productRepository.GetAllAsync();

                if (categoryId.HasValue)
                {
                    products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
                }
                if (!products.Any())
                {
                    return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), Messages.PRODUCT_LISTED_EMPTY);
                }

                var productListDTOs = products.Adapt<List<ProductListDTO>>();
                return new SuccessDataResult<List<ProductListDTO>>(productListDTOs, Messages.PRODUCT_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), Messages.PRODUCT_LISTED_ERROR + " " + ex.Message);
            }
        }

        public async Task<IDataResult<List<ProductListDTO>>> GetProductsByCompanyIdAsync(Guid companyId)
        {
            try
            {
                var products = await _productRepository.GetProductsByCompanyIdAsync(companyId);

                if (!products.Any())
                {
                    return new ErrorDataResult<List<ProductListDTO>>(new List<ProductListDTO>(), "Bu şirkete ait ürün bulunamadı.");
                }

                var productListDTOs = products.Adapt<List<ProductListDTO>>();
                return new SuccessDataResult<List<ProductListDTO>>(productListDTOs, "Ürünler başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<ProductListDTO>>(null, $"Hata oluştu: {ex.Message}");
            }
        }

        
    }
}