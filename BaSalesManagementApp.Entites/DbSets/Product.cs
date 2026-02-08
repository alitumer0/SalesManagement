
namespace BaSalesManagementApp.Entites.DbSets
{
    public class Product : AuditableEntity
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal PurchasePrice { get; set; }
        public byte[]? QRCode { get; set; }

        //NAVIGATION PROP.       
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual List<Promotion> Promotions { get; set; } = new ();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }


        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
