
using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class Branch : AuditableEntity
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;

        
        public virtual List<Warehouse> Warehouses { get; set; }
    }
}


