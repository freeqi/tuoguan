using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Utils
{
    public static class DateTimeHelper
    {
        public static DateTime ToDayLastMinitue(this DateTime time)
        {
            return time.Date.AddHours(23).AddMinutes(59);
            
        }

        public static string ShortDateString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }
        public static string LongDateString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
    }

   
}
