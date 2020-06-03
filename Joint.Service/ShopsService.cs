using Joint.Common;
using Joint.DLLFactory;
using Joint.Entity;
using Joint.IService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Joint.Service
{
    public partial class ShopsService : BaseService<Shops>, IShopsService
    {
        /// <summary>
        /// 根据商家ID来获取域名
        /// </summary>
        /// <param name="shopID"></param>
        /// <returns></returns>
        public string GetDomain(int shopID)
        {
            //ICommonService commonService = ServiceFactory.Create<ICommonService>();
            var shop = new ShopsService().GetEntity(shopID); //commonService.GetShopsInfo(shopID);
            //var shop = GetEntity(shopID);
            if (shop == null)
            {
                return WebConfigurationManager.AppSettings["WebSiteUrl"];
            }
            if (string.IsNullOrWhiteSpace(shop.Domain))
            {
                //如果是美发版的返回美发版
                if (shop.ShopType == 2)
                {
                    return WebConfigurationManager.AppSettings["MfWebSiteUrl"];
                }
                else
                {
                    return WebConfigurationManager.AppSettings["WebSiteUrl"];
                }
            }
            else
            {
                string strDomain = shop.Domain;
                if (!strDomain.StartsWith("http://"))
                {
                    strDomain = "http://" + strDomain;
                }
                if (!strDomain.EndsWith("/"))
                {
                    strDomain += "/";
                }
                return strDomain;
            }
        }

    }
}
