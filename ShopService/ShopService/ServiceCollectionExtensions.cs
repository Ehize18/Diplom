using Microsoft.EntityFrameworkCore;
using Shared.RabbitMQ;
using ShopService.Application.Services;
using ShopService.Database;
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
				.AddDatabase(configuration);
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
			services.AddDbContextPool<ShopDbContext>((serviceCollection, options) =>
			{
				var httpContextAccessor = serviceCollection.GetRequiredService<IHttpContextAccessor>();
				var httpContext = httpContextAccessor.HttpContext;

				var shopId = httpContext?.Request.Headers["ShopId"].ToString();

				if (!string.IsNullOrWhiteSpace(shopId))
				{
					var connectionStringTemplate = configuration.GetValue<string>("DBConnectionStringTemplate")!;
					var connectionString = string.Format(connectionStringTemplate, shopId);
					options.UseNpgsql(connectionString);
				}
				else
				{
					//options.UseNpgsql("Server=db;Port=5432;Database=test;User Id=postgres;password=123", );
					options.UseNpgsql(options => options.MigrationsAssembly("ShopService.Database"));
				}
			});

			services.AddTransient<DbMigrator>();

			return services;
		}
	}
}
