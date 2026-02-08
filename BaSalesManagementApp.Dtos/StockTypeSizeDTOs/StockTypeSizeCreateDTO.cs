using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.StockTypeSizeDTOs
{
    public class StockTypeSizeCreateDTO
    {
        public string Size { get; set; }
        public string Description { get; set; }
        public Guid StockTypeId { get; set; }
    }
}
