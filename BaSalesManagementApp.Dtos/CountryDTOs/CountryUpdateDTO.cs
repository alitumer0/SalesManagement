using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.CountryDTOs
{
    public class CountryUpdateDTO
    {
        public Guid Id { get; set; }
        public string NameTr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

    }
}
