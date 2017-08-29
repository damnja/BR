using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AA.Core.Common
{
	public abstract class KeyProvider
	{
		protected virtual RSA GetKey(Stream stream)
		{
			var rsa = RSA.Create();
			try
			{
				using (var reader = new StreamReader(stream, Encoding.UTF8, true, 8096, true))
				{
					rsa.ImportParameters(PEM.ReadRSAParameters(reader));
					return rsa;
				}
			}
			catch
			{
				rsa.Dispose();
				throw;
			}
		}
	}
}
