using AA.Core.Identity;
using AA.Windows.IdentityApp;
using DryIoc;
using System;
using System.IO;
using System.Security.Cryptography;

namespace AA.Windows.IdentityKeyGen
{
	class Program
	{
		private static Container _container;
		private static IPlatformSettings _platformSettings;


		private static Container PrepareIoC()
		{
			var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

			container.Register<IPlatformSettings, WindowsPlatformSettings>(Reuse.Singleton);
			return container;
		}

		private static void Initialize()
		{
			try
			{
				_container = PrepareIoC();
				_platformSettings = _container.Resolve<IPlatformSettings>();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		/// <summary>
		/// Start application as a console program
		/// </summary>
		private static void StartConsole(string[] args, out bool isConsoleApplicable)
		{
			try
			{
				isConsoleApplicable = true;
			}
			catch
			{
				isConsoleApplicable = false;
				return;
			}

			try
			{
				WindowsCommandConfiguration commandConfiguration;
				if (WindowsCommandConfiguration.TryCreate(args, out commandConfiguration) == false)
				{
					return;
				}

				// if requested, create a certificate signing request on demand.
				if (commandConfiguration.IsCSRRequested)
				{
					CreateCertificateSigningRequest(commandConfiguration.CertificateSigningRequestKeySize, commandConfiguration.CertificateSigningRequestPrimalityCertainty);
					return;
				}

				// if requested, encrypt .pfx file and save it under /AppData/Roaming/BlackRidge Technology/Identity Activation Agent/identities folder
				if (commandConfiguration.IsEncryptPfxRequested)
				{
					if (!String.IsNullOrEmpty(commandConfiguration.PathToThePfxFile))
					{
						if (File.Exists(commandConfiguration.PathToThePfxFile))
							EncryptAndImportPfxFile(commandConfiguration.PathToThePfxFile);
						else
						{
							Console.WriteLine("Path to the " + commandConfiguration.PathToThePfxFile + " file is not valid");
						}
					}
					else
					{
						Console.WriteLine("You must enter path value for the .pfx file");
					}
					return;
				}

				// if requested, move private key, cert file and chain file to /AppData/Roaming/BlackRidge Technology/identities folder
				if (commandConfiguration.IsBootstrapMoveKeysRequested)
				{
					if (String.IsNullOrEmpty(commandConfiguration.PathToPrivateKeyFile))
					{
						Console.WriteLine("You must enter path value for the private key file");
						return;
					}

					if (String.IsNullOrEmpty(commandConfiguration.PathToCertFile))
					{
						Console.WriteLine("You must enter path value for the cert file");
						return;
					}

					if (String.IsNullOrEmpty(commandConfiguration.PathToChainFile))
					{
						Console.WriteLine("You must enter path value for the chain file");
						return;
					}

					CreateIdentitiesFolder();
					MoveFileToIdentitiesFolder(commandConfiguration.PathToPrivateKeyFile, _platformSettings.PrivateKeyName);
					MoveFileToIdentitiesFolder(commandConfiguration.PathToCertFile, _platformSettings.CertName);
					MoveFileToIdentitiesFolder(commandConfiguration.PathToChainFile, _platformSettings.ChainName);

					return;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An uncaught exception ({ex.GetType().FullName}) was encountered: {ex.Message}");
				Console.WriteLine(ex.StackTrace);
			}
		}

		/// <summary>
		/// Encrypts pfx file and saves it under /AppData/Roaming/BlackRidge Technology/identities folder"
		/// </summary>
		private static void CreateIdentitiesFolder()
		{
			try
			{
				if (!Directory.Exists(_platformSettings.IdentitiesPath))
				{
					Directory.CreateDirectory(_platformSettings.IdentitiesPath);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		/// <summary>
		/// Move file file under /AppData/Roaming/BlackRidge Technology/identities folder"
		/// </summary>
		private static void MoveFileToIdentitiesFolder(string filePath, string platformIBAFileName)
		{
			try
			{
				if (File.Exists(filePath))
				{
					if (
						File.Exists(Path.Combine(_platformSettings.IdentitiesPath, platformIBAFileName)) == false
						|| Prompt($"File " + platformIBAFileName + " already exists. Overwrite " + platformIBAFileName + "? If so, type YES in all capital letters: ") == "YES")
					{

						//File.WriteAllBytes(Path.Combine(_platformSettings.IdentitiesPath, platformIBAFileName)
						//                   , ProtectedData.Protect(File.ReadAllBytes(filePath), new byte[] { },
						//                    DataProtectionScope.CurrentUser)
						//                   );

						File.WriteAllBytes(Path.Combine(_platformSettings.IdentitiesPath, platformIBAFileName)
										   , File.ReadAllBytes(filePath)
										   );

						Console.WriteLine("File " + platformIBAFileName + " is created at the location: " + _platformSettings.IdentitiesPath);
					}
				}
				else
					Console.WriteLine("Path " + filePath + " is not valid");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}


		/// <summary>
		/// Creates a certificate signing request.
		/// </summary>
		private static void CreateCertificateSigningRequest(int keySize, int primalityCertainty)
		{
			// create CSR
			var csr = new WindowsCertificateSigningRequest
			{
				Country = Prompt("Enter your country: "),
				State = Prompt("Enter your state: "),
				Location = Prompt("Enter your location: "),
				Organization = Prompt("Enter your organization: "),
				OrganizationalUnit = Prompt("Enter your organizational unit: "),
				KeySize = keySize,
				PrimalityCertainty = primalityCertainty
			};

			// set common name.
			csr.CommonName = Environment.MachineName;
			var newCommonName = Prompt($"Enter the common name (press enter for default: '{csr.CommonName}'): ", true);
			if (string.IsNullOrWhiteSpace(newCommonName) == false)
			{
				csr.CommonName = newCommonName;
			}

			// create the CSR.
			csr.Create();

			// save the CSR.
			var requestFile = "request.csr";
			Console.WriteLine($"CSR (writing to {requestFile}): ");
			Console.WriteLine(csr.Request);
			if (File.Exists(requestFile) == false || Prompt($"Overwrite '{requestFile}'? If so, type YES in all capital letters: ") == "YES")
			{
				File.WriteAllText(requestFile, csr.Request);
			}

			// save the key file.
			var keyFile = "request.key";
			Console.WriteLine($"Private Key (writing to {keyFile}): ");
			Console.WriteLine(csr.Key);
			if (File.Exists(keyFile) == false || Prompt($"Overwrite '{keyFile}'? If so, type YES in all capital letters: ") == "YES")
			{
				File.WriteAllText(keyFile, csr.Key);
			}
		}

		/// <summary>
		/// Encrypts pfx file and saves it under /AppData/Roaming/BlackRidge Technology/Identity Activation Agent/identities folder"
		/// </summary>
		private static void EncryptAndImportPfxFile(string pathToPfxFile)
		{
			try
			{
				if (!Directory.Exists(_platformSettings.IdentitiesPath))
				{
					Directory.CreateDirectory(_platformSettings.IdentitiesPath);
				}

				if (
					File.Exists(Path.Combine(_platformSettings.IdentitiesPath, _platformSettings.PfxFileName)) == false
					|| Prompt($"File already exists. Overwrite " + _platformSettings.PfxFileName + "? If so, type YES in all capital letters: ") == "YES")
				{
					File.WriteAllBytes(
						Path.Combine(_platformSettings.IdentitiesPath, _platformSettings.PfxFileName),
						ProtectedData.Protect(File.ReadAllBytes(pathToPfxFile), new byte[] { },
							DataProtectionScope.CurrentUser));

					Console.WriteLine("File: " + _platformSettings.PfxFileName + " is created at the location: " + _platformSettings.IdentitiesPath);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}


		/// <summary>
		/// Prompt the user for input.
		/// </summary>
		/// <param name="message">A friendly request to show the user.</param>
		/// <param name="allowEmpty">Allow empty responses if true</param>
		/// <param name="trimInput">Trim the input given by the user if true</param>
		/// <returns></returns>
		private static string Prompt(string message, bool allowEmpty = false, bool trimInput = true)
		{
			string value;
			do
			{
				Console.Write(message);
				value = (Console.ReadLine() ?? "");
				if (trimInput)
				{
					value = value.Trim();
				}
			} while (allowEmpty == false && string.IsNullOrEmpty(value));

			return value;
		}


		static void Main(string[] args)
		{
			//var container = PrepareIoC();
			//var platformSettings = container.Resolve<IPlatformSettings>();

			Initialize();
			bool isConsoleApplicable;
			StartConsole(args, out isConsoleApplicable);

			#region Importing private key into X509Certificate2
			//Import the certficate
			//X509Certificate2 certificate = new X509Certificate2("IvanWin.crt");
			//var privateKeyBytes = File.ReadAllBytes("IvanKey.pvk");

			//var cspParams = new CspParameters
			//{
			//    ProviderType = 1,
			//    Flags = CspProviderFlags.UseMachineKeyStore,
			//    KeyContainerName = Guid.NewGuid().ToString().ToUpperInvariant()
			//};
			//var rsa = new RSACryptoServiceProvider(cspParams);

			////rsa.ImportCspBlob(ExtractPrivateKeyBlobFromPvk(privateKeyBytes));
			//rsa.PersistKeyInCsp = true;
			//certificate.PrivateKey = rsa;
			#endregion

			#region generating the .pfx output file
			//// Export the PFX file
			//var certificateData = certificate.Export(X509ContentType.Pfx, String.Empty);
			//File.WriteAllBytes(@"YourCert.pfx", certificateData);


			////var csrReader = new StreamReader("IvanWin.crt");
			//var csrReader = new StreamReader("NikolaWin.crt");
			//var publicCert = csrReader.ReadToEnd();

			////var keyReader = new StreamReader("IvanKey.key");
			//var keyReader = new StreamReader("NikolaKey.key");
			//var privateKey = keyReader.ReadToEnd();

			//byte[] certBuffer = Helpers.GetBytesFromPEM(publicCert, PemStringType.Certificate);
			//byte[] keyBuffer = Helpers.GetBytesFromPEM(privateKey, PemStringType.RsaPrivateKey);

			//X509Certificate2 certificate = new X509Certificate2(certBuffer, String.Empty , X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

			//RSACryptoServiceProvider prov = Helpers.DecodeRsaPrivateKey(keyBuffer);
			//certificate.PrivateKey = prov;

			//var certificateData = certificate.Export(X509ContentType.Pfx, String.Empty);
			////File.WriteAllBytes(@"YourCertIvan.pfx", certificateData);
			//File.WriteAllBytes(@"YourCertNikola.pfx", certificateData);
			#endregion


			//File.WriteAllBytes( Path.Combine(platformSettings.IdentitiesPath,"PfxOutput.bin"),
			//    ProtectedData.Protect(File.ReadAllBytes("YourCertIvan.pfx"), new byte[] { },
			//        DataProtectionScope.LocalMachine));

			//File.WriteAllBytes("OutputPfx.bin",
			//    ProtectedData.Protect(File.ReadAllBytes("YourCertIvan.pfx"), new byte[] {},
			//        DataProtectionScope.LocalMachine));



		}

	}
}
