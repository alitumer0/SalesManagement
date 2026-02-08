using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.OrderVMs
{
    public class OrderUpdateVM
    {
        public Guid Id { get; set; }
        [Display(Name ="Order No")]
        public string OrderNo { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        public bool IsActive { get; set; }

        public Guid AdminId { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        public List<ProductListDTO>? Products { get; set; }
        public List<OrderDetailUpdateDTO> OrderDetails { get; set; }

















    }
}