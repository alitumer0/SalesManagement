namespace BaSalesManagementApp.Entities.Configurations
{
    public class CustomerConfiguration: AuditableEntityConfiguration<Customer>
    {
        public override void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(128);
            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(40);       

            builder.Property(x => x.CompanyId)
                .IsRequired();
        }
    }
}
