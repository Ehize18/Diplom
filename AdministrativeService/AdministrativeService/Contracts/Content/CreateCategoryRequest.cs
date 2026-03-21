using System.ComponentModel.DataAnnotations;

namespace AdministrativeService.Contracts.Content
{
	public record CreateCategoryRequest(
		[Required][MaxLength(50)] string Title,
		string? Description,
		Guid? ParentCategory = null,
		bool IsActive = false);
}
