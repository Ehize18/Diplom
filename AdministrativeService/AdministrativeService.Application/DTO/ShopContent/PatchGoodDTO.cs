using AdministrativeService.Core.Entities;

namespace AdministrativeService.Application.DTO.ShopContent
{
	public class PatchGoodDTO
	{
		public required Guid ShopId { get; set; }
		public required Guid GoodId { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public Guid? CategoryId { get; set; }
		public int? Count { get; set; }
		public decimal? Price { get; set; }
		public decimal? OldPrice { get; set; }
		public Guid? ImageId { get; set; }
		public required User User { get; set; }
	}
}
