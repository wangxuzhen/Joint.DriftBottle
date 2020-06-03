using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Common
{
    public class TypeHelper
    {
        public static bool ObjectToBool(object o)
        {
            return ObjectToBool(o, false);
        }

        public static bool ObjectToBool(object o, bool defaultValue)
        {
            if (o != null)
            {
                return StringToBool(o.ToString(), defaultValue);
            }
            return defaultValue;
        }

        public static DateTime ObjectToDateTime(object o)
        {
            return ObjectToDateTime(o, DateTime.Now);
        }

        public static DateTime ObjectToDateTime(object o, DateTime defaultValue)
        {
            if (o != null)
            {
                return StringToDateTime(o.ToString(), defaultValue);
            }
            return defaultValue;
        }

        public static decimal ObjectToDecimal(object o)
        {
            return ObjectToDecimal(o, 0M);
        }

        public static decimal ObjectToDecimal(object o, decimal defaultValue)
        {
            if (o != null)
            {
                return StringToDecimal(o.ToString(), defaultValue);
            }
            return defaultValue;
        }

        public static int ObjectToInt(object o)
        {
            return ObjectToInt(o, 0);
        }

        public static int ObjectToInt(object o, int defaultValue)
        {
            if (o != null)
            {
                return StringToInt(o.ToString(), defaultValue);
            }
            return defaultValue;
        }

        public static bool StringToBool(string s, bool defaultValue)
        {
            if (s.ToLower() == "false")
            {
                return false;
            }
            return ((s.ToLower() == "true") || defaultValue);
        }

        public static DateTime StringToDateTime(string s)
        {
            return StringToDateTime(s, DateTime.Now);
        }

        /// <summary>
        /// 字符串转换成时分秒开始时间（如2016-08-01 00:00:00）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime StrToBeginDateTime(string s)
        {
            DateTime dt =StringToDateTime(s, DateTime.Now);
            string strDate = dt.ToString("yyyy-MM-dd 00:00:00");
            return Convert.ToDateTime(strDate);
        }

        /// <summary>
        /// 默认时间是1970-1-1
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime UDateTime(object obj)
        {
            try
            {
                return DateTime.Parse(obj.ToString());
            }
            catch
            {
                return DateTime.Parse("1970-01-01 00:00:00");
            }
        }

        /// <summary>
        /// 字符串转换成时分秒结束时间（如2016-08-01 23:59:59）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime StrToEndDateTime(string s)
        {
            DateTime dt = StringToDateTime(s, DateTime.Now);
            string strDate = dt.ToString("yyyy-MM-dd 23:59:59");
            return Convert.ToDateTime(strDate);
        }

        public static DateTime StringToDateTime(string s, DateTime defaultValue)
        {
            DateTime time;
            if (!string.IsNullOrWhiteSpace(s) && DateTime.TryParse(s, out time))
            {
                return time;
            }
            return defaultValue;
        }




        public static decimal StringToDecimal(string s)
        {
            return StringToDecimal(s, 0M);
        }

        public static decimal StringToDecimal(string s, decimal defaultValue)
        {
            decimal num;
            if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static int StringToInt(string s)
        {
            return StringToInt(s, 0);
        }

        public static int StringToInt(string s, int defaultValue)
        {
            int num;
            if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static bool ToBool(string s)
        {
            return StringToBool(s, false);
        }
    }
}
