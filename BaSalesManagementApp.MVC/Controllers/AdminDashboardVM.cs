namespace BaSalesManagementApp.MVC.Models.AdminVMs
{
    public class AdminDashboardVM
    {
        public int TotalAdmins { get; set; }
        public int NewAdminsThisMonth { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalOrderAmount { get; set; }
    }
}
