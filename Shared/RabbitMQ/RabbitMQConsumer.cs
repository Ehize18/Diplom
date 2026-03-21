using System.Collections;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared.RabbitMQ
{
	public class RabbitMQConsumer : BaseRabbitMQClient
	{
		private readonly Dictionary<string, string> _consumerTags = new();

		public RabbitMQConsumer(IOptions<RabbitMQOptions> options, ILogger<RabbitMQConsumer> logger) : base(options, logger)
		{
		}

		public async Task<string> ConsumeAsync<T>(
			string queue,
			Func<object, BasicDeliverEventArgs, T, Task> handler,
			CancellationToken cancellationToken = default)
		{
			Logger.LogInformation($"Start consuming messages from queue {queue}");

			var consumer = new AsyncEventingBasicConsumer(Channel);
			consumer.ReceivedAsync += async (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);

				Logger.LogInformation($"Received message from queue {queue}: {message}");

				try
				{
					var deserializedMessage = JsonSerializer.Deserialize<T>(message, JsonSerializerOptions);
					if (deserializedMessage is not null)
					{
						await handler(model, ea, deserializedMessage);
						await Channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken);
						Logger.LogInformation($"Message from queue {queue} processed successfully");
					}
					else
					{
						Logger.LogWarning($"Message from queue {queue} deserialized to null");
						await Channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken);
					}
				}
				catch (Exception ex)
				{
					Logger.LogError(ex, $"Error processing message from queue {queue}: {message}");
					await Channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken);
				}
			};

			var result = await Channel.BasicConsumeAsync(queue, autoAck: false, consumer, cancellationToken);
			_consumerTags[queue] = result;

			Logger.LogInformation($"Consumer started for queue {queue} with tag {result}");
			return result;
		}

		public async Task StopConsumingByQueueAsync(string queue, CancellationToken cancellationToken = default)
		{
			if (_consumerTags.TryGetValue(queue, out var consumerTag))
			{
				Logger.LogInformation($"Stopping consumer for queue {queue} with tag {consumerTag}");
				await Channel.BasicCancelAsync(consumerTag, false, cancellationToken);
				_consumerTags.Remove(queue);
				Logger.LogInformation($"Consumer stopped for queue {queue}");
			}
			else
			{
				Logger.LogWarning($"No active consumer found for queue {queue}");
			}
		}

		public async Task StopConsumingByTagAsync(string consumerTag, CancellationToken cancellationToken = default)
		{
			var kv = _consumerTags.FirstOrDefault(x => x.Value == consumerTag);
			var queue = kv.Key;
			Logger.LogInformation($"Stopping consumer for queue {queue} with tag {consumerTag}");
			await Channel.BasicCancelAsync(consumerTag, false, cancellationToken);
			_consumerTags.Remove(queue);
			Logger.LogInformation($"Consumer stopped for queue {queue}");
		}

		public async Task StopAllConsumingAsync(CancellationToken cancellationToken = default)
		{
			Logger.LogInformation($"Stopping all consumers");
			foreach (var kvp in _consumerTags.ToList())
			{
				await StopConsumingByQueueAsync(kvp.Key, cancellationToken);
			}
		}

		public override void Dispose()
		{
			StopAllConsumingAsync().GetAwaiter().GetResult();
			base.Dispose();
		}
	}
}
