using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IOrderDetailRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<OrderDetail>, IAsyncDeletableRepository<OrderDetail>, IAsyncFindableRepository<OrderDetail>, IAsyncInsertableRepository<OrderDetail>, IAsyncOrderableRepository<OrderDetail>, IAsyncQueryableRepository<OrderDetail>, IDeletableRepository<OrderDetail>
    {
    }
}
