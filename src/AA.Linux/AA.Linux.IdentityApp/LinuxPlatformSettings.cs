using AA.Common;
using AA.Core.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AA.Linux.IdentityApp
{
    public class LinuxPlatformSettings : IPlatformSettings
    {
        public string HomePath => Environment.GetEnvironmentVariable("HOME") ?? @"C:\";
        public string ConfigurationPath =>Path.Combine("configuration.xml");
        public string IdentitiesPath => Path.Combine(HomePath, ManufacturerFolderName, ProductFolderName, "identities");
        public string ManufacturerFolderName => "BlackRidge Technology";
        public string ProductFolderName => "Identity Activation Agent";
        public string LogFolderPath => Path.Combine(HomePath, "BRLogs");
		public string PfxFileName => "IAA-pfx-file.pfx";

		public string LogName { get; }
        public string LogSourceName { get; }
	    public string PrivateKeyName { get; }
	    public string CertName { get; }
	    public string ChainName { get; }
	}
}
