//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Joint.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Shops : BaseModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Shops()
        {
            this.RelationPrivilegesShops = new HashSet<RelationPrivilegesShops>();
            this.RelationShopsModule = new HashSet<RelationShopsModule>();
            this.Role = new HashSet<Role>();
            this.Stores = new HashSet<Stores>();
        }
    
        public int ID { get; set; }
        public string ShopName { get; set; }
        public Nullable<int> ShopVersionID { get; set; }
        public string Domain { get; set; }
        public string DomainName { get; set; }
        public System.DateTime DueDate { get; set; }
        public decimal TotalMoney { get; set; }
        public string Remark { get; set; }
        public int AdminUserID { get; set; }
        public string OriginID { get; set; }
        public string AppID { get; set; }
        public string AppSecret { get; set; }
        public string MCHID { get; set; }
        public string MCHKey { get; set; }
        public string WeiXinAccount { get; set; }
        public string WeiXinPassword { get; set; }
        public string MerchantAccount { get; set; }
        public string MerchantPassword { get; set; }
        public string Operator { get; set; }
        public string OperatorContact { get; set; }
        public string QRCode { get; set; }
        public string NoticeOpenID { get; set; }
        public string NoticeNickName { get; set; }
        public int CreateUserID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public bool Disabled { get; set; }
        public string WeiXinBackground { get; set; }
        public string AttentionReply { get; set; }
        public Nullable<int> ShopType { get; set; }
        public Nullable<int> SalespersonID { get; set; }
        public Nullable<decimal> Deposit { get; set; }
        public Nullable<decimal> FinalPayment { get; set; }
        public string AuthorizerAppId { get; set; }
        public string AuthorizerRefreshToken { get; set; }
        public Nullable<System.DateTime> RefreshTokenTime { get; set; }
        public Nullable<bool> EnableWeixinPay { get; set; }
        public string AttentionMediaID { get; set; }
        public Nullable<int> AfterSales { get; set; }
        public string Province { get; set; }
        public string ProvinceCode { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public string County { get; set; }
        public string CountyCode { get; set; }
        public string LogoUrl { get; set; }
        public string SiteName { get; set; }
        public Nullable<decimal> AnnualFee { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelationPrivilegesShops> RelationPrivilegesShops { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelationShopsModule> RelationShopsModule { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Role> Role { get; set; }
        public virtual ShopVersion ShopVersion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stores> Stores { get; set; }
    }
}
