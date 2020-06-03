using Joint.Entity;
using Joint.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Joint.IService;
using Joint.DLLFactory;
using System.Linq.Expressions;
using Joint.Web.Areas.Admin.Models;
using System.Transactions;
using Joint.Common;

namespace Joint.Web.Areas.Admin.Controllers
{
    public class RoleController : BaseAdminController
    {
        // GET: Admin/Role
        public ActionResult Index(RoleSearchModel searchModel)
        {
            var expr = BuildSearchCriteria(searchModel);
            IRoleService roleService = ServiceFactory.Create<IRoleService>();
            var roles = roleService.GetEntitiesByPage(searchModel.PageIndex, 10, expr, false, t => t.ID);

            ViewBag.SearchModel = searchModel;

            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            List<Stores> allStores = storesService.GetEntities(t => t.ShopId == CurrentInfo.CurrentShop.ID && t.Disabled == false).ToList();// CurrentInfo.CurrentShop.ID 
            SelectList allStoresSelect = new SelectList(allStores, "ID", "StoreName", CurrentInfo.CurrentStore.ID);// CurrentInfo.CurrentStore.ID
            ViewData["allStoresSelect"] = allStoresSelect;
            //只有管理员才能给所有店铺添加人员
            ViewData["showStores"] = CurrentInfo.IsShopAdmin;

            return View(roles);
        }

        public ActionResult CusRoleManage(RoleSearchModel searchModel)
        {
            var expr = BuildSearchCriteria(searchModel);
            IRoleService roleService = ServiceFactory.Create<IRoleService>();
            var roles = roleService.GetEntitiesByPage(searchModel.PageIndex, 10, expr, false, t => t.ID);

            ViewBag.SearchModel = searchModel;

            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            List<Stores> allStores = storesService.GetEntities(t => t.ShopId == CurrentInfo.CurrentShop.ID && t.Disabled == false).ToList();
            SelectList allStoresSelect = new SelectList(allStores, "ID", "StoreName", CurrentInfo.CurrentStore.ID);
            ViewData["allStoresSelect"] = allStoresSelect;
            ViewData["showStores"] = CurrentInfo.IsShopAdmin;
            return View(roles);
        }

        public JsonResult AddRole(Role roleModel)
        {
            IRoleService roleService = ServiceFactory.Create<IRoleService>();
            bool flage = roleService.Exists(t => t.StoreID == CurrentInfo.CurrentStore.ID && t.Name == roleModel.Name);
            if (flage)
            {
                return Json(new Result(false, "数据库已经存在同名角色"), JsonRequestBehavior.AllowGet);
            }

            roleModel.ShopsID = CurrentInfo.CurrentShop.ID;
            //roleModel.StoreID = CurrentInfo.CurrentStore.ID;
            roleModel.CreateUserID = CurrentInfo.CurrentUser.ID;
            roleModel.CreateTime = DateTime.Now;
            var data = roleService.AddEntity(roleModel);

            return Json(new Result(data != null, ResultType.Add), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRole(Role role)
        {
            IRoleService roleService = ServiceFactory.Create<IRoleService>();
            bool flage = roleService.Exists(t => t.ID != role.ID && t.StoreID == CurrentInfo.CurrentStore.ID && t.Name == role.Name);
            if (flage)
            {
                return Json(new Result(false, "数据库已经存在同名角色"), JsonRequestBehavior.AllowGet);
            }
            var dbData = roleService.GetEntity(role.ID, false);
            role.StoreID = dbData.StoreID;
            role.ShopsID = dbData.ShopsID;
            role.CreateUserID = dbData.CreateUserID;
            role.CreateTime = dbData.CreateTime;
            flage = roleService.UpdateEntity(role);
            return Json(new Result(flage, ResultType.Update), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReDeleteRole(int ID)
        {
            IRoleService roleService = ServiceFactory.Create<IRoleService>();
            var data = roleService.GetEntity(ID);
            data.Disabled = false;
            bool flage = roleService.UpdateEntity(data);
            return Json(new Result(flage, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteRole(int ID)
        {
            IRoleService roleService = ServiceFactory.Create<IRoleService>();
            var data = roleService.GetEntity(ID);
            data.Disabled = true;
            bool flage = roleService.UpdateEntity(data);
            return Json(new Result(flage, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteRoles(string IDs)
        {
            List<int> idArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();

            IRoleService roleService = ServiceFactory.Create<IRoleService>();
            var data = roleService.GetEntities(idArr);
            bool success = true;
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var item in data)
                {
                    item.Disabled = true;
                    bool flage = roleService.UpdateEntity(item);
                    if (flage == false)
                    {
                        break;
                    }
                }
                scope.Complete();
            }

            return Json(new Result(success, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRoleByID(int ID)
        {
            IRoleService roleService = ServiceFactory.Create<IRoleService>();
            var data = roleService.GetEntity(ID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 构建查询表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<Role, Boolean>> BuildSearchCriteria(RoleSearchModel searchModel)
        {
            DynamicLambda<Role> bulider = new DynamicLambda<Role>();
            Expression<Func<Role, Boolean>> expr = null;

            if (CurrentInfo.IsShopAdmin)
            {
                Expression<Func<Role, Boolean>> tmpStoreID = t => t.ShopsID == CurrentInfo.CurrentShop.ID;
                expr = bulider.BuildQueryAnd(expr, tmpStoreID);
            }
            else
            {
                Expression<Func<Role, Boolean>> tmpStoreID = t => t.StoreID == CurrentInfo.CurrentStore.ID;
                expr = bulider.BuildQueryAnd(expr, tmpStoreID);
            }

            if (!string.IsNullOrWhiteSpace(searchModel.RoleName))
            {
                Expression<Func<Role, Boolean>> tmp = t => t.Name.Contains(searchModel.RoleName);
                expr = bulider.BuildQueryAnd(expr, tmp);
            }

            if (!string.IsNullOrWhiteSpace(searchModel.Disabled))
            {
                var flage = searchModel.Disabled == "1" ? true : false;
                Expression<Func<Role, Boolean>> tmp = t => t.Disabled == flage;
                expr = bulider.BuildQueryAnd(expr, tmp);
            }

            if (!string.IsNullOrWhiteSpace(searchModel.startDate))
            {
                try
                {
                    DateTime dt = Convert.ToDateTime(searchModel.startDate);
                    Expression<Func<Role, Boolean>> tmp = t => t.CreateTime >= dt;
                    expr = bulider.BuildQueryAnd(expr, tmp);
                }
                catch
                {

                }
            }

            if (!string.IsNullOrWhiteSpace(searchModel.endDate))
            {
                try
                {
                    DateTime dt = Convert.ToDateTime(searchModel.endDate).AddDays(1);
                    Expression<Func<Role, Boolean>> tmp = t => t.CreateTime < dt;
                    expr = bulider.BuildQueryAnd(expr, tmp);
                }
                catch
                {

                }
            }

            //Expression<Func<Role, Boolean>> tmpSolid = t => t.IsDeleted == false;
            //expr = bulider.BuildQueryAnd(expr, tmpSolid);
            //如果不是管理员账号则 把“店长”这个角色隐藏掉，防止其他客户修改
            if (!CurrentInfo.IsAdministrator)
            {
                Expression<Func<Role, Boolean>> tmpAdmin = t => t.ID != 42;
                expr = bulider.BuildQueryAnd(expr, tmpAdmin);
            }

            return expr;
        }

        public JsonResult SearchRoleName(string Query)
        {
            Expression<Func<Role, Boolean>> lbdWhere = null;
            if (Query != null)
            {
                Query = Query.Trim();
                if (!string.IsNullOrWhiteSpace(Query))
                {
                    lbdWhere = t => t.StoreID == CurrentInfo.CurrentStore.ID && t.Name.Contains(Query);
                }
            }
            else
            {
                lbdWhere = t => t.StoreID == CurrentInfo.CurrentStore.ID;
            }


            IRoleService roleService = ServiceFactory.Create<IRoleService>();
            //以id倒叙的顺序，取10条信息
            var storeAllRole = roleService.GetTopEntities(10, t => t.ID, lbdWhere, false).Select(t => new
            {
                Name = t.Name
            });
            return Json(storeAllRole, JsonRequestBehavior.AllowGet);
        }
    }
}