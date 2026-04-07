using System.ComponentModel.DataAnnotations;

namespace AdministrativeService.Contracts.Content
{
	public record UpdateGoodRequest(
		[MaxLength(100)] string? Title,
		string? Description,
		string? CategoryId,
		int? Count,
		decimal? Price,
		decimal? OldPrice,
		Guid? ImageId = null);
}
