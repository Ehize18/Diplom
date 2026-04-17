using AdministrativeService.Core.Entities;

namespace AdministrativeService.Application.DTO.ShopContent
{
	public class DeleteGoodDTO
	{
		public required Guid ShopId { get; set; }
		public required Guid GoodId { get; set; }
		public required User User { get; set; }
	}
}
