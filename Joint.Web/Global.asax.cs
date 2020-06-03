using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Joint.Web.Framework;
using log4net.Config;
using System.IO;
using Joint.Common;
using System.Web.Http;
using System.Diagnostics;

namespace Joint.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AreaRegistrationOrder.RegisterAllAreasOrder();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            XmlConfigurator.Configure();
            //定时任务
            //JobScheduler.Start();
            //简单机器码
            JeasuHelper.ThisMachineCode = Common.SecureHelper.MD5(MachineCode.GetMachineCodeString());

        }
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string strUrl = Request.Url.ToString().Trim().ToLower();
            if (strUrl.Contains("http://bbbb4.com"))
            {
                Response.RedirectPermanent(strUrl.Replace("http://bbbb4.com", "http://www.bbbb4.com"));
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            if (!(ex is HttpException && (ex.Message.Contains("was not found on controller") || ex.Message.Contains("上未找到公共操作方法"))))
            {
                Log.Error("Global中捕获未知错误，请求者IP：" + Request.UserHostAddress, ex);
            }
            //this.Context.Response.Clear();            
            //Server.Transfer("/StaticHtml/UpdateHtml/index.html");
            //HttpContext.Current.Response.Redirect("/StaticHtml/UpdateHtml/index.html");
        }
    }
}
