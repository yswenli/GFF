/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：58343535-9f8e-4c5a-b65c-a1321121a20f
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper
 * 类名称：GlobalResourceManager
 * 创建时间：2016/12/27 14:12:31
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GFF.Helper
{
    public class GlobalResourceManager
    {
        #region 初始化表情
        public static void Initialize()
        {
            try
            {
                #region Emotion
                List<string> tempList = FileHelper.GetOffspringFiles(AppDomain.CurrentDomain.BaseDirectory + "Emotion\\");
                List<string> emotionFileList = new List<string>();
                foreach (string file in tempList)
                {
                    string name = file.ToLower();
                    if (name.EndsWith(".bmp") || name.EndsWith(".jpg") || name.EndsWith(".jpeg") || name.EndsWith(".png") || name.EndsWith(".gif"))
                    {
                        emotionFileList.Add(name);
                    }
                }
                emotionFileList.Sort(new Comparison<string>(CompareEmotionName));
                List<Image> emotionList = new List<Image>();
                for (int i = 0; i < emotionFileList.Count; i++)
                {
                    emotionList.Add(Image.FromStream(new MemoryStream(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "Emotion\\" + emotionFileList[i]))));
                }
                GlobalResourceManager.emotionList = emotionList;
                #endregion
            }
            catch (Exception ee)
            {
                MessageBox.Show("加载系统资源时，出现错误。" + ee.Message);
            }
        }
        public static int CompareEmotionName(string a, string b)
        {
            if (a.Length != b.Length)
            {
                return a.Length - b.Length;
            }
            return a.CompareTo(b);
        }
        #endregion

        #region EmotionList、EmotionDictionary
        private static List<Image> emotionList;
        public static List<Image> EmotionList
        {
            get
            {
                return emotionList;
            }
        }
        private static Dictionary<uint, Image> emotionDictionary;
        public static Dictionary<uint, Image> EmotionDictionary
        {
            get
            {
                if (emotionDictionary == null)
                {
                    emotionDictionary = new Dictionary<uint, Image>();
                    for (uint i = 0; i < emotionList.Count; i++)
                    {
                        emotionDictionary.Add(i, emotionList[(int)i]);
                    }
                }
                else if (emotionDictionary.Count != emotionList.Count)
                {
                    emotionDictionary = new Dictionary<uint, Image>();
                    for (uint i = 0; i < emotionList.Count; i++)
                    {
                        emotionDictionary.Add(i, emotionList[(int)i]);
                    }
                }
                return emotionDictionary;
            }
        }
        #endregion
    }
}
