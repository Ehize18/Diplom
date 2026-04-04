using AdministrativeService.Core.Entities;

namespace AdministrativeService.Application.DTO.ShopContent
{
	public class PatchCategoryDTO
	{
		public required Guid ShopId { get; set; }
		public required Guid CategoryId { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public Guid? ParentCategoryId { get; set; }
		public Guid? ImageId { get; set; }
		public bool? IsActive { get; set; }
		public required User User { get; set; }
	}
}
