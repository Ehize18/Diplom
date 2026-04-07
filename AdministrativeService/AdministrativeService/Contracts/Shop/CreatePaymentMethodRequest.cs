using Shared.Enums;

namespace AdministrativeService.Contracts.Shop
{
	public record CreatePaymentMethodRequest(PaymentType PaymentType, Dictionary<string, string> Metadata);
}
