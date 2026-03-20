using AdministrativeService.Core.Entities;

namespace AdministrativeService.Application.DTO.Shop
{
	public class CreateShopDTO
	{
		public required User User { get; set; }
		public required string Title { get; set; }
		public string Description { get; set; } = string.Empty;
	}
}
