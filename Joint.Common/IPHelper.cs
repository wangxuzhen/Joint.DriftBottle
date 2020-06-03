using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Common
{
    public class IPHelper
    {
        public static IpInfoModel GetIpInfoByIP(string strIP)
        {
            //如果是本地IP则不需要请求了，别浪费次数
            if (strIP.Contains("::"))
            {
                return null;
            }

            string regionJson = Common.HttpClientHelper.GetResponseJson("http://ip.taobao.com/service/getIpInfo.php?ip=" + strIP);
            try
            {
                //dynamic jsonData = DynamicHelper.JsonToDynamic(regionJson);
                //string region = jsonData.data.region;
                //string city = jsonData.data.city;
                //string county = jsonData.data.county;
                //string isp = jsonData.data.isp;
                IpInfoModel ipinfoModel = regionJson.FromJson<IpInfoModel>();
                return ipinfoModel;

            }
            catch
            {

            }
            return null;
        }

        public static string GetAreaByIP(string strIP)
        {
            IpInfoModel ipInfo = GetIpInfoByIP(strIP);
            if (ipInfo == null)
            {
                return "区域获取失败";
            }

            string strArea = ipInfo.data.region + "." +
                ipInfo.data.city + "." +
                ipInfo.data.isp;
            return strArea;
        }
    }
}
