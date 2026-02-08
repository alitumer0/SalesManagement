using BaSalesManagementApp.Core.Entities.EntityTypeConfigurations;
using BaSalesManagementApp.Entites.DbSets;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entities.Configurations
{
    public class AdminConfiguration : AuditableEntityConfiguration<Admin>
    {
        public override void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(128);
            base.Configure(builder);
        }
    }
}
