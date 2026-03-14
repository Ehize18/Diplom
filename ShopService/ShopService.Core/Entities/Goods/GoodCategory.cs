namespace ShopService.Core.Entities
{
	/// <summary>
	/// Good's category.
	/// </summary>
	public class GoodCategory : BaseEntity
	{
		/// <summary>
		/// Category's title.
		/// </summary>
		public required string Title { get; set; }

		/// <summary>
		/// Category's description.
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Parent category.
		/// </summary>
		public GoodCategory? ParentCategory { get; set; }

		/// <summary>
		/// Parent category id.
		/// </summary>
		public Guid? ParentCategoryId { get; set; }

		/// <summary>
		/// Child categories.
		/// </summary>
		public List<GoodCategory> ChildCategories { get; set; } = new List<GoodCategory>();
	}
}
