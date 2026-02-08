using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.ProductTypeDtos;

namespace BaSalesManagementApp.MVC.Models.StockTypeSizeVMs
{
    public class StockTypeSizeCreateVM
    {

        public string Size { get; set; } = null!;
        public string Description { get; set; } = null!;

        public Guid? StockTypeId { get; set; }
        public List<StockTypeDto>? StockTypes { get; set; }

    }
}
