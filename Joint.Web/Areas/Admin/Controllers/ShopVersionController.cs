using Joint.Common;
using Joint.DLLFactory;
using Joint.Entity;
using Joint.IService;
using Joint.Web.Areas.Admin.Models;
using Joint.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Joint.Web.Areas.Admin.Controllers
{
    public class ShopVersionController : BaseAdminController
    {
        // GET: Admin/ShopVersion
        public ActionResult Index(OneKeySearchModel searchModel)
        {
            //如果不是管理员，拒绝访问
            if (!CurrentInfo.IsAdministrator)
            {
                return RedirectToAction("Error403", "Home", new { area = "Admin" });
            }

            Expression<Func<ShopVersion, Boolean>> lbdWhere = null;
            if (searchModel.SearchStr != null)
            {
                searchModel.SearchStr = searchModel.SearchStr.Trim();
                if (!string.IsNullOrWhiteSpace(searchModel.SearchStr))
                {
                    lbdWhere = t => t.Name.Contains(searchModel.SearchStr);
                }
            }


            IShopVersionService shopVersionService = ServiceFactory.Create<IShopVersionService>();
            var shopVersions = shopVersionService.GetEntitiesByPage(searchModel.PageIndex, 20, lbdWhere, true, t => t.Short);
            ViewBag.SearchModel = searchModel;
            return View(shopVersions);
        }


        public JsonResult GetShopVersions()
        {
            IShopVersionService shopVersionService = ServiceFactory.Create<IShopVersionService>();

            var data = shopVersionService.GetEntities(t => t.Disabled == false).OrderBy(t => t.Short).Select(t => new
            {
                ID = t.ID,
                Name = t.Name
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AddShopVersion(ShopVersion shopVersionModel)
        {
            IShopVersionService shopVersionService = ServiceFactory.Create<IShopVersionService>();
            bool flage = shopVersionService.Exists(t => t.Name == shopVersionModel.Name);
            if (flage)
            {
                return Json(new Result(false, "数据库已经存在同名角色"), JsonRequestBehavior.AllowGet);
            }

            shopVersionModel.CreateUserID = CurrentInfo.CurrentUser.ID;
            shopVersionModel.CreateTime = DateTime.Now;
            var data = shopVersionService.AddEntity(shopVersionModel);

            return Json(new Result(data != null, ResultType.Add), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateShopVersion(ShopVersion shopVersion)
        {
            IShopVersionService shopVersionService = ServiceFactory.Create<IShopVersionService>();
            bool flage = shopVersionService.Exists(t => t.ID != shopVersion.ID && t.Name == shopVersion.Name);
            if (flage)
            {
                return Json(new Result(false, "数据库已经存在同名角色"), JsonRequestBehavior.AllowGet);
            }
            var dbData = shopVersionService.GetEntity(shopVersion.ID, false);

            shopVersion.CreateUserID = dbData.CreateUserID;
            shopVersion.CreateTime = dbData.CreateTime;

            flage = shopVersionService.UpdateEntity(shopVersion);
            return Json(new Result(flage, ResultType.Update), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReDeleteShopVersion(int ID)
        {
            IShopVersionService shopVersionService = ServiceFactory.Create<IShopVersionService>();
            var data = shopVersionService.GetEntity(ID);
            data.Disabled = false;
            bool flage = shopVersionService.UpdateEntity(data);
            return Json(new Result(flage, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteShopVersion(int ID)
        {
            IShopVersionService shopVersionService = ServiceFactory.Create<IShopVersionService>();
            var data = shopVersionService.GetEntity(ID);
            data.Disabled = true;
            bool flage = shopVersionService.UpdateEntity(data);
            return Json(new Result(flage, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteShopVersions(string IDs)
        {
            List<int> idArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();

            IShopVersionService shopVersionService = ServiceFactory.Create<IShopVersionService>();
            var data = shopVersionService.GetEntities(idArr);
            bool success = true;
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var item in data)
                {
                    item.Disabled = true;
                    bool flage = shopVersionService.UpdateEntity(item);
                    if (flage == false)
                    {
                        break;
                    }
                }
                scope.Complete();
            }

            return Json(new Result(success, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShopVersionByID(int ID)
        {
            IShopVersionService shopVersionService = ServiceFactory.Create<IShopVersionService>();
            var data = shopVersionService.GetEntity(ID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}