/*****************************************************************************************************
 * 本代码版权归Wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * 所属域：WENLI-PC
 * 登录用户：Administrator
 * CLR版本：4.0.30319.17929
 * 唯一标识：20da4241-0bdc-4a06-8793-6d0889c31f95
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：MCITest


 * 创建年份：2015
 * 创建时间：2015-12-02 11:15:24
 * 创建人：Wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using Encoder = System.Drawing.Imaging.Encoder;

namespace GFF.Helper
{
    /// <summary>
    ///     图像处理类
    /// </summary>
    public static class ImageHelper
    {
        public static Image Clone(Image source)
        {
            using (var g = Graphics.FromImage(source))
            {
                var img = new Bitmap(source.Width, source.Height);
                g.DrawImage(img, 0, 0);
                g.Dispose();
                source.Dispose();
                return img;
            }
        }

        /// <summary>
        ///     将图片Image转换成Byte[]
        /// </summary>
        /// <param name="Image">image对象</param>
        /// <param name="imageFormat">后缀名</param>
        /// <returns></returns>
        public static byte[] ToBytes(Image Image, ImageFormat imageFormat)
        {
            if (Image == null)
                return null;
            byte[] data = null;
            using (var ms = new MemoryStream())
            {
                using (var Bitmap = new Bitmap(Image))
                {
                    Bitmap.Save(ms, imageFormat);
                    ms.Position = 0;
                    data = new byte[ms.Length];
                    ms.Read(data, 0, Convert.ToInt32(ms.Length));
                    ms.Flush();
                }
            }
            return data;
        }

        /// <summary>
        ///     byte[]转换成Image
        /// </summary>
        /// <param name="datas">二进制图片流</param>
        /// <returns>Image</returns>
        public static Image ToImage(byte[] datas)
        {
            var ms = new MemoryStream(datas);
            var returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static Image FromFile(string filePath)
        {
            return ToImage(File.ReadAllBytes(filePath));
        }

        public static Image FromUrl(string url)
        {
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                var ms = new MemoryStream();
                var buffer = new byte[1024];
                var numReadByte = 1024;
                do
                {
                    numReadByte = stream.Read(buffer, 0, numReadByte);
                    ms.Write(buffer, 0, numReadByte);
                } while (numReadByte > 0);
                return Image.FromStream(ms);
            }
        }

        /// <summary>
        ///     上传到服务器
        /// </summary>
        /// <param name="url"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string ToUrl(string url, Image image)
        {
            var result = string.Empty;
            try
            {
                var filePath = Environment.CurrentDirectory + "\\" + Guid.NewGuid().ToString("N") + ".png";
                image.Save(filePath);
                using (var webClient = new WebClientUtil())
                {
                    result = Encoding.UTF8.GetString(webClient.UploadFile(url, filePath));
                }
                image.Dispose();
                GC.Collect(0);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        ///     生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式HW/W/H/CUT</param>
        public static Image MakeThumbnail(Image originalImage, int width = 200, int height = 100, int mode = 0)
        {
            if (IsGif(originalImage))
                return MakeThumbnailGif(originalImage, width, height);
            using (originalImage)
            {
                var towidth = width;
                var toheight = height;
                var x = 0;
                var y = 0;
                var ow = originalImage.Width;
                var oh = originalImage.Height;

                switch (mode)
                {
                    case 2: //指定高宽缩放（可能变形）                 
                        break;
                    case 0: //指定宽，高按比例                     
                        toheight = oh*width/ow;
                        break;
                    case 1: //指定高，宽按比例 
                        towidth = ow*height/oh;
                        break;
                    default:
                        break;
                }

                //新建一个bmp图片 
                Image bitmap = new Bitmap(towidth, toheight);
                //新建一个画板 
                using (var g = Graphics.FromImage(bitmap))
                {
                    //设置高质量插值法 
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //设置高质量,低速度呈现平滑程度 
                    g.SmoothingMode = SmoothingMode.Default;
                    //清空画布并以透明背景色填充 
                    g.Clear(Color.Transparent);
                    //在指定位置并且按指定大小绘制原图片的指定部分 
                    g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                        new Rectangle(x, y, ow, oh),
                        GraphicsUnit.Pixel);
                }
                originalImage.Dispose();
                return bitmap;
            }
        }

        /// <summary>
        ///     当前内容是否是gif
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static bool IsGif(Image img)
        {
            var dimension = new FrameDimension(img.FrameDimensionsList[0]);
            return img.GetFrameCount(dimension) > 1;
        }

        /// <summary>
        ///     gif缩放
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Image MakeThumbnailGif(Image originalImage, int width = 200, int height = 100, int mode = 0)
        {
            var towidth = width;
            var toheight = height;
            var x = 0;
            var y = 0;
            var ow = originalImage.Width;
            var oh = originalImage.Height;
            switch (mode)
            {
                case 2: //指定高宽缩放（可能变形）                 
                    break;
                case 0: //指定宽，高按比例                     
                    toheight = oh*width/ow;
                    break;
                case 1: //指定高，宽按比例 
                    towidth = ow*height/oh;
                    break;
                default:
                    break;
            }

            Image gif = new Bitmap(towidth, toheight);
            Image frame = new Bitmap(towidth, toheight);
            var g = Graphics.FromImage(gif);
            var rg = new Rectangle(0, 0, towidth, toheight);
            var gFrame = Graphics.FromImage(frame);
            var ms = new MemoryStream();
            ms.Position = 0;
            foreach (var gd in originalImage.FrameDimensionsList)
            {
                var fd = new FrameDimension(gd);
                //因为是缩小GIF文件所以这里要设置为Time，如果是TIFF这里要设置为PAGE，因为GIF以时间分割，TIFF为页分割 
                var f = FrameDimension.Time;
                var count = originalImage.GetFrameCount(fd);
                var codecInfo = GetEncoder(ImageFormat.Gif);
                var encoder = Encoder.SaveFlag;
                EncoderParameters eps = null;
                for (var i = 0; i < count; i++)
                {
                    originalImage.SelectActiveFrame(f, i);
                    if (0 == i)
                    {
                        g.DrawImage(originalImage, rg);
                        eps = new EncoderParameters(1);
                        //第一帧需要设置为MultiFrame 
                        eps.Param[0] = new EncoderParameter(encoder, (long) EncoderValue.MultiFrame);
                        bindProperty(originalImage, gif);
                        gif.Save(ms, codecInfo, eps);
                    }
                    else
                    {
                        gFrame.DrawImage(originalImage, rg);
                        eps = new EncoderParameters(1);
                        //如果是GIF这里设置为FrameDimensionTime，如果为TIFF则设置为FrameDimensionPage 
                        eps.Param[0] = new EncoderParameter(encoder, (long) EncoderValue.FrameDimensionTime);
                        bindProperty(originalImage, frame);
                        gif.SaveAdd(frame, eps);
                    }
                }
                eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(encoder, (long) EncoderValue.Flush);
                gif.SaveAdd(eps);
            }
            originalImage.Dispose();
            gif.Dispose();
            g.Dispose();
            gFrame.Dispose();
            frame.Dispose();
            return Image.FromStream(ms);
        }

        /// <summary>
        ///     将源图片文件里每一帧的属性设置到新的图片对象里
        /// </summary>
        /// <param name="a">源图片帧</param>
        /// <param name="b">新的图片帧</param>
        private static void bindProperty(Image a, Image b)
        {
            //这个东西就是每一帧所拥有的属性，可以用GetPropertyItem方法取得这里用为完全复制原有属性所以直接赋值了 

            //顺便说一下这个属性里包含每帧间隔的秒数和透明背景调色板等设置，这里具体那个值对应那个属性大家自己在msdn搜索GetPropertyItem方法说明就有了 

            for (var i = 0; i < a.PropertyItems.Length; i++)
                b.SetPropertyItem(a.PropertyItems[i]);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();

            foreach (var codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;
            return null;
        }

        /**/
        /// <summary>  
        /// 获取图片编码信息  
        /// </summary>  
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// 图片有损压缩
        /// </summary>
        public static MemoryStream GetLossyCompression(Bitmap bitmap, long quality, string mode = "W", int width = 800, int height = 450)
        {
            int x = 0;
            int y = 0;

            int ow = bitmap.Width;
            int oh = bitmap.Height;

            int towidth = width;
            int toheight = height;

            switch (mode.ToUpper())
            {
                case "HW"://指定高宽缩放（可能变形）                 
                    break;
                case "W"://指定宽，高按比例                     
                    toheight = oh * towidth / ow;
                    break;
                case "H"://指定高，宽按比例 
                    towidth = ow * toheight / oh;
                    break;
                case "CUT"://指定高宽裁减（不变形）                 
                    if ((double)ow / (double)oh > (double)towidth / (double)toheight)
                    {
                        ow = oh * towidth / toheight;
                        y = 0;
                        x = (bitmap.Width - ow) / 2;
                    }
                    else
                    {
                        oh = ow * toheight / towidth;
                        x = 0;
                        y = (bitmap.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            using (Image _bitmap = new Bitmap(towidth, toheight))
            {
                using (Graphics g = Graphics.FromImage(_bitmap))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.Clear(Color.Transparent);
                    g.DrawImage(bitmap, new Rectangle(0, 0, towidth, toheight),
                     new Rectangle(x, y, ow, oh),
                     GraphicsUnit.Pixel);
                    MemoryStream ms = new MemoryStream();
                    ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    _bitmap.Save(ms, myImageCodecInfo, myEncoderParameters);
                    return ms;
                }
            }
        }

    }
}