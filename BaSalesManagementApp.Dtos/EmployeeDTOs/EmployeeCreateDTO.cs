using BaSalesManagementApp.Core.Enums;

namespace BaSalesManagementApp.Dtos.EmployeeDTOs
{
    public class EmployeeCreateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[]? PhotoData { get; set; }
        public string Title { get; set; }
        public Guid CompanyId { get; set; }

    }
}
