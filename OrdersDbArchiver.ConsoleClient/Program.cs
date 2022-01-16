using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using OrdersDbArchiver.ConsoleClient.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrdersDbArchiver.ConsoleClient
{
    internal class Program
    {
        private static CancellationTokenSource _cancellationTokenSource;

        static Task Main(string[] args)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;
            Messager.OnGetMessage += (sender, args) => Console.WriteLine(args.Message);
            WaitCloseApp();

            var builder = new HostBuilder()
                  .ConfigureHostConfiguration(config =>
                  {
                      config.AddCommandLine(args);
                  })
                  .ConfigureAppConfiguration((hostContext, builder) =>
                  {
                      builder.AddJsonFile("appsettings.json", true, true);
                      builder.AddEnvironmentVariables();
                      if (args != null)
                      {
                          builder.AddCommandLine(args);
                      }
                  })
                  .ConfigureServices((hostContext, services) =>
                  {
                      services.AddHostedService<Worker>(); 
                      services.Configure<AppConfigsModel>(hostContext.Configuration.GetSection("AppConfig"));
                  });

            return builder.RunConsoleAsync(cancellationToken);
        }

        private static Task WaitCloseApp()
        {
            return Task.Factory.StartNew(() =>
            {
                if (Console.Read() == 'q')
                {
                    Console.WriteLine("IsCancel");
                    _cancellationTokenSource.Cancel();
                }
            });
        }
    }
}
