namespace BaSalesManagementApp.Dtos.AdminDTOs
{
    public class AdminDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[]? PhotoData { get; set; }
    }
}
