using BaSalesManagementApp.Dtos.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.EmployeeDTOs
{
    public class EmployeeOrderHistoryDto
    {
        public string EmployeeName { get; set; }
        public List<OrderDTO> Orders { get; set; }
    }
}
