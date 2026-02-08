namespace BaSalesManagementApp.Entities.Configurations
{
    public class CategoryConfiguration : AuditableEntityConfiguration<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            base.Configure(builder);
        }
    }
}
