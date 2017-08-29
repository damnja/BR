using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AA.Core.Common.MessageEntities
{
	public class GatewayIdentity
	{
		public int AlgID { get; set; }
		public int Clock { get; set; }
		public int Enabled { get; set; }
		public int ID { get; set; }
		public string Key { get; set; }
		public byte[] KeyArray { get; set; }
		public int Key_Len { get; set; }
		public ulong Nonce { get; set; }
		public ulong Start_seq { get; set; }
		public bool Tac_mutual_auth { get; set; }
		public int TCP_tagging { get; set; }
		public int Timeout { get; set; }
		public int Token_size { get; set; }

		public static GatewayIdentity ParseToTacGatewayResponse(string responseString)
		{
			if (!string.IsNullOrEmpty(responseString))
			{
				int intValue;
				ulong ulongValue;
				bool boolValue;
				var exceptions = new List<Exception>();
				GatewayIdentity activation = new GatewayIdentity();
				XElement xmlRoot = XElement.Parse(responseString);

				string xmlAlg_ID = xmlRoot.Element(XName.Get("alg_ID"))?.Value;
				if (int.TryParse(xmlAlg_ID, out intValue))
					activation.AlgID = intValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"alg_ID\". Actual value {xmlAlg_ID}"));

				string xmlClock = xmlRoot.Element(XName.Get("clock"))?.Value;
				if (int.TryParse(xmlClock, out intValue))
					activation.Clock = intValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"clock\". Actual value {xmlClock}"));

				string xmlEnabled = xmlRoot.Element(XName.Get("enabled"))?.Value;
				if (int.TryParse(xmlEnabled, out intValue))
					activation.Enabled = intValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"enabled\". Actual value {xmlEnabled}"));

				string xmlId = xmlRoot.Element(XName.Get("ID"))?.Value;
				if (int.TryParse(xmlId, out intValue))
					activation.ID = intValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"ID\". Actual value {xmlId}"));

				activation.Key = xmlRoot.Element(XName.Get("key"))?.Value;
				activation.KeyArray = Convert.FromBase64String(activation.Key);

				string xmlKeyLen = xmlRoot.Element(XName.Get("key_len"))?.Value;
				if (int.TryParse(xmlKeyLen, out intValue))
					activation.Key_Len = intValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"key_len\". Actual value {xmlKeyLen}"));

				string xmlNonce = xmlRoot.Element(XName.Get("nonce"))?.Value;
				if (ulong.TryParse(xmlNonce, out ulongValue))
					activation.Nonce = ulongValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"nonce\". Actual value {xmlNonce}"));

				string xmlStartSeq = xmlRoot.Element(XName.Get("start_seq"))?.Value;
				if (ulong.TryParse(xmlStartSeq, out ulongValue))
					activation.Start_seq = ulongValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"start_seq\". Actual value {xmlStartSeq}"));

				string xmlTac_mutual_auth = xmlRoot.Element(XName.Get("TAC_mutual_auth"))?.Value;
				if (xmlTac_mutual_auth.TryParseToBoolean(out boolValue))
					activation.Tac_mutual_auth = boolValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"TAC_mutual_auth\". Actual value {xmlTac_mutual_auth}"));

				string xmlTcpTagging = xmlRoot.Element(XName.Get("TCP_tagging"))?.Value;
				if (int.TryParse(xmlTcpTagging, out intValue))
					activation.TCP_tagging = intValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"TCP_tagging\". Actual value {xmlTcpTagging}"));

				string xmlTimeout = xmlRoot.Element(XName.Get("timeout"))?.Value;
				if (int.TryParse(xmlTimeout, out intValue))
					activation.Timeout = intValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"timeout\". Actual value {xmlTimeout}"));

				string xmlTokenSize = xmlRoot.Element(XName.Get("token_size"))?.Value;
				if (int.TryParse(xmlTokenSize, out intValue))
					activation.Token_size = intValue;
				else
					exceptions.Add(new Exception($"ParseToTacGatewayResponse error on field \"token_size\". Actual value {xmlTokenSize}"));

				if (exceptions.Any())
					throw new AggregateException(exceptions);

				return activation;

			}
			return null;
		}
	}
}
