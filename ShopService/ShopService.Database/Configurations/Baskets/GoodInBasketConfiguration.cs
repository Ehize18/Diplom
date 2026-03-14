using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class GoodInBasketConfiguration : BaseConfiguration<GoodInBasket>
	{
		public override void Configure(EntityTypeBuilder<GoodInBasket> builder)
		{
			base.Configure(builder);

			builder.HasOne(x => x.Good)
				.WithMany()
				.HasForeignKey(x => x.GoodId)
				.IsRequired();

			builder.Property(x => x.Price)
				.HasColumnType("money")
				.IsRequired();

			builder.Property(x => x.Count)
				.HasColumnType("integer")
				.HasDefaultValue(1)
				.IsRequired();

			builder.HasOne(x => x.Basket)
				.WithMany(x => x.Goods)
				.HasForeignKey(x => x.BasketId)
				.IsRequired();
		}
	}
}
