using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Web.Framework
{
    /// <summary>
    /// 整个网站需要使用的配置项
    /// </summary>
    public abstract class BaseSettings
    {
        /// <summary>
        /// AES加密密匙
        /// </summary>
        public string AESSecretKey
        {
            get { return "JeasuSecretKey"; }
        }

        /// <summary>
        /// 网站域名
        /// </summary>
        public string SiteUrl
        {
            get { return "http://vip.iqcrj.com/"; }
            set { value = "http://vip.iqcrj.com/"; }
        }

        /// <summary>
        /// 是否自动清理缓存
        /// </summary>
        public bool AutoClearCache { get; set; }

        /// <summary>
        /// 判断是否是单机版
        /// </summary>
        public bool IsSingleVersion { get; set; }

        /// <summary>
        /// 商家电话
        /// </summary>
        public string ShopPhone { get; set; }

        /// <summary>
        /// 判断用户是否微信登陆
        /// </summary>
        public bool IsWeiXinLogin { get; set; }     

    }
}
