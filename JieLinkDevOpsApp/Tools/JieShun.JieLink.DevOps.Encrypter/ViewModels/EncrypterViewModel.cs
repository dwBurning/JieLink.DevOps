using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JieShun.JieLink.DevOps.Encrypter.ViewModels
{
    public class EncrypterViewModel : DependencyObject
    {

        public int EncryptProgress
        {
            get { return (int)GetValue(EncryptProgressProperty); }
            set { SetValue(EncryptProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EncryptProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EncryptProgressProperty =
            DependencyProperty.Register("EncryptProgress", typeof(int), typeof(EncrypterViewModel), new PropertyMetadata(0));


        public string EncryptMessage
        {
            get { return (string)GetValue(EncryptMessageProperty); }
            set { SetValue(EncryptMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EncryptMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EncryptMessageProperty =
            DependencyProperty.Register("EncryptMessage", typeof(string), typeof(EncrypterViewModel), new PropertyMetadata(""));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(EncrypterViewModel), new PropertyMetadata(""));

        

    }
}
