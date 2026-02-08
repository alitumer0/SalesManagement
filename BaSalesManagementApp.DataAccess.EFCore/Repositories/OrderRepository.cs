using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaSalesManagementApp.Core.Enums;
using System;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class OrderRepository : EFBaseRepository<Order>, IOrderRepository
    {
        private readonly BaSalesManagementAppDbContext _context;
        public OrderRepository(BaSalesManagementAppDbContext context) : base(context)
        {
            _context = context;
        }
        /// <summary>
        /// Orders ile Admins, Products ve Companies tablolarını birleştirerek siparişleri listeler. Bu listede Admin bilgisinin ve aktif firmalarınn olduğu siparişlerin de gösterilmesini sağlar.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetAllWithAdminAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Admin)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Company)  // Company Name & Status Durumu İçin
                .Where(x => x.Status != Status.Deleted)
                .ToListAsync();

            return orders;
        }

        /// <summary>
        /// Orders ile Admins, Products ve Companies tablolarını birleştirerek aranan siparişe dahil eder.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<Order> GetOrderWithAdminAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Admin)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Company)
                .Where(x => x.Status != Status.Deleted && x.Id == orderId)
                .FirstOrDefaultAsync();

            return order;
        }

        public async Task<Order> GetByIdWithDetailsAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails) // Sipariş detaylarını getir
                    .ThenInclude(od => od.Product) // Her detay için ürün bilgilerini getir
                    .ThenInclude(p => p.Company) // Ürüne ait şirket bilgilerini getir
                .Where(o => o.Status != Status.Deleted && o.Id == orderId) // Silinmemiş ve belirtilen ID'ye sahip siparişi filtrele
                .FirstOrDefaultAsync(); // İlk eşleşen kaydı döndür
        }
        public async Task<IEnumerable<Order>> GetOrdersByCompany(Guid companyId, bool tracking = true)
        {
            var query = _context.Orders
                .Include(o => o.Company) // İlişkili Şirket verisini dahil et
                .Include(o => o.OrderDetails)
                .Where(o => o.CompanyId == companyId); // Şirket ID'sine göre filtreleme

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Belirtilen çalışana ait siparişleri getirir.
        /// </summary>
        /// <param name="employeeId">Çalışanın benzersiz kimlik numarası.</param>
        /// <param name="tracking">Takip modunda olup olmayacağını belirler. Eğer `false` ise takip edilmeyen bir sorgu döndürülür.</param>
        /// <returns>Belirtilen çalışana ait siparişlerin bir listesini döndürür.</returns>
        public async Task<IEnumerable<Order>> GetOrdersByEmployee(Guid employeeId, bool tracking = true)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == employeeId);
            if (!employeeExists)
            {
                return Enumerable.Empty<Order>();
            }

            var query = _context.Orders
     .Include(o => o.Employee) // Employee ilişkisini dahil et
     .Include(o => o.OrderDetails) // OrderDetails ilişkisini dahil et
     .Where(o => o.EmployeeId == employeeId) // Null kontrolü
     .DefaultIfEmpty();
            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }
        public async Task<Order> AddAsync(Order order)
        {
            try
            {
                //benzersiz bir Sipariş numarası oluşturma
                var today = DateTime.UtcNow.ToString("yyyyMMdd");
                var orderCountToday = _context.Orders
                    .Count(o => o.OrderDate.Date == DateTime.UtcNow.Date);

                order.OrderNo = $"ORD-{today}-{orderCountToday + 1:D4}";
                var entry = await _context.Orders.AddAsync(order);
                return entry.Entity;
            }
            catch (Exception ex)
            {

                throw new Exception($"Order eklenirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<List<Order>> GetAllWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Company)
                .ToListAsync();
        }



        public IQueryable<Order> QueryActiveInRange(DateTime start, DateTime endInclusive)
        {
            return _context.Orders
                .Where(o =>
                    o.Status != Status.Deleted && //silinmemiş sipariş                    
                    o.OrderDate >= start &&       //başlangıç tarihi dahil
                    o.OrderDate <= endInclusive     //bitiş tarihi dahil
                );
        }

    }
}
