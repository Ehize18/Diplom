using Microsoft.EntityFrameworkCore;
using Shared.RabbitMQ.Contracts;
using ShopService.Database;

namespace ShopService.Application.Services
{
	public class DbMigrator
	{
		private readonly ShopDbContext _context;
		public DbMigrator(ShopDbContext context)
		{
			_context = context;
		}

		public async Task<ShopCreated> Migrate(Guid shopId)
		{
			try
			{
				if ((await _context.Database.GetPendingMigrationsAsync()).Any())
				{
					await _context.Database.MigrateAsync();
					return new ShopCreated
					{
						AlreadyMigrated = false,
						IsSuccess = true,
						ShopId = shopId
					};
				}
				else
				{
					return new ShopCreated
					{
						IsSuccess = true,
						AlreadyMigrated = true,
						ShopId = shopId
					};
				}
			}
			catch (Exception ex)
			{
				return new ShopCreated
				{
					IsSuccess = false,
					ShopId = shopId,
					AlreadyMigrated = false
				};
			}
		}
	}
}
