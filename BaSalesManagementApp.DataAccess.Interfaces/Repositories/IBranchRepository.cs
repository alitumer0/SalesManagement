
namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IBranchRepository :
        IAsyncRepository, IRepository,
        IAsyncTransactionRepository,
        IAsyncUpdateableRepository<Branch>,
        IAsyncDeletableRepository<Branch>,
        IAsyncFindableRepository<Branch>,
        IAsyncInsertableRepository<Branch>,
        IAsyncOrderableRepository<Branch>,
        IAsyncQueryableRepository<Branch>,
        IDeletableRepository<Branch>
    {
        Task<List<Branch>> GetBranchesByCompanyIdAsync(Guid? companyId);
    }
}
