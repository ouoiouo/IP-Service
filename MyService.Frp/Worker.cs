using MyService.Frp.Services;

namespace MyService.Frp
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var workerService = scope.ServiceProvider.GetService<WorkerService>();
                    workerService.Start();

                    var ddnsService = scope.ServiceProvider.GetService<DDNSService>();
                    ddnsService.Start();
                }

                await Task.Delay(1000 * 60, stoppingToken);
            }
        }
    }
}