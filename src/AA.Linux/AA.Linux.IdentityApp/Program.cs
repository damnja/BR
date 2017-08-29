using AA.Common;
using AA.Core.Identity;
using DryIoc;
using System.Threading.Tasks;


namespace AA.Linux.IdentityApp
{
	public class Program
	{
		private static Container PrepareIoC()
		{
			var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

			container.Register<IPlatformSettings, LinuxPlatformSettings>(Reuse.Singleton);
			container.Register<Logger>(made: Made.Of(() => new LinuxSysLogger(Arg.Of<IPlatformSettings>())), reuse: Reuse.Singleton);
			container.Register<Configuration>(made: Made.Of(() => new LinuxConfiguration(Arg.Of<IPlatformSettings>(), Arg.Of<Logger>())), reuse: Reuse.Singleton);
			container.Register<ConfigurationValidationTool>(made: Made.Of(() => new LinuxConfigurationValidationTool(Arg.Of<Configuration>())), reuse: Reuse.Singleton);
			container.Register<KeyProvider, LinuxKeyProvider>(Reuse.Singleton);
			container.Register<IapInterface, LinuxIapInterface>(Reuse.Singleton);
			
			return container;
		}

		public static void Main(string[] args)
		{
			Task.Run(() =>
			{
				var container = PrepareIoC();

				using (var iaa = new IAAService(container))
				{
					iaa.Start();
				}
			}).Wait();
		}
	}
}