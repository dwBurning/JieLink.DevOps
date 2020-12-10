using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JieShun.JieLink.DevOps.Updater.ViewModels
{
    public class MainWindowViewModel : DependencyObject
    {
        public int UpdateProgress
        {
            get { return (int)GetValue(UpdateProgressProperty); }
            set { SetValue(UpdateProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpdateProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateProgressProperty =
            DependencyProperty.Register("UpdateProgress", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));


        public string UpdateMessage
        {
            get { return (string)GetValue(UpdateMessageProperty); }
            set { SetValue(UpdateMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpdateMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateMessageProperty =
            DependencyProperty.Register("UpdateMessage", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(""));




        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(""));


    }
}
