using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Web.Framework
{
    /// <summary>
    /// 不需要权限验证标签，加上此标签后，该Class或Method将不进行权限验证，请谨慎使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UnAuthorize : Attribute
    {
    }
}
