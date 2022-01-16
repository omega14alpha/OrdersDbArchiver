using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrdersDbArchiver.BusinessLogicLayer;
using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OrdersDbArchiver.ServiceClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IOrdersArchiver _archiver;

        public Worker(ILogger<Worker> logger, IOptions<AppConfigsModel> appOptions)
        {
            _logger = logger;

            var configModel = appOptions.Value;
            _archiver = new OrdersArchiver(configModel, new FileInfoFactory(), new FileWatcher(configModel.Folders));
            _archiver.OnMessage += (sender, args) => _logger.LogInformation(args.Message);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _archiver.StartWork();
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _archiver.StopWork();
            return Task.CompletedTask;
        }
    }
}
