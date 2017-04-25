/**
 * This file is part of JLUtils library
 * (C) Jeremy Lam "JLChnToZ" 2016-2017.
 * Released under MIT License.
 */
using System;

namespace Utils {
    public static class DateTimeHelper {
        private const long maxTicks = 3155378975999999999L;

        private const long epoch0Ticks = 621355968000000000L;
        private const long minUnixTime = -62135596800L;
        private const long maxUnixTime = 253402300800L;

        public static long ToUnixTime(this DateTime source) {
            if(source.Kind == DateTimeKind.Local)
                source = source.ToUniversalTime();
            return (source.Ticks - epoch0Ticks) / TimeSpan.TicksPerSecond;
        }

        public static double ToUnixTimeFloat(this DateTime source) {
            if(source.Kind == DateTimeKind.Local)
                source = source.ToUniversalTime();
            return (double)(source.Ticks - epoch0Ticks) / TimeSpan.TicksPerSecond;
        }

        public static DateTime FromUnixTime(long unixTime) {
            if(unixTime < minUnixTime || unixTime > maxUnixTime)
                throw new ArgumentOutOfRangeException(
                    "unixTime",
                    unixTime,
                    string.Format("The input unix time is out of supported range (between {0} and {1}, provided {2}).", minUnixTime, maxUnixTime, unixTime)
                );
            return new DateTime(epoch0Ticks + unixTime * TimeSpan.TicksPerSecond, DateTimeKind.Utc);
        }

        public static DateTime FromUnixTime(double unixTime) {
            if(unixTime < minUnixTime || unixTime > maxUnixTime)
                throw new ArgumentOutOfRangeException(
                    "unixTime",
                    unixTime,
                    string.Format("The input unix time is out of supported range (between {0} and {1}, provided {2}).", minUnixTime, maxUnixTime, unixTime)
                );
            long ticks = epoch0Ticks + (long)Math.Round(unixTime * TimeSpan.TicksPerSecond);
            if(ticks > maxTicks) ticks = maxTicks;
            else if(ticks < 0) ticks = 0;
            return new DateTime(ticks, DateTimeKind.Utc);
        }
    }
}
