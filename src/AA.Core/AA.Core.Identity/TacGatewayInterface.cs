using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AA.Core.Common;
using AA.Core.Common.MessageEntities;

namespace AA.Core.Identity
{
	public class TacGatewayInterface
	{
		private readonly Configuration _configuration;
		private readonly Logger _logger;

		public delegate bool TokenReceivedHandler(object sender, TacGatewayArgs args);
		public event TokenReceivedHandler TokenReceived;

		public TacGatewayInterface(Configuration configuration, Logger logger)
		{
			_configuration = configuration;
			_logger = logger;
		}

		/// <summary>
		/// Event OnInitializeTokenReceived
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		protected virtual bool OnTokenReceived(TacGatewayArgs args)
		{
			var success = true;

			if (TokenReceived != null)
				success = TokenReceived(this, args);

			return success;
		}

		/// <summary>
		/// Sends initialize request to TAC GW
		/// In case of an error it will try to resend the same request number of times defined in configuration
		/// </summary>
		/// <returns></returns>
		public bool Initialize()
		{			
			GatewayIdentity gatewayIdentity;
			try
			{
				_logger.Info("Initialize requesting.").Wait();
				var response =
					TaskHelper.RetryOnException(_configuration.GatewayEndPoint.GatewayInitialize.MaximumRetries,
						_configuration.GatewayEndPoint.GatewayInitialize.InitializeRetryInterval,
						SendInitializeRequest).Result;
				gatewayIdentity = GatewayIdentity.ParseToTacGatewayResponse(response);

				if (gatewayIdentity.Key_Len != gatewayIdentity.KeyArray.Length)
					_logger.Warn($"Key_len value is {gatewayIdentity.Key_Len} and key Length is {gatewayIdentity.KeyArray.Length}").Wait();

				_logger.Info("Initialize finished.").Wait();
			}
			catch (AggregateException e)
			{
				_logger.Error("Error occurred while calling Initialize API of TAC Gateway. ", e.FormLogEntry()).Wait();
				return false;
			}
			catch (HttpRequestException e)
			{
				_logger.Error("Error occurred while calling Initialize API of TAC Gateway. ", e.FormLogEntry()).Wait();
				return false;
			}
			catch (Exception e)
			{
				_logger.Error("Error occurred while calling Initialize API of TAC Gateway. ", e.FormLogEntry()).Wait();
				return false;
			}

			var args = new TacGatewayArgs
			{
				GatewayIdentity = gatewayIdentity
			};

			return OnTokenReceived(args);
		}

		/// <summary>
		/// Sends Refresh request to TAC GW
		/// In case of an error it will try to send same request number of times defined in configuration
		/// </summary>
		/// <returns></returns>
		public bool Refresh()
		{

			GatewayIdentity gatewayIdentity;
			try
			{
				_logger.Info("Refresh requesting.").Wait();
				var response =
					TaskHelper.SendWithDelay(_configuration.GatewayEndPoint.GatewayRefresh.RefreshInterval,
						SendRefreshRequest).Result;
				gatewayIdentity = GatewayIdentity.ParseToTacGatewayResponse(response);

				if(gatewayIdentity.Key_Len != gatewayIdentity.KeyArray.Length)
					_logger.Warn($"Key_len value is {gatewayIdentity.Key_Len} and key Length is {gatewayIdentity.KeyArray.Length}").Wait();

				_logger.Info("Refresh finished.").Wait();
			}
			catch (AggregateException e)
			{
				_logger.Error("Error occurred while calling Refresh API of TAC Gateway. ", e.FormLogEntry()).Wait();
				return false;
			}
			catch (HttpRequestException e)
			{
				_logger.Error("Error occurred while calling Refresh API of TAC Gateway. ", e.FormLogEntry()).Wait();
				return false;
			}
			catch (Exception e)
			{
				_logger.Error("Error occurred while calling Refresh API of TAC Gateway. ", e.FormLogEntry()).Wait();
				return false;
			}

			var args = new TacGatewayArgs
			{
				GatewayIdentity = gatewayIdentity
			};

			return OnTokenReceived(args);
		}

		#region private methods

		/// <summary>
		/// Create message body, add certificates configure URI and 
		/// Sends Initialize Request to GW API
		/// </summary>
		/// <returns></returns>
		private async Task<string> SendInitializeRequest()
		{
			string responseData;
			var uri = _configuration.GatewayEndPoint.InitializeUri;
			string messageBody = GetInitializeRequestBody();
			HttpClientHandler handler = new HttpClientHandler();
			handler.ClientCertificates.Add(_configuration.IdentityActivationAgentCertificate);
			handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, policyErrors) =>
			{
				if (_configuration.GatewayEndPoint.VerifySsl)
				{
					return policyErrors == System.Net.Security.SslPolicyErrors.None;
				}

				return true;
			};

			using (HttpClient client = new HttpClient(handler))
			using (
				HttpResponseMessage response = await client.PostAsync(uri,
					new StringContent(messageBody, Encoding.UTF8, "application/json")))
			{
				if (response.IsSuccessStatusCode)
				{
					using (HttpContent content = response.Content)
					{
						responseData = content.ReadAsStringAsync().Result;
					}
				}
				else
				{
					throw new HttpRequestException(
						$"Failed to send initialize request to BR TAC gateway. Gateway response status code: {response.StatusCode}");
				}
			}

			return responseData;
		}

		/// <summary>
		/// Create message body, add certificates configure URI and 
		/// Sends Refresh Request to GW API
		/// </summary>
		/// <returns></returns>
		private async Task<string> SendRefreshRequest()
		{
			//await _logger.Info("Refresh requesting.");
			string responseData;
			var uri = _configuration.GatewayEndPoint.RefreshUri;
			var messageBody = GetRefreshRequestBody();
			HttpClientHandler handler = new HttpClientHandler();
			handler.ClientCertificates.Add(_configuration.IdentityActivationAgentCertificate);
			handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, policyErrors) =>
			{
				if (_configuration.GatewayEndPoint.VerifySsl)
				{
					return policyErrors == System.Net.Security.SslPolicyErrors.None;
				}

				return true;
			};

			using (HttpClient client = new HttpClient(handler))
			using (
				HttpResponseMessage response = await client.PostAsync(uri,
					new StringContent(messageBody, Encoding.UTF8, "application/json")))
			{
				if (response.IsSuccessStatusCode)
				{
					using (HttpContent content = response.Content)
					{
						responseData = content.ReadAsStringAsync().Result;
					}
				}
				else
				{
					throw new HttpRequestException(
						$"Failed to send initialize request to BR TAC gateway. Gateway response status code: {response.StatusCode}");
				}
			}

			return responseData;
		}


		/// <summary>
		/// Creating Initialize request body
		/// in JSON format
		/// </summary>
		/// <returns></returns>
		private string GetInitializeRequestBody()
		{
			return new
			{
				client_ID = string.Join("_", _configuration.CN, _configuration.CertificateHash),
				clock = Helpers.GetCurrentTime(),
				version = Helpers.GetOsVersionInfo(),
				os = Helpers.GetOsInfo(),
				status = Constants.GatewayInitializeRequestStatus
			}.ToJson();
		}

		/// <summary>
		/// Creating Refresh request body
		/// in JSON format
		/// </summary>
		/// <returns></returns>
		private string GetRefreshRequestBody()
		{
			return new
			{
				clock = Helpers.GetCurrentTime(),
				status = Constants.GatewayInitializeRequestStatus
			}.ToJson();
		}

		#endregion
	}
}
