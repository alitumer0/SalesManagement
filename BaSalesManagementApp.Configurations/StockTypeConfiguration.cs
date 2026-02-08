namespace BaSalesManagementApp.Entities.Configurations
{
    public class StockTypeConfiguration : AuditableEntityConfiguration<StockType>
    {
        public override void Configure(EntityTypeBuilder<StockType> builder)
        {
            base.Configure(builder);

            builder.Property(prdctType => prdctType.Name).IsRequired().HasMaxLength(128);
            builder.Property(prdctType => prdctType.Description).IsRequired().HasMaxLength(128);
        }
    }
}