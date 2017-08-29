using AA.Core.Common;
using AA.Core.Identity;
using DryIoc;
using System;

namespace AA.Windows.IdentityApp
{
	public class IaaService : IDisposable
	{
		private readonly Logger _logger;
		private readonly IdentityActivationAgent _identityActivationAgent;

		public IaaService(Container container)
		{
			_logger = container.Resolve<Logger>();
			_identityActivationAgent = new IdentityActivationAgent(container);
		}

		/// <summary>
		/// Start processing Activation Agent
		/// </summary>
		public void Start()
		{
			try
			{
				_identityActivationAgent.Start();
			}
			catch (Exception e)
			{
				_logger.Error("An unhandled error occurred in processing Activation agent.", e.FormLogEntry());
			}
		}

		/// <inheritdoc />
		/// <summary>
		/// Triggered if there is some error while running the Activation Agent
		/// or exiting application normal way
		/// </summary>
		public void Dispose()
		{
			_identityActivationAgent.Stop();
		}

		/// <summary>
		/// Occurs when the user is trying to log off or shut down the system.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void SystemEvents_SessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
		{
			_identityActivationAgent.Stop();
			_logger.Info("Exiting SystemEvents_SessionEnding");
		}


	}
}