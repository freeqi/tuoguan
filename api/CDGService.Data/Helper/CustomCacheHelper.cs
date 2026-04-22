using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CDGService.Data.Helper
{
    /// <summary>
    /// 自定义内存缓存助手
    /// </summary>
    public sealed class CustomCacheHelper
    {
        #region 单例模式
        //创建私有化静态obj锁  
        private static readonly object _ObjLock = new object();
        //创建私有静态字段，接收类的实例化对象  
        private static volatile CustomCacheHelper _CustomCacheHelper = null;
        //构造函数私有化  
        private CustomCacheHelper() { }
        //创建单利对象资源并返回  
        public static CustomCacheHelper GetSingleObj()
        {
            if (_CustomCacheHelper == null)
            {
                lock (_ObjLock)
                {
                    if (_CustomCacheHelper == null)
                    {
                        _CustomCacheHelper = new CustomCacheHelper();
                    }
                }
            }
            return _CustomCacheHelper;
        }
        #endregion

        /// <summary>
        /// 缓存字典 => 【key|value|time】
        /// </summary>
        private static ConcurrentDictionary<string, KeyValuePair<object, DateTime?>> _CacheDictionary = new ConcurrentDictionary<string, KeyValuePair<object, DateTime?>>();

        /// <summary>
        /// 1.主动过期
        /// </summary>
        static CustomCacheHelper()
        {
            Task.Run(() => {
                while (true)
                {
                    Thread.Sleep(1000 * 10); //10s检查一次
                    if (_CacheDictionary != null)
                    {
                        if (_CacheDictionary.Keys.Count > 0)
                        {
                            var keys = new List<string>();
                            foreach (var key in _CacheDictionary.Keys)
                            {
                                keys.Add(key);
                            }

                            foreach (var key in keys)
                            {
                                var valueTime = _CacheDictionary[key];
                                if (valueTime.Value < DateTime.Now)
                                {
                                    _CacheDictionary.TryRemove(key,out KeyValuePair<object, DateTime?> value); //Remove(key);
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key">key索引</param>
        /// <returns>object</returns>
        public object this[string key]
        {
            get => _CacheDictionary[key];
            set => _CacheDictionary[key] = new KeyValuePair<object, DateTime?>(value, null);
        }

        /// <summary>
        /// 设置相对过期缓存(默认单位s)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">数据包</param>
        /// <param name="seconds">相对过期时间</param>
        public void SetForSeconds(string key, object data, int seconds)
        {
            _CacheDictionary[key] = new KeyValuePair<object, DateTime?>(data, DateTime.Now.AddSeconds(seconds));
        }

        public void SetForMinute(string key, object data, int minutes) 
        {
            _CacheDictionary[key] = new KeyValuePair<object, DateTime?>(data, DateTime.Now.AddMinutes(minutes));
        }

        public void SetForHours(string key, object data, int hours) 
        {
            _CacheDictionary[key] = new KeyValuePair<object, DateTime?>(data, DateTime.Now.AddHours(hours));
        }

        public void SetForDate(string key, object data, DateTime dateTime)  
        {
            _CacheDictionary[key] = new KeyValuePair<object, DateTime?>(data, dateTime);
        }

        /// <summary>
        /// 设置绝对过期缓存
        /// </summary>
        /// <param name="key">键<</param>
        /// <param name="data">数据包</param>
        public void Set(string key, object data)
        {
            _CacheDictionary[key] = new KeyValuePair<object, DateTime?>(data, null);
            //this[key] = data; 
        }

        /// <summary>
        /// 通过key获取缓存value
        /// 2.被动过期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (Exist(key))
            {
                var valueTime = _CacheDictionary[key];
                return (T)valueTime.Key;  //return (T)this[key];
            }
            else
            {
                return default(T);
            }
        }


        /// <summary>
        /// 获取缓存个数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            int count = 0;
            if (_CacheDictionary != null)
                count = _CacheDictionary.Count;
            return count;
        }

        /// <summary>
        /// 删除指定key的value
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (Exist(key))
                _CacheDictionary.TryRemove(key, out KeyValuePair<object, DateTime?> value);
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void Cleaner()
        {
            if (_CacheDictionary != null && _CacheDictionary.Count > 0)
            {
                _CacheDictionary.Clear();

                //foreach (var key in _CacheDictionary.Keys)
                //{
                //    _CacheDictionary.TryRemove(key, out KeyValuePair<object, DateTime?> value);
                //}
            }
        }

        /// <summary>
        /// 检查key是否存在
        /// 2.被动过期
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exist(string key)
        {
            if (!string.IsNullOrWhiteSpace(key) && _CacheDictionary.ContainsKey(key))
            {
                var valTime = _CacheDictionary[key];
                if (valTime.Value != null && valTime.Value > DateTime.Now)
                {
                    return true; //缓存没过期
                }
                else
                {
                    _CacheDictionary.TryRemove(key, out KeyValuePair<object, DateTime?> value); //缓存过期清理
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
