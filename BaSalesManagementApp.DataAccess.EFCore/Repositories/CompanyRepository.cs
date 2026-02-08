using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Core.Utilities.Results;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using Microsoft.EntityFrameworkCore;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class CompanyRepository : EFBaseRepository<Company>, ICompanyRepository
    {
        private readonly BaSalesManagementAppDbContext _dbContext;
        public CompanyRepository(BaSalesManagementAppDbContext context) : base(context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Belirtilen şirketin durumunu pasif olarak ayarlayan asenkron işlemi gerçekleştirir.
        /// </summary>
        /// <param name="companyId">Pasif duruma getirilecek şirketin benzersiz kimliği.</param>
        /// <returns>
        /// Şirketin durumunu pasif olarak başarıyla ayarladığında başarı mesajı içeren bir IResult döner.
        /// Şirket bulunamazsa hata mesajı içeren bir sonuç döner.
        /// </returns>

        public async Task<IResult> SetCompanyToPassiveAsync(Guid companyId)
        {
            var company = await GetByIdAsync(companyId);
            if (company == null)
            {
                return new ErrorResult("Company not found");
            }

            // Sadece Status alanını güncelle
            company.Status = Status.Passive;

            // Şirketv güncellerken sadece Status alanının güncellenmesini sağlamak için:
            var entry = _dbContext.Entry(company);
            entry.Property(p => p.Status).IsModified = true;

            // Diğer tüm alanları değiştirilmemiş (Unchanged) olarak işaretle
            foreach (var property in entry.Properties)
            {
                if (property.Metadata.Name != nameof(company.Status))
                {
                    property.IsModified = false;
                }
            }

            await _dbContext.SaveChangesAsync();
            return new SuccessResult("Company status set to passive successfully");
        }

        public async Task<List<Company>> GetCompaniesByIdsAsync(IEnumerable<Guid> companyIds)
        {
            return await _dbContext.Companies
                .AsNoTracking()
                .Where(c => companyIds.Contains(c.Id))
                .Select(c => new Company
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}