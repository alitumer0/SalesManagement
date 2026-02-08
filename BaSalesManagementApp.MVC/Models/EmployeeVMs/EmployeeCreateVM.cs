using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaSalesManagementApp.MVC.Models.EmployeeVMs
{
    public class EmployeeCreateVM
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public IFormFile? Photo { get; set; }
        public string Title { get; set; }
        public Guid CompanyId { get; set; }
        public List<SelectListItem>? RoleOptions { get; set; }
        public List<SelectListItem>? CompanyOptions { get; set; }
    }
}