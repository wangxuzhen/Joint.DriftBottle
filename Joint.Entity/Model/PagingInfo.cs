using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Entity
{
    public class PagingInfo
    {
        /// <summary>
        /// 总数据条数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((decimal)((decimal)this.Total / (decimal)this.PageSize));
            }
        }
    }
}
