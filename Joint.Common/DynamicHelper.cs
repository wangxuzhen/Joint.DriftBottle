using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Collections;
using System.Xml.Linq;
using System.Xml;
using System.Data;
using System.Reflection;

namespace Joint.Common
{
    public class DynamicHelper
    {
        /// <summary>
        /// 把Json字符串转换成Dynamic对象
        /// </summary>
        /// <param name="strJson">Json字符串</param>
        /// <returns>Dynamic对象</returns>
        public static dynamic JsonToDynamic(string strJson)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJson() });
            dynamic dyn = serializer.Deserialize(strJson, typeof(object));
            return dyn;
        }

        /// <summary>
        /// 把Xml字符串转换成Dynamic对象
        /// </summary>
        /// <param name="strXML">xml字符串</param>
        /// <returns>Dynamic对象</returns>
        public static dynamic XMLToDynamic(string strXML)
        {
            return new DynamicXml(strXML);
        }

        /// <summary>
        /// 把XML的dynamic实例转换成DataTable
        /// </summary>
        /// <param name="dy">XML的dynamic实例</param>
        /// <param name="itemName">每个行的名字</param>
        /// <returns></returns>
        public static DataTable XmlToDataTable(dynamic dy, string itemName)
        {
            string xmlStr = dy.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);

            XmlNodeList xlist = doc.SelectNodes("//" + itemName);
            DataTable dt = new DataTable();
            DataRow dr;

            for (int i = 0; i < xlist.Count; i++)
            {
                dr = dt.NewRow();
                XmlElement xe = (XmlElement)xlist.Item(i);
                for (int j = 0; j < xe.Attributes.Count; j++)
                {
                    if (!dt.Columns.Contains("@" + xe.Attributes[j].Name))
                        dt.Columns.Add("@" + xe.Attributes[j].Name);
                    dr["@" + xe.Attributes[j].Name] = xe.Attributes[j].Value;
                }
                for (int j = 0; j < xe.ChildNodes.Count; j++)
                {
                    if (!dt.Columns.Contains(xe.ChildNodes.Item(j).Name))
                        dt.Columns.Add(xe.ChildNodes.Item(j).Name);
                    dr[xe.ChildNodes.Item(j).Name] = xe.ChildNodes.Item(j).InnerText;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static DataTable XmlToDataTable(dynamic dy)
        {
            return XmlToDataTable(dy, "item");
        }


        /// <summary>
        /// DataTable转换成XML
        /// </summary>
        /// <param name="dt">DataTable对象</param>
        /// <param name="tableName">table名字</param>
        /// <returns></returns>
        public static string DataTableToXml(DataTable dt, string tableName)
        {
            return DataTableToXml(dt, tableName, "item");
        }

        /// <summary>
        /// DataTable转换成XML
        /// </summary>
        /// <param name="dt">DataTable对象</param>
        /// <param name="tableName">table名字</param>
        /// <param name="rowName">行名</param>
        /// <returns></returns>
        public static string DataTableToXml(DataTable dt, string tableName, string rowName)
        {
            string strXml = @"<" + tableName + " />";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXml);
            XmlNode root = doc.SelectSingleNode("//" + tableName);
            // 创建子节点       
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                XmlElement xe = doc.CreateElement(rowName);
                XmlElement xeChild = null;
                if (!Object.Equals(dt, null))
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (dt.Columns[i].ColumnName.StartsWith("@"))
                        {
                            string AttributeName = dt.Columns[i].ColumnName.Replace("@", "");
                            // 为该子节点设置属性       
                            xe.SetAttribute(AttributeName, dt.Rows[j][i].ToString());
                        }
                        else
                        {
                            xeChild = doc.CreateElement(dt.Columns[i].ColumnName);

                            try
                            {
                                xeChild.InnerXml = dt.Rows[j][i].ToString();
                            }
                            catch
                            {
                                xeChild.InnerText = dt.Rows[j][i].ToString();
                            }
                            xe.AppendChild(xeChild);
                        }
                    }
                }
                // 保存子节点设置       
                root.AppendChild(xe);
            }
            return doc.InnerXml.ToString();
        }



        /// <summary>
        /// Json转Dynamic类





        /// </summary>
        private class DynamicJson : JavaScriptConverter
        {
            #region 实现JavaScriptConverter的接口





            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");

                return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<Type> SupportedTypes
            {
                get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
            }
            #endregion

            #region Nested type: DynamicJsonObject

            private sealed class DynamicJsonObject : DynamicObject
            {
                private readonly IDictionary<string, object> _dictionary;

                public DynamicJsonObject(IDictionary<string, object> dictionary)
                {
                    if (dictionary == null)
                        throw new ArgumentNullException("dictionary");
                    _dictionary = dictionary;
                }

                public override string ToString()
                {
                    var sb = new StringBuilder("{");
                    ToString(sb);
                    return sb.ToString();
                }

                private void ToString(StringBuilder sb)
                {
                    var firstInDictionary = true;
                    foreach (var pair in _dictionary)
                    {
                        if (!firstInDictionary)
                            sb.Append(",");
                        firstInDictionary = false;
                        var value = pair.Value;
                        var name = pair.Key;
                        if (value is string)
                        {
                            sb.AppendFormat("{0}:'{1}'", name, value);
                        }
                        else if (value is IDictionary<string, object>)
                        {
                            new DynamicJsonObject((IDictionary<string, object>)value).ToString(sb);
                        }
                        else if (value is ArrayList)
                        {
                            sb.Append(name + ":[");
                            var firstInArray = true;
                            foreach (var arrayValue in (ArrayList)value)
                            {
                                if (!firstInArray)
                                    sb.Append(",");
                                firstInArray = false;
                                if (arrayValue is IDictionary<string, object>)
                                    new DynamicJsonObject((IDictionary<string, object>)arrayValue).ToString(sb);
                                else if (arrayValue is string)
                                    sb.AppendFormat("'{0}'", arrayValue);
                                else
                                    sb.AppendFormat("{0}", arrayValue);

                            }
                            sb.Append("]");
                        }
                        else
                        {
                            sb.AppendFormat("{0}:{1}", name, value);
                        }
                    }
                    sb.Append("}");
                }

                public override bool TryGetMember(GetMemberBinder binder, out object result)
                {
                    if (!_dictionary.TryGetValue(binder.Name, out result))
                    {
                        // return null to avoid exception.  caller can check for null this way...
                        result = null;
                        return true;
                    }

                    var dictionary = result as IDictionary<string, object>;
                    if (dictionary != null)
                    {
                        result = new DynamicJsonObject(dictionary);
                        return true;
                    }

                    var arrayList = result as ArrayList;
                    if (arrayList != null && arrayList.Count > 0)
                    {
                        if (arrayList[0] is IDictionary<string, object>)
                            result = new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)));
                        else
                            result = new List<object>(arrayList.Cast<object>());
                    }

                    return true;
                }

                public override bool TrySetMember(SetMemberBinder binder, object value)
                {
                    _dictionary[binder.Name] = value;
                    return true;
                }

            }

            #endregion
        }

        /// <summary>
        /// XML转Dynamic类





        /// </summary>
        private class DynamicXml : DynamicObject, IEnumerable
        {
            private readonly List<XElement> _elements;

            public DynamicXml(string text)
            {
                var doc = XDocument.Parse(text);
                _elements = new List<XElement> { doc.Root };
            }

            protected DynamicXml(XElement element)
            {
                _elements = new List<XElement> { element };
            }

            protected DynamicXml(IEnumerable<XElement> elements)
            {
                _elements = new List<XElement>(elements);
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;
                if (binder.Name == "Value")
                    result = _elements[0].Value;
                else if (binder.Name == "Count")
                    result = _elements.Count;
                else
                {
                    var attr = _elements[0].Attribute(
                        XName.Get(binder.Name));
                    if (attr != null)
                        result = attr;
                    else
                    {
                        var items = _elements.Descendants(
                            XName.Get(binder.Name));

                        //若取不到值则返回null
                        if (items == null || items.Count() == 0)
                        {
                            result = null;
                            return true;
                        }

                        result = new DynamicXml(items);
                    }
                }
                return true;
            }


            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                int ndx = (int)indexes[0];
                result = new DynamicXml(_elements[ndx]);
                return true;
            }


            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                var attr = _elements[0].Attribute(
                      XName.Get(binder.Name));
                if (attr != null)
                    attr.Value = value.ToString();
                else
                {
                    var items = _elements.Descendants(
                        XName.Get(binder.Name));
                    if (items == null || items.Count() == 0)
                        return false;
                    foreach (var item in items)
                    {
                        item.Value = value.ToString();
                    }
                }

                //var a = binder.Name;
                return true;
            }

            public override string ToString()
            {
                return _elements[0].ToString();
            }

            /// <summary>
            /// 实现IEnumerable接口
            /// </summary>
            /// <returns></returns>
            public IEnumerator GetEnumerator()
            {
                foreach (var element in _elements)
                    yield return new DynamicXml(element);
            }



        }

        /// <summary>  
        /// DataSetToList  
        /// </summary>  
        /// <typeparam name="T">转换类型</typeparam>  
        /// <param name="ds">一个DataSet实例，也就是数据源</param>  
        /// <param name="tableIndext">DataSet容器里table的下标，只有用于取得哪个table，也就是需要转换表的索引</param>  
        /// <returns></returns>  
        public static List<T> DataSetToList<T>(DataSet ds, int tableIndext)
        {
            //确认参数有效  
            if (ds == null || ds.Tables.Count <= 0 || tableIndext < 0)
            {
                return null;
            }
            DataTable dt = ds.Tables[tableIndext]; //取得DataSet里的一个下标为tableIndext的表，然后赋给dt  

            IList<T> list = new List<T>();  //实例化一个list  
            // 在这里写 获取T类型的所有公有属性。 注意这里仅仅是获取T类型的公有属性，不是公有方法，也不是公有字段，当然也不是私有属性                                                 
            PropertyInfo[] tMembersAll = typeof(T).GetProperties();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //创建泛型对象。为什么这里要创建一个泛型对象呢？是因为目前我不确定泛型的类型。  
                T t = Activator.CreateInstance<T>();


                //获取t对象类型的所有公有属性。但是我不建议吧这条语句写在for循环里，因为没循环一次就要获取一次，占用资源，所以建议写在外面  
                //PropertyInfo[] tMembersAll = t.GetType().GetProperties();  


                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //遍历tMembersAll  
                    foreach (PropertyInfo tMember in tMembersAll)
                    {
                        //取dt表中j列的名字，并把名字转换成大写的字母。整条代码的意思是：如果列名和属性名称相同时赋值  
                        if (dt.Columns[j].ColumnName.ToUpper().Equals(tMember.Name.ToUpper()))
                        {
                            //dt.Rows[i][j]表示取dt表里的第i行的第j列；DBNull是指数据库中当一个字段没有被设置值的时候的值，相当于数据库中的“空值”。   
                            if (dt.Rows[i][j] != DBNull.Value)
                            {
                                //SetValue是指：将指定属性设置为指定值。 tMember是T泛型对象t的一个公有成员，整条代码的意思就是：将dt.Rows[i][j]赋值给t对象的tMember成员,参数详情请参照http://msdn.microsoft.com/zh-cn/library/3z2t396t(v=vs.100).aspx/html  

                                tMember.SetValue(t, dt.Rows[i][j], null);


                            }
                            else
                            {
                                tMember.SetValue(t, null, null);
                            }
                            break;//注意这里的break是写在if语句里面的，意思就是说如果列名和属性名称相同并且已经赋值了，那么我就跳出foreach循环，进行j+1的下次循环  
                        }
                    }
                }

                list.Add(t);
            }
            return list.ToList();

        } 

    }
}
