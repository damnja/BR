using AA.Core.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AA.Windows.IdentityApp
{
	public sealed class WindowsFileLogger : Logger
	{
		public WindowsFileLogger(IPlatformSettings platformSettings) : base(platformSettings)
		{
		}

		protected override Task Log(AA.Core.Common.LogLevel level, string message, IDictionary<string, string> data) => throw new NotImplementedException();
	}
}
