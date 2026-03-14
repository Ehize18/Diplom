using Shared.RabbitMQ;

namespace AdministrativeService.HostedServices
{
	public class RabbitHostedService : IHostedService
	{
		private readonly RabbitMQService _rabbit;
		private readonly Dictionary<string, Dictionary<string, string>>? _queueBinds;

		public RabbitHostedService(RabbitMQService rabbit, IConfiguration configuration)
		{
			_rabbit = rabbit;
			var section = configuration.GetSection("RabbitMQ");
			_queueBinds = section?.Get<Dictionary<string, Dictionary<string, string>>>();
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			if (_queueBinds is null)
			{
				await _rabbit.Init(new(), new(), new());
				return;
			}

			var exchanges = _queueBinds.Keys.ToList();
			var queues = _queueBinds.Values.SelectMany(x => x.Values.ToList()).ToList();

			await _rabbit.Init(exchanges, queues, _queueBinds);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_rabbit.Dispose();

			return Task.CompletedTask;
		}
	}
}
