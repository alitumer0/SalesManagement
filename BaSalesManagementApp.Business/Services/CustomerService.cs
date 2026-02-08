using BaSalesManagementApp.DataAccess.EFCore.Repositories;
using BaSalesManagementApp.DataAccess.Interfaces.Repositories;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CustomerDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.OrderDTOs;
using BaSalesManagementApp.Dtos.PromotionDTOs;
using BaSalesManagementApp.Entites.DbSets;
using Castle.Core.Resource;
using System.Security.Claims;

namespace BaSalesManagementApp.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountService _accountService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;

        /// <summary>
        /// Müşteri servisi sınıfının yeni bir örneğini başlatır.
        /// </summary>
        /// <param name="customerRepository">Müşteri veri erişim katmanı sınıfı.</param>
        /// <param name="companyRepository">Şirket veri erişim katmanı sınıfı.</param>
        /// <param name="countryRepository">Ülke veri erişim katmanı sınıfı.</param>
        /// <param name="cityRepository">Şehir veri erişim katmanı sınıfı.</param>
        public CustomerService(ICustomerRepository customerRepository, IAccountService accountService, ICompanyRepository companyRepository, ICountryRepository countryRepository, ICityRepository cityRepository)
        {
            _customerRepository = customerRepository;
            _accountService = accountService;
            _companyRepository = companyRepository;
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
        }

        /// <summary>
        /// Belirtilen sıralama kriterine göre müşteri listesini alır ve sıralar.
        /// </summary>
        /// <param name="sortOrder">Sıralama kriterleri: "date" (tarih, azalan), "datedesc" (tarih, artan), "alphabetical" (alfabetik, artan), "alphabeticaldesc" (alfabetik, azalan).</param>
        /// <returns>Sıralanmış müşteri listesini ve şirket isimlerini içeren bir görev döndürür. İşlem başarısız olursa hata mesajı döner.</returns>
        /// şehire göre asc ve desc sıralama görevleri vardır.
        public async Task<IDataResult<List<CustomerListDTO>>> GetAllAsync(string sortOrder)
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var companies = await _companyRepository.GetAllAsync();
                var cities = await _cityRepository.GetAllAsync();
                var countries = await _countryRepository.GetAllAsync();

                var customerListDTOs = customers.Adapt<List<CustomerListDTO>>() ?? new List<CustomerListDTO>();

                foreach (var customer in customerListDTOs)
                {
                    var company = companies.FirstOrDefault(x => x.Id == customer.CompanyId);
                    if (company != null)
                        customer.CompanyName = company.Name;

                    var city = cities.FirstOrDefault(x => x.Id == customer.CityId);
                    if (city != null)
                        customer.CityName = city.Name;

                    var country = countries.FirstOrDefault(x => x.Id == customer.CountryId);
                    //if (country != null)
                    //    customer.CountryName = country.Name;
                    if (country != null)
                    {
                        var currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                        customer.CountryName = currentCulture == "tr" ? country.NameTr : country.NameEn;
                    }
                }

                // Sıralama işlemi
                switch (sortOrder.ToLower())
                {
                    case "date":
                        customerListDTOs = customerListDTOs.OrderByDescending(c => c.CreatedDate).ToList();
                        break;
                    case "datedesc":
                        customerListDTOs = customerListDTOs.OrderBy(c => c.CreatedDate).ToList();
                        break;
                    case "alphabetical":
                        customerListDTOs = customerListDTOs.OrderBy(c => c.Name).ToList();
                        break;
                    case "alphabeticaldesc":
                        customerListDTOs = customerListDTOs.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "city_asc":
                        customerListDTOs = customerListDTOs.OrderBy(c => c.CityName ?? string.Empty).ToList();
                        break;
                    case "city_desc":
                        customerListDTOs = customerListDTOs.OrderByDescending(c => c.CityName ?? string.Empty).ToList();
                        break;
                    default:
                        customerListDTOs = customerListDTOs.OrderBy(c => c.Name).ToList();
                        break;
                }

                return new SuccessDataResult<List<CustomerListDTO>>(customerListDTOs, Messages.CUSTOMER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CustomerListDTO>>(Messages.CUSTOMER_LIST_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Yeni bir Müşteri ekler.
        /// Şirket, şehir ve ülke bilgileri doğrulanır; geçerli değilse hata mesajı döndürülür.
        /// </summary>
        /// <param name="customerCreateDTO">Eklenmek istenen müşteriyle ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda eklenen müşterilerin verilerini içerir.</returns>
        public async Task<IDataResult<CustomerDTO>> AddAsync(CustomerCreateDTO customerCreateDTO)
        {
            try
            {
                var company = await _companyRepository.GetByIdAsync(customerCreateDTO.CompanyId);
                var city = await _cityRepository.GetByIdAsync(customerCreateDTO.CityId);
                var country = await _countryRepository.GetByIdAsync(customerCreateDTO.CountryId);

                if (company == null || city == null || country == null)
                {
                    return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_ADD_ERROR);
                }

                var customer = customerCreateDTO.Adapt<Customer>();

                var currentUserId = _accountService.GetCurrentUserId();

                customer.CreatedBy = currentUserId;

                await _customerRepository.AddAsync(customer);
                await _customerRepository.SaveChangeAsync();

                return new SuccessDataResult<CustomerDTO>(customer.Adapt<CustomerDTO>(), Messages.CUSTOMER_ADD_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_ADD_ERROR + ex.Message);
            }
        }

        /// <summary>
        /// Benzersiz kimliğiyle bir müşteriyi siler.
        /// </summary>
        /// <param name="customerId">Silinmek istenen müşterinin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda silme işleminin başarılı olup olmadığını belirtir.</returns>
        public async Task<IResult> DeleteAsync(Guid customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return new ErrorResult(Messages.CUSTOMER_NOT_FOUND);
                }

                await _customerRepository.DeleteAsync(customer);
                await _customerRepository.SaveChangeAsync();

                return new SuccessResult(Messages.CUSTOMER_DELETE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorResult(Messages.CUSTOMER_DELETE_ERROR + ex.Message);
            }
        }

        /// <summary>
        /// Tüm müşterileri alır ve her müşterinin şirket, şehir ve ülke bilgilerini ekler.
        /// </summary>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda tüm müşterilerin listesini içerir.</returns>
        public async Task<IDataResult<List<CustomerListDTO>>> GetAllAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var companies = await _companyRepository.GetAllAsync();
                var cities = await _cityRepository.GetAllAsync();
                var countries = await _countryRepository.GetAllAsync();
                var customerListDTOs = customers.Adapt<List<CustomerListDTO>>() ?? new List<CustomerListDTO>();

                if (customers == null || customers.Count() == 0)
                {
                    return new ErrorDataResult<List<CustomerListDTO>>(customerListDTOs, Messages.CUSTOMER_LIST_EMPTY);
                }

                foreach (var customer in customerListDTOs)
                {
                    var company = companies.FirstOrDefault(x => x.Id == customer.CompanyId);
                    var city = cities.FirstOrDefault(x => x.Id == customer.CityId);
                    var country = countries.FirstOrDefault(x => x.Id == customer.CountryId);

                    if (company != null) customer.CompanyName = company.Name;
                    if (city != null) customer.CityName = city.Name;
                    //if (country != null) customer.CountryName = country.Name;
                    if (country != null)
                    {
                        var currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                        customer.CountryName = currentCulture == "tr" ? country.NameTr : country.NameEn;
                    }
                }

                return new SuccessDataResult<List<CustomerListDTO>>(customerListDTOs, Messages.CUSTOMER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CustomerListDTO>>(Messages.CUSTOMER_LIST_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Benzersiz kimliğiyle bir müşteri alır.
        /// </summary>
        /// <param name="customerId">Alınmak istenen müşterinin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda müşteri verilerini içerir, bulunamazsa null döner.</returns>
        public async Task<IDataResult<CustomerDTO>> GetByIdAsync(Guid customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_NOT_FOUND);
                }

                var customerDTO = customer.Adapt<CustomerDTO>();
                var company = await _companyRepository.GetByIdAsync(customerDTO.CompanyId);

                if (company != null)
                    customerDTO.CompanyName = company.Name;

                var country = await _countryRepository.GetByIdAsync(customerDTO.CountryId);
                if (country != null)
                {
                    var currentCulture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                    customerDTO.CountryName = currentCulture switch
                    {
                        "tr" => country.NameTr,
                        "en" => country.NameEn,
                        _ => country.NameEn
                    };
                }



                return new SuccessDataResult<CustomerDTO>(customer.Adapt<CustomerDTO>(), Messages.CUSTOMER_FOUND_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_GET_FAILED + ex.Message);
            }
        }


        /// <summary>
        /// Bir müşteriyi günceller.
        /// Müşteri, şirket, şehir ve ülke bilgileri doğrulanır; geçerli değilse hata mesajı döndürülür.
        /// </summary>
        /// <param name="customerUpdateDTO">Güncellenmiş müşteriyle ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda güncellenmiş müşteri verilerini içerir.</returns>
        public async Task<IDataResult<CustomerDTO>> UpdateAsync(CustomerUpdateDTO customerUpdateDTO)
        {
            try
            {
                var oldCustomer = await _customerRepository.GetByIdAsync(customerUpdateDTO.Id);
                if (oldCustomer == null)
                {
                    return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_NOT_FOUND);
                }

                var company = await _companyRepository.GetByIdAsync(customerUpdateDTO.CompanyId);
                if (company == null)
                {
                    return new ErrorDataResult<CustomerDTO>(Messages.COMPANY_GETBYID_ERROR);
                }

                var city = await _cityRepository.GetByIdAsync(customerUpdateDTO.CityId);
                var country = await _countryRepository.GetByIdAsync(customerUpdateDTO.CountryId);

                customerUpdateDTO.Adapt(oldCustomer);
                await _customerRepository.UpdateAsync(customerUpdateDTO.Adapt(oldCustomer));
                await _customerRepository.SaveChangeAsync();

                return new SuccessDataResult<CustomerDTO>(oldCustomer.Adapt<CustomerDTO>(), Messages.CUSTOMER_UPDATED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_UPDATED_FAILED);
            }
        }

        public async Task<IDataResult<List<CustomerListDTO>>> GetAllAsync(string sortOrder, string searchQuery)
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var companies = await _companyRepository.GetAllAsync();

                // Filtreleme işlemi
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    customers = customers.Where(c => c.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                var customerListDTOs = customers.Adapt<List<CustomerListDTO>>() ?? new List<CustomerListDTO>();

                foreach (var customer in customerListDTOs)
                {
                    var company = companies.FirstOrDefault(x => x.Id == customer.CompanyId);
                    if (company != null)
                        customer.CompanyName = company.Name;
                }

                // Sıralama işlemi
                switch (sortOrder.ToLower())
                {
                    case "date":
                        customerListDTOs = customerListDTOs.OrderByDescending(c => c.CreatedDate).ToList();
                        break;
                    case "datedesc":
                        customerListDTOs = customerListDTOs.OrderBy(c => c.CreatedDate).ToList();
                        break;
                    case "alphabetical":
                        customerListDTOs = customerListDTOs.OrderBy(c => c.Name).ToList();
                        break;
                    case "alphabeticaldesc":
                        customerListDTOs = customerListDTOs.OrderByDescending(c => c.Name).ToList();
                        break;
                }

                return new SuccessDataResult<List<CustomerListDTO>>(customerListDTOs, Messages.CUSTOMER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CustomerListDTO>>(Messages.CUSTOMER_LIST_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Müşteri id bilgisine göre geçmiş siparişlerini listeleyen metot
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="sortOrder"></param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public async Task<IDataResult<List<OrderListDTO>>> GetCustomerOrdersHistoryAsync(Guid customerId, string sortOrder, string searchQuery)
        {
            try
            {
                // CustomerId'ye göre filtreleme ve tarihe göre sıralama
                var orders = await _customerRepository.GetAllAsync(
                    o => o.CustomerId == customerId && o.IsActive, // CustomerId ve aktif siparişler
                    o => o.OrderDate,                              // Sipariş tarihine göre sıralama
                    orderDesc: true                                // Azalan sıralama
                );


                if (orders == null || !orders.Any())
                {
                    return new ErrorDataResult<List<OrderListDTO>>(Messages.ORDER_LIST_EMPTY);
                }
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    orders = orders.Where(order => order.OrderDetails
                    .Any(orderDetail => orderDetail.Product.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }


                var orderList = orders.Select(order => new OrderListDTO
                {
                    Id = order.Id,
                    TotalPrice = order.TotalPrice,
                    OrderDate = order.OrderDate,
                    IsActive = order.IsActive,
                    AdminName = order.Admin.FirstName + " " + order.Admin.LastName,
                    CustomerName = order.Customer.Name,
                    OrderDetails = order.OrderDetails != null
                ? order.OrderDetails.Where(od => od.Product.Company.Status != Status.Deleted)
            .Select(od => new OrderDetailListDTO
            {
                Id = od.Id,
                ProductName = od.Product.Name,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                Discount = od.Discount,
                CompanyName = od.Product.Company.Name,
                IsCompanyActive = od.Product.Company.Status != Status.Passive
            }).ToList()
        : new List<OrderDetailListDTO>() // OrderDetails null ise boş liste döndür

                }).ToList();

                if (orderList == null || orderList.Count == 0 || !orderList.Any())
                {
                    return new ErrorDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LIST_EMPTY);
                }

                // Sıralama işlemi
                switch (sortOrder.ToLower())
                {
                    case "date":
                        orderList = orderList.OrderByDescending(s => s.OrderDate).ToList(); // Tarihe göre azalan sıralama
                        break;
                    case "datedesc":
                        orderList = orderList.OrderBy(s => s.OrderDate).ToList(); // Tarihe göre artan sıralama
                        break;
                    case "active":
                        orderList = orderList.Where(s => s.IsActive == true).ToList(); // Aktif siparişler
                        break;
                    case "inactive":
                        orderList = orderList.Where(s => s.IsActive == false).ToList(); // Pasif siparişler
                        break;

                }

                return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LISTED_SUCCESS);


            }
            catch (Exception ex)
            {

                return new ErrorDataResult<List<OrderListDTO>>(new List<OrderListDTO>(), Messages.ORDER_LIST_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Company Id bilgisine göre müşterileri getirir ve sıralar
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        /// Şehire göre asc desc sıralama özelliğide vardır
        public async Task<IDataResult<List<CustomerListDTO>>> GetCustomersByCompanyId(Guid companyId, string sortOrder)
        {
            var customers = await _customerRepository.GetCustomersByCompany(companyId);
            var companies = await _companyRepository.GetAllAsync();
            var cities = await _cityRepository.GetAllAsync();
            var countries = await _countryRepository.GetAllAsync();

            var customerListDTOs = customers.Adapt<List<CustomerListDTO>>() ?? new List<CustomerListDTO>();

            if (customers == null || customers.Count() == 0)
            {
                return new ErrorDataResult<List<CustomerListDTO>>(customerListDTOs, Messages.CUSTOMER_LIST_EMPTY);
            }

            foreach (var customer in customerListDTOs)
            {
                var company = companies.FirstOrDefault(x => x.Id == companyId);
                var city = cities.FirstOrDefault(x => x.Id == customer.CityId);
                var country = countries.FirstOrDefault(x => x.Id == customer.CountryId);

                if (company != null) customer.CompanyName = company.Name;
                if (city != null) customer.CityName = city.Name;
                //if (country != null) customer.CountryName = country.Name;
                if (country != null)
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                    customer.CountryName = currentCulture == "tr" ? country.NameTr : country.NameEn;
                }
            }

            // Sıralama işlemi
            switch (sortOrder.ToLower())
            {
                case "date":
                    customerListDTOs = customerListDTOs.OrderByDescending(c => c.CreatedDate).ToList();
                    break;
                case "datedesc":
                    customerListDTOs = customerListDTOs.OrderBy(c => c.CreatedDate).ToList();
                    break;
                case "alphabetical":
                    customerListDTOs = customerListDTOs.OrderBy(c => c.Name).ToList();
                    break;
                case "alphabeticaldesc":
                    customerListDTOs = customerListDTOs.OrderByDescending(c => c.Name).ToList();
                    break;
                case "city_asc":
                    customerListDTOs = customerListDTOs.OrderBy(c => c.CityName ?? string.Empty).ToList();
                    break;
                case "city_desc":
                    customerListDTOs = customerListDTOs.OrderByDescending(c => c.CityName ?? string.Empty).ToList();
                    break;
                default:
                    customerListDTOs = customerListDTOs.OrderBy(c => c.Name).ToList();
                    break;
            }

            return new SuccessDataResult<List<CustomerListDTO>>(customerListDTOs, Messages.CUSTOMER_LISTED_SUCCESS);
        }
    }
}
