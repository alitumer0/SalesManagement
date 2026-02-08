using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Core.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class CategoryRepository : EFBaseRepository<Category>, ICategoryRepository
    {
        private readonly BaSalesManagementAppDbContext _dbContext;

        public CategoryRepository(BaSalesManagementAppDbContext context) : base(context)
        {
            _dbContext = context;
        }
        public async Task<List<Category>> GetCategoriesByCompanyIdAsync(Guid companyId)
        {
            var categories = await _dbContext.Categories
                .Where(p => p.CompanyId == companyId)
                .ToListAsync();

            return categories;
        }
    }
}
