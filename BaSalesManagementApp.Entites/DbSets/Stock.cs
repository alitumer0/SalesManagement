
namespace BaSalesManagementApp.Entites.DbSets
{
    public class Stock : AuditableEntity
    {
        public int Count { get; set; }

        //NAV
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
        public Guid? WarehouseId { get; set; }
        public virtual Warehouse? Warehouse { get; set; }

    }
}
