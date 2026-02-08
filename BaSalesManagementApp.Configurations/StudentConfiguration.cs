namespace BaSalesManagementApp.Entities.Configurations
{
    public class StudentConfiguration: AuditableEntityConfiguration<Student>
    {
        public override void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Age).IsRequired();
            base.Configure(builder);
        }
    }
}
