using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace AA.Windows.IdentityKeyGen
{
	public static class Extensions
	{
		public static bool TryTimeSpanParse(this string @this, out TimeSpan returnValue)
		{
			returnValue = default(TimeSpan);
			if (@this == null)
			{
				return false;
			}
			returnValue = default(TimeSpan);
			if (TimeSpan.TryParseExact(@this, new[] {
								@"%h\:%m\:%s\.FFF",
								@"%h\:%m\:%s",
								@"%h\:%m",
								@"s",
								@"s\.FFF"
						}, CultureInfo.CurrentCulture, out returnValue) == true)
			{
				return true;
			}

			int intValue;
			if (int.TryParse(@this, out intValue) == true)
			{
				returnValue = TimeSpan.FromSeconds(intValue);
				return true;
			}

			return false;
		}


		public static bool TryAssign(this PropertyInfo @this, object instance, string parameterHint, string value)
		{
			try
			{
				MethodInfo setter = @this?.GetSetMethod();
				if (setter == null)
				{
					Console.WriteLine($"Configuration option '{parameterHint}' cannot be configured, unable to locate set method.");
					return false;
				}

				var propertyType = @this.PropertyType;
				if (propertyType == typeof(bool))
				{
					bool convertedValue;
					if (bool.TryParse(value, out convertedValue))
					{
						setter.Invoke(instance, new object[] { convertedValue });
					}
				}
				else if (propertyType == typeof(int))
				{
					int convertedValue;
					if (int.TryParse(value, out convertedValue))
					{
						setter.Invoke(instance, new object[] { convertedValue });
					}
				}
				else if (propertyType == typeof(TimeSpan))
				{
					TimeSpan convertedValue;
					if (TryTimeSpanParse(value, out convertedValue))
					{
						setter.Invoke(instance, new object[] { convertedValue });
					}
				}
				else if (propertyType == typeof(IEnumerable<string>))
				{
					setter.Invoke(instance, new object[] { value.Split(',').Select(_ => _?.Trim()).Where(_ => string.IsNullOrWhiteSpace(_) == false) });
				}
				else
				{
					setter.Invoke(instance, new[] { value });
				}

				return true;
			}
			catch
			{
				//Logger.Write(Level.Error, $"Cannot assign configuration option '{parameterHint}' due to an exception.", ex, new
				//{
				//    value = value,
				//    type = @this.DeclaringType.FullName,
				//    property = @this?.Name
				//});
				return false;
			}
		}
	}
}
