using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface ICountryRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<Country>, IAsyncDeletableRepository<Country>, IAsyncFindableRepository<Country>, IAsyncInsertableRepository<Country>, IAsyncOrderableRepository<Country>, IAsyncQueryableRepository<Country>, IDeletableRepository<Country>
    {

       
    }
}
