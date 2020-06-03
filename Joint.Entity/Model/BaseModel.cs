using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Entity
{
    [Serializable]
    public class BaseModel
    {
        public int ID { get; set; }
        public BaseModel()
        {

        }
    }

    public static class EntityHelper
    {
        ///// <summary>
        ///// 快速拷贝
        ///// </summary>
        ///// <param name="toModel">拷贝到哪个对象</param>
        ///// <param name="fromModel">拷贝的源</param>
        //public static void CopyFrom(this BaseModel toModel, BaseModel fromModel)
        //{

        //}

        ///// <summary>
        ///// 快速拷贝
        ///// </summary>
        ///// <param name="toModel">拷贝到哪个对象</param>
        ///// <param name="fromModel">拷贝的源</param>
        ///// <param name="excludeAttribute">排除的字段</param>
        //public static void CopyFrom(this BaseModel toModel, BaseModel fromModel, List<string> excludeAttribute)
        //{
        //    if (fromModel == null)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        PropertyInfo[] fromProperties = fromModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        //        //如果没有属性
        //        if (fromProperties.Length <= 0)
        //        {
        //            return;
        //        }

        //        PropertyInfo[] toProperties = fromModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        //        //遍历赋值对象的属性
        //        foreach (PropertyInfo formItem in fromProperties)
        //        {
        //            //如果该属性是被排除的话，则不需要赋值
        //            if (excludeAttribute != null && excludeAttribute.Exists(t => t.Trim() == formItem.Name))
        //            {
        //                continue;
        //            }

        //            PropertyInfo toItem = toProperties.Where(t => t.Name == formItem.Name).FirstOrDefault();
        //            if (toItem != null)
        //            {
        //                if (formItem.PropertyType.IsValueType || formItem.PropertyType.Name.StartsWith("String"))
        //                {
        //                    if (toItem.CanWrite)
        //                    {
        //                        //把值拷贝给，需要赋值的对象属性
        //                        //toItem.SetValue(fromModel, Convert.ChangeType(formItem.GetValue(fromModel, null), toItem.PropertyType), null);
        //                        toItem.SetValue(fromModel, formItem.GetValue(fromModel, null), null);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


        //public static object Copy(this object o)
        //{
        //    Type t = o.GetType();
        //    PropertyInfo[] properties = t.GetProperties();
        //    Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);
        //    foreach (PropertyInfo pi in properties)
        //    {
        //        if (pi.CanWrite)
        //        {
        //            object value = pi.GetValue(o, null);
        //            pi.SetValue(p, value, null);
        //        }
        //    }
        //    return p;
        //}
    }
}
