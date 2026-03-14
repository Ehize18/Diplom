using AdministrativeService.Application.Services;
using AdministrativeService.Core.Entities;
using AdministrativeService.Database;
using AdministrativeService.Database.Interfaces;
using AdministrativeService.Database.Repositories;
using AdministrativeService.HostedServices;
using Microsoft.EntityFrameworkCore;
using Shared.RabbitMQ;

namespace AdministrativeService
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services
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
			services.AddSingleton<RabbitMQService>();
			services.AddHostedService<RabbitHostedService>();
			return services;
		}

		private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString(nameof(AdminDbContext));
			services.AddDbContext<AdminDbContext>(options =>
			{
				options.UseNpgsql(connectionString);
			});

			services.AddScoped<IBaseRepository<Shop>, ShopRepository>();

			return services;
		}

		private static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddScoped<ShopService>();

			return services;
		}
	}
}
