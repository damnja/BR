using System;
using AA.Core.Common;

namespace AA.Core.Identity
{
	public class BrTokenManager
	{
		protected readonly Logger Logger;
		protected readonly BrTokenInterface BrTokenInterface;
		public IntPtr BrTokenCtx => BrTokenInterface.BrTokenCtx;

		public BrTokenManager(Logger logger, BrTokenInterface brTokenInterface)
		{
			Logger = logger;
			BrTokenInterface = brTokenInterface;
		}

		public bool InitializeBrToken()
		{
			try
			{
				Logger.Info($"Calling IkeV2Init.").Wait();
				var responseStatus = TaskHelper.RetryOnError(2, TimeSpan.FromSeconds(2), BrTokenInterface.IkeV2Init).Result;
				 if (responseStatus == 0)
					return true;
			}
			catch (AggregateException e)
			{
				Logger.Error("Error occurred while Initializing BrToken. ", e.FormLogEntry()).Wait();
			}
			catch (Exception e)
			{
				Logger.Error("Error occurred while Initializing BrToken. ", e.FormLogEntry()).Wait();
			}

			return false;
		}

		public bool SetUp(byte[] key, byte[] cert, byte[] caCert, uint address)
		{
			try
			{
				Logger.Info($"Calling IkeV2SetIPv4. Address sent is {address}.").Wait();
				var result = BrTokenInterface.IkeV2SetIPv4(address).Result;
				if (result != 0)
					throw new Exception($"IkeV2SetIPv4 returned status {result}. Address sent is {address}.");

				Logger.Info($"Calling IkeV2SetKey. Key length is {key.Length}.").Wait();
				result = BrTokenInterface.IkeV2SetKey(key).Result;
				if (result != 0)
					throw new Exception($"IkeV2SetKey returned status {result}.");

				Logger.Info($"Calling IkeV2SetCertificate. Cert length is {cert.Length}.").Wait();
				result = BrTokenInterface.IkeV2SetCertificate(cert).Result;
				if (result != 0)
					throw new Exception($"IkeV2SetCertificate returned status {result}.");

				Logger.Info($"Calling IkeV2SetCaCertificate. CACert length is {caCert.Length}.").Wait();
				result = BrTokenInterface.IkeV2SetCaCertificate(caCert).Result;
				if (result != 0)
					throw new Exception($"IkeV2SetCaCertificate returned status {result}.");

				return true;
			}
			catch (AggregateException e)
			{
				Logger.Error("Error occurred on SetUp BrToken. ", e.FormLogEntry()).Wait();
			}
			catch (Exception e)
			{
				Logger.Error("Error occurred on SetUp BrToken. ", e.FormLogEntry()).Wait();
			}

			return false;
		}

		public bool GetBrToken()
		{
			try
			{
				Logger.Info($"Calling IkeV2Start.").Wait();
				var result = BrTokenInterface.IkeV2Start().Result;
				if (result != 0)
					throw new Exception($"IkeV2Start returned status {result}.");

				Logger.Info($"Calling IkeV2GetResult.").Wait();
				result = BrTokenInterface.IkeV2GetResult().Result;
				if (result != 0)
					throw new Exception($"IkeV2GetResult  returned status {result}.");

				Logger.Info($"Calling IkeV2GetBootstrapToken.").Wait();
				result = BrTokenInterface.IkeV2GetBootstrapToken().Result;
				if (result != 0)
					throw new Exception($"IkeV2GetResult returned status {result}.");
			}
			catch (AggregateException e)
			{
				Logger.Error("Error occurred on GetBrToken. ", e.FormLogEntry()).Wait();
			}
			catch (Exception e)
			{
				Logger.Error("Error occurred on GetBrToken. ", e.FormLogEntry()).Wait();
			}

			return false;
		}

		public bool FreeBrToken()
		{
			try
			{
				var responseStatus = TaskHelper.RetryOnError(2, TimeSpan.FromSeconds(2), BrTokenInterface.IkeV2Free).Result;
				if (responseStatus == 0)
					return true;
			}
			catch (AggregateException e)
			{
				Logger.Error("Error occurred while clearing BrToken. ", e.FormLogEntry()).Wait();
			}
			catch (Exception e)
			{
				Logger.Error("Error occurred while clearing BrToken. ", e.FormLogEntry()).Wait();
			}

			return false;
		}
	}
}
