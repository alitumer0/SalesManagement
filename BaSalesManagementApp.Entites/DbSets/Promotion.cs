
namespace BaSalesManagementApp.Entites.DbSets
{
    public class Promotion: AuditableEntity
    {
        public int Discount { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // nav props
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
