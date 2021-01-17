using CH.Project.Commont.LogCommont;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommentProject.CommentExtention.GeneralExtention
{
    public static class IOActionExtention
    {
        public static void InitFile(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// 104857600 100MB  1048576 1MB
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="size"></param>
        public static double LowFreeSpace(string driveName = "C")
        {
            DriveInfo driveInfo = new DriveInfo(driveName.ToUpper());
            return ((driveInfo.TotalFreeSpace * 1.0) / (1048576 * 1024));
        }

        /// <summary>
        /// 读取文本数据
        /// </summary>
        /// <param name="path"></param>
        public static string ReadFile(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            String line;
            StringBuilder builder = new StringBuilder();
            while ((line = sr.ReadLine()) != null)
            {
                builder.Append(line.ToString());
            }
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> ReadFileToList(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            String line;
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line.ToString());
            }
            return list;
        }

        public static void Write(string filePath, string data)
        {
            using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
            {
                sw.Write(data);
            }
        }

        /// <summary>
        /// 图片转Base64
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileBase64(String fileName)
        {
            FileStream filestream = new FileStream(fileName, FileMode.Open);
            byte[] arr = new byte[filestream.Length];
            filestream.Read(arr, 0, (int)filestream.Length);
            string baser64 = Convert.ToBase64String(arr);
            filestream.Close();
            return baser64;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateDirectory(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool CheckFile(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="append">是否在源文件基础追加</param>
        public static void CreateFile(string filePath, bool append = false)
        {
            if (File.Exists(filePath))
            {
                if (append == false)//不追加则创建文件
                {
                    try
                    {
                        File.Delete(filePath);
                        using (FileStream fstream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            fstream.SetLength(0);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogBuilder.CreateInstance().Error("MemoryCache Set Error:" + ex.Message);
                    }
                }
            }
            else
            {
                CreateDirectory(filePath);
                using (FileStream fstream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    fstream.SetLength(0);
                }
            }

        }
    }
}
