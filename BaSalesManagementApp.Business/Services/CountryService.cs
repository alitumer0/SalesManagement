using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Business.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        //Yeni bir ülke ekler ve işlem başarılıysa eklenen ülkeyi döndürür. Eğer bir hata oluşursa, uygun bir hata mesajıyla birlikte hata durumunu döndürür.
        public async Task<IDataResult<CountryDTO>> AddAsync(CountryCreateDTO countryCreateDTO)
        {
            try
            {


                var data = await _countryRepository.GetAsync(c => c.NameTr == countryCreateDTO.NameTr || c.NameEn == countryCreateDTO.NameEn);
                if (data != null) return new ErrorDataResult<CountryDTO>(Messages.COUNTRY_CREATE_ERROR);

                var newCountry = countryCreateDTO.Adapt<Country>();

                await _countryRepository.AddAsync(newCountry);
                await _countryRepository.SaveChangeAsync();

                return new SuccessDataResult<CountryDTO>(newCountry.Adapt<CountryDTO>(), Messages.COUNTRY_CREATE_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<CountryDTO>(Messages.COUNTRY_CREATE_ERROR);
            }
        }

        public bool CountryExist(string countryName)
        {
            return _countryRepository.AnyAsync(c => c.NameTr == countryName || c.NameEn == countryName).Result;

        }

        //Belirtilen bir ülkeyi siler ve işlem başarılıysa başarılı bir mesaj döndürür.Herhangi bir hata oluşursa uygun bir hata mesajıyla birlikte hata durumunu döndürür.

        public async Task<IResult> DeleteAsync(Guid countryId)
        {
            try
            {
                var deletingCountry = await _countryRepository.GetByIdAsync(countryId);

                await _countryRepository.DeleteAsync(deletingCountry);
                await _countryRepository.SaveChangeAsync();

                return new SuccessResult(Messages.COUNTRY_DELETE_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.COUNTRY_DELETE_ERROR);
            }
        }

        //Tüm ülkeleri getirir ve işlem başarılıysa ülke listesini döndürür.Eğer hiç ülke bulunamazsa uygun bir mesajla birlikte hata durumunu döndürür.

        public async Task<IDataResult<List<CountryListDTO>>> GetAllAsync()
        {
            try
            {
                IEnumerable<Country> countries;
                countries = await _countryRepository.GetAllAsync();

                if (countries.Count() == 0)
                {
                    return new ErrorDataResult<List<CountryListDTO>>(new List<CountryListDTO>(), Messages.COUNTRY_LISTED_NOTFOUND);
                }

                return new SuccessDataResult<List<CountryListDTO>>(countries.Adapt<List<CountryListDTO>>(), Messages.COUNTRY_LISTED_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<CountryListDTO>>(new List<CountryListDTO>(), Messages.COUNTRY_LISTED_ERROR);
            }
        }

        public async Task<IDataResult<List<CountryListDTO>>> GetAllAsync(string searchQuery)
        {
            try
            {
                // Fetch all countries from the repository
                var countries = await _countryRepository.GetAllAsync();

                // Convert to DTO list
                var countryListDTOs = countries.Adapt<List<CountryListDTO>>() ?? new List<CountryListDTO>();

                // Apply filtering based on the search query
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    countryListDTOs = countryListDTOs
                        .Where(c =>
                        (!string.IsNullOrEmpty(c.NameTr) && c.NameTr.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) || 
                        (!string.IsNullOrEmpty(c.NameEn) && c.NameEn.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(c.CreatedDate.ToString("dd.MM.yyyy")) && c.CreatedDate.ToString("dd.MM.yyyy")
                                               .Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                }

                // Check if any countries match the search query
                if (countryListDTOs.Count == 0)
                {
                    return new ErrorDataResult<List<CountryListDTO>>(countryListDTOs, Messages.COUNTRY_LISTED_NOTFOUND);
                }

                return new SuccessDataResult<List<CountryListDTO>>(countryListDTOs, Messages.COUNTRY_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CountryListDTO>>(new List<CountryListDTO>(), Messages.COUNTRY_LISTED_ERROR + ": " + ex.Message);
            }
        }

        //Belirli bir ülke kimliğine göre firmayı getirir.ülke bulunamazsa uygun bir hata mesajıyla birlikte hata durumunu döndürür.

        public async Task<IDataResult<CountryDTO>> GetByIdAsync(Guid countryId)
        {
            try
            {
                var country = await _countryRepository.GetByIdAsync(countryId);

                if (country == null)
                {
                    return new ErrorDataResult<CountryDTO>(Messages.COUNTRY_GETBYID_ERROR);
                }


                var countryNameTr = country.NameTr;
                var countryNameEn = country.NameEn;


                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Countries.json");
                var json = System.IO.File.ReadAllText(filePath);

                var jsonObject = JObject.Parse(json);
                var countryCodes = jsonObject["CountryCodes"] as JArray;

                var countryCode = countryCodes?.FirstOrDefault(c =>
                    c["name"]?.ToString().Equals(countryNameEn, StringComparison.OrdinalIgnoreCase) == true ||
                    c["turkish_name"]?.ToString().Equals(countryNameTr, StringComparison.OrdinalIgnoreCase) == true);

                var dialCode = countryCode["dialcode"]?.ToString();

                var countryDTO = new CountryDTO
                {
                    Id = countryId,
                    NameTr = countryNameTr,
                    NameEn = countryNameEn,
                    CountryCode = dialCode
                };

                return new SuccessDataResult<CountryDTO>(countryDTO.Adapt<CountryDTO>(), Messages.COUNTRY_GETBYID_SUCCESS);
            }
            catch
            {
                return new ErrorDataResult<CountryDTO>(Messages.COUNTRY_GETBYID_ERROR);
            }
        }

        //Belirli bir ülke kimliğine göre ülke bilgilerini günceller.Güncelleme başarılıysa güncellenen ülke bilgilerini döndürür.Herhangi bir hata oluşursa uygun bir hata mesajıyla birlikte hata durumunu döndürür.

        public async Task<IDataResult<CountryDTO>> UpdateAsync(CountryUpdateDTO countryUpdateDTO)
        {
            try
            {
                var updatingCountry = await _countryRepository.GetByIdAsync(countryUpdateDTO.Id);

                if (updatingCountry == null)
                {
                    return new ErrorDataResult<CountryDTO>(Messages.COUNTRY_GETBYID_ERROR);
                }

                await _countryRepository.UpdateAsync(countryUpdateDTO.Adapt(updatingCountry));
                await _countryRepository.SaveChangeAsync();

                return new SuccessDataResult<CountryDTO>(countryUpdateDTO.Adapt<CountryDTO>(), Messages.COUNTRY_UPDATE_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<CountryDTO>(Messages.COUNTRY_UPDATE_ERROR);
            }
        }
    }
}
