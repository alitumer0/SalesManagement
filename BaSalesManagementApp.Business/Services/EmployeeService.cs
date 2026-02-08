using BaSalesManagementApp.Core.Utilities.Results;
using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.EmployeeDTOs;
using BaSalesManagementApp.Dtos.MailDTOs;
using BaSalesManagementApp.Entites.DbSets;
using Microsoft.AspNetCore.Identity;

namespace BaSalesManagementApp.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAccountService _accountService;
        private readonly IMailService _mailService;
        private readonly IStringLocalizer<EmployeeService> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeService(IEmployeeRepository employeeRepository, IAccountService accountService, IMailService mailService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IStringLocalizer<EmployeeService> localizer)
        {
            _employeeRepository = employeeRepository;
            _accountService = accountService;
            _mailService = mailService;
            _userManager = userManager;
            _roleManager = roleManager;
            _localizer = localizer;
        }

        public async Task<IDataResult<EmployeeDTO>> AddAsync(EmployeeCreateDTO employeeCreateDTO)
        {
            if (await _accountService.AnyAsync(x => x.Email == employeeCreateDTO.Email))
            {
                return new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_EMAIL_IN_USE);
            }

            IdentityUser identityUser = new()
            {
                Email = employeeCreateDTO.Email,
                EmailConfirmed = false,
                UserName = employeeCreateDTO.Email
            };

            DataResult<EmployeeDTO> result = new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_ADD_ERROR);
            var strategy = await _employeeRepository.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transactionScope = await _employeeRepository.BeginTransactionAsync().ConfigureAwait(false);
                try
                {
                    // Rolü adı ile bul
                    var role = await _roleManager.FindByNameAsync(employeeCreateDTO.Title);
                    if (role == null)
                    {
                        result = new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_ADD_ERROR);
                        await transactionScope.RollbackAsync();
                        return;
                    }

                    // Rol enum'ını al (Eğer enum kullanmaya devam edecekseniz)
                    if (!Enum.TryParse(role.Name, out Roles roleEnum))
                    {
                        result = new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_ADD_ERROR);
                        await transactionScope.RollbackAsync();
                        return;
                    }

                    // Kullanıcıyı oluştur ve şifreyi al
                    var createUserResult = await _accountService.CreateUserAsync(identityUser, roleEnum);
                    var identityResult = createUserResult.IdentityResult;
                    var defaultPassword = createUserResult.Password;

                    if (!identityResult.Succeeded)
                    {
                        result = new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_ADD_ERROR);
                        await transactionScope.RollbackAsync();
                        return;
                    }

                    // Employee verisini oluştur ve kaydet
                    var employee = employeeCreateDTO.Adapt<Employee>();
                    employee.IdentityId = identityUser.Id;
                    employee.Title = role.Name; // Rol ismini Title alanına atayın
                    await _employeeRepository.AddAsync(employee);
                    await _employeeRepository.SaveChangeAsync();

                    // Başarı mesajını belirle
                    if (roleEnum == Roles.Manager)
                    {
                        result = new SuccessDataResult<EmployeeDTO>(employee.Adapt<EmployeeDTO>(), Messages.MANAGER_CREATED_SUCCESS);
                    }
                    else
                    {
                        result = new SuccessDataResult<EmployeeDTO>(employee.Adapt<EmployeeDTO>(),Messages.EMPLOYEE_ADD_SUCCESS);
                    }

                    // E-Posta Gönderim işlemi
                    var mailCreateDto = new MailCreateDto
                    {
                        Title = "Merhaba,",
                        Subject = "Şirket Hesabın Hakkında Bilgilendirme",
                        Body = $"Merhaba, {employee.FirstName}, <br><br>Kullanıcı Adınız : {identityUser.UserName} <br>Şifreniz : {defaultPassword}<br><br>Lütfen sisteme giriş yaparak şifrenizi güncelleyiniz.",
                        ReceiverMailAddress = employee.Email,
                    };
                    await _mailService.SendMailAsync(mailCreateDto);

                    await transactionScope.CommitAsync();
                }
                catch (Exception ex)
                {
                    result = new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_ADD_ERROR + ex.Message);
                    await transactionScope.RollbackAsync();
                }
                finally
                {
                    await transactionScope.DisposeAsync();
                }
            });

            return result;
        }

        public async Task<IDataResult<EmployeeDTO>> GetByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_GETBYID_ERROR);
            }

            SuccessDataResult<EmployeeDTO> result;

            if (employee.Title == Roles.Manager.ToString())
            {
                result = new SuccessDataResult<EmployeeDTO>(employee.Adapt<EmployeeDTO>(), Messages.MANAGER_FOUND_SUCCESS);
            }
            else
            {
                result = new SuccessDataResult<EmployeeDTO>(employee.Adapt<EmployeeDTO>(), Messages.EMPLOYEE_GETBYID_SUCCESS);
            }

            return result;
        }
        public async Task<IResult> DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return new ErrorResult(Messages.EMPLOYEE_DELETE_ERROR);
            }

            DataResult<EmployeeDTO> result = new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_DELETE_ERROR);
            var strategy = await _employeeRepository.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transactionScope = await _employeeRepository.BeginTransactionAsync().ConfigureAwait(false);
                try
                {
                    var identityResult = await _accountService.DeleteUserAsync(employee.IdentityId);
                    if (!identityResult.Succeeded)
                    {
                        result = new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_DELETE_ERROR + identityResult.Errors.FirstOrDefault()?.Description);
                        transactionScope.Rollback();
                        return;
                    }

                    await _employeeRepository.DeleteAsync(employee);
                    await _employeeRepository.SaveChangeAsync();

                    transactionScope.Commit();

                    if (employee.Title == Roles.Employee.ToString())
                    {
                        result = new SuccessDataResult<EmployeeDTO>(employee.Adapt<EmployeeDTO>(),Messages.EMPLOYEE_DELETE_SUCCESS);
                    }
                    else if (employee.Title == Roles.Manager.ToString())
                    {
                        result = new SuccessDataResult<EmployeeDTO>(employee.Adapt<EmployeeDTO>(), Messages.MANAGER_DELETED_SUCCESS);
                    }
                    else if (employee.Title == Roles.Admin.ToString())
                    {
                        result = new SuccessDataResult<EmployeeDTO>(employee.Adapt<EmployeeDTO>(), Messages.ADMIN_DELETED_SUCCESS);
                    }
                }
                catch (Exception ex)
                {
                    result = new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_DELETE_ERROR + ex.Message);
                    await transactionScope.RollbackAsync();
                }
                finally
                {
                    await transactionScope.DisposeAsync();
                }
            });

            return result;
        }

        public async Task<IDataResult<List<EmployeeListDTO>>> GetAllAsync()
        {
            IEnumerable<Employee> employeees;
            try
            {
                employeees = await _employeeRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<EmployeeListDTO>>(new List<EmployeeListDTO>(), Messages.EMPLOYEE_LISTED_ERROR + ex.Message);
            }

            if (employeees.Count() == 0)
            {
                return new ErrorDataResult<List<EmployeeListDTO>>(new List<EmployeeListDTO>(), Messages.EMPLOYEE_LISTED_NOTFOUND);
            }
            return new SuccessDataResult<List<EmployeeListDTO>>(employeees.Adapt<List<EmployeeListDTO>>(), Messages.EMPLOYEE_LISTED_SUCCESS);
        }

        /// <summary>
        /// Çalışanları belirtilen kritere göre sıralar ve liste halinde döner.
        /// </summary>
        /// <param name="sortEmployee">Sıralama kriteri (örn: "name", "namedesc").</param>
        /// <returns>Çalışan listesi ve sonuç mesajı.</returns>
        public async Task<IDataResult<List<EmployeeListDTO>>> GetAllAsync(string sortEmployee)
        {
            var employees = await _employeeRepository.GetAllAsync();


            employees = sortEmployee.ToLower() switch
            {
                "name" => employees.OrderBy(a => a.FirstName).ToList(),
                "namedesc" => employees.OrderByDescending(a => a.FirstName).ToList(),
                "createddate" => employees.OrderByDescending(a => a.CreatedDate).ToList(),
                "createddatedesc" => employees.OrderBy(a => a.CreatedDate).ToList(),
                _ => employees.OrderBy(a => a.FirstName).ToList(),
            };


            var employeeList = employees.Adapt<List<EmployeeListDTO>>();
            if (employeeList == null || employeeList.Count == 0)
            {
                return new ErrorDataResult<List<EmployeeListDTO>>(employeeList, Messages.EMPLOYEE_LISTED_NOTFOUND);
            }

            return new SuccessDataResult<List<EmployeeListDTO>>(employeeList, Messages.EMPLOYEE_LISTED_SUCCESS);
        }

        public async Task<IDataResult<EmployeeDTO>> UpdateAsync(EmployeeUpdateDTO employeeUpdateDTO)
        {
            var updatingEmployee = await _employeeRepository.GetByIdAsync(employeeUpdateDTO.Id);
            if (updatingEmployee == null)
            {
                return new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_GETBYID_ERROR);
            }

            var strategy = await _employeeRepository.CreateExecutionStrategy();
            using var transactionScope = await _employeeRepository.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                updatingEmployee.FirstName = employeeUpdateDTO.FirstName;
                updatingEmployee.LastName = employeeUpdateDTO.LastName;
                updatingEmployee.Email = employeeUpdateDTO.Email;
                updatingEmployee.CompanyId = employeeUpdateDTO.CompanyId;

                if (employeeUpdateDTO.IsPhotoRemoved)
                {
                    updatingEmployee.PhotoData=null;
                }
                else if (employeeUpdateDTO.PhotoData != null && employeeUpdateDTO.PhotoData.Length > 0)
                {
                    updatingEmployee.PhotoData = employeeUpdateDTO.PhotoData;
                }

                if (updatingEmployee.Title != employeeUpdateDTO.Title)
                {
                    var user = await _accountService.FindByIdAsync(updatingEmployee.IdentityId);

                    if (user != null)
                    {
                        var currentRoles = await _userManager.GetRolesAsync(user);

                        foreach (var role in currentRoles)
                        {
                            await _userManager.RemoveFromRoleAsync(user, role);
                        }
                        await _userManager.AddToRoleAsync(user, employeeUpdateDTO.Role.ToString());
                        updatingEmployee.Title = employeeUpdateDTO.Title;
                    }
                }

                await _employeeRepository.UpdateAsync(updatingEmployee);
                await _employeeRepository.SaveChangeAsync();

                transactionScope.Commit();
            }
            catch (Exception ex)
            {
                await transactionScope.RollbackAsync();
                return new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_UPDATE_ERROR + ex.Message);
            }
            finally
            {
                await transactionScope.DisposeAsync();
            }

            DataResult<EmployeeDTO> result;

            if (updatingEmployee.Title == Roles.Employee.ToString())
            {
                result = new SuccessDataResult<EmployeeDTO>(updatingEmployee.Adapt<EmployeeDTO>(), Messages.EMPLOYEE_UPDATE_SUCCESS);
            }
            else
            {
                result = new SuccessDataResult<EmployeeDTO>(updatingEmployee.Adapt<EmployeeDTO>(), Messages.MANAGER_UPDATE_SUCCESS);
            }

            return result;
        }
        public async Task<IDataResult<List<EmployeeListDTO>>> GetAllAsync(string sortOrder, string searchQuery)
        {
            try
            {
                // Tüm çalışanları al 
                var employees = await _employeeRepository.GetAllAsync();

                //  Dto'ya mapleme işlemini sona bırakacağız
                var employeeList = employees.ToList(); // 💡 ToList() ile belleğe al — filtreleme, string işlemleri daha güvenli yapılır

                //  searchQuery'e göre filtreleme yap
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim().ToLower();

                    var roleMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "çalışan", "Employee" },
                        { "employee", "Employee" },
                        { "yönetici", "Manager" },
                        { "manager", "Manager" }
                    };

                    if (roleMap.ContainsKey(searchQuery))
                    {
                        var titleToFilter = roleMap[searchQuery]; // veritabanındaki İngilizce karşılığı
                        employeeList = employeeList
                            .Where(e => e.Title != null && e.Title.Equals(titleToFilter, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    }
                    else
                    {
                        // Normal isim/soyisim/ünvan araması ve null kontrolü
                        employeeList = employeeList.Where(e =>
                            (!string.IsNullOrEmpty(e.FirstName) && e.FirstName.ToLower().Contains(searchQuery)) ||
                            (!string.IsNullOrEmpty(e.LastName) && e.LastName.ToLower().Contains(searchQuery)) ||
                            (!string.IsNullOrEmpty(e.Title) && e.Title.ToLower().Contains(searchQuery))
                        ).ToList();
                    }
                }

                //  Sıralama işlemi (case-insensitive güvenli)
                employeeList = sortOrder switch
                {
                    "firstName_asc" => employeeList.OrderBy(e => e.FirstName).ToList(),
                    "firstName_desc" => employeeList.OrderByDescending(e => e.FirstName).ToList(),
                    "lastName_asc" => employeeList.OrderBy(e => e.LastName).ToList(),
                    "lastName_desc" => employeeList.OrderByDescending(e => e.LastName).ToList(),
                    "alphabetical" => employeeList.OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList(),
                    _ => employeeList.OrderBy(e => e.FirstName).ToList()
                };

                // Liste boşsa uygun hata mesajı döndür
                if (!employeeList.Any())
                {
                    return new ErrorDataResult<List<EmployeeListDTO>>(new List<EmployeeListDTO>(), Messages.EMPLOYEE_LISTED_NOTFOUND);
                }

                //  DTO'ya mapleme işlemini en son yap
                var employeeDtoList = employeeList.Adapt<List<EmployeeListDTO>>();

                // Başarılı sonuç döndür
                return new SuccessDataResult<List<EmployeeListDTO>>(employeeDtoList, Messages.EMPLOYEE_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                //Hata durumunda boş liste + hata mesajı döndür
                return new ErrorDataResult<List<EmployeeListDTO>>(new List<EmployeeListDTO>(), Messages.EMPLOYEE_LISTED_ERROR + ex.Message);
            }
        }
       
		public async Task<IDataResult<List<EmployeeListDTO>>> GetByCompanyIdAsync(Guid companyId)
		{
            Console.WriteLine($"Service Layer - Filtering Employees by CompanyId: {companyId}");

            var employees = await _employeeRepository.GetByCompanyIdAsync(companyId);

            if (!employees.Any())
            {
                Console.WriteLine("Service Layer - No employees found for this company.");
                return new ErrorDataResult<List<EmployeeListDTO>>(new List<EmployeeListDTO>(), "Bu şirkete ait çalışan bulunamadı.");
            }

            foreach (var emp in employees)
            {
                Console.WriteLine($"Service Layer - Employee Name: {emp.FirstName}, CompanyId: {emp.CompanyId}");
            }

            var employeeDtos = employees.Adapt<List<EmployeeListDTO>>();

            Console.WriteLine($"Service Layer - {employeeDtos.Count} employees mapped to DTOs.");

            return new SuccessDataResult<List<EmployeeListDTO>>(employeeDtos, "Şirkete ait çalışanlar başarıyla listelendi.");
        }

        public async Task<IDataResult<EmployeeDTO>> GetByIdentityIdAsync(string identityId)
        {
            var employee = await _employeeRepository.GetByIdentityId(identityId);
            if (employee == null)
            {
                return new ErrorDataResult<EmployeeDTO>(Messages.EMPLOYEE_GETBYID_ERROR);
            }

            SuccessDataResult<EmployeeDTO> result;

            if (employee.Title == Roles.Manager.ToString())
            {
                result = new SuccessDataResult<EmployeeDTO>(employee.Adapt<EmployeeDTO>(), Messages.MANAGER_FOUND_SUCCESS);
            }
            else
            {
                result = new SuccessDataResult<EmployeeDTO>(employee.Adapt<EmployeeDTO>(), Messages.EMPLOYEE_GETBYID_SUCCESS);
            }

            return result;
        }

        public async Task<string> GetCompanyIdByUserIdAsync(string userId)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdentityId(userId);
                if (employee == null)
                {
                    return new SuccessDataResult<string>(null, Messages.EMPLOYEE_LISTED_NOTFOUND).Data;
                }
                return new SuccessDataResult<string>(employee.CompanyId.ToString(), Messages.EMPLOYEE_LISTED_SUCCESS).Data;
            }
            catch (Exception ex)
            {
                throw new Exception("Şirket ID'si alınamadı. Hata: " + ex.Message);
            }
        }
        public async Task<IDataResult<Guid?>> GetCompanyIdByUserIdAsync(Guid userId)
        {
            try
            {
                var companyId = await _employeeRepository.GetCompanyIdByUserIdAsync(userId);
                if (companyId == null)
                {
                    return new ErrorDataResult<Guid?>(null, "Şirket bilgisi bulunamadı.");
                }

                return new SuccessDataResult<Guid?>(companyId, "Şirket bilgisi başarıyla alındı.");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Guid?>(null, $"Hata oluştu: {ex.Message}");
            }
        }

		public async Task<IDataResult<EmployeeDTO>> GetByIdentityIdAsync(Guid employeeIdentityId)
		{
			var employee = await _employeeRepository.GetByIdentityId(employeeIdentityId.ToString());
			if (employee == null)
			{
				return new ErrorDataResult<EmployeeDTO>(Messages.ADMIN_GETBYID_ERROR);
			}

			return new SuccessDataResult<EmployeeDTO>(employee.Adapt<EmployeeDTO>(), Messages.ADMIN_GETBYID_SUCCESS);
		}
        public async Task<bool> IsManagerExistsAsync(Guid companyId)
        {
            return await _employeeRepository.AnyAsync(e => e.CompanyId == companyId && e.Title == "Manager");
        }

        public async Task<bool> IsAnotherManagerExistsAsync(Guid companyId, Guid excludingEmployeeId)
        {
            var employees = await _employeeRepository.GetAllAsync(e => e.CompanyId == companyId && e.Id != excludingEmployeeId && e.Title == "Manager");
            return employees.Any();
        }

        public Task<IDataResult<List<EmployeeListDTO>>> GetEmployeesByRoleAsync(string role)
        {
            throw new NotImplementedException();
        }

        public Task SearchAsync(string searchTerm)
        {
            throw new NotImplementedException();
        }
    }
}