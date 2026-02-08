using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.CityDTOs
{
    public class CityListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public Guid CountryId { get; set; }

        public string CountryName { get; set; }
        public DateTime CreatedDate { get; set; }
        
    }
}
