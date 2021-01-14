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
                            else if (pi.PropertyType.Name == "Int32")
                            {
                                if (value.ToString().ToLower() == "true")
                                {
                                    value = 1;
                                }
                                else if (value.ToString().ToLower() == "false")
                                {
                                    value = 0;
                                }
                                pi.SetValue(t, int.Parse(value.ToString()), null);
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

        /// <summary>
        /// Int32类型转换
        /// </summary>
        /// <param name="objValue"></param>=
        /// <returns></returns>
        public static int GetIntValue(object objValue, int intDefault = 0)
        {
            if (objValue == null)
            {
                return intDefault;
            }
            return GetIntValue(objValue.ToString(), intDefault);
        }

        /// <summary>
        /// Int32类型转换
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name="dtmDefault"></param>
        /// <returns></returns>
        public static int GetIntValue(string strValue, int intDefault)
        {
            if (string.IsNullOrWhiteSpace(strValue))
            {
                return intDefault;
            }
            int iValue = 0;
            if (!int.TryParse(strValue, out iValue))
            {
                iValue = intDefault;
            }
            return iValue;
        }
        /// <summary>
        /// Int32类型转换
        /// </summary>
        /// <param name="objValue"></param>=
        /// <returns></returns>
        public static uint GetUIntValue(object objValue, uint uintDefault = 0)
        {
            if (objValue == null)
            {
                return uintDefault;
            }
            return GetUIntValue(objValue.ToString(), uintDefault);
        }

        /// <summary>
        /// Int32类型转换
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name="dtmDefault"></param>
        /// <returns></returns>
        public static uint GetUIntValue(string strValue, uint uintDefault)
        {
            if (string.IsNullOrWhiteSpace(strValue))
            {
                return uintDefault;
            }
            uint iValue = 0;
            if (!uint.TryParse(strValue, out iValue))
            {
                iValue = uintDefault;
            }
            return iValue;
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        public static long GetTimeStamp()
        {
            DateTime d = DateTime.Now;
            TimeSpan ts = d.ToUniversalTime() - new DateTime(1970, 1, 1);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// MAC后3位加00 生成设备Id
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string ConvertDevicesId(string mac = "00-00-00-00-00-00-00-00", string index = "")
        {
            string empty = string.Empty;
            if (string.IsNullOrEmpty(mac))
            {
                return empty;
            }
            mac = mac.Replace("-", "").Replace(":", "").Substring(6, 6);
            mac = (string.IsNullOrEmpty(index) ? (mac + "00") : (mac + index));
            return Convert.ToUInt32(mac, 16).ToString();
        }
    }
}
