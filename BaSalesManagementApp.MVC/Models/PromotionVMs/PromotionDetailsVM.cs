using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.PromotionVMs
{
    public class PromotionDetailsVM
    {
        public Guid Id { get; set; }
        public int Discount { get; set; }
        public decimal Price { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }

        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }
    }
}
