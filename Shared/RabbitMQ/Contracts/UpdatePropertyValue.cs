namespace Shared.RabbitMQ.Contracts
{
	public class UpdatePropertyValue : UpdateProperty
	{
		public Guid? PropertyValueId { get; set; }
	}
}
