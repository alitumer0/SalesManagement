using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.StockVMs
{
    public class StockUpdateVM
    {
        public Guid Id { get; set; }
        [Range(0, 999999999, ErrorMessage = "Stok sayısı 9 haneden fazla olamaz.")]
        public int? Count { get; set; }
        public Guid ProductId { get; set; }
        public List<ProductDTO>? Products { get; set; }
        public Guid WarehouseId { get; set; }
        public List<WarehouseListVM>? Warehouses { get; set; }
    }
}
