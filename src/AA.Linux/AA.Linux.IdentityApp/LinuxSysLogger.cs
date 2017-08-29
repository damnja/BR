using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AA.Core.Identity;
using AA.Common;

namespace AA.Linux.IdentityApp
{
    public class LinuxSysLogger : Logger
    {
        private static ILogger _logger;

        public LinuxSysLogger(IPlatformSettings platformSettings) : base(platformSettings)
        {
            ConfigureLogger(platformSettings.LogFolderPath);
        }

        private void ConfigureLogger(string loggerPath)
        {
            LoggerHelper.ConfigureLoggerOutput(loggerPath);
            _logger = LoggerHelper.GetLogger();
        }

        private void LogMessage(Common.LogLevel logLevelParam, string message)
        {
            switch (logLevelParam)
            {
                case Common.LogLevel.Debug:
                    _logger.LogDebug(message);
                    break;
                case Common.LogLevel.Error:
                    _logger.LogError(message);
                    break;
                case Common.LogLevel.Info:
                    _logger.LogInformation(message);
                    break;
                case Common.LogLevel.Warn:
                    _logger.LogWarning(message);
                    break;
            }
        }

        protected override Task Log(Common.LogLevel level, string message, IDictionary<string, string> data)
        {
            var logMessage = new StringBuilder(message);
            if (data.Any())
            {
                logMessage.Append(Environment.NewLine);
                logMessage.Append($"Time: {DateTime.UtcNow}");
                logMessage.Append(Environment.NewLine);
                logMessage.Append("Additional Info:");
                foreach (var info in data)
                {
                    logMessage.Append(Environment.NewLine);
                    logMessage.Append($"{info.Key}: {info.Value}");
                }
            }

            return Task.Run(() =>
            {
                //Console.WriteLine($"Level: {level}, info: {logMessage.ToString()}");
                LogMessage(level, logMessage.ToString());
            });
        }
    }
}
