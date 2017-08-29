using System;
using System.Linq;
using System.Reflection;
using DryIoc;
using Mono.Options;

namespace AA.Linux.IdentityKeyGen
{
    public abstract class OptionAction
    {
        public CommandConfiguration Configuration { get; set; }
        public abstract void Do(string value);
    }

    public sealed class OptionAttribute : Attribute
    {
        public string Prototype { get; set; }
        public string Description { get; set; }
        public bool IsHidden { get; set; } = false;
        public bool IsToggle { get; set; } = false;
        public Type ActionType { get; set; }

        public OptionAttribute(string prototype, string description)
        {
            Prototype = prototype;
            Description = description;
        }

        private void Add(CommandConfiguration configuration, PropertyInfo property, string value)
        {
        }

        public void AddOption(PropertyInfo property, CommandConfiguration configuration, OptionSet optionSet)
        {
            OptionAction action = null;
            if (ActionType != null)
            {
                action = Activator.CreateInstance(ActionType) as OptionAction;
            }

            if (action == null)
            {
                optionSet.Add(Prototype, Description, value =>
                {
                    try
                    {
                        if (IsToggle)
                        {
                            var method = property.GetSetMethod();
                            if (method.GetParameters().Any(_ => _.ParameterType == typeof(bool)))
                            {
                                method.Invoke(configuration, new object[] { true });
                            }
                        }
                        else
                        {
                            property.TryAssign(configuration, Prototype, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to assign parameter '{Prototype}', an exception occurred.");
                        Console.WriteLine(ex);
                    }
                }, IsHidden);
            }
            else
            {
                action.Configuration = configuration;
                optionSet.Add(Prototype, Description, value =>
                {
                    try
                    {
                        action.Do(value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to assign parameter '{Prototype}', an exception occurred.");
                        Console.WriteLine(ex);
                    }
                }, IsHidden);
            }
        }
    }

    public sealed class CommandConfiguration
    {
        [Option("f|config=", "the configuration file to use")]
        public string ConfigurationFile { get; set; }

        [Option("h|help", "show usage", IsToggle = true)]
        public bool IsUsageRequested { get; set; } = false;

        [Option("P|print-config", "print configuration", IsToggle = true, IsHidden = true)]
        public bool IsPrintConfigurationRequested { get; set; } = false;

        [Option("E|encrypt-password", "encrypt a password for use in configuration", IsToggle = true)]
        public bool IsEncryptPasswordRequested { get; set; } = false;

        public bool IsUtilityMode
        {
            get
            {
                return IsUsageRequested || IsEncryptPasswordRequested || IsCSRRequested ||
                       IsPrintConfigurationRequested;
            }
        }

        [Option("g|genCsr", "generate a certificate signing request", IsToggle = true)]
        public bool IsCSRRequested { get; set; } = false;

        [Option("b=|bits=", "key size for certificate signing request")]
        public int CertificateSigningRequestKeySize { get; set; } = 2048;

        [Option("C=|primality-certainty=",
            "primality certainty value to use when generating prime numbers during key pair generation")]
        public int CertificateSigningRequestPrimalityCertainty { get; set; } = 100;

        [Option("I|import",
            "encrypt .pfx key and plate it under .../AppData/Roaming/BlackRidge Technology/Identity Activation Agent/identities folder", IsToggle = true
        )]
        public bool IsEncryptPfxRequested { get; set; } = false;

        [Option("path=", "Path to the .pfx file")]
        public string PathToThePfxFile { get; set; }

        [Option("MoveKeys|mvk",
            "move privatekey , cert and chain file to .../AppData/Roaming/BlackRidge Technology/identities folder", IsToggle = true
        )]
        public bool IsBootstrapMoveKeysRequested { get; set; } = false;

        [Option("pathToPrivateKey=", "Path to the private key file")]
        public string PathToPrivateKeyFile { get; set; }

        [Option("pathToCertKey=", "Path to the .crt file")]
        public string PathToCertFile { get; set; }

        [Option("pathToChain=", "Path to the chain file")]
        public string PathToChainFile { get; set; }

        public Version Version
        {
            get { return GetType()?.GetAssembly()?.GetName()?.Version; }
        }

        private CommandConfiguration()
        {

        }

        private static void Usage(OptionSet optionSet)
        {
            //Console.WriteLine("Usage: ADDIA.Service.exe [OPTIONS]+\n\nOptions:");
            //optionSet.WriteOptionDescriptions(Console.Error);
        }

        public static bool TryCreate(string[] args, out CommandConfiguration commandConfiguration)
        {
            commandConfiguration = new CommandConfiguration();
            OptionSet options = new OptionSet();
            GetOptions(commandConfiguration, options);

            try
            {
                options.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Usage(options);
                //xmlConfiguration = null;
                return false;
            }

            return true;
        }

        private static void GetOptions(CommandConfiguration configuration, OptionSet options)
        {
            Type type = typeof(CommandConfiguration);

            var properties = type.GetProperties()
                .Select(prop => new
                {
                    property = prop,
                    attr = prop.GetCustomAttribute<OptionAttribute>()
                })
                .Where(_ => _.attr != null);

            foreach (var property in properties)
            {
                property.attr.AddOption(property.property, configuration, options);
            }
        }
    }
}
