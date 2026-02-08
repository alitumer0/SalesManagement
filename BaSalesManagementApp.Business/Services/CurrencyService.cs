using BaSalesManagementApp.DataAccess.Context;
using BaSalesManagementApp.Dtos.CurrencyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BaSalesManagementApp.Business.Services
{
    public  class CurrencyService:ICurrencyService
    {
        private readonly BaSalesManagementAppDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly ICurrentExchangeRateRepository _exchangeRateRepository;

        public CurrencyService(BaSalesManagementAppDbContext dbContext, HttpClient httpClient, ICurrentExchangeRateRepository exchangeRateRepository)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
            _exchangeRateRepository = exchangeRateRepository;
        }

		public async Task<IResult> DeleteLastCurrency()
		{
			try
			{
                // 31 günden eski verileri al
                var oldRates = await _exchangeRateRepository
                    .GetAllAsync(rate => rate.CreatedDate <= DateTime.Now.AddDays(-31));

				if (!oldRates.Any())
				{
					return new ErrorResult("31 günden eski döviz kaydı bulunamadı.");
				}


				// Her bir eski veriyi ID ile sil
				foreach (var rate in oldRates)
				{
					 await _exchangeRateRepository.DeleteAsync(rate); // ID ile silme işlemi
				}

				await _exchangeRateRepository.SaveChangeAsync(); // Değişiklikleri kaydet

				return new SuccessResult("31 günden eski tüm döviz kayıtları başarıyla silindi.");
			}
			catch (Exception ex)
			{
				return new ErrorResult("Döviz kayıtları silinirken bir hata oluştu: " + ex.Message);
			}
		}


		/// <summary>
		/// Döviz birimlerinin ilgili internet sitesinden xml formatı ile alınıp işlenilmesi ve veritabanına kaydedilmesi sağlanır.
		/// </summary>
		/// <returns></returns>

		public async Task GetAllCurrencies()
        {
            string url = "http://www.tcmb.gov.tr/kurlar/today.xml";
            try
            {
                //var response = await _httpClient.GetAsync(url.ToString());
                // XML verisini çek
                XmlDocument doc = new XmlDocument();
                doc.Load(url);

                // Dolar ve Euro bilgilerini al
                var dollarNode = doc.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']");
                var euroNode = doc.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']");

                //ForexSelling değerlerini string olarak al
                var dollarText = dollarNode?["ForexSelling"]?.InnerText;
                var euroText = euroNode?["ForexSelling"]?.InnerText;

                decimal dollarRate =0;
                decimal euroRate =0;

                //verinin geçerli olup olmadığını kontrol et
                bool isValid = 
                    !string.IsNullOrEmpty(dollarText) && //Boş veya null olmamalı
                    !string.IsNullOrEmpty(euroText) && //Boş veya null olmamalı
                    decimal.TryParse(dollarText.Replace(".",","),out dollarRate) && // Sayıya çevrilebilmeli
                    decimal.TryParse(euroText.Replace(".", ","), out euroRate) && // Sayıya çevrilebilmeli
                    dollarRate > 0 && euroRate > 0; // 0 veya negatif olmamalı

                //Eğer veri geçerli değilse
                if (!isValid)
                {
                    //veritabanındaki en son eklenen döviz kurlarını al
                    var latestRate = await GetLatestCurrencyRateAsync();

                    if(latestRate==null)
                    {
                        throw new Exception("Geçerli döviz kuru verisi bulunamadı.");
                    }
                    //En son eklenen döviz kurlarını kullan
                    dollarRate = latestRate.Data.DollarRate;
                    euroRate = latestRate.Data.EuroRate;


                }

                // Veritabanına kaydet
                var newRate = new CurrentExchangeRate
                {
                        
                        DollarRate = dollarRate,
                        EuroRate = euroRate,
                        CreatedDate = DateTime.Now,
                        Status = Status.Added,
                };
                await _dbContext.CurrentExchangeRates.AddAsync(newRate);
                await _dbContext.SaveChangesAsync();
                
               
                
                    
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Veritabanındaki döviz kurlarını getirir ve DTO'ya çevrip döndürür.
        /// Döviz kurları en güncel tarihten itibaren sıralanmış şekilde döner.
        /// </summary>
        /// <returns>
        /// Döviz kurlarının listesini içeren DTO (CurrentExchangeRateListDTO) türünde bir <see cref="IDataResult{T}"/> sonucu.
        /// </returns>
        public async Task<IDataResult<List<CurrentExchangeRateListDTO>>> GetAllCurrentExchangeRatesAsync()
        {
            try
            {
                // Repository methodu alınıyor
                var exchangeRates = await _exchangeRateRepository.GetAllExchangeRatesAsync();

                //Boş veya 0 olan değerleri filtrele
                var filteredRates = exchangeRates
                    .Where(r => r.DollarRate > 0 && r.EuroRate > 0)
                    .OrderByDescending(r => r.CreatedDate) // En güncel tarihten başlayarak sıralıyoruz
                    .ToList();

                // Mapster
                var exchangeRateDTOs = filteredRates.Adapt<List<CurrentExchangeRateListDTO>>();

                return new SuccessDataResult<List<CurrentExchangeRateListDTO>>(exchangeRateDTOs, Messages.CURRENCY_LISTED_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<CurrentExchangeRateListDTO>>(Messages.CURRENCY_LIST_ERROR);
            }
        }


        /// <summary>
        /// Veritabanındaki en güncel döviz kuru bilgisini getirir.
        /// </summary>
        /// <returns>
        /// En güncel döviz kurunu CurrentExchangeRateDTO tipinde geri döndürür.
        /// </returns>
        public async Task<IDataResult<CurrentExchangeRateDTO>> GetLatestCurrencyRateAsync()
        {
            try
            {
                var allRatesResult = await GetAllCurrentExchangeRatesAsync();
                if (!allRatesResult.IsSuccess || allRatesResult.Data == null || !allRatesResult.Data.Any())
                {
                    return new ErrorDataResult<CurrentExchangeRateDTO>(Messages.CURRENCY_NOT_FOUND);
                }

                // Tarihe göre sıralayıp en yeni kaydı alıyoruz
                var latest = allRatesResult.Data
                              .OrderByDescending(r => r.CreatedDate)
                              .FirstOrDefault();

                CurrentExchangeRateDTO currentExchangeRateDTO = new CurrentExchangeRateDTO
                {
                    DollarRate = latest.DollarRate,
                    EuroRate = latest.EuroRate,
                };

                return new SuccessDataResult<CurrentExchangeRateDTO>(currentExchangeRateDTO, Messages.CURRENCY_RETRIEVED_SUCCESS);
            }
            catch (Exception)
            {
                return new ErrorDataResult<CurrentExchangeRateDTO>(Messages.CURRENCY_RETRIEVED_ERROR);
            }
        }


    }
}
