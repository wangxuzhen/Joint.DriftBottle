using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Joint.Common
{
    public class JeasuHelper
    {
        /// <summary>
        /// 项目二级名称
        /// </summary>
        public static string SubProjectName = "Admin";
       

        /// <summary>
        /// 验证验证码是否正确
        /// </summary>
        /// <returns></returns>
        public static bool ValidateMachineCode()
        {
            return JeasuHelper.CalculateMachineCode == JeasuHelper.ThisMachineCode;
        }

        /// <summary>
        /// 机器码
        /// </summary>
        public static string ThisMachineCode { get; set; }

        /// <summary>
        /// 计算后的机器码
        /// </summary>
        public static string CalculateMachineCode { get; set; }

        /// <summary>
        /// 在指定的范围获取随机数个数
        /// </summary>
        /// <param name="num">随机数个数</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns></returns>
        public static List<int> GetRandomNum(int num, int minValue, int maxValue)
        {
            List<int> intList = new List<int>();
            Random random = new Random();
            maxValue = maxValue + 1;
            //循环的次数  
            int Nums = num;
            while (Nums > 0)
            {
                int i = random.Next(minValue, maxValue);
                if (!intList.Contains(i))
                {
                    if (intList.Count < num)
                    {
                        intList.Add(i);
                    }
                }
                Nums -= 1;
            }
            return intList;
        }

        private static bool? isDebug = null;

        public static bool IsDebug(bool CheckTempPermission = true)
        {
            //如果需要检查临时权限
            if (CheckTempPermission == true)
            {
                if (HasTempPermission == true)
                {
                    return false;
                }
            }

            if (isDebug == null)
            {
                //如果是测试环境，则不执行定时任务，防止本地调试的时候给客户乱发提醒
                if (string.Equals(ConfigurationManager.AppSettings["IsDebug"], "true", StringComparison.OrdinalIgnoreCase)
                    && !System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("/AutoLogin.lock")))
                {
                    isDebug = true;
                }
                else
                {
                    isDebug = false;
                }
            }
            return (bool)isDebug;
        }

        //public static bool isTest = false;
        public static bool IsTest(string hostname)
        {
            if (hostname.Contains(".localhost") || hostname.Contains(".yomi"))
            {
                return true;
            }
            return false;
        }

        public static bool HasTempPermission
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["TempPermission"] != null)
                {
                    if (HttpContext.Current.Session["TempPermission"].ToString().ToLower() == "true")
                    {
                        return true;
                    }
                }
                return false;
            }
        }

    }
}
