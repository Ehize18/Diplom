using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class GoodCategoriesAllConfiguration : IEntityTypeConfiguration<GoodCategoriesAll>
	{
		public void Configure(EntityTypeBuilder<GoodCategoriesAll> builder)
		{
			builder.HasNoKey();

			builder.ToView("GoodCategoriesAllView");

			builder.HasOne(x => x.Good)
				.WithMany()
				.HasForeignKey(x => x.GoodId);

			builder.HasOne(x => x.ActualCategory)
				.WithMany()
				.HasForeignKey(x => x.ActualCategoryId);

			builder.HasOne(x => x.FilterCategory)
				.WithMany()
				.HasForeignKey(x => x.FilterCategoryId);
		}
	}
}
