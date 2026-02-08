using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.OrderDetailDTOs
{
    public class OrderDetailListDTO
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool IsCompanyActive { get; set; }
        public string CompanyName { get; set; }
        public string ProductName { get; set; }
        public Status? Status { get; set; }
        public CompanyDTO CompanyDTO { get; set; } = new CompanyDTO();
        
    }
}
