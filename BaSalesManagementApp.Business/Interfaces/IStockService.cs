using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.StockDTOs;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface IStockService
    {
		/// <summary>
		/// Tüm stokları getirir.
		/// </summary>
		/// <returns>Tüm stokların listesini ve sonuç durumunu döndürür</returns>
		/// 

		Task<IDataResult<List<StockListDTO>>> GetAllAsync(string orderOrder,string searchQuery);

		Task<IDataResult<List<StockListDTO>>> GetAllAsync(string orderOrder);

        Task<IDataResult<List<StockListDTO>>> GetAllAsync();

        /// <summary>
        /// Belirtilen ID'li stoğu getirir.
        /// </summary>
        /// <param name="stockId">Getirilecek stok ID'si</param>
        /// <returns>Belirtilen ID'li stok verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<StockDTO>> GetByIdAsync(Guid stockId);

        /// <summary>
        /// Yeni bir stok oluşturur.
        /// </summary>
        /// <param name="stockCreateDTO">Oluşturulacak stok bilgileri</param>
        /// <returns>Oluşturulan stok verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<StockDTO>> AddAsync(StockCreateDTO stockCreateDTO);

        /// <summary>
        /// ID'sine karşılık gelen stok verilen özellikler ile güncellenir
        /// </summary>
        /// <param name="stockUpdateDTO">Güncellenecek stok ID'si</param>
        /// <returns></returns>
        Task<IDataResult<StockDTO>> UpdateAsync(StockUpdateDTO stockUpdateDTO);

        /// <summary>
        /// Belirtilen ID'li stok silinir.
        /// </summary>
        /// <param name="stockId">Silinecek stok ID'si</param>
        /// <returns>Stok silme işleminin sonuç durumunu döndürür</returns>
        Task<IResult> DeleteAsync(Guid stockId);

        /// <summary>
        /// Sipariş detaylarına göre stok kontrolü yapar.
        /// </summary>
        /// <param name="orderDetails">Sipariş detayları</param>
        /// <returns>Stok durumunu döndürür</returns>
        Task<IResult> CheckStockAvailabilityAsync(List<OrderDetailCreateDTO> orderDetails);
        /// <summary>
        /// Sipariş oluşturma detaylarına göre stokları günceller.
        /// </summary>
        /// <param name="orderDetails">Sipariş detayları</param>
        /// <returns>Stok güncelleme işlemi sonucu</returns>
        Task<IResult> UpdateStockAsync(List<OrderDetailCreateDTO> orderDetails);
        /// <summary>
        /// Verilen sipariş detaylarına göre stok yeterliliğini kontrol eder.
        /// </summary>
        /// <param name="orderDetails">Sipariş detayları</param>
        /// <param name="orderId">Güncellenen siparişin ID'si</param>
        /// <returns>Stok yeterliliği sonucunu döndürür.</returns>
        Task<IResult> CheckStockAvailabilityAsync(List<OrderDetailUpdateDTO> orderDetails, Guid orderId);
        /// <summary>
        /// Sipariş güncelleme detaylarına göre stokları günceller.
        /// </summary>
        /// <param name="orderDetails">Sipariş detayları</param>
        /// <param name="orderId">Güncellenen siparişin ID'si</param>
        /// <returns>Stok güncelleme işlemi sonucu</returns>
        Task<IResult> UpdateStockAsync(List<OrderDetailUpdateDTO> orderDetails, Guid orderId);
        /// <summary>
        /// Sipariş silme işlemine göre stokları güncelleme
        /// </summary>
        /// <param name="orderId">Silinen siparişin ID'si</param>
        /// <returns>Stok güncelleme işlemi sonucu</returns>
        Task<IResult> DeleteStockAsync(Guid orderId);
        /// <summary>
        /// Manager ID'sine bağlı ilgili stok listesini getirir.
        /// </summary>
        /// <param name="userId">Manager rolüne ait ID numarası.</param>
        /// <returns>Manager rolü için stok listesini döndürür</returns>
        Task<List<StockListDTO>> GetStockListForManagerAsync(string userId);

        /// <summary>
        /// Şirket Id bilgisi alınarak, şirkete ait stokları getirir.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns> Asenkron işlemi temsil eden bir görev. Görev sonucunda stok listesini döndürür. </returns>
        Task<IDataResult<List<StockListDTO>>> GetStocksListByCompanyIdAsync(Guid companyId);
    }
}
