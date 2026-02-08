namespace BaSalesManagementApp.Entities.Configurations
{
    public class BranchConfiguration : AuditableEntityConfiguration<Branch>
    {
        public override void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Address).IsRequired();
            builder.Property(x => x.CompanyId).IsRequired();
            builder.HasMany(b => b.Warehouses)
           .WithOne(w => w.Branch)
           .HasForeignKey(w => w.BranchId);
            base.Configure(builder);
        }
    }
}
