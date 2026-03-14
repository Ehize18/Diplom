using AdministrativeService.Core.Enums;

namespace AdministrativeService.Core.Entities
{
	/// <summary>
	/// Shop administrator.
	/// </summary>
	public class ShopAdmin : BaseEntity
	{
		/// <summary>
		/// User.
		/// </summary>
		public required User User { get; set; }

		/// <summary>
		/// User id.
		/// </summary>
		public required Guid UserId { get; set; }

		/// <summary>
		/// Shop.
		/// </summary>
		public required Shop Shop { get; set; }

		/// <summary>
		/// Shop id.
		/// </summary>
		public required Guid ShopId { get; set; }

		/// <summary>
		/// Flags what admin can do.
		/// </summary>
		public required AdminFeature Feature { get; set; } = AdminFeature.None;
	}
}
