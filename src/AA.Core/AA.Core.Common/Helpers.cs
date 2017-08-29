using System;
using System.Linq;
using System.Net;

namespace AA.Core.Common
{
	public static class Helpers
	{
		public static double GetCurrentTime()
		{
			TimeSpan currentTime = (DateTime.UtcNow - new DateTime(1970, 1, 1));
			return currentTime.TotalSeconds;
		}

		public static uint GetAddress(string ipAdd)
		{
			var ipAddress = IPAddress.Parse(ipAdd);
			var ipBytes = ipAddress.GetAddressBytes();
			var ip = (uint)ipBytes[3] << 24;
			ip += (uint)ipBytes[2] << 16;
			ip += (uint)ipBytes[1] << 8;
			ip += ipBytes[0];
			return ip;
		}

		public static uint Ip2UInt(string ip)
		{
			double num = 0;
			if (!string.IsNullOrEmpty(ip))
			{
				var ipBytes = ip.Split('.');
				for (int i = ipBytes.Length - 1; i >= 0; i--)
				{
					num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
				}
			}
			return (uint)num;
		}

		public static string UIntToIp(uint uintIp)
		{
			string ip = string.Empty;
			for (int i = 0; i < 4; i++)
			{
				int num = (int)(uintIp / Math.Pow(256, (3 - i)));
				uintIp = uintIp - (uint)(num * Math.Pow(256, (3 - i)));
				if (i == 0)
					ip = num.ToString();
				else
					ip = ip + "." + num.ToString();
			}
			return ip;
		}

		public static string GetOsVersionInfo()
		{
			var osDescriptionsInfo =
				System.Runtime.InteropServices.RuntimeInformation.OSDescription.Split(' ').Where(x => x != "");
			return osDescriptionsInfo.Last();
		}

		public static string GetOsInfo()
		{
			var osDescriptionsInfo = System.Runtime.InteropServices.RuntimeInformation.OSDescription
				.Split(' ')
				.Where(x => x != "")
				.ToList();
			osDescriptionsInfo.RemoveAt(osDescriptionsInfo.Count() - 1);
			return string.Join(" ", osDescriptionsInfo);
		}
	}
}
