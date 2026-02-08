using BaSalesManagementApp.Dtos.EmployeeDTOs;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Yeni bir çalışan ekler.
        /// </summary>
        /// <param name="employeeCreateDTO">Eklenen çalışanın bilgileri</param>
        /// <returns>Eklenen çalışanın verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<EmployeeDTO>> AddAsync(EmployeeCreateDTO employeeCreateDTO);

        /// <summary>
        /// Belirtilen ID'ye sahip çalışanı getirir.
        /// </summary>
        /// <param name="employeeId">Getirilecek çalışanın ID'si</param>
        /// <returns>Belirtilen ID'ye sahip çalışanın verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<EmployeeDTO>> GetByIdAsync(Guid employeeId);

        /// <summary>
        /// Belirtilen ID'ye sahip çalışanı siler.
        /// </summary>
        /// <param name="employeeId">Silinecek çalışanın ID'si</param>
        /// <returns>çalışanı silme işleminin sonuç durumunu döndürür</returns>
        Task<IResult> DeleteAsync(Guid employeeId);

        /// <summary>
        /// Tüm çalışanları getirir.
        /// </summary>
        /// <returns>Tüm çalışanların listesini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<List<EmployeeListDTO>>> GetAllAsync();
		Task<IDataResult<List<EmployeeListDTO>>> GetAllAsync(string sortEmployee, string searchQuery);

        /// <summary>
        /// Tüm çalışanları getirir ve sıralama seçeneğine göre sıralar.
        /// </summary>
        /// <param name="sortEmployee">Sıralama düzeni (örn. "name", "date")</param>
        /// <returns>Tüm çalışanların listesini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<List<EmployeeListDTO>>> GetAllAsync(string sortEmployee);

        /// <summary>
        /// Belirtilen ID'ye sahip çalışanı verilen bilgilerle günceller.
        /// </summary>
        /// <param name="employeeUpdateDTO">Güncellenecek çalışanın bilgileri</param>
        /// <returns>Güncellenen çalışanın verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<EmployeeDTO>> UpdateAsync(EmployeeUpdateDTO employeeUpdateDTO);
            /// <summary>
      /// Belirtilen role sahip çalışanları getirir.
      /// </summary>
      /// <param name="role">Filtrelenecek çalışan rolü (örneğin: "Manager", "Employee")</param>
      /// <returns>Filtrelenmiş çalışan listesini ve işlem sonucunu döndürür</returns>
      Task<IDataResult<List<EmployeeListDTO>>> GetEmployeesByRoleAsync(string role);

        /// <summary>
        /// Belirtilen şirkete ait çalışanların listesini getirir.
        /// </summary>
        /// <param name="companyId">Şirketin benzersiz kimlik numarası (GUID).</param>
        /// <returns>
        /// İşleme göre bir IDataResult nesnesi döner. 
        /// Başarılı bir işlemde, şirket çalışanlarının DTO listesiyle birlikte gelir.
        /// Hata durumunda ise hata mesajıyla birlikte boş bir liste dönecektir.
        /// </returns>
        Task<IDataResult<List<EmployeeListDTO>>> GetByCompanyIdAsync(Guid companyId);

        /// <summary>
        /// Belirtilen Identity ID'ye sahip çalışanı getirir.
        /// </summary>
        /// <param name="identityId">Getirilecek çalışanın IdentityID'si</param>
        /// <returns>Belirtilen Identity ID'ye sahip çalışanın verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<EmployeeDTO>> GetByIdentityIdAsync(string identityId);
        /// <summary>
        /// Kullanıcı ID'sine göre şirket ID'sini getirir.
        /// </summary>
        /// <param name="userId">Şirketin benzersiz kimlik ID sini ifade eder.</param>
        /// <returns>Şirkete ait ID verisini döner. Şirket verisi yoksa null ifade dönebilir</returns>
        Task<string> GetCompanyIdByUserIdAsync(string userId);
        Task<IDataResult<Guid?>> GetCompanyIdByUserIdAsync(Guid userId);
        Task<bool> IsManagerExistsAsync(Guid companyId);
        /// <summary>
        /// Belirtilen şirkette başka bir manager olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="companyId">Şirket ID'si</param>
        /// <param name="excludingEmployeeId">Güncellenen çalışan ID'si (bu çalışan hariç tutulacak)</param>
        /// <returns>Şirkette başka bir manager varsa true, yoksa false döndürür</returns>
        Task<bool> IsAnotherManagerExistsAsync(Guid companyId, Guid excludingEmployeeId);
       
    }
}
