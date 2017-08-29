using System;
using System.Runtime.InteropServices;

namespace AA.Windows.IdentityApp.TacEntities
{
	/// <summary>
	/// Entities that are used by iap.dll for windows
	/// </summary>
	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
	public class ip_adr_t
	{
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.U4, SizeConst = 1)]
		public uint flags;

#if BIT64
		[FieldOffset(8)]
#elif BIT32
		[FieldOffset(4)]
#endif
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] ip4;
		//public uint ip4;

#if BIT64
		[FieldOffset(8)]
#elif BIT32
		[FieldOffset(4)]
#endif
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] v6Adr8;

#if BIT64
		[FieldOffset(8)]
#elif BIT32
		[FieldOffset(4)]
#endif
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public ushort[] v6Adr16;

#if BIT64
		[FieldOffset(8)]
#elif BIT32
		[FieldOffset(4)]
#endif
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] v6Adr32;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct epc_identity_t
	{
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = TacConstants.MAX_IDENT_NAME)]
		public byte[] name;

		//INT alg_id;
		public int alg_id;

		//INT tokensize;
		public int tokensize;

		//INT tcp_ident_tag;
		public int tcp_ident_tag;

		//INT keylen;
		public int keylen;

		//UCHAR key[MAX_KEY_LEN / 8];
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = TacConstants.MAX_KEY_LEN / 8)]
		public byte[] key;

		//U64 start_seq;          /* starting sequence number for client */
		public ulong start_seq;

		//U64 nonce;              /* nonce */
		public ulong nonce;

		//INT pad1;
		public int pad1;

		//INT require_TAC_mutual_authentication
		public int require_TAC_mutual_authentication;

		//U32 num_of_destinations;
		public UInt32 num_of_destinations;

		//ip_adr_t  dest[MAX_EPC_DESTINATIONS];
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = TacConstants.MAX_EPC_DESTINATIONS)]
		public ip_adr_t[] dest;
	}

	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
	public class epc_bootstrap
	{
		[FieldOffset(0)]
		public int flags;

		[FieldOffset(4)]
		public System.UInt16 num_tokens;

		[FieldOffset(6)]
		public System.UInt16 num_dest;

		[FieldOffset(8)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = TacConstants.MAX_BOOTSTRAP_TOKENS)]
		uint[] token;

		[FieldOffset(40)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = TacConstants.MAX_KEY_RESOURCES)]
		public DestName aaszNames;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct DestName
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = TacConstants.DEFAULT_MAX_IP_LEN)]
		public string destName;
	}
}
