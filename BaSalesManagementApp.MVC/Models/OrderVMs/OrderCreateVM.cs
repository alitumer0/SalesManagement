using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CustomerDTOs;
using BaSalesManagementApp.Dtos.EmployeeDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.PaymentTypeDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Entites.DbSets;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.OrderVMs
{
    public class OrderCreateVM
    {
        public OrderCreateVM()
        {
            PaymentTypes = new List<PaymentTypeListDTO>();
        }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }


        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public bool IsActive { get; set; }
        public Guid AdminId { get; set; }
		public Guid CustomerId { get; set; }
        public Guid? CompanyId { get; set; }
        public List<CompanyDTO>? Companies { get; set; } = new List<CompanyDTO>();
        public List<OrderDetailCreateDTO> OrderDetails { get; set; }/* = new List<OrderDetailCreateDTO>();*/

        public List<EmployeeListDTO> Employees { get; set; } = new List<EmployeeListDTO>(); // Çalışanlar listesi

        public List<ProductListDTO> Products { get; set; } = new List<ProductListDTO>();
        public List<CustomerListDTO> Customers { get; set; } = new List<CustomerListDTO>();
        public List<PaymentTypeListDTO>? PaymentTypes { get; set; } = new List<PaymentTypeListDTO>();
        public Guid? PaymentTypeId { get; set; } // Seçilen ödeme türü

        [Display(Name = "Currency")]
        public CurrencyType CurrencyType { get; set; } = CurrencyType.TRY;
    }
}