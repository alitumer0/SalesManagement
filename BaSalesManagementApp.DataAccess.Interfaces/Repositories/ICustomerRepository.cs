using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface ICustomerRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<Customer>, IAsyncDeletableRepository<Customer>, IAsyncFindableRepository<Customer>, IAsyncInsertableRepository<Customer>, IAsyncOrderableRepository<Customer>, IAsyncQueryableRepository<Customer>, IDeletableRepository<Customer>
    {
        /// <summary>
        /// Müşteri siparişleri için gerekli olan metot
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="expression">Filtreleme yapmak için</param>
        /// <param name="orderby">Artanveya azalan şekilde sıralama yapması için</param>
        /// <param name="orderDesc">tersten sıralama</param>
        /// <param name="tracking">Verinin sadece okunması veya üzerinde değişiklik yapılması durumunu kontrol eder</param>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetAllAsync<TKey>(Expression<Func<Order, bool>> expression, Expression<Func<Order, TKey>> orderby, bool orderDesc = false, bool tracking = true);

        /// <summary>
        /// Şirketin müşterilerini getirir.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        Task<IEnumerable<Customer>> GetCustomersByCompany(Guid companyId);
    }
}
