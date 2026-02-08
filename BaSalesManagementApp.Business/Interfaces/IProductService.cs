using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.WarehouseDTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface IProductService
    {

        /// <summary>
        /// Ürünleri sıralama, filtreleme ve sayfalama ile asenkron olarak getirir.
        /// </summary>
        /// <param name="sortOrder">Sıralama türü (örneğin, "alphabetical", "pricedesc").</param>
        /// <param name="page">Görüntülenecek sayfa numarası.</param>
        /// <param name="pageSize">Sayfa başına gösterilecek ürün sayısı.</param>
        /// <param name="categoryId">Filtreleme için kategori ID'si (opsiyonel).</param>
        /// <param name="companyId">Filtreleme için şirket ID'si (opsiyonel).</param>
        /// <param name="user">Kullanıcı bilgileri (örneğin, Manager rolü kontrolü için).</param>
        /// <returns>Sayfalanmış ürün listesini (IPagedList<ProductListDTO>) döndürür.</returns>
        Task<IPagedList<ProductListDTO>> GetProductsAsync(string sortOrder, int page, int pageSize, Guid? categoryId, Guid? companyId, ClaimsPrincipal user = null);

        /// <summary>
        /// Tüm ürünleri liste olarak döndürür
        /// </summary>
        /// <returns></returns>
        Task<IDataResult<List<ProductListDTO>>> GetAllAsync(string sortOrder, string searchQuery);

        /// <summary>
        /// İsim ve şirket kimliğine göre ürünü asenkron olarak getirir.
        /// </summary>
        /// <param name="name">Aranacak ürünün adı.</param>
        /// <param name="companyId">Ürünün ait olduğu şirketin kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucu, ürün bulunursa <see cref="ProductDTO"/> döndürür; aksi takdirde null döner.</returns>
        Task<ProductDTO> GetByNameAndCompanyAsync(string name, Guid companyId);

        /// <summary>
        /// Belirtilen kategori Id'sine göre ürünlerin listesini döndürür.
        /// </summary>
        /// <param name="categoryId">Kategori Id'si.</param>
        /// <returns>Ürünlerin listesi.</returns>
        Task<IDataResult<List<ProductListDTO>>> GetAllAsyncByCategory(Guid? categoryId);

        Task<IDataResult<List<ProductListDTO>>> GetAllAsync();

        /// <summary>
        /// Verilen Id değerine sahip ürünü döndürür.
        /// </summary>
        /// <param name="productId">İstenen ürünün Id değeri</param>
        /// <returns></returns>
        Task<IDataResult<ProductDTO>> GetByIdAsync(Guid productId);

        /// <summary>
        /// Yeni bir ürün oluşturur.
        /// </summary>
        /// <param name="productCreateDTO">Oluşturulacak ürünün özelliklerini içerir.</param>
        /// <returns></returns>
        Task<IDataResult<ProductDTO>> AddAsync(ProductCreateDTO productCreateDTO);

        /// <summary>
        /// ID'sine karşılık gelen ürünü verilen özellikler ile günceller
        /// </summary>
        /// <param name="productUpdateDTO"></param>
        /// <returns></returns>
        Task<IResult> UpdateAsync(ProductUpdateDTO productUpdateDTO);

        /// <summary>
        /// Id'si verilen ürünü siler
        /// </summary>
        /// <param name="productId">Silinecek olan ürünün Id değeri</param>
        /// <returns></returns>
        Task<IResult> DeleteAsync(Guid productId);

        Task<IResult> UpdateAllProductsAsync();

        Task<Product> GetByNameAsync(string productName);

        Task<bool> IsProductInOrderAsync(Guid productId);

        Task<bool> IsProductNameExistAsync(string productName);

        /// <summary>
        /// Tüm Ürünleri getirir.
        /// </summary>
        /// <param name="sortOrder">Ürün listesinin sıralama düzenini belirten parametre.</param>
        /// <returns>Tüm Ürün listesini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<List<ProductListDTO>>> GetAllAsyncProduct(string sortOrder);

        Task<IDataResult<List<ProductListDTO>>> GetProductsByCompanyIdAsync(Guid companyId);

       
    }
}