using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Tüm kategorileri getirir ve arama ile sıralama işlemlerini gerçekleştirir.
        /// </summary>
        /// <param name="sortOrder">Sıralama düzenini belirten bir dize (örneğin, "alphabetical", "date").</param>
        /// <param name="searchQuery">Arama sorgusu.</param>
        /// <returns>Filtrelenmiş ve sıralanmış kategori listesi.</returns>
        /// 
        Task<IDataResult<List<CategoryListDTO>>> GetAllAsync(string sortOrder);
        Task<IDataResult<List<CategoryListDTO>>> GetAllAsync(string sortOrder, string searchQuery);
       




        /// <summary>
        /// Yeni bir kategori ekler.
        /// </summary>
        /// <param name="categoryCreateDTO">Eklenmek istenen kategoriyle ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda eklenen kategori verilerini içerir.</returns>
        Task<IDataResult<CategoryDTO>> AddAsync(CategoryCreateDTO categoryCreateDTO);

        /// <summary>
        /// Benzersiz kimliğiyle bir kategori alır.
        /// </summary>
        /// <param name="categoryId">Alınmak istenen kategorinin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda kategori verilerini içerir, bulunamazsa null döner.</returns>
        Task<IDataResult<CategoryDTO>> GetByIdAsync(Guid categoryId);

        /// <summary>
        /// Benzersiz kimliğiyle bir kategoriyi siler.
        /// </summary>
        /// <param name="categoryId">Silinmek istenen kategorinin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda silme işleminin başarılı olup olmadığını belirtir.</returns>
        Task<IResult> DeleteAsync(Guid categoryId);

        /// <summary>
        /// Bir kategoriyi günceller.
        /// </summary>
        /// <param name="categoryUpdateDTO">Güncellenmiş kategoriyle ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda güncellenmiş kategori verilerini içerir.</returns>
        Task<IDataResult<CategoryDTO>> UpdateAsync(CategoryUpdateDTO categoryUpdateDTO);

        /// <summary>
        /// Tüm kategorileri alır.
        /// </summary>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda tüm kategorilerin listesini içerir.</returns>
        Task<IDataResult<List<CategoryListDTO>>> GetAllAsync();

        /// <summary>
        /// Gelen ürün listesindeki ürünlerin kategorilerini verir.
        /// </summary>
        /// <param name="products"></param>
        /// <returns> Asenkron işlemi temsil eden bir görev. Görev sonucunda kategori listesini döndürür. </returns>
        Task<List<Category>> GetCategoriesByProductListAsync(IDataResult<List<ProductListDTO>> products);

        /// <summary>
        /// Ürün listesine göre kategorileri getirir.
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        Task<IDataResult<List<CategoryListDTO>>> GetCategoriesByProductListAsyncs(IDataResult<List<ProductListDTO>> products);

        /// <summary>
        /// Şirketin kategorilerini getirir.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<IDataResult<List<CategoryListDTO>>> GetCategoriesByCompanyIdAsync(Guid companyId);

        /// <summary>
        /// Kategoriye ait ürün var mı yok mu kontrol eder.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda ürüne sahip olup olmadığı bilgisini döndürür.</returns>
        Task<IDataResult<bool>> HasProductsAsync(Guid categoryId);

    }
}