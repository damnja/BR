using AA.Core.Identity;
using AA.Windows.IdentityApp.TacApi;
using System;
using System.Threading.Tasks;

namespace AA.Windows.IdentityApp
{
	public class WindowsBrTokenInterface : BrTokenInterface
	{
		public override Task<int> IkeV2Init()
		{
			BrTokenCtx = TokenApi.ikev2_new();
			return Task.FromResult(0);
		}

		public override Task<int> IkeV2SetIPv4(uint address)
		{
			var response = TokenApi.ikev2_set_ipv4_address(BrTokenCtx, address);
			return Task.FromResult(response);
		}

		public override Task<int> IkeV2SetKey(byte[] key)
		{
			var response = TokenApi.ikev2_set_key(BrTokenCtx, key);
			return Task.FromResult(response);
		}

		public override Task<int> IkeV2SetCertificate(byte[] cert)
		{
			var response = TokenApi.ikev2_set_certificate(BrTokenCtx, cert);
			return Task.FromResult(response);
		}

		public override Task<int> IkeV2SetCaCertificate(byte[] caCert)
		{
			var response = TokenApi.ikev2_set_certificate(BrTokenCtx, caCert);
			return Task.FromResult(response);
		}

		public override Task<int> IkeV2Start()
		{
			var response = TokenApi.ikev2_start(BrTokenCtx);
			return Task.FromResult(response);
		}

		public override Task<int> IkeV2GetResult()
		{
			var response = TokenApi.ikev2_get_result(BrTokenCtx);
			return Task.FromResult(response);
		}

		public override Task<int> IkeV2GetBootstrapToken()
		{
			int len = 0;
			try
			{
				byte[] bToken = new byte[256];
				len = TokenApi.ikev2_get_token(BrTokenCtx, bToken, 256);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

			return Task.FromResult(len);
		}

		public override Task<int> IkeV2Abort()
		{
			int response = 0;
			if (BrTokenCtx != IntPtr.Zero)
				response = TokenApi.ikev2_abort(BrTokenCtx);

			return Task.FromResult(response);
		}

		public override Task<int> IkeV2Free()
		{
			int response = 0;
			if (BrTokenCtx != IntPtr.Zero)
				response = TokenApi.ikev2_free(BrTokenCtx);

			return Task.FromResult(response);
		}

		public override bool IkeV2InsertBootstrapToken(string token, IntPtr epcCtx)
		{
			throw new NotImplementedException();
		}
	}
}
