using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace AA.Core.Common
{
	public static class Extensions
	{
		/// <summary>
		/// Converts stream to byte[]
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static byte[] ToByteArray(this Stream @this)
		{
			using (var stream = new MemoryStream())
			{
				@this.CopyTo(stream);
				return stream.ToArray();
			}
		}

		/// <summary>
		/// Parsing string to boolean value
		/// 0 is false
		/// no/No is false
		/// 1 is true
		/// yes/YES is true
		/// </summary>
		/// <param name="str"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryParseToBoolean(this string str, out bool value)
		{
			value = false;
			bool parseSucceeded;
			parseSucceeded = false;

			if (!string.IsNullOrEmpty(str))
			{
				if (str.Equals("0"))
				{
					parseSucceeded = true;
					value = false;
				}

				if (str.Equals("1"))
				{
					parseSucceeded = true;
					value = true;
				}

				if (str.ToLower() == "yes")
				{
					parseSucceeded = true;
					value = true;
				}

				if (str.ToLower() == "no")
				{
					parseSucceeded = true;
					value = false;
				}

				if (str.ToLower() == "true")
				{
					parseSucceeded = true;
					value = true;
				}

				if (str.ToLower() == "false")
				{
					parseSucceeded = true;
					value = false;
				}
			}

			return parseSucceeded;
		}

		/// <summary>
		/// Serialize object to JSON
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ToJson(this object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static IEnumerable<Exception> GetExceptionStack(this Exception ex)
		{
			if (ex == null)
			{
				yield break;
			}

			var current = ex;
			do
			{
				var aggregateException = current as AggregateException;
				if (aggregateException != null)
				{
					foreach (var childException in aggregateException.InnerExceptions)
					{
						foreach (var subChildException in GetExceptionStack(childException))
						{
							yield return subChildException;
						}
					}
					//yield return current;
					current = null;
				}
				else
				{
					yield return current;
					current = current.InnerException;
				}
			} while (current != null);
		}

		/// <summary>
		/// Create log message from exception
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static Dictionary<string, string> FormLogEntry(this Exception ex)
		{
			Dictionary<string, string> logEntry = new Dictionary<string, string>();
			var exceptions = GetExceptionStack(ex).ToList();
			for (int index = 0; index < exceptions.Count; index++)
			{
				var exception = exceptions.Skip(index).First();
				logEntry.Add($"Exception Message  {(exceptions.Count - index)}", exception.Message);
			}

			return logEntry;
		}

		/// <summary>
		/// Checks if exception (or any of inner exceptions) has message
		/// "An unknown error occurred while processing the certificate" or
		/// "A security error occurred"
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static bool IsSecurityException(this Exception ex)
		{
			var exceptions = GetExceptionStack(ex).ToList();
			foreach (var exception in exceptions)
			{
				if (exception.Message.Equals(Constants.ErrorProcessingCertificate, StringComparison.CurrentCultureIgnoreCase) ||
					exception.Message.Equals(Constants.ErrorSecurity, StringComparison.CurrentCultureIgnoreCase))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Converting string to TimeSpan object
		/// </summary>
		/// <param name="this"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static TimeSpan ToTimeSpan(this string @this, TimeSpan? defaultValue = null)
		{
			TimeSpan timeSpan;
			return TryTimeSpanParse(@this, out timeSpan) ? timeSpan : (defaultValue == null ? TimeSpan.Zero : defaultValue.Value);
		}

		/// <summary>
		/// Parsing string to TimeSpan format
		/// </summary>
		/// <param name="this"></param>
		/// <param name="returnValue"></param>
		/// <returns></returns>
		public static bool TryTimeSpanParse(this string @this, out TimeSpan returnValue)
		{
			returnValue = default(TimeSpan);
			if (@this == null)
			{
				return false;
			}
			if (TimeSpan.TryParseExact(@this, new[] {
				@"%h\:%m\:%s\.FFF",
				@"%h\:%m\:%s",
				@"%h\:%m",
				@"s",
				@"s\.FFF"
			}, CultureInfo.CurrentCulture, out returnValue))
			{
				return true;
			}

			if (int.TryParse(@this, out var intValue))
			{
				returnValue = TimeSpan.FromSeconds(intValue);
				return true;
			}

			return false;
		}
	}
}
