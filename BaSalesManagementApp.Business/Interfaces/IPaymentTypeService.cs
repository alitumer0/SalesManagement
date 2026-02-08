using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.PaymentTypeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface IPaymentTypeService
    {
        /// <summary>
        /// Tüm ödeme tiplerini getirir ve arama ile sıralama işlemlerini gerçekleştirir.
        /// </summary>
        /// <param name="sortOrder">Sıralama düzenini belirten bir dize (örneğin, "alphabetical", "date").</param>
        /// <param name="searchQuery">Arama sorgusu.</param>
        /// <returns>Filtrelenmiş ve sıralanmış ödeme tipi listesi.</returns>
        /// 
        Task<IDataResult<List<PaymentTypeListDTO>>> GetAllAsync(string sortOrder);
        Task<IDataResult<List<PaymentTypeListDTO>>> GetAllAsync(string sortOrder, string searchQuery);

        /// <summary>
        /// Yeni bir kategori ekler.
        /// </summary>
        /// <param name="paymentTypeCreateDTO">Eklenmek istenen ödeme tipine ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda eklenen ödeme tipi verilerini içerir.</returns>
        Task<IDataResult<PaymentTypeDTO>> AddAsync(PaymentTypeCreateDTO paymentTypeCreateDTO);

        /// <summary>
        /// Benzersiz kimliğiyle bir ödeme tipi alır.
        /// </summary>
        /// <param name="paymentTypeId">Alınmak istenen ödeme tipinin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda ödeme tipi verilerini içerir, bulunamazsa null döner.</returns>
        Task<IDataResult<PaymentTypeDTO>> GetByIdAsync(Guid paymentTypeId);

        /// <summary>
        /// Benzersiz kimliğiyle bir ödeme tipi siler.
        /// </summary>
        /// <param name="paymentTypeId">Silinmek istenen kategorinin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda silme işleminin başarılı olup olmadığını belirtir.</returns>
        Task<IResult> DeleteAsync(Guid paymentTypeId);

        /// <summary>
        /// Bir ödeme tipini günceller.
        /// </summary>
        /// <param name="paymentTypeUpdateDTO">Güncellenmiş ödeme tipi ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda güncellenmiş ödeme tipi verilerini içerir.</returns>
        Task<IDataResult<PaymentTypeDTO>> UpdateAsync(PaymentTypeUpdateDTO paymentTypeUpdateDTO);

        /// <summary>
        /// Tüm ödeme tiplerini alır.
        /// </summary>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda tüm ödeme tipleri listesini içerir.</returns>
        Task<IDataResult<List<PaymentTypeListDTO>>> GetAllAsync();

        Task<IDataResult<List<PaymentTypeListDTO>>> GetAllWithStatusDefaultActivedAsync(Status status = Status.Actived);

        /// <summary>
        /// Bir şirkete ait ödeme tiplerini getirir.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<IDataResult<List<PaymentTypeListDTO>>> GetPaymentTypesByCompanyIdAsync(Guid companyId);
    }
}
