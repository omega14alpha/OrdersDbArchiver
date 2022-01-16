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
        private readonly AppConfigsModel _configModel;

        public Worker(IOptions<AppConfigsModel> appOptions)
        {
            _configModel = appOptions.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IOrdersArchiver dataHandler = new OrdersArchiver(_configModel, new FileInfoFactory(), new FileWatcher(_configModel.Folders));
            dataHandler.OnMessage += Messager.SendMessage;

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {


            return Task.CompletedTask;
        }
    }
}
