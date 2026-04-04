namespace Shared.RabbitMQ.Contracts
{
	public class UpdateProperty
	{
		public Guid? PropertyId { get; set; }
		public required string PropertyTitle { get; set; }
		public required UpdateType UpdateType { get; set; }
		public required Guid UpdatedById { get; set; }
		public required Guid ShopId { get; set; }
	}
}
