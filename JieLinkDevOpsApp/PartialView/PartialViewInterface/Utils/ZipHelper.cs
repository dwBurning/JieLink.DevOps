using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public  class ZipHelper
    {

        public static void ZipFile(string sSourceFile, string sTargetFile)
        {
            ZipFile(sSourceFile, sTargetFile, string.Empty);
        }

        public static void ZipFile(string sSourceFile, string sTargetFile, string password)
        {

            if (!File.Exists(sSourceFile))
            {
                throw new ArgumentException("压缩文件不存在!");

            }

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(sTargetFile)))
            {
                zipOutputStream.SetLevel(9);
                if (!string.IsNullOrEmpty(password))
                {
                    zipOutputStream.Password = password;
                }
                zipOutputStream.PutNextEntry(new ZipEntry(Path.GetFileName(sSourceFile))
                {
                    DateTime = DateTime.Now
                });
                using (FileStream fileStream = File.Open(sSourceFile, FileMode.Open))
                {
                    byte[] buffer = new byte[4 * 1024];  //缓冲区，每次操作大小
                    int num;
                    do
                    {
                        num = fileStream.Read(buffer, 0, buffer.Length);
                        zipOutputStream.Write(buffer, 0, num);
                    }
                    while (num > 0);
                }
            }
        }


        public static void UnzipFile(string zipFilePath, string filePath)
        {
            UnzipFile(zipFilePath, filePath, string.Empty);
        }


        public static void UnzipFile(string zipFilePath, string filePath, string password)
        {

            if (!filePath.EndsWith("\\"))
            {
                filePath += "\\";
            }
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            if (!File.Exists(zipFilePath))
            {
                throw new ArgumentException("解压文件不存在");
            }
            
            using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zipInputStream.Password = password;
                }
                for (; ; )
                {
                    ZipEntry nextEntry = zipInputStream.GetNextEntry();
                    if (nextEntry == null)
                    {
                        break;
                    }
                    if (nextEntry.IsDirectory)
                    {
                        string path = filePath + "\\" + nextEntry.Name;
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    }
                    else if (nextEntry.IsFile)
                    {
                        string path2 = filePath + nextEntry.Name.Replace("/", "\\");
                        Console.WriteLine("复制文件:"+path2);
                        FileInfo fi = new FileInfo(path2);
                        if (!Directory.Exists(fi.DirectoryName))
                        {
                            Directory.CreateDirectory(fi.DirectoryName);
                        }
                        using (FileStream fileStream = File.Create(path2))
                        {
                            byte[] array = new byte[2048];
                            int num;
                            do
                            {
                                num = zipInputStream.Read(array, 0, array.Length);
                                if (num > 0)
                                {
                                    fileStream.Write(array, 0, num);
                                }
                            }
                            while (num > 0);
                            continue;
                        }
                    }
                }

            }

        }
    }
}
