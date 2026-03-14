using AdministrativeService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdministrativeService.Database.Configurations
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

			builder.Property(x => x.TelegramId)
				.IsRequired(false);

			builder.HasMany(x => x.Shops)
				.WithOne(x => x.User)
				.HasForeignKey(x => x.UserId);
		}
	}
}
