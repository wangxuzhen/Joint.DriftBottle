using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Joint.IService;
using Joint.DLLFactory;
using Joint.Web.Framework;
using System.Text;
using Joint.Entity;
using System.Linq.Expressions;
using System.Data;
using Joint.Common;
using System.Transactions;

namespace Joint.Web.Areas.Admin.Controllers
{
    public class ModuleController : BaseAdminController
    {
        // GET: Admin/Module
        public ActionResult Index()
        {
            //如果不是管理员，拒绝访问
            if (!CurrentInfo.IsAdministrator)
            {
                return RedirectToAction("Error403", "Home", new { area = "Admin" });
            }

            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            var munes = moduleService.GetEntities(t => true);

            return View(munes);
        }

        public JsonResult GetModuleByID(int menuID)
        {
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            var data = moduleService.GetEntity(menuID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddMenu(Entity.Module model)
        {
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            bool hasExists = false;
            //如果Area，Controller,Action都不为空的时候则要检查数据库中是否已经含有这条数据
            if (!string.IsNullOrWhiteSpace(model.Area)
                && !string.IsNullOrWhiteSpace(model.Controller)
                && !string.IsNullOrWhiteSpace(model.Action))
            {
                hasExists = moduleService.Exists(t => t.LinkUrl == model.LinkUrl ||
                       (t.Area == model.Area
                       && t.Controller == model.Controller
                       && t.Action == model.Action)
                       );
            }

            if (hasExists)
            {
                return Json(new Result(false, "添加失败，数据库中已经含有相同Area，Controller,Action的菜单"), JsonRequestBehavior.AllowGet);
            }

            //如果数据库中没有相同的数据，则添加到数据库中
            model.ParentID = string.IsNullOrEmpty(model.ParentID.ToString()) ? 0 : model.ParentID;
            model.CreateUserID = CurrentInfo.CurrentUser.ID;
            model.CreateTime = DateTime.Now;
            model.IsMenu = true;


            var addSuccess = moduleService.AddEntity(model);

            //返回是否成功
            Result result = new Result(addSuccess != null, ResultType.Add);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DisableMenu(int menuID)
        {
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            var data = moduleService.GetEntity(menuID);
            data.Disabled = true;
            bool flage = moduleService.UpdateEntity(data);
            return Json(new Result(flage, "禁用成功"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StartUsingMenu(int menuID)
        {
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            var data = moduleService.GetEntity(menuID);
            data.Disabled = false;
            bool flage = moduleService.UpdateEntity(data);
            return Json(new Result(flage, "启用成功"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModules()
        {
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            var allModules = moduleService.GetEntities(t => true).ToList();

            List<Entity.Module> endModules = new List<Entity.Module>();
            var topMenu = allModules.Where(t => t.ParentID == 0).OrderBy(t => t.Sort).ToList();

            GetMenu(topMenu, allModules, endModules);

            var data = endModules.Select(t => new
            {
                ID = t.ID,
                Name = t.Name,
                Sort = t.Sort,
                Disabled = t.Disabled
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateMenu(Module model)
        {
            //BaseModel b = new BaseModel();
            //b.CopyFrom(b);

            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            var dbData = moduleService.GetEntity(model.ID, false);
            //dbData.CopyFrom(model, null);

            //model.ParentID = model.ParentID == null ? 0 : model.ParentID;
            model.IsMenu = true;
            model.CreateUserID = dbData.CreateUserID;
            model.CreateTime = dbData.CreateTime;

            var flage = moduleService.UpdateEntity(model);
            return Json(new Result(flage, ResultType.Update), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 递归整理菜单，用于下拉框使用
        /// </summary>
        /// <param name="parentModules">上级菜单</param>
        /// <param name="allModules">所有菜单</param>
        /// <param name="endModules">最后整理的菜单</param>
        private void GetMenu(List<Entity.Module> parentModules, List<Entity.Module> allModules, List<Entity.Module> endModules, int depth = 0)
        {
            foreach (var item in parentModules)
            {
                item.Name = GetDepthSign(depth) + item.Name;
                endModules.Add(item);
                //获取子菜单
                var subModules = allModules.Where(t => t.ParentID == item.ID).OrderBy(t => t.Sort);
                //遍历子菜单加入endModules，
                foreach (var subItem in subModules)
                {
                    subItem.Name = GetDepthSign(depth + 1) + subItem.Name;
                    endModules.Add(subItem);
                    //判断子菜单是否还有孩子菜单
                    var parentMenu = allModules.Where(t => t.ParentID == subItem.ID).OrderBy(t => t.Sort).ToList();
                    //若有孩子，继续递归
                    if (parentMenu.Count() > 0)
                    {
                        GetMenu(parentMenu, allModules, endModules, depth + 2);
                    }
                }
            }
        }

        private string GetDepthSign(int depth)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                //如果是最后一级的时候，就用├符号，否则的话前面加空格
                if (i == (depth - 1))
                {
                    sb.Append("├┄");
                }
                else
                {
                    sb.Append("&nbsp;&nbsp;");
                }

            }
            return sb.ToString();
        }

        public JsonResult GetCurrentUserSidebar(string Query)
        {
            Func<Module, Boolean> lblWhere = null;
            if (string.IsNullOrWhiteSpace(Query))
            {
                lblWhere = t => t.ParentID != 0 && t.IsMenu == true && !string.IsNullOrWhiteSpace(t.LinkUrl);
            }
            else
            {
                lblWhere = t => t.ParentID != 0 && t.IsMenu == true && !string.IsNullOrWhiteSpace(t.LinkUrl) && t.Name.Contains(Query);
            }

            //过滤掉首首页菜单
            var resultJson = CurrentInfo.Sidebar.Where(lblWhere).Select(t => new
            {
                LinkUrl = t.LinkUrl,
                Name = t.Name
            });

            return Json(resultJson, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddPrivileges(Privileges model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return Json(new Result(false, "权限名称不能为空"), JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrWhiteSpace(model.Code))
            {
                return Json(new Result(false, "权限代码不能为空"), JsonRequestBehavior.AllowGet);
            }

            if (ServiceHelper.GetPrivilegesService.Exists(t => t.Code == model.Code))
            {
                return Json(new Result(false, "权限代码已经存在，请修改"), JsonRequestBehavior.AllowGet);
            }
            model.CreateTime = DateTime.Now;
            model.CreateUserID = CurrentInfo.CurrentUser.ID;
            //moduleID有设置关联，不允许为0
            if (model.ModuleID == 0)
            {
                model.ModuleID = null;
            }
            var success = model.Add() != null;

            return Json(new Result(success, ResultType.Add), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePrivileges(Privileges model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return Json(new Result(false, "权限名称不能为空"), JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrWhiteSpace(model.Code))
            {
                return Json(new Result(false, "权限代码不能为空"), JsonRequestBehavior.AllowGet);
            }

            if (ServiceHelper.GetPrivilegesService.Exists(t => t.ID != model.ID && t.Code == model.Code))
            {
                return Json(new Result(false, "权限代码已经存在，请修改"), JsonRequestBehavior.AllowGet);
            }
            //model.CreateTime = DateTime.Now;
            //model.CreateUserID = CurrentInfo.CurrentUser.ID;
            //moduleID有设置关联，不允许为0
            if (model.ModuleID == 0)
            {
                model.ModuleID = null;
            }
            var dbModel = ServiceHelper.GetPrivilegesService.GetEntity(model.ID);
            dbModel.Name = model.Name;
            dbModel.ModuleID = model.ModuleID;
            dbModel.Code = model.Code;
            dbModel.Value = model.Value;
            dbModel.UserShow = model.UserShow;
            dbModel.StoreShow = model.StoreShow;
            dbModel.ShopShow = model.ShopShow;
            dbModel.RoleShow = model.RoleShow;
            dbModel.SystemShow = model.SystemShow;
            dbModel.Sort = model.Sort;
            dbModel.Describe = model.Describe;
            dbModel.Disabled = model.Disabled;

            var success = dbModel.Update();
            return Json(new Result(success, ResultType.Update), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPrivilegeCode(string privilegeName)
        {
            string privilegeCode = privilegeName.GetChineseHeadSpell();
            privilegeCode += DateTime.Now.ToString("yyyyMMdd");
            return Json(new Result(true, privilegeCode), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPrivilegesList(int rows, int page, int? moduleID)
        {
            IPrivilegesService privilegesService = ServiceFactory.Create<IPrivilegesService>();

            var lblWhere = BuildSearchCriteria(moduleID);
            int total;
            var resultData = privilegesService.GetEntitiesByPage(page, rows, out total, lblWhere, false, t => t.ID).Select(t => new
            {
                ID = t.ID,
                ModuleID = t.ModuleID,
                ModuleName = t.ModuleID == null ? "无" : t.Module.Name,
                Name = t.Name,
                Code = t.Code,
                Value = t.Value,
                UserShow = t.UserShow,
                RoleShow = t.RoleShow,
                StoreShow = t.StoreShow,
                ShopShow = t.ShopShow,
                SystemShow = t.SystemShow,
                Sort = t.Sort,
                Describe = t.Describe,
                Disabled = t.Disabled
            }).ToList();
            return Json(new { rows = resultData.ToDataTable(), total = total }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 构建查询表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<Privileges, Boolean>> BuildSearchCriteria(int? moduleID)
        {
            DynamicLambda<Privileges> bulider = new DynamicLambda<Privileges>();
            Expression<Func<Privileges, Boolean>> expr = null;
            if (moduleID != 0 && moduleID != null)
            {
                Expression<Func<Privileges, Boolean>> tmpStoreID = t => t.ModuleID == moduleID;
                expr = bulider.BuildQueryAnd(expr, tmpStoreID);
            }
            return expr;
        }


        public ActionResult ShowPrivileges(string PType, int ID)
        {
            List<Privileges> resultData = new List<Privileges>();
            if (PType.ToLower() == "SystemShow".ToLower())
            {
                resultData = ServiceHelper.GetPrivilegesService.GetEntities(t => t.SystemShow == true && t.Disabled == false).ToList();
            }
            else if (PType.ToLower() == "ShopShow".ToLower())
            {
                resultData = ServiceHelper.GetPrivilegesService.GetEntities(t => t.ShopShow == true && t.Disabled == false).ToList();
            }
            else if (PType.ToLower() == "StoreShow".ToLower())
            {
                resultData = ServiceHelper.GetPrivilegesService.GetEntities(t => t.StoreShow == true && t.Disabled == false).ToList();
            }
            else if (PType.ToLower() == "UserShow".ToLower())
            {
                resultData = ServiceHelper.GetPrivilegesService.GetEntities(t => t.UserShow == true && t.Disabled == false).ToList();
            }
            else if (PType.ToLower() == "RoleShow".ToLower())
            {
                resultData = ServiceHelper.GetPrivilegesService.GetEntities(t => t.RoleShow == true && t.Disabled == false).ToList();
            }
            return View(resultData);
        }

        public JsonResult GetHasPrivileges(string PType, int ID)
        {
            List<int> ids = null;

            if (PType.ToLower() == "SystemShow".ToLower())
            {
                ids = ServiceHelper.GetRelationPrivilegesSystemService.GetEntities(t => t.SystemID == ID).Select(t => t.PrivilegesID).ToList();
            }
            else if (PType.ToLower() == "ShopShow".ToLower())
            {
                ids = ServiceHelper.GetRelationPrivilegesShopsService.GetEntities(t => t.ShopsID == ID).Select(t => t.PrivilegesID).ToList();
            }
            else if (PType.ToLower() == "StoreShow".ToLower())
            {
                ids = ServiceHelper.GetRelationPrivilegesStoresService.GetEntities(t => t.StoresID == ID).Select(t => t.PrivilegesID).ToList();
            }
            else if (PType.ToLower() == "RoleShow".ToLower())
            {
                ids = ServiceHelper.GetRelationPrivilegesRoleService.GetEntities(t => t.RoleID == ID).Select(t => t.PrivilegesID).ToList();
            }
            else if (PType.ToLower() == "UserShow".ToLower())
            {
                ids = ServiceHelper.GetRelationPrivilegesUsersService.GetEntities(t => t.UsersID == ID).Select(t => t.PrivilegesID).ToList();
            }
            return Json(ids, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddPrivilegesByType(string PType, int ID, string privilegesIDSs)
        {
            List<int> ids = privilegesIDSs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
            if (PType.ToLower() == "SystemShow".ToLower())
            {
                using (TransactionScope sc = TransactionScopeHelper.GetTran())
                {
                    //删除系统的权限，重新添加
                    ServiceHelper.GetRelationPrivilegesSystemService.DeleteEntitiesByWhere(t => t.SystemID == ID);
                    foreach (var item in ids)
                    {
                        RelationPrivilegesSystem model = new RelationPrivilegesSystem();
                        model.SystemID = ID;
                        model.PrivilegesID = item;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        model.Add();
                    }
                    sc.Complete();
                }

                return Json(new Result(true, ResultType.Add), JsonRequestBehavior.AllowGet);

            }
            else if (PType.ToLower() == "ShopShow".ToLower())
            {
                using (TransactionScope sc = TransactionScopeHelper.GetTran())
                {
                    //删除商家的权限，重新添加
                    ServiceHelper.GetRelationPrivilegesShopsService.DeleteEntitiesByWhere(t => t.ShopsID == ID);
                    foreach (var item in ids)
                    {
                        RelationPrivilegesShops model = new RelationPrivilegesShops();
                        model.ShopsID = ID;
                        model.PrivilegesID = item;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        model.Add();
                    }
                    sc.Complete();
                }

                return Json(new Result(true, ResultType.Add), JsonRequestBehavior.AllowGet);
            }
            else if (PType.ToLower() == "StoreShow".ToLower())
            {
                using (TransactionScope sc = TransactionScopeHelper.GetTran())
                {
                    //删除门店的权限，重新添加
                    ServiceHelper.GetRelationPrivilegesStoresService.DeleteEntitiesByWhere(t => t.StoresID == ID);
                    foreach (var item in ids)
                    {
                        RelationPrivilegesStores model = new RelationPrivilegesStores();
                        model.StoresID = ID;
                        model.PrivilegesID = item;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        model.Add();
                    }
                    sc.Complete();
                }

                return Json(new Result(true, ResultType.Add), JsonRequestBehavior.AllowGet);
            }
            else if (PType.ToLower() == "RoleShow".ToLower())
            {
                using (TransactionScope sc = TransactionScopeHelper.GetTran())
                {
                    //删除门店的权限，重新添加
                    ServiceHelper.GetRelationPrivilegesRoleService.DeleteEntitiesByWhere(t => t.RoleID == ID);
                    foreach (var item in ids)
                    {
                        RelationPrivilegesRole model = new RelationPrivilegesRole();
                        model.RoleID = ID;
                        model.PrivilegesID = item;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        model.Add();
                    }
                    sc.Complete();
                }

                return Json(new Result(true, ResultType.Add), JsonRequestBehavior.AllowGet);
            }
            else if (PType.ToLower() == "UserShow".ToLower())
            {
                using (TransactionScope sc = TransactionScopeHelper.GetTran())
                {
                    //删除用户的权限，重新添加
                    ServiceHelper.GetRelationPrivilegesUsersService.DeleteEntitiesByWhere(t => t.UsersID == ID);
                    foreach (var item in ids)
                    {
                        RelationPrivilegesUsers model = new RelationPrivilegesUsers();
                        model.UsersID = ID;
                        model.PrivilegesID = item;
                        model.CreateUserID = CurrentInfo.CurrentUser.ID;
                        model.CreateTime = DateTime.Now;
                        model.Add();
                    }
                    sc.Complete();
                }

                return Json(new Result(true, ResultType.Add), JsonRequestBehavior.AllowGet);
            }

            return Json(new Result(false, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPrivilegesByID(int PrivilegesID)
        {
            var data = ServiceHelper.GetPrivilegesService.GetEntity(PrivilegesID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}