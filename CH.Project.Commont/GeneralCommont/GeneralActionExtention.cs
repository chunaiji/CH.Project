using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CommentProject.CommentExtention.GeneralExtention
{
    public class GeneralActionExtention
    {
        /// <summary>
        /// 日期转换为时间戳（时间戳单位秒）
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static long ConvertToTimeStamp(DateTime timeStamp)
        {
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(timeStamp.AddHours(-8) - Jan1st1970).TotalSeconds;
        }

        /// <summary>
        /// 时间戳转换为日期（时间戳单位秒）
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timeStamp)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return start.AddSeconds(timeStamp).AddHours(8);
        }

        #region 时间戳

        /// <summary>
        /// 时间转Long型时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimeStampLong(DateTime? dateTime = null)
        {
            if (dateTime.HasValue == false)
            {
                dateTime = DateTime.Now;
            }
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.Value.AddHours(-8) - Jan1st1970).TotalMilliseconds;
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetUnixTimeStamp(DateTime? dateTime)
        {
            return GetTimeStampLong(dateTime).ToString();
        }

        /// <summary>
        /// long 转时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetTime(long timeStamp)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return start.AddMilliseconds(timeStamp).AddHours(8);
        }

        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime(string timeStamp)
        {
            long lTime = 0;
            if (long.TryParse(timeStamp + "0000000", out lTime))
            {
                return GetTime(lTime);
            }
            return DateTime.MaxValue;
        }

        /// <summary>
        /// 获得两个时间差的时间戳
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static TimeSpan DateDiff(DateTime startTime, DateTime endTime)
        {
            TimeSpan startTimeSpan = new TimeSpan(startTime.Ticks);
            TimeSpan endTimeSpan = new TimeSpan(endTime.Ticks);
            TimeSpan ts = startTimeSpan.Subtract(endTimeSpan).Duration();
            return ts;
        }

        #endregion

        #region MD5函数
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            return MD5(str, Encoding.Default);
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            if (encoding == null)
                encoding = Encoding.Default;
            return MD5ByBytes(encoding.GetBytes(str));
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>MD5结果</returns>
        public static string MD5ByBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;
            bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += bytes[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="stream">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5ByStream(Stream stream)
        {
            byte[] bytes = new MD5CryptoServiceProvider().ComputeHash(stream);
            return MD5ByBytes(bytes);
        }

        #endregion

        #region 生成Guid
        /// <summary>
        /// 获取Guid
        /// </summary>
        /// <returns></returns>
        public static string GetNewGuid(string replaceChar = "-")
        {
            var data = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(replaceChar))
            {
                return data.Replace("-", "");
            }
            return data;
        }

        #endregion

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UrlDecode(string data, Encoding encoding)
        {
            return System.Web.HttpUtility.UrlDecode(data, encoding);
        }
        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UrlEncode(string data, Encoding encoding)
        {
            return System.Web.HttpUtility.UrlEncode(data, encoding);
        }
        /// <summary>
        /// 解码
        /// "\u64CD\u4F5C\u6210\u529F";
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string UnescapeDataString(string data)
        {
            return Uri.UnescapeDataString(data);
        }


        /// <summary>
        /// 取随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandom(int min = 10000, int max = 999999999)
        {
            var random = new Random();
            var rand = random.Next(min, max);
            return rand;
        }

        /// <summary>
        /// HMACSHA1算法加密
        /// </summary>
        public static string Hmacsha1Encrypt(string encryptText, string encryptKey)
        {
            using (HMACSHA1 mac = new HMACSHA1(Encoding.UTF8.GetBytes(encryptKey)))
            {
                var hash = mac.ComputeHash(Encoding.UTF8.GetBytes(encryptText));
                var pText = Encoding.UTF8.GetBytes(encryptText);
                var all = new byte[hash.Length + pText.Length];
                Array.Copy(hash, 0, all, 0, hash.Length);
                Array.Copy(pText, 0, all, hash.Length, pText.Length);
                return Convert.ToBase64String(all);
            }
        }
    }
}
