namespace Shared.RabbitMQ.Contracts
{
	public class PropertyUpdated
	{
		public required Guid PropertyId { get; set; }
		public required bool IsSuccess { get; set; }
		public string? Error { get; set; }
	}
}
