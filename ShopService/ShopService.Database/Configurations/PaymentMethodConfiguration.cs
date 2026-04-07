using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class PaymentMethodConfiguration : BaseConfiguration<PaymentMethod>
	{
		public override void Configure(EntityTypeBuilder<PaymentMethod> builder)
		{
			base.Configure(builder);

			builder.Property(x => x.Title)
				.HasColumnType("varchar")
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(x => x.Metadata)
				.HasColumnType("jsonb");

			builder.Ignore(x => x.MetadataBody);
		}
	}
}
