namespace BaSalesManagementApp.Entities.Configurations
{
    public class CompanyConfiguration: AuditableEntityConfiguration<Company>
    {
        public override void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);
            
            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(40);

            builder.HasMany(x => x.Branches)
                .WithOne(x => x.Company)
                .HasForeignKey(x => x.CompanyId);

            builder.HasMany(x => x.Employees)
                .WithOne(x => x.Company)
                .HasForeignKey(x => x.CompanyId);

            builder.HasMany(x => x.Promotions)
                .WithOne(x => x.Company)
                .HasForeignKey(x => x.CompanyId);

            builder.HasMany(x => x.Products)
                .WithOne(x => x.Company)
                .HasForeignKey(x => x.CompanyId);

            builder.HasMany(x => x.Customers)
                .WithOne(x => x.Company)
                .HasForeignKey(x => x.CompanyId);
        }
    }
}
