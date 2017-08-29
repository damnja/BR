using AA.Core.Identity;
using DryIoc;
using System;

namespace AA.Linux.IdentityApp
{
	public class IAAService : IDisposable
	{
		private readonly Container _container;
		private readonly IdentityActivationAgent _identityActivationAgent;
		public IAAService(Container container)
		{
			_container = container;
			_identityActivationAgent = new IdentityActivationAgent(container);
		}

		public void Start()
		{
			_identityActivationAgent.Start();
		}


		public void Dispose()
		{
			_identityActivationAgent.Stop();
		}
	}
}
