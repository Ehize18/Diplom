using AdministrativeService.Core;
using AdministrativeService.Core.Entities;
using AdministrativeService.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AdministrativeService.Database
{
	public class AdminDbContext : DbContext
	{
		public User DbUser { get; private set; } = Consts.SystemUser;

		public DbSet<User> User { get; set; }

		public DbSet<Shop> Shop { get; set; }

		public DbSet<ShopAdmin> ShopAdmin { get; set; }

		public AdminDbContext(DbContextOptions options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new UserConfiguration());
			modelBuilder.ApplyConfiguration(new ShopConfiguration());
			modelBuilder.ApplyConfiguration(new ShopAdminConfiguration());
			
			base.OnModelCreating(modelBuilder);
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
						entity.UpdatedBy = DbUser;
					}

					if (entry.State == EntityState.Added)
					{
						entity.CreatedAt = now;
						entity.CreatedBy = DbUser;
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
