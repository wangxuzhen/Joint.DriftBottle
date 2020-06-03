using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Common
{
    public  class IpInfoModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
    }

    public class Data
    {
        /// <summary>
        /// 中国
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string country_id { get; set; }

        /// <summary>
        /// 华北
        /// </summary>
        public string area { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string area_id { get; set; }

        /// <summary>
        /// 北京市
        /// </summary>
        public string region { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string region_id { get; set; }

        /// <summary>
        /// 北京市
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string city_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string county { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string county_id { get; set; }

        /// <summary>
        /// 联通
        /// </summary>
        public string isp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string isp_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ip { get; set; }

    }
}
