using BaSalesManagementApp.Business.Interfaces;
using BaSalesManagementApp.Business.Services;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaSalesManagementApp.BackgroundJobs.Extentions
{
	public static class HangfireExtensions
	{
		public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("HangfireConnectionString");
			services.AddHangfire(config =>
			{
				config.UseSqlServerStorage(connectionString);
			});

			services.AddHangfireServer();

			// Tekrarlanan işlerin planlanmasını sağlar.
			using (var serviceProvider = services.BuildServiceProvider())
			{
				var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();

				// Her gün saat 12:00'de çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
				"DailyJobAtNoon-notify-promotion-updates",
				service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
				Cron.Daily());

				// Her dakika çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
				"MinutelyJob-notify-promotion-updates",
				service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
				Cron.Minutely());

				// Her saat çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
			  "HourlyJob-notify-1-promotion-updates",
			  service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
			  Cron.Hourly());

				// Her hafta Pazar günü saat 12:00'de çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
			 "WeeklyJob-notify-promotion-updates",
			 service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
			 Cron.Weekly());

				// Her ayın 1'inde saat 12:00'de çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
			"MonthlyJob-notify-promotion-updates",
			service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
			Cron.Monthly());


				// Her gün saat 14:30'da çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
			"DailyJobAt2:30PM-notify-promotion-updates",
			 service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
			"30 14 * * *");

				// Her ayın 1'inde saat 10:00'da çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
		   "MonthlyJobAt10AM-notify-promotion-updates",
			service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
			 "0 10 1 * *");

				// Her Pazartesi saat 8:00'de çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
			"WeeklyJobAt8AM-notify-promotion-updates",
			 service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
			  "0 8 * * 1");

				// Her gün saat 22:30'da çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
			"DailyJobAt10:30PM-notify-1-promotion-updates",
			service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
			"30 22 * * *");

				// Haftanın her günü saat 15:00'te çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
			  "DailyJobAt3PM-notify-promotion-updates",
			  service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
			 "0 15 * * *");

				// Haftanın her günü saat 10:00'da çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
			 "DailyJobAt10AM-notify-promotion-updates",
			  service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
			 "0 10 * * *");

				// Haftanın her günü saat 22:00'de çalışacak job
				RecurringJob.AddOrUpdate<IPromotionService>(
		  "DailyJobAt10:30PM-notify-2-promotion-updates",
		   service => service.NotifyPromotionUpdatesWithJobId(Guid.NewGuid().ToString()),
		   "30 22 * * *");


				RecurringJob.AddOrUpdate<ICurrencyService>(
	"DailyJob-GetCurrenciesAt1510",
	service => service.GetAllCurrencies(),
	Cron.Daily(15, 30) // Her gün saat 15:10
);
				RecurringJob.AddOrUpdate<ICurrencyService>(
					"DailyJob-DeleteCurrenciesAt1515",
					service => service.DeleteLastCurrency(),
					Cron.Daily(15, 35) // Her gün saat 15:15
				);



				// Her Pazartesi saat 8:00'de çalışacak job product için 
				// RecurringJob.AddOrUpdate<IProductService>("WeeklyJobAt8AM-Product-updates", service => service.UpdateAllProductsAsync(), "0 8 * * 1");
				RecurringJob.AddOrUpdate<IProductService>(
	   "MinutelyJob-Product-updates",
	   service => service.UpdateAllProductsAsync(),
	   "*/2 * * * *"
   );

			}

			return services;
		}

		public static async Task UseHangfireDashboardWithPath(this IApplicationBuilder app, string pathMatch = "/Hangfire")
		{
			app.UseHangfireDashboard(pathMatch);

			// Queue yapısı bu şekilde entegre edilmiştir. Attribute olarak kullanılması yeterli olacaktır.
			var options = new BackgroundJobServerOptions
			{
				Queues = new[] { "alpha", "beta", "default" }
			};

			app.UseHangfireServer(options);
		}
	}
}
