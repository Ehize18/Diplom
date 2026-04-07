namespace Shared.RabbitMQ.Contracts
{
	public class OrderUpdated
	{
		public required Guid OrderId { get; set; }
		public required bool IsSuccess { get; set; }
		public string? Error { get; set; }
	}
}
