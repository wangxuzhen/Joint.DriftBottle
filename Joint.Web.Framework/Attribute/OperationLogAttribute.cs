using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Joint.Web.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OperationLogAttribute : ActionFilterAttribute
    {
        public string ParameterNameList;

        public OperationLogAttribute()
        {
        }

        public OperationLogAttribute(string message, string parameterNameList = "")
        {
            this.Message = message;
            this.ParameterNameList = parameterNameList;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //LogInfo model;
            //if (filterContext.Exception == null)
            //{
            //    string str = filterContext.RouteData.Values["controller"].ToString();
            //    string str2 = filterContext.RouteData.Values["action"].ToString();
            //    object obj1 = filterContext.RouteData.Values["area"];
            //    StringBuilder builder = new StringBuilder();
            //    builder.Append(this.Message + ",操作记录:");
            //    if (!string.IsNullOrEmpty(this.ParameterNameList))
            //    {
            //        Dictionary<string, string> dictionary = new Dictionary<string, string>();
            //        foreach (string str3 in this.ParameterNameList.Split(new char[] { ',', '|' }))
            //        {
            //            ValueProviderResult result = filterContext.Controller.ValueProvider.GetValue(str3);
            //            if ((result != null) && !dictionary.ContainsKey(str3))
            //            {
            //                dictionary.Add(str3, result.AttemptedValue);
            //            }
            //        }
            //        foreach (KeyValuePair<string, string> pair in dictionary)
            //        {
            //            builder.AppendFormat("{0}:{1} ", pair.Key, pair.Value);
            //        }
            //    }
            //    model = new LogInfo
            //    {
            //        Date = DateTime.Now,
            //        IPAddress = WebHelper.GetIP(),
            //        UserName = (filterContext.Controller as BaseAdminController).CurrentManager.UserName,
            //        PageUrl = str + "/" + str2,
            //        Description = builder.ToString()
            //    };
            //    Task.Factory.StartNew(() => { Instance<IOperationLogService>.Create.AddPlatformOperationLog(model); });
            //    base.OnActionExecuted(filterContext);
            //}
        }

        public string Message { get; set; }
    }
}
