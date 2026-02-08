
using Microsoft.EntityFrameworkCore;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class BranchRepository : EFBaseRepository<Branch>, IBranchRepository
    {
        private readonly BaSalesManagementAppDbContext _dbContext;

        public BranchRepository(BaSalesManagementAppDbContext context) : base(context)
        {
            _dbContext = context;

        }

        public Task<List<Branch>> GetBranchesByCompanyIdAsync(Guid? companyId)
        {
            var branches = _dbContext.Branches
            .Where(p => p.CompanyId == companyId)
            .ToListAsync();

            return branches;
        }
    }
}
