using System.Text.Json.Serialization;

namespace ShopService.Core.Entities
{
	/// <summary>
	/// Good in shop.
	/// </summary>
	public class Good : BaseEntity
	{
		/// <summary>
		/// Good's title.
		/// </summary>
		public required string Title { get; set; }

		/// <summary>
		/// Good's description.
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Good's count.
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Good's real price.
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// Good's old price.
		/// </summary>
		public decimal OldPrice { get; set; }

		/// <summary>
		/// Good category.
		/// </summary>
		[JsonIgnore]
		public GoodCategory? Category { get; set; }

		/// <summary>
		/// Good category id.
		/// </summary>
		public Guid CategoryId { get; set; }

		/// <summary>
		/// Good properties.
		/// </summary>
		public List<GoodProperty> Properties { get; set; } = new List<GoodProperty>();

		/// <summary>
		/// Soft delete flag.
		/// </summary>
		public bool IsDeleted { get; set; } = false;

		public Guid? ImageId { get; set; }
	}
}
