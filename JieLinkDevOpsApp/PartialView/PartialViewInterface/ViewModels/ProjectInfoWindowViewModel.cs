using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewInterface.ViewModels
{
    public class ProjectInfoWindowViewModel : DependencyObject
    {
        public string ProjectNo
        {
            get { return (string)GetValue(ProjectNoProperty); }
            set { SetValue(ProjectNoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProjectNo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProjectNoProperty =
            DependencyProperty.Register("ProjectNo", typeof(string), typeof(ProjectInfoWindowViewModel));



        public string RemoteAccount
        {
            get { return (string)GetValue(RemoteAccountProperty); }
            set { SetValue(RemoteAccountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoteAccount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoteAccountProperty =
            DependencyProperty.Register("RemoteAccount", typeof(string), typeof(ProjectInfoWindowViewModel));


        public string RemotePassword
        {
            get { return (string)GetValue(RemotePasswordProperty); }
            set { SetValue(RemotePasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemotePassword.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemotePasswordProperty =
            DependencyProperty.Register("RemotePassword", typeof(string), typeof(ProjectInfoWindowViewModel));


        public string ContactName
        {
            get { return (string)GetValue(ContactNameProperty); }
            set { SetValue(ContactNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContactName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContactNameProperty =
            DependencyProperty.Register("ContactName", typeof(string), typeof(ProjectInfoWindowViewModel));


        public string ContactPhone
        {
            get { return (string)GetValue(ContactPhoneProperty); }
            set { SetValue(ContactPhoneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContactPhone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContactPhoneProperty =
            DependencyProperty.Register("ContactPhone", typeof(string), typeof(ProjectInfoWindowViewModel));

    }
}
