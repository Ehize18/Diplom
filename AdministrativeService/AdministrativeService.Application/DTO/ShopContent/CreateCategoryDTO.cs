using AdministrativeService.Core.Entities;

namespace AdministrativeService.Application.DTO.ShopContent
{
	public class CreateCategoryDTO
	{
		public required Guid ShopId { get; set; }
		public required string Title { get; set; }
		public string? Description { get; set; }
		public Guid? ParentCategoryId { get; set; }
		public Guid? ImageId { get; set; }
		public Guid? Id { get; set; }
		public bool IsActive { get; set; } = false;
		public required User User { get; set; }
	}
}
