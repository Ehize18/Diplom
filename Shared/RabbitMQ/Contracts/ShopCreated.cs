namespace Shared.RabbitMQ.Contracts
{
	public class ShopCreated
	{
		public required Guid ShopId { get; set; }
		public required bool IsSuccess { get; set; }
		public required bool AlreadyMigrated { get; set; }
	}
}
