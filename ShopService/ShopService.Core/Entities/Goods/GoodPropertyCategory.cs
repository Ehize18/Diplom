namespace ShopService.Core.Entities
{
	/// <summary>
	/// Property category.
	/// </summary>
	public class GoodPropertyCategory : BaseEntity
	{
		/// <summary>
		/// Property category title.
		/// </summary>
		public required string Title { get; set; }

		/// <summary>
		/// Properties in category.
		/// </summary>
		public List<GoodProperty> Properties { get; set; } = new List<GoodProperty>();
	}
}
