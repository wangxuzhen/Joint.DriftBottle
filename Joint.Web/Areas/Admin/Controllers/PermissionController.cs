using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Joint.IService;
using Joint.DLLFactory;
using Joint.Web.Framework;
using Joint.Entity;
using System.Linq.Expressions;
using Joint.Web.Areas.Admin.Models;
using System.Transactions;
using Joint.Common;

namespace Joint.Web.Areas.Admin.Controllers
{
    public class PermissionController : BaseAdminController
    {
        // GET: Admin/Permission
        public ActionResult UserPermission(OneKeySearchModel searchModel)
        {
            Expression<Func<Users, Boolean>> lbdWhere = null;
            if (searchModel.SearchStr != null)
            {
                searchModel.SearchStr = searchModel.SearchStr.Trim();
                if (!string.IsNullOrWhiteSpace(searchModel.SearchStr))
                {
                    lbdWhere = t => t.UserName.Contains(searchModel.SearchStr)
                       || t.Phone.Contains(searchModel.SearchStr)
                       || t.TEL.Contains(searchModel.SearchStr)
                       || t.Idcard.Contains(searchModel.SearchStr)
                       || t.WorkNum.Contains(searchModel.SearchStr)
                       || t.RealName.Contains(searchModel.SearchStr)
                       || t.UserName.Contains(searchModel.SearchStr)
                       || t.Stores.Shops.ShopName.Contains(searchModel.SearchStr)
                       || t.Stores.StoreName.Contains(searchModel.SearchStr);
                }
            }


            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            var users = usersService.GetEntitiesByPage(searchModel.PageIndex, 10, lbdWhere, false, t => t.ID);
            //IWorkTypeService workTypeService = ServiceFactory.Create<IWorkTypeService>();

            //转换数据模型
            var listData = users.Models.Select(t => new UserListModel
            {
                ID = t.ID,
                WorkNum = t.WorkNum,
                RealName = t.RealName,
                Phone = t.Phone,
                RoleName = ServiceHelper.GetUsersService.GetUserAllRole(t.ID),//t.WorkTypeID == null ? "" : workTypeService.GetEntity(Convert.ToInt32(t.WorkTypeID)).WorkTypeName,
                UserName = t.UserName,
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

        public ActionResult ShopPermission(OneKeySearchModel searchModel)
        {
            //IUsersService usersService = ServiceFactory.Create<IUsersService>();
            Expression<Func<Shops, Boolean>> lbdWhere = null;
            if (searchModel.SearchStr != null)
            {
                searchModel.SearchStr = searchModel.SearchStr.Trim();
                if (!string.IsNullOrWhiteSpace(searchModel.SearchStr))
                {
                    lbdWhere = t => t.ShopName.Contains(searchModel.SearchStr);
                }
            }
            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            var shops = shopsService.GetEntitiesByPage(searchModel.PageIndex, 10, lbdWhere, false, t => t.ID);
            //转换数据模型
            var listData = shops.Models.Select(t => new ShopPermissionListModel
            {
                ID = t.ID,
                ShopName = t.ShopName,
                RealName = ServiceHelper.GetUsersService.GetEntity(t.AdminUserID).RealName,// t.Users.RealName,
                Phone = ServiceHelper.GetUsersService.GetEntity(t.AdminUserID).Phone
            });

            var listShop = new PageModel<ShopPermissionListModel>
            {
                Models = listData.ToList(),
                pagingInfo = shops.pagingInfo
            };

            ViewBag.SearchModel = searchModel;

            return View(listShop);
        }

        public ActionResult StorePermission(OneKeySearchModel searchModel)
        {
            Expression<Func<Stores, Boolean>> lbdWhere = null;
            if (searchModel.SearchStr != null)
            {
                searchModel.SearchStr = searchModel.SearchStr.Trim();
                if (!string.IsNullOrWhiteSpace(searchModel.SearchStr))
                {
                    lbdWhere = t => t.StoreName.Contains(searchModel.SearchStr)
                       || t.Phone.Contains(searchModel.SearchStr)
                       || t.Adress.Contains(searchModel.SearchStr)
                       || t.Shops.ShopName.Contains(searchModel.SearchStr);
                }
            }


            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            var stores = storesService.GetEntitiesByPage(searchModel.PageIndex, 10, lbdWhere, false, t => t.ID);
            //转换数据模型
            var listData = stores.Models.Select(t => new StorePermissionListModel
            {
                ID = t.ID,
                ShopName = t.Shops.ShopName,
                StoreName = t.StoreName,
                Phone = t.Phone,
                Adress = t.Adress,
                IsShowWeiXin = t.IsShowWeiXin == true ? "显示" : "不显示",
                IsMainStore = t.IsMainStore == true ? "是" : "否"//是否总店 
            });

            var listStore = new PageModel<StorePermissionListModel>
            {
                Models = listData.ToList(),
                pagingInfo = stores.pagingInfo
            };

            ViewBag.SearchModel = searchModel;

            return View(listStore);
        }

        public ActionResult RolePermission(RoleSearchModel searchModel)
        {
            var expr = BuildSearchCriteria(searchModel);
            IRoleService roleService = ServiceFactory.Create<IRoleService>();
            var roles = roleService.GetEntitiesByPage(searchModel.PageIndex, 5, expr, false, t => t.ID);

            ViewBag.SearchModel = searchModel;
            return View(roles);
        }


        /// <summary>
        /// 构建查询表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<Role, Boolean>> BuildSearchCriteria(RoleSearchModel searchModel)
        {
            DynamicLambda<Role> bulider = new DynamicLambda<Role>();
            Expression<Func<Role, Boolean>> expr = null;

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

            return expr;
        }

        public ActionResult PermissionMenu()
        {
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            var munes = moduleService.GetEntities(t => t.Disabled == false);

            return View(munes);
        }

        /// <summary>
        /// 商家给门店分配菜单的方法
        /// </summary>
        /// <returns></returns>
        public ActionResult ShopPermissionMenu(int? storeID)
        {
            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            Shops shop;

            //必须是管理员，才可以分配指定门店的权限
            if (storeID != null)
            {
                if (CurrentInfo.IsAdministrator)
                {
                    var singleStore = ServiceHelper.GetStoresService.GetEntity((int)storeID);
                    shop = shopsService.GetFirstOrDefault(t => t.ID == singleStore.ShopId);
                }
                else
                {
                    return RedirectToAction("Error403", "Home", new { area = "Admin" });
                }
            }
            else
            {
                shop = shopsService.GetFirstOrDefault(t => t.AdminUserID == CurrentInfo.CurrentUser.ID);
            }

            if (shop != null)
            {
                IRelationShopsModuleService relationShopsModuleService = ServiceFactory.Create<IRelationShopsModuleService>();
                //获取商家拥有的所有菜单
                var munes = relationShopsModuleService.GetEntities(t => t.ShopID == shop.ID).Select(t => t.Module);
                return View(munes);
            }
            else
            {
                return RedirectToAction("Error403", "Home", new { area = "Admin" });
            }
        }

        public ActionResult StorePermissionMenu()
        {
            IStoresService storesService = ServiceFactory.Create<IStoresService>();

            var store = storesService.GetFirstOrDefault(t => t.ID == CurrentInfo.CurrentStore.ID);
            if (store != null)
            {
                IRelationStoresModuleService relationStoresModuleService = ServiceFactory.Create<IRelationStoresModuleService>();
                //获取门店拥有的所有菜单
                var munes = relationStoresModuleService.GetEntities(t => t.StoresID == store.ID).Select(t => t.Module);
                return View(munes);
            }
            else
            {
                return RedirectToAction("Error403", "Home", new { area = "Admin" });
            }
        }

        public JsonResult AddRelationRoleModule(string IDs, string ModuleIDs)
        {
            if (string.IsNullOrWhiteSpace(IDs) || string.IsNullOrWhiteSpace(ModuleIDs))
            {
                return Json(new Result(false, "参数错误"), JsonRequestBehavior.AllowGet);
            }

            List<int> roleIDArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
            List<int> moduleArr = ModuleIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();

            //把所有菜单的父菜单取出来，但是只能保证三级菜单，多级有可能有bug
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            List<Module> allListModule = new List<Module>();
            allListModule.AddRange(moduleService.GetEntities(t => moduleArr.Contains(t.ID)).ToList());
            //foreach (var item in allListModule)
            //{
            //    Module parentModule = moduleService.GetFirstOrDefault(t => t.ID == item.ParentID);
            //    if (parentModule != null)
            //    {
            //        if (allListModule.Where(t => t.ID == parentModule.ID).Count() == 0)
            //        {
            //            allListModule.Add(parentModule);
            //        }
            //    }
            //}

            for (int i = 0; i < allListModule.Count(); i++)
            {
                int parentID = allListModule[i].ParentID;
                Module parentModule = moduleService.GetFirstOrDefault(t => t.ID == parentID);
                if (parentModule != null)
                {
                    if (allListModule.Where(t => t.ID == parentModule.ID).Count() == 0)
                    {
                        allListModule.Add(parentModule);
                    }
                }

            }

            //从新生成菜单ID
            moduleArr = allListModule.Select(t => t.ID).ToList();

            IRelationRoleModuleService relationRoleModuleService = ServiceFactory.Create<IRelationRoleModuleService>();
            List<RelationRoleModule> listRelationRolesModule = new List<RelationRoleModule>();

            int addCount = 0;
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var roleID in roleIDArr)
                {
                    //删除当前人员的所有的权限，然后在添加新的权限
                    var roleModule = relationRoleModuleService.GetEntities(t => t.RoleID == roleID);
                    relationRoleModuleService.DeleteEntities(roleModule.ToList());
                    foreach (var moduleID in moduleArr)
                    {
                        RelationRoleModule model = new RelationRoleModule();
                        model.RoleID = roleID;
                        model.ModuleID = moduleID;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        listRelationRolesModule.Add(model);
                    }
                }
                addCount = relationRoleModuleService.AddEntities(listRelationRolesModule).Count();
                scope.Complete();
            }

            return Json(new Result(addCount > 0, "成功分配权限"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddRelationShopModule(string IDs, string ModuleIDs)
        {
            if (string.IsNullOrWhiteSpace(IDs) || string.IsNullOrWhiteSpace(ModuleIDs))
            {
                return Json(new Result(false, "参数错误"), JsonRequestBehavior.AllowGet);
            }

            List<int> shopIDArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
            List<int> moduleArr = ModuleIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();

            //把所有菜单的父菜单取出来，但是只能保证三级菜单，多级有可能有bug
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            List<Module> allListModule = new List<Module>();
            allListModule.AddRange(moduleService.GetEntities(t => moduleArr.Contains(t.ID)).ToList());

            //复制一份副本，用于循环使用
            Module[] tempAllListModule = new Module[allListModule.Count];
            allListModule.CopyTo(tempAllListModule);

            foreach (var item in tempAllListModule)
            {
                Module parentModule = moduleService.GetFirstOrDefault(t => t.ID == item.ParentID);
                if (parentModule != null)
                {
                    if (allListModule.Where(t => t.ID == parentModule.ID).Count() == 0)
                    {
                        allListModule.Add(parentModule);
                    }
                }
            }
            //从新生成菜单ID
            moduleArr = allListModule.Select(t => t.ID).ToList();

            IRelationShopsModuleService relationShopsModuleService = ServiceFactory.Create<IRelationShopsModuleService>();
            List<RelationShopsModule> listRelationShopsModule = new List<RelationShopsModule>();

            int addCount = 0;
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var shopID in shopIDArr)
                {
                    //删除当前人员的所有的权限，然后在添加新的权限
                    var roleModule = relationShopsModuleService.GetEntities(t => t.ShopID == shopID);
                    relationShopsModuleService.DeleteEntities(roleModule.ToList());
                    foreach (var moduleID in moduleArr)
                    {
                        RelationShopsModule model = new RelationShopsModule();
                        model.ShopID = shopID;
                        model.ModuleID = moduleID;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        listRelationShopsModule.Add(model);
                    }
                }
                addCount = relationShopsModuleService.AddEntities(listRelationShopsModule).Count();
                scope.Complete();
            }

            return Json(new Result(addCount > 0, "成功分配权限"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult AddRelationShopVersionModule(string IDs, string ModuleIDs)
        {
            if (string.IsNullOrWhiteSpace(IDs) || string.IsNullOrWhiteSpace(ModuleIDs))
            {
                return Json(new Result(false, "参数错误"), JsonRequestBehavior.AllowGet);
            }

            List<int> shopVersionIDArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
            List<int> moduleArr = ModuleIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();

            //把所有菜单的父菜单取出来，但是只能保证三级菜单，多级有可能有bug
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            List<Module> allListModule = new List<Module>();

            allListModule.AddRange(moduleService.GetEntities(t => moduleArr.Contains(t.ID)).ToList());

            //复制一份副本，用于循环使用
            Module[] tempAllListModule = new Module[allListModule.Count];
            allListModule.CopyTo(tempAllListModule);

            foreach (var item in tempAllListModule)
            {
                Module parentModule = moduleService.GetFirstOrDefault(t => t.ID == item.ParentID);
                if (parentModule != null)
                {
                    if (allListModule.Where(t => t.ID == parentModule.ID).Count() == 0)
                    {
                        allListModule.Add(parentModule);
                    }
                }
            }
            //从新生成菜单ID
            moduleArr = allListModule.Select(t => t.ID).ToList();

            IRelationShopVersionModuleService relationShopVersionModuleService = ServiceFactory.Create<IRelationShopVersionModuleService>();
            List<RelationShopVersionModule> listRelationShopVersionModule = new List<RelationShopVersionModule>();

            int addCount = 0;
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var shopVersionID in shopVersionIDArr)
                {
                    //删除当版本的所有菜单，然后在添加新的菜单
                    var roleModule = relationShopVersionModuleService.GetEntities(t => t.ShopVersionID == shopVersionID);
                    relationShopVersionModuleService.DeleteEntities(roleModule.ToList());
                    foreach (var moduleID in moduleArr)
                    {
                        RelationShopVersionModule model = new RelationShopVersionModule();
                        model.ShopVersionID = shopVersionID;
                        model.ModuleID = moduleID;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        listRelationShopVersionModule.Add(model);
                    }
                }
                addCount = relationShopVersionModuleService.AddEntities(listRelationShopVersionModule).Count();
                scope.Complete();
            }

            return Json(new Result(addCount > 0, "成功分配权限"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult AddRelationUsersModule(string IDs, string ModuleIDs)
        {
            if (string.IsNullOrWhiteSpace(IDs) || string.IsNullOrWhiteSpace(ModuleIDs))
            {
                return Json(new Result(false, "参数错误"), JsonRequestBehavior.AllowGet);
            }

            List<int> userIDArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
            List<int> moduleArr = ModuleIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();

            //把所有菜单的父菜单取出来，但是只能保证三级菜单，多级有可能有bug
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            List<Module> allListModule = new List<Module>();
            allListModule.AddRange(moduleService.GetEntities(t => moduleArr.Contains(t.ID)).ToList());

            //复制一份副本，用于循环使用
            Module[] tempAllListModule = new Module[allListModule.Count];
            allListModule.CopyTo(tempAllListModule);

            foreach (var item in tempAllListModule)
            {
                Module parentModule = moduleService.GetFirstOrDefault(t => t.ID == item.ParentID);
                if (parentModule != null)
                {
                    if (allListModule.Where(t => t.ID == parentModule.ID).Count() == 0)
                    {
                        allListModule.Add(parentModule);
                    }
                }
            }
            //从新生成菜单ID
            moduleArr = allListModule.Select(t => t.ID).ToList();

            IRelationUsersModuleService relationUsersModuleService = ServiceFactory.Create<IRelationUsersModuleService>();

            List<RelationUsersModule> listRelationUsersModule = new List<RelationUsersModule>();
            int addCount = 0;
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var userID in userIDArr)
                {
                    //删除当前人员的所有的权限，然后在添加新的权限
                    var userModule = relationUsersModuleService.GetEntities(t => t.UsersID == userID);
                    relationUsersModuleService.DeleteEntities(userModule.ToList());
                    foreach (var moduleID in moduleArr)
                    {
                        RelationUsersModule model = new RelationUsersModule();
                        model.UsersID = userID;
                        model.ModuleID = moduleID;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        listRelationUsersModule.Add(model);
                    }
                }
                addCount = relationUsersModuleService.AddEntities(listRelationUsersModule).Count();
                scope.Complete();
            }

            return Json(new Result(addCount > 0, "成功分配权限"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddRelationStoreModule(string IDs, string ModuleIDs)
        {
            if (string.IsNullOrWhiteSpace(IDs) || string.IsNullOrWhiteSpace(ModuleIDs))
            {
                return Json(new Result(false, "参数错误"), JsonRequestBehavior.AllowGet);
            }

            List<int> storesIDArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
            List<int> moduleArr = ModuleIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();

            //把所有菜单的父菜单取出来，但是只能保证三级菜单，多级有可能有bug
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            List<Module> allListModule = new List<Module>();
            allListModule.AddRange(moduleService.GetEntities(t => moduleArr.Contains(t.ID)).ToList());

            //复制一份副本，用于循环使用
            Module[] tempAllListModule = new Module[allListModule.Count];
            allListModule.CopyTo(tempAllListModule);

            foreach (var item in tempAllListModule)
            {
                Module parentModule = moduleService.GetFirstOrDefault(t => t.ID == item.ParentID);
                if (parentModule != null)
                {
                    if (allListModule.Where(t => t.ID == parentModule.ID).Count() == 0)
                    {
                        allListModule.Add(parentModule);
                    }
                }
            }
            //从新生成菜单ID
            moduleArr = allListModule.Select(t => t.ID).ToList();

            IRelationStoresModuleService relationStoresModuleService = ServiceFactory.Create<IRelationStoresModuleService>();

            List<RelationStoresModule> listRelationStoresModule = new List<RelationStoresModule>();
            int addCount = 0;
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var storeID in storesIDArr)
                {
                    //删除当前人员的所有的权限，然后在添加新的权限
                    var userModule = relationStoresModuleService.GetEntities(t => t.StoresID == storeID);
                    relationStoresModuleService.DeleteEntities(userModule.ToList());
                    foreach (var moduleID in moduleArr)
                    {
                        RelationStoresModule model = new RelationStoresModule();
                        model.StoresID = storeID;
                        model.ModuleID = moduleID;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        listRelationStoresModule.Add(model);
                    }
                }
                addCount = relationStoresModuleService.AddEntities(listRelationStoresModule).Count();
                scope.Complete();
            }

            return Json(new Result(addCount > 0, "成功分配权限"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserPermission(int id)
        {
            IRelationUsersModuleService relationUsersModuleService = ServiceFactory.Create<IRelationUsersModuleService>();
            List<int> moduleIDs = relationUsersModuleService.GetEntities(t => t.UsersID == id).Select(t => t.ModuleID).ToList();
            return Json(moduleIDs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRolePermission(int id)
        {
            IRelationRoleModuleService relationRoleModuleService = ServiceFactory.Create<IRelationRoleModuleService>();
            List<int> moduleIDs = relationRoleModuleService.GetEntities(t => t.RoleID == id).Select(t => t.ModuleID).ToList();
            return Json(moduleIDs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStorePermission(int id)
        {
            IRelationStoresModuleService relationStoresModuleService = ServiceFactory.Create<IRelationStoresModuleService>();
            List<int> moduleIDs = relationStoresModuleService.GetEntities(t => t.StoresID == id).Select(t => t.ModuleID).ToList();
            return Json(moduleIDs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetShopPermission(int id)
        {
            IRelationShopsModuleService relationShopsModuleService = ServiceFactory.Create<IRelationShopsModuleService>();
            List<int> moduleIDs = relationShopsModuleService.GetEntities(t => t.ShopID == id).Select(t => t.ModuleID).ToList();
            return Json(moduleIDs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopVersionPermission(int id)
        {
            IRelationShopVersionModuleService relationShopVersionModuleService = ServiceFactory.Create<IRelationShopVersionModuleService>();
            List<int> moduleIDs = relationShopVersionModuleService.GetEntities(t => t.ShopVersionID == id).Select(t => t.ModuleID).ToList();
            return Json(moduleIDs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRolesByStoreID(int? shopType)
        {
            IStoresService storesService = ServiceFactory.Create<IStoresService>();
            IRoleService roleService = ServiceFactory.Create<IRoleService>();

            List<KeyValuePair<Stores, List<Role>>> StoresRoleKeyValueList = new List<KeyValuePair<Stores, List<Role>>>();
            //如果是商家管理员则显示所有门店的角色
            if (CurrentInfo.IsShopAdmin)
            {
                int ShopID = CurrentInfo.CurrentShop.ID;
                //这边是销售的测试账号开户，给客户分配角色所用的商家ID
                if (shopType != null)
                {
                    if (shopType == (int)ShopTypeEnum.汽车美容)
                    {
                        ShopID = 4;
                    }
                    else if (shopType == (int)ShopTypeEnum.美容美发)
                    {
                        ShopID = 66;
                    }
                }

                var storesList = storesService.GetEntities(t => t.ShopId == ShopID && t.Disabled == false);
                foreach (var item in storesList)
                {
                    var data = roleService.GetEntities(t => t.StoreID == item.ID && t.Disabled == false).ToList();
                    if (data.Count > 0)
                    {
                        KeyValuePair<Stores, List<Role>> singleKeyValue = new KeyValuePair<Stores, List<Role>>(item, data);
                        StoresRoleKeyValueList.Add(singleKeyValue);
                    }

                }
            }
            else
            {

                int StoreID = CurrentInfo.CurrentStore.ID;

                //这边是销售的测试账号开户，给客户分配角色所用的门店ID
                if (shopType != null)
                {
                    if (shopType == (int)ShopTypeEnum.汽车美容)
                    {
                        StoreID = 45;
                    }
                    else if (shopType == (int)ShopTypeEnum.美容美发)
                    {
                        StoreID = 101;
                    }
                }

                var data = roleService.GetEntities(t => t.StoreID == StoreID && t.Disabled == false).ToList();
                KeyValuePair<Stores, List<Role>> singleKeyValue = new KeyValuePair<Stores, List<Role>>(CurrentInfo.CurrentStore, data);
                StoresRoleKeyValueList.Add(singleKeyValue);
            }

            return View(StoresRoleKeyValueList);
        }

        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="UserIDs"></param>
        /// <param name="RoleIDs"></param>
        /// <returns></returns>
        public JsonResult SetUserRole(string UserIDs, string RoleIDs)
        {
            if (string.IsNullOrWhiteSpace(UserIDs) || string.IsNullOrWhiteSpace(RoleIDs))
            {
                return Json(new Result(false, "参数错误"), JsonRequestBehavior.AllowGet);
            }

            List<int> userIDArr = UserIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
            List<int> roleIDArr = RoleIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();

            if (!CurrentInfo.IsAdministrator && userIDArr.Contains(1330))
            {
                return Json(new Result(false, "拒绝修改"), JsonRequestBehavior.AllowGet);
            }

            IRelationUserRoleService relationUserRoleService = ServiceFactory.Create<IRelationUserRoleService>();
            List<RelationUserRole> listRelationUserRole = new List<RelationUserRole>();
            int addCount = 0;
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var userID in userIDArr)
                {
                    //删除当前人员的所有的权限，然后在添加新的权限
                    var userRoles = relationUserRoleService.GetEntities(t => t.UserID == userID);
                    relationUserRoleService.DeleteEntities(userRoles.ToList());
                    foreach (var roleID in roleIDArr)
                    {
                        RelationUserRole model = new RelationUserRole();
                        model.UserID = userID;
                        model.RoleID = roleID;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        listRelationUserRole.Add(model);
                    }
                }
                addCount = relationUserRoleService.AddEntities(listRelationUserRole).Count();
                scope.Complete();
            }

            return Json(new Result(addCount > 0, "成功分配用户角色"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetUserAllRole(int UserID)
        {
            IRelationUserRoleService relationUserRoleService = ServiceFactory.Create<IRelationUserRoleService>();
            List<int> roleIDs = relationUserRoleService.GetEntities(t => t.UserID == UserID).Select(t => t.RoleID).ToList();
            return Json(roleIDs, JsonRequestBehavior.AllowGet);
        }
    }
}