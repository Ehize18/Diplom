using System.ComponentModel.DataAnnotations;

namespace AdministrativeService.Contracts.Shop
{
	public record CreateShopRequest(
		[Required][Length(1, 50)] string Title);
}
