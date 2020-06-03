using Joint.Common;
using Joint.DLLFactory;
using Joint.Entity;
using Joint.IService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Joint.Web.Framework
{
    [Serializable]
    public class InitInfo
    {
        /// <summary>
        /// 当前系统的所有菜单，全局唯一
        /// </summary>        
        public static List<Entity.Module> AllSysModule = ServiceHelper.GetModuleService.GetEntities(t => t.Disabled == false).ToList();// ServiceFactory.Create<IModuleService>()

        private InitInfo()
        {
            InitData();
        }

        public static InitInfo Instance
        {
            get
            {
                //如果还没有
                //if (HttpContext.Current.Session == null)
                //{
                //    InitInfo tempInitInfo = new InitInfo();
                //    tempInitInfo.IsLogin = false;
                //    return tempInitInfo;
                //}
                //else
                //{
                InitInfo initInfo = DataCache.GetCache(HttpContext.Current.Session.SessionID) as InitInfo;
                if (initInfo == null)
                {
                    initInfo = new InitInfo();
                    initInfo.IsLogin = false;
                }
                return initInfo;
                //}
            }
        }

        ///// <summary>
        ///// 是否是旧版谷歌
        ///// </summary>
        //public bool IsOldChrome { get; set; }

        /// <summary>
        /// 当前账号是否是超级管理员
        /// </summary>
        public bool IsAdministrator
        {
            //管理员和测试管理员都有超级权限
            get { return CurrentUser.ID == 5; }
        }

        ///// <summary>
        ///// 是否拥有零时权限
        ///// </summary>
        //public bool HasTempPermission
        //{
        //    get
        //    {
        //        //如果是开发环境的时候，可以获取到临时权限
        //        //if (string.Equals(ConfigurationManager.AppSettings["IsDebug"], "true", StringComparison.OrdinalIgnoreCase))
        //        //{
        //        //    return true;
        //        //}

        //        if (HttpContext.Current.Session["TempPermission"] != null)
        //        {
        //            if (HttpContext.Current.Session["TempPermission"].ToString().ToLower() == "true")
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //}

        /// <summary>
        /// 当前账号是否是商家的管理员
        /// </summary>
        public bool IsShopAdmin
        {
            get
            {
                return CurrentShop.AdminUserID == CurrentUser.ID;
            }
        }

        /// <summary>
        /// 判断是否是店铺管理员
        /// </summary>
        public bool IsStoreAdmin
        {
            get
            {
                return CurrentStore.AdminUserID == CurrentUser.ID;
            }
        }

        public bool IsDebug { get; set; }

        /// <summary>
        /// 首页
        /// </summary>
        public string HomeUrl { get; set; }

        /// <summary>
        /// 活动图片名称
        /// </summary>
        public string HDImgUrl { get; set; }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public Entity.Users CurrentUser { get; set; }

        /// <summary>
        /// 当前站点设置信息
        /// </summary>
        public AdminSettings Settings { get; set; }

        /// <summary>
        /// 网站的Seo标题
        /// </summary>
        public string SeoTitle { get; set; }

        /// <summary>
        /// 当前门店角色
        /// </summary>
        public string CurrentRoleName
        {
            get;
            set;
        }

        /// <summary>
        /// 当前用户切换到哪个门店
        /// </summary>
        public Entity.Stores CurrentStore { get; set; }

        /// <summary>
        /// 当前用户切换到哪个门店
        /// </summary>
        public Entity.Shops CurrentShop { get; set; }

        /// <summary>
        /// 当前用户拥有的所有店铺
        /// </summary>
        public List<Entity.Stores> AllStore { get; set; }

        /// <summary>
        /// 当前菜单
        /// </summary>
        public Entity.Module CurrentModule { get; set; }

        /// <summary>
        /// 判断用户本次请求是否有对应的菜单
        /// </summary>
        public bool ThisRequestHasModule { get; set; }


        /// <summary>
        /// 该用户拥有的所有菜单
        /// </summary>
        public List<Entity.Module> Sidebar { get; set; }

        /// <summary>
        /// 判断是否登录
        /// </summary>
        /// <returns></returns>
        public bool IsLogin { get; set; }

        /// <summary>
        /// 用户选择的显示版本
        /// </summary>
        public int UserSelectVersion { get; set; }


        public bool HasSystemPrivileges(string code)
        {
            return ServiceHelper.GetPrivilegesService.HasSystemPrivileges(1, code);
        }

        public bool HasShopsPrivileges(string code)
        {
            return ServiceHelper.GetPrivilegesService.HasShopsPrivileges(CurrentShop.ID, code);
        }

        public bool HasStoresPrivileges(string code)
        {
            return ServiceHelper.GetPrivilegesService.HasStoresPrivileges(CurrentStore.ID, code);
        }

        public bool HasUsersPrivileges(string code)
        {
            return ServiceHelper.GetPrivilegesService.HasUsersPrivileges(CurrentUser.ID, code);
        }

        //public int GetDefaultPrintType()
        //{
        //    return ServiceHelper.GetCommonService.GetPrintToolType(CurrentStore.ID);
        //}

        /// <summary>
        /// 获取当前商家类型
        /// </summary>
        /// <returns></returns>
        public ShopTypeEnum GetCurrentShopType()
        {
            return (ShopTypeEnum)CurrentShop.ShopType;
        }

        public bool IsShopTypeByBoolean(params ShopTypeEnum[] ShopTypeEnumArr)
        {
            var ShopTypeEnumList = ShopTypeEnumArr.ToList();
            ShopTypeEnum currentShopType = GetCurrentShopType();
            if (ShopTypeEnumList.Exists(t => t == currentShopType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public string IsShopTypeByString(params ShopTypeEnum[] ShopTypeEnumArr)
        {
            if (IsShopTypeByBoolean(ShopTypeEnumArr))
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        /// <summary>
        /// 面包屑导航
        /// </summary>
        public MvcHtmlString Breadcrumb
        {
            get { return GetBreadcrumb(); }
        }

        public void SetCurrentModule(string area, string controller, string action)
        {
            //如果初始信息有值则设定菜单的显示位置
            var currentModule = Sidebar.Find(t =>
                string.Equals(t.Controller, controller, StringComparison.OrdinalIgnoreCase)
                && string.Equals(t.Action, action, StringComparison.OrdinalIgnoreCase)
                && string.Equals(t.Area, area, StringComparison.OrdinalIgnoreCase));
            //如果用户导航的菜单是存在的，则保存，否则，将页面跳转到首页
            if (currentModule != null)
            {
                CurrentModule = currentModule;
                ThisRequestHasModule = true;
                //如果是意向客户
                if (CurrentUser.IsIntention == true)
                {
                    WriteModuleLog(currentModule);
                }
            }
            else
            {
                ThisRequestHasModule = false;
            }
        }

        public void WriteModuleLog(Module currentModule)
        {
            //如果是首页就不要记录了
            if (currentModule.Area.ToLower() == "admin"
                && currentModule.Controller.ToLower() == "home"
                && currentModule.Action.ToLower() == "index")
            {
                return;
            }
            IUserOperationLogService userOperationLogService = ServiceFactory.Create<IUserOperationLogService>();
            UserOperationLog model = new UserOperationLog();
            model.UserID = CurrentUser.ID;
            model.ModuleID = currentModule.ID;
            model.CreateTime = DateTime.Now;
            userOperationLogService.AddEntity(model);
        }

        public List<Module> GetCurrentModuleList(List<Module> listModel, Entity.Module currentModule)
        {
            if (listModel == null)
            {
                listModel = new List<Module>();
            }

            if (CurrentModule != null)
            {
                Entity.Module parentModule = ServiceHelper.GetModuleService.GetEntity(currentModule.ParentID);
                if (currentModule.ParentID != 0)
                {
                    GetCurrentModuleList(listModel, parentModule);
                }

                listModel.Add(currentModule);
            }
            //listModel.Reverse(0, listModel.Count);
            return listModel;
        }

        bool? BAdblockPlus = null;

        /// <summary>
        /// 是否过滤活动过滤广告
        /// </summary>
        public bool IsAdblockPlus()
        {
            return true;//活动结束，全部停止
            //if (BAdblockPlus == null)
            //{
            //    int shopID = CurrentShop.ID;
            //    int? salespersonID = CurrentShop.SalespersonID;
            //    if (Settings.ShopPhone != "administrator"  //如果是单机版，不显示广告
            //            || CurrentShop.CreateTime > DateTime.Now.AddMonths(-1)
            //            || 986 == salespersonID //婉真的客户不显示
            //        //|| shopID == 1      //B4不能发聚客卷
            //            || shopID == 4      //B4不能发聚客卷
            //            || shopID == 14     //B4可以发聚客卷
            //            || shopID == 66     //B4不能发聚客卷
            //            || shopID == 127    //B4可以发聚客卷
            //            || shopID == 144    //商家要求不显示
            //        )
            //    {
            //        BAdblockPlus = true;
            //        //return true;
            //    }
            //    else
            //    {
            //        BAdblockPlus = false;
            //    }
            //}

            //return (bool)BAdblockPlus;
        }

        /// <summary>
        /// 面包削导航
        /// </summary>
        /// <returns></returns>
        public MvcHtmlString GetBreadcrumb()
        {
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();

            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class=\"breadcrumb\">");
            sb.Append("<li>");
            sb.Append("<i class=\"icon-home home-icon\"></i>");
            sb.Append("<a href=\"" + HomeUrl + "\">首页</a>");
            sb.Append("</li>");

            //早期的算法，备注备用
            //if (CurrentModule != null)
            //{
            //    AddParentModule(CurrentModule, sb, moduleService);
            //    sb.AppendFormat("<li class=\"active\">{0}<i class=\"icon-refresh green\" style=\"margin-left: 10px;cursor:pointer;\" onclick=\"javascript:location.reload();\">刷新</i></li>", CurrentModule.Name);
            //}

            var currentModuleList = GetCurrentModuleList(new List<Module>(), CurrentModule);
            List<string> seoTitleList = new List<string>();

            SeoTitle = string.Empty;
            for (int i = 0; i < currentModuleList.Count; i++)
            {
                seoTitleList.Add(currentModuleList[i].Name + (i == 1 ? "系统" : "软件"));
                //SeoTitle += currentModuleList[i].Name + (i == 1 ? "系统," : "软件,");

                if (i < currentModuleList.Count - 1)
                {
                    sb.Append("<li>");
                    sb.AppendFormat("<a href=\"#\">{0}</a>", currentModuleList[i].Name);
                    sb.Append("</li>");
                }
                else
                {
                    string goBuy = "";// CurrentUser.ID != 1330 ? "" : "<li><span id=\"goBuySpan\" class=\"goBuySpan label label-lg label-danger arrowed-in arrowed-right\" style=\"cursor:pointer;\" onclick=\"javascript:location.href = '/Admin/Home/ProductPrice';\">点击购买软件</span></li>";
                    string happyNews = IsAdblockPlus() == true ? "" : "<li><span id=\"goBuySpanNews\" class=\"goBuySpan label label-lg label-danger arrowed-in arrowed-right\" style=\"cursor:pointer;\" onclick=\"ShowPromotion();\">喜迎中秋国庆</span></li>";
                    string newRegist = (CurrentUser.ID == 8090 || CurrentUser.ID == 1330) ? "<li><span id=\"goBuySpanNews\" class=\"goBuySpan label label-lg label-danger arrowed-in arrowed-right\" style=\"cursor:pointer;\" onclick=\"showShowRegister();\">免费注册账号</span></li>" : "";
                    sb.AppendFormat("<li class=\"active\">{0}<i class=\"icon-refresh green\" style=\"margin-left: 10px;cursor:pointer;\" onclick=\"javascript:location.reload();\"> 刷新</i></li><i class=\"icon-share purple\" style=\"margin-left: 10px;cursor:pointer;\" onclick=\"javascript:window.open('/admin/home/index', 'newwindow', 'height=600, width=800, top=0, left=0')\"> 新窗口</i></li>{1}", currentModuleList[i].Name, goBuy + happyNews + newRegist);//goBuy
                }
            }

            if (InitInfo.Instance.IsShopTypeByBoolean(ShopTypeEnum.美容美发))
            {
                if (CurrentUser.ID == 1330)
                {
                    if (CurrentModule != null && CurrentModule.Controller.ToLower() == "home" && CurrentModule.Action.ToLower() == "index")
                    {
                        SeoTitle = "B4图造";
                    }
                    else
                    {
                        seoTitleList.Reverse(0, seoTitleList.Count);
                        SeoTitle = string.Join("_", seoTitleList.ToArray());

                        SeoTitle += "_B4图造";
                    }
                }
                else
                {
                    SeoTitle = "B4管理系统";
                }
            }
            else
            {
                SeoTitle = "B4管理系统";
            }


            //SeoTitle = string.Join(",", currentModuleList.Select().ToArray());// currentModuleList

            sb.Append("</ul>");

            return MvcHtmlString.Create(sb.ToString());
        }

        //早期的算法，备注备用
        ///// <summary>
        ///// 递归加载面包屑导航
        ///// </summary>
        ///// <param name="currentModule"></param>
        ///// <param name="sb"></param>
        ///// <param name="moduleService"></param>
        //private void AddParentModule(Entity.Module currentModule, StringBuilder sb, IModuleService moduleService)
        //{
        //    if (currentModule.ParentID != 0)
        //    {
        //        Entity.Module parentModule = moduleService.GetEntity(currentModule.ParentID);
        //        AddParentModule(parentModule, sb, moduleService);
        //        sb.Append("<li>");
        //        sb.AppendFormat("<a href=\"#\">{0}</a>", parentModule.Name);
        //        sb.Append("</li>");
        //    }
        //}


        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            Settings = new AdminSettings();

            if (string.Equals(ConfigurationManager.AppSettings["IsDebug"], "true", StringComparison.OrdinalIgnoreCase) && !System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("/AutoLogin.lock")))
            {
                IsDebug = true;
            }
            else
            {
                IsDebug = false;
            }

            ////是否开发环境
            ////string isDebug = ConfigurationManager.AppSettings["IsDebug"];
            //try
            //{
            //    IsDebug = Convert.ToBoolean(isDebug);
            //}
            //catch
            //{
            //    IsDebug = false;
            //}
        }

    }
}
