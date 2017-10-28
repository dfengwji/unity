using System;
using ZStart.Core.Manager;
namespace ZStart.Core.Util
{
    public class DateUtil
    {
        private DateUtil()
        {

        }

        public static void CalculateTime(long seconds,out int hours,out int min)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            if (seconds > 24 * 3600)
            {
                hours = (int)(seconds / 3600);
                min = (int)((seconds % 3600) / 60);
            }
            else if(seconds > 3600)
            {
                hours = t.Hours;
                min = t.Minutes;
            }
            else
            {
                hours = 0;
                min = (int)(seconds / 60);
            }
        }

        public static TimeSpan DateDiff(DateTime dateTime1, DateTime dateTime2)
        {
            try
            {
                TimeSpan ts1 = new TimeSpan(dateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(dateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                return ts1.Subtract(ts2).Duration();
            }
            catch
            {
            }
            return new TimeSpan();
        }

        public static string DateDiff(DateTime history)
        {
            DateTime today = DateTime.Now;
            if (history == null)
                return ZLanguageManager.GetPopupwin("Time.Standard", today.Year, today.Month, today.Day, today.Hour, today.Minute);
            
            TimeSpan span = DateDiff(today, history);
            if (span.Days > 60)
            {
                return ZLanguageManager.GetPopupwin("Time.Standard", history.Year, history.Month, history.Day, history.Hour, history.Minute);
            }
            else if (span.Days > 30)
            {
                return ZLanguageManager.GetPopupwin("Time.Diff.Month", 1);
            }
            else if (span.Days > 0)
            {
                return ZLanguageManager.GetPopupwin("Time.Diff.Day", span.Days);
            }
            else if (span.Hours > 0)
            {
                return ZLanguageManager.GetPopupwin("Time.Diff.Hour", span.Hours);
            }
            else if (span.Minutes > 10)
            {
                return ZLanguageManager.GetPopupwin("Time.Diff.Minute", span.Minutes);
            }
            else
            {
                return ZLanguageManager.GetPopupwin("Time.Diff.Latest");
            }
        }

        public static string DateDiff(long time)
        {
            DateTime tmp = UnixToDateTime(time);
            return DateDiff(tmp);
        }

        /// <summary>
        /// 2006-01-02 15:04:05
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string DateDiff(string time)
        {
            if (string.IsNullOrEmpty(time)) return "";
            DateTime history = Convert.ToDateTime(time);
            return DateDiff(history);
        }

        public static string FormatTime(long seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            string time = "";
            if (seconds > 24 * 3600)
            {
                long hour = seconds / 3600;
                long min = (seconds % 3600) / 60;
                long sec = (seconds % 3600) % 60;
                time = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, min, sec);
            }
            else if (seconds > 3600)
            {
                time = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
            }
            else if (seconds > 60)
            {
                long sec = seconds % 60;
                long min = seconds / 60;
                // time = string.Format("{0:D2}:{1:D2}", sec, min);
                time = string.Format("{1:D2}:{0:D2}", sec, min);
            }
            else
            {
                time = string.Format("{0:D1}", seconds);
            }
            return time;
        }

        public static string GetDate()
        {
            DateTime time = DateTime.Now;

            return ZLanguageManager.GetPopupwin("Date.Year.Month", time.Year, time.Month);
        }

        public static string GetTimeStamp()
        {
            DateTime time = DateTime.Now;
            //TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return time.Hour + ":" + time.Minute + ":" + time.Second;
        }

        public static int GetDiffDays(string start,string end)
        {
            if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
                return 0;
            DateTime dt1 = Convert.ToDateTime(start);
            DateTime dt2 = Convert.ToDateTime(end);
            TimeSpan span = dt2.Subtract(dt1);
            return span.Days; 
        }

        public static DateTime FormatDate(string time)
        {
            if (string.IsNullOrEmpty(time))
                return DateTime.Now;
            return Convert.ToDateTime(time);
        }

        public static double DateTimeToUnix(DateTime dateTime)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            //long t = (dateTime.Ticks - startTime.Ticks)/10000;            //除10000调整为13位
            return (dateTime - startTime).TotalSeconds;
        }

        /// <summary>
        /// unix时间戳转换成日期
        /// </summary>
        /// <param name="unixTimeStamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime UnixToDateTime(double timestamp)
        {
            DateTime startTime = new DateTime(1970, 1, 1);
            return startTime.AddSeconds(timestamp);
        }

        public static string GetNowStamp(long seconds)
        {
            DateTime daTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime newDay = daTime.AddSeconds(seconds);

            string nowTime = newDay.ToString();

            string[] timeGroup = nowTime.Split(' ');
            string[] yearGroup = timeGroup[0].Split('/');

            string time = "00";

            if (nowTime.Contains("AM") && timeGroup[1].Substring(0, 2) == "12")
                time = "00" + timeGroup[1].Substring(2);
            else if (nowTime.Contains("AM") && timeGroup[1].Substring(1, 1) == ":")
                time = "0" + timeGroup[1];
            else if (nowTime.Contains("PM"))
            {
                string[] timePM = timeGroup[1].Split(':');
                timePM[0] = (int.Parse(timePM[0]) + 12).ToString();
                time = timePM[0] + ":" + timePM[1] + ":" + timePM[2];
            }
            else
            {
                time = timeGroup[1];
            }


            nowTime = yearGroup[2] + "-" + yearGroup[0] + "-" + yearGroup[1] + " " + time;

            return nowTime;
        }  
    }
}
