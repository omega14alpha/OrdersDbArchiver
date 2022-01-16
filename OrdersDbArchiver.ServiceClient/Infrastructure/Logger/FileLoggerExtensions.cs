using Microsoft.Extensions.Logging;

namespace OrdersDbArchiver.ServiceClient.Infrastructure.Logger
{
    internal static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder factory, string filePath)
        {
            factory.AddProvider(new FileLoggerProvider(filePath));
            return factory;
        }
    }
}
