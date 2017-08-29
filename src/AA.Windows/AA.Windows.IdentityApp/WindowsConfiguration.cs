using AA.Core.Identity;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AA.Windows.IdentityApp
{
	public sealed class WindowsConfiguration : Configuration
	{
		public WindowsConfiguration(IPlatformSettings platformSettings, Logger logger) : base(platformSettings, logger)
		{
		}

		public override X509Certificate2 LoadIdentityActivationAgentCertificates()
		{
			try
			{
				X509Certificate2 certificate =
					new X509Certificate2(ProtectedData.Unprotect(File.ReadAllBytes(X509Certificate2Path), new byte[] { },
						DataProtectionScope.CurrentUser));

				return certificate;
			}
			catch
			{
				return null;
			}
		}

		public override byte[] ReadFileData(string path)
		{
			try
			{
				return File.ReadAllBytes(path);

				//return ProtectedData.Unprotect(File.ReadAllBytes(BootstrapKeyPath), new byte[] { },
				//		DataProtectionScope.CurrentUser));
			}
			catch
			{
				return null;
			}
		}
	}
}
