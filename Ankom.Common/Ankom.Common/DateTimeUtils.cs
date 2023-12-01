using System;

namespace Ankom.Common
{
    public static class DateTimeUtils
    {
        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }

        public static DateTime JavaTimeStampToDateTime(double javaTimeStamp)
        {
            // Java timestamp is millisecods past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp, double? timezone = null, bool UTC = false)
        {
            DateTime origin = UnixTimeStampToDateTime(timestamp);

            return (UTC) ? origin : ((timezone.HasValue) ? origin.AddHours(timezone.Value) : origin.ToLocalTime());
        }

        public static DateTime ConvertFromJavaTimestamp(double timestamp, double? timezone = null, bool UTC = false)
        {
            DateTime origin = JavaTimeStampToDateTime(timestamp);

            return (UTC) ? origin : ((timezone.HasValue) ? origin.AddHours(timezone.Value) : origin.ToLocalTime());
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (date.Kind == DateTimeKind.Local)
                date = date.ToUniversalTime();
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static double ConvertToJavaTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (date.Kind == DateTimeKind.Local)
                date = date.ToUniversalTime();
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalMilliseconds);
        }

    }
}
