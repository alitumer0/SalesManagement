namespace BaSalesManagementApp.MVC.Models.PaymentTypeVMs
{
    public class PaymentTypeListVM
    {
        public Guid Id { get; set; }
        public decimal Rate { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
