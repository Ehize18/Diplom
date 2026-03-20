using AdministrativeService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdministrativeService.Database.Configurations
{
	public class ShopConfiguration : BaseConfiguration<Shop>
	{
		public override void Configure(EntityTypeBuilder<Shop> builder)
		{
			base.Configure(builder);

			builder.Property(x => x.Title)
				.HasColumnType("varchar")
				.HasMaxLength(50)
				.IsRequired(true);

			builder.Property(x => x.Description)
				.HasColumnType("text")
				.IsRequired(false);

			builder.HasMany(x => x.Admins)
				.WithOne(x => x.Shop)
				.HasForeignKey(x => x.ShopId);

			builder.Navigation(x => x.Admins)
				.AutoInclude();
		}
	}
}
