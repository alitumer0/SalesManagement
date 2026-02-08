using BaSalesManagementApp.Core.Enums;

namespace BaSalesManagementApp.MVC.Models.EmployeeVMs
{
    public class EmployeeUpdateVM
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

		public IFormFile? Photo { get; set; }

		public byte[]? PhotoData { get; set; }
        public string? PhotoUrl { get; set; }

        public Roles? Role { get; set; }

        public Guid CompanyId { get; set; }
        public bool IsPhotoRemoved { get; set; }

        public string? _title;

        public string? Title
        {
            get
            {
                if ((Role == 0 || Role == null) && _title != null)
                {
                    return _title;
                }
                return Role.ToString();
            }
            set
            {
                if (Role == 0 || Role == null)
                {
                    _title = value;
                }
            }
        }
    }
}
