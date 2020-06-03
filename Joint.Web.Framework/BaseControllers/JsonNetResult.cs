using Joint.IRepository;
using Joint.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Joint.Web.Framework
{
    public class JsonNetResult : JsonResult
    {
        public IDbSession dbSession = DbSessionFactory.GetCurrentDbSession();

        public JsonSerializerSettings Settings { get; private set; }

        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
            {
                //这句是解决问题的关键,也就是json.net官方给出的解决配置选项.                 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //MissingMemberHandling = MissingMemberHandling.Ignore,
            };
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;
            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if (this.Data == null)
                return;

            //string result = JsonConvert.SerializeObject(this.Data, this.Settings);
            //response.Write(result);

            var scriptSerializer = JsonSerializer.Create(this.Settings);
            using (var sw = new StringWriter())
            {
                //关闭贪婪加载
                dbSession.LazyLoadingEnabled(false);
                scriptSerializer.Serialize(sw, this.Data);
                string result = sw.ToString();
                dbSession.LazyLoadingEnabled(true);
                response.Write(result);
            }
        }
    }
}
