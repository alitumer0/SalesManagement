namespace BaSalesManagementApp.MVC.Models.EmployeeVMs
{
    public class DailyOrderSummaryVM
    {
        public DateTime OrderDate { get; set; }
        public int TotalOrders { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalDailyPrice { get; set; }
    }
}
