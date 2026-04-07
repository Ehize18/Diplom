using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Enums;
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

			builder.HasOne(x => x.DeliveryMethod)
				.WithMany()
				.HasForeignKey(x => x.DeliveryMethodId)
				.IsRequired();

			builder.HasOne(x => x.PaymentMethod)
				.WithMany()
				.HasForeignKey(x => x.PaymentMethodId)
				.IsRequired();

			builder.Property(x => x.PaymentStatus)
				.HasColumnType("integer")
				.HasConversion(x => (int)x, y => (PaymentStatus)y);

			builder.Property(x => x.DeliveryStatus)
				.HasColumnType("integer")
				.HasConversion(x => (int)x, y => (DeliveryStatus)y);

			builder.Property(x => x.OrderStatus)
				.HasColumnType("integer")
				.HasConversion(x => (int)x, y => (OrderStatus)y);

			builder.Property(x => x.DeliveryExtras)
				.HasColumnType("text");

			builder.Navigation(x => x.Basket)
				.AutoInclude();

			builder.Navigation(x => x.DeliveryMethod)
				.AutoInclude();

			builder.Navigation(x => x.PaymentMethod)
				.AutoInclude();
		}
	}
}
