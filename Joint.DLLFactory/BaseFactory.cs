using Joint.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Joint.DLLFactory
{
    public abstract class BaseFactory
    {
        public static Assembly LoadAssembly(string assemblyName)
        {
            //从缓存载入DLL，若有返回，否则创建
            object obj = DataCache.GetCache(assemblyName);
            if (obj == null)
            {
                obj = Assembly.Load(assemblyName);
                DataCache.SetCache(assemblyName, obj);
            }
            return (Assembly)obj;
        }

        /// <summary>
        /// 通过反射创建对象
        /// </summary>
        /// <param name="assemblyPath">要加载程序集的名称</param>
        /// <param name="classFullName">完整的类名(包括命名空间)</param>
        /// <returns>反射产生的程序集实例</returns>
        public static object CreateObject(string assemblyName, string classFullName)
        {
            return LoadAssembly(assemblyName).CreateInstance(classFullName);
        }

        /// <summary>
        /// 创建数据层接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static T Create<T>(string assemblyName)
        {
            Type type = typeof(T);
            string iClassName = type.Name;
            //去除接口类名的第一个字母（I），就可以得到DLL的对应类名
            string className = iClassName.Substring(1, iClassName.Length - 1);
            string classFullName = assemblyName + "." + className;
            object obj = CreateObject(assemblyName, classFullName);
            return (T)obj;
        }
    }
}
