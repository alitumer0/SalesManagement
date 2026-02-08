using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class OrderDetail : AuditableEntity
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal PurchasePrice { get; set; }

        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}