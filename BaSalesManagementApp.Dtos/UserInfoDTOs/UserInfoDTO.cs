using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.UserInfoDTOs
{
    public class UserInfoDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[]? PhotoData { get; set; }
        public string Role { get; set; }
    }
}
