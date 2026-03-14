using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class OrderConfiguration : BaseConfiguration<Order>
	{
		public override void Configure(EntityTypeBuilder<Order> builder)
		{
			base.Configure(builder);

			builder.Property(x => x.FullPrice)
				.HasColumnType("money")
				.IsRequired();

			builder.HasOne(x => x.Basket)
				.WithMany()
				.HasForeignKey(x => x.BasketId)
				.IsRequired();
		}
	}
}
