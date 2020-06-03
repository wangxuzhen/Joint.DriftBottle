using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Joint.Web.Areas.Admin.Models
{
    public class UserListModel
    {
        public int ID { get; set; }
        public string WorkNum { get; set; }
        public string RealName { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }

        //public int? WorkTypeID { get; set; }
        public string RoleName { get; set; }
        public decimal? BasicSalary { get; set; }
        public string WeiXinVisible { get; set; }
        public string Idcard { get; set; }
        public string ShopName { get; set; }
        public string StoreName { get; set; }
        public DateTime CreateTime { get; set; }
        public bool Disabled { get; set; }
    }
}