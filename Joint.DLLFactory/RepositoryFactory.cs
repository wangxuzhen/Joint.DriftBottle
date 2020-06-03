using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.DLLFactory
{
    public class RepositoryFactory : BaseFactory
    {
        /// <summary>
        /// 从web.config中获取数据访问层的程序集名称,赋值给常量ASSEMBLYNAME
        /// </summary>
        private static readonly string ASSEMBLYNAME = ConfigurationManager.AppSettings["Repository"];

        /// <summary>
        /// 创建反射创建数据层接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>()
        {
            return Create<T>(ASSEMBLYNAME);
        }
    }
}
