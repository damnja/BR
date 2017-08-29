using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AA.Windows.IdentityKeyGen
{
	public sealed class WindowsCertificateSigningRequest
	{
		public string CommonName { get; set; }
		public string Country { get; set; }
		public string State { get; set; }
		public string Location { get; set; }
		public string Organization { get; set; }
		public string OrganizationalUnit { get; set; }
		public int KeySize { get; set; }
		public string SignatureAlgorithm { get; set; } = "SHA256withRSA";
		/// <summary>
		/// A measure of the uncertainty that the caller is willing to tolerate.
		/// The probability that sensitive primes generated represent a prime number will exceed (1 - 1/2<b>PrimalityCertainty</b>).
		/// The execution time of this constructor is proportional to the value of this parameter
		/// </summary>
		public int PrimalityCertainty { get; set; } = 100;
		public string Request { get; set; }
		public string Key { get; set; }

		public void Create()
		{
			// create an X509 name to represent the subject
			var attrs = new Dictionary<DerObjectIdentifier, string> {
				{ X509Name.C, Country },
				{ X509Name.ST, State },
				{ X509Name.L, Location },
				{ X509Name.O, Organization },
				{ X509Name.OU, OrganizationalUnit },
				{ X509Name.CN, CommonName }
			};
			var name = new X509Name(attrs.Keys.ToList(), attrs);

			// generate a public-private key pair.
			var pairGenerator = GeneratorUtilities.GetKeyPairGenerator("RSA");
			var secureRandom = new SecureRandom();
			var parameters = new RsaKeyGenerationParameters(new Org.BouncyCastle.Math.BigInteger("65537"), secureRandom, KeySize, PrimalityCertainty);
			pairGenerator.Init(parameters);
			var pair = pairGenerator.GenerateKeyPair();

			// create the actual CSR.
			var request = new Pkcs10CertificationRequest(
				new Asn1SignatureFactory(SignatureAlgorithm, pair.Private, secureRandom),
				name,
				pair.Public,
				null,
				pair.Private);

			// write the output in PEM format to our properties.
			using (var stringWriter = new StringWriter())
			{
				var pemWriter = new PemWriter(stringWriter);
				pemWriter.WriteObject(request);
				stringWriter.Flush();
				Request = stringWriter.ToString();
			}

			using (var stringWriter = new StringWriter())
			{
				var pemWriter = new PemWriter(stringWriter);
				pemWriter.WriteObject(pair.Private);
				stringWriter.Flush();
				Key = stringWriter.ToString();
			}
		}
	}
}
