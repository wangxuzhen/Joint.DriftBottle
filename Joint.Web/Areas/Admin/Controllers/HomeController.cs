using Joint.Common;
using Joint.DLLFactory;
using Joint.Entity;
using Joint.IService;
using Joint.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Joint.Web.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }


        [UnAuthorize]//设置标签，标示不用权限验证的方法
        public ActionResult Login(string userToken)
        {
            return RedirectToAction("Login", "Home", new { area = "Mobile" });

            //来源标识不为空，则标记
            if (Request["FromSource"] != null && !string.IsNullOrWhiteSpace(Request["FromSource"].ToString()))
            {
                Session["FromSource"] = Request["FromSource"].ToString();
            }
            //来源标识不为空，则标记
            if (Request["SourceType"] != null && !string.IsNullOrWhiteSpace(Request["SourceType"].ToString()))
            {
                Session["SourceType"] = Request["SourceType"].ToString();
            }

            int loginShopID = 0;
            try
            {
                loginShopID = Request["shopID"] == null ? 0 : Convert.ToInt32(Request["shopID"]);
            }
            catch
            {
            }

            ViewBag.PCodeBefore = JeasuHelper.ThisMachineCode;

            if (Request.Url.Host == "lht.qdchepu.cn")
            {
                ViewBag.IsUserDefinedLogo = true;
                ViewBag.UserDefinedName = "莱哥西妹";
                ViewBag.UserDefinedLogo = "logo-login_LGXM.png";
            }
            else if (loginShopID != 0)
            {
                var singleShop = ServiceHelper.GetShopsService.GetEntity(loginShopID);
                ViewBag.IsUserDefinedLogo = true;
                ViewBag.UserDefinedName = singleShop.SiteName;
                ViewBag.UserDefinedLogo = singleShop.LogoUrl;
            }
            else if (Request.Url.Host == "vip.imqrj.com" || Request.Url.Host == "vip2.iqcrj.com")
            {
                ViewBag.ShopType = 2;
            }
            else
            {
                ViewBag.ShopType = 1;
            }

            return View();
            //Session["WeiXinAutoLoginOpenID"] = null;
            //if (Request["Code"] != null)
            //{
            //    int shopID = TypeHelper.ObjectToInt(Request["state"]);
            //    string openID = WeiXinUtil.GetApiBaseOpenID(Request["Code"].ToString(), shopID);

            //    //获取门店类型
            //    ViewBag.ShopType = ServiceHelper.GetShopsService.GetEntity(shopID).ShopType;

            //    //如果是已经绑定账户的直接登陆
            //    if (!string.IsNullOrWhiteSpace(openID))
            //    {
            //        IUsersService usersService = ServiceHelper.GetUsersService;
            //        var user = usersService.GetFirstOrDefault(t => t.WeiXinAutoLoginOpenID == openID && t.Disabled != true);
            //        if (user != null)
            //        {
            //            //如果已经绑定了微信，则自动登录系统
            //            base.InitUserInfo(user);
            //            CurrentInfo.Settings.IsWeiXinLogin = true;
            //            //如果是意向客户登录则提醒销售
            //            if (CurrentInfo.CurrentUser.IsIntention == true && CurrentInfo.CurrentUser.ID != 8090 && CurrentInfo.CurrentUser.ID != 1330)
            //            {
            //                //尝试通知
            //                try
            //                {
            //                    LoginNotify();
            //                }
            //                catch (Exception ex)
            //                {
            //                    Log.Error(ex);
            //                }
            //            }

            //            return RedirectToAction("Index", "Mobile", new { area = "Admin" });
            //        }
            //        else
            //        {
            //            Session["WeiXinAutoLoginOpenID"] = openID;
            //        }
            //    }
            //}

            //if (userToken == "afb4b11b25864820b8e42f4b583d46ff")
            //{
            //    LoginAjax("afb4b11b25864820b8e42f4b583d46ff", "afb4b11b25864820b8e42f4b583d46ff", false);
            //    return RedirectToAction("Index", "Home", new { area = "Admin" });
            //}
            //else if (userToken == "9dfbaa36e3c1e27296cdc312e6e0a028")
            //{
            //    LoginAjax("9dfbaa36e3c1e27296cdc312e6e0a028", "9dfbaa36e3c1e27296cdc312e6e0a028", false);
            //    return RedirectToAction("Index", "Home", new { area = "Admin" });
            //}
            //else
            //{
            //    return View();
            //}

        }

        public ActionResult LoginThis(int ID)
        {
            if (CurrentInfo.IsAdministrator)
            {
                base.ClearLoginInfo();
                IUsersService usersService = ServiceFactory.Create<IUsersService>();
                var loginUser = usersService.GetEntity(ID);
                //base.ClearLoginInfo(false);
                base.InitUserInfo(loginUser);

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Error403", "Home", new { area = "Admin" });
            }
        }

        /// <summary>
        /// ajax登陆
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="rememberMe">是否记住密码</param>
        /// <param name="loginType">0网页登陆，1微信登陆</param>
        /// <returns></returns>
        [UnAuthorize]//设置标签，标示不用权限验证的方法
        public JsonResult LoginAjax(string userName, string password, bool rememberMe = false)
        {
            base.ClearLoginInfo(false);

            //检测用户名密码是否为空
            if (string.IsNullOrWhiteSpace(userName))
            {
                return Json(new Result(false, "用户名不能为空"), JsonRequestBehavior.AllowGet);
            }
           
            if (string.IsNullOrWhiteSpace(password))
            {
                return Json(new Result(false, "密码不能为空"), JsonRequestBehavior.AllowGet);
            }

            //记住我按钮
            base.RememberMe(userName, password, rememberMe);

            IUsersService usersService = ServiceFactory.Create<IUsersService>();

            Users loginUser = null;



            //游客登录,默认登录丁笑笑的账号
            if (userName == "afb4b11b25864820b8e42f4b583d46ff" && password == "afb4b11b25864820b8e42f4b583d46ff")//B4测试号
            {
                var tempLoginUser = usersService.GetEntity(1330);
                if (tempLoginUser.RealName != "丁笑笑" || tempLoginUser.Disabled == true)
                {
                    tempLoginUser.RealName = "丁笑笑";
                    tempLoginUser.Disabled = false;
                    tempLoginUser.IsIntention = true;
                    tempLoginUser.Update();
                }

                //上面有对对象进行修改，重新获取下最新对象，防止错误
                loginUser = usersService.GetEntity(1330);//.GetEntity();
            }
            else if (userName == "9dfbaa36e3c1e27296cdc312e6e0a028" && password == "9dfbaa36e3c1e27296cdc312e6e0a028")//B4测试号
            {
                var tempLoginUser = usersService.GetEntity(8090);
                if (tempLoginUser.RealName != "刘小爱" || tempLoginUser.Disabled == true)
                {
                    tempLoginUser.RealName = "刘小爱";
                    tempLoginUser.Disabled = false;
                    tempLoginUser.IsIntention = true;
                    tempLoginUser.Update();
                }

                //上面有对对象进行修改，重新获取下最新对象，防止错误
                loginUser = usersService.GetEntity(8090);//.GetEntity();
            }
            else
            {
                loginUser = usersService.Login(userName, password);
            }

            //如果用户不为空，则说明登录成功，然后初始化用户信息，包括菜单，门店等信息
            if (loginUser != null)
            {

                base.InitUserInfo(loginUser);
                //用户如果被禁用，则用户将被禁止登陆
                if (loginUser.Disabled == true)
                {
                    base.ClearLoginInfo(false);
                    return Json(new Result(false, "此用户已被禁用"), JsonRequestBehavior.AllowGet);
                }
                else if (CurrentInfo.CurrentStore.Disabled == true) //门店如果被禁用，则用户将被禁止登陆
                {
                    base.ClearLoginInfo(false);
                    return Json(new Result(false, "门店已被禁用，如有疑问请联系管理员"), JsonRequestBehavior.AllowGet);
                }
                else if (CurrentInfo.CurrentShop.Disabled == true && CurrentInfo.CurrentShop.DueDate <= DateTime.Now)//判断是否到期
                {
                    int shopid = CurrentInfo.CurrentShop.ID;
                    base.ClearLoginInfo(false);
                    return Json(new { success = false, isDue = true, msg = "/Admin/Home/NotLoginRenewalFees?shopID=" + shopid }, JsonRequestBehavior.AllowGet);
                }
                //商户如果被禁用，则用户将被禁止登陆
                else if (CurrentInfo.CurrentShop.Disabled == true)
                {
                    base.ClearLoginInfo(false);
                    return Json(new Result(false, "商户已被禁用，如有疑问请联系管理员"), JsonRequestBehavior.AllowGet);
                }

                //如果管理员登录，则再次验证，密码是否被修改过
                if (UserIsAdministrator(loginUser) && loginUser.PasswordSalt != "y8ce0ydh88")
                {
                    base.ClearLoginInfo(false);
                    return null;
                }

                string redirectUrl = CurrentInfo.HomeUrl;

                //如果有WeiXinAutoLoginOpenID则绑定
                if (Session["WeiXinAutoLoginOpenID"] != null)
                {
                    //绑定用户e
                    loginUser.WeiXinAutoLoginOpenID = Session["WeiXinAutoLoginOpenID"].ToString();
                    usersService.UpdateEntity(loginUser);
                    CurrentInfo.Settings.IsWeiXinLogin = true;
                    CurrentInfo.CurrentUser.WeiXinAutoLoginOpenID = Session["WeiXinAutoLoginOpenID"].ToString();

                }

                if (this.VisitorTerminalInfo.Terminal == EnumVisitorTerminal.WeiXin || this.VisitorTerminalInfo.Terminal == EnumVisitorTerminal.Moblie)
                {
                    redirectUrl = "/Mobile/Home/DriftBottleList?rd=20170907";
                }

                //如果是意向客户登录则提醒销售
                //if (CurrentInfo.CurrentUser.IsIntention == true && CurrentInfo.CurrentUser.ID != 8090 && CurrentInfo.CurrentUser.ID != 1330)
                //{
                //    LoginNotify();
                //}

                return Json(new Result(true, redirectUrl), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new Result(false, "用户名和密码不匹配"), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Logout()
        {
            //如果用户是微信登录退出的时候要解绑
            //if (CurrentInfo.Settings.IsWeiXinLogin)
            //{
            //int shopID = ServiceHelper.GetCommonService.GetVirtualShopsID(CurrentInfo.CurrentStore.ID);
            //IUsersService usersService = ServiceHelper.GetUsersService;
            //var modelUser = usersService.GetEntity(CurrentInfo.CurrentUser.ID);
            //modelUser.WeiXinAutoLoginOpenID = null;
            //usersService.UpdateEntity(modelUser);
            //base.ClearLoginInfo();
            ////string appID = ServiceHelper.GetShopsService.GetAppID(shopID);

            //string redirectUrl = ServiceHelper.GetShopsService.GetDomain(shopID);
            //string strUrl = redirectUrl + "/admin/Home/Login";

            //string authorizeUrl = WeiXinUtil.GetAuthorizeUrl(shopID, strUrl, "1");
            ////去微信拿code
            //return Redirect(authorizeUrl);
            //}
            //else
            //{
            //    base.ClearLoginInfo();
            //    return RedirectToAction("Login", "Home");
            //}
            base.ClearLoginInfo();

            //if (Request["LogoutType"] != null && Request["LogoutType"].ToString() == "2")
            //{
            //    //return Redirect(Request["backurl"].ToString());
            //    return RedirectToAction("Login", "Home", new { area = JeasuHelper.SubProjectName });
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Home", new { area = "Admin" });
            //}

            return RedirectToAction("Login", "Home", new { area = "Mobile" });
        }


        public ActionResult Error500()
        {
            return View();
        }

        public ActionResult Error403()
        {
            TempData["Title"] = "权限不足";
            TempData["Message"] = "对不起，您没有权限访问此页面";
            return View();
        }

        public ActionResult Blank()
        {
            return View();
        }

        public ActionResult AllIcon()
        {
            return View();
        }

        public ActionResult ModifyPassword()
        {
            return View();
        }

        public ActionResult ModifyPasswordAjax(string oldPassword, string newPassword, string confirmPassword)
        {


            IUsersService usersService = ServiceFactory.Create<IUsersService>();

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return Json(new Result(false, "密码不能为空"), JsonRequestBehavior.AllowGet);
            }

            if (newPassword != confirmPassword)
            {
                return Json(new Result(false, "两次输入的新密码不一致"), JsonRequestBehavior.AllowGet);
            }

            Entity.Users dbUser = usersService.GetEntity(CurrentInfo.CurrentUser.ID);

            string oldPasswordMd5 = Common.SecureHelper.MD5(oldPassword + dbUser.PasswordSalt);
            if (oldPasswordMd5 != dbUser.Password)
            {
                return Json(new Result(false, "原始密码错误，修改失败"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                //修改账号PasswordSalt要判断是否是管理员
                dbUser.PasswordSalt = Common.TextFilter.GetPasswordSalt(UserIsAdministrator(dbUser));//.Substring(Guid.NewGuid().ToString("N"), 10, false);
                string endPassword = newPassword + dbUser.PasswordSalt;
                //计算密码
                dbUser.Password = Common.SecureHelper.MD5(endPassword);
            }

            bool success = usersService.UpdateEntity(dbUser);
            if (success)
            {
                return Json(new Result(true, "修改成功，请使用新密码登录系统"), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new Result(false, "修改失败"), JsonRequestBehavior.AllowGet);
            }
        }


    }
}