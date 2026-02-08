using BaSalesManagementApp.Entites.DbSets;
namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IAdminRepository:
        IAsyncRepository, IRepository,
        IAsyncTransactionRepository,
        IAsyncUpdateableRepository<Admin>,
        IAsyncDeletableRepository<Admin>,
        IAsyncFindableRepository<Admin>,
        IAsyncInsertableRepository<Admin>,
        IAsyncOrderableRepository<Admin>,
        IAsyncQueryableRepository<Admin>,
        IDeletableRepository<Admin>
    {
        Task<Admin?> GetByIdentityId(string identityId);
        Task<(List<Admin> Items, int Total)> GetPagedAsync(string? search, string sort, int page, int pageSize);

        Task<int> CountAsync();
        Task<int> CountNewThisMonthAsync();
    }
}
