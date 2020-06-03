using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Common
{
    /// <summary>
    /// 键值对解析Helper，修改matchKey作为键值之间的符号，matchValue为键值对之间的符号
    /// </summary>
    public static class KeyValueHelper
    {
        static Dictionary<object, object> conents = new Dictionary<object, object>();
        public static string matchKey { get; set; }
        public static string matchValue { get; set; }

        /// <summary>
        /// 解析输入Bytes中的键值对
        /// </summary>
        /// <param name="data">输入字节数组</param>
        /// <returns>解析后的键值对字典</returns>
        public static Dictionary<object, object> GetConentByBytes(byte[] data)
        {
            conents.Clear();
            conents = GetConentByString(Encoding.Default.GetString(data));
            return conents;
        }

        /// <summary>
        /// 解析输入字符串中的键值对
        /// </summary>
        /// <param name="data">输入字符串</param>
        /// <returns>解析后的键值对字典</returns>
        public static Dictionary<object, object> GetConentByString(string data)
        {
            conents.Clear();

            //Predicate<string> matchEqual = delegate(string value)
            //{
            //    return value == "=" ? true : false;
            //};

            //Predicate<string> matchComma = delegate(string value)
            //{
            //    return value == "," ? true : false;
            //};

            if (data.Substring(data.Length - 1) != matchValue)
            {
                data = data + matchValue;
            }

            try
            {
                int pos = 0;
                int startIndex = 0;
                while (true)
                {
                    //Get Key
                    pos = data.IndexOf(matchKey, startIndex);
                    string key = data.Substring(startIndex, pos - startIndex);
                    startIndex = pos + 1;
                    //Get Value
                    pos = data.IndexOf(matchValue, startIndex);
                    string value = data.Substring(startIndex, pos - startIndex);
                    startIndex = pos + 1;
                    conents.Add(key, value);

                    if (startIndex >= data.Length)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Info: " + ex.ToString());
            }

            return conents;
        }
    }
}
