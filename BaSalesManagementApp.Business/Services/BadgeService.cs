using BaSalesManagementApp.Dtos.BadgeDTOs;
using BaSalesManagementApp.Dtos.BranchDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Business.Services
{
    public class BadgeService: IBadgeService
    {

        private readonly IBadgeRepository _badgeRepository;
        private readonly ICompanyRepository _companyRepository;

        public BadgeService(IBadgeRepository branchRepository, ICompanyRepository companyRepository)
        {
            _badgeRepository = branchRepository;
            _companyRepository = companyRepository;
        }


        //Yeni bir rozet ekler ve işlem başarılıysa eklenen rozeti döndürür. Eğer bir hata oluşursa, uygun bir hata mesajıyla birlikte hata durumunu döndürür.
        public async Task<IDataResult<BadgeDTO>> AddAsync(BadgeCreateDTO badgeCreateDTO)
        {
            try
            {
                var newBadge = badgeCreateDTO.Adapt<Badge>();

                await _badgeRepository.AddAsync(newBadge);
                await _badgeRepository.SaveChangeAsync();

                return new SuccessDataResult<BadgeDTO>(newBadge.Adapt<BadgeDTO>(), Messages.BADGE_ADD_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<BadgeDTO>(Messages.BADGE_ADD_ERROR);
            }
        }

        //Belirli bir company kimliğine ait rozetleri döndürür. 
        public async Task<List<Badge>> GetBadgesByCompanyIdAsync(Guid companyId)
        {
            //var allBadges = await _badgeRepository.GetAllAsync();
            //return allBadges.Where(badge => badge.CompanyId == companyId).ToList();

            //gereksiz yere tüm veriyi çekip bellekte tutması performans kaybı yaratabilir

            return await _badgeRepository.GetBadgesByCompanyIdAsync(companyId);
        }

        //Belirtilen bir rozeti siler ve işlem başarılıysa başarılı bir mesaj döndürür.Herhangi bir hata oluşursa uygun bir hata mesajıyla birlikte hata durumunu döndürür.

        public async Task<IResult> DeleteAsync(Guid badgeId)
        {
            try
            {
                var deletingBadge = await _badgeRepository.GetByIdAsync(badgeId);

                await _badgeRepository.DeleteAsync(deletingBadge);
                await _badgeRepository.SaveChangeAsync();

                return new SuccessResult(Messages.BADGE_DELETE_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.BADGE_DELETE_ERROR);
            }
        }


        public async Task<IDataResult<List<BadgeListDTO>>> GetAllAsync()
        {
            try
            {
                var badges = await _badgeRepository.GetAllAsync();

                if (!badges.Any())
                {
                    return new ErrorDataResult<List<BadgeListDTO>>(new List<BadgeListDTO>(), Messages.BADGE_LISTED_NOTFOUND);
                }

                // Sadece ilgili companyidleri için entity çekiliyor
                var companyIds = badges.Select(b => b.CompanyId).Distinct();
                var companies = await _companyRepository.GetCompaniesByIdsAsync(companyIds);

                var badgeListDTOs = badges.Select(badge =>
                {
                    var company = companies.FirstOrDefault(c => c.Id == badge.CompanyId);

                    return new BadgeListDTO
                    {
                        Id = badge.Id,
                        Name = badge.Name,
                        CompanyId = badge.CompanyId,
                        CompanyName = company?.Name ?? Messages.BADGE_COMPANY_LISTED_DELETED,
                        CompanyPhoto = null,
                        CreatedDate = badge.CreatedDate
                    };
                }).ToList();

                return new SuccessDataResult<List<BadgeListDTO>>(badgeListDTOs, Messages.BADGE_LISTED_SUCCESS);
            }

            catch (Exception)
            {
                return new ErrorDataResult<List<BadgeListDTO>>(new List<BadgeListDTO>(), Messages.BADGE_LISTED_ERROR);
            }
        }

        //Belirli bir rozet kimliğine göre rozeti getirir.Şube bulunamazsa uygun bir hata mesajıyla birlikte hata durumunu döndürür.
        public async Task<IDataResult<BadgeDTO>> GetByIdAsync(Guid badgeId)
        {
            try
            {
                var badge = await _badgeRepository.GetByIdAsync(badgeId);
                if (badge == null)
                {
                    return new ErrorDataResult<BadgeDTO>(Messages.BADGE_GETBYID_ERROR);
                }
                var badgeDTO = badge.Adapt<BadgeDTO>();
                var company = await _companyRepository.GetByIdAsync(badge.CompanyId);

                if (company != null)
                {
                    badgeDTO.CompanyName = company.Name;
                }

                return new SuccessDataResult<BadgeDTO>(badge.Adapt<BadgeDTO>(), Messages.BADGE_GETBYID_SUCCESS);
            }
            catch
            {
                return new ErrorDataResult<BadgeDTO>(Messages.BADGE_GETBYID_ERROR);
            }
        }


        //Belirli bir rozet kimliğine göre rozet bilgilerini günceller.Güncelleme başarılıysa güncellenen rozet bilgilerini döndürür.Herhangi bir hata oluşursa uygun bir hata mesajıyla birlikte hata durumunu döndürür.
        public async Task<IDataResult<BadgeDTO>> UpdateAsync(BadgeUpdateDTO badgeUpdateDTO)
        {
            try
            {
                var updatingBadge = await _badgeRepository.GetByIdAsync(badgeUpdateDTO.Id);

                if (updatingBadge == null)
                {
                    return new ErrorDataResult<BadgeDTO>(Messages.BADGE_GETBYID_ERROR);
                }

                var company = await _companyRepository.GetByIdAsync(badgeUpdateDTO.CompanyId);

                if (company == null)
                {
                    return new ErrorDataResult<BadgeDTO>(Messages.COMPANY_GETBYID_ERROR);
                }

                await _badgeRepository.UpdateAsync(badgeUpdateDTO.Adapt(updatingBadge));
                await _badgeRepository.SaveChangeAsync();

                return new SuccessDataResult<BadgeDTO>(updatingBadge.Adapt<BadgeDTO>(), Messages.BADGE_UPDATE_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<BadgeDTO>(Messages.BADGE_UPDATE_ERROR);
            }
        }


        public async Task<IDataResult<List<BadgeListDTO>>> GetAllAsync(string searchQuery)
        {
            try
            {
                var badges = await _badgeRepository.GetAllAsync();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    badges = badges.Where(b => b.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
                }

                if (!badges.Any())
                {
                    return new ErrorDataResult<List<BadgeListDTO>>(new List<BadgeListDTO>(), Messages.BADGE_LISTED_NOTFOUND);
                }

                var companyIds = badges.Select(b => b.CompanyId).Distinct();
                var companies = await _companyRepository.GetCompaniesByIdsAsync(companyIds);

                var badgeListDTOs = badges.Select(badge =>
                {
                    var company = companies.FirstOrDefault(c => c.Id == badge.CompanyId);

                    return new BadgeListDTO
                    {
                        Id = badge.Id,
                        Name = badge.Name,
                        CompanyId = badge.CompanyId,
                        CompanyName = company?.Name ?? Messages.BADGE_COMPANY_LISTED_DELETED,
                        CompanyPhoto = null,
                        CreatedDate = badge.CreatedDate
                    };
                }).ToList();

                return new SuccessDataResult<List<BadgeListDTO>>(badgeListDTOs, Messages.BADGE_LISTED_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<BadgeListDTO>>(new List<BadgeListDTO>(), Messages.BADGE_LISTED_ERROR);
            }
        }

        public async Task<IDataResult<List<BadgeListDTO>>> GetBadgesByCompanyIdAsynca(Guid companyId)
        {
            try
            {
                var badges = await _badgeRepository.GetBadgesByCompanyIdAsync(companyId);

                // Status değeri 3 olmayanları filtrele
                var filteredBadges = badges.Where(badges => badges.Status != (Status)3).ToList();

                if (!filteredBadges.Any())
                {
                    return new ErrorDataResult<List<BadgeListDTO>>(new List<BadgeListDTO>(), "Bu şirkete ait uygun rozet bulunamadı.");
                }

                var badgeListDTOs = filteredBadges.Adapt<List<BadgeListDTO>>();
                return new SuccessDataResult<List<BadgeListDTO>>(badgeListDTOs, "Rozetler başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<BadgeListDTO>>(null, $"Hata oluştu: {ex.Message}");
            }
        }

    }
}
