using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entities.Configurations
{
	public class CityConfiguration : AuditableEntityConfiguration<City>
	{
		public override void Configure(EntityTypeBuilder<City> builder)
		{
			builder.Property(c => c.Name).IsRequired().HasMaxLength(30);
			builder.Property(c => c.CountryId).IsRequired();

			builder.HasOne(x => x.Country);
			
			base.Configure(builder);
		}
	}
}
