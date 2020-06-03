using Joint.DLLFactory;
using Joint.IService;
using Joint.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Joint.Entity;
using Joint.Common;
using System.Globalization;
using System.Transactions;
using System.Linq.Expressions;
using Joint.IRepository;
using Joint.Repository;

namespace Joint.Web.Areas.Admin.Controllers
{
    public class StoresController : BaseAdminController
    {
        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// 获取商家下的门店
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="username"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public ActionResult GetShopStores(int pageSize, string username, int pageNumber)
        {
            SearchModel<StoresParams> searchModel = new SearchModel<StoresParams>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                Model = new StoresParams() { StoreName = username }

            };

            //IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            //var shop = shopsService.GetFirstOrDefault(t => t.AdminUserID == CurrentInfo.CurrentUser.ID);


            DynamicLambda<Stores> bulider = new DynamicLambda<Stores>();
            Expression<Func<Stores, Boolean>> expr = null;

            //如果是店铺管理员
            if (CurrentInfo.IsShopAdmin)
            {
                Expression<Func<Stores, Boolean>> tmp = t => t.ShopId == CurrentInfo.CurrentUser.ShopsID && t.Disabled != true;
                expr = bulider.BuildQueryAnd(expr, tmp);
            }
            else
            {
                Expression<Func<Stores, Boolean>> tmp = t => t.ID == CurrentInfo.CurrentStore.ID;
                expr = bulider.BuildQueryAnd(expr, tmp);
            }

            ////如果是商家本人的账号，则显示商家自己的门店
            //if (shop != null)
            //{
            //    Expression<Func<Stores, Boolean>> tmp = t => t.ShopId == shop.ID;
            //    expr = bulider.BuildQueryAnd(expr, tmp);
            //}

            if (!string.IsNullOrWhiteSpace(username))
            {
                username = username.Trim();
                Expression<Func<Stores, Boolean>> tmp = t => t.StoreName.Contains(username);
                expr = bulider.BuildQueryAnd(expr, tmp);
            }

            IStoresService storesService = ServiceFactory.Create<IStoresService>();

            int total;
            var data = storesService.GetEntitiesByPage(searchModel.PageIndex, searchModel.PageSize, out total, expr, false, t => t.ID).Select(t => new
            {
                ID = t.ID,
                ShopsName = t.Shops.ShopName,
                StoreName = t.StoreName,
                Adress = t.Adress,
                Phone = t.Phone,
                IsShowWeiXin = t.IsShowWeiXin == true ? "显示" : "不显示",
                IsMainStore = t.IsMainStore,
                Disabled = t.Disabled,
                AdminUserID = t.AdminUserID,
                ///  AdminName = t.AdminUserID==null?"0":t.AdminUserID.ToString() //t.Users1 != null ? t.Users1.RealName : ""
            }).ToList();

            return Json(new { total = total, rows = data }, JsonRequestBehavior.AllowGet);
        }

        public string GetRealName(int? adminUserID)
        {
            return ServiceHelper.GetUsersService.GetEntity((int)adminUserID).RealName;
        }

        /// <summary>
        /// 系统管理员使用的方法
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="username"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public ActionResult GetStores(int pageSize, string username, int pageNumber, int? shopID)
        {

            //SearchModel<StoresParams> searchModel = new SearchModel<StoresParams>
            //{
            //    PageIndex = pageNumber,
            //    PageSize = pageSize,
            //    Model = new StoresParams() { StoreName = username }

            //};

            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            //var shop = shopsService.GetFirstOrDefault(t => t.AdminUserID == CurrentInfo.CurrentUser.ID);


            DynamicLambda<Stores> bulider = new DynamicLambda<Stores>();
            Expression<Func<Stores, Boolean>> expr = null;

            ////如果是商家本人的账号，则显示商家自己的门店
            //if (shop != null)
            //{
            //    Expression<Func<Stores, Boolean>> tmp = t => t.ShopId == shop.ID;
            //    expr = bulider.BuildQueryAnd(expr, tmp);
            //}


            if (shopID != null)
            {
                if (shopID > 0)
                {
                    Expression<Func<Stores, Boolean>> tmp = t => t.ShopId == shopID;
                    expr = bulider.BuildQueryAnd(expr, tmp);
                }
                else
                {
                    Expression<Func<Stores, Boolean>> tmp = t => t.VirtualShopsID == shopID;
                    expr = bulider.BuildQueryAnd(expr, tmp);
                }
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                username = username.Trim();
                Expression<Func<Stores, Boolean>> tmp = t => t.StoreName.Contains(username) ||
                    t.Shops.ShopName.Contains(username);
                expr = bulider.BuildQueryAnd(expr, tmp);
            }

            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            int total;
            var data = storesService.GetEntitiesByPage(pageNumber, pageSize, out total, expr, false, t => t.ID).Select(t => new
            {
                ID = t.ID,
                ShopsName = t.Shops.ShopName,
                ShopsID = t.ShopId,
                StoreName = t.StoreName,
                Adress = t.Adress,
                Phone = t.Phone,
                IsShowWeiXin = t.IsShowWeiXin == true ? "显示" : "不显示",
                IsMainStore = t.IsMainStore,
                Disabled = t.Disabled,
                VirtualShopsID = t.VirtualShopsID
            }).ToList();


            return Json(new { total = total, rows = data }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 门店商家管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CustStoreManage()
        {
            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            IStoresService storesService = ServiceFactory.Create<IStoresService>();

            //判断当前用户是否是这个商家的管理员，如果不是则拒绝访问
            //var shop = shopsService.GetFirstOrDefault(t => t);
            //bool isIntention = CurrentInfo.CurrentUser.IsIntention == null ? false : Convert.ToBoolean(CurrentInfo.CurrentUser.IsIntention);
            //if (!CurrentInfo.IsShopAdmin && !isIntention)
            //{
            //    return RedirectToAction("Error403", "Home", new { area = "Admin" });
            //}

            //判断是否已经设置过主店，如果设置了，则不允许用户再次修改
            var hasMainStore = storesService.Exists(t => t.ShopId == CurrentInfo.CurrentUser.ShopsID && t.IsMainStore == true);
            ViewBag.HasMainStore = hasMainStore;

            return View();
        }


        public ActionResult DisableStores(string SID)
        {
            string[] strIds = SID.Split(',');
            List<int> list = new List<int>();
            foreach (string id in strIds)
            {
                list.Add(int.Parse(id));
            }

            var flag = false;
            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var item in list)
                {
                    var data = storesService.GetEntity(item);
                    data.Disabled = true;
                    storesService.UpdateEntity(data);
                }
                scope.Complete();
                flag = true;
            }
            return Json(new Result(flag, ResultType.Other), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReDisableStores(string SID)
        {
            string[] strIds = SID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<int> list = new List<int>();
            foreach (string id in strIds)
            {
                list.Add(int.Parse(id));
            }

            var flag = false;
            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var item in list)
                {
                    var data = storesService.GetEntity(item);
                    data.Disabled = false;
                    storesService.UpdateEntity(data);
                }
                scope.Complete();
                flag = true;
            }

            return Json(new Result(flag, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStoresByID(int ID)
        {
            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            Stores stores = storesService.GetEntity(ID);
            var jsonData = new
            {
                ID = stores.ID,
                ShopId = stores.Shops.ID,
                ShopName = stores.Shops.ShopName,
                StoreName = stores.StoreName,
                Phone = stores.Phone,
                Adress = stores.Adress,
                BankName = stores.BankName,
                BankCard = stores.BankCard,
                IsShare = stores.IsShare,
                IsShowWeiXin = stores.IsShowWeiXin,
                IsMainStore = stores.IsMainStore,
                Disabled = stores.Disabled,
                LatitudeY = stores.LatitudeY == null ? "" : stores.LatitudeY,
                LongitudeX = stores.LongitudeX == null ? "" : stores.LongitudeX,
                StoresImage = stores.StoresImage,
                ReceiptLogo = stores.ReceiptLogo,
                PrintRemark = stores.PrintRemark
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditStores(Stores storesModel)
        {
            //var stores = strStores.FromJson<Stores>();

            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            var dbData = storesService.GetEntity(storesModel.ID, false);
            bool flage = storesService.Exists(t => t.ID != storesModel.ID && t.ShopId == storesModel.ShopId && t.StoreName == storesModel.StoreName);
            if (flage)
            {
                return Json(new Result(false, "已经存在同名门店请修改"), JsonRequestBehavior.AllowGet);
            }

            //如果用户有传文件，并且文件和数据库中保存的不一样，则需要保存，如果和数据库中一致，无需保存
            if (!string.IsNullOrWhiteSpace(storesModel.StoresImage) && storesModel.StoresImage != dbData.StoresImage)
            {
                storesModel.StoresImage = FileHelper.Move(storesModel.StoresImage, "/Upload/Reality/" + CurrentInfo.CurrentStore.ID + "/storeImage/");
            }
            if (!string.IsNullOrWhiteSpace(storesModel.ReceiptLogo) && storesModel.ReceiptLogo != dbData.ReceiptLogo)
            {
                string newRoute = ServiceHelper.GetCommonService.ThumbnailImage(storesModel.ReceiptLogo, CurrentInfo.CurrentStore.ID, 128, 128, "CUT", "ReceiptLogo");
                storesModel.ReceiptLogo = newRoute;
            }

            if (storesModel.LongitudeX == "NaN")
            {
                storesModel.LongitudeX = "0";
            }
            if (storesModel.LatitudeY == "NaN")
            {
                storesModel.LatitudeY = "0";
            }
            dbData.StoreName = storesModel.StoreName;
            dbData.Phone = storesModel.Phone;
            dbData.Adress = storesModel.Adress;
            dbData.LatitudeY = storesModel.LatitudeY;
            dbData.LongitudeX = storesModel.LongitudeX;
            dbData.BankName = storesModel.BankName;
            dbData.BankCard = storesModel.BankCard;
            dbData.IsMainStore = storesModel.IsMainStore;
            dbData.IsShare = storesModel.IsShare;
            dbData.IsShowWeiXin = storesModel.IsShowWeiXin;
            dbData.StoresImage = storesModel.StoresImage;
            dbData.PrintRemark = storesModel.PrintRemark;
            dbData.Disabled = storesModel.Disabled;
            bool isSuccess = dbData.Update();

            return Json(new Result(isSuccess, ResultType.Update), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddStores(Stores storesModel)
        {
            //var stores = strStores.FromJson<Stores>();

            IStoresService storesService = ServiceFactory.Create<IStoresService>();

            bool flage = storesService.Exists(t => t.ShopId == storesModel.ShopId && t.StoreName == storesModel.StoreName);

            if (flage)
            {
                return Json(new Result(false, "已经存在同名门店请修改"), JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrWhiteSpace(storesModel.StoresImage))
            {
                storesModel.StoresImage = FileHelper.Move(storesModel.StoresImage, "/Upload/Reality/" + CurrentInfo.CurrentStore.ID + "/storeImage/");
            }
            if (!string.IsNullOrWhiteSpace(storesModel.ReceiptLogo))
            {
                string newRoute = ServiceHelper.GetCommonService.ThumbnailImage(storesModel.ReceiptLogo, CurrentInfo.CurrentStore.ID, 128, 128, "CUT", "ReceiptLogo");
                storesModel.ReceiptLogo = newRoute;
            }
            if (storesModel.LongitudeX == "NaN")
            {
                storesModel.LongitudeX = "0";
            }
            if (storesModel.LatitudeY == "NaN")
            {
                storesModel.LatitudeY = "0";
            }
            var data = storesService.AddEntity(storesModel);

            return Json(new Result(data != null, ResultType.Add), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 共享不可见门店
        /// </summary>
        /// <returns></returns>
        public ActionResult EditNoShareStore(int storeID)
        {
            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            int shopID = CurrentInfo.CurrentShop.ID;
            var storeIDList = storesService.GetEntities(t => t.ShopId == shopID).ToList();
            ViewBag.CurrentStoreID = CurrentInfo.CurrentStore.ID;
            return View(storeIDList);
        }

        //public JsonResult GetNoShareStoreID(int ID)
        //{
        //    IShareStroreSettingService shareStroreSettingService = ServiceFactory.Create<IShareStroreSettingService>();

        //    List<int> ids = new List<int>();
        //    string idStrs = "";//得到共享配置表

        //    var data = shareStroreSettingService.GetEntities(t => t.CurrentStoreID == ID).FirstOrDefault();
        //    if (data != null)
        //    {
        //        idStrs = data.NoShowStoreID;
        //    }

        //    List<string> listStr = idStrs.Split(',').ToList();
        //    foreach (var i in listStr)
        //    {
        //        ids.Add(TypeHelper.ObjectToInt(i));
        //    }

        //    return Json(ids, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult AddNoShareStore(int ID, string storeIDSs)
        //{
        //    IShareStroreSettingService shareStroreSettingService = ServiceFactory.Create<IShareStroreSettingService>();
        //    var data = shareStroreSettingService.GetEntities(t => t.CurrentStoreID == ID).FirstOrDefault();
        //    if (data != null)
        //    {
        //        data.NoShowStoreID = storeIDSs;
        //        bool IsSuccess = shareStroreSettingService.UpdateEntity(data);
        //        return Json(new Result() { success = IsSuccess, msg = "设置成功" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        data = new ShareStroreSetting();
        //        data.NoShowStoreID = storeIDSs;
        //        data.OperateUser = CurrentInfo.CurrentUser.ID;
        //        data.ShopsID = CurrentInfo.CurrentShop.ID;
        //        data.CurrentStoreID = ID;
        //        data.CreateTime = DateTime.Now;
        //        data = shareStroreSettingService.AddEntity(data);
        //        return Json(new Result() { success = data.ID != 0, msg = "设置成功" }, JsonRequestBehavior.AllowGet);

        //    }


        //}

        /// <summary>
        /// 编辑门店简介
        /// </summary>
        /// <returns></returns>
        public ActionResult EditProfile(int storeID)
        {
            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            var store = storesService.GetEntity(storeID);
            ViewBag.Profile = store.Profile;
            return View();
        }

        //保存门店简介
        [ValidateInput(false)]
        public JsonResult SaveProfile(int storeID, string profile)
        {
            bool result = false;
            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            var store = storesService.GetEntity(storeID);
            if (store.ShopId == CurrentInfo.CurrentShop.ID)
            {
                store.Profile = profile;
                result = storesService.UpdateEntity(store);
            }

            return Json(new Result() { success = result, msg = "" }, JsonRequestBehavior.AllowGet);
        }


        //public JsonResult GetVirtualShops(int shopID)
        //{
        //    var virtualShops = ServiceHelper.GetCommonService.GetVirtualShops(shopID);
        //    return Json(new { success = true, VirtualShops = virtualShops }, JsonRequestBehavior.AllowGet);
        //}

        //门店绑定公众号和解绑公众号操作
        public JsonResult ToggleVirtualShops(int storeID, int? virtualShopsID)
        {

            IStoresService storesService = ServiceFactory.Create<IStoresService>();

            bool result = false;
            var storeModel = storesService.GetEntity(storeID);
            if (storeModel != null)
            {
                if (virtualShopsID != null)
                {
                    storeModel.VirtualShopsID = virtualShopsID;
                }
                else
                {
                    storeModel.VirtualShopsID = null;
                }
                result = storesService.UpdateEntity(storeModel);
            }
            //设置成功
            return Json(new Result(result, result ? (virtualShopsID != null ? "绑定成功" : "解绑成功") : "操作失败"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUsers(int storeID)
        {
            var store = ServiceHelper.GetStoresService.GetEntity(storeID);
            if (store.ShopId == CurrentInfo.CurrentShop.ID)
            {


                Expression<Func<Users, Boolean>> lbdUsersWhere = null;
                lbdUsersWhere = t => t.DefaultStoreID == storeID && t.Disabled == false && t.IsIntention != true;
                IUsersService usersService = ServiceFactory.Create<IUsersService>();

                var users = usersService.GetEntities(lbdUsersWhere).ToList().Select(t => new
                {
                    ID = t.ID,
                    Name = t.RealName + (string.IsNullOrWhiteSpace(t.WorkNum) ? "" : "-工号:" + t.WorkNum)
                }).ToList();
                return Json(new { Users = users }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult SetManager(int storeID, int userID)
        {
            bool result = false;
            var store = ServiceHelper.GetStoresService.GetEntity(storeID);
            if (store.ShopId == CurrentInfo.CurrentShop.ID)
            {
                store.AdminUserID = userID;
                result = ServiceHelper.GetStoresService.UpdateEntity(store);
            }
            return Json(new Result(result, "设置成功"), JsonRequestBehavior.AllowGet);
        }




    }
}