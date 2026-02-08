namespace BaSalesManagementApp.Core.Entities.EntityTypeConfigurations
{
    public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedBy).IsRequired();
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.ModifiedBy).IsRequired(false);
            builder.Property(x => x.ModifiedDate).IsRequired(false);
           

        }
    }
}
