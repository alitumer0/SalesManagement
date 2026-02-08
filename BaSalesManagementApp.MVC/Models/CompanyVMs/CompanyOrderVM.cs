using BaSalesManagementApp.MVC.Models.OrderVMs;

namespace BaSalesManagementApp.MVC.Models.CompanyVMs
{
    public class CompanyOrderVM
    {
        public Guid CompanyId { get; set; }
        public List<OrderListVM> Orders { get; set; }
        public string? CompanyName { get; set; }

    }
}
