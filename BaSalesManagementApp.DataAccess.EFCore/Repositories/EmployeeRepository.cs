
using BaSalesManagementApp.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class EmployeeRepository : EFBaseRepository<Employee>, IEmployeeRepository
    {
        private readonly BaSalesManagementAppDbContext _dbContext;

        public EmployeeRepository(BaSalesManagementAppDbContext context, BaSalesManagementAppDbContext dbContext) : base(context)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Employee>> GetByCompanyIdAsync(Guid companyId, bool tracking = true)
        {
            Console.WriteLine($"Fetching employees for CompanyId: {companyId}");

            // Status enum'ı doğru referanslandıktan sonra sorgu şu şekilde çalışmalıdır:
            var query = _table.Where(e => e.CompanyId == companyId && e.Status != Status.Deleted);

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            var employees = await query.ToListAsync();

            Console.WriteLine($"Found {employees.Count} employees for CompanyId: {companyId}");
            return employees;
        }

        public Task<Employee?> GetByIdentityId(string identityId)
        {
            return _table.FirstOrDefaultAsync(x => x.IdentityId == identityId);
        }
        public async Task<Guid?> GetCompanyIdByUserIdAsync(Guid userId)
        {
            var companyId = await _dbContext.Employees
                .Where(e => e.IdentityId == userId.ToString())
                .Select(e => e.CompanyId)
                .FirstOrDefaultAsync();

            return companyId;
        }


    }
}
