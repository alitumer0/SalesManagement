namespace BaSalesManagementApp.Entities.Configurations
{
    public class OrderConfiguration : AuditableEntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.TotalPrice).IsRequired().HasPrecision(18, 2);
            builder.HasIndex(x => x.OrderNo).IsUnique();
            builder.Property(x => x.OrderDate).IsRequired();

            builder.Property(x => x.CurrencyType)
                   .IsRequired(false);

            builder.HasMany(x => x.OrderDetails)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);
            // Customer ile ilişkiyi yapılandırma
            builder.HasOne(x => x.Customer)  // Order, bir Customer'a sahip
                .WithMany(c => c.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);  // Sipariş silindiğinde, ilişkili müşteri silinmesin, sadece sipariş silinsin
            base.Configure(builder);
        }
    }
}
