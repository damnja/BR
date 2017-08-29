using AA.Core.Common;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Linq;

namespace AA.Core.Identity
{
	public abstract class Configuration
	{
		public IPlatformSettings PlatformSettings;
		protected Logger Logger;
		protected XmlNameTable NameTable { get; private set; }
		protected XElement Root { get; private set; }
		public GatewayEndPoint GatewayEndPoint { get; private set; }
		public X509Certificate2 IdentityActivationAgentCertificate { get; private set; }
		public bool IsLoaded => Root != null;

		public uint IP => Helpers.GetAddress(GatewayEndPoint.BootstrapService);
		public uint IpNew => Helpers.Ip2UInt(GatewayEndPoint.BootstrapService);

		public virtual string X509Certificate2Path => Path.Combine(PlatformSettings.IdentitiesPath, PlatformSettings.PfxFileName);
		public virtual string BootstrapCertPath => Path.Combine(PlatformSettings.IdentitiesPath, PlatformSettings.CertName);
		public virtual string BootstrapChainPath => Path.Combine(PlatformSettings.IdentitiesPath, PlatformSettings.ChainName);
		public virtual string BootstrapKeyPath => Path.Combine(PlatformSettings.IdentitiesPath, PlatformSettings.PrivateKeyName);

		public byte[] BootstrapCert { get; private set; }
        public byte[] BootstrapChain { get; private set; }
        public byte[] BootstrapKey { get; private set; }

        public X509Certificate2 BootstrapX509Cert { get; private set; }
		public X509Certificate2 BootstrapX509Chain { get; private set; }
		
        public abstract X509Certificate2 LoadIdentityActivationAgentCertificates();
		public abstract byte[] ReadFileData(string path);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="platformSettings"></param>
        /// <param name="logger"></param>
        protected Configuration(IPlatformSettings platformSettings, Logger logger)
		{
			PlatformSettings = platformSettings;
			Logger = logger;
			var fullConfigurationPath = Path.Combine(System.AppContext.BaseDirectory, PlatformSettings.ConfigurationPath);
			LoadConfiguration(fullConfigurationPath);
		}

		/// <summary>
		/// Read properties from IAA.Configuration.xml, and fill the properties
		/// </summary>
		/// <param name="path"></param>
		private void LoadConfiguration(string path)
		{
			try
			{
				using (var stream = File.OpenRead(path))
				using (var textReader = new StreamReader(stream, true))
				using (var reader = XmlReader.Create(textReader))
				{
					NameTable = reader.NameTable;
					Root = XElement.Load(reader, LoadOptions.SetLineInfo);
					GatewayEndPoint = GetGatewayEndpoint();

					IdentityActivationAgentCertificate = LoadIdentityActivationAgentCertificates();
                    LoadBootstrapCertificates();
				}
			}
			catch (Exception e)
			{
				Logger.Error($"Error while reading configuration file. Trying to find configuration file on {path}", e.FormLogEntry());
			}
		}

		/// <summary>
		/// Read Gateway end point from Configuration.xml
		/// </summary>
		private GatewayEndPoint GetGatewayEndpoint()
		{
			int intValue;
			ushort ushortValue;
			bool boolValue;
			var gatewayEl = Root?.Element(XName.Get(Constants.GatewayEndPoint, Constants.Ns));
			var gatewayInitializeEl = gatewayEl?.Element(XName.Get(Constants.GatewayInitialize, Constants.Ns));
			var gatewayRefreshEl = gatewayEl?.Element(XName.Get(Constants.GatewayRefresh, Constants.Ns));
			var gateway = gatewayEl == null
				? null
				: new GatewayEndPoint
				{
					UserActivation = !bool.TryParse(gatewayEl.Element(XName.Get(Constants.UserActivation, Constants.Ns))?.Value ?? "", out boolValue) || boolValue,
					GatewayInitialize = new GatewayInitialize
					{
						InitializeUrl = gatewayInitializeEl?.Attribute(Constants.GatewayInitializeUrl)?.Value,
						InitializeRetryInterval = (gatewayInitializeEl?.Attribute(Constants.GatewayInitializeRetryInterval)?.Value ?? "").ToTimeSpan(TimeSpan.FromSeconds(10)),
						MaximumRetries = int.TryParse(gatewayInitializeEl?.Attribute(Constants.GatewayInitializeMaximumReties)?.Value ?? "", out intValue)
							? intValue : 5,
					},
					GatewayRefresh = gatewayRefreshEl == null ? null : new GatewayRefresh
					{
						RefreshUrl = gatewayRefreshEl.Attribute(Constants.GatewayRefreshUrl)?.Value,
						RefreshInterval = (gatewayRefreshEl.Attribute(Constants.GatewayRefreshInterval)?.Value ?? "").ToTimeSpan(TimeSpan.FromSeconds(10)),
					},
					Hostname = gatewayEl.Element(XName.Get(Constants.HostName, Constants.Ns))?.Value,
					Port = ushort.TryParse(gatewayEl.Element(XName.Get(Constants.Port, Constants.Ns))?.Value ?? "", out ushortValue)
						? ushortValue : Constants.PortDef,
					UseSsl = !bool.TryParse(gatewayEl.Element(XName.Get(Constants.UseSsl, Constants.Ns))?.Value ?? "", out boolValue) || boolValue,
					VerifySsl = !bool.TryParse(gatewayEl.Element(XName.Get(Constants.EnableSslVerification, Constants.Ns))?.Value ?? "", out boolValue) || boolValue,
					UserBootstrapActivation = bool.TryParse(gatewayEl.Element(XName.Get(Constants.UserBootstrapActivation, Constants.Ns))?.Value ?? "", out boolValue) && boolValue,
					BootstrapService = gatewayEl.Element(XName.Get(Constants.BootstrapService, Constants.Ns))?.Value,
					BootstrapRetries = int.TryParse(gatewayInitializeEl?.Attribute(Constants.BootstrapRetries)?.Value ?? "", out intValue)
							? intValue : 5,
				};

			return gateway;
		}

		/// <summary>
		/// Read bootstrap certificates in needed formats
		/// </summary>
		private void LoadBootstrapCertificates()
		{
			BootstrapCert = ReadFileData(BootstrapCertPath);
			BootstrapKey = ReadFileData(BootstrapKeyPath);
			BootstrapChain = ReadFileData(BootstrapChainPath);

			BootstrapX509Cert = LoadBootstrapX509Value(BootstrapCert);
			BootstrapX509Chain = LoadBootstrapX509Value(BootstrapChain);
		}


		/// <summary>
		/// Load X509Certificate2 from byte[]
		/// </summary>
		/// <param name="certInfo"></param>
		/// <returns></returns>
		public X509Certificate2 LoadBootstrapX509Value(byte[] certInfo)
		{
			try
			{
				return new X509Certificate2(certInfo);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Get CN property of configuration
		/// </summary>
		public string CN => IdentityActivationAgentCertificate?.GetNameInfo(X509NameType.SimpleName, false);

		/// <summary>
		/// Get SHA256 of configuration
		/// </summary>
		public string CertificateHash
		{
			get
			{
				using (var hasher = SHA256.Create())
				{
					var hash = hasher.ComputeHash(IdentityActivationAgentCertificate.RawData);
					return BitConverter.ToString(hash).Replace("-", "").ToLower();
				}
			}
		}
	}
}
