using BaSalesManagementApp.Dtos.AdminDTOs;

namespace BaSalesManagementApp.Business.Interfaces
{
    /// <summary>
    /// Admin işlemlerini yöneten servis arayüzü.
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// Yeni bir admin ekler.
        /// </summary>
        /// <param name="adminCreateDTO">Eklenen adminin bilgileri</param>
        /// <returns>Eklenen adminin verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<AdminDTO>> AddAsync(AdminCreateDTO adminCreateDTO);


        /// <summary>
        /// Belirtilen ID'ye sahip adminin getirir.
        /// </summary>
        /// <param name="adminId">Getirilecek adminin ID'si</param>
        /// <returns>Belirtilen ID'ye sahip adminin verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<AdminDTO>> GetByIdAsync(Guid adminId);


        /// <summary>
        /// Belirtilen IdentityID'ye sahip admini getirir.
        /// </summary>
        /// <param name="adminIdentityId">Getirilecek adminin IdentityID'si</param>
        /// <returns>Belirtilen IdentityID'ye sahip adminin verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<AdminDTO>> GetByIdentityIdAsync(Guid adminIdentityId);


        /// <summary>
        /// Tüm adminleri getirir.
        /// </summary>
        /// <returns>Tüm adminlerin listesini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<List<AdminListDTO>>> GetAllAsync();

        /// <summary>
        /// Tüm adminleri getirir ve sıralama seçeneğine göre sıralar.
        /// </summary>
        /// <param name="sortAdmin">Sıralama düzeni (örn. "name", "date")</param>
        /// <returns>Tüm adminlerin listesini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<List<AdminListDTO>>> GetAllAsync(string sortAdmin);

        /// <summary>
		/// Tüm adminleri getirir, arama sorgusuna ve sıralama seçeneğine göre filtreler.
		/// </summary>
		/// <param name="searchQuery">E-posta adresi, oluşturma tarihi, ad veya soyada göre arama yapmak için kullanılan sorgu.</param>
		/// <returns>
		/// Filtrelenmiş ve sıralanmış adminleri içeren <see cref="IDataResult{List{AdminListDTO}}"/> nesnesi döner.
		/// Başarılıysa admin listesi ve başarı mesajı, başarısızsa hata mesajı döndürülür.
		/// </returns>
		Task<IDataResult<List<AdminListDTO>>> GetAllAsync(string sortAdmin,string searchQuery);

		/// <summary>
		/// Belirtilen ID'ye sahip admini siler.
		/// </summary>
		/// <param name="adminId">Silinecek adminin ID'si</param>
		/// <returns>Admin silme işleminin sonuç durumunu döndürür</returns>
		Task<IResult> DeleteAsync(Guid adminId);

        /// <summary>
        /// Belirtilen ID'ye sahip admini verilen bilgilerle günceller.
        /// </summary>
        /// <param name="adminUpdateDTO">Güncellenecek adminin bilgileri</param>
        /// <returns>Güncellenen yöneadmininticinin verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<AdminDTO>> UpdateAsync(AdminUpdateDTO adminUpdateDTO);

        /// <summary>
        /// Admin listesini veritabanından sayfalı olarak getirir.
        /// Filtreleme (search), sıralama (sort) ve sayfalama (page/pageSize)
        /// işlemleri veritabanında çalışır (Skip/Take).
        /// </summary>
        /// <param name="search">
        /// Arama ifadesi. Ad, soyad, email veya tarih ("dd.MM.yyyy") eşleşmesi.
        /// </param>
        /// <param name="sort">
        /// Sıralama anahtarı: "name", "namedesc", "createddate", "createddatedesc".
        /// </param>
        /// <param name="page">1’den başlayan sayfa numarası.</param>
        /// <param name="pageSize">Sayfa başına kayıt adedi.</param>
        /// <returns>
        /// Items: İstenen sayfadaki kayıtlar,
        /// Total: Tüm kayıt sayısı (sayfalama için gerekir).
        /// </returns>
        Task<(List<AdminListDTO> Items, int Total)> GetPagedAsync(string? search, string sort, int page, int pageSize);
        Task<int> CountAsync();
        Task<int> CountNewThisMonthAsync();
    }
}
