using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BlockSms.Core.Extension
{
    public static class EnumExtensions
    {

        /// <summary>
        /// 获取指定枚举成员的描述
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToDescriptionString(this Enum obj)
        {
            var attribs = (DescriptionAttribute[])obj.GetType().GetField(obj.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attribs.Length > 0 ? attribs[0].Description : obj.ToString();
        }
        /// <summary>
        /// 获取枚举对象列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<EnumberEntity> ToList(this Enum obj)
        {
            var list = new List<EnumberEntity>();
            foreach (var e in Enum.GetValues(obj.GetType()))//枚举转List
            {
                var m = new EnumberEntity();
                object[] objArr = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objArr != null && objArr.Length > 0)
                {
                    DescriptionAttribute da = objArr[0] as DescriptionAttribute;
                    m.Desction = da.Description;
                }
                m.EnumValue = Convert.ToInt32(e);
                m.EnumName = e.ToString();
                list.Add(m);
            }
            return list;
        }
    }
    public class EnumberEntity
    {
        /// <summary>
        /// 枚举的描述
        /// </summary>
        public string Desction { set; get; }

        /// <summary>
        /// 枚举名称
        /// </summary>
        public string EnumName { set; get; }

        /// <summary>
        /// 枚举对象的值
        /// </summary>
        public int EnumValue { set; get; }
    }
}
