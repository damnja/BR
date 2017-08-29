using AA.Core.Identity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AA.Windows.IdentityApp
{
	public class WindowsConfigurationValidationTool : ConfigurationValidationTool
	{
		public WindowsConfigurationValidationTool(Configuration configuration) : base(configuration)
		{
		}

		public override IEnumerable<ConfigurationFault> GetFaults()
		{
			if (Configuration == null || Configuration.IsLoaded == false)
				yield return new ConfigurationFault { IsFatal = true, Message = "Unable to load configuration" };

			if (Configuration?.GatewayEndPoint == null)
				yield return new ConfigurationFault { IsFatal = true, Message = "There are no configured gateways." };

			if (string.IsNullOrEmpty(Configuration?.GatewayEndPoint?.Hostname) || Uri.CheckHostName(Configuration.GatewayEndPoint.Hostname) == UriHostNameType.Unknown)
				yield return new ConfigurationFault { IsFatal = true, Message = "Gateway Host name is invalid." };

			if (Configuration?.IdentityActivationAgentCertificate == null)
				if (Configuration != null)
					yield return new ConfigurationFault
					{
						IsFatal = true,
						Message = "Gateway X509Certificate2 cannot be loaded." + Environment.NewLine +
												  $"X509Certificate2 file location is {Configuration.X509Certificate2Path}"
					};
		}

		public override IEnumerable<ConfigurationFault> GetBootstrapFaults()
		{
			if (Configuration == null || Configuration.IsLoaded == false)
				yield return new ConfigurationFault { IsFatal = true, Message = "Unable to load configuration" };

			if (Configuration?.GatewayEndPoint == null)
				yield return new ConfigurationFault { IsFatal = true, Message = "There are no configured gateways." };

			if (string.IsNullOrEmpty(Configuration?.GatewayEndPoint?.BootstrapService) || Uri.CheckHostName(Configuration.GatewayEndPoint.BootstrapService) == UriHostNameType.Unknown)
				yield return new ConfigurationFault { IsFatal = true, Message = "Gateway BootstrapService is invalid." };

			if (Configuration?.BootstrapX509Cert == null)
				yield return new ConfigurationFault
				{
					IsFatal = true,
					Message = "Bootstrap certificate cannot be loaded." + Environment.NewLine +
							  $"Certificate file location is {Configuration.BootstrapCertPath}"
				};

			if (Configuration?.BootstrapX509Cert != null && Configuration.BootstrapX509Cert.NotAfter <= DateTime.Now)
				yield return new ConfigurationFault
				{
					IsFatal = true,
					Message = "Bootstrap certificate has expired." + Environment.NewLine +
							   $"Bootstrap expiration date is {Configuration.BootstrapX509Cert.NotAfter} " + Environment.NewLine +
							   $"Certificate file location is {Configuration.BootstrapCertPath}"
				};
			else if (Configuration?.BootstrapX509Cert != null && Configuration.BootstrapX509Cert.NotAfter <= DateTime.Now.AddMonths(1))
			{
				yield return new ConfigurationFault
				{
					IsFatal = false,
					Message = "Bootstrap certificate will expired within one month." + Environment.NewLine +
							   $"Bootstrap expiration date is {Configuration.BootstrapX509Cert.NotAfter} " + Environment.NewLine +
							   $"Certificate file location is {Configuration.BootstrapCertPath}"
				};
			}

			if (Configuration?.BootstrapX509Cert != null && Configuration.BootstrapX509Cert.NotAfter <= DateTime.Now)
				yield return new ConfigurationFault
				{
					IsFatal = true,
					Message = "Bootstrap certificate has expired." + Environment.NewLine +
							   $"Bootstrap expiration date is {Configuration.BootstrapX509Cert.NotAfter} " + Environment.NewLine +
							   $"Certificate file location is {Configuration.BootstrapCertPath}"
				};

			if (Configuration?.BootstrapX509Chain == null)
				yield return new ConfigurationFault
				{
					IsFatal = true,
					Message = "Bootstrap Chain cannot be loaded." + Environment.NewLine +
							  $"Chain file location is {Configuration.BootstrapChainPath}"
				};

			if (Configuration?.BootstrapKey == null || Configuration?.BootstrapKey.Length == 0)
				yield return new ConfigurationFault
				{
					IsFatal = true,
					Message = "Bootstrap Key cannot be loaded." + Environment.NewLine +
							  $"Key file location is {Configuration.BootstrapKeyPath}"
				};

			//if(Configuration?.BootstrapCert != null && Configuration?.BootstrapChain != null)
			//{
			//	var valid = ValidateCertificateByCA();
			//}
		}

		private new bool ValidateCertificateByCA()
		{
			X509Chain chain = new X509Chain();
			//chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
			//chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
			chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
			chain.ChainPolicy.VerificationTime = DateTime.Now;
			chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);

			// This part is very important. You're adding your known root here.
			// It doesn't have to be in the computer store at all. Neither certificates do.
			chain.ChainPolicy.ExtraStore.Add(Configuration.BootstrapX509Chain);

			bool isChainValid = chain.Build(Configuration.BootstrapX509Cert);
			return isChainValid;
		}
	}
}
