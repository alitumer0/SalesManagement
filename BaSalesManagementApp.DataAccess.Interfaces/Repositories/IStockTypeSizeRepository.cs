using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IStockTypeSizeRepository :IAsyncRepository,IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<StockTypeSize>, IAsyncDeletableRepository<StockTypeSize>, IAsyncFindableRepository<StockTypeSize>, IAsyncInsertableRepository<StockTypeSize>, IAsyncOrderableRepository<StockTypeSize>, IAsyncQueryableRepository<StockTypeSize>, IDeletableRepository<StockTypeSize>
    {
    }
}
