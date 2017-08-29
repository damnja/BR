using AA.Core.Identity;
using DryIoc;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using AA.Core.Common;

namespace AA.Windows.IdentityApp
{
	public static class Program
	{

		/// <summary>
		/// Preparing Dry inversion of Control
		/// Mapping windows classes to base standard classes
		/// </summary>
		/// <returns></returns>
		private static Container PrepareIoC()
		{
			var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

			container.Register<IPlatformSettings, WindowsPlatformSettings>(Reuse.Singleton);
			container.Register<Logger>(made: Made.Of(() => new WindowsEventLogger(Arg.Of<IPlatformSettings>())), reuse: Reuse.Singleton);
			container.Register<Configuration>(made: Made.Of(() => new WindowsConfiguration(Arg.Of<IPlatformSettings>(), Arg.Of<Logger>())), reuse: Reuse.Singleton);
			container.Register<ConfigurationValidationTool>(made: Made.Of(() => new WindowsConfigurationValidationTool(Arg.Of<Configuration>())), reuse: Reuse.Singleton);
			container.Register<KeyProvider, WindowsKeyProvider>(Reuse.Singleton);
			container.Register<IapInterface, WindowsIapInterface>(Reuse.Singleton);
			container.Register<BrTokenInterface, WindowsBrTokenInterface>(Reuse.Singleton);

			return container;
		}

		/// <summary>
		/// Entry point for Windows application
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			var container = PrepareIoC();

			if (!IsAdmin())
			{
				RequstAdmin(container, args);
			}
			else
			{
				using (var iaaService = new IaaService(container))
				{
					Microsoft.Win32.SystemEvents.SessionEnding += iaaService.SystemEvents_SessionEnding;
					iaaService.Start();
				}
			}
		}

		private static void RequstAdmin(Container container, string[] args)
		{
			var logger = container.Resolve<Logger>();
			try
			{
				ProcessStartInfo proc = new ProcessStartInfo();
				proc.UseShellExecute = true;
				proc.WorkingDirectory = Environment.CurrentDirectory;
				proc.FileName = Assembly.GetEntryAssembly().CodeBase;

				foreach (string arg in args)
				{
					proc.Arguments += String.Format("\"{0}\" ", arg);
				}

				proc.Verb = "runas";


				Process.Start(proc);
			}
			catch
			{
				logger.Error("This application requires elevated credentials in order to operate correctly!");
			}
		}

		private static bool IsAdmin()
		{
			WindowsIdentity id = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new WindowsPrincipal(id);

			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}
	}
}
