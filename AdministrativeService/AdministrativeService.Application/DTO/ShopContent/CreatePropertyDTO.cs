using AdministrativeService.Core.Entities;

namespace AdministrativeService.Application.DTO.ShopContent
{
	public class CreatePropertyDTO
	{
		public required string Title { get; set; }
		public required User User { get; set; }
		public required Guid ShopId { get; set; }
	}
}
