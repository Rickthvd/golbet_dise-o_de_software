using System;

namespace GolBet.Services.Helpers
{
    public static class DateTimeExtensions
    {
        private static readonly TimeZoneInfo ColombiaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "SA Pacific Standard Time" : "America/Bogota");

        public static DateTime ToColombiaTime(this DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, ColombiaTimeZone);
        }

        public static DateTime ToUtcFromColombia(this DateTime colombiaDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(colombiaDateTime, ColombiaTimeZone);
        }
    }
}