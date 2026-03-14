using Microsoft.EntityFrameworkCore;
using ShopService.Core.Entities;
using ShopService.Database.Configurations;

namespace ShopService.Database
{
	/// <summary>
	/// Shop <see cref="DbContext"/>.
	/// </summary>
	public class ShopDbContext : DbContext
	{
		private readonly string? _connectionString;

		DbSet<User> User { get; set; }

		DbSet<Order> Order { get; set; }

		DbSet<Good> Good { get; set; }

		DbSet<GoodCategory> GoodCategory { get; set; }

		DbSet<GoodProperty> GoodProperty { get; set; }

		DbSet<GoodPropertyCategory> GoodPropertyCategory { get; set; }

		DbSet<Basket> Basket { get; set; }

		DbSet<GoodInBasket> GoodInBasket { get; set; }

		public ShopDbContext(DbContextOptions options) : base(options)
		{
		}

		public ShopDbContext(string connectionString)
		{
			_connectionString = connectionString;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (_connectionString != null)
			{
				optionsBuilder.UseNpgsql(_connectionString);
			}
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			ApplyConfigurations(modelBuilder);

			base.OnModelCreating(modelBuilder);
		}

		protected virtual void ApplyConfigurations(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new UserConfiguration());
			modelBuilder.ApplyConfiguration(new OrderConfiguration());
			modelBuilder.ApplyConfiguration(new GoodConfiguration());
			modelBuilder.ApplyConfiguration(new GoodCategoryConfiguration());
			modelBuilder.ApplyConfiguration(new GoodPropertyConfiguration());
			modelBuilder.ApplyConfiguration(new GoodPropertyCategoryConfiguration());
			modelBuilder.ApplyConfiguration(new BasketConfiguration());
			modelBuilder.ApplyConfiguration(new GoodInBasketConfiguration());
		}
	}
}
