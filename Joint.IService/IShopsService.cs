using Joint.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.IService
{
    public partial interface IShopsService : IBaseService<Shops>
    {
        /// <summary>
        /// 根据商家ID来获取域名
        /// </summary>
        /// <param name="shopID"></param>
        /// <returns></returns>
        string GetDomain(int shopID);
    }
}
