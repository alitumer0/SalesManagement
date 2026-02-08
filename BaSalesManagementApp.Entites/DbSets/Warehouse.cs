
using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class Warehouse : AuditableEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }

        //NAV       
        public Guid BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ICollection<Stock>? Stock { get; set; }
    }
}


