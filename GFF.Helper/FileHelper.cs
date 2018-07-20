/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：4962a0d9-18e8-4675-831f-0cd43fc9d6d7
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper
 * 类名称：FileHelper
 * 创建时间：2016/12/27 14:09:44
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace GFF.Helper
{
    public class FileHelper
    {
        public static string RootDir = Application.StartupPath + "/systemData/";


        /// <summary>
        /// 序列化对象到制定文件中
        /// </summary>
        public static bool SerializeFile<T>(string filePath, T t)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(fs, t);
                    fs.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 将制定文件中的字节流信息反序列化为对象
        /// </summary>
        public static T DeserializeFile<T>(string filePath)
        {
            T t = default(T);
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    t = (T)formatter.Deserialize(fs);
                    fs.Close();
                    return t;
                }
            }
            catch (Exception ex)
            {
                return t;
            }

        }
        /// <summary>
        /// 序列化对象为字节数组
        /// </summary>
        public static byte[] Serialize(Object obj)
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }
        /// <summary>
        /// 将字节数组还原为对象
        /// </summary>
        public static Object Deserialize(byte[] bs)
        {
            using (MemoryStream ms = new MemoryStream(bs))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }
        /// <summary>
        /// 获取表情图片
        /// </summary>
        public static System.Drawing.Bitmap[] GetFaces()
        {
            try
            {
                string facesDirPath = RootDir + "/Face/";
                string[] filePaths = Directory.GetFiles(facesDirPath, "*.gif");
                Bitmap[] faces = new Bitmap[filePaths.Length];
                int i = 0;
                foreach (string path in filePaths)
                {
                    using (Bitmap old = new Bitmap(path))
                    {
                        faces[i++] = old;
                    }
                }
                return faces;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void ClearDirectory(string dirPath)
        {
            string[] files = Directory.GetFiles(dirPath);
            foreach (string str in files)
            {
                File.Delete(str);
            }
            foreach (string str2 in Directory.GetDirectories(dirPath))
            {
                DeleteDirectory(str2);
            }
        }

        public static void Copy(string sourceParentDirectoryPath, IEnumerable<string> filesBeCopyed, IEnumerable<string> directoriesCopyed, string destParentDirectoryPath)
        {
            string str2;
            bool flag = sourceParentDirectoryPath == destParentDirectoryPath;
            if (filesBeCopyed != null)
            {
                foreach (string str in filesBeCopyed)
                {
                    str2 = str;
                    while (flag && File.Exists(destParentDirectoryPath + str2))
                    {
                        str2 = "副本-" + str2;
                    }
                    string path = sourceParentDirectoryPath + str;
                    if (File.Exists(path))
                    {
                        File.Copy(path, destParentDirectoryPath + str2);
                    }
                }
            }
            if (directoriesCopyed != null)
            {
                foreach (string str in directoriesCopyed)
                {
                    str2 = str;
                    while (flag && Directory.Exists(destParentDirectoryPath + str2))
                    {
                        str2 = "副本-" + str2;
                    }
                    if (Directory.Exists(sourceParentDirectoryPath + str))
                    {
                        CopyDirectoryAndFiles(sourceParentDirectoryPath, str, destParentDirectoryPath, str2);
                    }
                }
            }
        }

        private static void CopyDirectoryAndFiles(string sourceParentDirectoryPath, string dirBeCopyed, string destParentDirectoryPath, string newDirName)
        {
            Directory.CreateDirectory(destParentDirectoryPath + newDirName);
            DirectoryInfo info = new DirectoryInfo(sourceParentDirectoryPath + dirBeCopyed);
            foreach (FileInfo info2 in info.GetFiles())
            {
                File.Copy(info2.FullName, destParentDirectoryPath + newDirName + @"\" + info2.Name);
            }
            foreach (DirectoryInfo info3 in info.GetDirectories())
            {
                CopyDirectoryAndFiles(sourceParentDirectoryPath + dirBeCopyed + @"\", info3.Name, destParentDirectoryPath + newDirName + @"\", info3.Name);
            }
        }

        public static void DeleteDirectory(string dirPath)
        {
            foreach (string str in Directory.GetFiles(dirPath))
            {
                File.Delete(str);
            }
            foreach (string str2 in Directory.GetDirectories(dirPath))
            {
                DeleteDirectory(str2);
            }
            DirectoryInfo info = new DirectoryInfo(dirPath);
            info.Refresh();
            if ((info.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                info.Attributes &= ~FileAttributes.ReadOnly;
            }
            info.Delete();
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static void DoGetOffspringFiles(string rootPath, DirectoryInfo dir, ref List<string> list)
        {
            foreach (FileInfo info in dir.GetFiles())
            {
                list.Add(info.FullName.Substring(rootPath.Length));
            }
            foreach (DirectoryInfo info2 in dir.GetDirectories())
            {
                DoGetOffspringFiles(rootPath, info2, ref list);
            }
        }

        public static string EnsureExtendName(string origin_path, string extend_name)
        {
            if (Path.GetExtension(origin_path) != extend_name)
            {
                origin_path = origin_path + extend_name;
            }
            return origin_path;
        }

        public static void GenerateFile(string filePath, string text)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Flush();
            writer.Close();
            stream.Close();
        }

        public static ulong GetDirectorySize(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                return 0L;
            }
            ulong num = 0L;
            DirectoryInfo info = new DirectoryInfo(dirPath);
            foreach (FileInfo info2 in info.GetFiles())
            {
                num += (ulong)info2.Length;
            }
            DirectoryInfo[] directories = info.GetDirectories();
            if (directories.Length > 0)
            {
                for (int i = 0; i < directories.Length; i++)
                {
                    num += GetDirectorySize(directories[i].FullName);
                }
            }
            return num;
        }

        public static string GetFileContent(string file_path)
        {
            if (!File.Exists(file_path))
            {
                return null;
            }
            StreamReader reader = new StreamReader(file_path, Encoding.UTF8);
            string str = reader.ReadToEnd();
            reader.Close();
            return str;
        }

        public static string GetFileDirectory(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        public static string GetFileNameNoPath(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public static ulong GetFileSize(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            return (ulong)info.Length;
        }

        public static string GetFileToOpen(string title)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*",
                FileName = ""
            };
            if (title != null)
            {
                dialog.Title = title;
            }
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return null;
        }

        public static string GetFileToOpen(string title, string extendName, string iniDir)
        {
            return GetFileToOpen2(title, iniDir, new string[] { extendName });
        }

        public static string GetFileToOpen2(string title, string iniDir, params string[] extendNames)
        {
            int num;
            StringBuilder builder = new StringBuilder("(");
            for (num = 0; num < extendNames.Length; num++)
            {
                builder.Append("*");
                builder.Append(extendNames[num]);
                if (num < (extendNames.Length - 1))
                {
                    builder.Append(";");
                }
                else
                {
                    builder.Append(")");
                }
            }
            builder.Append("|");
            for (num = 0; num < extendNames.Length; num++)
            {
                builder.Append("*");
                builder.Append(extendNames[num]);
                if (num < (extendNames.Length - 1))
                {
                    builder.Append(";");
                }
            }
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = builder.ToString(),
                FileName = "",
                InitialDirectory = iniDir
            };
            if (title != null)
            {
                dialog.Title = title;
            }
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return null;
        }

        public static string GetFolderToOpen(bool newFolderButton)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = newFolderButton
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            return null;
        }

        /// <summary>
        /// 获取目标目录下以及其子目录下的所有文件（采用相对路径）。
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static List<string> GetOffspringFiles(string dirPath)
        {
            List<string> list = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            DoGetOffspringFiles(dirPath, dir, ref list);
            return list;
        }

        public static string GetPathToSave(string title, string defaultName, string iniDir)
        {
            string extension = Path.GetExtension(defaultName);
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = string.Format("The Files (*{0})|*{0}", extension),
                FileName = defaultName,
                InitialDirectory = iniDir,
                OverwritePrompt = false
            };
            if (title != null)
            {
                dialog.Title = title;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return null;
        }

        public static void Move(string oldParentDirectoryPath, IEnumerable<string> filesBeMoved, IEnumerable<string> directoriesBeMoved, string newParentDirectoryPath)
        {
            string str2;
            if (filesBeMoved != null)
            {
                foreach (string str in filesBeMoved)
                {
                    str2 = oldParentDirectoryPath + str;
                    if (File.Exists(str2))
                    {
                        File.Move(str2, newParentDirectoryPath + str);
                    }
                }
            }
            if (directoriesBeMoved != null)
            {
                foreach (string str in directoriesBeMoved)
                {
                    str2 = oldParentDirectoryPath + str;
                    if (Directory.Exists(str2))
                    {
                        Directory.Move(str2, newParentDirectoryPath + str);
                    }
                }
            }
        }

        public static void ReadFileData(FileStream fs, byte[] buff, int count, int offset)
        {
            int num2;
            for (int i = 0; i < count; i += num2)
            {
                num2 = fs.Read(buff, offset + i, count - i);
            }
        }

        public static byte[] ReadFileReturnBytes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            FileStream input = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader reader = new BinaryReader(input);
            byte[] buffer = reader.ReadBytes((int)input.Length);
            reader.Close();
            input.Close();
            return buffer;
        }

        public static void WriteBuffToFile(byte[] buff, string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            FileStream output = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(buff);
            writer.Flush();
            writer.Close();
            output.Close();
        }

        public static void WriteBuffToFile(byte[] buff, int offset, int len, string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            FileStream output = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(buff, offset, len);
            writer.Flush();
            writer.Close();
            output.Close();
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path">目录名</param>
        public static void CreateDir(string path)
        {
            string[] dirs = path.Split(char.Parse("\\"));
            string tmp = "";
            if (!Directory.Exists(path))
            {
                foreach (string item in dirs)
                {
                    tmp += item + "\\";
                    if (!Directory.Exists(tmp))
                    {
                        Directory.CreateDirectory(tmp);
                    }
                }
            }
        }
    }
}
