/*****************************************************************************************************
 * 本代码版权归Wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * 所属域：WENLI-PC
 * 登录用户：Administrator
 * CLR版本：4.0.30319.17929
 * 唯一标识：94aee558-896d-424d-b921-472e3e200704
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：Em.MCIFrameWork.Controls.Emotion
 * 类名称：ImageEntity
 * 文件名：ImageEntity
 * 创建年份：2015
 * 创建时间：2015-12-16 16:06:24
 * 创建人：Wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GFF.Helper;

namespace GFF.Component.Emotion
{
    [Serializable]
    public class ImageEntity
    {
        public ImageEntity()
        {
        }

        public ImageEntity(string fullName)
            : this()
        {
            this.FullName = fullName.Replace(Application.StartupPath + "\\", ""); ;

            this.FilePath = Path.GetDirectoryName(fullName).Replace(Application.StartupPath + "\\", "");

            this.RelativePath = this.FilePath.Replace(Application.StartupPath + "\\", "");

            this.FileName = Path.GetFileName(fullName);

            try
            {
                this.Image = Image.FromStream(new MemoryStream(File.ReadAllBytes(fullName)));
                this.MD5 = EncryptionHelper.MD5Helper.GetFileMd5(fullName);
            }
            catch (Exception ex)
            {
                this.Image = null;
                try
                {
                    File.Delete(fullName);
                }
                catch
                {
                }
                Debug.WriteLine(fullName);
                Debug.WriteLine(ex);
            }
        }

        public ImageEntity(string filePath, object tag)
            : this(filePath)
        {
            this.Tag = tag;
        }

        public string FileName { get; set; }

        private string _fullName;
        public string FullName
        {
            get
            {
                return Path.Combine(Application.StartupPath, this._fullName);
            }
            set { this._fullName = value; }
        }

        public string RelativePath { get; set; }

        private string _filePath;
        public string FilePath
        {
            get
            {
                return Path.Combine(Application.StartupPath, this._filePath);
            }
            set { this._filePath = value; }
        }

        public bool IsCustom { get; set; }

        public Image Image { get; set; }

        public string MD5 { get; set; }

        public object Tag { get; private set; }

        public bool IsDelete { get; set; }
    }
}
