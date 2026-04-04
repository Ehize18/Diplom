using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopService.Core.Entities;

namespace ShopService.Database.Configurations
{
	public class UserConfiguration : BaseConfiguration<User>
	{
		public override void Configure(EntityTypeBuilder<User> builder)
		{
			base.Configure(builder);

			builder.Property(x => x.Username)
				.HasColumnType("varchar")
				.HasMaxLength(50)
				.IsRequired(true);

			builder.Property(x => x.PasswordHash)
				.HasColumnType("text")
				.IsRequired(false);

			builder.Property(x => x.IsAdmin)
				.HasColumnType("boolean")
				.HasDefaultValue(false)
				.IsRequired(true);

			builder.Property(x => x.TelegramId)
				.IsRequired(false);

			builder.Property(x => x.VkId)
				.IsRequired(false);

			builder.HasMany(x => x.Baskets)
				.WithOne(x => x.User)
				.HasForeignKey(x => x.UserId);
		}
	}
}
