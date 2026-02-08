namespace BaSalesManagementApp.MVC.Models.AdminVMs
{
    public class AdminListVM
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public byte[]? PhotoData { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}