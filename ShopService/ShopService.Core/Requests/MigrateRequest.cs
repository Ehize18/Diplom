using System.ComponentModel.DataAnnotations;

namespace ShopService.Core.Requests
{
	public record MigrateRequest(
		[Required] Guid ShopId);
}
