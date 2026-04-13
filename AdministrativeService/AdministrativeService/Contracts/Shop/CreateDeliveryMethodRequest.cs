using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace AdministrativeService.Contracts.Shop
{
	public record CreateDeliveryMethodRequest(
		DeliveryType DeliveryType,
		Dictionary<string, string> Metadata,
		[MaxLength(50)] string Title);
}
