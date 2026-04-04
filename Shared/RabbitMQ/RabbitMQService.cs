using System.Collections.Concurrent;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared.RabbitMQ
{
	public abstract class RabbitMQService : IDisposable
	{
		private readonly RabbitMQConsumer _consumer;

		private readonly RabbitMQPublisher _publisher;


		private readonly ConcurrentDictionary<Guid, TaskCompletionSource<object?>> _pendingRequests = new ConcurrentDictionary<Guid, TaskCompletionSource<object?>>();
		protected abstract string[] Exchanges { get; }
		protected abstract string[] Queues { get; }
		protected abstract int DefaultTimeout { get; }

		protected abstract Dictionary<string, Dictionary<string, string>> QueueBinds { get; }

		public bool IsInited { get; private set; } = false;

		public RabbitMQService(RabbitMQConsumer consumer, RabbitMQPublisher publisher)
		{
			_consumer = consumer;
			_publisher = publisher;
		}

		public async Task Init(CancellationToken cancellationToken = default)
		{
			_publisher.Exchanges = _consumer.Exchanges;
			_publisher.Queues = _consumer.Queues;
			_publisher.QueueBinds = _consumer.QueueBinds;

			var declareTasks = new List<Task>();

			foreach (var exchange in Exchanges)
			{
				declareTasks.Add(_publisher.ExchangeDeclareAsync(exchange, cancellationToken: cancellationToken));
			}

			foreach (var queue in Queues)
			{
				declareTasks.Add(_publisher.QueueDeclareAsync(queue, cancellationToken: cancellationToken));
			}

			await Task.WhenAll(declareTasks);

			var bindTasks = new List<Task>();

			foreach (var exchange in QueueBinds.Keys)
			{
				foreach (var queueBind in QueueBinds[exchange])
				{
					var routingKey = queueBind.Key;
					var queue = queueBind.Value;
					await _publisher.QueueBindAsync(queue, exchange, routingKey, cancellationToken);
				}
			}

			await InitConsumers(cancellationToken);
			IsInited = true;
		}

		protected abstract Task InitConsumers(CancellationToken cancellationToken = default);

		public async Task<T?> GetAnswerAsync<T>(Guid messageId, CancellationToken cancellationToken = default)
		{
			var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

			if (!_pendingRequests.TryAdd(messageId, tcs))
			{
				throw new InvalidOperationException("Message already exists");
			}

			try
			{
				using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
				timeoutCts.CancelAfter(DefaultTimeout);

				var result = await tcs.Task.WaitAsync(timeoutCts.Token);

				if (result is T)
				{
					return (T?)result;
				}

				return default;
			}
			catch (OperationCanceledException)
			{
				_pendingRequests.TryRemove(messageId, out _);
				return default;
			}
			finally
			{
				_pendingRequests.TryRemove(messageId, out _);
			}
		}

		public async Task<bool> PublishMessage<TProperties>(TProperties properties, object message, string exchange, string routingKey,
			CancellationToken cancellationToken = default) where TProperties : IReadOnlyBasicProperties, IAmqpHeader
		{
			return await
				_publisher.Publish(properties, exchange, routingKey, message, cancellationToken);
		}

		public async Task<string> AddConsumer<T>(string queue, Func<object, BasicDeliverEventArgs, T, Task> handler, CancellationToken cancellationToken = default)
		{
			return await
				_consumer.ConsumeAsync(queue, handler, cancellationToken);
		}

		public async Task<string> AddReadConsumer<T>(string queue, CancellationToken cancellationToken = default) where T : class
		{
			return await _consumer.ConsumeAsync<T>(queue, (model, ea, message) =>
			{
				var props = ea.BasicProperties;

				if (Guid.TryParse(props.CorrelationId, out var correlationId))
				{
					if (_pendingRequests.TryRemove(correlationId, out var tcs))
					{
						tcs.SetResult(message);
					}
				}

				return Task.CompletedTask;
			}, cancellationToken);
		}

		public async Task RemoveConsumer(string consumerTag)
		{
			await _consumer.StopConsumingByTagAsync(consumerTag);
		}

		public BasicProperties CreateProperties()
		{
			return _publisher.CreateProperties();
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
