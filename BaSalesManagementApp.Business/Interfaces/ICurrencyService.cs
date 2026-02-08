using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.CurrencyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Business.Interfaces
{

    /// <summary>
    /// Döviz birimleri ile ilgili işlemleri yöneten servis arayüzü.
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
        /// Tüm döviz birimlerini dış kaynaktan alır ve işlenmek üzere döndürür.
        /// Bu işlem genellikle otomatik olarak belirli zaman aralıklarında gerçekleştirilir.
        /// </summary>
        /// <returns>Asenkron bir işlem olarak çalışır.</returns>
        Task GetAllCurrencies();

        /// <summary>
        /// Veritabanındaki mevcut döviz kurlarını getirir.
        /// </summary>
        /// <returns>Asenkron olarak döviz kurlarını içeren bir liste döner.</returns>
        Task<IDataResult<List<CurrentExchangeRateListDTO>>> GetAllCurrentExchangeRatesAsync();
        Task<IResult> DeleteLastCurrency();

        /// <summary>
        /// Veritabanındaki en güncel döviz kuru bilgisini getirir.
        /// </summary>
        /// <returns>
        /// En güncel döviz kurunu CurrentExchangeRateDTO tipinde geri döndürür.
        /// </returns>
        Task<IDataResult<CurrentExchangeRateDTO>> GetLatestCurrencyRateAsync();
    }

}
