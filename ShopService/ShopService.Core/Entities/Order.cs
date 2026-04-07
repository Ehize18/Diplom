using Shared.Enums;

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

		public DeliveryMethod? DeliveryMethod { get; set; }

		public Guid DeliveryMethodId { get; set; }

		public string DeliveryExtras { get; set; } = string.Empty;

		public PaymentMethod? PaymentMethod { get; set; }

		public Guid PaymentMethodId { get; set; }

		public PaymentStatus PaymentStatus { get; set; }

		public DeliveryStatus DeliveryStatus { get; set; }

		public OrderStatus OrderStatus { get; set; }
	}
}
