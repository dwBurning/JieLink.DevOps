using PartialViewInterface.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewSetting.ViewModel
{
    public class DBConnViewModel : DependencyObject
    {
        public DelegateCommand ExecuteDataArchiveCommand { get; set; }

        public DBConnViewModel()
        {
            Ip = "127.0.0.1";
            Port = 3306;
            DbName = "db_newg3_main";
            UserName = "jieLink";

            DataSource = new Dictionary<int, string>();
            DataSource.Add(1,"JieLink2.x");
            DataSource.Add(2,"JieLink3.x");
        }

        public string Ip
        {
            get { return (string)GetValue(IpProperty); }
            set { SetValue(IpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IpProperty =
            DependencyProperty.Register("Ip", typeof(string), typeof(DBConnViewModel));



        public int Port
        {
            get { return (int)GetValue(PortProperty); }
            set { SetValue(PortProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Port.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PortProperty =
            DependencyProperty.Register("Port", typeof(int), typeof(DBConnViewModel));



        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(DBConnViewModel));




        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(DBConnViewModel));




        public string DbName
        {
            get { return (string)GetValue(DbNameProperty); }
            set { SetValue(DbNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DbName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DbNameProperty =
            DependencyProperty.Register("DbName", typeof(string), typeof(DBConnViewModel));

        public int SelectIndex
        {
            get { return (int)GetValue(SelectIndexProperty); }
            set { SetValue(SelectIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectIndexProperty =
            DependencyProperty.Register("SelectIndex", typeof(int), typeof(DBConnViewModel));


        public Dictionary<int, string> DataSource
        {
            get { return (Dictionary<int, string>)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(Dictionary<int, string>), typeof(DBConnViewModel));
    }
}
