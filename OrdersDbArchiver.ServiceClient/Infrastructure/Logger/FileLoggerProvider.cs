using Microsoft.Extensions.Logging;

namespace OrdersDbArchiver.ServiceClient.Infrastructure.Logger
{
    internal class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;

        public FileLoggerProvider(string filePath)
        {
            _filePath = filePath;
        }

        public ILogger CreateLogger(string categoryName) => new FileLogger(_filePath);

        public void Dispose()
        {
        }
    }
}
