using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public class FileHelper
    {
        /// <summary>
        /// 获取所有的dll
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="extension">扩展名</param>
        /// <returns></returns>
        public static List<string> GetAllFiles(string path, string extension = "*.dll")
        {
            return Directory.GetFiles(path, extension).ToList();
        }

        public static List<FileInfo> GetAllFileInfo(string path, string extension = "*.dll")
        {
            List<string> files = GetAllFiles(path, extension);
            List<FileInfo> fileInfos = new List<FileInfo>();
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    fileInfos.Add(fileInfo);
                }
            }

            fileInfos.Sort((a, b) =>
            {
                return (int)(a.LastWriteTime - b.LastWriteTime).TotalSeconds;
            });

            return fileInfos;
        }
    }
}
