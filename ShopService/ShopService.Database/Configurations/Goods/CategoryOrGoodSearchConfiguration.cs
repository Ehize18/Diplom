
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class CategoryOrGoodSearchConfiguration : IEntityTypeConfiguration<CategoryOrGoodSearch>
	{
		public void Configure(EntityTypeBuilder<CategoryOrGoodSearch> builder)
		{
			builder.HasNoKey();

			builder.ToView("CategoryGoodSearch");
		}
	}
}
