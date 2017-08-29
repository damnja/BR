using System;
using System.IO;
using AA.Core.Identity;

namespace AA.Windows.IdentityApp
{
	public sealed class WindowsPlatformSettings : IPlatformSettings
	{
		public string ConfigurationPath => ("AAConfiguration.xml");
		public string IdentitiesPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ManufacturerFolderName, "identities");
		public string ManufacturerFolderName => "BlackRidge Technology";
		public string ProductFolderName => "Identity Activation Agent";
		public string PfxFileName => "IAA-DPAPI-pfx-file.bin";
		public string LogFolderPath { get; }
		public string LogName => "Application";
		public string LogSourceName => "IAAService";
		public string PrivateKeyName => "PrivateKey.key";
		public string CertName => "Cert.crt";
		public string ChainName => "Chain.txt";
	}
}
