//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Net;
//using System.Runtime.InteropServices;
//using System.Text;

//namespace CommentProject.CommentExtention.GeneralExtention
//{
//    public static class ImageActionExtentions
//    {
//        /// <summary>
//        /// Base64 转 图片
//        /// </summary>
//        /// <param name="base64string"></param>
//        /// <returns></returns>
//        public static Bitmap GetImageFromBase64(string base64string)
//        {
//            byte[] b = Convert.FromBase64String(base64string);
//            using (MemoryStream ms = new MemoryStream(b))
//            {
//                Bitmap bitmap = new Bitmap(ms);
//                return bitmap;
//            }
//        }

//        /// <summary>
//        /// Image 转 Base64
//        /// </summary>
//        /// <param name="imagefile"></param>
//        /// <returns></returns>
//        public static string GetBase64FromImage(string imagefile)
//        {
//            if (File.Exists(imagefile))
//            {
//                Bitmap bmp = new Bitmap(imagefile);
//                return GetBase64FromImage(bmp);
//            }
//            return "找不到文件！";
//        }

//        /// <summary>
//        /// Image 转 Base64
//        /// </summary>
//        /// <param name="bmp"></param>
//        /// <returns></returns>
//        public static string GetBase64FromImage(Bitmap bmp)
//        {
//            string strbaser64 = "";
//            try
//            {
//                using (MemoryStream ms = new MemoryStream())
//                {
//                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
//                    byte[] arr = new byte[ms.Length];
//                    ms.Position = 0;
//                    ms.Read(arr, 0, (int)ms.Length);
//                    strbaser64 = Convert.ToBase64String(arr);
//                }
//            }
//            catch (Exception)
//            {
//                throw new Exception("转换失败！");
//            }
//            return strbaser64;
//        }

//        ///// <summary>
//        ///// 生成二维码
//        ///// </summary>
//        ///// <param name="data"></param>
//        ///// <param name="level">L M Q</param>
//        //public static Bitmap GeneratQRCode(string data, string level, bool hasIcon)
//        //{
//        //    QRCodeGenerator.ECCLevel eccLevel = (QRCodeGenerator.ECCLevel)(level == "L" ? 0 : level == "M" ? 1 : level == "Q" ? 2 : 3);
//        //    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
//        //    {
//        //        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, eccLevel))
//        //        {
//        //            using (QRCode qrCode = new QRCode(qrCodeData))
//        //            {
//        //                if (hasIcon == true)
//        //                {
//        //                    return qrCode.GetGraphic(20, Color.Black, Color.White, GetIconBitmap());
//        //                }
//        //                else
//        //                {
//        //                    return qrCode.GetGraphic(20, Color.Black, Color.White, null);
//        //                }
//        //            }
//        //        }
//        //    }
//        //}

//        /// <summary>
//        /// 默认图标
//        /// </summary>
//        /// <param name="path"></param>
//        /// <returns></returns>
//        private static Bitmap GetIconBitmap(string path = "")
//        {
//            using (Bitmap img = new Bitmap("default"))
//            {
//                if (File.Exists(path))
//                {
//                    return new Bitmap(path);
//                }
//                return img;
//            }
//        }

//        /// <summary>
//        /// 保存图片
//        /// </summary>
//        /// <param name="bitmap"></param>
//        /// <param name="path"></param>
//        public static void SaveBitmap(Bitmap bitmap, string path)
//        {
//            var temp = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, path);
//            var dir = Path.GetDirectoryName(temp);
//            if (!Directory.Exists(dir))
//            {
//                Directory.CreateDirectory(dir);
//            }
//            bitmap.Save(temp);
//        }

//        ///// <summary>
//        ///// 创建 条形码
//        ///// </summary>
//        ///// <param name="data"></param>
//        ///// <param name="width"></param>
//        ///// <param name="height"></param>
//        ///// <param name="margin"></param>
//        ///// <returns></returns>
//        //public static Bitmap GeneratBarCode(string data, int width, int height, int margin)
//        //{
//        //    QrCodeEncodingOptions options = GetQrCodeEncodingOptions(width, height, margin);
//        //    options.PureBarcode = false; // 是否是纯码，如果为 false，则会在图片下方显示数字

//        //    BarcodeWriter barCodeWriter = new BarcodeWriter();
//        //    barCodeWriter.Format = BarcodeFormat.CODE_128;//条形码
//        //    barCodeWriter.Options = options;
//        //    ZXing.Common.BitMatrix bm = barCodeWriter.Encode(data);
//        //    return barCodeWriter.Write(bm);
//        //}

//        ///// <summary>
//        ///// 生成 QrCodeEncodingOptions
//        ///// </summary>
//        ///// <param name="width"></param>
//        ///// <param name="height"></param>
//        ///// <param name="margin"></param>
//        ///// <returns></returns>
//        //private static QrCodeEncodingOptions GetQrCodeEncodingOptions(int width, int height, int margin)
//        //{
//        //    ZXing.QrCode.QrCodeEncodingOptions options = new QrCodeEncodingOptions();
//        //    options.CharacterSet = "UTF-8";
//        //    options.Width = width;
//        //    options.Height = height;
//        //    options.Margin = margin;
//        //    return options;
//        //}

//        ///// <summary>
//        ///// 创建二维码 中间带图标
//        ///// </summary>
//        ///// <param name="data"></param>
//        ///// <param name="width"></param>
//        ///// <param name="height"></param>
//        ///// <param name="margin"></param>
//        ///// <returns></returns>
//        //public static Bitmap GeneratQRCode(string data, int width, int height, int margin, Bitmap middleImg)
//        //{
//        //    var obj = GeneratQRCode(data, width, height, margin);
//        //    var bitmap = obj.Item1;
//        //    var rectangle = obj.Item2;

//        //    //计算插入图片的大小和位置
//        //    int middleImgW = Math.Min((int)(rectangle[2] / 3.5), middleImg.Width);
//        //    int middleImgH = Math.Min((int)(rectangle[3] / 3.5), middleImg.Height);
//        //    int middleImgL = (bitmap.Width - middleImgW) / 2;
//        //    int middleImgT = (bitmap.Height - middleImgH) / 2;

//        //    //将img转换成bmp格式，否则后面无法创建 Graphics对象
//        //    Bitmap bmpimg = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//        //    using (Graphics g = Graphics.FromImage(bmpimg))
//        //    {
//        //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
//        //        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
//        //        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
//        //        g.DrawImage(bitmap, 0, 0);
//        //    }

//        //    //在二维码中插入图片
//        //    Graphics myGraphic = Graphics.FromImage(bmpimg);
//        //    //白底
//        //    myGraphic.FillRectangle(Brushes.White, middleImgL, middleImgT, middleImgW, middleImgH);
//        //    myGraphic.DrawImage(middleImg, middleImgL, middleImgT, middleImgW, middleImgH);

//        //    return bmpimg;
//        //}

//        ///// <summary>
//        ///// 创建二维码
//        ///// </summary>
//        ///// <param name="data"></param>
//        ///// <param name="width"></param>
//        ///// <param name="height"></param>
//        ///// <param name="margin"></param>
//        ///// <returns></returns>
//        //public static Tuple<Bitmap, int[]> GeneratQRCode(string data, int width, int height, int margin)
//        //{
//        //    QrCodeEncodingOptions options = GetQrCodeEncodingOptions(width, height, margin);
//        //    //options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
//        //    //options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);

//        //    BarcodeWriter barCodeWriter = new BarcodeWriter();
//        //    barCodeWriter.Format = BarcodeFormat.QR_CODE;//二维码
//        //    barCodeWriter.Options = options;

//        //    ZXing.Common.BitMatrix bm = barCodeWriter.Encode(data);

//        //    //获取二维码实际尺寸（去掉二维码两边空白后的实际尺寸）
//        //    var bitmap = barCodeWriter.Write(bm);
//        //    int[] rectangle = bm.getEnclosingRectangle();
//        //    return Tuple.Create(bitmap, rectangle);
//        //}

//        ///// <summary>
//        ///// 解析二维码
//        ///// </summary>
//        ///// <param name="barcodeBitmap"></param>
//        ///// <returns></returns>
//        //private static string DecodeQrCode(Bitmap barcodeBitmap)
//        //{
//        //    BarcodeReader reader = new BarcodeReader();
//        //    reader.Options.CharacterSet = "UTF-8";
//        //    var result = reader.Decode(barcodeBitmap);
//        //    return result == null ? "解析失败" : result.Text;
//        //}

//        /// <summary>
//        /// Bitmap转Byte[]
//        /// </summary>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        public static byte[] BitmapConvertToBytes(Bitmap data)
//        {
//            using (MemoryStream ms = new MemoryStream())
//            {
//                data.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
//                byte[] bytes = ms.GetBuffer();  //byte[]   bytes=   ms.ToArray(); 这两句都可以，至于区别么，下面有解释
//                return bytes;
//            }
//        }


//        #region 灰度处理
//        /// <summary>
//        /// 将源图像灰度化，并转化为8位灰度图像。
//        /// </summary>
//        /// <param name="original"> 源图像。 </param>
//        /// <returns> 8位灰度图像。 </returns>
//        public static Bitmap RgbToGrayScale(Bitmap original)
//        {
//            if (original != null)
//            {
//                // 将源图像内存区域锁定
//                Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
//                BitmapData bmpData = original.LockBits(rect, ImageLockMode.ReadOnly,
//                        PixelFormat.Format24bppRgb);

//                // 获取图像参数
//                int width = bmpData.Width;
//                int height = bmpData.Height;
//                int stride = bmpData.Stride;  // 扫描线的宽度,比实际图片要大
//                int offset = stride - width * 3;  // 显示宽度与扫描线宽度的间隙
//                IntPtr ptr = bmpData.Scan0;   // 获取bmpData的内存起始位置的指针
//                int scanBytesLength = stride * height;  // 用stride宽度，表示这是内存区域的大小

//                // 分别设置两个位置指针，指向源数组和目标数组
//                int posScan = 0, posDst = 0;
//                byte[] rgbValues = new byte[scanBytesLength];  // 为目标数组分配内存
//                Marshal.Copy(ptr, rgbValues, 0, scanBytesLength);  // 将图像数据拷贝到rgbValues中
//                // 分配灰度数组
//                byte[] grayValues = new byte[width * height]; // 不含未用空间。
//                // 计算灰度数组

//                byte blue, green, red, YUI;



//                for (int i = 0; i < height; i++)
//                {
//                    for (int j = 0; j < width; j++)
//                    {

//                        blue = rgbValues[posScan];
//                        green = rgbValues[posScan + 1];
//                        red = rgbValues[posScan + 2];
//                        YUI = (byte)(0.229 * red + 0.587 * green + 0.144 * blue);
//                        //grayValues[posDst] = (byte)((blue + green + red) / 3);
//                        grayValues[posDst] = YUI;
//                        posScan += 3;
//                        posDst++;

//                    }
//                    // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel
//                    posScan += offset;
//                }

//                // 内存解锁
//                Marshal.Copy(rgbValues, 0, ptr, scanBytesLength);
//                original.UnlockBits(bmpData);  // 解锁内存区域

//                // 构建8位灰度位图
//                Bitmap retBitmap = BuiltGrayBitmap(grayValues, width, height);
//                return retBitmap;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// 用灰度数组新建一个8位灰度图像。
//        /// </summary>
//        /// <param name="rawValues"> 灰度数组(length = width * height)。 </param>
//        /// <param name="width"> 图像宽度。 </param>
//        /// <param name="height"> 图像高度。 </param>
//        /// <returns> 新建的8位灰度位图。 </returns>
//        public static Bitmap BuiltGrayBitmap(byte[] rawValues, int width, int height)
//        {
//            // 新建一个8位灰度位图，并锁定内存区域操作
//            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
//            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height),
//                 ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

//            // 计算图像参数
//            int offset = bmpData.Stride - bmpData.Width;        // 计算每行未用空间字节数
//            IntPtr ptr = bmpData.Scan0;                         // 获取首地址
//            int scanBytes = bmpData.Stride * bmpData.Height;    // 图像字节数 = 扫描字节数 * 高度
//            byte[] grayValues = new byte[scanBytes];            // 为图像数据分配内存

//            // 为图像数据赋值
//            int posSrc = 0, posScan = 0;                        // rawValues和grayValues的索引
//            for (int i = 0; i < height; i++)
//            {
//                for (int j = 0; j < width; j++)
//                {
//                    grayValues[posScan++] = rawValues[posSrc++];
//                }
//                // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel
//                posScan += offset;
//            }

//            // 内存解锁
//            Marshal.Copy(grayValues, 0, ptr, scanBytes);
//            bitmap.UnlockBits(bmpData);  // 解锁内存区域

//            // 修改生成位图的索引表，从伪彩修改为灰度
//            ColorPalette palette;
//            // 获取一个Format8bppIndexed格式图像的Palette对象
//            using (Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
//            {
//                palette = bmp.Palette;
//            }
//            for (int i = 0; i < 256; i++)
//            {
//                palette.Entries[i] = Color.FromArgb(i, i, i);
//            }
//            // 修改生成位图的索引表
//            bitmap.Palette = palette;

//            return bitmap;
//        }
//        #endregion

//        #region 二值化
//        /*
//        1位深度图像 颜色表数组255个元素 只有用前两个 0对应0  1对应255 
//        1位深度图像每个像素占一位
//        8位深度图像每个像素占一个字节  是1位的8倍
//        */
//        /// <summary>
//        /// 将源灰度图像二值化，并转化为1位二值图像。
//        /// </summary>
//        /// <param name="bmp"> 源灰度图像。 </param>
//        /// <returns> 1位二值图像。 </returns>
//        public static Bitmap BitmapTo2Bit(Bitmap bmp)
//        {
//            if (bmp != null)
//            {
//                // 将源图像内存区域锁定
//                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
//                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly,
//                        PixelFormat.Format8bppIndexed);

//                // 获取图像参数
//                int leng, offset_1bit = 0;
//                int width = bmpData.Width;
//                int height = bmpData.Height;
//                int stride = bmpData.Stride;  // 扫描线的宽度,比实际图片要大
//                int offset = stride - width;  // 显示宽度与扫描线宽度的间隙
//                IntPtr ptr = bmpData.Scan0;   // 获取bmpData的内存起始位置的指针
//                int scanBytesLength = stride * height;  // 用stride宽度，表示这是内存区域的大小
//                if (width % 32 == 0)
//                {
//                    leng = width / 8;
//                }
//                else
//                {
//                    leng = width / 8 + (4 - (width / 8 % 4));
//                    if (width % 8 != 0)
//                    {
//                        offset_1bit = leng - width / 8;
//                    }
//                    else
//                    {
//                        offset_1bit = leng - width / 8;
//                    }
//                }

//                // 分别设置两个位置指针，指向源数组和目标数组
//                int posScan = 0, posDst = 0;
//                byte[] rgbValues = new byte[scanBytesLength];  // 为目标数组分配内存
//                Marshal.Copy(ptr, rgbValues, 0, scanBytesLength);  // 将图像数据拷贝到rgbValues中
//                // 分配二值数组
//                byte[] grayValues = new byte[leng * height]; // 不含未用空间。
//                // 计算二值数组
//                int x, v, t = 0;
//                for (int i = 0; i < height; i++)
//                {
//                    for (x = 0; x < width; x++)
//                    {
//                        v = rgbValues[posScan];
//                        t = (t << 1) | (v > 100 ? 1 : 0);


//                        if (x % 8 == 7)
//                        {
//                            grayValues[posDst] = (byte)t;
//                            posDst++;
//                            t = 0;
//                        }
//                        posScan++;
//                    }

//                    if ((x %= 8) != 7)
//                    {
//                        t <<= 8 - x;
//                        grayValues[posDst] = (byte)t;
//                    }
//                    // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel
//                    posScan += offset;
//                    posDst += offset_1bit;
//                }

//                // 内存解锁
//                Marshal.Copy(rgbValues, 0, ptr, scanBytesLength);
//                bmp.UnlockBits(bmpData);  // 解锁内存区域

//                // 构建1位二值位图
//                Bitmap retBitmap = twoBit(grayValues, width, height);
//                return retBitmap;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// 二值化后白色设置透明
//        /// </summary>
//        /// <param name="bmp"></param>
//        /// <returns></returns>
//        public static Bitmap BitmapTo2BitMakeTransparent(Bitmap bmp)
//        {
//            var bitmap = BitmapTo2Bit(bmp);
//            if (bitmap != null)
//            {
//                bitmap.MakeTransparent(Color.White);
//            }
//            return bitmap;
//        }

//        /// <summary>
//        /// 用二值数组新建一个1位二值图像。
//        /// </summary>
//        /// <param name="rawValues"> 二值数组(length = width * height)。 </param>
//        /// <param name="width"> 图像宽度。 </param>
//        /// <param name="height"> 图像高度。 </param>
//        /// <returns> 新建的1位二值位图。 </returns>
//        public static Bitmap twoBit(byte[] rawValues, int width, int height)
//        {
//            // 新建一个1位二值位图，并锁定内存区域操作
//            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format1bppIndexed);
//            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height),
//                 ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

//            // 计算图像参数
//            int offset = bmpData.Stride - bmpData.Width / 8;        // 计算每行未用空间字节数
//            IntPtr ptr = bmpData.Scan0;                         // 获取首地址
//            int scanBytes = bmpData.Stride * bmpData.Height;    // 图像字节数 = 扫描字节数 * 高度
//            byte[] grayValues = new byte[scanBytes];            // 为图像数据分配内存

//            // 为图像数据赋值
//            int posScan = 0;                        // rawValues和grayValues的索引
//            for (int i = 0; i < height; i++)
//            {
//                for (int j = 0; j < bmpData.Width / 8; j++)
//                {
//                    grayValues[posScan] = rawValues[posScan];
//                    posScan++;
//                }
//                // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel
//                posScan += offset;
//            }

//            // 内存解锁
//            Marshal.Copy(grayValues, 0, ptr, scanBytes);
//            bitmap.UnlockBits(bmpData);  // 解锁内存区域

//            // 修改生成位图的索引表
//            ColorPalette palette;
//            // 获取一个Format8bppIndexed格式图像的Palette对象
//            using (Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format1bppIndexed))
//            {
//                palette = bmp.Palette;
//            }
//            for (int i = 0; i < 2; i = +254)
//            {
//                palette.Entries[i] = Color.FromArgb(i, i, i);
//            }
//            // 修改生成位图的索引表
//            bitmap.Palette = palette;

//            return bitmap;
//        }
//        #endregion

//        //byte[] 转图片  
//        public static Bitmap BytesToBitmap(byte[] Bytes)
//        {
//            MemoryStream stream = null;
//            try
//            {
//                stream = new MemoryStream(Bytes);
//                return new Bitmap((Image)new Bitmap(stream));
//            }
//            catch (ArgumentNullException ex)
//            {
//                throw ex;
//            }
//            catch (ArgumentException ex)
//            {
//                throw ex;
//            }
//            finally
//            {
//                stream.Close();
//            }
//        }

//        //图片转byte[]   
//        public static byte[] BitmapToBytes(Bitmap Bitmap)
//        {
//            MemoryStream ms = null;
//            try
//            {
//                ms = new MemoryStream();
//                Bitmap.Save(ms, Bitmap.RawFormat);
//                byte[] byteImage = new Byte[ms.Length];
//                byteImage = ms.ToArray();
//                return byteImage;
//            }
//            catch (ArgumentNullException ex)
//            {
//                throw ex;
//            }
//            finally
//            {
//                ms.Close();
//            }
//        }

//        /// <summary>  
//        /// 将 Stream 转成 byte[]  
//        /// </summary>  
//        public static byte[] StreamToBytes(Stream stream)
//        {
//            byte[] bytes = new byte[stream.Length];
//            stream.Read(bytes, 0, bytes.Length);

//            // 设置当前流的位置为流的开始  
//            stream.Seek(0, SeekOrigin.Begin);
//            return bytes;
//        }

//        /// <summary>  
//        /// 将 byte[] 转成 Stream  
//        /// </summary>  
//        public static Stream BytesToStream(byte[] bytes)
//        {
//            Stream stream = new MemoryStream(bytes);
//            return stream;
//        }

//        /* - - - - - - - - - - - - - - - - - - - - - - - -  
//         * Stream 和 文件之间的转换 
//         * - - - - - - - - - - - - - - - - - - - - - - - */
//        /// <summary>  
//        /// 将 Stream 写入文件  
//        /// </summary>  
//        public static void StreamToFile(Stream stream, string fileName)
//        {
//            // 把 Stream 转换成 byte[]  
//            byte[] bytes = new byte[stream.Length];
//            stream.Read(bytes, 0, bytes.Length);
//            // 设置当前流的位置为流的开始  
//            stream.Seek(0, SeekOrigin.Begin);

//            // 把 byte[] 写入文件  
//            FileStream fs = new FileStream(fileName, FileMode.Create);
//            BinaryWriter bw = new BinaryWriter(fs);
//            bw.Write(bytes);
//            bw.Close();
//            fs.Close();
//        }

//        /// <summary>  
//        /// 从文件读取 Stream  
//        /// </summary>  
//        public static Stream FileToStream(string fileName)
//        {
//            // 打开文件  
//            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
//            // 读取文件的 byte[]  
//            byte[] bytes = new byte[fileStream.Length];
//            fileStream.Read(bytes, 0, bytes.Length);
//            fileStream.Close();
//            // 把 byte[] 转换成 Stream  
//            Stream stream = new MemoryStream(bytes);
//            return stream;
//        }

//        /// <summary>
//        /// 将文件转换成byte[] 数组
//        /// </summary>
//        /// <param name="fileUrl">文件路径文件名称</param>
//        /// <returns>byte[]</returns>
//        public static byte[] GetFileData(string fileUrl)
//        {
//            FileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
//            try
//            {
//                byte[] buffur = new byte[fs.Length];
//                fs.Read(buffur, 0, (int)fs.Length);

//                return buffur;
//            }
//#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
//            catch (Exception ex)
//#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
//            {
//                //MessageBoxHelper.ShowPrompt(ex.Message);
//                return null;
//            }
//            finally
//            {
//                if (fs != null)
//                {

//                    //关闭资源
//                    fs.Close();
//                }
//            }
//        }


//        /// <summary>
//        /// 将文件转换成byte[] 数组
//        /// </summary>
//        /// <param name="fileUrl">文件路径文件名称</param>
//        /// <returns>byte[]</returns>

//        public static byte[] AuthGetFileData(string fileUrl)
//        {
//            using (FileStream fs = new FileStream(fileUrl, FileMode.OpenOrCreate, FileAccess.ReadWrite))
//            {
//                byte[] buffur = new byte[fs.Length];
//                using (BinaryWriter bw = new BinaryWriter(fs))
//                {
//                    bw.Write(buffur);
//                    bw.Close();
//                }
//                return buffur;
//            }
//        }

//        public static Bitmap FileToBitmap(string fileUrl)
//        {
//            var bytes = GetFileData(fileUrl);
//            return BytesToBitmap(bytes);
//        }

//        /// <summary>
//        /// 生成文字图片
//        /// </summary>
//        /// <param name="text"></param>
//        /// <param name="isBold"></param>
//        /// <param name="fontSize"></param>
//        public static Image CreateImage(string text, bool isBold, int fontSize)
//        {
//            int wid = 400;
//            int high = 200;
//            Font font;
//            if (isBold)
//            {
//                font = new Font("Arial", fontSize, FontStyle.Bold);
//            }
//            else
//            {
//                font = new Font("Arial", fontSize, FontStyle.Regular);
//            }
//            //绘笔颜色
//            SolidBrush brush = new SolidBrush(Color.Black);
//            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
//            Bitmap image = new Bitmap(wid, high);
//            Graphics g = Graphics.FromImage(image);
//            SizeF sizef = g.MeasureString(text, font, PointF.Empty, format);//得到文本的宽高
//            int width = (int)(sizef.Width + 1);
//            int height = (int)(sizef.Height + 1);
//            image.Dispose();
//            image = new Bitmap(width, height);
//            image.MakeTransparent(Color.White);
//            g = Graphics.FromImage(image);
//            g.Clear(Color.White);//透明

//            RectangleF rect = new RectangleF(0, 0, width, height);
//            //绘制图片
//            g.DrawString(text, font, brush, rect);
//            //释放对象
//            g.Dispose();
//            return image;
//        }

//        /// <summary>  
//        /// 合并图片  
//        /// </summary>  
//        /// <param name="imgBack"></param>  
//        /// <param name="img"></param>  
//        /// <returns></returns>  
//        public static Bitmap CombinImage(Image imgBack, Image img, int xDeviation = 0, int yDeviation = 0)
//        {
//            Bitmap bmp = new Bitmap(imgBack.Width, imgBack.Height + img.Height);
//            Graphics g = Graphics.FromImage(bmp);
//            g.Clear(Color.White);
//            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height); //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);     
//            //g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);//相片四周刷一层黑色边框    
//            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);    
//            g.DrawImage(img, imgBack.Width / 2 - img.Width / 2 + xDeviation, imgBack.Height + yDeviation, img.Width, img.Height);
//            GC.Collect();
//            return bmp;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sourceImg">背景图片</param>
//        /// <param name="destImg">待贴上去的文件</param>
//        /// <returns></returns>
//        //public static Image CombinImage(string sourceImg, string destImg)
//        //{
//        //    Image imgBack = Image.FromFile(sourceImg);     //相框图片  
//        //    Image img = Image.FromFile(destImg);        //照片图片
//        //    //从指定的System.Drawing.Image创建新的System.Drawing.Graphics        
//        //    Graphics g = Graphics.FromImage(imgBack);

//        //    g.DrawImage(imgBack, 0, 0, 148, 124);      // g.DrawImage(imgBack, 0, 0, 相框宽, 相框高); 
//        //    g.FillRectangle(System.Drawing.Brushes.Black, 16, 16, (int)112 + 2, ((int)73 + 2));//相片四周刷一层黑色边框

//        //    //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);
//        //    g.DrawImage(img, 17, 17, 112, 73);
//        //    GC.Collect();
//        //    return imgBack;
//        //}

//        public static Image CombinImage(string sourceImg, string destImg, int left, int right)
//        {
//            //Image imgBack = Image.FromFile(sourceImg);     //相框图片  

//            ////从指定的System.Drawing.Image创建新的System.Drawing.Graphics        
//            //Graphics g = Graphics.FromImage(imgBack);
//            //g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);
//            //// g.DrawImage(imgBack, 0, 0, 相框宽, 相框高); 
//            ////g.FillRectangle(System.Drawing.Brushes.Black, 16, 16, 
//            ////    (int)112 + 2, ((int)73 + 2));//相片四周刷一层黑色边框

//            //Image img = Image.FromFile(destImg);        //照片图片
//            ////g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);
//            //g.DrawImage(img, 250, 670, img.Width, 300);
//            //GC.Collect();
//            //return imgBack;
//            var img = Image.FromFile(destImg);
//            return CombinImageCore(Image.FromFile(sourceImg), Image.FromFile(destImg), left, right, img.Width, img.Height);

//        }

//        public static Image CombinImageCore(Image backImg, Image desImg, int left, int right, int width, int height)
//        {
//            //从指定的System.Drawing.Image创建新的System.Drawing.Graphics        
//            Graphics g = Graphics.FromImage(backImg);
//            g.DrawImage(backImg, 0, 0, backImg.Width, backImg.Height);
//            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);
//            g.DrawImage(desImg, left, right, width, height);
//            GC.Collect();
//            return backImg;
//        }

//        /// <summary>
//        /// 网络图片流转Image
//        /// </summary>
//        /// <param name="url"></param>
//        /// <returns></returns>
//        public static Image UrlConvertToImage(string url)
//        {
//            WebClient mywebclient = new WebClient();
//            byte[] Bytes = mywebclient.DownloadData(url);
//            using (MemoryStream ms = new MemoryStream(Bytes))
//            {
//                Image outputImg = Image.FromStream(ms);
//                return outputImg;
//            }
//        }

//        public static Bitmap ImageConvertToBitmap(Image image)
//        {
//            return new Bitmap(image);
//        }
//    }
//}
