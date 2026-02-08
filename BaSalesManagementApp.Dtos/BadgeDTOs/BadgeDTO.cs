using BaSalesManagementApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.BadgeDTOs
{
    public class BadgeDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public Status Status { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
