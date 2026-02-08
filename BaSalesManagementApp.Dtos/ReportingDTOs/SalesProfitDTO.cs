using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.ReportingDTOs
{
    public class SalesProfitDTO
    {
        // "Sales" veya "Profit"
        public string Metric { get; init; } = "";

        // Hesaplanan mevcut dönem aralığı
        public DateTime Start { get; init; }
        public DateTime End { get; init; }

        // İsteğe bağlı bağlam
        public Guid? CompanyId { get; init; }

        // Toplam / KPI alanları
        public decimal TotalTL { get; init; }   // pratik: CurrentTL ile aynı dolduruyoruz
        public decimal CurrentTL { get; init; }
        public decimal PreviousTL { get; init; }
        public decimal ChangePct { get; init; }
    }
}
