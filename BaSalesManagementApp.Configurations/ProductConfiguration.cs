namespace BaSalesManagementApp.Entities.Configurations
{
    public class ProductConfiguration : AuditableEntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Price).IsRequired().HasPrecision(18, 2);
            // PRODUCT VE CATEGORY arasında bire-çok ilişki
            builder.Property(w => w.CategoryId)
            .IsRequired();


            builder.HasMany(x => x.Promotions)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId);

            builder.HasMany(x => x.OrderDetails)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId);


            builder.Property(x => x.CompanyId).IsRequired();

            base.Configure(builder);
        }
    }
}
