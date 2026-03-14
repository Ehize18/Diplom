using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class GoodConfiguration : BaseConfiguration<Good>
	{
		public override void Configure(EntityTypeBuilder<Good> builder)
		{
			base.Configure(builder);

			builder.Property(x => x.Title)
				.HasColumnType("varchar")
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(x => x.Description)
				.HasColumnType("text")
				.IsRequired(false);

			builder.Property(x => x.Count)
				.HasColumnType("integer")
				.HasDefaultValue(0)
				.IsRequired();

			builder.Property(x => x.Price)
				.HasColumnType("money")
				.HasDefaultValue(0)
				.IsRequired();

			builder.Property(x => x.OldPrice)
				.HasColumnType("money")
				.HasDefaultValue(0)
				.IsRequired();

			builder.HasOne(x => x.Category)
				.WithMany()
				.HasForeignKey(x => x.CategoryId)
				.IsRequired();

			builder.HasMany(x => x.Properties)
				.WithMany(x => x.Goods);

			builder.Property(x => x.IsDeleted)
				.HasColumnType("boolean")
				.HasDefaultValue(false)
				.IsRequired();
		}
	}
}
