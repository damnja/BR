using System;
using System.Collections.Generic;
using System.Text;

namespace AA.Core.Common.MessageEntities
{
	public class TacGatewayArgs : EventArgs
	{
		public GatewayIdentity GatewayIdentity { get; set; }
	}
}
