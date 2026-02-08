namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IOrderRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<Order>, IAsyncDeletableRepository<Order>, IAsyncFindableRepository<Order>, IAsyncInsertableRepository<Order>, IAsyncOrderableRepository<Order>, IAsyncQueryableRepository<Order>, IDeletableRepository<Order>
    {
        /// <summary>
        ///  Orders ile Admin tablosunu birleştirerek siparişleri listeleme.
        /// </summary>
        /// <returns></returns>
        Task<List<Order>> GetAllWithAdminAsync();
        Task<Order> GetOrderWithAdminAsync(Guid orderId);
        Task<Order> GetByIdWithDetailsAsync(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByCompany(Guid companyId, bool tracking = true);
        /// <summary>
        /// Belirtilen çalışana ait tüm siparişleri getirir.
        /// </summary>
        /// <param name="employeeId">Çalışanın benzersiz kimlik numarası (ID).</param>
        /// <param name="tracking">
        /// Entity Framework Core tarafından izleme yapılmasını belirler. 
        /// Eğer `true` ise takip modunda çalışır, `false` ise takip edilmeden sorgu yapılır (performans için iyileştirilmiş).
        /// </param>
        /// <returns>Belirtilen çalışana ait siparişlerin bir listesini asenkron olarak döndürür.</returns>
        Task<IEnumerable<Order>> GetOrdersByEmployee(Guid employeeId, bool tracking = true);

        /// <summary>
        /// Tüm siparişleri, ilişkili sipariş detayları ve ürün bilgileriyle birlikte getirir.
        /// Siparişlere ait OrderDetails koleksiyonu ve her bir detayın Product nesnesi eager loading ile yüklenir.
        /// </summary>
        /// <returns>Order nesnelerinin tam listesi (detaylarla birlikte)</returns>
        Task<List<Order>> GetAllWithDetailsAsync();

        // Aktif + silinmemiş siparişleri, verilen tarih aralığında döndürür (bitiş dahil).
        IQueryable<Order> QueryActiveInRange(DateTime start, DateTime endInclusive);
    }
}
