using Joint.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Joint.IService;
using Joint.DLLFactory;
using Joint.Entity;
using Joint.Web.Areas.Admin.Models;
using System.Linq.Expressions;
using Joint.Common;
using System.Transactions;

namespace Joint.Web.Areas.Admin.Controllers
{
    public class UserController : BaseAdminController
    {
        // GET: Admin/User
        public ActionResult Index(OneKeySearchModel searchModel)
        {
            Expression<Func<Users, Boolean>> lbdWhere = null;
            if (searchModel.SearchStr != null)
            {
                searchModel.SearchStr = searchModel.SearchStr.Trim();
                if (!string.IsNullOrWhiteSpace(searchModel.SearchStr))
                {
                    lbdWhere = t => t.RealName.Contains(searchModel.SearchStr)
                       || t.UserName.Contains(searchModel.SearchStr)
                       || t.Phone.Contains(searchModel.SearchStr)
                       || t.TEL.Contains(searchModel.SearchStr)
                       || t.Idcard.Contains(searchModel.SearchStr)
                       || t.WorkNum.Contains(searchModel.SearchStr)
                       || t.UserName.Contains(searchModel.SearchStr)
                       || t.Stores.Shops.ShopName.Contains(searchModel.SearchStr)
                       || t.Stores.StoreName.Contains(searchModel.SearchStr);
                }
            }

            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            var users = usersService.GetEntitiesByPage(searchModel.PageIndex, 10, lbdWhere, false, t => t.ID);

            ////该用户在当前门店下的所有角色
            //var userAllRole =ServiceHelper.GetUsersService.GetEntity(0).RelationUserRole;
            ////过滤出用户当前门店的所有角色
            //var userCurrentStoreRole = userAllRole.Where(t => t.Role.StoreID == currentSore.ID);

            ////当前用户使用的角色名称
            //initInfo.CurrentRoleName = string.Join(",", userCurrentStoreRole.Select(t => t.Role.Name).ToList());

            //转换数据模型
            var listData = users.Models.Select(t => new UserListModel
            {
                ID = t.ID,
                WorkNum = t.WorkNum,
                RealName = t.RealName,
                Phone = t.Phone,
                RoleName = ServiceHelper.GetUsersService.GetUserAllRole(t.ID),//t.WorkTypeID == null ? "" : workTypeService.GetEntity(Convert.ToInt32(t.WorkTypeID)).WorkTypeName,
                UserName = t.UserName,
                BasicSalary = t.BasicSalary,
                WeiXinVisible = t.WeiXinVisible == true ? "显示" : "隐藏",
                Idcard = t.Idcard,
                ShopName = t.Stores.Shops.ShopName,
                StoreName = t.Stores.StoreName,
                CreateTime = t.CreateTime,
                Disabled = t.Disabled
            });

            var listUser = new PageModel<UserListModel>
            {
                Models = listData.ToList(),
                pagingInfo = users.pagingInfo
            };

            ViewBag.SearchModel = searchModel;

            return View(listUser);
        }


        public ActionResult AddTestAccount()
        {
            InitInfo.Instance.SetCurrentModule("Admin", "User", "AddTestAccount");
            Entity.Users editUser = null;
            //有ID说明是编辑状态
            if (Request["ID"] != null && !string.IsNullOrWhiteSpace(Request["ID"].ToString()))
            {
                IUsersService usersService = ServiceFactory.Create<IUsersService>();
                editUser = usersService.GetEntity(Convert.ToInt32(Request["ID"]));
                //如果不是管理员，也不是内部销售，也不是本店的员工，则无权限修改
                if (!CurrentInfo.IsAdministrator && CurrentInfo.CurrentUser.Remark != "B4内部销售" && editUser.DefaultStoreID != CurrentInfo.CurrentStore.ID)
                {
                    return RedirectToAction("Error403", "Home", new { area = "Admin" });
                }
            }
            //editUser为null的时候说明是添加
            int? shopID = editUser == null ? CurrentInfo.CurrentShop.ID : editUser.ShopsID;
            int? storeID = editUser == null ? CurrentInfo.CurrentStore.ID : editUser.DefaultStoreID;

            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            List<Stores> allStores = storesService.GetEntities(t => t.ShopId == shopID && t.Disabled == false).ToList();// CurrentInfo.CurrentShop.ID
            SelectList allStoresSelect = new SelectList(allStores, "ID", "StoreName", storeID);// CurrentInfo.CurrentStore.ID
            ViewData["allStoresSelect"] = allStoresSelect;
            //在添加测试账号的时候则默认不显示，只允许添加到测试门店里面
            ViewData["showStores"] = false;
            ViewBag.IsIntention = true;

            //账号版本，测试账号开户用得到，其他无用
            List<SelectListItem> listShopType = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(typeof(ShopTypeEnum)))
            {
                listShopType.Add(new SelectListItem
                {
                    Text = item.ToString(),
                    Value = ((int)item).ToString()
                });
            }
            //InventoryDetailsType.Insert(0, new SelectListItem { Text = "所有类别", Value = "0" });
            //InventoryDetailsType[6].Selected = true;
            ViewData["ListShopType"] = new SelectList(listShopType, "Value", "Text", "1");

            ViewBag.BackUrl = "/Admin/IntentionUser/Index";

            return View("ShowAddUser");
        }


        public ActionResult ShowAddUser()
        {
            InitInfo.Instance.SetCurrentModule("Admin", "User", "Index");
            Entity.Users editUser = null;
            //有ID说明是编辑状态
            if (Request["ID"] != null)
            {
                IUsersService usersService = ServiceFactory.Create<IUsersService>();
                editUser = usersService.GetEntity(Convert.ToInt32(Request["ID"]));
                //如果不是管理员，并且不是店铺内部人员，禁止修改改员工信息
                if (!CurrentInfo.IsAdministrator && CurrentInfo.CurrentUser.Remark != "B4内部销售" && editUser.Stores.ShopId != CurrentInfo.CurrentShop.ID)
                {
                    return RedirectToAction("Error403", "Home", new { area = "Admin" });
                }
            }

            //editUser为null的时候说明是添加
            int? shopID = editUser == null ? CurrentInfo.CurrentShop.ID : editUser.ShopsID;
            int? storeID = editUser == null ? CurrentInfo.CurrentStore.ID : editUser.DefaultStoreID;

            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            List<Stores> allStores = storesService.GetEntities(t => t.ShopId == shopID && t.Disabled == false).ToList();// CurrentInfo.CurrentShop.ID 
            SelectList allStoresSelect = new SelectList(allStores, "ID", "StoreName", storeID);// CurrentInfo.CurrentStore.ID
            ViewData["allStoresSelect"] = allStoresSelect;

            //只有管理员才能给所有店铺添加人员
            ViewData["showStores"] = CurrentInfo.IsShopAdmin;
            ViewBag.IsIntention = false;

            //账号版本，测试账号开户用得到，其他无用
            List<SelectListItem> listShopType = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(typeof(ShopTypeEnum)))
            {
                listShopType.Add(new SelectListItem
                {
                    Text = item.ToString(),
                    Value = ((int)item).ToString()
                });
            }
            //InventoryDetailsType.Insert(0, new SelectListItem { Text = "所有类别", Value = "0" });
            //InventoryDetailsType[6].Selected = true;
            ViewData["ListShopType"] = new SelectList(listShopType, "Value", "Text", "1");

            return View();
        }



        /// <summary>
        /// 构建查询表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<Users, Boolean>> BuildSearchCriteria(OneKeySearchModel searchModel)
        {
            DynamicLambda<Users> bulider = new DynamicLambda<Users>();
            Expression<Func<Users, Boolean>> expr = null;
            //判断是否是商家的总账号，如果是的话，可以看所有员工的数据，否则只能看自己门店的数据
            if (CurrentInfo.IsShopAdmin)
            {
                //IStoresService storesService = ServiceFactory.Create<IStoresService>();
                //List<int> allStoresID = storesService.GetEntities(t => t.ShopId == CurrentInfo.CurrentShop.ID && t.Disabled == false).Select(t => t.ID).ToList();
                //foreach (var item in allStoresID)
                //{

                //}
                Expression<Func<Users, Boolean>> tmpStoreID = t => t.ShopsID == CurrentInfo.CurrentShop.ID && t.IsIntention != true;
                expr = bulider.BuildQueryAnd(expr, tmpStoreID);
            }
            else
            {
                Expression<Func<Users, Boolean>> tmpStoreID = t => t.DefaultStoreID == CurrentInfo.CurrentStore.ID && t.IsIntention != true;
                expr = bulider.BuildQueryAnd(expr, tmpStoreID);
            }

            if (searchModel.SearchStr != null)
            {
                searchModel.SearchStr = searchModel.SearchStr.Trim();
                if (!string.IsNullOrWhiteSpace(searchModel.SearchStr))
                {
                    Expression<Func<Users, Boolean>> tmpUser = t => (t.UserName.Contains(searchModel.SearchStr)
                       || t.Phone.Contains(searchModel.SearchStr)
                       || t.TEL.Contains(searchModel.SearchStr)
                       || t.Idcard.Contains(searchModel.SearchStr)
                       || t.WorkNum.Contains(searchModel.SearchStr)
                       || t.RealName.Contains(searchModel.SearchStr));

                    expr = bulider.BuildQueryAnd(expr, tmpUser);
                }
            }

            //如果是意向客户 把测试管理员账号和内部销售的账号过滤掉
            if (CurrentInfo.CurrentUser.IsIntention == true)
            {
                Expression<Func<Users, Boolean>> tmpAdmin = t => t.Remark != "B4内部销售" && t.Remark != "B4内部售后";
                expr = bulider.BuildQueryAnd(expr, tmpAdmin);
            }

            return expr;
        }


        public ActionResult CusUserManage(OneKeySearchModel searchModel)
        {
            Expression<Func<Users, Boolean>> lbdWhere = BuildSearchCriteria(searchModel);


            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            var users = usersService.GetEntitiesByPage(searchModel.PageIndex, 10, lbdWhere, false, t => t.ID);

            //var ddddd = users.Models.ToList();

            //转换数据模型
            var listData = users.Models.Select(t => new UserListModel
            {
                ID = t.ID,
                WorkNum = t.WorkNum,
                RealName = t.RealName,
                Phone = t.Phone,
                BasicSalary = t.BasicSalary,
                WeiXinVisible = t.WeiXinVisible == true ? "显示" : "隐藏",
                UserName = t.UserName,
                Idcard = t.Idcard,
                StoreName = t.Stores.StoreName,
                CreateTime = t.CreateTime,
                Disabled = t.Disabled
            });


            var listUser = new PageModel<UserListModel>
            {
                Models = listData.ToList(),
                pagingInfo = users.pagingInfo
            };

            ViewBag.SearchModel = searchModel;

            return View(listUser);
        }

        //private void AdddRelationUserWorkType(int UserID, int WorkTypeID)
        //{
        //    IRelationUserWorkTypeService relationUserWorkTypeService = ServiceFactory.Create<IRelationUserWorkTypeService>();
        //    var dbData = relationUserWorkTypeService.GetFirstOrDefault(t => t.UserID == UserID);

        //    //存在就更新，不存在就添加
        //    if (dbData == null)
        //    {
        //        RelationUserWorkType model = new RelationUserWorkType();
        //        model.UserID = UserID;
        //        model.WorkTypeID = WorkTypeID;
        //        model.CreateTime = DateTime.Now;
        //        model.CreateUserID = CurrentInfo.CurrentUser.ID;
        //        relationUserWorkTypeService.AddEntity(model);
        //    }
        //    else
        //    {
        //        dbData.WorkTypeID = WorkTypeID;
        //        relationUserWorkTypeService.UpdateEntity(dbData);
        //    }
        //}

        public JsonResult GetUserByID(int ID = 0)
        {
            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            Users singleUser = new Users();
            if (ID != 0)
            {
                singleUser = usersService.GetEntity(ID);
            }

            return Json(singleUser, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddUser(Entity.Users user, int? shopType)
        {
            //如果是测试账号则默认门店设置在45下面
            if (user.IsIntention == true)
            {
                if (shopType == 1)
                {
                    user.DefaultStoreID = 45;
                }
                else if (shopType == 2)
                {
                    user.DefaultStoreID = 101;
                }
                else
                {
                    return Json(new Result(false, "该版本还未开通，无法添加"), JsonRequestBehavior.AllowGet);
                }
            }

            IService.IUsersService usersService = ServiceFactory.Create<IUsersService>();
            //是否需要账号
            if (user.NeedAccount)
            {
                if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
                {
                    return Json(new Result(false, "用户名和密码不能为空，若无需账号请选择‘不需要登录账号’"), JsonRequestBehavior.AllowGet);
                }

                bool flage = usersService.Exists(t => t.UserName == user.UserName);
                if (flage)
                {
                    return Json(new Result(false, "已经存在同名用户，请更换"), JsonRequestBehavior.AllowGet);
                }
                //添加账号PasswordSalt随机
                user.PasswordSalt = Common.TextFilter.GetPasswordSalt();//Common.TextFilter.Substring(Guid.NewGuid().ToString("N"), 10, false);
                string endPassword = user.Password + user.PasswordSalt;
                //计算密码
                user.Password = Common.SecureHelper.MD5(endPassword);
            }

            //user.DefaultStoreID = CurrentInfo.CurrentStore.ID;
            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            var storesModel = storesService.GetEntity(user.DefaultStoreID.ToString().ToInt32());
            user.ShopsID = storesModel.ShopId;

            if (!string.IsNullOrWhiteSpace(user.Photo))
            {

                //生成缩略图  并删除原图
                string fileFullName = FileHelper.Move(user.Photo, "/Upload/Reality/" + CurrentInfo.CurrentStore.ID + "/UserImg/");
                string extension = System.IO.Path.GetExtension(fileFullName);
                //缩略图路径
                string thumbnailPath = ImgHelper.GetThumbnailPathByWidth(fileFullName, 60);
                //生成缩略图
                ImgHelper.MakeThumbnail(
                    System.Web.HttpContext.Current.Server.MapPath(fileFullName),
                    System.Web.HttpContext.Current.Server.MapPath(thumbnailPath),
                    60,
                    60,
                    "W",
                    extension
                 );
                FileHelper.DeleteFile(System.Web.HttpContext.Current.Server.MapPath(fileFullName));

                user.Photo = thumbnailPath;

                //user.Photo = FileHelper.Move(user.Photo, "/Upload/Reality/" + CurrentInfo.CurrentStore.ID + "/UserImg/");
            }

            if (!string.IsNullOrWhiteSpace(user.IdcardPhoto))
            {
                user.IdcardPhoto = FileHelper.Move(user.IdcardPhoto, "/Upload/Reality/" + CurrentInfo.CurrentStore.ID + "/UserImg/");
            }

            user.CreateUserID = CurrentInfo.CurrentUser.ID;
            user.CreateTime = DateTime.Now;
            user.Disabled = false;

            bool success = false;
            using (TransactionScope transactionScope = TransactionScopeHelper.GetTran())
            {
                var data = usersService.AddEntity(user);
                //if (data.WorkTypeID != null)
                //{
                //    AdddRelationUserWorkType(data.ID, Convert.ToInt32(data.WorkTypeID));
                //}
                transactionScope.Complete();
                success = data != null;
            }

            return Json(new Result(success, ResultType.Add), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateUser(Entity.Users user)
        {
            if (user.ID == 0)
            {
                return Json(new Result(false, "参数错误"), JsonRequestBehavior.AllowGet);
            }

            IService.IUsersService usersService = ServiceFactory.Create<IUsersService>();
            var dbData = usersService.GetEntity(user.ID, false);

            //如果不是管理员，是不允许禁用店铺管理员账号的，包括店铺管理员自己也不行
            if (!CurrentInfo.IsAdministrator)
            {
                if (user.NeedAccount == false && user.ID == CurrentInfo.CurrentShop.AdminUserID)
                {
                    return Json(new Result(false, "店铺管理员必须有登录账号"), JsonRequestBehavior.AllowGet);
                }
            }


            //是否需要账号
            if (user.NeedAccount)
            {
                if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
                {
                    return Json(new Result(false, "用户名和密码不能为空，若无需账号请选择‘不需要登录账号’"), JsonRequestBehavior.AllowGet);
                }

                bool flage = usersService.Exists(t => t.ID != user.ID && t.UserName == user.UserName);
                if (flage)
                {
                    return Json(new Result(false, "已经存在同名用户，请更换"), JsonRequestBehavior.AllowGet);
                }

                //判断用户是否修改了密码
                if (dbData.Password != user.Password)
                {
                    string endPassword = user.Password + dbData.PasswordSalt;
                    user.Password = Common.SecureHelper.MD5(endPassword);
                }
            }
            else
            {
                user.UserName = null;
                user.Password = null;
                user.PasswordSalt = null;
            }

            //user.DefaultStoreID = dbData.DefaultStoreID;
            user.ShopsID = dbData.ShopsID;

            if (!string.IsNullOrWhiteSpace(user.Photo))
            {
                if (dbData.Photo != user.Photo)
                {
                    //生成缩略图  并删除原图
                    string fileFullName = FileHelper.Move(user.Photo, "/Upload/Reality/" + CurrentInfo.CurrentStore.ID + "/UserImg/");
                    string extension = System.IO.Path.GetExtension(fileFullName);
                    //缩略图路径
                    string thumbnailPath = ImgHelper.GetThumbnailPathByWidth(fileFullName, 60);
                    //生成缩略图
                    ImgHelper.MakeThumbnail(
                        System.Web.HttpContext.Current.Server.MapPath(fileFullName),
                        System.Web.HttpContext.Current.Server.MapPath(thumbnailPath),
                        60,
                        60,
                        "W",
                        extension
                     );
                    FileHelper.DeleteFile(System.Web.HttpContext.Current.Server.MapPath(fileFullName));

                    user.Photo = thumbnailPath;
                }
            }
            else
            {
                user.Photo = dbData.Photo;
            }


            if (!string.IsNullOrWhiteSpace(user.IdcardPhoto))
            {
                if (dbData.IdcardPhoto != user.IdcardPhoto)
                {
                    user.IdcardPhoto = FileHelper.Move(user.IdcardPhoto, "/Upload/Reality/" + CurrentInfo.CurrentStore.ID + "/UserImg/");
                }
            }
            else
            {
                user.IdcardPhoto = dbData.IdcardPhoto;
            }

            user.CreateUserID = dbData.CreateUserID;
            user.CreateTime = dbData.CreateTime;
            user.Disabled = dbData.Disabled;
            user.PasswordSalt = dbData.PasswordSalt;
            //保持上次登录的信息
            user.LastLoginArea = dbData.LastLoginArea;
            user.LastLoginIP = dbData.LastLoginIP;
            user.LastLoginTime = dbData.LastLoginTime;
            user.LoginTimes = dbData.LoginTimes;

            bool success = false;
            using (TransactionScope transactionScope = TransactionScopeHelper.GetTran())
            {
                success = usersService.UpdateEntity(user);
                //if (user.WorkTypeID != null)
                //{
                //    AdddRelationUserWorkType(user.ID, Convert.ToInt32(user.WorkTypeID));
                //}
                transactionScope.Complete();
            }

            //如果更新的是当前用户资料，则更新用户头像
            if (success && CurrentInfo.CurrentUser.ID == user.ID)
            {
                CurrentInfo.CurrentUser.Photo = user.Photo;
            }


            return Json(new Result(success, ResultType.Update), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReDeleteUser(int ID)
        {
            //throw new Exception();
            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            var data = usersService.GetEntity(ID);
            data.Disabled = false;
            bool flage = usersService.UpdateEntity(data);
            return Json(new Result(flage, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUser(int ID)
        {
            RefuseIntentionUser();

            //throw new Exception();
            IUsersService usersService = ServiceFactory.Create<IUsersService>();

            //如果不是系统管理员，则没有权限禁用自己店铺的管理员账号
            if (!CurrentInfo.IsAdministrator)
            {
                if (ID == CurrentInfo.CurrentShop.AdminUserID)
                {
                    return Json(new Result(false, "店铺管理员账号不可禁用或删除"), JsonRequestBehavior.AllowGet);
                }
            }

            var data = usersService.GetEntity(ID);
            data.Disabled = true;
            bool flage = usersService.UpdateEntity(data);
            return Json(new Result(flage, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUsers(string IDs)
        {
            List<int> idArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            var data = usersService.GetEntities(idArr);

            bool flage = true;
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var item in data)
                {
                    item.Disabled = true;
                    flage = usersService.UpdateEntity(item);
                    if (flage == false)
                    {
                        break;
                    }
                }
                scope.Complete();
            }


            return Json(new Result(flage, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetAllWorkType(int UserID = 0)
        //{
        //    //IRelationUserWorkTypeService relationUserWorkTypeService = ServiceFactory.Create<IRelationUserWorkTypeService>();
        //    //RelationUserWorkType userWorkType = null;
        //    //if (UserID > 0)
        //    //{
        //    //    userWorkType = relationUserWorkTypeService.GetFirstOrDefault(t => t.UserID == UserID);
        //    //}
        //    IUsersService userService = ServiceFactory.Create<IUsersService>();
        //    //如果是新增则默认是当前用户的storeID，否则的话，就是当前编辑用户的的storeID
        //    int? storeID = CurrentInfo.CurrentStore.ID;
        //    if (UserID > 0)
        //    {
        //        var user = userService.GetEntity(UserID);
        //        storeID = user.DefaultStoreID;
        //    }


        //    IWorkTypeService workTypeService = ServiceFactory.Create<IWorkTypeService>();
        //    var list = workTypeService.GetEntities(t => t.StoreID == storeID && t.Disabled == false).Select(t => new// CurrentInfo.CurrentStore.ID
        //    {
        //        ID = t.ID,
        //        Name = t.WorkTypeName,
        //    }).ToList();
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetStoreUserJsonByStoreID(int pageindex, int pagesize, string cols, string qtext)
        {
            Expression<Func<Users, Boolean>> lbdWhere = null;
            //构建查询条件
            if (!string.IsNullOrWhiteSpace(qtext))
            {
                qtext = qtext.Trim();
                lbdWhere = t => t.DefaultStoreID == CurrentInfo.CurrentStore.ID
                    && (t.RealName.Contains(qtext)
                    || t.Phone.Contains(qtext)
                    || t.WorkNum.Contains(qtext));
            }
            else
            {
                lbdWhere = t => t.DefaultStoreID == CurrentInfo.CurrentStore.ID;
            }



            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            int total;
            //pageindex默认从0开始，也就是第一页是0，所以pageindex + 1
            var data = usersService.GetEntitiesByPage(pageindex + 1, pagesize, out total, lbdWhere, false, t => t.ID).Select(t => new  //(t => t.DefaultStoreID == CurrentInfo.CurrentUser.DefaultStoreID)
            {
                ID = t.ID,
                RealName = t.RealName,
                WorkNum = t.WorkNum,
                Phone = t.Phone
            }).ToList();

            //返回值记得加total前段要用
            return Json(new { total = total, rows = data }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑微信预约员工介绍
        /// </summary>
        /// <returns></returns>
        public ActionResult EditEmpInfo(int ID)
        {
            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            var user = usersService.GetEntity(ID);
            ViewBag.UserID = ID;
            ViewBag.Description = user.Description;
            return View();
        }

        //保存员工介绍信息
        [ValidateInput(false)]
        public JsonResult SaveDescription(int userID, string description)
        {
            bool result = false;
            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            var user = usersService.GetEntity(userID);
            if (user.ShopsID == CurrentInfo.CurrentShop.ID)
            {
                user.Description = description;
                user.WeiXinVisible = true;
                result = usersService.UpdateEntity(user);
            }

            return Json(new Result() { success = result, msg = "" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RemoveUserBindWeiXin(int userID)
        {
            try
            {
                var userModel = ServiceHelper.GetUsersService.GetEntity(userID);
                if (userModel.DefaultStoreID == CurrentInfo.CurrentStore.ID)
                {
                    userModel.WeiXinOpenID = null;
                    userModel.WeiXinOpenName = null;
                    ServiceHelper.GetUsersService.UpdateEntity(userModel);
                    return Json(new Result(true, "解绑成功"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Error403", "Home", new { area = "Admin" });
                }
            }
            catch (Exception ex)
            {
                return Json(new Result(false, ex.Message), JsonRequestBehavior.AllowGet);
            }

        }

    }
}