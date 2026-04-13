using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace AdministrativeService.Contracts.Shop
{
	public record CreatePaymentMethodRequest(
		PaymentType PaymentType,
		Dictionary<string, string> Metadata,
		[MaxLength(50)] string Title);
}
