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
using Joint.IRepository;
using Joint.Repository;
using System.Threading;

namespace Joint.Web.Framework
{

    public abstract class BaseAdminController : BaseController
    {
        /// <summary>
        /// AES加密key
        /// </summary>
        //public readonly string AESSecretKey = "JeasuSecretKey";

        //private int CKLoginTimeOut = 30;
        public static InitInfo CurrentInfo
        {
            get
            {
                return InitInfo.Instance;
            }

            private set
            {
                InitInfo initInfo = InitInfo.Instance;
                initInfo = value;
            }
        }

        public void ClearLoginInfo(bool ClearWeiXinAutoLogin = false)
        {
            //if (CurrentInfo.CurrentUser != null)
            //{
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.Today.AddYears(-2));
            ////清理页面路径缓存
            //var url = Url.Action("Sidebar", "Partial", new { ID = CurrentInfo.CurrentUser.ID });
            ////var urlSidebar = Url.Action("Sidebar", "Partial", new { Area = "Admin" });
            //HttpResponse.RemoveOutputCacheItem("/Admin/Home/Index");
            //HttpResponse.RemoveOutputCacheItem("/Home/Index");
            //HttpResponse.RemoveOutputCacheItem("/Admin/Partial/Sidebar");
            //HttpResponse.RemoveOutputCacheItem("/Partial/Sidebar");
            //HttpResponse.RemoveOutputCacheItem("/Sidebar");
            //HttpResponse.RemoveOutputCacheItem("/Admin");
            //Response.Cache.SetNoStore();
            ////HttpResponse.RemoveOutputCacheItem("*");
            //}

            DataCache.DeleteCache(Session.SessionID);
            System.Web.HttpContext.Current.Session["Jeasu_UserID"] = null;
            System.Web.HttpContext.Current.Session["Jeasu_StoreID"] = null;
            System.Web.HttpContext.Current.Session["LastOperateTime"] = null;

            if (ClearWeiXinAutoLogin == true)
            {
                System.Web.HttpContext.Current.Session["WeiXinAutoLoginOpenID"] = null;
            }

            //ServiceHelper.GetCommonService.DisposeDbContext();

            //EFContextFactory.GetCurrentDbContext().Dispose();
        }


        public void RememberMe(string userName, string password, bool rememberMe = false)
        {
            HttpCookie cookie = Request.Cookies["RememberMe"];
            if (cookie == null)
            {
                cookie = new HttpCookie("RememberMe");
            }
            cookie.Expires = DateTime.Now.AddMonths(1);
            cookie.Value = rememberMe.ToString();
            Response.AppendCookie(cookie);

            //判断用户是否勾选记住我的选项
            if (rememberMe)
            {
                cookie = Request.Cookies["UserName"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("UserName");
                }
                cookie.Expires = DateTime.Now.AddMonths(1);
                cookie.Value = userName;
                Response.AppendCookie(cookie);

                cookie = Request.Cookies["Password"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("Password");
                }
                cookie.Expires = DateTime.Now.AddMonths(1);
                cookie.Value = password;
                Response.AppendCookie(cookie);
            }
            else//如果没有勾选，则取消记住密码
            {
                cookie = Request.Cookies["UserName"];
                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddYears(-1);
                    Response.AppendCookie(cookie);
                }

                cookie = Request.Cookies["Password"];
                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddYears(-1);
                    Response.AppendCookie(cookie);
                }
            }
        }

        /// <summary>
        /// 判断用户是否是管理员
        /// </summary>
        /// <param name="singleUser"></param>
        /// <returns></returns>
        public bool UserIsAdministrator(Users singleUser)
        {
            if (singleUser.ID == 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ClearLoginCookie()
        {
            //清理COOKIE
            //HttpCookie cookie = base.HttpContext.Request.Cookies["Lax-User"];
            //if (cookie != null)
            //{
            //    cookie.Expires = DateTime.Now.AddYears(-1);
            //    base.HttpContext.Response.AppendCookie(cookie);
            //}
            //cookie = base.HttpContext.Request.Cookies["Lax-SellerManager"];
            //if (cookie != null)
            //{
            //    cookie.Expires = DateTime.Now.AddYears(-1);
            //    base.HttpContext.Response.AppendCookie(cookie);
            //}
            //cookie = base.HttpContext.Request.Cookies["Xnq_LastOpTime"];
            //if (cookie != null)
            //{
            //    cookie.Expires = DateTime.Now.AddYears(-1);
            //    base.HttpContext.Response.AppendCookie(cookie);
            //}         
        }

        //private void SetLoginCookie(int UserID, string SessionID)
        //{
        //    HttpCookie cookie = base.HttpContext.Request.Cookies["Jeasu_UserID"];
        //    if (cookie == null)
        //    {
        //        cookie = new HttpCookie("Jeasu_UserID");
        //    }
        //    cookie.Value = UserID.ToString();
        //    base.HttpContext.Response.AppendCookie(cookie);

        //    cookie = base.HttpContext.Request.Cookies["Xnq_SessionID"];
        //    if (cookie == null)
        //    {
        //        cookie = new HttpCookie("Xnq_SessionID");
        //    }
        //    cookie.Value = SessionID;
        //    base.HttpContext.Response.AppendCookie(cookie);
        //}



        private void DisposeService(ControllerContext filterContext)
        {
            //if (!filterContext.IsChildAction)
            //{
            //    List<IService> list = filterContext.HttpContext.Session["_serviceInstace"] as List<IService>;
            //    if (list != null)
            //    {
            //        foreach (IService service in list)
            //        {
            //            try
            //            {
            //                service.Dispose();
            //            }
            //            catch (Exception exception)
            //            {
            //                Log.Error(service.GetType().ToString() + "释放失败！", exception);
            //            }
            //        }
            //        filterContext.HttpContext.Session["_serviceInstace"] = null;
            //    }
            //}
        }


        /// <summary>
        /// 判断网站是否关闭
        /// </summary>
        /// <returns></returns>
        private bool IsInstalled()
        {
            //string str = ConfigurationManager.AppSettings["IsInstalled"];
            //if (str != null)
            //{
            //    return bool.Parse(str);
            //}
            return true;
        }

        private DateTime BeginExecutingTime;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            BeginExecutingTime = DateTime.Now;
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //如果用户不是通过ajax访问网站的时候，则判断下他是否有访问权限
            //if (!WebHelper.IsAjax())
            string area = filterContext.RouteData.DataTokens["area"].ToString().ToLower();
            string controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
            string action = filterContext.RouteData.Values["action"].ToString().ToLower();

            //判断用户请求的菜单是否在系统菜单内（目前权限只做到菜单级别，所以只能粗略判断）
            bool isExistInSys = InitInfo.AllSysModule.Exists(t =>
                string.Equals(t.Area, area, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(t.Controller, controller, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(t.Action, action, StringComparison.OrdinalIgnoreCase));

            //如果当前菜单存在系统菜单内，且当前用户并没有此菜单，则不允许用户访问
            if (isExistInSys == true && CurrentInfo.ThisRequestHasModule == false)
            {
                filterContext.Result = new RedirectResult("/Admin/Home/Error403");
            }

            base.OnActionExecuted(filterContext);


            TimeSpan tsExecut = DateTime.Now - BeginExecutingTime;
            if (tsExecut.TotalSeconds > 3)
            {
                int userID = 0;
                int storeID = 0;
                string storeName = string.Empty;
                int shopID = 0;
                string userIP = string.Empty;
                try
                {
                    userID = CurrentInfo.CurrentUser.ID;
                    storeID = CurrentInfo.CurrentStore.ID;
                    storeName = CurrentInfo.CurrentStore.StoreName;
                    userIP = Request.UserHostAddress;
                    shopID = CurrentInfo.CurrentShop.ID;
                }
                catch
                {

                }

                //请求地址
                string url = Request.Url.PathAndQuery.ToString();
                //页面post过来的参数
                string requestFormData = string.Empty;
                try
                {
                    requestFormData = filterContext.RequestContext.HttpContext.Request.Form.ToString();
                }
                catch (Exception ex)
                {
                    requestFormData = "FormData参数获取失败,原因：" + ex.Message; ;
                }

                Log.Debug(string.Format("执行过长时间，请注意，URL:{0}，FormData：{1}，执行时长：{2}秒" + Environment.NewLine + "用户ID：{3},商家ID：{4},门店ID：{5},门店名称：{6},用户IP：{7} ",
                    url, requestFormData, tsExecut.TotalSeconds, userID, shopID, storeID, storeName, userIP));
            }


            //if (InitInfo.Instance.IsShopTypeByBoolean(ShopTypeEnum.美容美发))
            //{
            //    var list = CurrentInfo.GetCurrentModuleList(new List<Module>(), CurrentInfo.CurrentModule);
            //    ViewBag.Title = "B4美容美发管理软件、美发软件,美容院管理软件,美发会员管理系统,美发收银系统,美甲店管理系统";
            //}

        }

        private void SaveLoginSession()
        {
            System.Web.HttpContext.Current.Session["Jeasu_UserID"] = CurrentInfo.CurrentUser.ID;
            System.Web.HttpContext.Current.Session["Jeasu_StoreID"] = CurrentInfo.CurrentStore.ID;
            System.Web.HttpContext.Current.Session["LastOperateTime"] = DateTime.Now;
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            //如果是单机版，需要检查数据是否合法
            //if (CurrentInfo.Settings.IsSingleVersion)
            //{
            //    //如果是单机版登录，判断门店数量是否正确，而且不能使用管理账号登录，管理员账号有另一个路口
            //    var falge = SingleVersion.Instance.CheckVersion(CurrentInfo.Settings.ShopPhone);

            //    //如果验证未通过，则直接返回首页
            //    if (falge == false)
            //    {
            //        ClearLoginInfo();
            //        filterContext.Result = new RedirectResult("/Admin/Home/Login");
            //    }
            //}

            //测试环境自动登录
            //if (!CurrentInfo.IsLogin && QiCeHelper.IsTest(Request.Url.Host.ToString()))
            //{
            //    DebugLogin();
            //}

            //如果需要身份验证
            if (NeedAuthorization(filterContext))
            {

                //string requestUrl = Server.UrlEncode(Session["SiteName"].ToString().PathAndQuery);

                string area = filterContext.RouteData.DataTokens["area"].ToString().ToLower();
                string controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
                string action = filterContext.RouteData.Values["action"].ToString().ToLower();

                if (CurrentInfo.IsLogin)
                {
                    #region 人为BUG代码
                    //客户要求来点BUG
                    //if (CurrentInfo.CurrentUser.UserName == "qcgjsj")
                    //{
                    //    Random rd = new Random(DateTime.Now.Millisecond);
                    //    int tempInt = rd.Next(1, 11);
                    //    Thread.Sleep(tempInt * 500);
                    //    if (tempInt <= 2)
                    //    {
                    //        throw new Exception("发生未知错误X29I3P5K6");
                    //    }
                    //} 
                    #endregion

                    CurrentInfo.SetCurrentModule(area, controller, action);
                    //ViewBag.InitInfo = CurrentInfo;
                    //保存登录session
                    SaveLoginSession();

                }
                else if (System.Web.HttpContext.Current.Session["LastOperateTime"] != null &&
                    (DateTime.Now - Convert.ToDateTime(System.Web.HttpContext.Current.Session["LastOperateTime"])).Minutes < 480)//如果是网站重启了，且用户没有超过8小时没有操作
                {
                    int userID = Convert.ToInt32(System.Web.HttpContext.Current.Session["Jeasu_UserID"]);
                    int storeID = Convert.ToInt32(System.Web.HttpContext.Current.Session["Jeasu_StoreID"]);
                    IUsersService userService = ServiceFactory.Create<IUsersService>();
                    var su = userService.GetEntity(userID);

                    //当前门店
                    IStoresService storesService = ServiceFactory.Create<IStoresService>();
                    var cs = storesService.GetEntity(storeID);
                    //如果为空说明已经失效，重新登录
                    if (su == null && cs == null)
                    {
                        filterContext.Result = new RedirectResult(string.Format("/{0}/Home/Index", JeasuHelper.SubProjectName));
                    }
                    else
                    {
                        InitUserInfo(su, cs);
                        CurrentInfo.SetCurrentModule(area, controller, action);
                        //保存登录session
                        SaveLoginSession();
                    }
                }
                else
                {
                    if (WebHelper.IsAjax())
                    {
                        Result data = new Result
                        {
                            success = false,
                            msg = "登录超时请重新登录"
                        };
                        filterContext.Result = Json(data, JsonRequestBehavior.AllowGet);
                        filterContext.HttpContext.Response.StatusCode = 200;
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult(string.Format("/{0}/Home/Login", JeasuHelper.SubProjectName));
                    }

                }
            }

            base.OnAuthorization(filterContext);
        }

        /// <summary>
        /// 是否需要身份验证，如果有UnAuthorize的标签代表不需要身份验证，如登录页
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private bool NeedAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(UnAuthorize), false).Length != 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void InitUserInfo(Users currentUser, Stores switchCurrentSore = null)
        {
            InitInfo initInfo = CurrentInfo;
            //当前用户
            //IUsersService usersService = ServiceFactory.Create<IUsersService>();
            //var currentUser = usersService.GetEntity(1);
            initInfo.CurrentUser = currentUser;

            //当前用户的所属门店
            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            var currentSore = switchCurrentSore == null ? currentUser.Stores : switchCurrentSore;//soresService.GetEntity(1);
            initInfo.CurrentStore = currentSore;

            //当前商家
            initInfo.CurrentShop = currentUser.Stores.Shops;

            //该用户在当前门店下的所有角色
            var userAllRole = currentUser.RelationUserRole;
            //用户所有的门店
            List<Entity.Stores> allStore = null;

            List<Entity.Module> sidebar = new List<Entity.Module>();

            //如果是系统管理员，则默认加载所有的菜单//否则就加载用户拥有的权限菜单
            if (initInfo.IsAdministrator)//userAllRole.Any(t => t.RoleID == 1)
            {
                //当前用户使用的角色名称
                initInfo.CurrentRoleName = "超级管理员";
                //左侧菜单(全部获取)
                IModuleService moduleService = ServiceFactory.Create<IModuleService>();
                sidebar = moduleService.GetEntities(t => t.Disabled == false).ToList();

                //管理员可以查看所有门店
                allStore = storesService.GetEntities(t => t.ShopId == initInfo.CurrentShop.ID && t.Disabled == false).ToList();
                //initInfo.Sidebar = sidebar;

            }
            else if (initInfo.CurrentShop.AdminUserID == currentUser.ID)
            {
                //如果是商家，则获取商家的所有菜单
                ////当前用户使用的角色名称
                //initInfo.CurrentRoleName = "店铺管理员";
                //IRelationShopsModuleService relationShopsModuleService = ServiceFactory.Create<IRelationShopsModuleService>();
                //var shopAllModule = relationShopsModuleService.GetEntities(t => t.ShopID == initInfo.CurrentShop.ID).Select(t => t.Module).ToList();
                //foreach (var itemShopModule in shopAllModule)
                //{
                //    //菜单没有被禁用，并且之前没有添加到菜单列表里面
                //    if (!itemShopModule.Disabled && !sidebar.Any(t => t.ID == itemShopModule.ID))
                //    {
                //        sidebar.Add(itemShopModule);
                //    }
                //}

                //如果是商家，则获取商家的对应门店的所有菜单，因为商家下面的门店有可能购买的版本不一样
                //当前用户使用的角色名称
                initInfo.CurrentRoleName = "系统管理员";
                IRelationStoresModuleService relationStoresModuleService = ServiceFactory.Create<IRelationStoresModuleService>();
                var storeAllModule = relationStoresModuleService.GetEntities(t => t.StoresID == initInfo.CurrentStore.ID).Select(t => t.Module).ToList();
                foreach (var itemStoreModule in storeAllModule)
                {
                    if (!itemStoreModule.Disabled && !sidebar.Any(t => t.ID == itemStoreModule.ID))
                    {
                        sidebar.Add(itemStoreModule);
                    }
                }

                //商家管理员可以查看所有门店
                allStore = storesService.GetEntities(t => t.ShopId == initInfo.CurrentShop.ID && t.Disabled == false).ToList();
            }
            else
            {
                //根据角色获取到所有门店
                allStore = userAllRole.Select(t => t.Role.Stores).Where(t => t.Disabled == false).Distinct().ToList();

                IRelationUserRoleService relationUserRoleService = ServiceFactory.Create<IRelationUserRoleService>();

                //过滤出用户当前门店的所有角色
                var userCurrentStoreRole = userAllRole.Where(t => t.Role.StoreID == currentSore.ID);

                //当前用户使用的角色名称
                initInfo.CurrentRoleName = string.Join(",", userCurrentStoreRole.Select(t => t.Role.Name).ToList());

                //用户左侧菜单
                foreach (var itemUserRole in userCurrentStoreRole)
                {
                    //单个角色拥有的菜单
                    var singleRoleModule = itemUserRole.Role.RelationRoleModule;
                    foreach (var itemRoleModule in singleRoleModule)
                    {
                        Module singleModule = itemRoleModule.Module;
                        //菜单没有被禁用，并且之前没有添加到菜单列表里面
                        if (!singleModule.Disabled && !sidebar.Any(t => t.ID == singleModule.ID))
                        {
                            sidebar.Add(singleModule);
                        }
                    }
                }
            }

            //首先获取该用户下面的所有菜单
            IRelationUsersModuleService relationUsersModuleService = ServiceFactory.Create<IRelationUsersModuleService>();
            var userAllModule = relationUsersModuleService.GetEntities(t => t.UsersID == currentUser.ID).Select(t => t.Module).ToList();
            foreach (var itemUserModel in userAllModule)
            {
                //菜单没有被禁用，并且之前没有添加到菜单列表里面
                if (!itemUserModel.Disabled && !sidebar.Any(t => t.ID == itemUserModel.ID))
                {
                    sidebar.Add(itemUserModel);
                }
            }

            //System.Web.HttpContext.Current.Session["InitInfo"] = initInfo;
            //设置缓存2小时
            DataCache.SetCache(HttpContext.Session.SessionID, initInfo, TimeSpan.FromMinutes(1)); // 
            //更新登录次数
            IUsersService userService = ServiceFactory.Create<IUsersService>();
            currentUser.LoginTimes = currentUser.LoginTimes == null ? 1 : currentUser.LoginTimes + 1;
            currentUser.LastLoginTime = DateTime.Now;
            currentUser.LastLoginIP = Request.UserHostAddress;

            //如果是意向客户，则查询一下IP在哪个城市
            if (currentUser.IsIntention == true)
            {
                currentUser.LastLoginArea = Common.IPHelper.GetAreaByIP(Request.UserHostAddress);
            }

            userService.UpdateEntity(currentUser);

            initInfo.AllStore = allStore;
            initInfo.Sidebar = sidebar;
            initInfo.IsLogin = true;

            ////确定当前用户显示的首页,如果有首页则显示，没有首页的话，默认显示微信聚客系统的介绍页
            //if (CurrentInfo.Sidebar.Exists(t =>
            //    t.Area != null && t.Area.ToLower() == "admin" &&
            //    t.Controller != null && t.Controller.ToLower() == "home" &&
            //    t.Action != null && t.Action.ToLower() == "index"))
            //{
            //    initInfo.HomeUrl = "/" + JeasuHelper.SubProjectName;
            //}
            //else
            //{
            //    initInfo.HomeUrl = "/Admin/CashCoupon/UseInstructions";
            //}

            initInfo.HomeUrl = "/Mobile/Home/DriftBottleList";
            #region 广告图片提示收年费
            if (string.IsNullOrWhiteSpace(CurrentInfo.HDImgUrl))
            {
            }

            #endregion

            InitSetting();
        }


        /// <summary>
        /// 获取用户升级的广告图片，暂时无用
        /// </summary>
        private void GetUpgradeAdImg()
        {
            IRelationShopsModuleService relationShopsModuleService = ServiceHelper.GetRelationShopsModuleService;
            //是否有旗舰版
            //bool hasQJB = relationShopsModuleService.Exists(t => t.ShopID == CurrentInfo.CurrentShop.ID && t.Module.Controller == "BusinessSolution" && t.Module.Action == "FestivalRemind" && t.Module.Area == "Admin");

            //是否有聚客
            bool hasJKQ = relationShopsModuleService.Exists(t => t.ShopID == CurrentInfo.CurrentShop.ID && t.Module.Controller == "CashCoupon" && t.Module.Action == "CustomerCashCoupon" && t.Module.Area == "Admin");
            //是否有员工分红
            bool hasYGFH = relationShopsModuleService.Exists(t => t.ShopID == CurrentInfo.CurrentShop.ID && t.Module.Controller == "Shareholder" && t.Module.Action == "ShareholderList" && t.Module.Area == "Admin");
            //是否有众筹系统
            bool hasZCXT = relationShopsModuleService.Exists(t => t.ShopID == CurrentInfo.CurrentShop.ID && t.Module.Controller == "EmployeeWelfare" && t.Module.Action == "EmployeeWelfareList" && t.Module.Area == "Admin");

            CurrentInfo.HDImgUrl = "/Areas/Admin/Content/Img/shiyihuodong1.jpg";
            //高级功能都没有
            if (!hasJKQ && !hasYGFH && !hasZCXT)
            {
                CurrentInfo.HDImgUrl = "/Areas/Admin/Content/Img/shiyihuodong3.jpg";
            }
            else if (hasJKQ && !hasYGFH && !hasZCXT)//有聚客，没有众筹和员工
            {
                CurrentInfo.HDImgUrl = "/Areas/Admin/Content/Img/shiyihuodong2.jpg";
            }
            else
            {
                CurrentInfo.HDImgUrl = "/Areas/Admin/Content/Img/shiyihuodong1.jpg";
            }
            //ViewBag.UserIsIntention = CurrentInfo.CurrentUser.IsIntention == true ? "true" : "false";
        }

        /// <summary>
        /// 获取当前门店的员工下拉框
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItemModel> GetCurrentStoreUser()
        {
            ICommonService commonService = ServiceFactory.Create<ICommonService>();
            return commonService.GetStoreUser(CurrentInfo.CurrentStore.ID);
        }

        /// <summary>
        /// 拒绝测试账号提交的请求
        /// </summary>
        public void RefuseIntentionUser()
        {
            if (CurrentInfo.CurrentUser.IsIntention == true || CurrentInfo.CurrentUser.ID == 1330)
            {
                throw new JeasuException("试用版不允许操作此功能", false);
            }
        }



        /// <summary>
        /// //初始化配置
        /// </summary>
        protected override void InitSetting()
        {
            AdminSettings settings = CurrentInfo.Settings;

            #region 单机版配置信息
            //如果单机版字符串存在session，则读取session的，否则读取配置
            if (System.Web.HttpContext.Current.Session["SingleV"] != null && System.Web.HttpContext.Current.Session["SingleV"].ToString() == "f2e3e264c30935e101235b20b106d9e7")
            {
                settings.IsSingleVersion = false;
            }
            else if (ConfigurationManager.AppSettings["SingleV"] != null && ConfigurationManager.AppSettings["SingleV"].ToString() == "f2e3e264c30935e101235b20b106d9e7")
            {
                settings.IsSingleVersion = false;
            }
            else
            {
                settings.IsSingleVersion = true;
            }

            //读取商家电话
            settings.ShopPhone = ConfigurationManager.AppSettings["ShopPhone"];

            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            //获取
            //var allStoreCount = storesService.GetEntities(t => t.Disabled == false).Count();
            ////当前的
            //settings.SingleVersion.StoreCount = allStoreCount;
            #endregion


            //settings.SiteUrl = "";
            //是否自动清除网站缓存
            settings.AutoClearCache = true;

            //网站地址
            settings.SiteUrl = ConfigurationManager.AppSettings["WebSiteUrl"];
        }

        public void ReloadSetting()
        {
            AdminSettings settings = CurrentInfo.Settings;
            settings = new AdminSettings();
            InitSetting();
        }

        /// <summary>
        /// 单机版用户不允许使用管理员账号
        /// </summary>
        /// <returns></returns>
        public bool CheckSingleVersion()
        {
            //如果是管理员账号，则拒绝
            if (CurrentInfo.IsAdministrator)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 在测试环境下自动登录
        /// </summary>
        private void DebugLogin()
        {
            ClearLoginInfo();
            //当前用户
            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            var user = usersService.GetEntity(1330);
            if (user != null)
            {
                InitUserInfo(user);
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            string errorNum = string.Empty;
            string message = filterContext.Exception.Message;
            base.OnException(filterContext);

            //编码
            errorNum = DateTime.Now.ToString("ddHHmmss");

            //如果是自定义错误，且不需要写日志，则直接返回客户端，无需记录
            if (filterContext.Exception is JeasuException && ((JeasuException)filterContext.Exception).needLog == false)
            {
                message = filterContext.Exception.Message;
            }
            else
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                object area = filterContext.RouteData.DataTokens["area"];

                //如果是自定义错误，则显示自定义错误信息
                if (filterContext.Exception is JeasuException && ((JeasuException)filterContext.Exception).needLog == true)
                {
                    message = filterContext.Exception.Message;
                }
                else
                {
                    if (filterContext.Exception.Message.Contains("另一个进程被死锁在"))
                    {
                        message = "服务器忙请重试，编码：" + errorNum;
                    }
                    else
                    {
                        message = "接口未对接，编码：" + errorNum;
                    }
                }

                //页面post过来的参数
                var requestFormData = filterContext.RequestContext.HttpContext.Request.Form.ToString();

                try
                {
                    Log.Error(string.Format("页面未捕获的异常：Area:{0},Controller:{1},Action:{2} 编码：{3} " + Environment.NewLine +
                                                          "用户ID：{4}，公司ID：{5}，部门ID：{6}，部门名称：{7}，用户IP：{8} " + Environment.NewLine +
                                                          "请求参数：Url:{9}，FormData:{10}",
                                                          area, controller, action, errorNum,
                                                          CurrentInfo.CurrentUser.ID, CurrentInfo.CurrentShop.ID, CurrentInfo.CurrentStore.ID, CurrentInfo.CurrentStore.StoreName, Request.UserHostAddress,
                                                          Request.Url.PathAndQuery.ToString(), requestFormData), filterContext.Exception);
                }
                catch (Exception ex)
                {
                    Log.Error("页面异常：" + filterContext.Exception);
                }

            }
            if (WebHelper.IsAjax())
            {
                Result data = new Result
                {
                    success = false,
                    msg = message
                };
                filterContext.Result = base.Json(data);
                filterContext.HttpContext.Response.StatusCode = 200;
                filterContext.ExceptionHandled = true;
                //this.DisposeService(filterContext);
            }
            else
            {
                ViewResult result2 = new ViewResult
                {
                    //MasterName = "~/Areas/Admin/Views/Shared/_Layout.cshtml",
                    ViewName = "~/Areas/Admin/Views/Home/Error500.cshtml"
                };
                message = "编码：" + errorNum;
                result2.TempData.Add("Message", message);
                filterContext.Result = result2;
                filterContext.HttpContext.Response.StatusCode = 200;
                filterContext.ExceptionHandled = true;
                //this.DisposeService(filterContext);
            }
            if (filterContext.Exception is HttpRequestValidationException)
            {
                if (WebHelper.IsAjax())
                {
                    Result result4 = new Result
                    {
                        success = false,
                        msg = "您提交了非法字符!"
                    };
                    filterContext.Result = base.Json(result4);
                }
                else
                {
                    ContentResult result5 = new ContentResult();
                    result5.Content = "<script src='/scripts/jquery-1.11.1.min.js'></script>";
                    result5.Content = result5.Content + "<script src='/scripts/jquery.artDialog.js'></script>";
                    result5.Content = result5.Content + "<script src='/scripts/artDialog.iframeTools.js'></script>";
                    result5.Content = result5.Content + "<link href='/content/artdialog.css' rel='stylesheet' />";
                    result5.Content = result5.Content + "<link href='/content/bootstrap.min.css' rel='stylesheet' />";
                    result5.Content = result5.Content + "<script>$(function(){$.dialog.errorTips('您提交了非法字符！',function(){window.history.back(-1)},2);});</script>";
                    filterContext.Result = result5;
                }
                filterContext.HttpContext.Response.StatusCode = 200;
                filterContext.ExceptionHandled = true;
                //this.DisposeService(filterContext);
            }

            //base.OnException(filterContext);
        }

    }
}
