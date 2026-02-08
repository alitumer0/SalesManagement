using BaSalesManagementApp.MVC.Models.OrderVMs;

namespace BaSalesManagementApp.MVC.Models.EmployeeVMs
{
    public class EmployeeOrderVM
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<OrderListVM> Orders { get; set; } = new List<OrderListVM>();
        public List<DailyOrderSummaryVM> DailyOrderSummary { get; set; } = new List<DailyOrderSummaryVM>();

    }
}
