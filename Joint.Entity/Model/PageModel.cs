using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Joint.Entity
{
    public class PageModel<T>
    {
        public PageModel()
        {
            pagingInfo = new PagingInfo();
        }
        
        public List<T> Models { get; set; }
        public PagingInfo pagingInfo { get; set; }
    }
}
