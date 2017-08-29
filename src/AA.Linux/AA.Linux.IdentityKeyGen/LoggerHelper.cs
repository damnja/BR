using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog;

namespace AA.Linux.IdentityKeyGen
{ 
	public sealed class LoggerHelper
    {
        private static ILoggerFactory _loggerFactory;
        //private static Microsoft.Extensions.Logging.ILogger _logger;

        public static void ConfigureLoggerOutput(string path)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.RollingFile(Path.Combine(path, "log-KeyGenerator.txt"))
            .CreateLogger();
        }

        private static void ConfigureLogger()
        {
            if(_loggerFactory==null)
                _loggerFactory=new LoggerFactory()
                    .AddSerilog();
        }

        public static Microsoft.Extensions.Logging.ILogger GetLogger()
        {
            ConfigureLogger();

            return _loggerFactory.CreateLogger("AppLogger"); 
        }
    }
}
