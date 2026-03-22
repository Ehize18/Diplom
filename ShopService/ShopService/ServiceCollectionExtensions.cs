using Microsoft.EntityFrameworkCore;
using Shared.RabbitMQ;
using ShopService.Application.Services;
using ShopService.Core;
using ShopService.Core.Entities;
using ShopService.Database;
using ShopService.Database.Interfaces;
using ShopService.Database.Repositories;
using ShopService.HostedServices;

namespace ShopService
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddHttpContextAccessor()
				.AddRabbitMQ(configuration)
				.AddDatabase(configuration)
				.AddServices();
			return services;
		}

		private static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<RabbitMQOptions>(configuration.GetSection(nameof(RabbitMQOptions)));
			services.AddSingleton<RabbitMQConsumer>();
			services.AddSingleton<RabbitMQPublisher>();
			services.AddSingleton<MessageService>();
			services.AddHostedService<RabbitHostedService>();
			return services;
		}

		private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<ConnectionStringProvider>(_ =>
			{
				var connectionStringTemplate = configuration.GetValue<string>("DBConnectionStringTemplate")!;
				return new ConnectionStringProvider(connectionStringTemplate);
			});

			services.AddDbContext<ShopDbContext>();

			services.AddTransient<DbMigrator>();
			services.AddScoped<IBaseRepository<GoodCategory>, CategoryRepository>();
			services.AddScoped<IBaseRepository<Good>, GoodRepository>();

			return services;
		}

		private static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddScoped<CategoryService>();
			services.AddScoped<GoodService>();
			services.AddScoped<DataService>();

			return services;
		}
	}
}
