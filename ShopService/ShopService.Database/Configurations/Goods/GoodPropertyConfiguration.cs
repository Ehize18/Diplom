using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class GoodPropertyConfiguration : BaseConfiguration<GoodProperty>
	{
		public override void Configure(EntityTypeBuilder<GoodProperty> builder)
		{
			base.Configure(builder);

			builder.Property(x => x.Title)
				.HasColumnType("varchar")
				.HasMaxLength(50)
				.IsRequired();

			builder.HasOne(x => x.Category)
				.WithMany(x => x.Properties)
				.HasForeignKey(x => x.CategoryId)
				.IsRequired();

			builder.HasMany(x => x.Goods)
				.WithMany(x => x.Properties);
		}
	}
}
