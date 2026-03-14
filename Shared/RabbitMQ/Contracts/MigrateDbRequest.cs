namespace Shared.RabbitMQ.Contracts
{
	public class MigrateDbRequest
	{
		public required Guid ShopId { get; set; }
	}
}
