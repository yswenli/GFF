/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：afa9cde9-e164-48fd-9d00-986f102303d2
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Model.Entity
 * 类名称：VideoSize
 * 创建时间：2016/11/10 17:17:36
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GFF.Model.Enum;

namespace GFF.Model.Entity
{
    /// <summary>
    /// 视频显示尺寸大小 
    /// </summary>
    public sealed class VideoSize
    {
        /// <summary>
        ///  视频显示高度
        /// </summary>
        public static int Width = 160;

        /// <summary>
        ///  视频显示宽度
        /// </summary>
        public static int Height = 120;

        /// <summary>
        /// 设置大小模式
        /// </summary>
        /// <param name="Model">视频显示尺寸大小</param>
        public static void SetModel(VideoSizeModel Model)
        {
            switch (Model)
            {
                case VideoSizeModel.W160_H120:
                    Width = 160;
                    Height = 120;
                    break;
                case VideoSizeModel.W176_H144:
                    Width = 176;
                    Height = 144;
                    break;
                case VideoSizeModel.W320_H240:
                    Width = 320;
                    Height = 240;
                    break;
                case VideoSizeModel.W352_H288:
                    Width = 352;
                    Height = 288;
                    break;
                case VideoSizeModel.W640_H480:
                    Width = 640;
                    Height = 480;
                    break;
                case VideoSizeModel.W800_H600:
                    Width = 800;
                    Height = 600;
                    break;
            }
        }
    }
}
