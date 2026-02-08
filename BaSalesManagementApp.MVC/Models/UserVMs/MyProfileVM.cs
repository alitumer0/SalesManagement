namespace BaSalesManagementApp.MVC.Models.UserVMs
{
    public class MyProfileVM
    {
        public Guid Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public IFormFile? Photo { get; set; }
        public byte[]? PhotoData { get; set; }
        public bool RemovePhoto { get; set; } // Fotoğraf kaldırılacak mı
    }
}
