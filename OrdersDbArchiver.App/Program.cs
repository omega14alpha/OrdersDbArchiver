using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrdersDbArchiver.App.Infrastructure.Logger;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using System;
using System.IO;

namespace OrdersDbArchiver.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, builder) =>
            {
                builder.AddJsonFile("appsettings.json", true, true);
            })
            .ConfigureLogging(loggerFactory =>
            {
                string loggerFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logger.txt");
                loggerFactory.AddFile(loggerFilePath);
            })
            .UseWindowsService(options =>
            {
                options.ServiceName = "OrdersDbArchiverService";
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
                services.Configure<AppConfigsModel>(hostContext.Configuration.GetSection("AppConfig"));
            });
    }
}
