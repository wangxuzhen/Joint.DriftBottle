using Joint.DLLFactory;
using Joint.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Web.Framework
{
    public partial class ServiceHelper
    {
        public static ICommonService GetCommonService
        {
            get { return ServiceFactory.Create<ICommonService>(); }
        }        

    }
}
