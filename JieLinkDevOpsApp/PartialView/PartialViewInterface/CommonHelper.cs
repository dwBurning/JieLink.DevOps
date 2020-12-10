using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface
{
    public class CommonHelper
    {

        /// <summary> 
        /// 利用反射将DataTable转换为List<T>对象
        /// </summary> 
        /// <param name="dt">DataTable 对象</param> 
        /// <returns>List<T>集合</returns> 
        public static List<T> DataTableToList<T>(DataTable dt) where T : class, new()
        {
            // 定义集合 
            List<T> ts = new List<T>();
            //定义一个临时变量 
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行 
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性 
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性 
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量 
                                       //检查DataTable是否包含此列（列名==对象的属性名）  
                    if (dt.Columns.Contains(tempName))
                    {
                        //取值 
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性 
                        if (value != DBNull.Value)
                        {
                            if (pi.PropertyType.Name == "DateTime")
                            {
                                pi.SetValue(t, Convert.ToDateTime(value), null);
                            }
                            else if (pi.PropertyType.Name == "String")
                            {
                                pi.SetValue(t, Convert.ToString(value), null);
                            }
                            else
                            {
                                pi.SetValue(t, value, null);
                            }
                        }
                    }
                }
                //对象添加到泛型集合中 
                ts.Add(t);
            }
            return ts;
        }

        /// <summary>
        /// 日期转换
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name="dtmDefault"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeValue(object objValue, DateTime dtmDefault)
        {
            return GetDateTimeValue(objValue.ToString(), dtmDefault);
        }
        /// <summary>
        /// 日期转换
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name="dtmDefault"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeValue(string strValue, DateTime dtmDefault)
        {
            if (!string.IsNullOrWhiteSpace(strValue))
            {
                DateTime t;
                if (DateTime.TryParse(strValue, out t))
                {
                    return t;
                }
                else
                {
                    return dtmDefault;
                }
            }
            else
            {
                return dtmDefault;
            }
        }

        /// <summary>
        /// 生成20位唯一数字串，并发可用
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueId()
        {
            System.Threading.Thread.Sleep(1);
            var random = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            string uniqueId = DateTime.Now.ToString("yyMMddHHmmssffff") + random.Next(1000, 9999);
            return uniqueId;
        }
    }
}
