using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AA.Core.Common
{
	public static class TaskHelper
	{
		public static async Task<T> RetryOnException<T>(
		   int times, TimeSpan delay, Func<T, Task<T>> operation, T args)
		{
			if (times <= 0)
				throw new ArgumentOutOfRangeException(nameof(times));

			var exceptions = new List<Exception>();
			var attempts = 0;
			do
			{
				try
				{
					attempts++;
					return await operation(args);
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);

					if (attempts == times)
						throw new AggregateException(exceptions);

					await Task.Delay(delay);
				}
			} while (true);
		}

		public static async Task<T> RetryOnException<T>(
		   int times, TimeSpan delay, Func<Task<T>> operation)
		{
			if (times <= 0)
				throw new ArgumentOutOfRangeException(nameof(times));

			var exceptions = new List<Exception>();
			var attempts = 0;
			do
			{
				try
				{
					attempts++;
					return await operation();
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);

					if (attempts == times)
						throw new AggregateException(exceptions);

					await Task.Delay(delay);
				}
			} while (true);
		}

		public static async Task<int> RetryOnError<T>(
		   int times, TimeSpan delay, Func<T, Task<int>> operation, T args)
		{
			if (times <= 0)
				throw new ArgumentOutOfRangeException(nameof(times));

			var exceptions = new List<Exception>();
			var attempts = 0;
			do
			{
				try
				{
					attempts++;
					var rc = await operation(args);
					if (rc != 0)
						throw new Exception($"Operation returned invalid status: {rc}");

					return 0;
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);

					if (attempts == times)
						throw new AggregateException(exceptions);

					await Task.Delay(delay);
				}
			} while (true);
		}

		public static async Task<int> RetryOnError(
		   int times, TimeSpan delay, Func<Task<int>> operation)
		{
			if (times <= 0)
				throw new ArgumentOutOfRangeException(nameof(times));

			var exceptions = new List<Exception>();
			var attempts = 0;
			do
			{
				try
				{
					attempts++;
					var rc = await operation();
					if (rc != 0)
						throw new Exception($"Operation returned invalid status: {rc}");

					return 0;
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);

					if (attempts == times)
						throw new AggregateException(exceptions);

					await Task.Delay(delay);
				}
			} while (true);
		}

		public static async Task<T> SendWithDelay<T>(TimeSpan delay, Func<Task<T>> operation)
		{
			try
			{
				await Task.Delay(delay);
				return await operation();
			}
			catch
			{
				throw;
				//TODO: write some logic depending of exception type, and/or exceptions count - if needed
				//return await SendWithDelay(delay, operation);
			}
		}

		public static T SendWithTimeout<T>(TimeSpan timeout, Func<T> operation)
		{
			var task = Task.Run(operation);
			if (task.Wait(timeout))
			{
				var rc = task.Result;
				return rc;
			}
			else
			{
				throw new Exception($"Timed out calling method {operation.GetMethodInfo().Name}");
			}
		}
	}
}
