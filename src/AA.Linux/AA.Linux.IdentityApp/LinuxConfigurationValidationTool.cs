using AA.Core.Identity;
using System;
using System.Collections.Generic;

namespace AA.Linux.IdentityApp
{
	class LinuxConfigurationValidationTool : ConfigurationValidationTool
    {
        public LinuxConfigurationValidationTool(Configuration configuration) : base(configuration)
        {
        }

        public override IEnumerable<ConfigurationFault> GetFaults()
        {
            if (Configuration == null || Configuration.IsLoaded == false)
                yield return new ConfigurationFault { IsFatal = true, Message = "Unable to load configuration." };

            if (Configuration?.GatewayEndPoint == null)
                yield return new ConfigurationFault { IsFatal = true, Message = "There are no configured gateways." };

            if (Configuration?.GatewayEndPoint != null && Configuration.GatewayEndPoint.UserBootstrapActivation)
                yield return new ConfigurationFault { IsFatal = true, Message = "Bootstrap is enabled." };

            if (string.IsNullOrEmpty(Configuration?.GatewayEndPoint?.Hostname) || Uri.CheckHostName(Configuration.GatewayEndPoint.Hostname) == UriHostNameType.Unknown)
                yield return new ConfigurationFault { IsFatal = true, Message = "Gateway Hostname is invalid." };

            if (Configuration?.IdentityActivationAgentCertificate == null)
                yield return new ConfigurationFault { IsFatal = true, Message = "Gateway X509Certificate2 cannot be loaded." };
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

		    if (Configuration?.BootstrapX509Chain == null)
			    yield return new ConfigurationFault
			    {
				    IsFatal = true,
				    Message = "Bootstrap Chain cannot be loaded." + Environment.NewLine +
				              $"Chain file location is {Configuration.BootstrapChainPath}"
			    };

		    if (Configuration?.BootstrapKey == null || Configuration.BootstrapKey.Length == 0)
			    yield return new ConfigurationFault
			    {
				    IsFatal = true,
				    Message = "Bootstrap Key cannot be loaded." + Environment.NewLine +
				              $"Key file location is {Configuration.BootstrapKeyPath}"
			    };
		}
    }
}
