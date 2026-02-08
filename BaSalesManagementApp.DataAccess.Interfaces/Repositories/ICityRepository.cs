using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface ICityRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<City>, IAsyncDeletableRepository<City>, IAsyncFindableRepository<City>, IAsyncInsertableRepository<City>,/* IAsyncOrderableRepository<City>,*/ IAsyncQueryableRepository<City>, IDeletableRepository<City>
    {
        
    }
}
