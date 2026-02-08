using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BaSalesManagementApp.Entities.Configurations
{
    public class CurrentExchangeRateConfiguration : AuditableEntityConfiguration<CurrentExchangeRate>
    {
        public override void Configure(EntityTypeBuilder<CurrentExchangeRate> builder)
        {
           
            builder.Property(x => x.DollarRate).IsRequired();
            builder.Property(x => x.EuroRate).IsRequired();
            builder.Property(x => x.CreatedDate).IsRequired();
            base.Configure(builder);
        }
    }
}