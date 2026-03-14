using Shared.RabbitMQ;
using Shared.RabbitMQ.Contracts;
using ShopService.Application.Services;

namespace ShopService.HostedServices
{
	public class RabbitHostedService : IHostedService
	{
		private readonly RabbitMQService _rabbit;
		private readonly Dictionary<string, Dictionary<string, string>>? _queueBinds;
		private readonly IServiceProvider _serviceProvider;

		public RabbitHostedService(RabbitMQService rabbit, IConfiguration configuration, IServiceProvider serviceProvider)
		{
			_rabbit = rabbit;
			var section = configuration.GetSection("RabbitMQ");
			_queueBinds = section?.Get<Dictionary<string, Dictionary<string, string>>>();
			_serviceProvider = serviceProvider;
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
			await StartConsuming();
		}

		private async Task StartConsuming()
		{
			await _rabbit.AddConsumer<MigrateDbRequest>("MigrateRequest", async request =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var dbMigrator = _serviceProvider.GetRequiredService<DbMigrator>();
					var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
					var connectionStringTemplate = configuration.GetValue<string>("DBConnectionStringTemplate")!;
					var response = await dbMigrator.Migrate(request.ShopId, connectionStringTemplate);

					if (response != null)
					{
						await _rabbit.PublishMessage(response, "ShopExchange", "MigrateResponse");
					}
				}
			});
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_rabbit.Dispose();

			return Task.CompletedTask;
		}
	}
}
