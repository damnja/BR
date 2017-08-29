using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AA.Core.Identity;

namespace AA.Windows.IdentityApp
{
	public sealed class WindowsEventLogger : Logger
	{
		public WindowsEventLogger(IPlatformSettings platformSettings) : base(platformSettings)
		{
			if (!EventLog.SourceExists(platformSettings.LogSourceName))
				EventLog.CreateEventSource(platformSettings.LogSourceName, platformSettings.LogName);
		}

		/// <summary>
		/// Maps LogLevel to EventLogLevel
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private EventLogEntryType ToEventLogEntryType(AA.Core.Common.LogLevel value)
		{
			switch (value)
			{
				case AA.Core.Common.LogLevel.Debug:
					return EventLogEntryType.SuccessAudit;
				case AA.Core.Common.LogLevel.Info:
					return EventLogEntryType.Information;
				case AA.Core.Common.LogLevel.Warn:
					return EventLogEntryType.Warning;
				case AA.Core.Common.LogLevel.Error:
					return EventLogEntryType.Error;
				default:
					return EventLogEntryType.FailureAudit;
			}
		}

		/// <summary>
		/// Logs message to Event log
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		protected override Task Log(AA.Core.Common.LogLevel level, string message, IDictionary<string, string> data)
		{
			var logMessage = new StringBuilder(message);
			if (data.Any())
			{
				logMessage.Append(Environment.NewLine);
				logMessage.Append("Additional Info:");
				foreach (var info in data)
				{
					logMessage.Append(Environment.NewLine);
					logMessage.Append($"{info.Key} : {info.Value} ");
				}
			}


			return Task.Run(() =>
			{
				EventLog.WriteEntry(PlatformSetting.LogSourceName, logMessage.ToString(), ToEventLogEntryType(level), 1000);
			});
		}
	}
}
