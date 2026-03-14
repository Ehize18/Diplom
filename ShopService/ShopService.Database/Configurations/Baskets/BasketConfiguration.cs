using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class BasketConfiguration : BaseConfiguration<Basket>
	{
		public override void Configure(EntityTypeBuilder<Basket> builder)
		{
			base.Configure(builder);

			builder.Property(x => x.IsCurrent)
				.HasColumnType("boolean")
				.HasDefaultValue(true)
				.IsRequired();

			builder.HasMany(x => x.Goods)
				.WithOne(x => x.Basket)
				.HasForeignKey(x => x.BasketId)
				.IsRequired();
		}
	}
}
