using System.ComponentModel.DataAnnotations;

namespace AdministrativeService.Core.Requests
{
	public record CreateShopRequest(
		[Required][Length(1, 50)] string Title);
}
