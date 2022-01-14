using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrdersDbArchiver.BusinessLogicLayer;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;

namespace OrdersDbArchiver.App
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly AppConfigsModel _configModel;

        public Worker(ILogger<Worker> logger, IOptions<AppConfigsModel> appOptions)
        {
            _logger = logger;
            _configModel = appOptions.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IOrdersArchiver dataHandler = new OrdersArchiver(_configModel, new FileInfoFactory(), new FileWatcher(_configModel.Folders.ObservedFolder));
            dataHandler.OnMessage += (sender, args) => _logger.LogInformation(args.Message);
            dataHandler.StartWork();

            return Task.CompletedTask;
        }
    }
}
