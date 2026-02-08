using BaSalesManagementApp.Dtos.CustomerDTOs;
using BaSalesManagementApp.Dtos.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface ICustomerService
    {
        /// <summary>
        /// Tüm Müşterilerigetirir.
        /// </summary>
        /// <returns>Tüm Müşterilerin listesini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<List<CustomerListDTO>>> GetAllAsync(string sortOrder);

		Task<IDataResult<List<CustomerListDTO>>> GetAllAsync(string sortOrder,string searchQuery);
		/// <summary>
		/// Yeni bir Müşteri ekler.
		/// </summary>
		/// <param name="customerCreateDTO">Eklenmek istenen müşteriyle ilgili bilgileri içeren veri transfer nesnesi.</param>
		/// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda eklenen müşterilerin verilerini içerir.</returns>
		Task<IDataResult<CustomerDTO>> AddAsync(CustomerCreateDTO customerCreateDTO);

        /// <summary>
        /// Benzersiz kimliğiyle bir müşteri alır.
        /// </summary>
        /// <param name="customerId">Alınmak istenen müşterinin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda müşteri verilerini içerir, bulunamazsa null döner.</returns>
        Task<IDataResult<CustomerDTO>> GetByIdAsync(Guid customerId);

        /// <summary>
        /// Benzersiz kimliğiyle bir müşteriyi siler.
        /// </summary>
        /// <param name="customerId">Silinmek istenen müşterinin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda silme işleminin başarılı olup olmadığını belirtir.</returns>
        Task<IResult> DeleteAsync(Guid customerId);

        /// <summary>
        /// Bir müşteriyi günceller.
        /// </summary>
        /// <param name="customerUpdateDTO">Güncellenmiş müşteriyle ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda güncellenmiş müşteri verilerini içerir.</returns>
        Task<IDataResult<CustomerDTO>> UpdateAsync(CustomerUpdateDTO customerUpdateDTO);

        /// <summary>
        /// Tüm müşterileri alır.
        /// </summary>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda tüm müşterilerin listesini içerir.</returns>
        Task<IDataResult<List<CustomerListDTO>>> GetAllAsync();


        /// <summary>
        /// Müşteri id bilgisine göre geçmiş siparişlerini listeler
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="sortOrder"></param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        Task<IDataResult<List<OrderListDTO>>> GetCustomerOrdersHistoryAsync(Guid customerId, string sortOrder, string searchQuery);


        /// <summary>
        /// Company Id bilgisine göre tüm müşterileri listeler
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<IDataResult<List<CustomerListDTO>>> GetCustomersByCompanyId(Guid companyId, string sortOrder);
    }
}
