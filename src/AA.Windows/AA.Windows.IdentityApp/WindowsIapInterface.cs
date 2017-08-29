using AA.Core.Common.MessageEntities;
using AA.Core.Identity;
using AA.Windows.IdentityApp.TacApi;
using AA.Windows.IdentityApp.TacEntities;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace AA.Windows.IdentityApp
{
	public class WindowsIapInterface : IapInterface
	{
		public WindowsIapInterface(Logger logger) : base(logger)
		{
		}

		public override Task<int> Initialize()
		{
			int rc = IdentityApi.epc_init(out IapCtx);
			return Task.FromResult(rc);
		}

		public override Task<int> AddIdentityRecord(GatewayIdentity gatewayIdentity)
		{
			IntPtr pnt = IntPtr.Zero;
			int i;
			//Logger.Info("AddIdentityRecord started!");
			try
			{
				epc_identity_t id1 = new epc_identity_t();

				id1.name = new byte[TacConstants.MAX_IDENT_NAME];
				id1.key = new byte[TacConstants.MAX_KEY_LEN / 8];
				id1.dest = new ip_adr_t[TacConstants.MAX_EPC_DESTINATIONS];

				id1.tcp_ident_tag = TacConstants.EPC_TAG_TCP_SEQNO;
				id1.num_of_destinations = TacConstants.EPC_IDENT_DEST_WILDCARD;


				id1.start_seq = gatewayIdentity.Start_seq;
				id1.alg_id = gatewayIdentity.AlgID;
				id1.nonce = gatewayIdentity.Nonce;
				id1.keylen = gatewayIdentity.Key_Len;
				id1.tokensize = gatewayIdentity.Token_size;
				id1.require_TAC_mutual_authentication = Convert.ToInt32(gatewayIdentity.Tac_mutual_auth);

				var nameBytes = Encoding.ASCII.GetBytes(TacConstants.EPC_IDENTITY_NAME);
				Array.Copy(nameBytes, id1.name, nameBytes.Length);

				i = 0;
				foreach (var b in gatewayIdentity.KeyArray)
				{
					id1.key[i] = b;
					i++;
				}
				//Logger.Info("AddIdentityRecord before Marshal.AllocHGlobal");

				pnt = Marshal.AllocHGlobal(Marshal.SizeOf(id1));
				//Logger.Info("AddIdentityRecord after Marshal.AllocHGlobal");
				Marshal.StructureToPtr(id1, pnt, false);// Copy the structure to unmanaged memory.	
														//Logger.Info("AddIdentityRecord after Marshal.StructureToPtr");

				var rc = IdentityApi.epc_identity_add(IapCtx, pnt);

				return Task.FromResult(rc);

			}
			finally
			{
				if (pnt != IntPtr.Zero)
					Marshal.FreeHGlobal(pnt);
			}
		}

		public override Task<int> RemoveIdentityRecord()
		{
			int rc = 0;
			if (IapCtx != IntPtr.Zero)
				rc = IdentityApi.epc_identity_del(IapCtx, TacConstants.EPC_IDENTITY_NAME);

			return Task.FromResult(rc);
		}

		public override Task<int> TerminateIdentity()
		{
			int rc = 0;
			if (IapCtx != IntPtr.Zero)
			{
				rc = IdentityApi.epc_term(IapCtx);
			}

			return Task.FromResult(rc);
		}

		public override Task<int> InsertBootstrapToken(string token)
		{
			throw new NotImplementedException();
		}
	}
}
