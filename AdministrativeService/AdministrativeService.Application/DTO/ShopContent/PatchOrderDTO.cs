using AdministrativeService.Core.Entities;

namespace AdministrativeService.Application.DTO.ShopContent
{
	public class PatchOrderDTO
	{
		public required Guid ShopId { get; set; }
		public required Guid OrderId { get; set; }
		public required string EntityType { get; set; }
		public required int StatusValue { get; set; }
		public required User User { get; set; }
	}
}
