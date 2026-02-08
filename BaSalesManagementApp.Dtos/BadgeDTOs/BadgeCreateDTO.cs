using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.BadgeDTOs
{
    public class BadgeCreateDTO
    {
        public string Name { get; set; } = null!;
        public int SalesQuantity { get; set; }
        public Guid CompanyId { get; set; }
    }
}
