using BaSalesManagementApp.Entites.DbSets;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class CustomerRepository : EFBaseRepository<Customer>, ICustomerRepository
    {
        private readonly BaSalesManagementAppDbContext _context;

        public CustomerRepository(BaSalesManagementAppDbContext context) : base(context)
        {
            _context = context;

        }

        public async Task<IEnumerable<Order>> GetAllAsync<TKey>(Expression<Func<Order, bool>> expression, Expression<Func<Order, TKey>> orderby, bool orderDesc = false, bool tracking = true)
        {
            var query = _context.Orders
                        .Where(expression)
                        .Include(o => o.OrderDetails) // OrderDetails dahil edilir
                        .Include(o => o.Admin) // Admin dahil edilir
                        .AsQueryable();

            //Eğer verilerin sadece okunması istenildiyse,bu veri üzerinde bir değişiklik yapılmasına izin verilmediği taktirde sağlanan kontrol
            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            query = orderDesc ? query.OrderByDescending(orderby) : query.OrderBy(orderby);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomersByCompany(Guid companyId)
        {
            var customerExists = await _context.Customers.AnyAsync(c => c.CompanyId == companyId);

            if (!customerExists)
            {
                return Enumerable.Empty<Customer>();
            }

            var query = await _context.Companies
                        .Include(c => c.Customers)
                        .SingleOrDefaultAsync(c => c.Id == companyId);


            return query.Customers.ToList();

        }
    }
}
