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
    public class EmployeeConfiguration : AuditableEntityConfiguration<Employee>
    {
        public override void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.CompanyId).IsRequired();
            base.Configure(builder);


        }
    }
}
