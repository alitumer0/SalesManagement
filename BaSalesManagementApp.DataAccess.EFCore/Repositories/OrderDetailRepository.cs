using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class OrderDetailRepository : EFBaseRepository<OrderDetail>,IOrderDetailRepository
    {
        public OrderDetailRepository(BaSalesManagementAppDbContext context) : base(context)
        {

        }
    }
}
