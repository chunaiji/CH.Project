using CH.Project.Commont.LogCommont;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommentProject.CommentExtention.GeneralExtention
{
    public static class HttpActionExtention
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        public static async Task<Tuple<bool, string>> HttpRequestCore(string url, string DataReq, List<KeyValuePair<string, string>> keyValuePair = null, string method = "POST",
            CookieCollection cookies = null, string userAgent = "", string contentType = "application/json", int timeout = 5000)
        {
            byte[] byData = Encoding.UTF8.GetBytes(DataReq);
            string retrnVlaue = "";
            var flag = false;
            HttpWebRequest req = null;
            try
            {
                req = WebRequest.Create(url) as HttpWebRequest;
                req.Method = method;
                req.Timeout = timeout;
                req.ReadWriteTimeout = timeout;
                req.ContentType = contentType;
                req.ContentLength = byData.Length;
                req.UserAgent = DefaultUserAgent;
                if (keyValuePair != null)
                {
                    foreach (var item in keyValuePair)
                    {
                        req.Headers.Add(item.Key, item.Value);
                    }
                }
                if (!string.IsNullOrEmpty(userAgent))
                {
                    req.UserAgent = userAgent;
                }
                if (cookies != null)
                {
                    req.CookieContainer = new CookieContainer();
                    req.CookieContainer.Add(cookies);
                }
                if (method.ToLower().Equals("post"))
                {
                    using (Stream rs = req.GetRequestStream())
                    {
                        rs.Write(byData, 0, byData.Length);
                        rs.Flush();
                    }
                }
                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    flag = resp.StatusCode == HttpStatusCode.OK;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.Default))
                    {
                        if (flag)
                        {
                            retrnVlaue = await sr.ReadToEndAsync();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogBuilder.CreateInstance().Error("HttpRequestCore Error:" + ex.Message);
            }
            finally
            {
                if (req != null)
                {
                    req.Abort();
                }
            }
            var value = DecodeUnicode(retrnVlaue);
            LogBuilder.CreateInstance().Info($"Url:{url},Response:{value}");
            return Tuple.Create<bool, string>(flag, value);
        }

        private static string DecodeUnicode(string s)
        {
            Regex reUnicode = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);

            return reUnicode.Replace(s, m =>
            {
                short c;
                if (short.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out c))
                {
                    return "" + (char)c;
                }
                return m.Value;
            });
        }

        //public static async Task<string> HttpPostRequestCore(string url, string DataReq, List<KeyValuePair<string, string>> keyValuePair = null, CookieCollection cookies = null,
        //    string userAgent = "", int timeout = 5000, string contentType = "application/x-www-form-urlencoded")
        //{
        //    return await HttpRequestCore(url, DataReq, keyValuePair, "POST", cookies, userAgent, contentType, timeout);
        //}

        public static async Task<Tuple<bool, string>> HttpGetRequestCore(string url, List<KeyValuePair<string, string>> keyValuePair = null, CookieCollection cookies = null,
            string userAgent = "", int timeout = 5000, string contentType = "application/x-www-form-urlencoded")
        {
            return await HttpRequestCore(url, "", keyValuePair, "POST", cookies, userAgent, contentType, timeout);
        }

        public static async Task<Tuple<bool, string>> HttpGetRequestCore(string url, Dictionary<string, string> headerDic, CookieCollection cookies = null,
           string userAgent = "", int timeout = 5000, string contentType = "application/x-www-form-urlencoded")
        {
            List<KeyValuePair<string, string>> headerList = null;
            if (headerList != null && headerList.Count > 0)
            {
                foreach (var key in headerDic.Keys)
                {
                    headerList.Add(new KeyValuePair<string, string>(key, headerDic[key]));
                }
            }
            return await HttpGetRequestCore(url, headerList, cookies, userAgent, timeout, contentType);
        }


        /// <summary>
        /// Post 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="DataReq"></param>
        /// <param name="keyValuePair"></param>
        /// <param name="cookies"></param>
        /// <param name="userAgent"></param>
        /// <param name="timeout"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<Tuple<bool, string>> HttpPostRequestCore(string url, string DataReq, List<KeyValuePair<string, string>> keyValuePair = null, CookieCollection cookies = null,
            string userAgent = "", string contentType = "application/x-www-form-urlencoded", int timeout = 5000)
        {
            return await HttpRequestCore(url, DataReq, keyValuePair, "POST", cookies, userAgent, contentType, timeout);
        }

        public static async Task<Tuple<bool, string>> HttpPostRequestCore(string url, string DataReq, Dictionary<string, string> headerDic, CookieCollection cookies = null,
          string userAgent = "", string contentType = "application/x-www-form-urlencoded", int timeout = 5000)
        {
            List<KeyValuePair<string, string>> headerList = null;
            if (headerList != null && headerList.Count > 0)
            {
                foreach (var key in headerDic.Keys)
                {
                    headerList.Add(new KeyValuePair<string, string>(key, headerDic[key]));
                }
            }
            return await HttpPostRequestCore(url, DataReq, headerList, cookies, userAgent, contentType, timeout);
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headerList"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<Tuple<bool, string>> HttpClientGet(string url, List<KeyValuePair<string, string>> headerList = null, int timeout = 30000)
        {
            var retrnVlaue = string.Empty;
            //var flag = false;
            var httpclientHandler = new HttpClientHandler();
            httpclientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
            using (var httpClient = new HttpClient(httpclientHandler))
            {
                if (headerList != null)
                {
                    foreach (var item in headerList)
                    {
                        if (item.Key.ToLower().Equals("content-type"))
                        {
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(item.Value));
                        }
                        else
                        {
                            httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }
                }
                //var httpResponseMessage = await httpClient.GetAsync(url);
                retrnVlaue = await httpClient.GetStringAsync(url);
                //flag = httpResponseMessage.StatusCode == (HttpStatusCode)StatusCodes.Status200OK;
                //if (flag)
                //{
                //    retrnVlaue = httpResponseMessage.Content.ToString();
                LogBuilder.CreateInstance().Info($"请求URL：{url},HttpClientGet 返回结果：{retrnVlaue},请求时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                //}
            }
            return Tuple.Create<bool, string>(true, retrnVlaue);
        }

        public static async Task<Tuple<bool, string>> HttpClientGet(string url, Dictionary<string, string> headerDic = null, int timeout = 30000)
        {
            List<KeyValuePair<string, string>> headerList = null;
            if (headerList != null && headerList.Count > 0)
            {
                foreach (var key in headerDic.Keys)
                {
                    headerList.Add(new KeyValuePair<string, string>(key, headerDic[key]));
                }
            }
            return await HttpClientGet(url, headerList, timeout);
        }

        /// <summary>
        /// Post 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formData"></param>
        /// <param name="headerList"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<Tuple<bool, string>> HttpClientPost(string url, Dictionary<string, string> formData, List<KeyValuePair<string, string>> headerList = null, int timeout = 30000)
        {
            var retrnVlaue = string.Empty;
            var flag = false;
            var httpclientHandler = new HttpClientHandler();
            httpclientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
            using (var httpClient = new HttpClient(httpclientHandler))
            {
                if (headerList != null)
                {
                    foreach (var item in headerList)
                    {
                        if (item.Key.ToLower().Equals("content-type"))
                        {
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(item.Value));
                        }
                        else
                        {
                            httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    formData.FillFormDataStream(ms);//填充formData
                    using (HttpContent hc = new StreamContent(ms))
                    {
                        var httpResponseMessage = await httpClient.PostAsync(url, hc);
                        flag = httpResponseMessage.StatusCode == (HttpStatusCode)StatusCodes.Status200OK;
                        retrnVlaue = await httpResponseMessage.Content.ReadAsStringAsync();
                        LogBuilder.CreateInstance().Info($"请求URL：{url},HttpClientPost 返回结果：{retrnVlaue},请求时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                    }
                }
            }
            return Tuple.Create<bool, string>(flag, retrnVlaue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formData"></param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<Tuple<bool, string>> HttpClientPost(string url, Dictionary<string, string> formData, Dictionary<string, string> headerDic = null, int timeout = 30000)
        {
            List<KeyValuePair<string, string>> headerList = null;
            if (headerList != null && headerList.Count > 0)
            {
                foreach (var key in headerDic.Keys)
                {
                    headerList.Add(new KeyValuePair<string, string>(key, headerDic[key]));
                }
            }
            return await HttpClientPost(url, formData, headerList, timeout);
        }



        /// <summary>
        /// 组装QueryString的方法
        /// 参数之间用&连接，首位没有符号，如：a=1&b=2&c=3
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        private static string GetQueryString(this Dictionary<string, string> formData)
        {
            if (formData == null || formData.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            var i = 0;
            foreach (var kv in formData)
            {
                i++;
                sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
                if (i < formData.Count)
                {
                    sb.Append("&");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 填充表单信息的Stream
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="stream"></param>
        private static void FillFormDataStream(this Dictionary<string, string> formData, Stream stream)
        {
            string dataString = GetQueryString(formData);
            var formDataBytes = formData == null ? new byte[0] : Encoding.UTF8.GetBytes(dataString);
            stream.Write(formDataBytes, 0, formDataBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);//设置指针读取位置
        }
    }
}
