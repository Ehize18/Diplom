using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class GoodCategoryConfiguration : BaseConfiguration<GoodCategory>
	{
		public override void Configure(EntityTypeBuilder<GoodCategory> builder)
		{
			base.Configure(builder);

			builder.Property(x => x.Title)
				.HasColumnType("varchar")
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(x => x.Description)
				.HasColumnType("text")
				.IsRequired(false);

			builder.Property(x => x.IsActive)
				.HasColumnType("boolean")
				.HasDefaultValue(false)
				.IsRequired();

			builder.Ignore(x => x.ChildsCount);

			builder.HasOne(x => x.ParentCategory)
				.WithMany(x => x.ChildCategories)
				.HasForeignKey(x => x.ParentCategoryId)
				.IsRequired(false);
		}
	}
}
