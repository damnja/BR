using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AA.Core.Identity;

namespace AA.Linux.IdentityApp
{
    public class LinuxBrTokenInterface : BrTokenInterface
    {
	    public override Task<int> IkeV2Init()
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> IkeV2SetIPv4(uint address)
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> IkeV2SetKey(byte[] key)
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> IkeV2SetCertificate(byte[] cert)
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> IkeV2SetCaCertificate(byte[] caCert)
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> IkeV2Start()
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> IkeV2GetResult()
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> IkeV2GetBootstrapToken()
	    {
		    throw new NotImplementedException();
	    }

	    public override bool IkeV2InsertBootstrapToken(string token, IntPtr epcCtx)
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> IkeV2Abort()
	    {
		    throw new NotImplementedException();
	    }

	    public override Task<int> IkeV2Free()
	    {
		    throw new NotImplementedException();
	    }
    }
}
