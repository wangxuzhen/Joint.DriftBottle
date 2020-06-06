/// <summary>
/// 用于封装Pay个人支付模块
/// Shayuxiang 2020.06.07
/// 用法:
/// var ret = PayjsApp.Instance.SendOrder(100,"123456","测试订单");
/// </summary>
namespace payjstest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    public class PayjsApp
    {
        /// <summary>
        /// 商户号 -- 应当读取配置文件
        /// </summary>
        private static string MchId = "";

        /// <summary>
        /// 商户密匙
        /// </summary>
        private static string Key = "";
        /// <summary>
        /// 创建订单操作单例
        /// </summary>
        private static PayjsApp _instance = null;
        public static PayjsApp Instance => _instance ?? (_instance = new PayjsApp());

        /// <summary>
        /// 请求payjs的地址
        /// </summary>
        private const string url = "https://payjs.cn/api/cashier";

        /// <summary>
        /// 支付成功通知页
        /// </summary>
        private const string notify_url = "";

        /// <summary>
        /// 支付成功跳转
        /// </summary>
        private const string success_url = "";

        /// <summary>
        /// 我们的LOGO地址(文件URL可访问)，预留
        /// </summary>
        private const string logo = "";

        private PayjsApp()
        { }

        /// <summary>
        /// 请求一个订单
        /// </summary>
        /// <param name="fee"></param>
        public string SendOrder(int fee, string outtradeno, string title)
        {
            HttpWebRequest hwRequest = null;
            HttpWebResponse hwResponse = null;
            string strResult = string.Empty;
            //发起请求
            try
            {
                hwRequest = (System.Net.HttpWebRequest)WebRequest.Create(url);
                //hwRequest.Timeout = 30000;
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
                Dictionary<string, string> _params = new Dictionary<string, string>();
                _params.Add("total_fee", fee.ToString()); //订单金额
                _params.Add("out_trade_no", outtradeno);  //我们自己的订单编号
                _params.Add("body", title);  //我们自己的订单编号
                _params.Add("notify_url", notify_url);  //通知页
                _params.Add("callback_url", success_url);  //成功支付的跳转
                _params.Add("logo", logo);  //成功支付的跳转

                string str = BuildParam(_params); //添加参数
                if (!string.IsNullOrEmpty(str))
                {
                    byte[] data = Encoding.UTF8.GetBytes(str);
                    using (Stream stream = hwRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
            catch (WebException ex)
            {
                //这里应该计入日志
                Console.WriteLine("订单请求异常");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //响应
                try
                {
                    hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                    StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.ASCII);
                    strResult = srReader.ReadToEnd();
                    srReader.Close();
                    hwResponse.Close();
                }
                catch (WebException ex)
                {
                    Console.WriteLine("订单响应异常");
                    Console.WriteLine(ex.Message);
                }

            }
            return strResult;

        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private string BuildParam(Dictionary<string, string> param)
        {
            param = Sign(param);
            if (!(param == null || param.Count == 0))
            {
                StringBuilder sb = new StringBuilder();
                foreach (string key in param.Keys)
                {
                    sb.AppendFormat("{0}={1}&", key, param[key]);
                }
                return sb.ToString().Substring(0, sb.Length - 1);
            }
            return "";
        }

        /// <summary>
        /// 排序加密
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private Dictionary<string, string> Sign(Dictionary<string, string> param)
        {
            param.Add("mchid", MchId);
            //去掉空的，排序
            Dictionary<string, string> newParam = param.Where(w => w.Value.Trim() != "").
                OrderBy(o => o.Key).ToDictionary(d => d.Key, d => d.Value);
            string paramStr = "";
            //拼接
            foreach (KeyValuePair<string, string> keyPair in newParam)
            {
                paramStr += (keyPair.Key + "=" + keyPair.Value + "&");
            }
            //加上key
            paramStr += "key=" + Key;
            //md5后大写
            string sign = (Md5(paramStr)).ToUpper();
            newParam.Add("sign", sign);
            return newParam;
        }

        /// <summary>
        /// 进行MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string Md5(string str)
        {
            //from https://www.cnblogs.com/zq20/p/6268243.html
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }


    }
}
