using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class CurrentExchangeRateRepository:EFBaseRepository<CurrentExchangeRate>,ICurrentExchangeRateRepository
    {
        public CurrentExchangeRateRepository(BaSalesManagementAppDbContext context) : base(context)
        {

        }
             
        /// <summary>
        /// döviz kurlarını veritabanından alır ve en güncel tarihe göre sıralayarak döndürür.
        /// </summary>
        /// <returns>döviz kurlarının listesi.</returns>
        public async Task<List<CurrentExchangeRate>> GetAllExchangeRatesAsync()
        {
              return await _table

			  .Where(x => ((int)x.Status == 3))
			  .OrderByDescending(x => x.CreatedDate) 
              .ToListAsync(); 
        }
    }
}
