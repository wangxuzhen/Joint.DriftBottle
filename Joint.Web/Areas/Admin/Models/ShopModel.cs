using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Joint.Web.Areas.Admin.Models
{
    public class ShopModel
    {
        #region 商店信息
        public int ID { get; set; }
        public string ShopName { get; set; }
        public int AdminUserID { get; set; }
        public int CreateUserID { get; set; }
        public DateTime CreateTime { get; set; }
        public bool Disabled { get; set; }
        public string Domain { get; set; }
        public string DomainName { get; set; }
        /// <summary>
        /// 商家版本，初始购买什么版本的程序ID
        /// </summary>
        public int ShopVersionID { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalMoney { get; set; }
        public string Remark { get; set; }

        /// <summary>
        /// 商家版本
        /// </summary>
        public int? ShopType { get; set; }

        /// <summary>
        /// 销售员
        /// </summary>
        public int? SalespersonID { get; set; }

        /// <summary>
        /// 首付
        /// </summary>
        public decimal? Deposit { get; set; }

        /// <summary>
        /// 尾款
        /// </summary>
        public decimal? FinalPayment { get; set; }

        //商家ID
        public int? ShopID { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        
        /// <summary>
        /// 省份代码
        /// </summary>
        public string ProvinceCode { get; set; }
        
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        
        /// <summary>
        /// 城市
        /// </summary>
        public string CityCode { get; set; }


        /// <summary>
        /// 县
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// 县
        /// </summary>
        public string CountyCode { get; set; } 

        /// <summary>
        /// 售后维护人员
        /// </summary>
        public int? AfterSales { get; set; }

        /// <summary>
        /// 商家Logo
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// 商家名称
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// 年费
        /// </summary>
        public decimal? AnnualFee { get; set; }


        #endregion

        #region 账号信息

        public string RealName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Idcard { get; set; }
        public string Phone { get; set; }
        #endregion
    }
}