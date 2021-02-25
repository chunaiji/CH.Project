using Castle.Core.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.RedisCommont
{
    public class StackExchangeRedisHelper
    {
        private static readonly object Locker = new object();

        private ConnectionMultiplexer redisMultiplexer;


        public virtual string RedisConnection { get; set; }

        IDatabase db = null;

        private static StackExchangeRedisHelper _redisClient = null;

        public static StackExchangeRedisHelper redisClient
        {
            get
            {
                if (_redisClient == null)
                {
                    lock (Locker)
                    {
                        if (_redisClient == null)
                        {
                            _redisClient = new StackExchangeRedisHelper();

                        }
                    }
                }

                return _redisClient;
            }
        }

        /// <summary>
        /// "password=CHU383039284,139.199.190.97:16379,allowAdmin=true"
        /// </summary>
        /// <param name="redisConnection"></param>
        public void InitConnect(string redisConnection = "password=123456,47.107.180.18:6379,allowAdmin=true")
        {
            try
            {
                this.RedisConnection = redisConnection;
                redisMultiplexer = ConnectionMultiplexer.Connect(RedisConnection);
                db = redisMultiplexer.GetDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                redisMultiplexer = null;
                db = null;
            }
        }

        public StackExchangeRedisHelper()
        {

        }


        #region String

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        public bool SetStringKey(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            return db.StringSet(key, value, expiry);
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        public RedisValue GetStringKey(string key)
        {
            return db.StringGet(key);
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        public T GetStringKey<T>(string key)
        {
            if (db == null)
            {
                return default;
            }
            if (db.KeyExists(new RedisKey(key) { }))
            {
                var value = db.StringGet(key);
                if (value.IsNullOrEmpty)
                {
                    return default;
                }
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <param name="obj"></param>
        public bool SetStringKey<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            if (db == null)
            {
                return false;
            }

            string json = JsonConvert.SerializeObject(obj);
            return db.StringSet(key, json, expiry);
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="redisChannel"></param>
        public void Subscriber(string redisChannel = "messages")
        {
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(this.RedisConnection))
            {
                ISubscriber sub = redis.GetSubscriber();
                //订阅名为 messages 的通道
                sub.Subscribe(redisChannel, (channel, message) =>
                {
                    //输出收到的消息
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
                });
                Console.WriteLine("已订阅 messages");
            }
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="redisChannel"></param>
        public void Publish(string sendData, string redisChannel = "messages")
        {
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(this.RedisConnection))
            {
                ISubscriber sub = redis.GetSubscriber();
                sub.Publish(redisChannel, sendData);
            }
        }
        #endregion
    }
}
