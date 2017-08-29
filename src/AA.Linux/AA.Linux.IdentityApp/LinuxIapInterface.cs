using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AA.Common.MessageEntities;
using AA.Core.Identity;

namespace AA.Linux.IdentityApp
{
    public class LinuxIapInterface : IapInterface
    {
		public LinuxIapInterface(Logger logger) : base(logger)
		{
		}

		public override Task<int> Initialize()
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> AddIdentityRecord(GatewayIdentity gatewayIdentity)
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> RemoveIdentityRecord()
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> TerminateIdentity()
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> InsertBootstrapToken(string token)
	    {
		    throw new NotImplementedException();
	    }
    }
}
