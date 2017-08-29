using AA.Core.Common.MessageEntities;
using System;
using System.Threading.Tasks;

namespace AA.Core.Identity
{
	/// <summary>
	/// Abstract class to work with iap.dll
	/// </summary>
	public abstract class IapInterface
	{
		public IntPtr IapCtx;
		protected Logger Logger;

		protected IapInterface(Logger logger)
		{
			Logger = logger;
		}

		public abstract Task<int> Initialize();

		public abstract Task<int> AddIdentityRecord(GatewayIdentity gatewayIdentity);

		public abstract Task<int> RemoveIdentityRecord();

		public abstract Task<int> TerminateIdentity();

		public abstract Task<int> InsertBootstrapToken(string token);
	}
}
