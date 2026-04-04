namespace AdministrativeService.Core.Entities
{
	/// <summary>
	/// Shop.
	/// </summary>
	public class Shop : BaseEntity
	{
		/// <summary>
		/// Shop's title.
		/// </summary>
		public required string Title { get; set; }

		/// <summary>
		/// Shop's description.
		/// </summary>
		public string? Description { get; set; }

		public long? VkGroupId { get; set; }

		public List<ShopAdmin>? Admins { get; set; }
	}
}
