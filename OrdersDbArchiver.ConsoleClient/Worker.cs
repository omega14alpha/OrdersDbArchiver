using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrdersDbArchiver.BusinessLogicLayer;
using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using OrdersDbArchiver.ConsoleClient.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace OrdersDbArchiver.ConsoleClient
{
    public class Worker : BackgroundService
    {
        private readonly IOrdersArchiver _archiver;

        public Worker(IOptions<AppConfigsModel> appOptions)
        {
            var configModel = appOptions.Value;
            _archiver = new OrdersArchiver(configModel, new FileInfoFactory(), new FileWatcher(configModel.Folders));
            _archiver.OnMessage += Messager.SendMessage;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _archiver.StartWork();
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _archiver.StopWork(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
