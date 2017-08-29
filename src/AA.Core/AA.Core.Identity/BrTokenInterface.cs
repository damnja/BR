using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AA.Core.Identity
{
	public abstract class BrTokenInterface
	{
		public IntPtr BrTokenCtx;

		public abstract Task<int> IkeV2Init();
		public abstract Task<int> IkeV2SetIPv4(uint address);
		public abstract Task<int> IkeV2SetKey(byte[] key);
		public abstract Task<int> IkeV2SetCertificate(byte[] cert);
		public abstract Task<int> IkeV2SetCaCertificate(byte[] caCert);
		public abstract Task<int> IkeV2Start();
		public abstract Task<int> IkeV2GetResult();
		public abstract Task<int> IkeV2GetBootstrapToken();
		public abstract bool IkeV2InsertBootstrapToken(string token, IntPtr epcCtx);
		public abstract Task<int> IkeV2Abort();
		public abstract Task<int> IkeV2Free();
	}
}
