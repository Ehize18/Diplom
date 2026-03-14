namespace ShopService.Core.Entities
{
	/// <summary>
	/// Order.
	/// </summary>
	public class Order : BaseEntity
	{
		/// <summary>
		/// Full price of order.
		/// </summary>
		public decimal FullPrice { get; set; }

		/// <summary>
		/// Basket.
		/// </summary>
		public Basket? Basket { get; set; }

		/// <summary>
		/// Basket id.
		/// </summary>
		public Guid BasketId { get; set; }
	}
}
