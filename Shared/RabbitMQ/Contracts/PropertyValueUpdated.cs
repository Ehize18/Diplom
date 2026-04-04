namespace Shared.RabbitMQ.Contracts
{
	public class PropertyValueUpdated
	{
		public required Guid PropertyValueId { get; set; }
		public required bool IsSuccess { get; set; }
		public string? Error { get; set; }
	}
}
