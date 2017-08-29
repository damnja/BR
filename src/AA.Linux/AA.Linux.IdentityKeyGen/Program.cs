using AA.Core.Identity;
using AA.Linux.IdentityApp;
using DryIoc;
using System;
using System.IO;

namespace AA.Linux.IdentityKeyGen
{
	class Program
    {
        private static Container _container;
        private static IPlatformSettings _platformSettings;

        private static void ConfigureLogger()
        {
            LoggerHelper.ConfigureLoggerOutput(_platformSettings.IdentitiesPath);
        }

        private static Container PrepareIoC()
        {
            var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

            container.Register<IPlatformSettings, LinuxPlatformSettings>(Reuse.Singleton);
            return container;
        }

        private static void Initialize()
        {
            try
            {
                _container = PrepareIoC();
                _platformSettings = _container.Resolve<IPlatformSettings>();
                ConfigureLogger();
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
                //Console.OpenStandardOutput();
                isConsoleApplicable = true;
            }
            catch
            {
                isConsoleApplicable = false;
                return;
            }

            try
            {
                CommandConfiguration commandConfiguration;
                if (CommandConfiguration.TryCreate(args, out commandConfiguration) == false)
                {
                    return;
                }

                // if requested, create a certificate signing request on demand.
                if (commandConfiguration.IsCSRRequested)
                {
                    CreateCertificateSigningRequest(commandConfiguration.CertificateSigningRequestKeySize, commandConfiguration.CertificateSigningRequestPrimalityCertainty);
                    return;
                }


                // if requested, move .pfx file to ?HOME/Productname folder
                if (commandConfiguration.IsEncryptPfxRequested)
                {
                    if (!String.IsNullOrEmpty(commandConfiguration.PathToThePfxFile))
                    {
                        if (File.Exists(commandConfiguration.PathToThePfxFile))
                            ImportPfxFile(commandConfiguration.PathToThePfxFile);
                        else
                        {
                            Console.WriteLine("Path to the " + commandConfiguration.PathToThePfxFile + " file is not valid");
                            //_logger.LogWarning("Path to the " + commandConfiguration.PathToThePfxFile + " file is not valid");
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
        private static void MoveFileToIdentitiesFolder(string filePath, string platformIbaFileName)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    if (
                        File.Exists(Path.Combine(_platformSettings.IdentitiesPath, platformIbaFileName)) == false
                        || Prompt($"File " + platformIbaFileName + " already exists. Overwrite " + platformIbaFileName + "? If so, type YES in all capital letters: ") == "YES")
                    {

                        File.WriteAllBytes(Path.Combine(_platformSettings.IdentitiesPath, platformIbaFileName),
                                            File.ReadAllBytes(filePath)
                                           );

                        Console.WriteLine("File " + platformIbaFileName + " is created at the location: " + _platformSettings.IdentitiesPath);
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
            var csr = new CertificateSigningRequest
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
        private static void ImportPfxFile(string pathToPfxFile)
        {
            try
            {
                string pfxFilePath = Path.Combine(_platformSettings.IdentitiesPath, _platformSettings.PfxFileName);


                if (!Directory.Exists(_platformSettings.IdentitiesPath))
                {
                    Directory.CreateDirectory(_platformSettings.IdentitiesPath);
                }

                if (
                    File.Exists(pfxFilePath) == false
                    || Prompt($"File "+_platformSettings.PfxFileName + " already exists at the location "+_platformSettings.IdentitiesPath +". Overwrite it? If so, type YES in all capital letters: ") == "YES")
                {
                    File.WriteAllBytes(
                    pfxFilePath, File.ReadAllBytes(pathToPfxFile));

                    Console.WriteLine("File: " + pfxFilePath + " is created");
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
            Initialize();
            bool isConsoleApplicable;

            StartConsole(args, out isConsoleApplicable);            
        }
    }
}