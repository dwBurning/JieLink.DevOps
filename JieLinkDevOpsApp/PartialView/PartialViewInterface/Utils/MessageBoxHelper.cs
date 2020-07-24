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
    }
}
