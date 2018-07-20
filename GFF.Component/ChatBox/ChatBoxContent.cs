/*****************************************************************************************************
 * 本代码版权归Wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * 所属域：WENLI-PC
 * 登录用户：Administrator
 * CLR版本：4.0.30319.17929
 * 唯一标识：514d1f0c-d181-460f-a428-315c6bdc9247
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：GFF.Component.ChatBox
 * 类名称：ChatBoxContent
 * 文件名：ChatBoxContent
 * 创建年份：2015
 * 创建时间：2015-12-16 15:54:31
 * 创建人：Wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GFF.Component.ChatBox
{
    #region ChatBoxContent
    [Serializable]
    public class ChatBoxContent
    {
        public ChatBoxContent() { }
        public ChatBoxContent(string _text, Font _font, Color c)
        {
            this.text = _text;
            this.font = _font;
            this.color = c;
        }

        #region Text
        private string text = "";
        /// <summary>
        /// 纯文本信息
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        #endregion

        #region Font
        private Font font;
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }
        #endregion

        #region Color
        private Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        #endregion

        #region ForeignImageDictionary
        private Dictionary<uint, Image> foreignImageDictionary = new Dictionary<uint, Image>();
        /// <summary>
        /// 非内置的表情图片。key - 在ChatBox中的位置。
        /// </summary>
        public Dictionary<uint, Image> ForeignImageDictionary
        {
            get { return foreignImageDictionary; }
            set { foreignImageDictionary = value; }
        }
        #endregion

        #region EmotionDictionary
        private Dictionary<uint, uint> emotionDictionary = new Dictionary<uint, uint>();
        /// <summary>
        /// 内置的表情图片。key - 在ChatBox中的位置 ，value - 表情图片在内置列表中的index。
        /// </summary>
        public Dictionary<uint, uint> EmotionDictionary
        {
            get { return emotionDictionary; }
            set { emotionDictionary = value; }
        }
        #endregion

        #region PicturePositions
        private List<uint> picturePositions = new List<uint>();
        /// <summary>
        /// 所有图片的位置。从小到大排列。
        /// </summary>
        public List<uint> PicturePositions
        {
            get { return picturePositions; }
            set { picturePositions = value; }
        }
        #endregion

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.text) && (this.foreignImageDictionary == null || this.foreignImageDictionary.Count == 0) && (this.emotionDictionary == null || this.emotionDictionary.Count == 0);
        }

        public bool ContainsForeignImage()
        {
            return this.foreignImageDictionary != null && this.foreignImageDictionary.Count > 0;
        }

        public void AddForeignImage(uint pos, Image img)
        {
            this.foreignImageDictionary.Add(pos, img);
        }

        public void AddEmotion(uint pos, uint emotionIndex)
        {
            this.emotionDictionary.Add(pos, emotionIndex);
        }

        public string GetTextWithPicPlaceholder(string placeholder)
        {
            if (this.picturePositions == null || this.picturePositions.Count == 0)
            {
                return this.Text;
            }

            string tmp = this.Text;
            for (int i = this.picturePositions.Count - 1; i >= 0; i--)
            {
                tmp = tmp.Insert((int)this.picturePositions[i], placeholder);
            }
            return tmp;
        }
    }
    #endregion
}
