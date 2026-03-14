using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
	{
		public virtual void Configure(EntityTypeBuilder<TEntity> builder)
		{
			builder.HasKey(x => x.Id);

			builder.HasOne(x => x.CreatedBy)
				.WithMany()
				.HasForeignKey(x => x.CreatedById)
				.IsRequired(true);
			builder.HasOne(x => x.UpdatedBy)
				.WithMany()
				.HasForeignKey(x => x.UpdatedById)
				.IsRequired(false);

			builder.Property(x => x.CreatedAt)
				.HasColumnType("timestamptz");
			builder.Property(x => x.UpdatedAt)
				.HasColumnType("timestamptz");
		}
	}
}
