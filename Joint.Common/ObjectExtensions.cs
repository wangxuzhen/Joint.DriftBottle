using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Common
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// json序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            T local = default(T);
            try
            {
                local = JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                throw new Exception("序列化JSON数据失败,JSON数据:" + json);
            }
            return local;
        }

        /// <summary>
        /// 对象序列化成json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
