using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AA.Core.Common;

namespace AA.Core.Identity
{
	public abstract class Logger
	{
		protected IPlatformSettings PlatformSetting;
		protected Logger(IPlatformSettings platformSettings)
		{
			PlatformSetting = platformSettings;
		}

		protected abstract Task Log(LogLevel level, string message, IDictionary<string, string> data);

		public Task Debug(string message, IDictionary<string, string> data = null) => Log(LogLevel.Debug, message, data ?? new Dictionary<string, string>());
		public Task Info(string message, IDictionary<string, string> data = null) => Log(LogLevel.Info, message, data ?? new Dictionary<string, string>());
		public Task Warn(string message, IDictionary<string, string> data = null) => Log(LogLevel.Warn, message, data ?? new Dictionary<string, string>());
		public Task Error(string message, IDictionary<string, string> data = null) => Log(LogLevel.Error, message, data ?? new Dictionary<string, string>());
	}
}
