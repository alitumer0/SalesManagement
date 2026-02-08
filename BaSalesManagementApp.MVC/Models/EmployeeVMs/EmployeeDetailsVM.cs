using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.EmployeeVMs
{
    public class EmployeeDetailsVM
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

		public byte[]? PhotoData { get; set; }

        public string? Title { get; set; }

        [Display(Name = "Company Name")]
        public Guid CompanyId { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

    }
}
