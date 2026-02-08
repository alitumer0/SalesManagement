namespace BaSalesManagementApp.Entities.Configurations
{
    public class StockConfiguration : AuditableEntityConfiguration<Stock>
    {
        public override void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.Property(x => x.Count).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            base.Configure(builder);


        }
    }
}
