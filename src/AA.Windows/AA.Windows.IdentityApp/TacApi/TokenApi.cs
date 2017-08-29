using System;
using System.Runtime.InteropServices;

namespace AA.Windows.IdentityApp.TacApi
{
	public static class TokenApi
	{
		//BRToken
#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern IntPtr ikev2_new();

#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int ikev2_free(IntPtr ctx);

#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int ikev2_set_ipv4_address(IntPtr context, uint address);

#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int ikev2_set_ipv6_address(IntPtr ctx, byte[] address);

#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int ikev2_set_key(IntPtr ctx, byte[] pemPrivateKey);

#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int ikev2_set_certificate(IntPtr ctx, byte[] pemPrivateKey);

#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int ikev2_set_cacertificate(IntPtr ctx, byte[] pemPrivateKey);

#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int ikev2_start(IntPtr ctx);

#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int ikev2_abort(IntPtr ctx);

#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int ikev2_get_result(IntPtr ctx);

#if BIT64
		[DllImport("TACLibs/BRToken_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/BRToken_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int ikev2_get_token(IntPtr ctx, byte[] token, int length);


	}
}
