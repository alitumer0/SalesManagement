using BaSalesManagementApp.Dtos.CompanyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IBadgeRepository :
        IAsyncRepository, IRepository,
        IAsyncTransactionRepository,
        IAsyncUpdateableRepository<Badge>,
        IAsyncDeletableRepository<Badge>,
        IAsyncFindableRepository<Badge>,
        IAsyncInsertableRepository<Badge>,
        IAsyncOrderableRepository<Badge>,
        IAsyncQueryableRepository<Badge>,
        IDeletableRepository<Badge>
    {
        Task<List<Badge>> GetBadgesByCompanyIdAsync(Guid? companyId);
    }
}
