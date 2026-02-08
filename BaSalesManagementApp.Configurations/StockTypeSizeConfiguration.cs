using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entities.Configurations
{
    public class StockTypeSizeConfiguration : AuditableEntityConfiguration<StockTypeSize>
    {
        public override void Configure(EntityTypeBuilder<StockTypeSize> builder)
        {
            base.Configure(builder);

            builder.Property(prdctType => prdctType.Size).IsRequired().HasMaxLength(128);
            builder.Property(prdctType => prdctType.Description).HasMaxLength(128);
        }
    }
}
