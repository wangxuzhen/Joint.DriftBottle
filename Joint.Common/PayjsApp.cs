/// <summary>
/// 用于封装Pay个人支付模块(未测试)
/// Shayuxiang 2020.06.07
/// 发起支付:
/// var ret = PayjsApp.Instance.SendOrder(100,"123456","测试订单");
/// 
/// 回调页面
///             PayjsApp.Instance.Notify((order) => {
///                //交易成功
///            }, (order) => {
///                //交易失败
///            });
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
    using System.Web;

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
        /// 处理回调通知
        /// </summary>
        /// <returns></returns>
        public void Notify(Action<NotifyObject> success, Action<NotifyObject> fail)
        {
            //获取当前url
            string[] url = HttpContext.Current.Request.Url.Query.Replace("?", string.Empty).Split('&');
            NotifyObject notifyObject = new NotifyObject(url);
            //交易成功
            if (notifyObject.IsTradeSuccess())
            {
                success.Invoke(notifyObject);
            }
            else
            {
                fail.Invoke(notifyObject);
            }
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

    public class NotifyObject
    {

        /// <summary>
        /// payjs的订单号
        /// </summary>
        public string payjs_order_id { get; private set; }

        /// <summary>
        /// 返回代码
        /// </summary>
        public int return_code { get; private set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string mchid { get; private set; }
        /// <summary>
        /// 支付金额:分
        /// </summary>
        public int total_fee { get; private set; }
        /// <summary>
        /// 我们的订单号
        /// </summary>
        public string out_trade_no { get; private set; }
        /// <summary>
        /// 微信用户手机显示订单号
        /// </summary>
        public string transaction_id { get; private set; }
        /// <summary>
        /// 支付成功时间
        /// </summary>
        public string time_end { get; private set; }

        public NotifyObject(string[] url)
        {
            foreach (var u in url)
            {
                if (u.StartsWith("return_code"))
                {
                    return_code = int.Parse(u.Split('=')[1]);
                }
                if (u.StartsWith("total_fee"))
                {
                    total_fee = int.Parse(u.Split('=')[1]);
                }
                if (u.StartsWith("out_trade_no"))
                {
                    out_trade_no = u.Split('=')[1];
                }
                if (u.StartsWith("mchid"))
                {
                    mchid = u.Split('=')[1];
                }
                if (u.StartsWith("payjs_order_id"))
                {
                    payjs_order_id = u.Split('=')[1];
                }
                if (u.StartsWith("transaction_id"))
                {
                    transaction_id = u.Split('=')[1];
                }
                if (u.StartsWith("time_end"))
                {
                    time_end = u.Split('=')[1];
                }
            }
        }

        /// <summary>
        /// 是否交易成功
        /// </summary>
        /// <returns></returns>
        public bool IsTradeSuccess()
        {
            return return_code == 1 ? true : false;
        }


    }
}
