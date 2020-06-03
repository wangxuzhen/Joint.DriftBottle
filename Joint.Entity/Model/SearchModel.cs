using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Entity
{
    public class SearchModel<T>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int? CategoryType { get; set; }
        public T Model { get; set; }
    }
}
