using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.StockTypeSizeDTOs
{
    public class StockTypeSizeDTO
    {
        public Guid Id { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? StockTypeId { get; set; }
        public string? StockTypeName { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
