namespace Shared.RabbitMQ
{
	public class RabbitMQService : IDisposable
	{
		private readonly RabbitMQConsumer _consumer;

		private readonly RabbitMQPublisher _publisher;

		public bool IsInited { get; private set; } = false;

		public RabbitMQService(RabbitMQConsumer consumer, RabbitMQPublisher publisher)
		{
			_consumer = consumer;
			_publisher = publisher;
		}

		public async Task Init(List<string> exchanges, List<string> queues, Dictionary<string, Dictionary<string, string>> queueBinds)
		{
			_publisher.Exchanges = _consumer.Exchanges;
			_publisher.Queues = _consumer.Queues;
			_publisher.QueueBinds = _consumer.QueueBinds;

			var declareTasks = new List<Task>();

			foreach (var exchange in exchanges)
			{
				declareTasks.Add(_publisher.ExchangeDeclareAsync(exchange));
			}

			foreach (var queue in queues)
			{
				declareTasks.Add(_publisher.QueueDeclareAsync(queue));
			}

			await Task.WhenAll(declareTasks);

			var bindTasks = new List<Task>();

			foreach (var exchange in queueBinds.Keys)
			{
				foreach (var queueBind in queueBinds[exchange])
				{
					var routingKey = queueBind.Key;
					var queue = queueBind.Value;
					bindTasks.Add(_publisher.QueueBindAsync(queue, exchange, routingKey));
				}
			}

			await Task.WhenAll(bindTasks);
			IsInited = true;
		}

		public async Task<bool> PublishMessage(object message, string exchange, string routingKey, CancellationToken cancellationToken = default)
		{
			return await
				_publisher.Publish(message, exchange, routingKey, cancellationToken);
		}

		public async Task<string> AddConsumer<T>(string queue, Func<T, Task> handler, CancellationToken cancellationToken = default)
		{
			return await
				_consumer.ConsumeAsync(queue, handler, cancellationToken);
		}

		public async Task RemoveConsumer(string consumerTag)
		{
			await _consumer.StopConsumingByTagAsync(consumerTag);
		}

		public void Dispose()
		{
			if (_consumer != null)
			{
				_consumer.Dispose();
			}

			if (_publisher != null)
			{
				_publisher.Dispose();
			}
		}
	}
}
