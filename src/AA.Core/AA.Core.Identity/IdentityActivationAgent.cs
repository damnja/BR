using DryIoc;
using System;
using System.Linq;
using AA.Core.Common;

namespace AA.Core.Identity
{
	public sealed class IdentityActivationAgent
	{
		private readonly Container _container;
		private readonly Configuration _configuration;
		private readonly Logger _logger;
		private readonly ConfigurationValidationTool _configurationValidationTool;
		private readonly TacGatewayInterface _tacGatewayInterface;
		private readonly IapManager _iapManager;
		

		public IdentityActivationAgent(Container container)
		{
			_container = container;
			_logger = container.Resolve<Logger>();
			_configuration = container.Resolve<Configuration>();
			_configurationValidationTool = container.Resolve<ConfigurationValidationTool>();
			_tacGatewayInterface = new TacGatewayInterface(_configuration, _logger);

			var iapInterface = container.Resolve<IapInterface>();
			_iapManager = new IapManager(_logger, iapInterface);

			_tacGatewayInterface.TokenReceived += _iapManager.RefreshTacDriver;
		}

		

		/// <summary>
		/// Main functionality for Identity Activation Agent
		/// </summary>
		public void Start()
		{
			try
			{
				if (!Validate())
					return;

				_logger.Info("IAA Start").Wait();

				if (!_iapManager.InitializeTacDriver())
				{
					return;
				}

				//check if Bootstrap is enabled in Configuration
				if (_configuration.GatewayEndPoint.UserBootstrapActivation)
				{
					if (!InvokeBootstrap())
						return;
				}

				//check if UserActivation is enabled in Configuration
				if (_configuration.GatewayEndPoint.UserActivation)
				{
					if (_tacGatewayInterface.Initialize())
					{
						bool refreshSucceeded = true;
						while (refreshSucceeded)
						{
							refreshSucceeded = _tacGatewayInterface.Refresh();
						}
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error("IAA Error occurred during processing.", e.FormLogEntry()).Wait();
			}
		}


		/// <summary>
		/// When application is stopping 
		/// Identity from TAC driver should be removed
		/// and terminated
		/// </summary>
		public void Stop()
		{
			try
			{
				_iapManager.RemoveIdentityFromTacDriver();
			}
			catch (Exception e)
			{
				_logger.Error("IAA unhandled exception occurred while Removing Identity from TAC driver", e.FormLogEntry()).Wait();
			}

			try
			{
				_iapManager.TerminateIdentityInTacDriver();
			}
			catch (Exception e)
			{
				_logger.Error("IAA unhandled exception occurred while Terminating Identity In TAC driver", e.FormLogEntry()).Wait();
			}
			_logger.Info("IAA Stopping").Wait();
		}

		/// <summary>
		/// Validate activation agent
		/// configuration file, and certificate
		/// </summary>
		/// <returns></returns>
		public bool Validate()
		{
			_logger.Info("IAA Starting Validation Activation Agent").Wait();
			var fauls = _configurationValidationTool.GetFaults().ToList();
			if (fauls.Any())
				_logger.Error("IAA FAULTS", fauls.ToDictionary(x => x.Message, x => x.IsFatal.ToString())).Wait();


			if (fauls.Any(x => x.IsFatal))
				return false;

			return true;
		}

		/// <summary>
		/// Executes Bootstrap service if it is enabled in configuration
		/// </summary>
		/// <returns></returns>
		public bool InvokeBootstrap()
		{
			try
			{
				using (var bootstrapActivationAgent = new BootstrapActivationAgent(_container, _iapManager.IapCtx))
					return bootstrapActivationAgent.Start();
			}
			catch(Exception e)
			{
				_logger.Error("An unhandled error occurred while processing bootstrap", e.FormLogEntry()).Wait();
				return false;
			}
		}
	}
}
