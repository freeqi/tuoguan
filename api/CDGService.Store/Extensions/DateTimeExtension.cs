using System;

namespace CDGService.Store.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime OrginTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
         
        /// <summary>
        /// 分钟级的刻度
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToMinuteTick(this DateTime time)
        {
            return (long)Math.Floor((time.ToUniversalTime() - OrginTime).TotalMinutes); 
        }


        /// <summary>
        /// 从分钟刻度转成时间（UTC时间）
        /// </summary>
        /// <param name="timeTick"></param>
        /// <returns></returns>
        public static DateTime FromMinuteTickToTime(this long timeTick)
        {
            return OrginTime.AddMinutes(timeTick);
        }

        /// <summary>
        /// 转换为小时查询串
        /// </summary>
        /// <param name="time">源时间</param>
        /// <returns></returns>
        public static string ToHourQueryString(this DateTime time)
        {
            return time.ToUniversalTime().ToString("yyyyMMddHH0000");
        }

        /// <summary>
        /// 将时间精确到小时
        /// </summary>
        /// <param name="time">源时间</param>
        /// <returns></returns>
        public static DateTime ToHourPricisionTime(this DateTime time)
        {
            var utcTime = time.ToUniversalTime();
            var result = new DateTime(utcTime.Year, utcTime.Month, utcTime.Day, utcTime.Hour, 0, 0, DateTimeKind.Utc);
            return result;
        }

    }
}
