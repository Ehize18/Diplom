using System.ComponentModel.DataAnnotations;

namespace AdministrativeService.Contracts.Content
{
	public record CreatePropertyRequest([MinLength(1)] string Title);
}
