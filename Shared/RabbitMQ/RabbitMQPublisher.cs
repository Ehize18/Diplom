using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
	public class RabbitMQPublisher : BaseRabbitMQClient
	{
		public RabbitMQPublisher(IOptions<RabbitMQOptions> options, ILogger<RabbitMQPublisher> logger) : base(options, logger)
		{
		}

		public async Task<bool> Publish<TProperties>(TProperties properties, string exchange, string routingKey, object message,
			CancellationToken cancellationToken = default) where TProperties : IReadOnlyBasicProperties, IAmqpHeader
		{
			Logger.LogInformation($"Start publish message to {exchange} with {routingKey}");
			string serializedMessage;
			byte[] messageBytes;
			try
			{
				serializedMessage = JsonSerializer.Serialize(message, JsonSerializerOptions);
				messageBytes = Encoding.UTF8.GetBytes(serializedMessage);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, $"Message serialization failed {message}");
				return false;
			}

			try
			{
				Logger.LogInformation($"Publishing message to {exchange} with {routingKey}");
				await Channel.BasicPublishAsync(exchange, routingKey, true, properties,  messageBytes, cancellationToken);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, $"Publishing message to {exchange} with {routingKey} failed");
				return false;
			}

			Logger.LogInformation($"Published message to {exchange} with {routingKey}");
			return true;
		}
	}
}
