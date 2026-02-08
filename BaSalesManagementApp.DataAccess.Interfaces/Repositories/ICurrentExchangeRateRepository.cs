using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface ICurrentExchangeRateRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<CurrentExchangeRate>, IAsyncDeletableRepository<CurrentExchangeRate>, IAsyncFindableRepository<CurrentExchangeRate>, IAsyncInsertableRepository<CurrentExchangeRate>, IAsyncOrderableRepository<CurrentExchangeRate>, IAsyncQueryableRepository<CurrentExchangeRate>, IDeletableRepository<CurrentExchangeRate>
    {
        /// <summary>
        /// Tüm döviz kurlarını tarihe göre azalan sırada getirir.
        /// </summary>
        /// <returns>Döviz kurları listesi.</returns>
        Task<List<CurrentExchangeRate>> GetAllExchangeRatesAsync();
    }
}
