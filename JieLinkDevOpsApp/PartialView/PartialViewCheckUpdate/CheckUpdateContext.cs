using Panuon.UI.Silver;
using PartialViewCheckUpdate.Views;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewCheckUpdate
{
    public static class CheckUpdateContext
    {
        public static string InstallPath { get; set; }

        public static string SetUpPackagePath { get; set; }

        public static void ShowDemonstrateWindow(string fileName, string title)
        {
            if (!fileName.EndsWith(".gif"))
            {
                fileName = fileName + ".gif";
            }
            string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string path = BaseDirectoryPath + "Gif\\" + fileName;
            if (File.Exists(path))
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                DemonstrateWindow window = new DemonstrateWindow(path, title);
                window.WindowState = WindowState.Maximized;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                (Application.Current.MainWindow as WindowX).IsMaskVisible = true;
                window.ShowDialog();
                (Application.Current.MainWindow as WindowX).IsMaskVisible = false;
            }
            else
            {
                MessageBoxHelper.MessageBoxShowWarning("文件不存在！");
            }
        }
    }
}
