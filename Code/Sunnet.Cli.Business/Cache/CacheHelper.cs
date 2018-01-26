using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/27 2:47:13
 * Description:		Please input class summary
 * Version History:	Created,2014/10/27 2:47:13
 * 
 * 
 **************************************************************************/
using System.Web;
using System.Web.Caching;
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Business
{

    public static class CacheHelper
    {
        /// <summary>
        /// 更改缓存时保证单线程修改
        /// </summary>
        internal static readonly object Synchronize = new object();

        /// <summary>
        /// 默认的缓存过期时间
        /// </summary>
        internal const int DefaultExpiredSeconds = 1 * 30 * 60;

        internal const int Student_View_ExpiredSeconds = 1 * 30 * 60;

        static Dictionary<string, DateTime> TimeOfAdded { get; set; }
        static Dictionary<string, DateTime> TimeOfExpired { get; set; }
        static Dictionary<string, int> CounterOfKey { get; set; }
        static CacheHelper()
        {
            TimeOfAdded = new Dictionary<string, DateTime>();
            TimeOfExpired = new Dictionary<string, DateTime>();
            CounterOfKey = new Dictionary<string, int>();
        }

        private static System.Web.Caching.Cache CacheProvider
        {
            get { return HttpRuntime.Cache; }
        }

        private static void CheckKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new NullReferenceException("key can not be null");
            }
            if (TimeOfExpired != null && TimeOfExpired.ContainsKey(key) && DateTime.Now > TimeOfExpired[key])
            {
                CacheProvider.Remove(key);
            }
        }


        /// <summary>
        /// 设置需要定时过期的 Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="duration"></param>
        private static void EnsureTimeofKey(string key, int duration)
        {
            if (TimeOfExpired.ContainsKey(key))
                TimeOfExpired[key] = DateTime.Now.AddSeconds(duration);
            else
                TimeOfExpired.Add(key, DateTime.Now.AddSeconds(duration));
        }

        /// <summary>
        /// 清空或重置缓存计数器
        /// </summary>
        /// <param name="key"></param>
        private static void ClearCounter(string key)
        {
            if (CounterOfKey.ContainsKey(key))
                CounterOfKey[key] = 0;
            else
                CounterOfKey.Add(key, 0);
        }

        /// <summary>
        /// 使缓存的计数器加一.
        /// </summary>
        /// <param name="key">The key.</param>
        public static void Counter(string key)
        {
            if (CounterOfKey.ContainsKey(key))
                CounterOfKey[key] = CounterOfKey[key] + 1;
            else
                CounterOfKey.Add(key, 1);
        }

        /// <summary>
        /// 添加缓存项：<paramref name="key"/> -> <paramref name="value"/>.永不过期,需要维护
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Add(string key, object value)
        {
            lock (Synchronize)
            {
                CheckKey(key);
                ClearCounter(key);
                if (value != null)
                    CacheProvider.Insert(key, value);
            }
        }

        /// <summary>
        /// 添加缓存项：<paramref name="key"/> -> <paramref name="value"/>.定时过期
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="duration">自动过期时间：<paramref name="duration"/>(s).</param>
        public static void Add(string key, object value, int duration)
        {
            lock (Synchronize)
            {
                CheckKey(key);
                EnsureTimeofKey(key, duration);
                ClearCounter(key);
                if (value != null)
                    CacheProvider.Insert(key, value);
            }
        }

        private static object Get(string key)
        {
            CheckKey(key);
            return CacheProvider.Get(key);
        }

        public static T Get<T>(string key) where T : class
        {
            var value = Get(key) as T;
            return value;
        }

        public static void Remove(string key)
        {
            lock (Synchronize)
            {
                CacheProvider.Remove(key);
            }
        }

        #region
        private static DateTime _measuresUpdatedOn;
        private static List<MeasureEntity> Measures;
        private static List<MeasureEntity> GetMeasuresFromMem()
        {
            var expired = _measuresUpdatedOn <= DateTime.Now.AddMilliseconds(5 * 60 * 1000);

            if (Measures == null || expired)
            {
                Measures = null;
                lock (Measures)
                {
                    if (Measures == null)
                        Measures = GetMeasuresFromDb();
                }
            }
            return Measures;
        }
        private static List<MeasureEntity> GetMeasuresFromDb()
        {
            _measuresUpdatedOn = DateTime.Now;
            // code get from db;
            return new List<MeasureEntity>();
        }
        #endregion

    }
}
