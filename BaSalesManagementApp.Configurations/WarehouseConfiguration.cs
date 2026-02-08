namespace BaSalesManagementApp.Entities.Configurations
{
    public class WarehouseConfiguration : AuditableEntityConfiguration<Warehouse>
    {
        public override void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Address).IsRequired();
            // Warehouse ile Branch arasında bire-çok ilişki
            builder.Property(w => w.BranchId)
            .IsRequired();

            base.Configure(builder);
        }
    }
}
