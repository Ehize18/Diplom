namespace AdministrativeService.Contracts.Content
{
	public record UpdateOrderRequest(
		string EntityType,  // "OrderStatus", "PaymentStatus", "DeliveryStatus"
		int StatusValue
	);
}
