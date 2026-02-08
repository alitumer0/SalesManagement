using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.ProductTypeDtos;

namespace BaSalesManagementApp.MVC.Models.StockTypeSizeVMs
{
    public class StockTypeSizeUpdateVM
    {
        public Guid Id { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public Guid? StockTypeId { get; set; }
        public List<StockTypeDto>? StockTypes { get; set; }

    }
}
