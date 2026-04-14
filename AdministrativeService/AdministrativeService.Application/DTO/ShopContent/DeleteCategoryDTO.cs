using AdministrativeService.Core.Entities;

namespace AdministrativeService.Application.DTO.ShopContent
{
	public class DeleteCategoryDTO
	{
		public required Guid ShopId { get; set; }
		public required Guid CategoryId { get; set; }
		public required User User { get; set; }
	}
}
