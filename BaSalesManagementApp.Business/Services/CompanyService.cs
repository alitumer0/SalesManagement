using BaSalesManagementApp.Business.Interfaces;
using BaSalesManagementApp.Dtos.CityDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using BaSalesManagementApp.Entites.DbSets;
using Microsoft.EntityFrameworkCore;

namespace BaSalesManagementApp.Business.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IBranchService _branchService;
        private readonly IOrderRepository _orderRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ICountryRepository _countryRepository;
        public CompanyService(ICompanyRepository companyRepository, IBranchService branchService, IOrderRepository orderRepository, ICityRepository cityRepository, ICountryRepository countryRepository)
        {
            _companyRepository = companyRepository;
            _branchService = branchService;
            _orderRepository = orderRepository;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
        }

        //Yeni bir firma ekler ve işlem başarılıysa eklenen firmayı döndürür. Eğer bir hata oluşursa, uygun bir hata mesajıyla birlikte hata durumunu döndürür.
        public async Task<IDataResult<CompanyDTO>> AddAsync(CompanyCreateDTO companyCreateDTO)
        {
            try
            {
                var newBranch = companyCreateDTO.Adapt<Company>();

                await _companyRepository.AddAsync(newBranch);
                await _companyRepository.SaveChangeAsync();

                return new SuccessDataResult<CompanyDTO>(newBranch.Adapt<CompanyDTO>(), Messages.COMPANY_ADD_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<CompanyDTO>(Messages.COMPANY_ADD_ERROR);
            }
        }

        //Belirtilen bir firmayı siler ve işlem başarılıysa başarılı bir mesaj döndürür.Herhangi bir hata oluşursa uygun bir hata mesajıyla birlikte hata durumunu döndürür.

        public async Task<IResult> DeleteAsync(Guid companyId)
        {
            try
            {
                var deletingCompany = await _companyRepository.GetByIdAsync(companyId);
                var branches = await _branchService.GetBranchesByCompanyIdAsync(companyId);

                foreach (var branch in branches)
                {
                    await _branchService.DeleteAsync(branch.Id);
                }

                // Şirkete ait bir ürünün herhangi bir sipariş içerisinde olup olmadığını kontrol eder
                var companyInOrders = await _orderRepository.AnyAsync(o => o.OrderDetails.Any(od => od.Product.CompanyId == companyId));

                if (companyInOrders)
                {
                    var result = await _companyRepository.SetCompanyToPassiveAsync(companyId);
                    if (!result.IsSuccess)
                    {
                        return result;

                    }
                    await _companyRepository.SaveChangeAsync();
                    return new SuccessResult((Messages.COMPANY_PASSIVED_SUCCESS));
                }

                await _companyRepository.DeleteAsync(deletingCompany);
                await _companyRepository.SaveChangeAsync();

                return new SuccessResult(Messages.COMPANY_DELETE_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.COMPANY_DELETE_ERROR);
            }
        }

        //Tüm firmaları getirir ve işlem başarılıysa firma listesini döndürür.Eğer hiç şube bulunamazsa uygun bir mesajla birlikte hata durumunu döndürür.

        /// <summary>
        /// Sistemdeki tüm şirketleri getirir. Şirket bilgileri DTO formatına dönüştürülerek döndürülür.
        /// City → Country gibi ilişkili alanlara erişim varsayılan Entity yapısı üzerinden yapılır.
        /// </summary>
        /// <returns>Tüm şirketleri içeren CompanyListDTO listesi.</returns>

        public async Task<IDataResult<List<CompanyListDTO>>> GetAllAsync()
        {
            try
            {
                // Tüm şirketleri getir (City > Country dahil olmalı — Include kullanılmayacaksa veri erişimi garanti olmalı)

                var companies = await _companyRepository.GetAllAsync(x => x.Name, false);//Alfabetik olarak sıralıyoruz


                if (!companies.Any())
                {
                    return new ErrorDataResult<List<CompanyListDTO>>(new List<CompanyListDTO>(), Messages.COMPANY_LISTED_NOTFOUND);
                }

                // DTO'ya  Çeviriyoruz
                var companyDTOs = companies.Adapt<List<CompanyListDTO>>() ?? new List<CompanyListDTO>();

                // Culture-aware CountryName ve CountryId ata
                var currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                foreach (var company in companyDTOs)
                {
                    var companyEntity = companies.FirstOrDefault(x => x.Id == company.Id);
                    if (companyEntity != null)
                    {
                        company.CountryName = currentCulture == "tr"
                            ? companyEntity.City?.Country?.NameTr ?? "Bilinmiyor"
                            : companyEntity.City?.Country?.NameEn ?? "Bilinmiyor";

                        company.CountryId = companyEntity.City?.Country?.Id ?? Guid.Empty;
                    }
                }

                return new SuccessDataResult<List<CompanyListDTO>>(companyDTOs, Messages.COMPANY_LISTED_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<CompanyListDTO>>(new List<CompanyListDTO>(), Messages.COMPANY_LISTED_ERROR);
            }
        }

        /// <summary>
        /// Arama kriterine göre filtrelenmiş şirket listesini getirir.
        /// Arama, şirket adında geçen değerlere göre yapılır.
        /// Şirket bilgileri DTO formatına dönüştürülerek döndürülür.
        /// Ayrıca kültüre göre ülke adı (`CountryName`) bilgisi eklenir.
        /// </summary>
        /// <param name="searchQuery">Şirket adına göre yapılacak arama sorgusu.</param>
        /// <returns>Filtrelenmiş CompanyListDTO listesi.</returns>

        public async Task<IDataResult<List<CompanyListDTO>>> GetAllAsync(string searchQuery)
        {
            try
            {
                // 1. Tüm ilişkili verileri al (City > Country dahil olacak şekilde)
                var companies = await _companyRepository.GetAllAsync();

                // 2. DTO'ya adapt et
                var companyDTOs = companies.Adapt<List<CompanyListDTO>>() ?? new List<CompanyListDTO>();

                // 3. Ülke adını kültüre göre ekle
                var currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                foreach (var company in companyDTOs)
                {
                    var companyEntity = companies.FirstOrDefault(x => x.Id == company.Id);
                    if (companyEntity != null)
                    {
                        company.CountryName = currentCulture == "tr"
                            ? companyEntity.City?.Country?.NameTr ?? "Bilinmiyor"
                            : companyEntity.City?.Country?.NameEn ?? "Bilinmiyor";

                        company.CountryId = companyEntity.City?.Country?.Id ?? Guid.Empty;
                    }

                }

                // 4. Arama filtresi varsa uygula (DTO üzerinden)
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    companyDTOs = companyDTOs
                        .Where(c => !string.IsNullOrEmpty(c.Name) && c.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (!companyDTOs.Any())
                {
                    return new ErrorDataResult<List<CompanyListDTO>>(companyDTOs, Messages.COMPANY_LISTED_NOTFOUND);
                }

                return new SuccessDataResult<List<CompanyListDTO>>(companyDTOs, Messages.COMPANY_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CompanyListDTO>>(new List<CompanyListDTO>(), Messages.COMPANY_LISTED_ERROR + ": " + ex.Message);
            }
        }

        /// <summary>
        /// Şirketleri isme veya oluşturulma tarihine göre sıralar, arama sorgusuna göre filtreler
        /// ve isteğe bağlı olarak ülkeye göre filtreleme uygular.
        /// Dönen şirketler DTO formatına çevrilir ve ülke adı (`CountryName`) bilgisi kültüre göre eklenir.
        /// </summary>
        /// <param name="sortCompany">Sıralama türü (örn. "name_asc", "name_desc", "date_asc", "date_desc").</param>
        /// <param name="searchQuery">Şirket adına göre yapılacak arama sorgusu.</param>
        /// <param name="CountryId">Ülkeye göre filtreleme yapmak için kullanılacak ülke ID’si (isteğe bağlı).</param>
        /// <returns>Filtrelenmiş ve sıralanmış CompanyListDTO listesi.</returns>

        public async Task<IDataResult<List<CompanyListDTO>>> GetAllAsync(string sortCompany, string searchQuery, Guid? CountryId)
        {
            try
            {
                var Companies = await _companyRepository.GetAllAsync();

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    Companies = Companies.Where(c => c.Name != null && c.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
                }

                if (CountryId.HasValue && CountryId.Value != Guid.Empty)
                {
                    Companies = Companies
                        .Where(c => c.City != null && c.City.CountryId == CountryId);
                }

                Companies = sortCompany?.ToLower() switch
                {
                    "name_asc" => Companies.OrderBy(a => a.Name),
                    "name_desc" => Companies.OrderByDescending(a => a.Name),
                    "date_asc" => Companies.OrderBy(a => a.CreatedDate),
                    "date_desc" => Companies.OrderByDescending(a => a.CreatedDate),
                    _ => Companies.OrderBy(a => a.Name)
                };

                if (!Companies.Any())
                {
                    return new ErrorDataResult<List<CompanyListDTO>>(new List<CompanyListDTO>(), Messages.COMPANY_LISTED_NOTFOUND);
                }

                var companyListDTOs = Companies.Adapt<List<CompanyListDTO>>();

                // EKLENEN KISIM — ülke adı ve id set ediliyor
                var currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                foreach (var dto in companyListDTOs)
                {
                    var entity = Companies.FirstOrDefault(x => x.Id == dto.Id);
                    if (entity != null)
                    {
                        dto.CountryName = currentCulture == "tr"
                            ? entity.City?.Country?.NameTr ?? "Bilinmiyor"
                            : entity.City?.Country?.NameEn ?? "Bilinmiyor";

                        dto.CountryId = entity.City?.Country?.Id ?? Guid.Empty;
                    }
                }

                return new SuccessDataResult<List<CompanyListDTO>>(companyListDTOs, Messages.COMPANY_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CompanyListDTO>>(new List<CompanyListDTO>(), Messages.COMPANY_LISTED_ERROR + ex.Message);
            }
        }



        //Belirli bir firma kimliğine göre firmayı getirir.Firma bulunamazsa uygun bir hata mesajıyla birlikte hata durumunu döndürür.

        public async Task<IDataResult<CompanyDTO>> GetByIdAsync(Guid companyId)
        {
            CompanyDTO companyDTO = new CompanyDTO();

            try
            {
                var company = await _companyRepository.GetByIdAsync(companyId);
                if (company == null)
                {
                    return new ErrorDataResult<CompanyDTO>(Messages.COMPANY_GETBYID_ERROR);
                }

                var city = await _cityRepository.GetByIdAsync(company.CityID.GetValueOrDefault());
                var country = await _countryRepository.GetByIdAsync(city?.CountryId ?? Guid.Empty);

                companyDTO = company.Adapt<CompanyDTO>();

                if (city != null)
                {
                    companyDTO.CityName = city.Name;
                }
                //if (country != null)
                //{
                //    companyDTO.CountryName = country.Name;

                //}
                if (country != null)
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                    companyDTO.CountryName = currentCulture == "tr" ? country.NameTr : country.NameEn;
                }


                companyDTO.LogoUrl = company.CompanyPhoto != null
                   ? $"data:image/png;base64,{Convert.ToBase64String(company.CompanyPhoto)}"
                   : "/img/CompanyDefault.png";


                return new SuccessDataResult<CompanyDTO>(companyDTO, Messages.COMPANY_GETBYID_SUCCESS);
            }
            catch
            {
                return new ErrorDataResult<CompanyDTO>(Messages.COMPANY_GETBYID_ERROR);
            }

        }

        public async Task<bool> IsCompanyInOrderAsync(Guid companyId)
        {
            return await _orderRepository.AnyAsync(o => o.OrderDetails.Any(od => od.Product.CompanyId == companyId));
        }

        //Belirli bir firma kimliğine göre firma bilgilerini günceller.Güncelleme başarılıysa güncellenen firma bilgilerini döndürür.Herhangi bir hata oluşursa uygun bir hata mesajıyla birlikte hata durumunu döndürür.

        public async Task<IDataResult<CompanyDTO>> UpdateAsync(CompanyUpdateDTO companyUpdateDTO)
        {
            try
            {
                // Güncellenecek şirketi veritabanından getiriyoruz
                var updatingCompany = await _companyRepository.GetByIdAsync(companyUpdateDTO.Id);

                if (updatingCompany == null)
                {
                    return new ErrorDataResult<CompanyDTO>(Messages.COMPANY_GETBYID_ERROR);
                }

                // Temel bilgileri eşleştir
                updatingCompany.CountryCode = companyUpdateDTO.CountryCode;
                updatingCompany.CityID = companyUpdateDTO.CityId;
                updatingCompany.Phone = companyUpdateDTO.Phone;

                // Fotoğraf kaldırma işlemi
                if (companyUpdateDTO.RemovePhoto)
                {
                    updatingCompany.CompanyPhoto = null; // Fotoğrafı null olarak işaretle
                }
                // Yeni bir fotoğraf yüklendiyse, onu kaydediyoruz
                else if (companyUpdateDTO.CompanyPhoto != null && companyUpdateDTO.CompanyPhoto.Length > 0)
                {
                    updatingCompany.CompanyPhoto = companyUpdateDTO.CompanyPhoto;
                }

                // Güncellemeyi veritabanına kaydediyoruz
                await _companyRepository.UpdateAsync(updatingCompany);
                await _companyRepository.SaveChangeAsync();

                // Güncellenmiş şirket DTO'sunu döndür
                return new SuccessDataResult<CompanyDTO>(updatingCompany.Adapt<CompanyDTO>(), Messages.COMPANY_UPDATE_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<CompanyDTO>(Messages.COMPANY_UPDATE_ERROR);
            }
        }


        /// <summary>
        /// Belirtilen şirketin durumunu günceller.
        /// </summary>
        /// <param name="companyId">Güncellenecek şirketin kimliği.</param>
        /// <param name="newStatus">Yeni durum.</param>
        /// <returns>Güncellenmiş şirket bilgilerini içeren başarılı sonuç veya hata mesajı ile hata sonucu.</returns>
        public async Task<IDataResult<CompanyDTO>> ChangeStatusAsync(Guid companyId, Status newStatus)
        {
            try
            {
                // Şirketi ID'ye göre alıyoruz
                var updatingCompany = await _companyRepository.GetByIdAsync(companyId);

                if (updatingCompany == null)
                {
                    return new ErrorDataResult<CompanyDTO>(Messages.COMPANY_GETBYID_ERROR);
                }

                // Şirketin statüsünü yeni statü ile güncelliyoruz
                updatingCompany.Status = newStatus;

                // Repository'de güncelleme işlemi
                await _companyRepository.UpdateAsync(updatingCompany);
                await _companyRepository.SaveChangeAsync();

                // Güncellenmiş şirketi DTO'ya adapte ederek geri döndürüyoruz
                var updatedCompanyDTO = updatingCompany.Adapt<CompanyDTO>();

                return new SuccessDataResult<CompanyDTO>(updatedCompanyDTO, Messages.COMPANY_UPDATE_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<CompanyDTO>(Messages.COMPANY_UPDATE_ERROR);
            }
        }
        public async Task<IDataResult<CityDTO>> GetCityAndCountryByCompanyIdAsync(Guid companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);


            if (company == null || company.CityID == null)
            {
                return new ErrorDataResult<CityDTO>(Messages.CITY_NOT_FOUND);
            }

            var city = await _cityRepository.GetByIdAsync(company.CityID.Value);

            if (city == null)
            {
                return new ErrorDataResult<CityDTO>(Messages.CITY_NOT_FOUND);
            }

            var country = await _countryRepository.GetByIdAsync(city.CountryId);
            var cityDto = new CityDTO
            {
                Id = city.Id,
                Name = city.Name,
                CountryId = country.Id,


            };

            return new SuccessDataResult<CityDTO>(cityDto, Messages.CITY_GET_SUCCESS);
        }

        /// <summary>
        /// Ülke Id bilgisi alınarak, o ülkedeki şirketleri getirir.
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns> Asenkron işlemi temsil eden bir görev. Görev sonucunda şirket listesini döndürür. </returns>

        public async Task<IDataResult<List<CompanyListDTO>>> GetCompaniesListByCountryIdAsync(Guid countryId)
        {
            var citiesResult = await _cityRepository.GetAllAsync();
            var cities = citiesResult.Where(c => c.CountryId == countryId).ToList();

            var companiesResult = await _companyRepository.GetAllAsync();
            var companies = companiesResult.Where(cmp => cities.Any(c => c.Id == cmp.CityID)).ToList();

            var companyListDTO = companies.Adapt<List<CompanyListDTO>>();

            return new SuccessDataResult<List<CompanyListDTO>>(companyListDTO, Messages.COMPANY_LISTED_SUCCESS);
        }


    }
}
