using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class StockTypeSize : AuditableEntity
    {
        public string Size { get; set; }
        public string Description { get; set; }
        public Guid? StockTypeId { get; set; }
        public virtual StockType? StockType { get; set; }
        public Guid? CompanyId { get; set; }

    }
}
