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
    
    public partial class WeiXinUser : BaseModel
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string OpenID { get; set; }
        public Nullable<double> lng { get; set; }
        public Nullable<double> lat { get; set; }
        public string AllPosition { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string Subscribe { get; set; }
        public string Nickname { get; set; }
        public Nullable<short> Sex { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Headimgurl { get; set; }
        public string Subscribe_time { get; set; }
        public Nullable<short> State { get; set; }
        public string JQUnitCode { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    }
}
