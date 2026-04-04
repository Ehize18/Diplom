using System.Text.Json.Serialization;

namespace ShopService.Core.Entities
{
	/// <summary>
	/// Good's property.
	/// </summary>
	public class GoodProperty : BaseEntity
	{
		/// <summary>
		/// Property title.
		/// </summary>
		public required string Title { get; set; }

		/// <summary>
		/// Property category.
		/// </summary>
		[JsonIgnore]
		public GoodPropertyCategory? Category { get; set; }

		/// <summary>
		/// Property category id.
		/// </summary>
		public Guid CategoryId { get; set; }

		/// <summary>
		/// Goods.
		/// </summary>
		public List<Good> Goods { get; set; } = new List<Good>();
	}
}
