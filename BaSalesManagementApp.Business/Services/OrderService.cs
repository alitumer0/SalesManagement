using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.OrderDTOs;
using BaSalesManagementApp.Dtos.ReportingDTOs;
using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.Business.Services
{
    /// <summary>
    /// OrderService sınıfı, siparişlerle ilgili CRUD işlemlerini gerçekleştirir.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrencyService _currencyService;

        /// <summary>
        /// OrderService kurucusu, IOrderRepository bağımlılığını alır.
        /// </summary>
        /// <param name="orderRepository">Sipariş deposu</param>
        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IEmployeeRepository employeeRepository, IAdminRepository adminRepository, ICustomerRepository customerRepository, IProductRepository productRepository, ICurrencyService currencyService)
        {
            _orderRepository = orderRepository;
            _adminRepository = adminRepository;
            _orderDetailRepository = orderDetailRepository;
            _employeeRepository = employeeRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _currencyService = currencyService;

        }

        /// <summary>
        /// Tüm siparişleri getirir.
        /// </summary>
        /// <returns>Tüm siparişlerin listesi</returns>
        public async Task<IDataResult<List<OrderListDTO>>> GetAllAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllWithAdminAsync();
                var orderList = orders.Select(order => new OrderListDTO
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    TotalPrice = order.TotalPrice,
                    OrderDate = order.OrderDate,
                    IsActive = order.IsActive,
                    AdminName = order.Admin.FirstName + " " + order.Admin.LastName,
                    OrderDetails = order.OrderDetails
                        .Where(od => od.Product.Company.Status != Status.Deleted) // Silinmiş şirketlere ait order detail'lar gösterilmiyor
                        .Select(od => new OrderDetailListDTO
                        {
                            Id = od.Id,
                            OrderId = order.Id,
                            ProductId = od.Product.Id,
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice,
                            Discount = od.Discount,
                            TotalPrice = od.TotalPrice,
                            ProductName = od.Product.Name,
                            CompanyName = od.Product.Company.Name,
                            IsCompanyActive = od.Product.Company.Status != Status.Passive
                        }).ToList()
                }).ToList();

                if (orderList == null || orderList.Count == 0)
                {
                    return new ErrorDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LIST_EMPTY);
                }

                return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<OrderListDTO>>(new List<OrderListDTO>(), Messages.ORDER_LIST_FAILED + ex.Message);
            }
        }
        /// <summary>
        /// Tüm siparişleri getirir ve belirli bir sıralama kriterine göre sıralar.
        /// </summary>
        /// <param name="sortOrder">Sıralama kriteri (date, datedesc, active, inactive).</param>
        /// <returns>
        /// Filtrelenmiş ve sıralanmış siparişleri içeren <see cref="IDataResult{List{OrderListDTO}}"/> nesnesi döner.
        /// Başarılıysa sipariş listesi ve başarı mesajı, başarısızsa hata mesajı döndürülür.
        /// </returns>
        public async Task<IDataResult<List<OrderListDTO>>> GetAllAsync(string sortOrder)
        {
            try
            {
                var orders = await _orderRepository.GetAllWithAdminAsync();
                var orderList = orders.Select(order => new OrderListDTO
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    TotalPrice = order.TotalPrice,
                    OrderDate = order.OrderDate,
                    IsActive = order.IsActive,
                    CustomerName = order.Customer.Name,
                    CompanyName = order.Company.Name,
                    AdminName = order.Admin.FirstName + " " + order.Admin.LastName,
                    OrderDetails = order.OrderDetails
                        .Where(od => od.Product.Company.Status != Status.Deleted) // Silinmiş şirketlere ait order detail'lar gösterilmiyor
                        .Select(od => new OrderDetailListDTO
                        {
                            Id = od.Id,
                            OrderId = order.Id,
                            ProductId = od.Product.Id,
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice,
                            Discount = od.Discount,
                            TotalPrice = od.TotalPrice,
                            PurchasePrice = od.PurchasePrice,
                            ProductName = od.Product.Name,
                            //CompanyName = od.Product.Company.Name,
                            IsCompanyActive = od.Product.Company.Status != Status.Passive
                        }).ToList()
                }).ToList();

                if (orderList == null || orderList.Count == 0)
                {
                    return new ErrorDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LIST_EMPTY);
                }

                switch (sortOrder.ToLower())
                {
                    case "date":
                        orderList = orderList.OrderByDescending(s => s.OrderDate).ToList();
                        break;
                    case "datedesc":
                        orderList = orderList.OrderBy(s => s.OrderDate).ToList();
                        break;
                    case "active":
                        orderList = orderList.Where(s => s.IsActive == true).ToList();
                        break;
                    case "inactive":
                        orderList = orderList.Where(s => s.IsActive == false).ToList();
                        break;
                        //case "alphabetical":
                        //    orderList = orderList.OrderBy(s => s.ProductName).ToList();
                        //    break;
                        //case "alphabeticaldesc":
                        //    orderList = orderList.OrderByDescending(s => s.ProductName).ToList();
                        //    break;
                }

                return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<OrderListDTO>>(new List<OrderListDTO>(), Messages.ORDER_LIST_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Belirtilen ID'li siparişi getirir.
        /// </summary>
        /// <param name="orderId">Getirilecek siparişin ID'si</param>
        /// <returns>Belirtilen ID'li siparişin verileri</returns>
        public async Task<IDataResult<OrderDTO>> GetByIdAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ErrorDataResult<OrderDTO>(Messages.ORDER_NOT_FOUND);
                }

                return new SuccessDataResult<OrderDTO>(order.Adapt<OrderDTO>(), Messages.ORDER_FOUND_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<OrderDTO>(Messages.ORDER_GET_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Aranan siparişi admin, product ve company bilgileri ile birlikte getirir.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<IDataResult<OrderListDTO>> GetOrderWithDetailsByIdAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderWithAdminAsync(orderId);
                if (order == null)
                {
                    return new ErrorDataResult<OrderListDTO>(Messages.ORDER_NOT_FOUND);
                }


                var orderListDTO = order.Adapt<OrderListDTO>();

                if (order != null)
                {
                    orderListDTO.CustomerName = order.Customer.Name;
                }

                var orderDetails = order.OrderDetails.ToList();
                for (int i = 0; i < orderDetails.Count; i++)
                {
                    orderListDTO.OrderDetails[i].CompanyDTO.Name = orderDetails[i].Product.Company.Name;
                    orderListDTO.OrderDetails[i].CompanyDTO.Address = orderDetails[i].Product.Company.Address;
                    orderListDTO.OrderDetails[i].CompanyDTO.Phone = orderDetails[i].Product.Company.Phone;
                    orderListDTO.OrderDetails[i].CompanyDTO.Status = orderDetails[i].Product.Company.Status;

                }

                return new SuccessDataResult<OrderListDTO>(orderListDTO, Messages.ORDER_FOUND_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<OrderListDTO>(Messages.ORDER_GET_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Yeni bir sipariş oluşturur.
        /// </summary>
        /// <param name="orderCreateDTO">Oluşturulacak sipariş bilgileri</param>
        /// <returns>Sipariş oluşturma işleminin sonucu</returns>
        public async Task<IDataResult<OrderDTO>> AddAsync(OrderCreateDTO orderCreateDTO, string identityId)
        {
            try
            {
                var order = orderCreateDTO.Adapt<Order>();

                foreach (var detail in order.OrderDetails)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    detail.PurchasePrice = product?.PurchasePrice ?? 0;
                }
                //Identity Id yi kullan
                var admin = await _adminRepository.GetByIdentityId(identityId);
                var employee = await _employeeRepository.GetByIdentityId(identityId);

                if (admin == null)
                {
                    order.EmployeeId = employee.Id;
                    //throw new Exception("Admin could not be determined from the IdentityId.");
                }
                else if (employee == null)
                {
                    order.AdminId = admin.Id;
                    //throw new Exception("Employee could not be determined from the IdentityId.");
                }

                // order değişkeni içerisindeki OrderDetails'a orderId burada eklendi
                await _orderRepository.AddAsync(order);
                await _orderRepository.SaveChangeAsync();

                // EF Relationship Tracking ve relationship fix-up sayesinde buraya gerek kalmadı
                //if (orderCreateDTO.OrderDetails != null && orderCreateDTO.OrderDetails.Any())
                //{
                //    foreach (var OrderDetailCreateDTO in orderCreateDTO.OrderDetails)
                //    {
                //        var orderDetail = OrderDetailCreateDTO.Adapt<OrderDetail>();
                //        orderDetail.OrderId = order.Id;
                //        await _orderDetailRepository.AddAsync(orderDetail);
                //    }
                //    await _orderDetailRepository.SaveChangeAsync();
                //}

                return new SuccessDataResult<OrderDTO>(order.Adapt<OrderDTO>(), Messages.ORDER_CREATED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<OrderDTO>(orderCreateDTO.Adapt<OrderDTO>(), Messages.ORDER_CREATE_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Mevcut bir siparişi günceller, sipariş detaylarını ekler, günceller veya kaldırır.
        /// </summary>
        /// <param name="orderUpdateDTO">Güncellenecek sipariş bilgilerini içeren DTO.</param>
        /// <returns>
        /// Güncellenmiş sipariş bilgisini içeren <see cref="IDataResult{OrderDTO}"/> nesnesi döner.
        /// Başarılıysa güncellenmiş sipariş ve başarı mesajı, başarısızsa hata mesajı döndürülür.
        /// </returns>
        public async Task<IDataResult<OrderDTO>> UpdateAsync(OrderUpdateDTO orderUpdateDTO)
        {
            try
            {
                // Mevcut siparişi detaylarıyla getir
                var existingOrder = await _orderRepository.GetByIdWithDetailsAsync(orderUpdateDTO.Id);

                if (existingOrder == null)
                {
                    return new ErrorDataResult<OrderDTO>(Messages.ORDER_NOT_FOUND);
                }

                // Sipariş toplam fiyatını güncelle
                existingOrder.TotalPrice = orderUpdateDTO.TotalPrice;

                // Mevcut ürünlerin ID'lerini takip et
                var updatedOrderDetailIds = new HashSet<Guid>();

                foreach (var detailDTO in orderUpdateDTO.OrderDetails)
                {
                    // Mevcut detayları kontrol et
                    var existingDetail = existingOrder.OrderDetails.FirstOrDefault(d => d.ProductId == detailDTO.ProductId && d.Discount == detailDTO.Discount);

                    if (existingDetail != null)
                    {
                        // Aynı ürün bulundu: Miktarı ve fiyatı güncelle
                        existingDetail.Quantity = detailDTO.Quantity;
                        existingDetail.Discount = detailDTO.Discount; // Yeni indirim uygulanır
                        existingDetail.TotalPrice = existingDetail.Quantity * existingDetail.UnitPrice * ((100 - existingDetail.Discount) / 100);

                        updatedOrderDetailIds.Add(existingDetail.Id); // Güncellenen detayı takip et
                    }
                    else
                    {
                        // Yeni ürün ekleniyor
                        var newDetail = new OrderDetail
                        {
                            OrderId = existingOrder.Id,
                            ProductId = detailDTO.ProductId,
                            Quantity = detailDTO.Quantity,
                            UnitPrice = detailDTO.UnitPrice,
                            Discount = detailDTO.Discount,
                            TotalPrice = detailDTO.TotalPrice
                        };

                        existingOrder.OrderDetails.Add(newDetail);
                        updatedOrderDetailIds.Add(newDetail.Id); // Yeni detayın ID'sini ekle
                        await _orderDetailRepository.AddAsync(newDetail); // Yeni detay ekle
                    }
                }

                // Silinmesi gereken detayları belirle
                var detailsToRemove = existingOrder.OrderDetails
                    .Where(d => !updatedOrderDetailIds.Contains(d.Id))
                    .ToList();

                foreach (var detail in detailsToRemove)
                {
                    existingOrder.OrderDetails.Remove(detail); // Detayı ilişkiden kaldır
                    await _orderDetailRepository.DeleteAsync(detail); // Veritabanından sil
                }

                // Güncellemeyi kaydet
                await _orderRepository.UpdateAsync(existingOrder);
                await _orderRepository.SaveChangeAsync();

                // Güncellenen siparişi DTO olarak döndür
                return new SuccessDataResult<OrderDTO>(existingOrder.Adapt<OrderDTO>(), Messages.ORDER_UPDATE_SUCCES);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<OrderDTO>(Messages.ORDER_UPDATE_FAILED + ex.Message);
            }
        }
        /// <summary>
        /// Belirtilen ID'li siparişi siler.
        /// </summary>
        /// <param name="orderId">Silinecek siparişin ID'si</param>
        /// <returns>Sipariş silme işleminin sonucu</returns>
        public async Task<IResult> DeleteAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ErrorResult(Messages.ORDER_NOT_FOUND);
                }

                await _orderRepository.DeleteAsync(order);
                await _orderRepository.SaveChangeAsync();

                return new SuccessResult(Messages.ORDER_DELETED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorResult(Messages.ORDER_DELETE_FAILED + ex.Message);
            }
        }

        // Daha önce eklenip bu şekilde bırakılmış gerekli midir ? 
        public Task<IDataResult<OrderDTO>> AddAsync(OrderCreateDTO orderCreateDTO)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Tüm siparişleri getirir, arama sorgusuna ve sıralama seçeneğine göre filtreler.
        /// </summary>
        /// <param name="sortOrder">Sıralama kriteri (date, datedesc, active, inactive).</param>
        /// <param name="searchQuery">Ürün adına göre arama yapmak için kullanılan sorgu.</param>
        /// <returns>
        /// Filtrelenmiş ve sıralanmış siparişleri içeren <see cref="IDataResult{List{OrderListDTO}}"/> nesnesi döner.
        /// Başarılıysa sipariş listesi ve başarı mesajı, başarısızsa hata mesajı döndürülür.
        /// </returns>
        public async Task<IDataResult<List<OrderListDTO>>> GetAllAsync(string sortOrder, string searchQuery)
        {
            try
            {
                // Tüm siparişleri ilişkilerle birlikte çekiyoruz
                var orders = await _orderRepository.GetAllWithAdminAsync();

                // 🔍 Arama: ürün adı, müşteri adı, şirket adı, sipariş no, tutar, admin/employee adı
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    searchQuery = searchQuery.ToLower();

                    orders = orders.Where(order =>
                        // Ürün adı
                        order.OrderDetails.Any(od => od.Product.Name.ToLower().Contains(searchQuery)) ||

                        // Müşteri adı
                        (order.Customer != null && order.Customer.Name.ToLower().Contains(searchQuery)) ||

                        // Şirket adı
                        (order.Company != null && order.Company.Name.ToLower().Contains(searchQuery)) ||

                        // Admin adı
                        (order.Admin != null &&
                            ($"{order.Admin.FirstName} {order.Admin.LastName}".ToLower().Contains(searchQuery))) ||

                        // Employee adı
                        (order.Employee != null &&
                            ($"{order.Employee.FirstName} {order.Employee.LastName}".ToLower().Contains(searchQuery))) ||

                        // Sipariş numarası
                        (!string.IsNullOrEmpty(order.OrderNo) && order.OrderNo.ToLower().Contains(searchQuery)) ||

                        // Tutar (string karşılaştırma için)
                        order.TotalPrice.ToString().Contains(searchQuery)
                    ).ToList();
                }

                // DTO'ya dönüştür
                var orderList = orders.Select(order => new OrderListDTO
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    TotalPrice = order.TotalPrice,
                    OrderDate = order.OrderDate,
                    IsActive = order.IsActive,
                    AdminName = order.Admin != null
                        ? $"{order.Admin.FirstName} {order.Admin.LastName}"
                        : order.Employee != null
                            ? $"{order.Employee.FirstName} {order.Employee.LastName}"
                            : "",
                    CustomerName = order.Customer?.Name ?? "",
                    CompanyName = order.Company?.Name ?? "",
                    OrderDetails = order.OrderDetails
                        .Where(od => od.Product.Company.Status != Status.Deleted) // Silinmiş şirketlere ait order detail'lar gösterilmiyor
                        .Select(od => new OrderDetailListDTO
                        {
                            Id = od.Id,
                            ProductName = od.Product?.Name ?? "No Product",
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice,
                            Discount = od.Discount,
                            CompanyName = od.Product?.Company?.Name ?? "No Company",
                            IsCompanyActive = od.Product.Company.Status != Status.Passive
                        }).ToList()
                }).ToList();

                if (orderList == null || orderList.Count == 0)
                {
                    return new ErrorDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LIST_EMPTY);
                }

                // 📌 Sıralama işlemi
                switch (sortOrder?.ToLower())
                {
                    case "date":
                        orderList = orderList.OrderByDescending(s => s.OrderDate).ToList();
                        break;
                    case "datedesc":
                        orderList = orderList.OrderBy(s => s.OrderDate).ToList();
                        break;
                    case "active":
                        orderList = orderList.Where(s => s.IsActive).ToList();
                        break;
                    case "inactive":
                        orderList = orderList.Where(s => !s.IsActive).ToList();
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
        /// Belirtilen şirkete ait siparişleri getirir.
        /// </summary>
        /// <param name="companyId">Şirketin benzersiz kimliği.</param>
        /// <returns>
        /// Şirketin siparişlerini içeren <see cref="IDataResult{List{OrderListDTO}}"/> nesnesi döner.
        /// Başarılıysa sipariş listesi ve başarı mesajı, başarısızsa hata mesajı döndürülür.
        /// </returns>
        public async Task<IDataResult<List<OrderListDTO>>> GetOrdersByCompanyIdAsync(Guid companyId)
        {
            try
            {
                var query = await _orderRepository.GetOrdersByCompany(companyId);
                if (query == null)
                    return new ErrorDataResult<List<OrderListDTO>>(Messages.ORDER_LIST_EMPTY);

                var orderList = query.Select(x => new OrderListDTO
                {
                    Id = x.Id,
                    OrderNo = x.OrderNo,
                    CompanyId = x.CompanyId,
                    TotalPrice = x.TotalPrice,
                    AdminId = x.AdminId,
                    CompanyName = x.Company.Name,
                    IsActive = x.IsActive,
                    OrderDate = x.OrderDate,
                    AdminName = x.Employee.FirstName + " " + x.Employee.LastName,
                    OrderDetails = x.OrderDetails.Select(x => new OrderDetailListDTO
                    {
                        Id = x.Id,
                        ProductName = x.Product.Name,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        Discount = x.Discount,
                        CompanyName = x.Product.Company.Name
                    }).ToList()

                }).ToList();

                if (orderList is null)
                {
                    return new ErrorDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LIST_EMPTY);
                }

                return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {

                return new ErrorDataResult<List<OrderListDTO>>(new List<OrderListDTO>(), Messages.ORDER_LIST_FAILED + ex.Message);
            }
        }
        /// <summary>
        /// Verilen çalışan ID'sine göre ilgili siparişleri getirir ve DTO'ya dönüştürerek döner.
        /// </summary>
        /// <param name="employeeId">Çalışanın benzersiz kimlik numarası (Guid).</param>
        /// <returns>
        /// Eğer çalışanla ilişkili siparişler bulunursa bir başarı sonucu ile birlikte 
        /// <see cref="List{OrderListDTO}"/> döner.
        /// Eğer çalışanla ilişkili sipariş bulunmazsa, hata sonucu ve boş bir liste döner.
        /// </returns>
        /// <exception cref="Exception">Beklenmeyen bir hata oluşması durumunda yakalanır ve hata mesajıyla döner.</exception>
        public async Task<IDataResult<List<OrderListDTO>>> GetOrdersByEmployeeIdAsync(Guid employeeId)
        {
            try
            {
                // Çalışanın siparişlerini veri tabanından getir
                var orders = await _orderRepository.GetOrdersByEmployee(employeeId);

                if (orders == null || !orders.Any())
                {
                    return new ErrorDataResult<List<OrderListDTO>>(Messages.ORDER_LIST_EMPTY);
                }

                Console.WriteLine("Hata öncesi");

                // DTO'ya dönüştür
                var orderList = orders
                    .Where(order => order != null) // Null siparişleri filtrele
                    .Select(order => new OrderListDTO
                    {
                        Id = order.Id,
                        OrderNo = order.OrderNo,
                        TotalPrice = order.TotalPrice,
                        OrderDate = order.OrderDate,
                        IsActive = order.IsActive,
                        EmployeeName = $"{order.Employee?.FirstName} {order.Employee?.LastName}" ?? "No Employee",
                        CustomerName = order.Customer?.Name ?? "No Customer",
                        CompanyName = order.Company?.Name ?? "No Company",
                        OrderDetails = order.OrderDetails?
                            .Where(detail => detail != null) // Null detayları filtrele
                            .Select(detail => new OrderDetailListDTO
                            {
                                ProductName = detail.Product?.Name ?? "No Product",
                                CompanyName = detail.Product?.Company?.Name ?? "No Company",
                                Quantity = detail.Quantity,
                                UnitPrice = detail.UnitPrice,
                                Discount = detail.Discount,
                                TotalPrice = detail.Quantity * detail.UnitPrice * (1 - detail.Discount / 100)
                            }).ToList() ?? new List<OrderDetailListDTO>() // Eğer null ise boş liste döndür
                    }).ToList();

                return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<OrderListDTO>>(Messages.ORDER_LIST_FAILED + ex.Message);
            }

        }

        /// <summary>
        /// Şirket Id bilgisi alınarak, şirkete ait siparişleri getirir.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns> Asenkron işlemi temsil eden bir görev. Görev sonucunda sipariş listesini döndürür. </returns>
        //public async Task<IDataResult<List<OrderListDTO>>> GetOrdersListByCompanyIdAsync(Guid companyId)
        //{
        //    var customersResult = await _customerRepository.GetAllAsync();
        //    var customers = customersResult.Where(c => c.CompanyId == companyId).ToList();

        //    var ordersResult = await _orderRepository.GetAllAsync();
        //    var orders = ordersResult.Where(o => customers.Any(c => c.Id == o.CustomerId)).ToList();

        //    var orderList = orders.Select(order => new OrderListDTO
        //    {
        //        Id = order.Id,
        //        OrderNo = order.OrderNo,
        //        TotalPrice = order.TotalPrice,
        //        CurrencyType = order.CurrencyType,
        //        OrderDate = order.OrderDate,
        //        IsActive = order.IsActive,
        //        CompanyId = order.CompanyId,
        //        CompanyName = order.Company.Name,
        //        Email = order.Employee.Email,
        //        EmployeeId = order.EmployeeId,
        //        EmployeeName = order.Employee.FirstName,
        //        CustomerId = order.CustomerId,
        //        CustomerName = order.Customer.Name,
        //        AdminName = order.Admin != null
        //            ? order.Admin.FirstName + " " + order.Admin.LastName
        //            : order.Employee.FirstName + " " + order.Employee.LastName
        //    }).ToList();

        //    return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LISTED_SUCCESS);
        //}

        /// <summary> employee null olunca System.NullReferenceException hatası fırlatıyordu. null kontrolü yaparak bu hatayı engelledik.
        public async Task<IDataResult<List<OrderListDTO>>> GetOrdersListByCompanyIdAsync(Guid companyId)
        {
            //  Şirkete bağlı müşteri Id lerini alıyoruz
            var customersResult = await _customerRepository.GetAllAsync();
            var customerIds = customersResult
                .Where(c => c.CompanyId == companyId)
                .Select(c => (Guid?)c.Id)
                .ToHashSet();

            // Siparişleri müşteri Id’leri üzerinden alıyoruz
            var ordersResult = await _orderRepository.GetAllAsync();
            var orders = ordersResult
                .Where(o => o.CustomerId != null && customerIds.Contains(o.CustomerId))
                .ToList();

            //null kontrolü
            var orderList = orders.Select(order => new OrderListDTO
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                TotalPrice = order.TotalPrice,
                CurrencyType = order.CurrencyType,
                OrderDate = order.OrderDate,
                IsActive = order.IsActive,

                CompanyId = order.Customer?.CompanyId ?? Guid.Empty,
                CompanyName = order.Customer?.Company?.Name ?? "-",

                Email = order.Employee?.Email ?? "-",
                EmployeeId = order.EmployeeId,
                EmployeeName = order.Employee != null
                    ? (order.Employee.FirstName + " " + order.Employee.LastName)
                    : "-",

                CustomerId = order.CustomerId ?? Guid.Empty,
                CustomerName = order.Customer?.Name ?? "-",

                AdminName = order.Admin != null
                    ? (order.Admin.FirstName + " " + order.Admin.LastName)
                    : (order.Employee != null
                        ? (order.Employee.FirstName + " " + order.Employee.LastName)
                        : "-")
            }).ToList();

            return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LISTED_SUCCESS);
        }
        public async Task<IDataResult<List<OrderListDTO>>> GetOrdersListByCustomerIdAsync(Guid customerId)
        {
            // Müşteri ID'sine göre müşteri verilerini alıyoruz
            var customerResult = await _customerRepository.GetAllAsync();
            var customer = customerResult.FirstOrDefault(c => c.Id == customerId);

            if (customer == null)
            {
                return new ErrorDataResult<List<OrderListDTO>>(null, Messages.CUSTOMER_LISTED_ERROR);
            }

            // Müşteri ile ilişkili olan tüm siparişleri alıyoruz
            var ordersResult = await _orderRepository.GetAllAsync();
            var orders = ordersResult.Where(o => o.CustomerId == customerId).ToList();

            // Sipariş verilerini DTO formatında dönüştürüyoruz
            var orderList = orders.Select(order => new OrderListDTO
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                TotalPrice = order.TotalPrice,
                CurrencyType = order.CurrencyType,
                OrderDate = order.OrderDate,
                IsActive = order.IsActive,
                CompanyId = order.CompanyId,
                CompanyName = order.Company != null ? order.Company.Name : "",
                Email = order.Employee != null ? order.Employee.Email : "",
                CustomerId = order.CustomerId,
                CustomerName = order.Customer != null ? order.Customer.Name : "",
                AdminName = order.Admin != null
        ? order.Admin.FirstName + " " + order.Admin.LastName
        : order.Employee != null
            ? order.Employee.FirstName + " " + order.Employee.LastName
            : ""
            }).ToList();
            // Başarılı bir şekilde listeyi döndürüyoruz
            return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.CUSTOMER_LISTED_SUCCESS);
        }

        public async Task<decimal> GetTotalIncomeAsync() //toplam gelir
        {
            var orders = await _orderRepository.GetAllWithDetailsAsync();//Tüm siparişleri, içindeki OrderDetails + Product + Company Repositoryden geliyor

            var totalIncome = orders
                .Where(o => o.IsActive && o.DeletedDate == null) //aktif ve silinmemiş
                .SelectMany(o => o.OrderDetails)
                .Where(od => od.Product.Company.Status != Status.Deleted)//şirketi silinmemiş ürünler
                .Where(od => (od.TotalPrice ?? (od.UnitPrice * od.Quantity)) > (od.PurchasePrice * od.Quantity)) // ✔️ Zarar edenleri filtrele
                .Sum(od => od.TotalPrice ?? (od.UnitPrice * od.Quantity)); //toplam gelir

            return totalIncome;
        }

        public async Task<SalesProfitDTO> GetExpenseMetricAsync(int? year = null, Guid? companyId = null, bool costInOrderCurrency = true)
        {
            // dönemleri hazırla (yıllık verildiyse yıl, yoksa son 30 gün)
            (DateTime start, DateTime end) cur, prev;
            if (year.HasValue)
            {
                var s = new DateTime(year.Value, 1, 1);
                var e = new DateTime(year.Value, 12, 31, 23, 59, 59);
                cur = (s, e);
                prev = (s.AddYears(-1), e.AddYears(-1));
            }
            else
            {
                var curEnd = DateTime.UtcNow;
                var curStart = curEnd.AddDays(-30);
                cur = (curStart, curEnd);
                prev = (curStart.AddDays(-30), curStart.AddSeconds(-1));
            }

            var (usd, eur) = await GetRatesAsync();

            async Task<decimal> SumExpenseAsync(DateTime s, DateTime e)
            {
                var q = _orderRepository.QueryActiveInRange(s, e);

                if (companyId.HasValue && companyId.Value != Guid.Empty)
                {
                    q = q.Where(o => o.CompanyId == companyId.Value
                                  || (o.Customer != null && o.Customer.CompanyId == companyId.Value));
                }

                // TOPLAM GİDER: PurchasePrice * Quantity * (kur)
                return await q.AsNoTracking()
                    .SelectMany(o => o.OrderDetails
                        .Where(od => od.Status != Status.Deleted))
                    .SumAsync(od =>
                        (od.PurchasePrice * od.Quantity) *
                        (costInOrderCurrency
                            ? (od.Order.CurrencyType == CurrencyType.USD ? usd
                               : od.Order.CurrencyType == CurrencyType.EUR ? eur : 1m)
                            : 1m)
                    );
            }

            var current = await SumExpenseAsync(cur.start, cur.end);
            var previous = await SumExpenseAsync(prev.start, prev.end);

            var changePct = previous == 0m
                ? (current > 0m ? 100m : 0m)
                : ((current - previous) / previous) * 100m;

            return new SalesProfitDTO
            {
                Metric = "Expenses",
                Start = cur.start,
                End = cur.end,
                CompanyId = companyId,
                TotalTL = current,
                CurrentTL = current,
                PreviousTL = previous,
                ChangePct = changePct
            };
        }


        public async Task<decimal> GetTotalProfitAsync() //tüm aktif (silinmemiş) siparişlerden "sadece kar eden satırların karını" topla
        {
            var orders = await _orderRepository.GetAllWithDetailsAsync(); //Tüm siparişleri, ilişkili sipariş detayları ve ürün bilgileriyle birlikte getirir.

            var validDetails = orders
                .Where(o => o.IsActive) //aktif ve silinmemiş siparişler
                .SelectMany(o => o.OrderDetails)
                .Where(od => od.Product.Company.Status != Status.Deleted) //silmemiş şirketler
                .Where(od => (od.TotalPrice ?? (od.UnitPrice * od.Quantity)) > (od.PurchasePrice * od.Quantity)); // ✔️ Zarar edenleri filtrele

            var income = validDetails.Sum(od => od.TotalPrice ?? (od.UnitPrice * od.Quantity));//toplam gelir
            var expenses = validDetails.Sum(od => od.PurchasePrice * od.Quantity); //toplam gider

            return income - expenses; //fark (kar)
        }

        // Yıllık toplam KAR (TL)
        public async Task<SalesProfitDTO> GetProfitMetricAsync(int year, Guid? companyId = null, bool costInOrderCurrency = true)
        {
            var (usd, eur) = await GetRatesAsync();

            var curStart = new DateTime(year, 1, 1);
            var curEnd = new DateTime(year, 12, 31, 23, 59, 59);
            var prevStart = curStart.AddYears(-1);
            var prevEnd = curEnd.AddYears(-1);

            async Task<decimal> SumProfitRangeAsync(DateTime s, DateTime e)
            {
                var q = _orderRepository.QueryActiveInRange(s, e);

                if (companyId.HasValue && companyId.Value != Guid.Empty)
                    q = q.Where(o => o.CompanyId == companyId.Value
                                  || (o.Customer != null && o.Customer.CompanyId == companyId.Value));

                // Tek SQL; LINQ içinde GetRate çağırmıyoruz (EF çeviremez), onun yerine ternary kullanıyoruz
                return await q.AsNoTracking()
                    .SelectMany(o => o.OrderDetails
                        .Where(od => od.Status != Status.Deleted )
                        .Select(od => new
                        {
                            o.CurrencyType,
                            od.Quantity,
                            od.UnitPrice,
                            od.Discount,
                            od.TotalPrice,
                            od.PurchasePrice
                        }))
                    .SumAsync(x =>
                        // Satış TL (indirimli)
                        ((x.TotalPrice ?? (x.UnitPrice * x.Quantity * (x.Discount > 1m ? (1m - (x.Discount / 100m)) : (1m - x.Discount))))
                            * (x.CurrencyType == CurrencyType.USD ? usd
                               : x.CurrencyType == CurrencyType.EUR ? eur : 1m))
                        -
                        // Maliyet TL
                        ((x.PurchasePrice * x.Quantity) *
                         (costInOrderCurrency
                             ? (x.CurrencyType == CurrencyType.USD ? usd
                               : x.CurrencyType == CurrencyType.EUR ? eur : 1m)
                             : 1m))
                    );
            }

            var current = await SumProfitRangeAsync(curStart, curEnd);
            var previous = await SumProfitRangeAsync(prevStart, prevEnd);

            var changePct = previous == 0m
                ? (current > 0m ? 100m : 0m)
                : ((current - previous) / previous) * 100m;

            return new SalesProfitDTO
            {
                Metric = "Profit",
                Start = curStart,
                End = curEnd,
                CompanyId = companyId,
                TotalTL = current,
                CurrentTL = current,
                PreviousTL = previous,
                ChangePct = changePct
            };
        }

        //Toplam satış (ciro) tutarını hesaplar.
        public async Task<SalesProfitDTO> GetSalesMetricAsync(int? year = null, Guid? companyId = null)
        {
            // Dönemleri hazırla
            (DateTime start, DateTime end) cur, prev;

            if (year.HasValue)
            {
                var s = new DateTime(year.Value, 1, 1);
                var e = new DateTime(year.Value, 12, 31, 23, 59, 59);
                cur = (s, e);
                prev = (s.AddYears(-1), e.AddYears(-1));
            }
            else
            {
                var curEnd = DateTime.UtcNow;
                var curStart = curEnd.AddDays(-30);
                cur = (curStart, curEnd);
                prev = (curStart.AddDays(-30), curStart.AddSeconds(-1));
            }

            var (usd, eur) = await GetRatesAsync();

            // Tek SQL'de toplama yapan yardımcı
            async Task<decimal> SumSalesRangeAsync(DateTime s, DateTime e)
            {
                var q = _orderRepository.QueryActiveInRange(s, e);

                if (companyId.HasValue && companyId.Value != Guid.Empty)
                    q = q.Where(o => o.CompanyId == companyId.Value
                                  || (o.Customer != null && o.Customer.CompanyId == companyId.Value));

                return await q.AsNoTracking()
                    .SelectMany(o => o.OrderDetails
                        .Where(od => od.Status != Status.Deleted ))
                    .SumAsync(od =>
                        ((od.TotalPrice ?? (od.UnitPrice * od.Quantity)) *
                         (od.Order.CurrencyType == CurrencyType.USD ? usd
                          : od.Order.CurrencyType == CurrencyType.EUR ? eur : 1m))
                    );
            }

            var current = await SumSalesRangeAsync(cur.start, cur.end);
            var previous = await SumSalesRangeAsync(prev.start, prev.end);

            var changePct = previous == 0m
                ? (current > 0m ? 100m : 0m)
                : ((current - previous) / previous) * 100m;

            return new SalesProfitDTO
            {
                Metric = "Sales",
                Start = cur.start,
                End = cur.end,
                CompanyId = companyId,
                TotalTL = current,       // toplam = mevcut dönem toplamı
                CurrentTL = current,
                PreviousTL = previous,
                ChangePct = changePct
            };
        }


        //Güncel kur değerlerini çevir
        private async Task<(decimal usd, decimal eur)> GetRatesAsync()
        {
            var latest = await _currencyService.GetLatestCurrencyRateAsync();

            // Varsayılan 1 => kur bulunamazsa TRY gibi davranır (sonuç TL’ye çevrilmiş olur)
            decimal usd = 1m, eur = 1m;

            if (latest.IsSuccess && latest.Data != null)
            {
                if (latest.Data.DollarRate > 0) usd = latest.Data.DollarRate;
                if (latest.Data.EuroRate > 0) eur = latest.Data.EuroRate;
            }

            return (usd, eur);
        }

    }
}
