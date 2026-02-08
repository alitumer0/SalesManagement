using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entities.Configurations
{
    public class CountryConfiguration: AuditableEntityConfiguration<Country>
    {
        public override void Configure(EntityTypeBuilder<Country> builder)
        {
          
            builder.Property(c => c.NameTr).IsRequired().HasMaxLength(30);
            builder.Property(c => c.NameEn).IsRequired().HasMaxLength(30);
            base.Configure(builder);    
        }
    }
}
