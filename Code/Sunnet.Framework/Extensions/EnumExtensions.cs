using Sunnet.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Sunnet.Framework.Extensions
{
    /// <summary>
    ///     枚举扩展方法类
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        ///     获取枚举项的Description特性的描述文字
        /// </summary>
        /// <param name="enumeration"> </param>
        /// <returns> </returns>
        public static string ToDescription(this Enum enumeration)
        {
            Type type = enumeration.GetType();
            MemberInfo[] members = type.GetMember(enumeration.CastTo<string>());
            if (members.Length > 0 && members[0].GetCustomAttributes(typeof(DescriptionAttribute), true).Any())
            {
                return members[0].ToDescription();
            }
            return enumeration.CastTo<string>();
        }

        /// <summary>
        ///     获取枚举项的DisplayNameAttribute特性的描述文字
        /// </summary>
        /// <param name="enumeration"> </param>
        /// <returns> </returns>
        public static string GetDisplayName(this Enum enumeration)
        {
            Type type = enumeration.GetType();
            MemberInfo[] members = type.GetMember(enumeration.CastTo<string>());
            if (members.Length > 0 && members[0].GetCustomAttributes(typeof(DisplayAttribute), true).Any())
            {
                var displayNameAttr = members[0].GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault();
                if (displayNameAttr != null)
                    return displayNameAttr.Name;
            }
            return enumeration.CastTo<string>();
        }


        /// <summary>
        /// 将枚举<paramref name="enumValue"/>转为 IEnumerable&lt;SelectListItem&gt; ，并设置选中项 (下拉框的Value 值为枚举值)
        /// </summary>
        /// <param name="enumValue">需要选中的枚举值</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> ToSelectList(this Enum enumValue)
        {
            return from Enum e in Enum.GetValues(enumValue.GetType())
                   select new SelectListItem
                   {
                       Selected = e.Equals(enumValue),
                       Text = e.ToDescription(),
                       Value = e.GetValue().ToString()
                   };
        }

        /// <summary>
        /// 将枚举<paramref name="enumType"/>转为 IEnumerable&lt;SelectListItem&gt; (下拉框的Value 值为 枚举值对应的数值)
        /// </summary>
        /// <param name="enumType">枚举</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> ToSelectList(this Type enumType)
        {
            return from Enum e in Enum.GetValues(enumType)
                   select new SelectListItem
                   {
                       Text = e.ToDescription(),
                       Value = e.GetValue().ToString()
                   };
        }


        /// <summary>
        /// 将枚举<paramref name="enumValue"/>转为 IEnumerable&lt;SelectListItem&gt;
        /// </summary>
        /// 
        /// <param name="enumValue">枚举</param>
        /// <param name="showIntValue">下拉框的值，显示枚举对应的数值</param>
        /// <param name="option">添加inserDefault</param>
        /// <returns>value | text</returns>
        public static IEnumerable<SelectListItem> ToSelectList(this Enum enumValue, bool showIntValue
            , SelectOptions option = null)
        {
            IEnumerable<SelectListItem> enums = null;
            if (showIntValue)
                enums = from Enum e in Enum.GetValues(enumValue.GetType())
                        select new SelectListItem
                        {
                            Selected = e.Equals(enumValue),
                            Text = e.ToDescription(),
                            Value = e.GetValue().ToString()
                        };
            else
                enums = from Enum e in Enum.GetValues(enumValue.GetType())
                        select new SelectListItem
                        {
                            Selected = e.Equals(enumValue),
                            Text = e.ToDescription(),
                            Value = e.ToString()
                        };

            List<SelectListItem> list = enums.ToList<SelectListItem>();
            if (option != null && option.InsertDefault)
            {
                list.ForEach(x => x.Selected = false);
                list.Insert(0, new SelectListItem() { Text = option.Text, Value = option.Valu.ToString() });
            }
            return list;
        }

        public static int GetValue(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// 将枚举<paramref name="enumValue"/>转为 IEnumerable&lt;Enum&gt;
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Enum> ToList(this Enum enumValue)
        {
            return from Enum e in Enum.GetValues(enumValue.GetType())
                   select e;
        }

        /// <summary>
        /// Adds the default item.
        /// </summary>
        /// <param name="listItems">The list items.</param>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index of the default item.</param>
        /// <returns>The source list</returns>
        public static IEnumerable<SelectListItem> AddDefaultItem(this IEnumerable<SelectListItem> listItems,
            string text, object value, int index = 0)
        {
            var selectListItems = listItems as List<SelectListItem> ?? listItems.ToList();
            selectListItems.ForEach(x => x.Selected = false);
            selectListItems.Insert(index,
                new SelectListItem() { Text = text, Value = value.ToString(), Selected = true });
            return selectListItems;
        }

        /// <summary>
        /// 将一个或多个枚举常数的名称或数字值的字符串表示转换成等效的枚举对象.
        /// </summary>
        /// <typeparam name="T">要转换的枚举类型</typeparam>
        /// <param name="value">值或者枚举的字符串.</param>
        /// <returns></returns>
        /// Author  :  Jack Zhang (JACKZ)
        /// Date    :  4/24 16:14
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
