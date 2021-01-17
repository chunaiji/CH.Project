using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CommentProject.CommentExtention.GeneralExtention
{
    public static class UploadFileActionExtention
    {
        #region 保存文件
        private static ParamsModel Params;

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="fileDirectory"></param>
        public static void InitParamsModel(string fileDirectory = "/upload")
        {
            Params = new ParamsModel()
            {
                FileDirectory = fileDirectory,
                FileType = ".pdf,.xls,.xlsx,.doc,.docx,.txt,.png,.jpg,.gif,.mp3,.wav,.raw",
                MaxSizeM = 10,
                PathSaveType = PathSaveType.DateTimeNow,
                IsRenameSameFile = true
            };
        }

        /// <summary>
        /// 保存表单文件
        /// </summary>
        /// <param name="postFile">HttpPostedFile</param>
        /// <returns>是否成功 文件绝对路径 文件Md5值</returns>
        public static async Task<Tuple<bool, string, string>> Save(IFormFile postFile, string dirPath)
        {
            return await CommonSave(postFile, dirPath);
        }

        /// <summary>
        /// 生成保存文件绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="webRootPath"></param>
        /// <returns></returns>
        public static string CreateSavePath(string path, string webRootPath = "")
        {
            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = WebRootPath;
            }
            webRootPath = webRootPath.TrimEnd('\\');
            return Path.Combine(webRootPath, path.TrimStart('\\').TrimStart('/'));
        }

        public static string WebRootPath
        {
            get { return System.AppDomain.CurrentDomain.BaseDirectory; }
        }

        ///// <summary>
        ///// 保存表单文件,根据编号创建子文件夹
        ///// </summary>
        ///// <param name="postFile">HttpPostedFile</param>
        ///// <param name="number">编号</param>
        ///// <returns></returns>
        //public static Tuple<bool, string> Save(HttpPostedFile postFile, string number)
        //{
        //    Params.PathSaveType = PathSaveType.Code;
        //    Params.CodeName = number;
        //    return CommonSave(postFile);
        //}

        /// <summary>
        /// 文件保存路径(默认:/upload)
        /// </summary>
        public static void SetFileDirectory(string fileDirectory)
        {
            if (fileDirectory == null)
            {
                throw new ArgumentNullException("fileDirectory");
            }
            var isMapPath = Regex.IsMatch(fileDirectory, @"[a-z]\:\\", RegexOptions.IgnoreCase);
            if (isMapPath)
            {
                fileDirectory = GetRelativePath(fileDirectory);
            }
            Params.FileDirectory = fileDirectory;
        }

        /// <summary>
        /// 根据物理路径获取相对路径
        /// </summary>
        /// <param name="fileDirectory"></param>
        /// <param name="sever"></param>
        /// <returns></returns>
        private static string GetRelativePath(string fileDirectory)
        {
            //var sever = HttpContext.Current.Server;
            //fileDirectory = "/" + fileDirectory.Replace(sever.MapPath("~/"), "").TrimStart('/').Replace('\\', '/');
            //return fileDirectory;
            return "";
        }

        /// <summary>
        /// 验证文件类型)
        /// </summary>
        /// <param name="fileName"></param>
        private static Tuple<bool, string> CheckingType(string fileName)
        {
            if (Params.FileType != "*")
            {
                // 获取允许允许上传类型列表
                string[] typeList = Params.FileType.Split(',');

                // 获取上传文件类型(小写)
                string type = Path.GetExtension(fileName).ToLowerInvariant(); ;

                // 验证类型
                if (typeList.Contains(type) == false)
                {
                    return Tuple.Create(false, "文件类型非法!");
                }
                return Tuple.Create(true, string.Empty);
            }
            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// 检查文件
        /// </summary>
        /// <param name="contentLength"></param>
        /// <returns></returns>
        private static Tuple<bool, string> CheckSize(long contentLength)
        {
            if ((contentLength / 1024.0 / 1024.0) > Params.MaxSizeM)
            {
                return Tuple.Create(false, string.Format("对不起上传文件过大，不能超过{0}M！", Params.MaxSizeM));
            }
            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// 保存表单文件,根据HttpPostedFile
        /// </summary>
        /// <param name="postFile">HttpPostedFile</param>
        /// <param name="isRenameSameFile">值为true 同名文件进行重命名，false覆盖原有文件</param>
        /// <param name="fileName">新的文件名</param>
        /// <returns>是否成功 文件绝对路径 文件Md5值</returns>
        private static async Task<Tuple<bool, string, string>> CommonSave(IFormFile postFile, string dirPath)
        {
            try
            {
                if (postFile == null || postFile.Length == 0)
                {
                    return Tuple.Create(false, "没有文件!", string.Empty);
                }
                //文件名
                string fileMD5 = GeneralActionExtention.MD5ByStream(postFile.OpenReadStream()).ToUpper();
                string FileEextension = Path.GetExtension(postFile.FileName);
                //string fileName = Params.IsUseOldFileName ? postFile.FileName : DateTime.Now.ToString("yyyyMMddhhmmssms") + Path.GetExtension(postFile.FileName);
                //验证格式
                var checkingType = CheckingType(postFile.FileName);
                //验证大小
                var checkSize = CheckSize(postFile.Length);
                if (checkingType.Item1 == false || checkSize.Item1 == false)
                {
                    return Tuple.Create(false, $"校验不通过:{checkingType.Item2}-{checkSize.Item2}", string.Empty);
                }
                if (Directory.Exists(dirPath) == false)
                {
                    Directory.CreateDirectory(dirPath);
                }
                var filePath = dirPath + fileMD5 + FileEextension;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                // 保存文件
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postFile.CopyToAsync(stream);
                }
                return Tuple.Create(true, filePath, fileMD5);
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex.Message, string.Empty);
            }
        }

        ///// <summary>
        ///// 获取目录
        ///// </summary>
        ///// <returns></returns>
        //private static string GetDirectory(ref string webDir)
        //{
        //    var sever = HttpContext.Current.Server;


        //    // 存储目录
        //    string directory = Params.FileDirectory;

        //    // 目录格式
        //    string childDirectory = DateTime.Now.ToString("yyyy-MM-dd");
        //    if (Params.PathSaveType == PathSaveType.Code && string.IsNullOrEmpty(Params.CodeName) == false)
        //    {
        //        childDirectory = Params.CodeName;//防止文件夹名字为空
        //    }
        //    webDir = directory.TrimEnd('/') + "/" + childDirectory + '/';
        //    string dir = sever.MapPath(webDir);
        //    // 创建目录
        //    if (Directory.Exists(dir) == false)
        //    {
        //        Directory.CreateDirectory(dir);
        //    }
        //    return dir;
        //}

        private class ParamsModel
        {
            /// <summary>
            /// 文件保存路径
            /// </summary>
            public string FileDirectory { get; set; }
            /// <summary>
            /// 允许上传的文件类型, 逗号分割,必须全部小写!
            /// </summary>
            public string FileType { get; set; }
            /// <summary>
            /// 允许上传多少大小
            /// </summary>
            public double MaxSizeM { get; set; }
            /// <summary>
            /// 路径存储类型
            /// </summary>
            public PathSaveType PathSaveType { get; set; }
            /// <summary>
            /// 重命名同名文件?
            /// </summary>
            public bool IsRenameSameFile { get; set; }
            /// <summary>
            /// 是否使用原始文件名
            /// </summary>
            public bool IsUseOldFileName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string CodeName { get; set; }
        }

        /// <summary>
        /// 路径存储类型
        /// </summary>
        private enum PathSaveType
        {
            /// <summary>
            /// 根据时间创建存储目录
            /// </summary>
            DateTimeNow = 0,
            /// <summary>
            /// 根据ID编号方式来创建存储目录
            /// </summary>
            Code = 1

        }
        #endregion

        #region 上传文件

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="address">http://localhost:52573/api/file/FileUpload</param>
        /// <param name="fileNamePath">要上传的本地路径（全路径）</param>
        /// <returns>成功返回1，失败返回2</returns>
        public static Tuple<bool, string> UploadFileRequest(string address, string filePath)
        {
            var uploadResult = false;
            var responseResult = string.Empty;
            //要上传的文件
            if (File.Exists(filePath) == false)
            {
                return new Tuple<bool, string>(uploadResult, "文件不存在！");
            }
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                //二进制对象
                BinaryReader binaryReader = new BinaryReader(fileStream);
                string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");//当前时间戳
                string strPostHeader = GetHeader(strBoundary, filePath).ToString();
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);
                // 根据uri创建HttpWebRequest对象   
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(address));
                httpReq.Method = "POST";
                //对发送的数据不使用缓存   
                httpReq.AllowWriteStreamBuffering = false;
                //设置获得响应的超时时间（300秒）   
                httpReq.Timeout = 30 * 1000;
                httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");
                long length = fileStream.Length + postHeaderBytes.Length + boundaryBytes.Length;
                long fileLength = fileStream.Length;
                httpReq.ContentLength = length;
                try
                {
                    //每次上传400k  
                    int bufferLength = 4096 * 100;
                    byte[] buffer = new byte[bufferLength]; //已上传的字节数   
                    long offset = 0;         //开始上传时间   
                    DateTime startTime = DateTime.Now;
                    int size = binaryReader.Read(buffer, 0, bufferLength);
                    using (Stream postStream = httpReq.GetRequestStream()) //发送请求头部消息
                    {
                        postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                        while (size > 0)
                        {
                            postStream.Write(buffer, 0, size);
                            offset += size;
                            size = binaryReader.Read(buffer, 0, bufferLength);
                        }
                        //添加尾部的时间戳   
                        postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                        //获取服务器端的响应   
                        WebResponse webRespon = httpReq.GetResponse();
                        using (Stream stream = webRespon.GetResponseStream())
                        {
                            //读取服务器端返回的消息  
                            using (StreamReader streamReader = new StreamReader(stream))
                            {
                                responseResult = streamReader.ReadLine();
                                uploadResult = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //LogExtentions.GetInstance().Error(ex);
                    uploadResult = false;
                    responseResult = ex.Message;
                }
                finally
                {
                    binaryReader.Close();
                }
            }
            return new Tuple<bool, string>(uploadResult, responseResult);
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static StringBuilder GetHeader(string strBoundary, string saveFilePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(strBoundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("supervisorfile");
            sb.Append("\"; filename=\"");
            sb.Append(saveFilePath);
            sb.Append("\";");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");
            return sb;
        }
        #endregion
    }
}

