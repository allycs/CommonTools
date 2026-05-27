namespace Allycs.Common
{
    using System;

    /// <summary>
    /// 日期时间扩展方法类，提供便捷的日期时间操作功能
    /// </summary>
    public static class DateTimeExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 将DateTime转换为Unix时间戳（秒）
        /// </summary>
        /// <param name="dateTime">要转换的日期时间</param>
        /// <returns>Unix时间戳</returns>
        /// <remarks>
        /// 如果输入不是UTC时间，会先转换为UTC时间再计算时间戳
        /// </remarks>
        public static int ToUnixTimestamp(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
                dateTime = dateTime.ToUniversalTime();

            var secondsSinceEpoch = (long)(dateTime - UnixEpoch).TotalSeconds;

            if (secondsSinceEpoch < int.MinValue || secondsSinceEpoch > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(dateTime), "日期时间超出可转换范围");

            return (int)secondsSinceEpoch;
        }

        /// <summary>
        /// 将DateTime转换为长Unix时间戳（毫秒）
        /// </summary>
        /// <param name="dateTime">要转换的日期时间</param>
        /// <returns>长Unix时间戳（毫秒）</returns>
        public static long ToUnixTimestampMillis(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
                dateTime = dateTime.ToUniversalTime();

            return (long)(dateTime - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// 将Unix时间戳转换为DateTime
        /// </summary>
        /// <param name="timestamp">Unix时间戳（秒）</param>
        /// <returns>DateTime对象（本地时间）</returns>
        public static DateTime FromUnixTimestamp(int timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp).ToLocalTime();
        }

        /// <summary>
        /// 将长Unix时间戳（毫秒）转换为DateTime
        /// </summary>
        /// <param name="timestamp">Unix时间戳（毫秒）</param>
        /// <returns>DateTime对象（本地时间）</returns>
        public static DateTime FromUnixTimestampMillis(long timestamp)
        {
            return UnixEpoch.AddMilliseconds(timestamp).ToLocalTime();
        }

        /// <summary>
        /// 检查日期时间是否有效（晚于最小有效日期）
        /// </summary>
        /// <param name="dateTime">要检查的日期时间</param>
        /// <returns>如果日期时间有效返回原值，否则返回最小有效日期</returns>
        public static DateTime EnsureValid(this DateTime dateTime)
        {
            var minDate = new DateTime(1753, 1, 1, 12, 0, 0);
            return dateTime < minDate ? minDate : dateTime;
        }

        /// <summary>
        /// 获取日期时间的中文星期表示
        /// </summary>
        /// <param name="dateTime">要转换的日期时间</param>
        /// <returns>中文星期（如：周一、周二）</returns>
        public static string ToChineseDayOfWeek(this DateTime dateTime)
        {
            string[] weekdays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            return weekdays[(int)dateTime.DayOfWeek];
        }

        /// <summary>
        /// 获取日期时间的中文星期表示（别名方法）
        /// </summary>
        /// <param name="dateTime">要转换的日期时间</param>
        /// <returns>中文星期（如：周一、周二）</returns>
        public static string WeekDayOfChinese(this DateTime dateTime)
        {
            return ToChineseDayOfWeek(dateTime);
        }

        /// <summary>
        /// 获取日期时间的英文星期表示
        /// </summary>
        /// <param name="dateTime">要转换的日期时间</param>
        /// <returns>英文星期（如：Monday、Tuesday）</returns>
        public static string ToEnglishDayOfWeek(this DateTime dateTime)
        {
            return dateTime.DayOfWeek.ToString();
        }

        /// <summary>
        /// 判断是否为今天
        /// </summary>
        /// <param name="dateTime">要检查的日期时间</param>
        /// <returns>如果是今天返回true，否则返回false</returns>
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }

        /// <summary>
        /// 判断是否为昨天
        /// </summary>
        /// <param name="dateTime">要检查的日期时间</param>
        /// <returns>如果是昨天返回true，否则返回false</returns>
        public static bool IsYesterday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(-1);
        }

        /// <summary>
        /// 判断是否为明天
        /// </summary>
        /// <param name="dateTime">要检查的日期时间</param>
        /// <returns>如果是明天返回true，否则返回false</returns>
        public static bool IsTomorrow(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(1);
        }

        /// <summary>
        /// 判断是否为周末（周六或周日）
        /// </summary>
        /// <param name="dateTime">要检查的日期时间</param>
        /// <returns>如果是周末返回true，否则返回false</returns>
        public static bool IsWeekend(this DateTime dateTime)
        {
            return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// 获取日期的开始时间（00:00:00）
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>当天开始时间</returns>
        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// 获取日期的结束时间（23:59:59.999）
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>当天结束时间</returns>
        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// 获取月份的第一天
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>月份第一天</returns>
        public static DateTime StartOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// 获取月份的最后一天
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>月份最后一天</returns>
        public static DateTime EndOfMonth(this DateTime dateTime)
        {
            return StartOfMonth(dateTime).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 获取年份的第一天
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>年份第一天</returns>
        public static DateTime StartOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        /// <summary>
        /// 获取年份的最后一天
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>年份最后一天</returns>
        public static DateTime EndOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 12, 31, 23, 59, 59, 999);
        }

        /// <summary>
        /// 计算两个日期之间的天数差
        /// </summary>
        /// <param name="dateTime">起始日期</param>
        /// <param name="other">结束日期</param>
        /// <returns>天数差（绝对值）</returns>
        public static int DaysDifference(this DateTime dateTime, DateTime other)
        {
            return Math.Abs((dateTime.Date - other.Date).Days);
        }

        /// <summary>
        /// 计算年龄
        /// </summary>
        /// <param name="birthDate">出生日期</param>
        /// <returns>年龄</returns>
        public static int CalculateAge(this DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }
    }
}
