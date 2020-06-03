using Joint.Common;
using Joint.DLLFactory;
using Joint.Entity;
using Joint.IRepository;
using Joint.IService;
using Joint.Repository;
using Joint.Web.Areas.Admin.Models;
using Joint.Web.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Joint.Web.Areas.Admin.Controllers
{
    public class ShopsController : BaseAdminController
    {
        // GET: Admin/Shops
        public ActionResult Index()
        {
            //如果不是管理员，拒绝访问
            //if (!CurrentInfo.IsAdministrator)
            //{
            //    return RedirectToAction("Error403", "Home", new { area = "Admin" });
            //}
            //TimeSpan ts = new TimeSpan(9, 30, 0);
            IShopsService shopsService = ServiceHelper.GetShopsService;
            //下一个商户号，默认从610001开始
            string nextMerchantID = "610001";
            var dataShop = shopsService.GetTopEntities(1, t => t.ID, t => true, false);
            if (dataShop != null)
            {
                nextMerchantID = "61" + (dataShop.FirstOrDefault().ID + 1).ToString().PadLeft(4, '0');
            }
            ViewBag.NextMerchantID = nextMerchantID;

            //销售员
            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            List<SelectListItemModel> storeAllSaleUser = usersService.GetEntities(t => t.DefaultStoreID == CurrentInfo.CurrentStore.ID && t.Disabled == false && t.Remark.Trim() == "奇策内部销售").ToList().Select(
                    t => new SelectListItemModel
                    {
                        Value = t.ID.ToString(),
                        Text = t.RealName
                    }
                ).ToList();

            List<SelectListItemModel> storeAllAfterSales = usersService.GetEntities(t => t.DefaultStoreID == CurrentInfo.CurrentStore.ID && t.Disabled == false && t.Remark.Trim() == "奇策内部售后").ToList().Select(
                   t => new SelectListItemModel
                   {
                       Value = t.ID.ToString(),
                       Text = t.RealName
                   }
               ).ToList();

            //storeAllSaleUser.Insert(0, new SelectListItemModel { Value = "0", Text = "无" });
            //SelectList allUserSelect = new SelectList(storeAllSaleUser, "Value", "Text", "0");// CurrentInfo.CurrentStore.ID
            ViewData["storeAllSaleUser"] = new SelectList(storeAllSaleUser, "Value", "Text", "0");// CurrentInfo.CurrentStore.ID
            ViewData["storeAllAfterSales"] = new SelectList(storeAllAfterSales, "Value", "Text", "0");// CurrentInfo.CurrentStore.ID
            //SelectList  afterSales
            return View();
        }

        /// <summary>
        /// 创建商家二维码
        /// </summary>
        /// <param name="XShow"></param>
        /// <returns></returns>
        public ActionResult CreateQRCode(string XShow)
        {
            string shopUrl = string.Empty;
            if (CurrentInfo.GetCurrentShopType() == ShopTypeEnum.美容美发)//美容美发显示奇策，奇策的显示美容美发的
            {
                shopUrl = "http://www.bbbb4.com/home/jkxt/?XShow=" + XShow;
            }
            else
            {
                shopUrl = "http://mf.iqcrj.com/homemeifa/jkxt/?XShow=" + XShow;
            }

            QRCodeHelper qrCode = new QRCodeHelper();
            Bitmap qrImg = qrCode.Create(shopUrl, 12);
            MemoryStream ms = new MemoryStream();
            qrImg.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] bytes = ms.GetBuffer();
            return File(bytes, @"image/jpeg");
        }

        public ActionResult GetShopsPage(int pageSize, int pageNumber, string shopName, string MerchantID)
        {

            //SearchModel<ShopsParams> searchModel = new SearchModel<ShopsParams>
            //{
            //    PageIndex = pageNumber,
            //    PageSize = pageSize,
            //    Model = new ShopsParams() { ShopName = shopName }
            //};

            var expr = BuildSearchCriteria(shopName, MerchantID);
            int total;
            //WhereHelper<Shops> where = new WhereHelper<Shops>();
            //if (!string.IsNullOrEmpty(shopName))
            //{
            //    where.Contains("ShopName", shopName);
            //}

            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            var data = shopsService.GetEntitiesByPage(pageNumber, pageSize, out total, expr, false, t => t.ID).ToList().Select(t => new
            {
                ID = t.ID,
                MerchantID = "61" + t.ID.ToString().PadLeft(4, '0'),
                ShopName = t.ShopName,
                UserID = t.AdminUserID,
                //ThisUser = ServiceHelper.GetUsersService.GetEntity(t.AdminUserID),
                UserName = ServiceHelper.GetUsersService.GetEntity(t.AdminUserID).UserName,
                TotalMoney = t.TotalMoney,
                Phone = ServiceHelper.GetUsersService.GetEntity(t.AdminUserID).Phone,
                DueDate = t.DueDate.ToString("yyyy-MM-dd"),
                CreateUser = "sdsdjskjdksjd",
                CreateTime = t.CreateTime.ToString("yyyy-MM-dd"),
                NoticeOpenID = t.NoticeOpenID,
                Disabled = t.Disabled == true,
                ShopType = t.ShopType,
                ShopVersionName = t.ShopVersion.Name,
                SalespersonName = "销售员",
                Deposit = t.Deposit,
                FinalPayment = t.FinalPayment,
                RegionText = t.Province + "." + t.City + "." + t.County
            }).ToList();

            return Json(new { total = total, rows = data }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 构建查询表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<Shops, Boolean>> BuildSearchCriteria(string strSearchkey, string MerchantID)
        {
            DynamicLambda<Shops> bulider = new DynamicLambda<Shops>();
            Expression<Func<Shops, Boolean>> expr = null;

            //如果商户号不为空，则优先搜索商户号
            if (!string.IsNullOrWhiteSpace(MerchantID))
            {
                int intMerchantID = 0;
                try
                {
                    if (MerchantID.Length >= 6 && MerchantID.StartsWith("61"))
                    {
                        MerchantID = MerchantID.Trim().Substring(2, 4);

                        intMerchantID = Convert.ToInt32(MerchantID.Trim());
                    }
                }
                catch
                {

                }

                Expression<Func<Shops, Boolean>> temp = t => t.ID == intMerchantID;
                expr = bulider.BuildQueryAnd(expr, temp);
            }
            else
            {
                Expression<Func<Shops, Boolean>> tmpStoreID = t => t.ShopName.Contains(strSearchkey)
                || t.Province.Contains(strSearchkey)
                || t.City.Contains(strSearchkey)
                || t.County.Contains(strSearchkey)
                //|| t.Users.UserName.Contains(strSearchkey)
                //|| t.Users.Phone.Contains(strSearchkey)
                //|| t.Users2.RealName.Contains(strSearchkey)
                || t.Remark.Contains(strSearchkey);
                expr = bulider.BuildQueryAnd(expr, tmpStoreID);
            }
            return expr;
        }

        public ActionResult GetShopsByID(int ID)
        {
            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            IStoresService storesService = ServiceFactory.Create<IStoresService>();

            ShopModel shopModel = new ShopModel();
            var shops = shopsService.GetEntity(ID);
            var users = ServiceHelper.GetUsersService.GetEntity(shops.AdminUserID);// shops.Users;
            var stores = users.Stores;

            shopModel.ID = shops.ID;
            shopModel.ShopName = shops.ShopName;
            shopModel.AdminUserID = users.ID;

            shopModel.Disabled = shops.Disabled;
            shopModel.Domain = shops.Domain;
            shopModel.DomainName = shops.DomainName;
            shopModel.ShopVersionID = shops.ShopVersionID == null ? 0 : Convert.ToInt32(shops.ShopVersionID);
            shopModel.DueDate = shops.DueDate;
            shopModel.TotalMoney = shops.TotalMoney;
            shopModel.Remark = shops.Remark;

            shopModel.ShopType = shops.ShopType;
            shopModel.SalespersonID = shops.SalespersonID;
            shopModel.Deposit = shops.Deposit;
            shopModel.FinalPayment = shops.FinalPayment;
            shopModel.Province = shops.Province;
            shopModel.ProvinceCode = shops.ProvinceCode;
            shopModel.City = shops.City;
            shopModel.CityCode = shops.CityCode;
            shopModel.County = shops.County;
            shopModel.CountyCode = shops.CountyCode;
            shopModel.AfterSales = shops.AfterSales;
            shopModel.LogoUrl = shops.LogoUrl;
            shopModel.AnnualFee = shops.AnnualFee;
            shopModel.SiteName = shops.SiteName;
            //shopModel.

            shopModel.RealName = users.RealName;
            shopModel.UserName = users.UserName;
            shopModel.Password = users.Password;
            shopModel.Idcard = users.Idcard;
            shopModel.Phone = users.Phone;

            return Json(shopModel, JsonRequestBehavior.AllowGet);

        }



        public ActionResult DisableShops(string SID)
        {

            string[] strIds = SID.Split(',');
            List<int> list = new List<int>();
            foreach (string id in strIds)
            {
                list.Add(int.Parse(id));
            }

            var flag = false;
            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var item in list)
                {
                    var data = shopsService.GetEntity(item);
                    data.Disabled = true;
                    shopsService.UpdateEntity(data);
                }
                scope.Complete();
                flag = true;
            }

            return Json(new Result(flag, ResultType.Other), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReDisableShops(string SID)
        {
            string[] strIds = SID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<int> list = new List<int>();
            foreach (string id in strIds)
            {
                list.Add(int.Parse(id));
            }

            var flag = false;
            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                foreach (var item in list)
                {
                    var data = shopsService.GetEntity(item);
                    data.Disabled = false;
                    shopsService.UpdateEntity(data);
                }
                scope.Complete();
                flag = true;
            }

            return Json(new Result(flag, ResultType.Other), JsonRequestBehavior.AllowGet);
        }

        public bool AddRelationShopModule(string IDs, string ModuleIDs, int createUserID = 0)
        {
            if (string.IsNullOrWhiteSpace(IDs) || string.IsNullOrWhiteSpace(ModuleIDs))
            {
                return false;
            }

            if (createUserID == 0)
            {
                createUserID = CurrentInfo.CurrentUser.ID;
            }


            List<int> shopIDArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
            List<int> moduleArr = ModuleIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();

            //把所有菜单的父菜单取出来，但是只能保证三级菜单，多级有可能有bug
            IModuleService moduleService = ServiceFactory.Create<IModuleService>();
            List<Module> allListModule = new List<Module>();
            allListModule.AddRange(moduleService.GetEntities(t => moduleArr.Contains(t.ID)).ToList());


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
                        model.CreateUserID = createUserID;
                        model.CreateTime = DateTime.Now;
                        listRelationShopsModule.Add(model);
                    }
                }
                addCount = relationShopsModuleService.AddEntities(listRelationShopsModule).Count();
                scope.Complete();
            }

            if (addCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AddRelationStoreModule(string IDs, string ModuleIDs, int createUserID = 0)
        {
            if (string.IsNullOrWhiteSpace(IDs) || string.IsNullOrWhiteSpace(ModuleIDs))
            {
                return false;
            }

            if (createUserID == 0)
            {
                createUserID = CurrentInfo.CurrentUser.ID;
            }


            List<int> storesIDArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
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
                        model.CreateUserID = createUserID;
                        model.CreateTime = DateTime.Now;
                        listRelationStoresModule.Add(model);
                    }
                }
                addCount = relationStoresModuleService.AddEntities(listRelationStoresModule).Count();
                scope.Complete();
            }

            if (addCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool AddRelationRoleModule(string IDs, string ModuleIDs, int createUserID = 0)
        {
            if (string.IsNullOrWhiteSpace(IDs) || string.IsNullOrWhiteSpace(ModuleIDs))
            {
                return false;
            }

            if (createUserID == 0)
            {
                createUserID = CurrentInfo.CurrentUser.ID;
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
                        model.CreateUserID = createUserID;
                        model.CreateTime = DateTime.Now;
                        listRelationRolesModule.Add(model);
                    }
                }
                addCount = relationRoleModuleService.AddEntities(listRelationRolesModule).Count();
                scope.Complete();
            }

            if (addCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        private bool AddRelationUsersModule(string IDs, string ModuleIDs, int createUserID = 0)
        {
            if (string.IsNullOrWhiteSpace(IDs) || string.IsNullOrWhiteSpace(ModuleIDs))
            {
                return false;
            }
            if (createUserID == 0)
            {
                createUserID = CurrentInfo.CurrentUser.ID;
            }


            List<int> userIDArr = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
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
                        model.CreateUserID = createUserID;
                        model.CreateTime = DateTime.Now;
                        listRelationUsersModule.Add(model);
                    }
                }
                addCount = relationUsersModuleService.AddEntities(listRelationUsersModule).Count();
                scope.Complete();
            }
            if (addCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public int nowAddShopID = 0;


        public Shops CreateShop(ShopModel shopModel, int createUserID = 0)
        {
            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            IStoresService storesService = ServiceFactory.Create<IStoresService>();

            if (createUserID == 0)
            {
                createUserID = CurrentInfo.CurrentUser.ID;
            }

            //bool saveSuccess = false;
            Shops shops = null;
            //用事务来添加一个商家
            using (TransactionScope scope = TransactionScopeHelper.GetTran(TransactionScopeOption.Required, TimeSpan.FromSeconds(20)))
            {

                //商家账号
                Users users = new Users();
                users.RealName = shopModel.RealName;
                users.UserName = shopModel.UserName;
                //添加账号PasswordSalt随机
                users.PasswordSalt = Common.TextFilter.GetPasswordSalt(); //Common.TextFilter.Substring(Guid.NewGuid().ToString("N"), 10, false);
                string endPassword = shopModel.Password + users.PasswordSalt;
                users.Password = Common.SecureHelper.MD5(endPassword);
                users.NeedAccount = true;

                users.Idcard = shopModel.Idcard;
                users.Phone = shopModel.Phone;
                users.DefaultStoreID = null;
                users.CreateUserID = createUserID;
                users.CreateTime = DateTime.Now;
                users.Disabled = shopModel.Disabled;

                users = usersService.AddEntity(users);

                //商家信息
                shops = new Shops();
                shops.ShopName = shopModel.ShopName;
                shops.Domain = shopModel.Domain;
                shops.DomainName = shopModel.DomainName;
                shops.DueDate = shopModel.DueDate;
                shops.TotalMoney = shopModel.TotalMoney;
                shops.Remark = shopModel.Remark;
                shops.AdminUserID = users.ID;
                shops.CreateUserID = createUserID;
                shops.CreateTime = DateTime.Now;
                shops.Disabled = shopModel.Disabled;
                shops.ShopVersionID = shopModel.ShopVersionID;
                shops.ShopType = shopModel.ShopType;
                shops.SalespersonID = shopModel.SalespersonID;
                shops.Deposit = shopModel.Deposit;
                shops.FinalPayment = shopModel.FinalPayment;
                shops.Province = shopModel.Province;
                shops.ProvinceCode = shopModel.ProvinceCode;
                shops.City = shopModel.City;
                shops.CityCode = shopModel.CityCode;
                shops.County = shopModel.County;
                shops.CountyCode = shopModel.CountyCode;
                shops.AfterSales = shopModel.AfterSales;
                shops.LogoUrl = shopModel.LogoUrl;
                shops.AnnualFee = shopModel.AnnualFee;
                shops.SiteName = shopModel.SiteName;

                //默认赋值
                {
                    shops.DueDate = DateTime.MaxValue;
                    shops.TotalMoney = 0;
                    shops.Disabled = false;
                    shops.ShopVersionID = 1;
                }

                shops = shopsService.AddEntity(shops);

                //门店（开通商家的时候默认开通一号店）
                Stores stores = new Stores();
                stores.ShopId = shops.ID;
                stores.StoreName = "一号部门";
                stores.IsShare = false;
                stores.IsShowWeiXin = false;
                stores.IsMainStore = false;

                stores = storesService.AddEntity(stores);

                users.ShopsID = shops.ID;
                users.DefaultStoreID = stores.ID;
                usersService.UpdateEntity(users);

                //Category categoryModel = new Category();
                //categoryModel.Name = "总仓";
                //categoryModel.Sort = 1;
                //categoryModel.CategoryType = 3;
                //categoryModel.Name = categoryModel.Name.Trim();
                //categoryModel.UserID = users.ID;
                //categoryModel.CreateTime = DateTime.Now;
                //categoryModel.StoreID = stores.ID;
                //categoryModel = categoryModel.Add();

                //Role roleModel = new Role();
                //roleModel.Name = "经理";
                //roleModel.Sort = 1;
                //roleModel.ShopsID = shops.ID;
                //roleModel.StoreID = stores.ID;
                //roleModel.CreateUserID = users.ID;
                //roleModel.CreateTime = DateTime.Now;
                //roleModel = roleModel.Add();


                //这边要取出这个版本所有菜单，然后加到用户菜单关系表 和 商家菜单关系表中
                IRelationShopVersionModuleService relationShopVersionModuleService = ServiceFactory.Create<IRelationShopVersionModuleService>();
                var listModuleID = relationShopVersionModuleService.GetEntities(t => t.ShopVersionID == shopModel.ShopVersionID).Select(t => t.ModuleID).ToList();
                var strModuleID = string.Join(",", listModuleID.ToArray());

                //添加商家和菜单的关系
                AddRelationShopModule(shops.ID.ToString(), strModuleID, createUserID);
                //添加门店和菜单的关系
                AddRelationStoreModule(stores.ID.ToString(), strModuleID, createUserID);
                //添加用户和菜单的关系
                AddRelationUsersModule(users.ID.ToString(), strModuleID, createUserID);
                ////添加角色和菜单的关系
                //AddRelationRoleModule(roleModel.ID.ToString(), strModuleID, createUserID);
                scope.Complete();
            }

            return shops;
        }


        public ActionResult AddShops(ShopModel shopModel)
        {
            bool flage = ServiceHelper.GetShopsService.Exists(t => t.ShopName == shopModel.ShopName);
            if (flage)
            {
                return Json(new Result(false, "已经存在同名机构"), JsonRequestBehavior.AllowGet);
            }



            Shops singleShops = CreateShop(shopModel);

            return Json(new Result(singleShops != null, ResultType.Add), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResetShopVersion(int shopsID, int shopVersionID)
        {
            //如果不是管理员，拒绝调用此方法
            if (!CurrentInfo.IsAdministrator)
            {
                return Json(new Result(false, ResultType.Update), JsonRequestBehavior.AllowGet);
            }

            //这边要取出这个版本所有菜单，然后加到用户菜单关系表 和 商家菜单关系表中
            IRelationShopVersionModuleService relationShopVersionModuleService = ServiceFactory.Create<IRelationShopVersionModuleService>();
            var listModuleID = relationShopVersionModuleService.GetEntities(t => t.ShopVersionID == shopVersionID).Select(t => t.ModuleID).ToList();
            var strModuleID = string.Join(",", listModuleID.ToArray());

            IShopsService shopsService = ServiceHelper.GetShopsService;
            IStoresService storesService = ServiceHelper.GetStoresService;
            var modelStore = storesService.GetTopEntities(1, t => t.ID, t => t.ShopId == shopsID, true).FirstOrDefault();
            int shopAdminUserID = shopsService.GetEntity(shopsID).AdminUserID;
            Role roleModel = ServiceHelper.GetRoleService.GetFirstOrDefault(t => t.StoreID == modelStore.ID);

            //添加商家和菜单的关系
            AddRelationShopModule(shopsID.ToString(), strModuleID);
            //添加门店和菜单的关系
            AddRelationStoreModule(modelStore.ID.ToString(), strModuleID);
            //添加用户和菜单的关系
            AddRelationUsersModule(shopAdminUserID.ToString(), strModuleID);

            //如果店家有添加角色
            if (roleModel != null)
            {
                //添加角色和菜单的关系
                AddRelationRoleModule(roleModel.ID.ToString(), strModuleID);
            }

            //System.Threading.Thread.Sleep(2000);

            return Json(new Result(true, ResultType.Update), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopsJson(int pageindex, int pagesize, string cols, string qtext)
        {
            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            int total = 0;

            //pageindex默认从0开始，也就是第一页是0，所以pageindex + 1
            var data = shopsService.GetEntitiesByPage(pageindex + 1, pagesize, out total, t => t.Disabled != true && t.ShopName.Contains(qtext), false, t => t.ID).ToList().Select(t => new  //(t => t.DefaultStoreID == CurrentInfo.CurrentUser.DefaultStoreID)
            {
                ID = t.ID,
                ShopName = t.ShopName,
                DueDate = t.DueDate.ToString("yyyy-MM-dd")
            }).ToList();

            //返回值记得加total前端要用
            return Json(new { total = total, rows = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditShops(ShopModel shopModel)
        {
            IUsersService usersService = ServiceFactory.Create<IUsersService>();
            IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            IStoresService storesService = ServiceFactory.Create<IStoresService>();

            if (string.IsNullOrWhiteSpace(shopModel.ProvinceCode) || string.IsNullOrWhiteSpace(shopModel.CityCode) || string.IsNullOrWhiteSpace(shopModel.CountyCode))
            {
                return Json(new Result(false, "省、市、县必填"), JsonRequestBehavior.AllowGet);
            }

            bool flage = shopsService.Exists(t => t.ID != shopModel.ID && t.ShopName == shopModel.ShopName);
            if (flage)
            {
                return Json(new Result(false, "已经存在同名商家,请修改"), JsonRequestBehavior.AllowGet);
            }
            //判断是改变了版本
            bool versionHasChange = false;
            bool saveSuccess = false;
            //用事务来更新一个商家
            using (TransactionScope scope = TransactionScopeHelper.GetTran())
            {
                //商家信息
                Shops shops = shopsService.GetEntity(shopModel.ID);
                shops.ShopName = shopModel.ShopName;
                shops.Domain = shopModel.Domain;
                shops.DomainName = shopModel.DomainName;
                //如果版本改变了，则需要重置商家的菜单
                if (shops.ShopVersionID != shopModel.ShopVersionID)
                {
                    versionHasChange = true;
                }
                shops.ShopVersionID = shopModel.ShopVersionID;
                shops.DueDate = shopModel.DueDate;
                shops.TotalMoney = shopModel.TotalMoney;
                shops.Remark = shopModel.Remark;
                shops.Disabled = shopModel.Disabled;

                shops.ShopType = shopModel.ShopType;
                shops.SalespersonID = shopModel.SalespersonID;
                shops.Deposit = shopModel.Deposit;
                shops.FinalPayment = shopModel.FinalPayment;
                shops.Province = shopModel.Province;
                shops.ProvinceCode = shopModel.ProvinceCode;
                shops.City = shopModel.City;
                shops.CityCode = shopModel.CityCode;
                shops.County = shopModel.County;
                shops.CountyCode = shopModel.CountyCode;
                shops.AfterSales = shopModel.AfterSales;
                shops.LogoUrl = shopModel.LogoUrl;
                shops.AnnualFee = shopModel.AnnualFee;
                shops.SiteName = shopModel.SiteName;

                bool update1 = shopsService.UpdateEntity(shops);

                //商家账号
                Users users = usersService.GetEntity(shops.AdminUserID);
                users.RealName = shopModel.RealName;
                users.UserName = shopModel.UserName;
                //如果用户修改了密码，则要重新生成密码规则
                if (users.Password != shopModel.Password)
                {
                    //修改账号PasswordSalt要判断是否是管理员
                    users.PasswordSalt = Common.TextFilter.GetPasswordSalt(UserIsAdministrator(users));// Common.TextFilter.Substring(Guid.NewGuid().ToString("N"), 10, false);
                    string endPassword = shopModel.Password + users.PasswordSalt;
                    users.Password = Common.SecureHelper.MD5(endPassword);
                }

                users.Idcard = shopModel.Idcard;
                users.Phone = shopModel.Phone;
                users.Disabled = shopModel.Disabled;

                bool update2 = usersService.UpdateEntity(users);

                //如果版本修改了
                if (versionHasChange)
                {
                    ResetShopVersion(shopModel.ID, shopModel.ShopVersionID);
                }

                scope.Complete();
                saveSuccess = update1 && update2;
            }

            return Json(new Result(saveSuccess, ResultType.Add), JsonRequestBehavior.AllowGet);

            //IShopsService shopsService = ServiceFactory.Create<IShopsService>();
            //bool flag = shopsService.Exists(t => t.ID != shopModel.ID && string.Equals(t.ShopName, shopModel.ShopName, StringComparison.OrdinalIgnoreCase));
            //if (flag)
            //{
            //    return Json(new Result(false, "该商家已存在"), JsonRequestBehavior.AllowGet);
            //}
            //bool IsSuccess = shopsService.UpdateEntity(shopModel);
            //return Json(new Result(IsSuccess, ResultType.Update), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCommercialTenant(string strIDs)
        {
            List<int> shopVersionIDArr = new List<int>();
            IShopsService shopsService = ServiceHelper.GetShopsService;
            DateTime beginDate = Convert.ToDateTime("2016-10-01");
            IQueryable<Shops> shopsListTemp;
            //ShopVersionID=32说明是自由组合的菜单，在刷菜单的时候需要避开这些菜单
            if (string.IsNullOrWhiteSpace(strIDs))
            {
                shopsListTemp = shopsService.GetEntities(t => t.CreateTime > beginDate && t.ShopVersionID != 32);
            }
            else
            {
                shopVersionIDArr = strIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Convert.ToInt32(t)).ToList();
                shopsListTemp = shopsService.GetEntities(t => shopVersionIDArr.Contains((int)t.ShopVersionID) && t.CreateTime > beginDate && t.ShopVersionID != 32);
            }


            var shopsList = shopsListTemp.Select(t => new
            {
                ShopsID = t.ID,
                ShopVersionID = t.ShopVersionID,
                ShopName = t.ShopName,
            }).ToList();

            return Json(new { success = true, data = shopsList }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetProvince()
        //{
        //    var provinceList = ServiceHelper.GetCHAProvinceCitiesService.GetAllEntities().Select(t => new
        //    {
        //        Text = t.Province,
        //        Value = t.ProvinceCode
        //    }).ToList().DistinctBy(t => t.Text).ToList();

        //    return Json(provinceList, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetCity(string ProvinceCode)
        //{
        //    var provinceList = ServiceHelper.GetCHAProvinceCitiesService.GetEntities(t => t.ProvinceCode == ProvinceCode).Select(t => new
        //    {
        //        Text = t.City,
        //        Value = t.CityCode
        //    }).ToList().DistinctBy(t => t.Text).ToList();

        //    return Json(provinceList, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetCounty(string ProvinceCode, string CityCode)
        //{
        //    var provinceList = ServiceHelper.GetCHAProvinceCitiesService.GetEntities(t => t.ProvinceCode == ProvinceCode && t.CityCode == CityCode).Select(t => new
        //    {
        //        Text = t.County,
        //        Value = t.CountyCode
        //    }).ToList().DistinctBy(t => t.Text).ToList();

        //    return Json(provinceList, JsonRequestBehavior.AllowGet);
        //}
    }
}
