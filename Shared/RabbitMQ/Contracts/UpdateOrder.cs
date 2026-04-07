namespace Shared.RabbitMQ.Contracts
{
	public class UpdateOrder
	{
		public required Guid OrderId { get; set; }
		public required string EntityType { get; set; } // "OrderStatus", "PaymentStatus", "DeliveryStatus"
		public required int StatusValue { get; set; }
		public required Guid UpdatedById { get; set; }
		public required Guid ShopId { get; set; }
	}
}
