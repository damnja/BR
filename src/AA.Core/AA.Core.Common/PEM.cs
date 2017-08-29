using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;

namespace AA.Core.Common
{
	public static class PEM
	{
		private class PasswordFinder : IPasswordFinder
		{
			private string Password;
			public char[] GetPassword() => Password.ToCharArray();
			public PasswordFinder(string password) => Password = password;
		}

		public static RSAParameters ReadRSAParameters(TextReader reader, string password = null)
		{
			var pemReader = password == null ? new PemReader(reader) : new PemReader(reader, new PasswordFinder(password));

			var obj = pemReader.ReadObject();
			if (obj == null)
			{
				throw new NotSupportedException("Unsupported key format");
			}
			else if (obj is RsaPrivateCrtKeyParameters parameters)
			{
				return new RSAParameters
				{
					Modulus = parameters.Modulus.ToByteArrayUnsigned(),
					Exponent = parameters.PublicExponent.ToByteArrayUnsigned(),
					D = parameters.Exponent.ToByteArrayUnsigned(),
					P = parameters.P.ToByteArrayUnsigned(),
					Q = parameters.Q.ToByteArrayUnsigned(),
					DP = parameters.DP.ToByteArrayUnsigned(),
					DQ = parameters.DQ.ToByteArrayUnsigned(),
					InverseQ = parameters.QInv.ToByteArrayUnsigned()
				};
			}
			else
			{
				throw new NotSupportedException("Unsupported key format");
			}
		}
		public static X509Certificate GetCertificate(TextReader reader)
		{
			return null;
		}
	}
}
