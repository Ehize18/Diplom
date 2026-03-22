using System.ComponentModel.DataAnnotations;

namespace AdministrativeService.Contracts.Content
{
	public record CreateGoodRequest(
		[Required][MaxLength(50)] string Title,
		string? Description,
		Guid CategoryId,
		int Count = 0,
		decimal Price = 0,
		decimal OldPrice = 0);
}
