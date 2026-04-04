using System.Text.Json.Serialization;

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

		public bool IsActive { get; set; } = false;

		/// <summary>
		/// Parent category.
		/// </summary>
		[JsonIgnore]
		public GoodCategory? ParentCategory { get; set; }

		/// <summary>
		/// Parent category id.
		/// </summary>
		public Guid? ParentCategoryId { get; set; }

		/// <summary>
		/// Child categories.
		/// </summary>
		[JsonIgnore]
		public List<GoodCategory> ChildCategories { get; set; } = new List<GoodCategory>();

		public Guid? ImageId { get; set; }
	}
}
