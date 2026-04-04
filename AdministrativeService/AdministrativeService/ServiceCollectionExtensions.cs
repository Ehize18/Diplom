using System.Text;
using AdministrativeService.Application.Interfaces;
using AdministrativeService.Application.Services;
using AdministrativeService.Core.Entities;
using AdministrativeService.Database;
using AdministrativeService.Database.Interfaces;
using AdministrativeService.Database.Repositories;
using AdministrativeService.HostedServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Shared.Jwt;
using Shared.RabbitMQ;
using Shared.S3;

namespace AdministrativeService
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddRabbitMQ(configuration)
				.AddDatabase(configuration)
				.AddServices()
				.AddJwtAuthentication(configuration.GetSection("Jwt"))
				.AddS3Service(configuration)
				.AddApplicationCors();
			return services;
		}

		private static IServiceCollection AddS3Service(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddMinio(minio =>
				{
					minio.WithEndpoint(configuration["S3:Endpoint"])
						.WithCredentials(
							configuration["S3:AccessKey"],
							configuration["S3:SecretKey"]
						)
						.WithSSL(false)
						.Build();
				})
				.AddScoped<IMinioService, MinioService>();

			return services;
		}

		private static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<RabbitMQOptions>(configuration.GetSection(nameof(RabbitMQOptions)));
			services.AddSingleton<RabbitMQConsumer>();
			services.AddSingleton<RabbitMQPublisher>();
			services.AddSingleton<MessageService>();
			services.AddHostedService<MessageHostedService>();
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
			services.AddScoped<IBaseRepository<User>, UserRepository>();

			return services;
		}

		private static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddScoped<ShopService>();
			services.AddScoped<ShopContentService>();
			services.AddScoped<IAuthService, AuthService>();

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
							if (string.IsNullOrWhiteSpace(context.Token))
							{
								context.Token = context.Request.Cookies["AdminToken"];
							}
							return Task.CompletedTask;
						}
					};
				});

			return services;
		}

		static IServiceCollection AddApplicationCors(this IServiceCollection services)
		{
			services.AddCors(
				o => o.AddPolicy("DevPolicy",
					builder => builder
						.WithOrigins("http://localhost:4200", "http://localhost:8080", "https://admin.ehize.ru")
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials()));
			return services;
		}
	}
}
