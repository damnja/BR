using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AA.Core.Identity
{
	/// <summary>
	/// Represents error in configuration.xml
	/// </summary>
    public struct ConfigurationFault
    {
        public string Message { get; set; }
        public bool IsFatal { get; set; }
    }

    public abstract class ConfigurationValidationTool
    {
        protected readonly Configuration Configuration;

        protected ConfigurationValidationTool(Configuration configuration)
        {
            Configuration = configuration;
        }

		protected bool ValidateCertificateByCA()
		{
			X509Chain chain = new X509Chain();
			chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
			chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
			chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
			chain.ChainPolicy.VerificationTime = DateTime.Now;
			chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);

			// This part is very important. You're adding your known root here.
			// It doesn't have to be in the computer store at all. Neither certificates do.
			chain.ChainPolicy.ExtraStore.Add(Configuration.BootstrapX509Chain);

			bool isChainValid = chain.Build(Configuration.BootstrapX509Cert);
			return isChainValid;
		}

		/// <summary>
		/// Find all errors in configuration.xml
		/// like missing required data
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<ConfigurationFault> GetFaults();

	    /// <summary>
	    /// Find all errors in configuration.xml related to Bootstrap
	    /// like missing required data
	    /// </summary>
	    /// <returns></returns>
		public abstract IEnumerable<ConfigurationFault> GetBootstrapFaults();
    }
}
