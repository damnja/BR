using AA.Core.Common;
using AA.Core.Common.MessageEntities;
using System;

namespace AA.Core.Identity
{
	/// <summary>
	/// Wrapper around IapInterface
	/// added retry logic
	/// </summary>
	public class IapManager
	{
		private readonly Logger _logger;
		private readonly IapInterface _iapInterface;
		public IntPtr IapCtx => _iapInterface.IapCtx;

		public IapManager(Logger logger, IapInterface iapInterface)
		{
			_logger = logger;
			_iapInterface = iapInterface;
		}

		public bool InitializeTacDriver()
		{
			try
			{
				_logger.Info("IAA InitializeTacDriver.").Wait();
				var responseStatus = TaskHelper.RetryOnError(Constants.RetryTacDriverValue, 
					TimeSpan.FromSeconds(Constants.DelayTacDriverValue),
					_iapInterface.Initialize).Result;
				if (responseStatus == 0)
					return true;
			}
			catch (AggregateException e)
			{
				_logger.Error("Error occurred while Initializing Identity in TAC driver. ", e.FormLogEntry()).Wait();

			}
			catch (Exception e)
			{
				_logger.Error("Error occurred while Initializing Identity in TAC driver. ", e.FormLogEntry()).Wait();

			}

			return false;
		}

		public bool RefreshTacDriver(object source, TacGatewayArgs args)
		{
			try
			{
				_logger.Info("IAA RefreshTacDriver.").Wait();
				var gatewayIdentity = args.GatewayIdentity;
				var responseStatus = TaskHelper.RetryOnError(Constants.RetryTacDriverValue, 
                    TimeSpan.FromSeconds(Constants.DelayTacDriverValue), 
                    _iapInterface.AddIdentityRecord, gatewayIdentity).Result;
				if (responseStatus == 0)
					return true;
			}
			catch (AggregateException e)
			{
				_logger.Error("Error occurred while Adding Identity in TAC driver. ", e.FormLogEntry()).Wait();
			}
			catch (Exception e)
			{
				_logger.Error("Error occurred while Adding Identity in TAC driver. ", e.FormLogEntry()).Wait();
			}

			return false;
		}

		public bool RemoveIdentityFromTacDriver()
		{
			try
			{
				_logger.Info("IAA RemoveIdentityFromTacDriver.").Wait();
				var responseStatus = TaskHelper.RetryOnError(Constants.RetryTacDriverValue, 
					TimeSpan.FromSeconds(Constants.DelayTacDriverValue), 
					_iapInterface.RemoveIdentityRecord).Result;
				if (responseStatus == 0)
					return true;
			}
			catch (AggregateException e)
			{
				_logger.Error("Error occurred while Deleting Identity from TAC driver. ", e.FormLogEntry()).Wait();
			}
			catch (Exception e)
			{
				_logger.Error("Error occurred whileDeleting Identity from TAC driver. ", e.FormLogEntry()).Wait();
			}


			return false;
		}

		public bool TerminateIdentityInTacDriver()
		{
			try
			{
				_logger.Info("IAA TerminateIdentityInTacDriver.").Wait();
				var responseStatus = _iapInterface.TerminateIdentity().Result;
				if (responseStatus == 0)
					return true;
			}
			catch (AggregateException e)
			{
				_logger.Error("Error occurred while Terminating Identity in TAC driver. ", e.FormLogEntry()).Wait();
			}
			catch (Exception e)
			{
				_logger.Error("Error occurred while Terminating Identity in TAC driver. ", e.FormLogEntry()).Wait();
			}

			return false;
		}
	}
}
