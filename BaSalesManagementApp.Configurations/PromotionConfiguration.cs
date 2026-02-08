namespace BaSalesManagementApp.Entities.Configurations
{
    public class PromotionConfiguration : AuditableEntityConfiguration<Promotion>
    {
        public override void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.Property(x => x.Discount).IsRequired();
            builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();

            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.CompanyId).IsRequired();

            base.Configure(builder);
        }
    }
}
