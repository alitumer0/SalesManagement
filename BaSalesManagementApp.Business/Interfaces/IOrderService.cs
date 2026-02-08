using BaSalesManagementApp.Dtos.OrderDTOs;
using BaSalesManagementApp.Dtos.ReportingDTOs;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Tüm siparişleri getirir.
        /// </summary>
        /// <returns>Tüm siparişlerin listesini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<List<OrderListDTO>>> GetAllAsync();
        Task<IDataResult<List<OrderListDTO>>> GetAllAsync(string sortOrder);
        Task<IDataResult<List<OrderListDTO>>> GetAllAsync(string sortOrder, string searchQuery);
        /// <summary>
        /// Aranan siparişi admin, product ve company bilgileri ile birlikte getirir.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<IDataResult<OrderListDTO>> GetOrderWithDetailsByIdAsync(Guid orderId);

        /// <summary>
        /// Belirtilen ID'li siparişi getirir.
        /// </summary>
        /// <param name="orderId">Getirilecek siparişin ID'si</param>
        /// <returns>Belirtilen ID'li siparişin verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<OrderDTO>> GetByIdAsync(Guid orderId);

        /// <summary>
        /// Yeni bir sipariş oluşturur ve siparişi oluştururken IdentityId bilgisini de alır. 
        /// </summary>
        /// <param name="orderCreateDTO">Oluşturulacak siparişin bilgileri</param>
        /// <returns>Oluşturulan siparişin verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<OrderDTO>> AddAsync(OrderCreateDTO orderCreateDTO, string identityId);

        /// <summary>
        /// ID'sine karşılık gelen siparişi verilen özellikler ile günceller
        /// </summary>
        /// <param name="orderUpdateDTO">Güncellenecek siparişin ID'si</param>
        /// <returns></returns>
        Task<IDataResult<OrderDTO>> UpdateAsync(OrderUpdateDTO orderUpdateDTO);

        /// <summary>
        /// Belirtilen ID'li siparişi siler.
        /// </summary>
        /// <param name="orderId">Silinecek siparişin ID'si</param>
        /// <returns>Siparişi silme işleminin sonuç durumunu döndürür</returns>
        Task<IResult> DeleteAsync(Guid orderId);
        Task<IDataResult<List<OrderListDTO>>> GetOrdersByCompanyIdAsync(Guid companyId);
        /// <summary>
        /// Belirli bir çalışana ait siparişleri getirir.
        /// </summary>
        /// <param name="employeeId">Çalışanın benzersiz kimlik numarası.</param>
        /// <returns>OrderListDTO tipinde siparişlerin yer aldığı bir veri sonucu döner.</returns>
        Task<IDataResult<List<OrderListDTO>>> GetOrdersByEmployeeIdAsync(Guid employeeId);

        /// <summary>
        /// Şirket Id bilgisi alınarak, şirkete ait siparişleri getirir.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns> Asenkron işlemi temsil eden bir görev. Görev sonucunda sipariş listesini döndürür. </returns>
        Task<IDataResult<List<OrderListDTO>>> GetOrdersListByCompanyIdAsync(Guid companyId);

        /// <summary>
        /// Şirket Id bilgisi alınarak, şirkete ait siparişleri getirir.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns> Asenkron işlemi temsil eden bir görev. Görev sonucunda sipariş listesini döndürür. </returns>
        Task<IDataResult<List<OrderListDTO>>> GetOrdersListByCustomerIdAsync(Guid customerId);

        /// <summary>
        /// Sistemdeki aktif ve silinmemiş siparişlerin toplam gelirini hesaplar.
        /// Yalnızca kâr edilen satışlar dikkate alınır.
        /// </summary>
        /// <returns>Toplam gelir (₺) olarak decimal değer döner.</returns>
        Task<decimal> GetTotalIncomeAsync();
        /// <summary>
        /// Sistemdeki aktif ve silinmemiş siparişlerden elde edilen toplam kârı hesaplar.
        /// Kâr = Gelir - Gider, sadece kârlı satışlar dikkate alınır.
        /// </summary>
        /// <returns>Toplam kâr (₺) olarak decimal değer döner.</returns>
        Task<decimal> GetTotalProfitAsync();

        //Toplam Satış
        Task<SalesProfitDTO> GetSalesMetricAsync(int? year = null, Guid? companyId = null);

        // Ortak DTO ile Kâr (Profit) metrikleri
        Task<SalesProfitDTO> GetProfitMetricAsync(int year, Guid? companyId = null, bool costInOrderCurrency = true);

        //Toplam Gider
        Task<SalesProfitDTO> GetExpenseMetricAsync(int? year = null, Guid? companyId = null, bool costInOrderCurrency = true);


    }
}
