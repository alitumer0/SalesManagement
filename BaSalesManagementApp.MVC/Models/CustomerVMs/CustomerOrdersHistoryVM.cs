using BaSalesManagementApp.MVC.Models.OrderVMs;

namespace BaSalesManagementApp.MVC.Models.CustomerVMs
{
    public class CustomerOrdersHistoryVM
    {
        public Guid customerId { get; set; }
        public string? CustomerName { get; set; }
        public List<OrderListVM> Orders { get; set; }
    }
}
