using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Joint.Web.Areas.Admin.Models
{
    public class StorePermissionListModel
    {
        public int ID { get; set; }
        public string ShopName { get; set; }
        public string StoreName { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string IsShowWeiXin { get; set; }
        public string IsMainStore { get; set; }
    }
}