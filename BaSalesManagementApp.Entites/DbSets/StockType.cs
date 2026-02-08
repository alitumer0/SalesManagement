namespace BaSalesManagementApp.Entites.DbSets
{
    public class StockType : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid CategoryId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<StockTypeSize>? StockTypeSizes { get; set; }
    }
}