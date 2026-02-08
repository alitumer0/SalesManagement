using BaSalesManagementApp.Core.Utilities.Results;
using BaSalesManagementApp.Dtos.CompanyDTOs;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface ICompanyRepository: IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<Company>, IAsyncDeletableRepository<Company>, IAsyncFindableRepository<Company>, IAsyncInsertableRepository<Company>, IAsyncOrderableRepository<Company>, IAsyncQueryableRepository<Company>, IDeletableRepository<Company>
    {

        /// <summary>
        /// Belirtilen şirketi pasif duruma getiren asenkron işlemi gerçekleştirir.
        /// </summary>
        /// <param name="companyId">Pasif duruma getirilecek şirketin benzersiz kimliği.</param>
        /// <returns>
        /// İşlemin sonucunu temsil eden bir IResult döner.
        /// Başarılı ise başarı durumunu, başarısız ise hata mesajını içerir.
        /// </returns>
        Task<IResult> SetCompanyToPassiveAsync(Guid companyId);
        Task<List<Company>> GetCompaniesByIdsAsync(IEnumerable<Guid> companyIds);
    }
}
