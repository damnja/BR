using System;
using System.Collections.Generic;
using System.Text;

namespace AA.Core.Common
{
	public static class Constants
	{
		public const int DefaultMaxNameLen = 64;
		public const int MaxKeyName = 63;

		//configuration properties
		public const string Ns = "http://www.blackridge.us/XMLSchema/IAAConfiguration/v1.0";
		public const string GatewayEndPoint = "GatewayEndPoint";

		public const string UserActivation = "UserActivation";

		//GW initialize
		public const string GatewayInitialize = "GatewayInitialize";
		public const string GatewayInitializeUrl = "url";
		public const string GatewayInitializeMaximumReties = "maximum-retries";
		public const string GatewayInitializeRetryInterval = "retry-interval";

		//GW refresh
		public const string GatewayRefresh = "GatewayRefresh";
		public const string GatewayRefreshUrl = "url";
		public const string GatewayRefreshInterval = "refresh-interval";

		public const string HostName = "Hostname";
		public const string Port = "Port";
		public const ushort PortDef = 8443;
		public const string UseSsl = "UseSsl";
		public const string EnableSslVerification = "VerifySsl";

		public const string UserBootstrapActivation = "UserBootstrapActivation";
		public const string BootstrapService = "BootstrapService";
		public const string BootstrapRetries = "BootstrapRetries";


		//Gateway
		public const string GatewayInitializeRequestStatus = "Healthy";

		//Error messages
		public const string ErrorProcessingCertificate = "An unknown error occurred while processing the certificate";
		public const string ErrorSecurity = "A security error occurred";

		//Driver
		public const int RetryTacDriverValue = 2;
		public const int DelayTacDriverValue = 2;
	}
}
