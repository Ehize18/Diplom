using Microsoft.EntityFrameworkCore;
using Shared.RabbitMQ.Contracts;
using ShopService.Database;

namespace ShopService.Application.Services
{
	public class DbMigrator
	{
		public async Task<MigrateDbResponse> Migrate(Guid shopId, string connectionStringTemplate)
		{
			var connectionString = string.Format(connectionStringTemplate, shopId);
			using (var dbContext = new ShopDbContext(connectionString))
			{
				try
				{
					if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
					{
						await dbContext.Database.MigrateAsync();
						return new MigrateDbResponse
						{
							AlreadyMigrated = false,
							IsSuccess = true,
							ShopId = shopId
						};
					}
					else
					{
						return new MigrateDbResponse
						{
							IsSuccess = true,
							AlreadyMigrated = true,
							ShopId = shopId
						};
					}
				}
				catch (Exception ex)
				{
					return new MigrateDbResponse
					{
						IsSuccess = false,
						ShopId = shopId,
						AlreadyMigrated = false
					};
				}
			};
		}
	}
}
