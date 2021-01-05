using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewInterface.Utils
{
    public static class MessageBoxHelper
    {
        public static void MessageBoxShowError(string message)
        {
            var result = MessageBoxX.Show(message, "错误", Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations()
            {
                MessageBoxIcon = MessageBoxIcon.Error,
                ButtonBrush = "#FF4C4C".ToColor().ToBrush(),
            });
        }

        public static void MessageBoxShowInfo(string message)
        {
            var result = MessageBoxX.Show(message, "提示", Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations()
            {
                MessageBoxIcon = MessageBoxIcon.Info,
                ButtonBrush = "#20A0FF".ToColor().ToBrush(),
            });
        }

        public static void MessageBoxShowSuccess(string message)
        {
            var result = MessageBoxX.Show(message, "提示", Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations()
            {
                MessageBoxIcon = MessageBoxIcon.Success,
                ButtonBrush = "#20A0FF".ToColor().ToBrush(),
            });
        }

        public static void MessageBoxShowWarning(string message)
        {
            var result = MessageBoxX.Show(message, "警告", Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations()
            {
                MessageBoxIcon = MessageBoxIcon.Warning,
                ButtonBrush = "#F1C825".ToColor().ToBrush(),
            });
        }


        public static MessageBoxResult MessageBoxShowQuestion(string message)
        {
            var result = MessageBoxX.Show(message, "询问", Application.Current.MainWindow, MessageBoxButton.YesNo, new MessageBoxXConfigurations()
            {
                MessageBoxIcon = MessageBoxIcon.Question,
                ButtonBrush = "#F1C825".ToColor().ToBrush(),
            });

            return result;
        }

        /// <summary>
        /// 等待窗
        /// handler.UpdateMessage("新的提示信息");更新提示信息
        /// handler.Close()关闭窗口
        /// </summary>
        /// <param name="message"></param>
        public static IPendingHandler MessageBoxShowWaiting(string message)
        {
            return PendingBox.Show(message, "等待", false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                LoadingForeground = "#20A0FF".ToColor().ToBrush(),
                ButtonBrush = "#20A0FF".ToColor().ToBrush(),
                PendingBoxStyle = PendingBoxStyle.Classic,
                LoadingSize = 50,
            });
        }

    }
}
