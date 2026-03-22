using System.ComponentModel.DataAnnotations;

namespace AdministrativeService.Contracts.Content
{
	public record UpdateCategoryRequest(
		[MaxLength(50)] string? Title,
		string? Description,
		bool? IsActive,
		string? ParentCategoryId = "00000000-0000-0000-0000-000000000000");
}
