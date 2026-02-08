using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.OrderDTOs;
using BaSalesManagementApp.Dtos.ProductTypeDtos;
using BaSalesManagementApp.Dtos.StockDTOs;

namespace BaSalesManagementApp.Business.Services
{
    /// <summary>
    /// StockService sınıfı, stoklarla ilgili CRUD işlemlerini gerçekleştirir.
    /// </summary>
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IEmployeeService _employeeService;
        public StockService(IStockRepository stockRepository, IProductRepository productRepository, IOrderRepository orderRepository, IWarehouseRepository warehouseRepository, IEmployeeService employeeService)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _warehouseRepository = warehouseRepository;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Yeni bir stok oluşturur.
        /// </summary>
        /// <param name="stockCreateDTO">Oluşturulacak stok bilgileri</param>
        /// <returns>Stok oluşturma işlemi sonucu</returns>
        public async Task<IDataResult<StockDTO>> AddAsync(StockCreateDTO stockCreateDTO)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(stockCreateDTO.ProductId);
                if (product == null)
                {
                    return new ErrorDataResult<StockDTO>(Messages.PRODUCT_LISTED_EMPTY);
                }

                // Depo kontrolü
                var warehouse = await _warehouseRepository.GetByIdAsync(stockCreateDTO.WarehouseId);
                if (warehouse == null)
                {
                    return new ErrorDataResult<StockDTO>("Depo bulunamadı."); // Hata mesajı
                }

                // Aynı depoda aynı ürünün olup olmadığını kontrol et
                var existingStock = await _stockRepository.GetStockByWarehouseAndProductAsync(
                    stockCreateDTO.WarehouseId, stockCreateDTO.ProductId);

                if (existingStock != null)
                {
                    // Eğer mevcutsa adetleri birleştir
                    existingStock.Count += stockCreateDTO.Count;
                    existingStock.ModifiedDate = DateTime.UtcNow;

                    await _stockRepository.UpdateAsync(existingStock);
                    await _stockRepository.SaveChangeAsync();

                    // Dönüş DTO'su
                    var updatedStockDTO = existingStock.Adapt<StockDTO>();
                    updatedStockDTO.ProductName = product.Name; // Ürün ismi atanıyor
                    updatedStockDTO.WarehouseName = warehouse.Name;

                    return new SuccessDataResult<StockDTO>(updatedStockDTO, Messages.BRANCH_ADD_SUCCESS);
                }
                else
                {
                    // Eğer mevcut değilse yeni stok oluştur
                    var stock = stockCreateDTO.Adapt<Stock>();
                    stock.CreatedDate = DateTime.UtcNow;

                    await _stockRepository.AddAsync(stock);
                    await _stockRepository.SaveChangeAsync();

                    // Dönüş DTO'su
                    var stockDTO = stock.Adapt<StockDTO>();
                    stockDTO.ProductName = product.Name; // Ürün ismi atanıyor
                    stockDTO.WarehouseName = warehouse.Name;

                    return new SuccessDataResult<StockDTO>(stockDTO, Messages.STOCK_CREATED_SUCCESS);
                }
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<StockDTO>(stockCreateDTO.Adapt<StockDTO>(), Messages.STOCK_CREATE_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Belirtilen ID'li stoğu siler.
        /// </summary>
        /// <param name="stockId">Silinecek stok ID'si</param>
        /// <returns>Stok silme işlemi sonucu</returns>
        public async Task<IResult> DeleteAsync(Guid stockId)
        {
            try
            {
                var stock = await _stockRepository.GetByIdAsync(stockId);
                if (stock == null)
                {
                    return new ErrorResult(Messages.STOCK_NOT_FOUND);
                }
                await _stockRepository.DeleteAsync(stock);
                await _stockRepository.SaveChangeAsync();
                return new SuccessResult(Messages.STOCK_DELETED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new SuccessResult(Messages.STOCK_DELETE_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Tüm stokları getirir.
        /// </summary>
        /// <returns>Tüm stok listesi</returns>
        public async Task<IDataResult<List<StockListDTO>>> GetAllAsync(string sortOrder)
        {
            var stocks = await _stockRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync();
            var warehouses = await _warehouseRepository.GetAllAsync();

            var stockList = stocks.Select(stock =>
            {
                var product = products.FirstOrDefault(p => p.Id == stock.ProductId);
                var warehouse = warehouses.FirstOrDefault(w => w.Id == stock.WarehouseId);

                return new StockListDTO
                {
                    Id = stock.Id,
                    Count = stock.Count,
                    ProductName = product?.Name,
                    WarehouseName = warehouse?.Name, // Depo adı null olabilir
                    CreatedDate = stock.CreatedDate,
                    ModifiedDate = stock.ModifiedDate
                };
            }).ToList();

            return new SuccessDataResult<List<StockListDTO>>(stockList, Messages.STOCK_LISTED_SUCCESS);
        }

        public async Task<IDataResult<List<StockListDTO>>> GetAllAsync()
        {
            try
            {
                var stocks = await _stockRepository.GetAllAsync();
                var products = await _productRepository.GetAllAsync();
                var stockList = stocks.Adapt<List<StockListDTO>>();

                if (stockList == null || stockList.Count == 0)
                {
                    return new ErrorDataResult<List<StockListDTO>>(stockList, Messages.STOCK_LIST_EMPTY);
                }


                foreach (var stockDTO in stockList)
                {
                    var product = products.FirstOrDefault(p => p.Id == stockDTO.ProductId);
                    stockDTO.ProductName = product?.Name;
                }

                return new SuccessDataResult<List<StockListDTO>>(stockList, Messages.STOCK_LISTED_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<StockListDTO>>(new List<StockListDTO>(), Messages.STOCK_LIST_FAILED);
            }
        }


        /// <summary>
        /// Belirtilen ID'li stok getirilir.
        /// </summary>
        /// <param name="stockId">Getirilecek stok ID'si</param>
        /// <returns>Belirtilen ID'li stok verileri</returns>
        public async Task<IDataResult<StockDTO>> GetByIdAsync(Guid stockId)
        {
            try
            {
                var stock = await _stockRepository.GetByIdAsync(stockId);
                if (stock == null)
                {
                    return new ErrorDataResult<StockDTO>(Messages.STOCK_NOT_FOUND);
                }
                //ProductName ekle
                var stockDTO = stock.Adapt<StockDTO>();
                var product = await _productRepository.GetByIdAsync(stock.ProductId);
                if (product != null)
                {
                    stockDTO.ProductName = product.Name;
                }
                return new SuccessDataResult<StockDTO>(stockDTO, Messages.STOCK_FOUND_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<StockDTO>(Messages.STOCK_GET_FAILED + ex.Message);
            }
        }

        /// <summary>
        ///  Stok bilgilerini günceller.
        /// </summary>
        /// <param name="stockUpdateDTO">Güncellenecek stok bilgileri</param>
        /// <returns></returns>
        public async Task<IDataResult<StockDTO>> UpdateAsync(StockUpdateDTO stockUpdateDTO)
        {
            try
            {
                var stockOnHand = await _stockRepository.GetByIdAsync(stockUpdateDTO.Id);
                if (stockOnHand == null)
                {
                    return new ErrorDataResult<StockDTO>(Messages.STOCK_NOT_FOUND);
                }
                var product = await _productRepository.GetByIdAsync(stockUpdateDTO.ProductId);
                if (product == null)
                {
                    return new ErrorDataResult<StockDTO>(Messages.PRODUCT_LISTED_ERROR);
                }

                stockOnHand = stockUpdateDTO.Adapt(stockOnHand);
                await _stockRepository.UpdateAsync(stockOnHand);
                await _stockRepository.SaveChangeAsync();
                return new SuccessDataResult<StockDTO>(stockOnHand.Adapt<StockDTO>(), Messages.STOCK_UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<StockDTO>(Messages.STOCK_UPDATE_FAILED + ex.Message);
            }
        }



        public async Task<IDataResult<List<StockListDTO>>> GetAllAsync(string sortOrder, string searchQuery)
        {
            try
            {
                // Veritabanındaki tüm stokları getir
                var stocks = await _stockRepository.GetAllAsync();
                // Ürünleri getir
                var products = await _productRepository.GetAllAsync();

                // Stoklar ve ürünler arasında ilişki kur
                var stockList = (from stock in stocks
                                 join product in products on stock.ProductId equals product.Id
                                 select new StockListDTO
                                 {
                                     Id = stock.Id,
                                     CreatedDate = stock.CreatedDate,
                                     ProductId = stock.ProductId,
                                     ProductName = product.Name
                                 }).ToList();

                // Eğer arama sorgusu varsa filtreleme işlemi yap
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    stockList = stockList
                        .Where(s => s.ProductName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Sıralama işlemi
                switch (sortOrder.ToLower())
                {
                    case "alphabetical":
                        stockList = stockList.OrderBy(s => s.ProductName).ToList();
                        break;
                    case "reverse":
                        stockList = stockList.OrderByDescending(s => s.ProductName).ToList();
                        break;
                    default:
                        // Varsayılan sıralama (Alphabetical by ProductName)
                        stockList = stockList.OrderBy(s => s.ProductName).ToList();
                        break;
                }

                // Eğer liste boşsa hata döndür
                if (stockList == null || !stockList.Any())
                {
                    return new ErrorDataResult<List<StockListDTO>>(Messages.STOCK_LIST_EMPTY);
                }

                return new SuccessDataResult<List<StockListDTO>>(stockList, Messages.STOCK_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajı döndür
                return new ErrorDataResult<List<StockListDTO>>(
                    new List<StockListDTO>(),
                    $"{Messages.STOCK_LIST_FAILED} - {ex.Message}"
                );
            }
        }

        public async Task<IResult> CheckStockAvailabilityAsync(List<OrderDetailCreateDTO> orderDetails)
        {
            try
            {
                // Tüm stokları getir
                var stocks = await _stockRepository.GetAllAsync();
                foreach (var detail in orderDetails)
                {
                    // İlgili ürünün stok bilgisini bul
                    var stock = stocks.FirstOrDefault(x => x.ProductId == detail.ProductId);
                    if (stock == null || stock.Count < detail.Quantity)
                    {
                        return new ErrorDataResult<StockDTO>(Messages.STOCK_NOT_FOUND);
                    }
                }
                return new SuccessResult(Messages.STOCK_FOUND_SUCCESS);
            }
            catch (Exception ex)
            {
                return new SuccessResult(Messages.STOCK_GET_FAILED + ex.Message);
            }
        }
        public async Task<IResult> UpdateStockAsync(List<OrderDetailCreateDTO> orderDetails)
        {
            try
            {
                var stocks = await _stockRepository.GetAllAsync();
                foreach (var detail in orderDetails)
                {
                    var stock = stocks.FirstOrDefault(x => x.ProductId == detail.ProductId);
                    if (stock != null)
                    {
                        stock.Count -= detail.Quantity;
                        await _stockRepository.UpdateAsync(stock);
                    }
                    else
                    {
                        return new ErrorDataResult<StockDTO>(Messages.STOCK_NOT_FOUND);
                    }
                }
                await _stockRepository.SaveChangeAsync();
                return new SuccessDataResult<StockDTO>(Messages.STOCK_UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<StockDTO>(Messages.STOCK_UPDATE_FAILED + ex.Message);
            }
        }
        public async Task<IResult> CheckStockAvailabilityAsync(List<OrderDetailUpdateDTO> orderDetails, Guid orderId)
        {
            try
            {
                // Mevcut siparişi getir
                var orderResult = await _orderRepository.GetByIdAsync(orderId);
                if (orderResult == null)
                {
                    return new ErrorResult(Messages.ORDER_NOT_FOUND);
                }
                var order = orderResult.Adapt<OrderDTO>(); // Sipariş DTO'suna dönüştürülür
                var stocks = await _stockRepository.GetAllAsync(); // Tüm stokları getir
                foreach (var detail in orderDetails)
                {
                    // Eski siparişten alınan miktar
                    int oldQuantity = order.OrderDetails
                        .Where(x => x.ProductId == detail.ProductId)
                        .Select(x => x.Quantity)
                        .FirstOrDefault();
                    // Stok kontrolü
                    var stock = stocks.FirstOrDefault(x => x.ProductId == detail.ProductId);
                    if (stock == null || detail.Quantity > (stock.Count + oldQuantity))
                    {
                        return new ErrorDataResult<StockDTO>(Messages.STOCK_NOT_FOUND);
                    }
                }
                return new SuccessResult(Messages.STOCK_FOUND_SUCCESS);
            }
            catch (Exception ex)
            {
                return new SuccessResult(Messages.STOCK_GET_FAILED + ex.Message);
            }
        }
        public async Task<IResult> UpdateStockAsync(List<OrderDetailUpdateDTO> orderDetails, Guid orderId)
        {
            try
            {
                // Sipariş bilgilerini getir
                var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
                if (order == null || order.OrderDetails == null || !order.OrderDetails.Any())
                {
                    return new ErrorResult(Messages.ORDER_NOT_FOUND);
                }
                // Eski miktarları bir sözlükte tut
                var oldQuantities = order.OrderDetails
                    .Where(od => orderDetails.Any(x => x.ProductId == od.ProductId))
                    .ToDictionary(od => od.ProductId, od => od.Quantity);
                var stockList = await _stockRepository.GetAllAsync();
                foreach (var detail in orderDetails)
                {
                    var stock = stockList.FirstOrDefault(x => x.ProductId == detail.ProductId);
                    if (stock != null)
                    {
                        // Eski miktarı sözlükten al
                        var oldQuantity = oldQuantities.ContainsKey(detail.ProductId) ? oldQuantities[detail.ProductId] : 0;
                        stock.Count = (stock.Count + oldQuantity) - detail.Quantity;
                        await _stockRepository.UpdateAsync(stock);
                    }
                    else
                    {
                        return new ErrorDataResult<StockDTO>(Messages.STOCK_NOT_FOUND);
                    }
                }
                await _stockRepository.SaveChangeAsync();
                return new SuccessDataResult<StockDTO>(Messages.STOCK_UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new SuccessResult(Messages.STOCK_GET_FAILED + ex.Message);
            }
        }
        public async Task<IResult> DeleteStockAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                var stockList = await _stockRepository.GetAllAsync();
                foreach (var detail in order.OrderDetails)
                {
                    var stock = stockList.FirstOrDefault(x => x.ProductId == detail.ProductId);
                    if (stock != null)
                    {
                        stock.Count += detail.Quantity;
                        await _stockRepository.UpdateAsync(stock);
                    }
                    else
                    {
                        return new ErrorDataResult<StockDTO>(Messages.STOCK_NOT_FOUND);
                    }
                }
                await _stockRepository.SaveChangeAsync();
                return new SuccessDataResult<StockDTO>(Messages.STOCK_UPDATE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new SuccessResult(Messages.STOCK_GET_FAILED + ex.Message);
            }
        }

        public async Task<List<StockListDTO>> GetStockListForManagerAsync(string userId)
        {
            try
            {
                var companyId = await _employeeService.GetCompanyIdByUserIdAsync(userId);
                var productsResult = await _productRepository.GetAllAsync();
                var stocksResult = await _stockRepository.GetAllAsync();
                var warehouseResult = await _warehouseRepository.GetAllAsync();
                if (productsResult != null && stocksResult != null && warehouseResult != null)
                {
                    var products = productsResult.Where(p => p.CompanyId == Guid.Parse(companyId)).ToList();
                    var stocks = stocksResult.Where(s => products.Any(p => p.Id == s.ProductId)).ToList();
                    var warehouses = warehouseResult.Where(w => stocks.Any(s => s.Id == w.Id)).ToList();

                    return stocks.Adapt<List<StockListDTO>>();
                }
                return new List<StockListDTO>();
            }
            catch (Exception ex)
            {
                return new List<StockListDTO> { new StockListDTO { ProductName = ex.Message } };
            }
        }

        /// <summary>
        /// Şirket Id bilgisi alınarak, şirkete ait stokları getirir.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns> Asenkron işlemi temsil eden bir görev. Görev sonucunda stok listesini döndürür. </returns>
        public async Task<IDataResult<List<StockListDTO>>> GetStocksListByCompanyIdAsync(Guid companyId)
        {
            var productsResult = await _productRepository.GetAllAsync();
            var products = productsResult.Where(p => p.CompanyId == companyId).ToList();

            var stocksResult = await _stockRepository.GetAllAsync();
            var stocks = stocksResult.Where(s => products.Any(p => p.Id == s.ProductId)).ToList();

            var stockListDTO = stocks.Adapt<List<StockListDTO>>();

            return new SuccessDataResult<List<StockListDTO>>(stockListDTO, Messages.STOCK_LISTED_SUCCESS);
        }
    }
}
