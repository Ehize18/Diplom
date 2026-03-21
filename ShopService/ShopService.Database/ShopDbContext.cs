using Microsoft.EntityFrameworkCore;
using ShopService.Core;
using ShopService.Core.Entities;
using ShopService.Database.Configurations;

namespace ShopService.Database
{
	/// <summary>
	/// Shop <see cref="DbContext"/>.
	/// </summary>
	public class ShopDbContext : DbContext
	{
		public User DbUser { get; private set; } = Consts.SystemUser;

		private readonly string? _connectionString;

		public DbSet<User> User { get; set; }

		public DbSet<Order> Order { get; set; }

		public DbSet<Good> Good { get; set; }

		public DbSet<GoodCategory> GoodCategory { get; set; }

		public DbSet<GoodProperty> GoodProperty { get; set; }

		public DbSet<GoodPropertyCategory> GoodPropertyCategory { get; set; }

		public DbSet<Basket> Basket { get; set; }

		public DbSet<GoodInBasket> GoodInBasket { get; set; }

		public ShopDbContext(ConnectionStringProvider connectionStringProvider, DbContextOptions options) : base(options)
		{
			_connectionString = connectionStringProvider.ConnectionString;
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

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var now = DateTime.UtcNow;

			foreach (var entry in ChangeTracker.Entries())
			{
				if (entry.Entity is BaseEntity entity)
				{
					if (entry.State == EntityState.Modified)
					{
						entity.UpdatedAt = now;
						entity.UpdatedById = DbUser.Id;
					}

					if (entry.State == EntityState.Added)
					{
						entity.CreatedAt = now;
						entity.CreatedById = DbUser.Id;
					}
				}
			}

			return base.SaveChangesAsync(cancellationToken);
		}

		public void SetUser(User user)
		{
			DbUser = user;
		}

		public async Task<bool> SetUser(Guid userId, CancellationToken cancellationToken = default)
		{
			var user = await User.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

			if (user == null)
			{
				return false;
			}
			DbUser = user;
			return true;
		}
	}
}
