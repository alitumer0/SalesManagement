namespace BaSalesManagementApp.Dtos.EmployeeDTOs
{
    public class EmployeeListDTO
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

		public byte[]? PhotoData { get; set; }

        public string? Title { get; set; }

        public Guid CompanyId { get; set; }

        public string CompanyName { get; set; }

        public byte[]? CompanyPhoto { get; set; } = null!;

        public DateTime CreatedDate { get; set; }
    }
}
