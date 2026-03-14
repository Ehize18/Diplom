using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
	public abstract class BaseRabbitMQClient: IDisposable
	{
		private readonly IConnectionFactory _connectionFactory;

		private IConnection? _connection;

		private IChannel? _channel;

		protected IConnection Connection
		{
			get
			{
				if (_connection?.IsOpen != true)
				{
					_connection = _connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
				}
				return _connection;
			}
		}

		protected IChannel Channel
		{
			get
			{
				if (_channel?.IsOpen != true)
				{
					_channel = Connection.CreateChannelAsync().GetAwaiter().GetResult();
				}
				return _channel;
			}
		}

		protected RabbitMQOptions RabbitMQOptions { get; }

		protected ILogger<BaseRabbitMQClient> Logger { get; }

		public List<string> Exchanges { get; set; } = new();

		public List<string> Queues { get; set; } = new();

		public Dictionary<string, Dictionary<string, string>> QueueBinds { get; set; } = new();

		protected BaseRabbitMQClient(IOptions<RabbitMQOptions> options, ILogger<BaseRabbitMQClient> logger)
		{
			Logger = logger;
			RabbitMQOptions = options.Value;
			_connectionFactory = new ConnectionFactory
			{
				HostName = RabbitMQOptions.HostName,
				UserName = RabbitMQOptions.UserName,
				Password = RabbitMQOptions.Password
			};
		}

		public async Task ExchangeDeclareAsync(string exchange, string type = ExchangeType.Direct,
			bool durable = false, bool autoDelete = false,
			CancellationToken cancellationToken = default)
		{
			if (!Exchanges.Contains(exchange))
			{
				await Channel.ExchangeDeclareAsync(exchange, type, durable, autoDelete, cancellationToken: cancellationToken);
				Exchanges.Add(exchange);
			}
		}

		public async Task QueueDeclareAsync(string queue,
			bool durable = false, bool exclusive = false, bool autoDelete = false,
			CancellationToken cancellationToken = default)
		{
			if (!Queues.Contains(queue))
			{
				await Channel.QueueDeclareAsync(queue, durable, exclusive, autoDelete, cancellationToken: cancellationToken);
				Queues.Add(queue);
			}
		}

		public async Task QueueBindAsync(string queue, string exchange, string routingKey,
			CancellationToken cancellationToken = default)
		{
			if (!QueueBinds.ContainsKey(exchange) ||
				!QueueBinds[exchange].ContainsKey(routingKey))
			{
				await Channel.QueueBindAsync(queue, exchange, routingKey, cancellationToken: cancellationToken);
				if (QueueBinds.ContainsKey(exchange))
				{
					QueueBinds[exchange].Add(routingKey, queue);
					return;
				}
				QueueBinds.Add(exchange, new()
				{
					{
						routingKey, queue
					}
				});
			}
		}

		public virtual void Dispose()
		{
			if (_channel != null)
			{
				_channel.Dispose();
			}
			if (_connection != null)
			{
				_connection.Dispose();
			}
		}
	}
}
