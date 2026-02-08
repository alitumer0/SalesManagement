using BaSalesManagementApp.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BaSalesManagementApp.Dtos.CompanyDTOs;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class BadgeRepository: EFBaseRepository<Badge>, IBadgeRepository
    {
        private readonly BaSalesManagementAppDbContext _dbContext;

        public BadgeRepository(BaSalesManagementAppDbContext context): base(context)
        {
            _dbContext = context;
        }

        public Task<List<Badge>> GetBadgesByCompanyIdAsync(Guid? companyId)
        {
            var badges = _dbContext.Badges
            .Where(p => p.CompanyId == companyId)
            .ToListAsync();

            return badges;
        }

        public async Task<(IEnumerable<Badge> badges, int totalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _dbContext.Badges.AsNoTracking();

            var totalCount = await query.CountAsync();

            var badges = await query
                .OrderByDescending(b => b.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (badges, totalCount);
        }

        public async Task<(IEnumerable<Badge> badges, int totalCount)> SearchPagedAsync(string searchQuery, int pageNumber, int pageSize)
        {
            var query = _dbContext.Badges
                .AsNoTracking()
                .Where(b => b.Name.Contains(searchQuery));

            var totalCount = await query.CountAsync();

            var badges = await query
                .OrderByDescending(b => b.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (badges, totalCount);
        }

        public new async Task<IEnumerable<Badge>> GetAllAsync()
        {
            return await _dbContext.Badges
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
