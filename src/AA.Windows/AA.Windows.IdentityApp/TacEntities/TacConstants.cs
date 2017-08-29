using System;

namespace AA.Windows.IdentityApp.TacEntities
{
	public class TacConstants
	{
		public const int MAX_IDENT_NAME = 256;
		public const int MAX_KEY_LEN = 1024;
		public const int MAX_EPC_DESTINATIONS = 8;
		public const ulong INIT_START_SEQ = 18362826596582667863;
		public const ulong INIT_NONCE = 559297625330716516;
		public const uint EPC_IDENT_DEST_WILDCARD = UInt32.MaxValue;
		public const int EPC_TAG_TCP_SEQNO = 1;
		public const int MAX_BOOTSTRAP_TOKENS = 8;
		public const int MAX_KEY_RESOURCES = 8;
		public const int DEFAULT_MAX_IP_LEN = 64;
		public const int INT_SIZE = 4;
		public const int LONG_SIZE = 8;

		public const int ALG_ID = 0;
		public const string EPC_IDENTITY_NAME = "Identity_number1";
	}
}
