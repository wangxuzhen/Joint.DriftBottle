using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Joint.Web.Areas.Admin.Models
{
    public class BaseSearchModel
    {
        public BaseSearchModel()
        {
            PageIndex = 1;
        }

        public int PageIndex { get; set; }
    }
}