using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Common
{
    public class HttpClientHelper
    {
        public static string Get(string url)
        {
            return Get<string>(url);
        }


        public static T Get<T>(string url)
        {
            string strJson = GetResponseJson(url);
            return strJson.FromJson<T>();
        }

        public static string GetResponseJson(string url)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string responseJson = response.Content.ReadAsStringAsync().Result;
                return responseJson;
            }
            else
            {
                return "Error,StatusCode:" + response.StatusCode.ToString();
            }
        }

        /// <summary>
        /// Post返回json
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Post(string url, object obj)
        {
            return PostResponseJson(url, obj.ToJson());
        }

        /// <summary>
        ///  Post返回 对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Post<T>(string url, object obj)
        {
            string strJson = Post(url, obj);
            return strJson.FromJson<T>();
        }

        public static string PostResponseJson(string url, string requestJson)
        {
            HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");//application/json; charset=utf-8            
            httpContent.Headers.ContentType.CharSet = "utf-8";
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                string responseJson = response.Content.ReadAsStringAsync().Result;
                return responseJson;
            }
            else
            {
                return "Error,StatusCode:" + response.StatusCode.ToString();
            }

            ////定义request并设置request的路径
            //WebRequest request = WebRequest.Create(url);
            //request.Method = "post";

            ////初始化request参数
            //string postData = requestJson;// "{ ID: \"1\", NAME: \"Jim\", CREATETIME: \"1988-09-11\" }";

            ////设置参数的编码格式，解决中文乱码
            //byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            ////设置request的MIME类型及内容长度
            //request.ContentType = "application/json";
            //request.ContentLength = byteArray.Length;

            ////打开request字符流
            //Stream dataStream = request.GetRequestStream();
            //dataStream.Write(byteArray, 0, byteArray.Length);
            //dataStream.Close();

            ////定义response为前面的request响应
            //WebResponse response = request.GetResponse();

            ////获取相应的状态代码
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            ////定义response字符流
            //dataStream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(dataStream);
            //string responseFromServer = reader.ReadToEnd();//读取所有
            //return responseFromServer;
        }

    }

}
