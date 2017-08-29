namespace AA.Core.Identity
{
	public interface IPlatformSettings
	{
		string ConfigurationPath { get; }
		string IdentitiesPath { get; }
		string ManufacturerFolderName { get; }
		string ProductFolderName { get; }

		string LogFolderPath { get; }
		string LogName { get; }
		string LogSourceName { get; }

		string PrivateKeyName { get; }
		string CertName { get; }
		string ChainName { get; }

		//string DpapiProtectedPfxFileName { get; }
		//string LinuxPfxFileName { get; }

		string PfxFileName { get; }
	}
}
