
using AdministrativeService.Database;
using Microsoft.EntityFrameworkCore;

namespace AdministrativeService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
			builder.Services.AddOpenApi();
			builder.Services.AddSwaggerGen();

			builder.Services.ConfigureServices(builder.Configuration);

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.MapOpenApi();
				app.UseSwagger();
				app.UseSwaggerUI();
				app.UseCors("DevPolicy");
			}

			//app.UseHttpsRedirection();

			app.UseAuthorization();

			var serviceProvider = app.Services;
			
			using (var scope = serviceProvider.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<AdminDbContext>();

				if (dbContext.Database.GetPendingMigrations().Any())
				{
					dbContext.Database.Migrate();
				}
			}


			app.MapControllers();

			app.Run();
		}
	}
}
