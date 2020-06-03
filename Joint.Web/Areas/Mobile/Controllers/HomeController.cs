using Joint.DLLFactory;
using Joint.Entity;
using Joint.IService;
using Joint.Web.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Joint.Common;

namespace Joint.Web.Areas.Mobile.Controllers
{
    public class HomeController : BaseAdminController
    {

        [UnAuthorize]
        public ActionResult Index()
        {


            if (CurrentInfo.IsLogin)
            {

                return RedirectToAction("DriftBottleList", "Home", new { area = "Mobile" });

            }
            else
            {
                return View();
            }
            
        }

        public ActionResult List()
        {
            return View();
        }
        [UnAuthorize]
        public ActionResult Login()
        {
            return View();
        }


        [UnAuthorize]
        public ActionResult Register()
        {
            return View();
        }
        [UnAuthorize]
        public ActionResult RegisterNotice()
        {
            return View();
        }

        public static object lockRegister = 1;
        [UnAuthorize]
        public JsonResult RegisterAjax(string userName, string realName, bool sexual, string password, string ConfirmPassword, bool hasReadRegister)
        {
            if (hasReadRegister == false)
            {
                return Json(new Result(false, "请先阅读《用户注册协议》，同意后方可注册！"), JsonRequestBehavior.AllowGet);
            }

            if (realName.Length == 0)
            {
                return Json(new Result(false, "请输入昵称"), JsonRequestBehavior.AllowGet);
            }

            if (realName.Length > 10)
            {
                return Json(new Result(false, "昵称不要超过10个字"), JsonRequestBehavior.AllowGet);
            }

            if (realName.ToLower().Contains("admin"))
            {
                return Json(new Result(false, "昵称不要包含admin"), JsonRequestBehavior.AllowGet);
            }


            if (!password.Equals(ConfirmPassword))
            {
                return Json(new Result(false, "两次输入的密码不一致"), JsonRequestBehavior.AllowGet);
            }

            lock (lockRegister)
            {
                bool flage = ServiceHelper.GetUsersService.Exists(t => t.UserName == userName);
                if (flage)
                {
                    return Json(new Result(false, "账号已经被注册，请更换"), JsonRequestBehavior.AllowGet);
                }
                Users user = new Users();
                user.UserName = userName;
                user.RealName = realName;
                user.Sexual = sexual;
                user.Password = password;
                //添加账号PasswordSalt随机
                user.PasswordSalt = Common.TextFilter.GetPasswordSalt();//Common.TextFilter.Substring(Guid.NewGuid().ToString("N"), 10, false);
                string endPassword = user.Password + user.PasswordSalt;
                //计算密码
                user.Password = Common.SecureHelper.MD5(endPassword);

                //user的必填字段
                user.NeedAccount = true;
                Users adminUser = ServiceHelper.GetUsersService.GetEntity(5);
                user.ShopsID = adminUser.ShopsID;
                user.DefaultStoreID = adminUser.DefaultStoreID;
                user.LoginTimes = 1;
                user.IsIntention = false;
                user.CreateTime = DateTime.Now;
                user.CreateUserID = adminUser.ID;

                bool success = user.Add() == null ? false : true;
                if (success)
                {
                    //Admin.Controllers.HomeController ach = new Admin.Controllers.HomeController();
                    //ach.LoginAjax(userName, password, false);
                    //return RedirectToAction("Index", "Home", new { area = "Admin" });
                    return Json(new Result(true, "注册成功，正在登录"), JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new Result(false, "注册失败，请联系管理员"), JsonRequestBehavior.AllowGet);
                }

                //    LoginAjax("afb4b11b25864820b8e42f4b583d46ff", "afb4b11b25864820b8e42f4b583d46ff", false);
                //    return RedirectToAction("Index", "Home", new { area = "Admin" });

                //return View();
                return null;
            }
        }


        public ActionResult DriftBottleList()
        {
            return View();
        }

        public ActionResult SendMassage()
        {
            if (Common.SecureHelper.MD5(Request["BottleID"].ToString() + Common.SecureHelper.JeasuAutoKey) == Request["massageKey"].ToString())
            {
                Bottle bottle = ServiceHelper.GetBottleService.GetEntity(Convert.ToInt32(Request["BottleID"]), false);

                //如果瓶子已经被回复过，并且不是本人创建的瓶子，也不是本人回复过的瓶子，则提示用户
                if (bottle.FirstReplyUserID != null && (bottle.CreateUserID != CurrentInfo.CurrentUser.ID && bottle.FirstReplyUserID != CurrentInfo.CurrentUser.ID))
                {
                    ViewBag.HasReply = "1";
                }
                //设置瓶子被查看的时间
                if (bottle.CreateUserID == CurrentInfo.CurrentUser.ID)
                {
                    //本人查看的时间
                    bottle.CreateViewTime = DateTime.Now;
                }
                else
                {
                    //捡到瓶子的人查看的时间
                    bottle.ReplyViewTime = DateTime.Now;
                }
                bottle.Update();
            }

            return View();
        }

        public ActionResult MyBottleList()
        {
            List<int> list = ServiceHelper.GetBottleService.GetNoReadCount(CurrentInfo.CurrentUser.ID);
            ViewBag.FishingText = list[0] > 0 ? ("+" + list[0]) : "";
            ViewBag.ThrowText = list[1] > 0 ? ("+" + list[1]) : "";
            return View();
        }

        public ActionResult GetBottleList()
        {
            TimeSpan ts = DateTime.Now - new DateTime(2020, 2, 25);
            DataTable dt = ServiceHelper.GetBottleService.GetBottleListByRd(CurrentInfo.CurrentUser.ID, DateTime.Now.AddDays(0 - ts.Days));
            List<Bottle> listBottle = dt.ToList<Bottle>();
            foreach (var item in listBottle)
            {
                item.MassageKey = Common.SecureHelper.MD5(item.ID + Common.SecureHelper.JeasuAutoKey);
            }
            //返回值记得加total前段要用
            return Json(listBottle, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMyBottleList(int pageIndex, int bottleType, string massageKey)
        {
            if (Common.SecureHelper.MD5(CurrentInfo.CurrentUser.ID + Common.SecureHelper.JeasuAutoKey) == massageKey)
            {
                int outTotal = 0;
                List<Bottle> listBottle;
                //我回复的瓶子
                if (bottleType == 1)
                {
                    listBottle = ServiceHelper.GetBottleService.GetEntitiesByPage(pageIndex + 1, 10, out outTotal, t => t.FirstReplyUserID == CurrentInfo.CurrentUser.ID, false, t => t.LastReplyTime).ToList();
                }
                else//我丢出去的瓶子
                {
                    listBottle = ServiceHelper.GetBottleService.GetEntitiesByPage(pageIndex + 1, 10, out outTotal, t => t.CreateUserID == CurrentInfo.CurrentUser.ID, false, t => t.LastReplyTime).ToList();
                }

                foreach (var item in listBottle)
                {
                    item.MassageKey = Common.SecureHelper.MD5(item.ID + Common.SecureHelper.JeasuAutoKey);
                    item.LastReplyMassage = SubMsg(item.LastReplyMassage);
                }


                //el.CreateUserName


                var lastList = listBottle.Select(t => new
                {
                    t.ID,
                    t.Massage,
                    t.MassageKey,
                    t.FishingCount,
                    t.CreateUserID,
                    t.FirstReplyUserID,
                    t.LastReplyUserID,
                    t.CreateUserName,
                    t.LastReplyUserName,
                    t.LastReplyMassage,
                    LastReplyTime = Convert.ToDateTime(t.LastReplyTime).ToString("yyyy/MM/dd HH:mm:ss"),
                    ReplyViewTime = Convert.ToDateTime(t.ReplyViewTime).ToString("yyyy/MM/dd HH:mm:ss"),
                    CreateViewTime = Convert.ToDateTime(t.CreateViewTime).ToString("yyyy/MM/dd HH:mm:ss"),
                    CreateTime = Convert.ToDateTime(t.CreateTime).ToString("yyyy/MM/dd HH:mm:ss"),
                    t.Sexual
                }).ToList();


                return Json(lastList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new Result(false, "发生错误，我的瓶子获取失败"), JsonRequestBehavior.AllowGet);
            }

        }

        public string SubMsg(string msg)
        {
            if (msg != null && msg.Length > 16)
            {
                return msg.Substring(0, 17) + "...";
            }
            else
            {
                return msg;
            }
        }

        public ActionResult SendDriftBottle()
        {
            return View();
        }


        public JsonResult GetNoReadCount()
        {
            List<int> list = ServiceHelper.GetBottleService.GetNoReadCount(CurrentInfo.CurrentUser.ID);
            list.Add(list[0] + list[1]);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewMassage(int ConversationMaxID, int bottleID, string massageKey)
        {
            if (Common.SecureHelper.MD5(bottleID + Common.SecureHelper.JeasuAutoKey) == massageKey)
            {
                int outTotal = 0;
                var listConversation = ServiceHelper.GetConversationService.GetEntities(t => t.BottleID == bottleID && t.ID > ConversationMaxID).ToList().Select(t => new
                {
                    ID = t.ID,
                    BottleID = t.BottleID,
                    Massage = t.Massage,
                    CreateUserName = t.CreateUserName,
                    Sexual = t.Sexual,
                    CreateUserID = t.CreateUserID,
                    CreateTime = ((DateTime)t.CreateTime).ToString("MM/dd HH:mm:ss")
                }).ToList();

                //返回值记得加total前段要用
                return Json(listConversation, JsonRequestBehavior.AllowGet);


            }
            else
            {
                return Json(new Result(false, "发生错误，获取最新聊天记录失败"), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMassageListByPage(int ConversationMinid, int bottleID, string massageKey)
        {
            if (Common.SecureHelper.MD5(bottleID + Common.SecureHelper.JeasuAutoKey) == massageKey)
            {
                int outTotal = 0;
                //取历史聊天记录要取比前端小的ID，更早
                var listConversation = ServiceHelper.GetConversationService.GetEntitiesByPage(1, 5, out outTotal, t => t.BottleID == bottleID && t.ID < ConversationMinid, false, t => t.ID).ToList().Select(t => new
                {
                    ID = t.ID,
                    BottleID = t.BottleID,
                    Massage = t.Massage,
                    CreateUserName = t.CreateUserName,
                    Sexual = t.Sexual,
                    CreateUserID = t.CreateUserID,
                    CreateTime = ((DateTime)t.CreateTime).ToString("MM/dd HH:mm:ss")
                }).ToList();
                //返回值记得加total前段要用
                return Json(listConversation, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new Result(false, "发生错误，获取聊天记录失败"), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddDriftBottle(Bottle bottle)
        {
            //Bottle bottle = new Bottle();
            if (string.IsNullOrWhiteSpace(bottle.Massage))
            {
                return Json(new Result(false, "多少说点东西！"), JsonRequestBehavior.AllowGet);
            }
            if (bottle.Massage.Length > 300)
            {
                return Json(new Result(false, "内容太多了！"), JsonRequestBehavior.AllowGet);
            }

            string currentUserNanme = "妩媚的阳光";
            //如果是管理员丢瓶子，则随机名字
            if (CurrentInfo.CurrentUser.ID == 6)
            {
                Random rd = new Random();
                currentUserNanme = NetUserNames.NetUserGirl[rd.Next(0, NetUserNames.NetUserGirl.Count)];
            }
            else
            {
                currentUserNanme = CurrentInfo.CurrentUser.RealName;
            }

            bottle.FishingCount = 0;
            bottle.CreateTime = DateTime.Now;
            bottle.CreateUserID = CurrentInfo.CurrentUser.ID;
            bottle.Sexual = CurrentInfo.CurrentUser.Sexual;
            bottle.CreateUserName = currentUserNanme;

            bottle.LastReplyMassage = bottle.Massage;
            bottle.LastReplyUserID = CurrentInfo.CurrentUser.ID; ;


            bottle.LastReplyUserName = currentUserNanme;
            bottle.LastReplyTime = DateTime.Now;
            bottle.CreateViewTime = DateTime.Now;
            bottle.ReplyViewTime = DateTime.Now;
            bool flage = bottle.Add() == null ? false : true;
            return Json(new Result(flage, ResultType.Add), JsonRequestBehavior.AllowGet);
        }

        public static object lockReply = 1;
        public JsonResult ReplyBottle(Conversation conversation)
        {
            lock (lockReply)
            {

                //验证密匙
                if (Common.SecureHelper.MD5(conversation.BottleID + Common.SecureHelper.JeasuAutoKey) == conversation.MassageKey)
                {
                    //Bottle bottle = new Bottle();
                    if (string.IsNullOrWhiteSpace(conversation.Massage))
                    {
                        return Json(new Result(false, "多少说点东西！"), JsonRequestBehavior.AllowGet);
                    }

                    //Bottle bottle = new Bottle();
                    if (conversation.Massage.Length <= 3)
                    {
                        return Json(new Result(false, "内容太少咯，请多说点！"), JsonRequestBehavior.AllowGet);
                    }

                    if (conversation.Massage.Length > 300)
                    {
                        return Json(new Result(false, "内容太多了！"), JsonRequestBehavior.AllowGet);
                    }

                    //如果FirstReplyUserID不为null 则说明有人回复过了
                    if (ServiceHelper.GetBottleService.Exists(t => t.ID == conversation.BottleID && t.FirstReplyUserID != null && t.CreateUserID != CurrentInfo.CurrentUser.ID && t.FirstReplyUserID != CurrentInfo.CurrentUser.ID))
                    {
                        return Json(new Result(false, "来迟了，瓶子已经被回复，请换个瓶子试试"), JsonRequestBehavior.AllowGet);
                    }

                    Bottle bottle = ServiceHelper.GetBottleService.GetEntity((int)conversation.BottleID, false);
                    //如果是第一个回复，则也需要把瓶子插入到回话中
                    if (!ServiceHelper.GetConversationService.Exists(t => t.BottleID == conversation.BottleID))
                    {
                        //第一个回复的人不可以是自己
                        if (bottle.CreateUserID == CurrentInfo.CurrentUser.ID)
                        {
                            return Json(new Result(false, "还是等有缘人回复把"), JsonRequestBehavior.AllowGet);
                        }

                        Conversation cv = new Conversation();
                        cv.Massage = bottle.Massage;
                        cv.BottleID = conversation.BottleID;
                        cv.CreateUserName = bottle.CreateUserName;
                        cv.CreateUserID = bottle.CreateUserID;
                        cv.CreateTime = bottle.CreateTime;
                        cv.Sexual = bottle.Sexual;
                        cv.Add();

                        //更新瓶子的回复人
                        bottle.FishingCount = bottle.FishingCount + 1;//被打捞了一次
                        bottle.FirstReplyUserID = CurrentInfo.CurrentUser.ID;
                        //如果是管理员，则随机名字
                        if (CurrentInfo.CurrentUser.ID == 6)
                        {
                            string netUser = "一地毛";
                            Random rd = new Random();
                            if (bottle.Sexual == true)
                            {
                                netUser = NetUserNames.NetUserGirl[rd.Next(0, NetUserNames.NetUserGirl.Count)];
                            }
                            else
                            {
                                netUser = NetUserNames.NetUserBoy[rd.Next(0, NetUserNames.NetUserBoy.Count)];
                            }
                            bottle.FirstReplyUserName = netUser;
                        }
                        else
                        {
                            bottle.FirstReplyUserName = CurrentInfo.CurrentUser.RealName;
                        }
                        //扣除金币
                        CurrentInfo.CurrentUser.GoldCoin = CurrentInfo.CurrentUser.GoldCoin == null ? 0 : CurrentInfo.CurrentUser.GoldCoin;
                        CurrentInfo.CurrentUser.Fishing = CurrentInfo.CurrentUser.Fishing - 1;
                    }

                    //bottle.Update();
                    conversation.CreateUserID = CurrentInfo.CurrentUser.ID;

                    //如果是管理员，则随机名字，取上创建人的名字
                    if (CurrentInfo.CurrentUser.ID == 6)
                    {
                        //说明这个瓶子是管理员自己发的
                        if (bottle.CreateUserID == 6)
                        {
                            conversation.CreateUserName = bottle.CreateUserName;
                            conversation.Sexual = bottle.Sexual;
                        }
                        else//如果这个瓶子是别人发的
                        {
                            conversation.CreateUserName = bottle.FirstReplyUserName;
                            conversation.Sexual = !((bool)bottle.Sexual);
                        }

                    }
                    else
                    {
                        conversation.CreateUserName = CurrentInfo.CurrentUser.RealName;
                        conversation.Sexual = CurrentInfo.CurrentUser.Sexual;
                    }

                    conversation.CreateTime = DateTime.Now;
                    conversation.MassageKey = string.Empty;
                    //设置瓶子的最后回复内容
                    bottle.LastReplyUserID = CurrentInfo.CurrentUser.ID;
                    if (CurrentInfo.CurrentUser.ID == 6)//如果是管理员则设置最后回复名字是自动生成的
                    {
                        bottle.LastReplyUserName = conversation.CreateUserName;
                    }
                    else
                    {
                        bottle.LastReplyUserName = CurrentInfo.CurrentUser.RealName;
                    }


                    bottle.LastReplyMassage = conversation.Massage;
                    bottle.LastReplyTime = DateTime.Now;

                    bottle.Update();


                    bool flage = conversation.Add() == null ? false : true;
                    return Json(new Result(flage, ResultType.Other), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new Result(false, "发生错误，回复失败"), JsonRequestBehavior.AllowGet);
                }
            }
        }


        public ActionResult UserCenter()
        {
            return View();

        }
    }
}