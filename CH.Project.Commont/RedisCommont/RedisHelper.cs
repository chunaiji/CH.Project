using NewLife.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.RedisCommont
{
    public class RedisHelper
    {
        protected virtual string ServerName { get; set; }
        protected virtual string Password { get; set; }
        protected virtual int DBIndex { get; set; } = 0;
        protected static Redis RedisClient { get; set; }

        public static RedisHelper Instantiation = new RedisHelper();
        private RedisHelper()
        {

        }

        public RedisHelper CreateInstantiation()
        {
            if (Instantiation == null)
            {
                Instantiation = new RedisHelper();
            }
            return Instantiation;
        }

        public Redis GetRedisClient()
        {
            if (RedisClient == null)
            {
                RedisClient = new Redis(ServerName, Password, DBIndex); ;
            }
            return RedisClient;
        }

        public void Set<T>(string key, T value, int timeOut = 3600 * 24)
        {
            RedisClient.Set(key, value, timeOut);
        }

        public void Get<T>(string key)
        {
            RedisClient.Get<T>(key);
        }

        public bool Remove<T>(string key)
        {
            return RedisClient.Remove(key) > 0;
        }

        public bool Removes<T>(string[] key)
        {
            return RedisClient.Remove(key) > 0;
        }
        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire"></param>
        public void SetExpire(string key, TimeSpan expire)
        {
            RedisClient.SetExpire(key, expire);
        }
        /// <summary>
        /// 批量获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            return RedisClient.GetAll<T>(keys);
        }
        /// <summary>
        /// 增量累加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long Increment(string key, long value)
        {
            return RedisClient.Increment(key, value);
        }
        /// <summary>
        /// 增量累加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double IncrementDouble(string key, double value)
        {
            return RedisClient.Increment(key, value);
        }
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return RedisClient.ContainsKey(key);
        }
    }
}
