using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entities.Configurations
{
    public class BadgeConfiguration : AuditableEntityConfiguration<Badge>
    {
        public override void Configure(EntityTypeBuilder<Badge> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            builder.Property(x => x.CompanyId).IsRequired();
            builder.Property(x => x.SalesQuantity).IsRequired();
            base.Configure(builder);
        }
    }
}
