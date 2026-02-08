using BaSalesManagementApp.MVC.Models.OrderVMs;

namespace BaSalesManagementApp.MVC.Models.EmployeeVMs
{
    public class EmployeeOrdersHistoryVM
    {
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; } // Personelin Adı
        public List<OrderListVM> Orders { get; set; } = new List<OrderListVM>();
    }
}
