using AA.Common;
using AA.Core.Identity;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace AA.Linux.IdentityApp
{
	public class LinuxConfiguration : Configuration
	{
		public LinuxConfiguration(IPlatformSettings platformSettings, Logger logger) : base(platformSettings, logger)
		{
		}

		public override X509Certificate2 LoadIdentityActivationAgentCertificates()
		{
			try
			{
				X509Certificate2 certificate =	new X509Certificate2(X509Certificate2Path);

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
			}
			catch
			{
				return null;
			}
		}
	}
}
