using System;

namespace ZStart.EBook.Common
{
    public class Utility
    {
        public static string FormatTime(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
        }
    }
}
