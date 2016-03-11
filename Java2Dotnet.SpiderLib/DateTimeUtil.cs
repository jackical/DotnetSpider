﻿using System;

namespace Java2Dotnet.Spider.Lib
{
	public static class DateTimeUtil
	{
		/// <returns></returns>
		public static string GetCurrentTimeStampString()
		{
			return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString("f0");
		}

		/// <returns></returns>
		public static double GetCurrentTimeStamp()
		{
			return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		}

		private static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
		private const long TicksPerMicrosecond = 10;

		public static string RunIdStr = DateTime.Now.ToString("yyyy_MM");
		public static string LastRunIdStr = DateTime.Now.AddMonths(-1).ToString("yyyy_MM");
		public static string TodayRunId = DateTime.Now.Date.ToString("yyyy_MM_dd");
		public static DateTime RunIdtoMinute = DateTime.Now.ToLocalTime();
		//public static DateTime MONTHLY_RUN_ID = DateTime.Now.Date.Month;

		static DateTimeUtil()
		{
			var now = DateTime.Now.Date;

			FirstDayofThisMonth = now.AddDays(now.Day * -1 + 1);

			LastDayofThisMonth = FirstDayofThisMonth.AddMonths(1).AddDays(-1);

			FirstDayofLastMonth = FirstDayofThisMonth.AddMonths(-1);

			LastDayofLastMonth = FirstDayofThisMonth.AddDays(-1);

			FirstDayofThisWeek = now.AddDays(Convert.ToInt32(now.DayOfWeek.ToString("d")) * -1);

			FirstDayofLastWeek = FirstDayofThisWeek.AddDays(-7);

			LastDayofThisWeek = FirstDayofThisWeek.AddDays(6);

			LastDayofLastWeek = FirstDayofThisWeek.AddDays(-1);
		}

		public static DateTime FirstDayofThisMonth { get; }
		public static DateTime LastDayofThisMonth { get; private set; }
		public static DateTime FirstDayofLastMonth { get; private set; }
		public static DateTime LastDayofLastMonth { get; private set; }
		public static DateTime FirstDayofLastWeek { get; private set; }
		public static DateTime FirstDayofThisWeek { get; }
		public static DateTime LastDayofThisWeek { get; private set; }
		public static DateTime LastDayofLastWeek { get; private set; }

		public static DateTime GetFirstDayofMoth(DateTime selectDate)
		{
			return selectDate.AddDays(selectDate.Day * -1 + 1).Date;
		}

		/// <summary>
		/// Returns the number of microseconds since Epoch of the current UTC date and time.
		/// </summary>
		public static long UtcNow => FromDateTimeOffset(DateTimeOffset.UtcNow);

		/// <summary>
		/// Converts the microseconds since Epoch time provided to a DateTimeOffset.
		/// </summary>
		public static DateTimeOffset ToDateTimeOffset(long microsecondsSinceEpoch)
		{
			return Epoch.AddTicks(microsecondsSinceEpoch * TicksPerMicrosecond);
		}

		/// <summary>
		/// Converts the DateTimeOffset provided to the number of microseconds since Epoch.
		/// </summary>
		public static long FromDateTimeOffset(DateTimeOffset dateTimeOffset)
		{
			return dateTimeOffset.Subtract(Epoch).Ticks / TicksPerMicrosecond;
		}

		/// <summary>
		/// Converts the DateTimeOffset provided to the number of microseconds since Epoch.
		/// </summary>
		public static long ToMicrosecondsSinceEpoch(this DateTimeOffset dateTimeOffset)
		{
			return FromDateTimeOffset(dateTimeOffset);
		}
	}
}
