using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.StockTypeSizeDTOs
{
    public class StockTypeSizeListDTO
    {
        public Guid Id { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public string? StockTypeName { get; set; }

    }
}
