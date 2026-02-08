namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IEmployeeRepository :
        IAsyncRepository, IRepository,
        IAsyncTransactionRepository,
        IAsyncUpdateableRepository<Employee>,
        IAsyncDeletableRepository<Employee>,
        IAsyncFindableRepository<Employee>,
        IAsyncInsertableRepository<Employee>,
        IAsyncOrderableRepository<Employee>,
        IAsyncQueryableRepository<Employee>,
        IDeletableRepository<Employee>
    {
        Task<Employee?> GetByIdentityId(string identityId);

        /// <summary>
        /// Belirtilen şirkete ait çalışanların listesini getirir.
        /// </summary>
        /// <param name="companyId">Şirketin benzersiz kimlik numarası (GUID).</param>
        /// <param name="tracking">Takip modunda mı onagöre çalışacağını belirtir. Varsayılan olarak true.</param>
        /// <returns>Çalışanların koleksiyonu.</returns>
        Task<IEnumerable<Employee>> GetByCompanyIdAsync(Guid companyId, bool tracking = true);
        Task<Guid?> GetCompanyIdByUserIdAsync(Guid userId);
    }
}
