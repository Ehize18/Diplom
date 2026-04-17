using Microsoft.EntityFrameworkCore.Design;

namespace ShopService.Database
{
	public class DesignDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
	{
		public ShopDbContext CreateDbContext(string[] args)
		{
			return new ShopDbContext("Server=localhost;Port=5432;Database=test;User Id=postgres;password=123");
		}
	}
}
