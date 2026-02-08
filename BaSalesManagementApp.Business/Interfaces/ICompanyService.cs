using BaSalesManagementApp.Dtos.CityDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface ICompanyService
    {
        /// <summary>
        /// Yeni bir firma ekler.
        /// </summary>
        /// <param name="companyCreateDTO">Eklenmek istenen firmayla ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda eklenen şube verilerini içerir.</returns>
        Task<IDataResult<CompanyDTO>> AddAsync(CompanyCreateDTO companyCreateDTO);

        /// <summary>
        /// Benzersiz kimliğiyle bir firmayı alır.
        /// </summary>
        /// <param name="companyId">Alınmak istenen firmanın benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda firma verilerini içerir, bulunamazsa null döner.</returns>
        /// 
        Task<IDataResult<CompanyDTO>> GetByIdAsync(Guid companyId);

        /// <summary>
        /// Benzersiz kimliğiyle bir firmayı siler.
        /// </summary>
        /// <param name="companyId">Silinmek istenen firmanın benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda silme işleminin başarılı olup olmadığını belirtir.</returns>
        Task<IResult> DeleteAsync(Guid companyId);

        /// <summary>
        /// Tüm firmaları alır.
        /// </summary>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda tüm firmaların listesini içerir.</returns>
        Task<IDataResult<List<CompanyListDTO>>> GetAllAsync();
        /// <summary>
        /// Tüm adminleri getirir ve sıralama seçeneğine göre sıralar.İstenirse filtreleme için anahtar kelime ile uyumlu şirketler getirilebilir.
        /// </summary>
        /// <param name="sortCompany">Sıralama Düzeni name_asc/date_asc vb...</param>
        /// <param name="searchQuery">Arama anahtar kelimesi</param>
        /// <returns></returns>
        Task<IDataResult<List<CompanyListDTO>>> GetAllAsync(string sortCompany, string searchQuery, Guid? CountryId);
        Task<IDataResult<List<CompanyListDTO>>> GetAllAsync(string searchQuery);

        /// <summary>
        /// Bir firmayı günceller.
        /// </summary>
        /// <param name="companyUpdateDTO">Güncellenmiş firmayla ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda güncellenmiş firma verilerini içerir.</returns>
        Task<IDataResult<CompanyDTO>> UpdateAsync(CompanyUpdateDTO companyUpdateDTO);


        /// <summary>
        /// Verilen şirketin aktif bir siparişi olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="companyId">Kontrol edilecek şirketin kimliği.</param>
        /// <returns>Şirketin aktif bir siparişi varsa <c>true</c>; aksi halde <c>false</c> döner.</returns>
        Task<bool> IsCompanyInOrderAsync(Guid companyId);
        /// <summary>
        /// Belirtilen şirketin durumunu günceller.
        /// </summary>
        /// <param name="companyId">Güncellenecek şirketin kimliği.</param>
        /// <param name="newStatus">Yeni durum.</param>
        /// <returns>Güncellenmiş şirket bilgilerini içeren başarılı sonuç veya hata mesajı ile hata sonucu.</returns>
       
        Task<IDataResult<CompanyDTO>> ChangeStatusAsync(Guid companyId, Status newStatus);
        Task<IDataResult<CityDTO>> GetCityAndCountryByCompanyIdAsync(Guid companyId);

        /// <summary>
        /// Ülke Id bilgisi alınarak, o ülkedeki şirketleri getirir.
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns> Asenkron işlemi temsil eden bir görev. Görev sonucunda şirket listesini döndürür. </returns>
        Task<IDataResult<List<CompanyListDTO>>> GetCompaniesListByCountryIdAsync(Guid countryId);
    }

}

