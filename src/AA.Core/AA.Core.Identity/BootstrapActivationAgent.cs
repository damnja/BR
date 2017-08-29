using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AA.Core.Common;

namespace AA.Core.Identity
{
	public class BootstrapActivationAgent : IDisposable
	{
		private readonly Container _container;
		private readonly Logger _log;
		private readonly Configuration _configuration;
		private readonly ConfigurationValidationTool _configurationValidationTool;
		private readonly BrTokenInterface _brTokenInterface;
		private readonly BrTokenManager _brTokenManager;
		private readonly IntPtr _iapContextparam;

		public BootstrapActivationAgent(Container container, IntPtr iapContextParam)
		{
			_container = container;
			_log = _container.Resolve<Logger>();
			_configuration = _container.Resolve<Configuration>();
			_configurationValidationTool = _container.Resolve<ConfigurationValidationTool>();
			_brTokenInterface = _container.Resolve<BrTokenInterface>();
			_brTokenManager = new BrTokenManager(_log, _brTokenInterface);
			_iapContextparam = iapContextParam;
		}

		public virtual bool Start()
		{
			try
			{
				_log.Info("IBA Start.").Wait();
				if (!Validate())
				{
					return false;
				}

				if (!_brTokenManager.InitializeBrToken())
				{
					return false;
				}

				if (!_brTokenManager.SetUp(_configuration.BootstrapKey, _configuration.BootstrapCert, _configuration.BootstrapChain, _configuration.IP))
				{
					return false;
				}

				if (!_brTokenManager.GetBrToken())
				{
					return false;
				}
			}
			catch (Exception e)
			{
				_log.Error("IBA Error occurred during processing.", e.FormLogEntry()).Wait();
			}

			return false;
		}



        public void Stop()
        {
            try
            {
                _log.Info("IBA Stop.").Wait();
                _brTokenManager.FreeBrToken();
            }
            catch (Exception e)
            {
                _log.Error("IBA unhandled exception occurred while Removing Identity from TAC driver", e.FormLogEntry()).Wait();
            }

            _log.Info("IAA Dispose").Wait();
        }

        public bool Validate()
		{
			_log.Info("IBA ValidateActivationAgent").Wait();
			var fauls = _configurationValidationTool.GetBootstrapFaults().ToList();
			if (fauls.Any())
				_log.Error("IBA FAULTS", fauls.ToDictionary(x => x.Message, x => x.IsFatal.ToString())).Wait();

			if (fauls.Any(x => x.IsFatal))
				return false;

			return true;
		}

		public void Dispose()
		{
			Stop();
			_container?.Dispose();
		}
	}
}
