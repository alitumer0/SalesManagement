using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entities.Configurations
{
    public class PaymentTypeConfiguration : AuditableEntityConfiguration<PaymentType>
    {
        public override void Configure(EntityTypeBuilder<PaymentType> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Rate).IsRequired().HasPrecision(18,2);
            base.Configure(builder);
        }
    }
}
