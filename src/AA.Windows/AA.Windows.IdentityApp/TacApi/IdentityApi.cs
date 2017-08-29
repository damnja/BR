using System;
using System.Runtime.InteropServices;

namespace AA.Windows.IdentityApp.TacApi
{
	public static class IdentityApi
	{
#if BIT64
		[DllImport("TACLibs/iap_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/iap_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int epc_init(out IntPtr value);

#if BIT64
		[DllImport("TACLibs/iap_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/iap_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int epc_identity_add(IntPtr arg, IntPtr ident);

#if BIT64
		[DllImport("TACLibs/iap_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/iap_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int epc_identity_del(IntPtr arg, string identity);

#if BIT64
		[DllImport("TACLibs/iap_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/iap_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int epc_term(IntPtr arg);

#if BIT64
		[DllImport("TACLibs/iap_64.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#elif BIT32
		[DllImport("TACLibs/iap_32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
#endif
		public static extern int epc_bootstrap_add(IntPtr ctx, IntPtr bs);
	}
}
