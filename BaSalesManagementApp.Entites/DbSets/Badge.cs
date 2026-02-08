using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class Badge : AuditableEntity
    {
        public string Name { get; set; } = null!;
        public int SalesQuantity { get; set; }
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;
    }
}
