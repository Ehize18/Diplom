using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class GoodPropertyCategoryConfiguration : BaseConfiguration<GoodPropertyCategory>
	{
		public override void Configure(EntityTypeBuilder<GoodPropertyCategory> builder)
		{
			base.Configure(builder);

			builder.Property(x => x.Title)
				.HasColumnType("varchar")
				.HasMaxLength(50)
				.IsRequired();

			builder.HasMany(x => x.Properties)
				.WithOne(x => x.Category)
				.HasForeignKey(x => x.CategoryId)
				.IsRequired();
		}
	}
}
