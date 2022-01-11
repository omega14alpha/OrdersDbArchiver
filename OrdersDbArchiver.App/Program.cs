using Microsoft.Extensions.Configuration;
using OrdersDbArchiver.BusinessLogicLayer;
using System;

namespace OrdersDbArchiver.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            var config = builder.SetBasePath(Environment.CurrentDirectory).AddJsonFile("appsettings.json").Build();
            OrdersArchiver dataHandler = new OrdersArchiver(config["ConnectionStrings:OrderArhiverConnection"], config["Folders:ObservedFolder"], config["Folders:TargetFolder"]);
            dataHandler.Start();
        }
    }
}
