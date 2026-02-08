using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.OrderVMs
{
    public class OrderListVM
    {
        public Guid Id { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Admin Name")]
        public string AdminName { get; set; }

        [Display(Name = "Order No")]
        public string OrderNo { get; set; }

        public string CustomerName { get; set; }

        public string CompanyName { get; set; }

        public List<OrderDetailListDTO> OrderDetails { get; set; } = new List<OrderDetailListDTO>();
    }
}