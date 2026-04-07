using Shared.Enums;

namespace AdministrativeService.Contracts.Shop
{
	public record CreateDeliveryMethodRequest(DeliveryType DeliveryType, Dictionary<string, string> Metadata);
}
