using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Joint.Web.Areas.Admin.Models
{
    public class RoleSearchModel : BaseSearchModel
    {

        private string roleName;
        public string RoleName
        {
            get { return roleName; }
            set { roleName = value; }
        }

        private string disabled;

        public string Disabled
        {
            get { return disabled; }
            set { disabled = value; }
        }

        private string _startDate;

        public string startDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        private string _endDate;

        public string endDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
    }
}