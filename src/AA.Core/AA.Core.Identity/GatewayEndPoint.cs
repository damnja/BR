using System;
using System.Net;
using System.Threading.Tasks;

namespace AA.Core.Identity
{
	/// <summary>
	/// Represents the GatewayInitialize section in GatewayEndPoint in Configuration.xml
	/// </summary>
	public class GatewayInitialize
	{
		public string InitializeUrl { get; set; }
		public TimeSpan InitializeRetryInterval { get; set; }
		public int MaximumRetries { get; set; }
	}

	/// <summary>
	/// Represents the GatewayRefresh section in GatewayEndPoint in Configuration.xml
	/// </summary>
	public class GatewayRefresh
	{
		public string RefreshUrl { get; set; }
		public TimeSpan RefreshInterval { get; set; }
	}

	/// <summary>
	/// Represents the GatewayEndPoint section in Configuration.xml
	/// </summary>
	public class GatewayEndPoint
	{
		public GatewayInitialize GatewayInitialize { get; set; }
		public GatewayRefresh GatewayRefresh { get; set; }
	   
		public bool UserActivation { get; set; }
		public bool UserBootstrapActivation { get; set; }

		public string Hostname { get; set; }
		public string BootstrapService { get; set; }
		public int BootstrapRetries { get; set; }

		public ushort Port { get; set; }
		public bool UseSsl { get; set; }
		public bool VerifySsl { get; set; }
		
		public Task<IPAddress[]> GetAddresses() => Dns.GetHostAddressesAsync(Hostname);

		public Uri InitializeUri
		{
			get
			{
				UriBuilder uri = new UriBuilder(UseSsl ? "https" : "http", Hostname, Port)
				{
					Path = $"{GatewayInitialize?.InitializeUrl?.TrimEnd('/')}"
				};
				return uri.Uri;
			}
		}

		public Uri RefreshUri
		{
			get
			{
				UriBuilder uri = new UriBuilder(UseSsl ? "https" : "http", Hostname, Port)
				{
					Path = $"{GatewayRefresh?.RefreshUrl?.TrimEnd('/')}"
				};
				return uri.Uri;
			}
		}
	}

	/// <summary>
	/// Represents the BootstrapData in Configuration.xml
	/// </summary>
	public class BootstrapData
	{
		public string BootstrapGateway { get; set; }
		public int BootstrapRetries { get; set; }
	}
}
