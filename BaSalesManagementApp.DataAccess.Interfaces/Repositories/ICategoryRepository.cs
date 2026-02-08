using BaSalesManagementApp.Core.Utilities.Results;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface ICategoryRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<Category>, IAsyncDeletableRepository<Category>, IAsyncFindableRepository<Category>, IAsyncInsertableRepository<Category>, IAsyncOrderableRepository<Category>, IAsyncQueryableRepository<Category>, IDeletableRepository<Category>
    {
        Task<List<Category>> GetCategoriesByCompanyIdAsync(Guid companyId);
    }
}
