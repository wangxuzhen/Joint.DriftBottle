using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Joint.Web
{
    public class UrlParamsHelper
    {
        public static string GetProperties<T>(T t)
        {
            string tStr = string.Empty;
            if (t == null)
            {
                return tStr;
            }
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return tStr;
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(t, null);
                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    //过滤pageindex的这个参数
                    if (name != "PageIndex" && value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        tStr += string.Format("&{0}={1}", name, value);
                    }
                }
                else
                {
                    GetProperties(value);
                }
            }
            return tStr;
        }

        //public static void Move(string tempPath,string descDirPath)
        //{
        //    tempPath = Server.MapPath("~" + tempPath);
        //    string ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
        //    descDirPath = Server.MapPath("~" + "/Upload/storeImage/" + ymd + "/");

        //    FileHelper.Move(tempPath, descDirPath);
        //}

    }
}