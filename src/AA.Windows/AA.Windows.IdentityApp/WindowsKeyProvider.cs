using System.IO;
using System.Security.Cryptography;
using AA.Core.Common;

namespace AA.Windows.IdentityApp
{
	public sealed class WindowsKeyProvider : KeyProvider
	{
		protected override RSA GetKey(Stream stream)
		{
			var plaintext = ProtectedData.Unprotect(stream.ToByteArray(), new byte[] { }, DataProtectionScope.CurrentUser);

			using (var memoryStream = new MemoryStream(plaintext))
			{
				return base.GetKey(memoryStream);
			}
		}
	}
}
