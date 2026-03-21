namespace Shared.RabbitMQ.Contracts
{
	public class UpdateCategory
	{
		public Guid? CategoryId { get; set; }
		public required UpdateType UpdateType { get; set; }
		public string? CategoryTitle { get; set; }
		public string? CategoryDescription { get; set; }
		public Guid? ParentCategoryId { get; set; }
		public bool? IsActive { get; set; }

		public required Guid UpdatedById { get; set; }
		public required Guid ShopId { get; set; }
	}
}
