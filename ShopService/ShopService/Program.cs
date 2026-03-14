
using Microsoft.EntityFrameworkCore;
using ShopService.Database;

namespace ShopService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var config = builder.Configuration;

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
			builder.Services.AddOpenApi();
			builder.Services.AddSwaggerGen();

			builder.Services.ConfigureServices(config);

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.MapOpenApi();
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
