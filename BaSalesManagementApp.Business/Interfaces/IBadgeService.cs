using BaSalesManagementApp.Dtos.BadgeDTOs;
using BaSalesManagementApp.Dtos.BranchDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface IBadgeService
    {
        /// <summary>
        /// Yeni bir rozet ekler.
        /// </summary>
        /// <param name="badgeCreateDTO">Eklenmek istenen rozetle ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda eklenen rozet verilerini içerir.</returns>
        Task<IDataResult<BadgeDTO>> AddAsync(BadgeCreateDTO badgeCreateDTO);

        /// <summary>
        /// Benzersiz kimliğiyle bir rozeti alır.
        /// </summary>
        /// <param name="badgeId">Alınmak istenen rozetin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda rozet verilerini içerir, bulunamazsa null döner.</returns>
        Task<IDataResult<BadgeDTO>> GetByIdAsync(Guid badgeId);

        /// <summary>
        /// Benzersiz kimliğiyle bir rozeti siler.
        /// </summary>
        /// <param name="badgeId">Silinmek istenen rozetin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda silme işleminin başarılı olup olmadığını belirtir.</returns>
        Task<IResult> DeleteAsync(Guid badgeId);

        /// <summary>
        /// Tüm rozetleri alır.
        /// </summary>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda tüm rozetlerin listesini içerir.</returns>
        Task<IDataResult<List<BadgeListDTO>>> GetAllAsync();
        Task<IDataResult<List<BadgeListDTO>>> GetAllAsync(string searchQuery);

        /// <summary>
		/// Bir rozeti günceller.
		/// </summary>
		/// <param name="badgeUpdateDTO">Güncellenmiş rozetle ilgili bilgileri içeren veri transfer nesnesi.</param>
		/// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda güncellenmiş rozet verilerini içerir.</returns>
		Task<IDataResult<BadgeDTO>> UpdateAsync(BadgeUpdateDTO badgeUpdateDTO);

        /// <summary>
        /// Company kimliğine ait rozetleri listeler.
        /// </summary>
        /// <param name="companyId">Rozetlerine ulaşılmak istenen companynin benzersiz kimliği</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda ilgili companye ait rozetleri listeler</returns>
        Task<List<Badge>> GetBadgesByCompanyIdAsync(Guid companyId);



        /// <summary>
        /// Company kimliğine ait rozetleri listeler.
        /// </summary>
        /// <param name="companyId">Rozetlerine ulaşılmak istenen companynin benzersiz kimliği</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda ilgili companye ait rozetleri listeler</returns>
        Task<IDataResult<List<BadgeListDTO>>> GetBadgesByCompanyIdAsynca(Guid companyId);

   
      
    }
}
