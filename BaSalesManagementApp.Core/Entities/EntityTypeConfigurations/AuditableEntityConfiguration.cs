namespace BaSalesManagementApp.Core.Entities.EntityTypeConfigurations
{
    public class AuditableEntityConfiguration<T>: BaseEntityConfiguration<T> where T : AuditableEntity
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.DeletedBy).IsRequired(false);
            builder.Property(x => x.DeletedDate).IsRequired(false);
        }
    }
}
