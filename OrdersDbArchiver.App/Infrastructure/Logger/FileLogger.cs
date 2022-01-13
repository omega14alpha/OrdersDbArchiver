using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace OrdersDbArchiver.App.Infrastructure.Logger
{
    internal class FileLogger : ILogger
    {
        private readonly string _filePath;

        private static object _lock = new object();

        public FileLogger(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException();
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    File.AppendAllText(_filePath, formatter(state, exception) + Environment.NewLine);
                }
            }
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
