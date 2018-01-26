using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sunnet.Framework.Extensions
{
    /// <summary>
    ///     集合扩展方法类
    /// </summary>
    public static class CollectionsExtensions
    {
        #region IEnumerable的扩展

        /// <summary>
        ///     将集合展开并分别转换成字符串，再以指定的分隔符衔接，拼成一个字符串返回
        /// </summary>
        /// <param name="collection"> 要处理的集合 </param>
        /// <param name="separator"> 分隔符 </param>
        /// <returns> 拼接后的字符串 </returns>
        public static string ExpandAndToString<T>(this IEnumerable<T> collection, string separator)
        {
            List<T> source = collection as List<T> ?? collection.ToList();
            if (source.IsEmpty())
            {
                return null;
            }
            string result = source.Cast<object>().Aggregate<object, string>(null, (current, o) => current + string.Format("{0}{1}", o, separator));
            return result.Substring(0, result.Length - separator.Length);
        }

        /// <summary>
        ///     集合是否为空
        /// </summary>
        /// <param name="collection"> 要处理的集合 </param>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <returns> 为空返回True，不为空返回False </returns>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return !collection.Any();
        }

        /// <summary>
        ///     根据第三方条件是否为真来决定是否执行指定条件的查询
        /// </summary>
        /// <param name="source"> 要查询的源 </param>
        /// <param name="predicate"> 查询条件 </param>
        /// <param name="condition"> 第三方条件 </param>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <returns> 查询的结果 </returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool condition)
        {
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        ///     根据指定条件返回集合中不重复的元素
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <typeparam name="TKey">动态筛选条件类型</typeparam>
        /// <param name="source">要操作的源</param>
        /// <param name="keySelector">重复数据筛选条件</param>
        /// <returns>不重复元素的集合</returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Select(group => group.First());
        }

        /// <summary>
        /// 内存排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">Source list</param>
        /// <param name="order">Order Property</param>
        /// <param name="direction">Order Direction:ASC | DESC</param>
        /// <returns></returns>
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, string order, string direction)
        {
            bool descending = false;
            string property = "";

            if (order.Length > 0 && order != "")
            {
                property = order;
                descending = direction.ToLower().Contains("desc");
                System.Reflection.PropertyInfo prop = typeof(T).GetProperty(property);
                if (prop == null)
                {
                    throw new Exception("No property '" + property + "' in + " + typeof(T).Name + "'");
                }
                if (descending)
                    return list.OrderByDescending(x => prop.GetValue(x, null));
                else
                    return list.OrderBy(x => prop.GetValue(x, null));
            }
            return list;
        }
        #endregion    
    }
}
