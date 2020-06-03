using Joint.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Joint.DLLFactory;
using Joint.IService;
using System.Text;
using Joint.Entity;

namespace Joint.Web.Framework
{

    [Compress]
    public abstract class BaseController : Controller
    {
        public VisitorTerminal VisitorTerminalInfo { get; set; }

        protected void InitVisitorTerminal()
        {
            VisitorTerminal terminal = new VisitorTerminal();
            string str = base.Request.UserAgent.ToString().ToLower();

            bool isIpad = str.Contains("ipad");
            bool isIphoneOs = str.Contains("iphone os");
            bool isMidp = str.Contains("midp");
            bool isRv = str.Contains("rv:1.2.3.4");
            isRv = isRv ? isRv : str.Contains("ucweb");
            bool isAndroid = str.Contains("android");
            bool isWindowsCe = str.Contains("windows ce");
            bool isWindowsMobile = str.Contains("windows mobile");
            bool isMicromessenger = str.Contains("micromessenger");
            bool isWindowsPhone = str.Contains("windows phone ");
            terminal.Terminal = EnumVisitorTerminal.PC;

            if (((isIpad || isIphoneOs) || (isMidp || isRv)) || ((isAndroid || isWindowsCe) || (isWindowsMobile || isWindowsPhone)))
            {
                terminal.Terminal = EnumVisitorTerminal.Moblie;
            }
            if (isIpad || isIphoneOs)
            {
                terminal.OperaSystem = EnumVisitorOperaSystem.IOS;
                if (isIpad)
                {
                    terminal.Terminal = EnumVisitorTerminal.PAD;
                }
                else
                {
                    terminal.Terminal = EnumVisitorTerminal.Moblie;
                }
            }
            if (isAndroid)
            {
                terminal.OperaSystem = EnumVisitorOperaSystem.Android;
                terminal.Terminal = EnumVisitorTerminal.Moblie;
            }
            if (isMicromessenger)
            {
                terminal.Terminal = EnumVisitorTerminal.WeiXin;
            }

            VisitorTerminalInfo = terminal;
        }

        public bool IsWeiXinOrMoblie()
        {
            //如果是3，说明在电脑端演示手机管家,则显示手机版
            if (InitInfo.Instance.UserSelectVersion == 3)
            {
                return true;
            }

            //如果是手机
            if (this.VisitorTerminalInfo.Terminal == EnumVisitorTerminal.WeiXin || this.VisitorTerminalInfo.Terminal == EnumVisitorTerminal.Moblie)
            {
                ////如果在手机模式下，选择了pc版展示，则还是显示pc
                //if (InitInfo.Instance.UserSelectVersion == 1)
                //{
                //    return false;
                //}
                return true;
            }
            return false;
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.InitVisitorTerminal();
            //如果在手机模式下，选择了pc版展示，则还是显示pc
            ViewBag.IsWeiXinOrMoblie = InitInfo.Instance.UserSelectVersion == 1 ? false : IsWeiXinOrMoblie();
            base.OnActionExecuting(filterContext);
        }



        /// <summary>
        /// 获取用户最后操作时间
        /// </summary>
        /// <param name="date"></param>
        protected void SetLastOperateTime(DateTime? date = new DateTime?())
        {
            if (!date.HasValue)
            {
                date = new DateTime?(DateTime.Now);
            }
            HttpCookie cookie = base.HttpContext.Request.Cookies["Xnq_LastOpTime"];
            if (cookie == null)
            {
                cookie = new HttpCookie("Xnq_LastOpTime");
            }
            else
            {
                DateTime.FromBinary(long.Parse(cookie.Value));
            }
            cookie.Value = date.Value.Ticks.ToString();
            base.HttpContext.Response.AppendCookie(cookie);
        }
        //public bool HasPrivileges(string priCode)
        //{

        //}

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        /// <summary>
        /// 初始化配置信息
        /// </summary>
        protected abstract void InitSetting();


        /// <summary>
        /// 获取一个商家的第一家门店
        /// </summary>
        /// <param name="shopID">商家</param>
        /// <returns></returns>
        public Stores GetShopFirstStore(int shopID)
        {
            Stores firstStore = null;
            IStoresService service = ServiceFactory.Create<IStoresService>();
            if (shopID > 0)
            {
                firstStore = service.GetEntities(o => o.ShopId == shopID && o.VirtualShopsID == null).OrderBy(t => t.ID).FirstOrDefault();
            }
            else
            {
                firstStore = service.GetEntities(o => o.VirtualShopsID == shopID).OrderBy(t => t.ID).FirstOrDefault();
            }
            return firstStore;
        }

        public class Result
        {
            public Result()
            {

            }

            /// <summary>
            /// 返回值构造函数
            /// </summary>
            /// <param name="success">是否成功</param>
            /// <param name="resultType">操作类型</param>
            public Result(bool success, ResultType resultType)
            {
                if (success)
                {
                    this.success = true;
                    switch (resultType)
                    {
                        case ResultType.Add: msg = "添加成功"; break;
                        case ResultType.Delete: msg = "删除成功"; break;
                        case ResultType.Update: msg = "更新成功"; break;
                        default: msg = "操作成功"; break;
                    }
                }
                else
                {
                    this.success = false;
                    switch (resultType)
                    {
                        case ResultType.Add: msg = "添加失败"; break;
                        case ResultType.Delete: msg = "删除失败"; break;
                        case ResultType.Update: msg = "更新失败"; break;
                        default: msg = "操作失败"; break;
                    }
                }
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="success">是否成功</param>
            /// <param name="msg">自定义返回内容</param>
            public Result(bool success, string msg)
            {
                this.success = success;
                this.msg = msg;
            }
            public string msg { get; set; }
            public bool success { get; set; }
        }

        public enum ResultType
        {
            /// <summary>
            /// 添加成功,添加失败
            /// </summary>
            Add = 1,
            /// <summary>
            /// 删除成功,删除失败
            /// </summary>
            Delete = 2,
            /// <summary>
            /// 更新成功,更新失败
            /// </summary>
            Update = 3,
            /// <summary>
            /// 返回“操作成功”或“操作失败”
            /// </summary>
            Other = 4
        }

    }
}
