namespace BaSalesManagementApp.MVC.Models.AdminVMs
{
    public class AdminCreateVM
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
