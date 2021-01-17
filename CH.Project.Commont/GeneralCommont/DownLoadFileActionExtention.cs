using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace CommentProject.CommentExtention.GeneralExtention
{
    /// <summary>
    /// 
    ///   DownLoadFile dlf = new DownLoadFile();
    ///   dlf.ThreadNum = 2;//线程数，不设置默认为3
    ///   dlf.doSendMsg += SendMsgHander;//下载过程处理事件
    ///   dlf.FileId = id;
    ///   dlf.AddDown(src, @"G:\Image\XRImageDir", 0, fileName);
    ///   dlf.StartDown();
    /// </summary>
    public class DownLoadFileActionExtention
    {
        /// <summary>
        /// 文件唯一识别ID
        /// </summary>
        public virtual string FileId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private readonly List<Thread> ThreadList;

        public delegate void SendMsgDel(DownMessageDto msg);
        public virtual event SendMsgDel SendMsgAction;

        /// <summary>
        /// 下载线程数
        /// </summary>
        public virtual int ThreadNum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ThreadNum">线程数量</param>
        public DownLoadFileActionExtention(int threadNum = 6)
        {
            ThreadNum = threadNum;
            SendMsgAction += Change;
            ThreadList = new List<Thread>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        private void Change(DownMessageDto msg)
        {
            if (msg.Tag == DownStatus.Error || msg.Tag == DownStatus.End)
            {
                StartDown();
            }
            else if (msg.Tag == DownStatus.Start)
            {
                Console.WriteLine("开始下载");
            }
        }

        /// <summary>
        /// 新增下载任务 一个下载任务一个线程
        /// </summary>
        /// <param name="DownUrl"></param>
        /// <param name="Dir"></param>
        /// <param name="Id"></param>
        /// <param name="FileName"></param>
        public void AddDownThread(string DownUrl, string Dir, string FileName = "")
        {
            ThreadList.Add(new Thread(() =>
            {
                DownloadCore(DownUrl, Dir, FileName);
            }));
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="StartNum"></param>
        public void StartDown()
        {
            for (int i = 0; i < ThreadList.Count; i++)
            {
                if (ThreadList[i].ThreadState == ThreadState.Unstarted || ThreadList[i].ThreadState == ThreadState.Suspended)
                {
                    ThreadList[i].Start();
                }
            }
        }



        /// <summary>
        /// 下载核心
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dir"></param>
        /// <param name="filename"></param>
        /// <param name="id"></param>
        private void DownloadCore(string path, string dir, string filename)
        {
            try
            {
                using (FileDownloader loader = new FileDownloader(path, dir, filename, ThreadNum))
                {
                    DownMessageDto msg = new DownMessageDto();
                    msg.Tag = DownStatus.Start;
                    msg.MD5String = FileId;
                    msg.Length = (int)loader.getFileSize();
                    SendMsgAction(msg);

                    using (DownloadProgressListener linstenter = new DownloadProgressListener(msg))
                    {
                        linstenter.doSendMsg = new DownloadProgressListener.dlgSendMsg(SendMsgAction);
                        loader.DownloadAction(linstenter);//真正开始下载数据
                    }
                }
            }
            catch (Exception ex)
            {
                DownMessageDto msg = new DownMessageDto();
                msg.Length = 0;
                msg.Tag = DownStatus.Error;
                msg.ErrMessage = ex.Message;
                msg.MD5String = FileId;
                SendMsgAction(msg);

                Console.WriteLine(ex.Message);
            }
        }

        #region 辅助类

        /// <summary>
        /// 下载状态
        /// </summary>
        public enum DownStatus
        {
            Start,
            GetLength,
            DownLoad,
            End,
            Error
        }

        public class DownMessageDto
        {
            public virtual string MD5String { get; set; }

            public virtual int Length { get; set; }

            /// <summary>
            /// 下载状态
            /// </summary>
            public virtual DownStatus Tag { get; set; }

            /// <summary>
            /// 下载总量
            /// </summary>
            public virtual long Size { get; set; }

            public virtual float Speed { get; set; }
            public virtual string SpeedInfo { get { return GetFileSize(Speed); } }

            public virtual float Surplus { get; set; }

            public virtual string ErrMessage { get; set; }

            public virtual string SizeInfo { get; set; }

            public virtual string LengthInfo
            {
                get { return GetFileSize(Length); }
                set { LengthInfo = value; }
            }

            public virtual double Progress { get; set; }

            public virtual string SurplusInfo
            {
                get
                {
                    if (Surplus > 0)
                    {
                        return GetDateName((int)Math.Round(Surplus, 0));
                    }
                    return SurplusInfo;
                }
                set { SurplusInfo = value; }
            }

            private string GetFileSize(float Len)
            {
                float temp = Len;
                string[] sizes = { "B", "KB", "MB", "GB" };
                int order = 0;
                while (temp >= 1024 && order + 1 < sizes.Length)
                {
                    order++;
                    temp = temp / 1024;
                }
                return String.Format("{0:0.##} {1}", temp, sizes[order]);
            }

            private string GetDateName(int Second)
            {
                float temp = Second;
                string suf = "秒";
                if (Second > 60)
                {
                    suf = "分钟";
                    temp = temp / 60;
                    if (Second > 60)
                    {
                        suf = "小时";
                        temp = temp / 60;
                        if (Second > 24)
                        {
                            suf = "天";
                            temp = temp / 24;
                            if (Second > 30)
                            {
                                suf = "月";
                                temp = temp / 30;
                                if (Second > 12)
                                {
                                    suf = "年";
                                    temp = temp / 12;
                                }
                            }
                        }
                    }
                }
                return String.Format("{0:0} {1}", temp, suf);
            }
        }
        public interface IDownloadProgressListener
        {
            void OnDownloadSize(long size);
        }
        public class DownloadProgressListener : IDownloadProgressListener, IDisposable
        {
            DownMessageDto downMsg = null;
            public DownloadProgressListener(DownMessageDto downmsg)
            {
                this.downMsg = downmsg;
                //this.id = id;
                //this.Length = Length;
            }
            public delegate void dlgSendMsg(DownMessageDto msg);
            public dlgSendMsg doSendMsg = null;
            public void OnDownloadSize(long size)
            {
                if (downMsg == null)
                {
                    downMsg = new DownMessageDto();
                }
                //下载速度
                if (downMsg.Size == 0)
                {
                    downMsg.Speed = size;
                }
                else
                {
                    downMsg.Speed = (float)(size - downMsg.Size);

                }

                if (downMsg.Speed == 0)
                {
                    downMsg.Surplus = -1;
                    downMsg.SurplusInfo = "未知";
                }
                else
                {
                    downMsg.Surplus = ((downMsg.Length - downMsg.Size) / downMsg.Speed);
                }
                downMsg.Size = size; //下载总量

                //下载中
                downMsg.Tag = DownStatus.DownLoad;
                if (size == downMsg.Length)
                {
                    //下载完成
                    downMsg.Tag = DownStatus.End;
                    downMsg.Speed = 0;
                    downMsg.SurplusInfo = "已完成";
                }
                doSendMsg?.Invoke(downMsg);//通知具体调用者下载进度

            }

            public void Dispose()
            {
                GC.Collect();
            }
        }
        public class FileDownloader : IDisposable
        {
            /// <summary>
            /// 已下载文件长度
            /// </summary>
            private long downloadSize = 0;
            /// <summary>
            /// 原始文件长度
            /// </summary>
            private long fileSize = 0;
            /// <summary>
            /// 线程数
            /// </summary>
            private DownloadThread[] threads;
            /// <summary>
            /// 本地保存文件
            /// </summary>
            private string saveFile;
            /// <summary>
            /// 缓存各线程下载的长度
            /// </summary>
            public Dictionary<int, long> data = new Dictionary<int, long>();
            /// <summary>
            /// 当前线程下载数据块
            /// </summary>
            private long block;
            /// <summary>
            /// 下载路径
            /// </summary>
            private string downloadUrl;
            /// <summary>
            ///  获取线程数
            /// </summary>
            /// <returns> 获取线程数</returns>
            public int getThreadSize()
            {
                return threads.Length;
            }
            /// <summary>
            ///   获取文件大小
            /// </summary>
            /// <returns>获取文件大小</returns>
            public long getFileSize()
            {
                return fileSize;
            }
            /// <summary>
            /// 累计已下载大小
            /// </summary>
            /// <param name="size">累计已下载大小</param>
            public void append(long size)
            {
                lock (this)  //锁定同步..............线程开多了竟然没有同步起来.文件下载已经完毕了,下载总数量却不等于文件实际大小,找了半天原来这里错误的
                {
                    downloadSize += size;
                }

            }
            /// <summary>
            /// 更新指定线程最后下载的位置
            /// </summary>
            /// <param name="threadId">threadId 线程id</param>
            /// <param name="pos">最后下载的位置</param>
            public void update(int threadId, long pos)
            {
                if (data.ContainsKey(threadId))
                {
                    this.data[threadId] = pos;
                }
                else
                {
                    this.data.Add(threadId, pos);
                }
            }

            /// <summary>
            /// 构建下载准备,获取文件大小，计算文件大小
            /// </summary>
            /// <param name="downloadUrl">下载路径</param>
            /// <param name="fileSaveDir"> 文件保存目录</param>
            /// <param name="threadNum">下载线程数</param>
            public FileDownloader(string downloadUrl, string fileSaveDir, string filename = "", int threadNum = 3)
            {
                try
                {
                    if (string.IsNullOrEmpty(filename))
                    {
                        filename = Uri.UnescapeDataString(Path.GetFileName(downloadUrl));//获取文件名称 uri 解码中文字符
                    }
                    if (!Directory.Exists(fileSaveDir))
                    {
                        Directory.CreateDirectory(fileSaveDir);
                    }
                    this.saveFile = Path.Combine(fileSaveDir, filename); //构建保存文件 
                    //构建http 请求
                    this.downloadUrl = downloadUrl;
                    this.threads = new DownloadThread[threadNum];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downloadUrl);
                    request.Referer = downloadUrl.ToString();
                    request.Method = "GET";
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.1124)";
                    request.ContentType = "application/octet-stream";
                    request.Accept = "image/gif, image/jpeg, image/pjpeg, image/pjpeg, application/x-shockwave-flash, application/xaml+xml, application/vnd.ms-xpsdocument, application/x-ms-xbap, application/x-ms-application, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                    request.Timeout = 20 * 1000;
                    request.AllowAutoRedirect = true;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            this.fileSize = response.ContentLength;//根据响应获取文件大小
                            if (this.fileSize <= 0)
                            {
                                throw new Exception("获取文件大小失败");
                            }
                            if (filename.Length == 0)
                            {
                                throw new Exception("获取文件名失败");
                            }

                            this.block = (this.fileSize % this.threads.Length) == 0 ? this.fileSize / this.threads.Length : this.fileSize / this.threads.Length + 1;//计算每条线程下载的数据长度
                        }
                        else
                        {
                            throw new Exception("服务器返回状态失败,StatusCode:" + response.StatusCode);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw new Exception("无法连接下载地址");
                }
            }

            /// <summary>
            /// 开始下载文件
            /// </summary>
            /// <param name="listener">监听下载数量的变化,如果不需要了解实时下载的数量,可以设置为null</param>
            /// <returns>已下载文件大小</returns>
            public long DownloadAction(IDownloadProgressListener listener)
            {
                try
                {
                    using (FileStream fstream = new FileStream(this.saveFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        if (this.fileSize > 0)
                        {
                            fstream.SetLength(this.fileSize);
                        }
                        fstream.Close();
                    }
                    if (this.data.Count != this.threads.Length)
                    {
                        this.data.Clear();
                        for (int i = 0; i < this.threads.Length; i++)
                        {
                            this.data.Add(i + 1, 0);//初始化每条线程已经下载的数据长度为0
                        }
                    }
                    //for (int i = 0; i < this.threads.Length; i++)//开启线程进行下载
                    //{
                    //    long downLength = this.data[i + 1];
                    //    if (downLength < this.block && this.downloadSize < this.fileSize)//判断线程是否已经完成下载,否则继续下载	+
                    //    {
                    //        this.threads[i] = new DownloadThread(this, downloadUrl, this.saveFile, this.block, this.data[i + 1], i + 1);
                    //        this.threads[i].ThreadRun();
                    //    }
                    //    else
                    //    {
                    //        this.threads[i] = null;
                    //    }
                    //}
                    bool notFinish = true;//下载未完成
                    while (notFinish)// 循环判断所有线程是否完成下载,（可以加事件监听？？）
                    {
                        Thread.Sleep(900);
                        notFinish = false;//假定全部线程下载完成
                        for (int i = 0; i < this.threads.Length; i++)
                        {
                            if (this.threads[i] != null)//如果发现线程未完成下载
                            {
                                if (!this.threads[i].IsFinish())
                                {
                                    notFinish = true;//设置标志为下载没有完成
                                    if (this.threads[i].GetDownLength() == -1)//如果下载失败,再重新下载
                                    {
                                        this.threads[i] = new DownloadThread(this, downloadUrl, this.saveFile, this.block, this.data[i + 1], i + 1);
                                        this.threads[i].ThreadRun();
                                    }
                                }
                                else
                                {
                                    this.threads[i] = null;
                                }
                            }
                        }
                        if (listener != null)
                        {
                            listener.OnDownloadSize(this.downloadSize);//通知目前已经下载完成的数据长度
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw new Exception("下载文件失败");
                }
                return this.downloadSize;
            }

            public void Dispose()
            {
                GC.Collect();
            }
        }

        public class DownloadThread
        {
            private string saveFilePath;
            private string downUrl;
            private long block;
            private int threadId = -1;
            private long downLength;
            private bool finish = false;
            private FileDownloader downloader;

            public DownloadThread(FileDownloader downloader, string downUrl, string saveFile, long block, long downLength, int threadId)
            {
                this.downUrl = downUrl;
                this.saveFilePath = saveFile;
                this.block = block;
                this.downloader = downloader;
                this.threadId = threadId;
                this.downLength = downLength;
            }

            /// <summary>
            /// 线程开始下载
            /// </summary>
            public void ThreadRun()
            {
                //task
                //Thread td = new Thread(new ThreadStart(() =>
                //{
                if (downLength < block)//未下载完成
                {
                    try
                    {
                        int startPos = (int)(block * (threadId - 1) + downLength);//开始位置
                        int endPos = (int)(block * threadId - 1);//结束位置
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downUrl);
                        request.Referer = downUrl.ToString();
                        request.Method = "GET";
                        request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.1124)";
                        request.AllowAutoRedirect = false;
                        request.ContentType = "application/octet-stream";
                        request.Accept = "image/gif, image/jpeg, image/pjpeg, image/pjpeg, application/x-shockwave-flash, application/xaml+xml, application/vnd.ms-xpsdocument, application/x-ms-xbap, application/x-ms-application, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                        request.Timeout = 10 * 1000;
                        request.AllowAutoRedirect = true;
                        request.AddRange(startPos, endPos);
                        using (Stream responseStream = ((HttpWebResponse)request.GetResponse()).GetResponseStream())
                        {
                            lock (this)
                            {
                                byte[] buffer = new byte[102400]; //缓冲区大小
                                long offset = -1;
                                using (Stream fileStream = new FileStream(this.saveFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)) //设置文件以共享方式读写,否则会出现当前文件被另一个文件使用.
                                {
                                    fileStream.Seek(startPos, SeekOrigin.Begin); //移动文件位置
                                    while ((offset = responseStream.Read(buffer, 0, buffer.Length)) != 0)
                                    {
                                        downloader.append(offset); //更新已经下载当前总文件大小 offset 实际下载流大小
                                        fileStream.Write(buffer, 0, (int)offset);
                                        downLength += offset;  //设置当前线程已下载位置
                                        downloader.update(this.threadId, downLength);
                                    }
                                    Console.WriteLine("Thread " + this.threadId + " download finish");
                                    this.finish = true;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.downLength = -1;
                        Console.WriteLine("Thread " + this.threadId + ":" + e.Message);
                    }
                }
                //}

                //));
                //td.IsBackground = true;
                //td.Start();
            }
            /// <summary>
            /// 下载是否完成
            /// </summary>
            /// <returns></returns>
            public bool IsFinish()
            {
                return finish;
            }
            /// <summary> 
            ///  已经下载的内容大小  
            /// </summary>
            /// <returns>如果返回值为-1,代表下载失败</returns>
            public long GetDownLength()
            {
                return downLength;
            }

        }
        #endregion
    }
}
