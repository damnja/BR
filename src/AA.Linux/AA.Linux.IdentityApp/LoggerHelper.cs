using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AA.Linux.IdentityApp
{
    public sealed class LoggerHelper
    {
        private static ILoggerFactory _loggerFactory;

        public static void ConfigureLoggerOutput(string path)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine(path, "log-IAALInux.txt"))
                .CreateLogger();
        }

        private static void ConfigureLogger()
        {
            if (_loggerFactory == null)
                _loggerFactory = new LoggerFactory()
                    .AddSerilog();
        }

        public static Microsoft.Extensions.Logging.ILogger GetLogger()
        {
            ConfigureLogger();

            return _loggerFactory.CreateLogger("AppLogger");
        }
    }
}
