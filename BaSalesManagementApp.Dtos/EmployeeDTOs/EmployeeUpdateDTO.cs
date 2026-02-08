using BaSalesManagementApp.Core.Enums;

namespace BaSalesManagementApp.Dtos.EmployeeDTOs
{
    public class EmployeeUpdateDTO
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

		public byte[]? PhotoData { get; set; }

        public Roles Role { get; set; }

        public string? Title => Role.ToString();

        public Guid CompanyId { get; set; }
        public bool IsPhotoRemoved { get; set; }
    }
}
