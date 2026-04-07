using System.Text.Json.Serialization;

namespace ShopService.Core.Entities
{
	/// <summary>
	/// Good in basket.
	/// </summary>
	public class GoodInBasket : BaseEntity
	{
		/// <summary>
		/// Good.
		/// </summary>
		public Good? Good { get; set; }

		/// <summary>
		/// Good id.
		/// </summary>
		public Guid GoodId { get; set; }

		/// <summary>
		/// Good price.
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// Good count in basket.
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Basket.
		/// </summary>
		[JsonIgnore]
		public Basket? Basket { get; set; }

		/// <summary>
		/// Basket id.
		/// </summary>
		public Guid BasketId { get; set; }
	}
}
