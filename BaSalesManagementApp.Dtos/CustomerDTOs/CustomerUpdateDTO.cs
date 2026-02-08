using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.CustomerDTOs
{
    public class CustomerUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Email { get; set; }
        public string Phone { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public Guid CityId { get; set; }
        public Guid CountryId { get; set; }
    }
}
