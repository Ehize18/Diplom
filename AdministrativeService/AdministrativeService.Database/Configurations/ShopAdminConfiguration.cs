using AdministrativeService.Core.Entities;
using AdministrativeService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdministrativeService.Database.Configurations
{
	public class ShopAdminConfiguration : BaseConfiguration<ShopAdmin>
	{
		public override void Configure(EntityTypeBuilder<ShopAdmin> builder)
		{
			base.Configure(builder);

			builder.HasOne(x => x.User)
				.WithMany(x => x.Shops)
				.HasForeignKey(x => x.UserId)
				.IsRequired(true);

			builder.HasOne(x => x.Shop)
				.WithMany(x => x.Admins)
				.HasForeignKey(x => x.ShopId)
				.IsRequired(true);

			builder.Property(x => x.Feature)
				.HasColumnType("integer")
				.HasConversion(x => (int)x, y => (AdminFeature)y);
		}
	}
}
