using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Jwt;
using Shared.RabbitMQ;
using ShopService.Application.Services;
using ShopService.Core;
using ShopService.Core.Entities;
using ShopService.Database;
using ShopService.Database.Interfaces;
using ShopService.Database.Repositories;
using ShopService.HostedServices;

using ShopService.Options;

namespace ShopService
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddHttpContextAccessor()
				.AddJwtAuthentication(configuration.GetSection("Jwt"))
				.AddRabbitMQ(configuration)
				.AddDatabase(configuration)
				.AddServices()
				.AddApplicationCors();

			services.Configure<VkOptions>(configuration.GetSection("Vk"));

			return services;
		}

		private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration jwtConfig)
		{
			var securityKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(jwtConfig.GetValue<string>("SecretKey")!));

			services.Configure<JwtOptions>(jwtConfig);
			services.AddScoped<IJwtProvider, JwtProvider>();

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
				{
					options.TokenValidationParameters = new()
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = securityKey
					};
					options.Events = new JwtBearerEvents()
					{
						OnMessageReceived = context =>
						{
							var shopIdHeader = context.Request.Headers["X-Shop-Id"];
							if (Guid.TryParse(shopIdHeader, out var shopId))
							{
								context.Token = context.Request.Cookies[shopIdHeader + "Token"];
							}
							return Task.CompletedTask;
						}
					};
				});

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
			services.AddScoped<ConnectionStringProvider>(serviceProvider =>
			{
				var connectionStringTemplate = configuration.GetValue<string>("DBConnectionStringTemplate")!;
				var connectionStringProvider = new ConnectionStringProvider(connectionStringTemplate);
				var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
				var httpContext = httpContextAccessor.HttpContext;
				if (httpContext != null)
				{
					if (httpContext.Request.Headers.TryGetValue("X-Shop-Id", out var shopId))
					{
						if (Guid.TryParse(shopId, out var shopIdGuid))
						{
							connectionStringProvider.ShopId = shopIdGuid;
						}
					}
				}

				return connectionStringProvider;
			});

			services.AddDbContext<ShopDbContext>();

			services.AddTransient<DbMigrator>();
			services.AddScoped<IBaseRepository<GoodCategory>, CategoryRepository>();
			services.AddScoped<IGoodRepository, GoodRepository>();
			services.AddScoped<IBaseRepository<Good>, GoodRepository>();
			services.AddScoped<IBaseRepository<User>, UserRepository>();
			services.AddScoped<IBasketRepository, BasketRepository>();
			services.AddScoped<ISearchRepository, CategoryOrGoodSearchRepository>();
			services.AddScoped<IGoodPropertyRepository, GoodPropertyRepository>();
			services.AddScoped<IBaseRepository<GoodPropertyCategory>, GoodPropertyRepository>();
			services.AddScoped<IBaseRepository<PaymentMethod>, PaymentMethodRepository>();
			services.AddScoped<IBaseRepository<DeliveryMethod>, DeliveryMethodRepository>();
			services.AddScoped<IOrderRepository, OrderRepository>();
			services.AddScoped<IBaseRepository<Order>, OrderRepository>();

			return services;
		}

		private static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddScoped<CategoryService>();
			services.AddScoped<GoodService>();
			services.AddScoped<DataService>();
			services.AddScoped<UserService>();
			services.AddScoped<BasketService>();
			services.AddScoped<PropertyService>();
			services.AddScoped<MethodsService>();
			services.AddScoped<OrderService>();

			return services;
		}

		static IServiceCollection AddApplicationCors(this IServiceCollection services)
		{
			services.AddCors(
				o => o.AddPolicy("DevPolicy",
					builder => builder
						.WithOrigins("http://localhost:4200", "http://localhost:8080", "http://localhost:4300", "http://192.168.1.52:4300", "https://localhost.localdomain", "https://ehize.ru")
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials()));
			return services;
		}
	}
}
