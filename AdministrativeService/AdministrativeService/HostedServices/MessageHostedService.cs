using AdministrativeService.Application.Services;

namespace AdministrativeService.HostedServices
{
	public class MessageHostedService : IHostedService
	{
		private readonly MessageService _messageService;

		public MessageHostedService(MessageService messageService, IConfiguration configuration)
		{
			_messageService = messageService;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _messageService.Init(cancellationToken);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_messageService.Dispose();

			return Task.CompletedTask;
		}
	}
}
