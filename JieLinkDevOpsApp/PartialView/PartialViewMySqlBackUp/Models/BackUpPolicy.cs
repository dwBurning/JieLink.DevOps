using PartialViewInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewMySqlBackUp.Models
{
    public class BackUpPolicy : DependencyObject
    {
        /// <summary>
        /// 星期天
        /// </summary>
        public bool Sunday
        {
            get { return (bool)GetValue(SundayProperty); }
            set { SetValue(SundayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Sunday.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SundayProperty =
            DependencyProperty.Register("Sunday", typeof(bool), typeof(BackUpPolicy));

        /// <summary>
        /// 星期一
        /// </summary>

        public bool Monday
        {
            get { return (bool)GetValue(MondayProperty); }
            set { SetValue(MondayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Monday.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MondayProperty =
            DependencyProperty.Register("Monday", typeof(bool), typeof(BackUpPolicy));

        /// <summary>
        /// 星期二
        /// </summary>

        public bool Tuesday
        {
            get { return (bool)GetValue(TuesdayProperty); }
            set { SetValue(TuesdayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tuesday.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TuesdayProperty =
            DependencyProperty.Register("Tuesday", typeof(bool), typeof(BackUpPolicy));


        /// <summary>
        /// 星期三
        /// </summary>
        public bool Wednesday
        {
            get { return (bool)GetValue(WednesdayProperty); }
            set { SetValue(WednesdayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Wednesday.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WednesdayProperty =
            DependencyProperty.Register("Wednesday", typeof(bool), typeof(BackUpPolicy));


        /// <summary>
        /// 星期四
        /// </summary>
        public bool Thursday
        {
            get { return (bool)GetValue(ThursdayProperty); }
            set { SetValue(ThursdayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thursday.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThursdayProperty =
            DependencyProperty.Register("Thursday", typeof(bool), typeof(BackUpPolicy));


        /// <summary>
        /// 星期五
        /// </summary>
        public bool Friday
        {
            get { return (bool)GetValue(FridayProperty); }
            set { SetValue(FridayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Friday.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FridayProperty =
            DependencyProperty.Register("Friday", typeof(bool), typeof(BackUpPolicy));


        /// <summary>
        /// 星期六
        /// </summary>
        public bool Saturday
        {
            get { return (bool)GetValue(SaturdayProperty); }
            set { SetValue(SaturdayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Saturday.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaturdayProperty =
            DependencyProperty.Register("Saturday", typeof(bool), typeof(BackUpPolicy));



        public DateTime SelectedTime
        {
            get { return (DateTime)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Time.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(DateTime), typeof(BackUpPolicy));


        /// <summary>
        /// 是否全库备份-任务
        /// </summary>
        public bool IsTaskBackUpDataBase
        {
            get { return (bool)GetValue(IsTaskBackUpDataBaseProperty); }
            set { SetValue(IsTaskBackUpDataBaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBackUpDataBase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTaskBackUpDataBaseProperty =
            DependencyProperty.Register("IsTaskBackUpDataBase", typeof(bool), typeof(BackUpPolicy));




        public bool IsTaskBackUpTables
        {
            get { return (bool)GetValue(IsTaskBackUpTablesProperty); }
            set { SetValue(IsTaskBackUpTablesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTaskBackUpTables.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTaskBackUpTablesProperty =
            DependencyProperty.Register("IsTaskBackUpTables", typeof(bool), typeof(BackUpPolicy));



        public string ItemString
        {
            get { return (string)GetValue(ItemStringProperty); }
            set { SetValue(ItemStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemStringProperty =
            DependencyProperty.Register("ItemString", typeof(string), typeof(BackUpPolicy));

        public BackUpType BackUpType
        {
            get
            {
                if (IsTaskBackUpDataBase) return BackUpType.DataBase;
                else if (IsTaskBackUpTables) return BackUpType.Tables;
                return BackUpType.Tables;
            }
        }

        /// <summary>
        /// 当前选中的需要备份的数据库
        /// </summary>
        public string SelectedDatabase
        {
            get { return (string)GetValue(SelectedDatabaseProperty); }
            set { SetValue(SelectedDatabaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDatabaseProperty =
            DependencyProperty.Register("SelectedDatabase", typeof(string), typeof(BackUpPolicy));

        /// <summary>
        /// 当前选中的需要备份的数据库的数据库连接
        /// </summary>
        public string SelectedDBConnectionStr
        {
            get 
            {
                if (EnvironmentInfo.DbConnEntity == null)
                {
                    return string.Empty;
                }

                var entity = EnvironmentInfo.DbConnEntity;
                return $"Data Source={entity.Ip};port={entity.Port};User ID={entity.UserName};Password={entity.Password};Initial Catalog={SelectedDatabase};Pooling=true;charset=utf8;"; ; 
            }
        }

        public string PolicyToString
        {
            get
            {
                StringBuilder policyStr = new StringBuilder();
                if (Sunday) policyStr.Append("周日").Append(",");
                if (Monday) policyStr.Append("周一").Append(",");
                if (Tuesday) policyStr.Append("周二").Append(",");
                if (Wednesday) policyStr.Append("周三").Append(",");
                if (Thursday) policyStr.Append("周四").Append(",");
                if (Friday) policyStr.Append("周五").Append(",");
                if (Saturday) policyStr.Append("周六").Append(",");
                policyStr.Append(SelectedTime.ToString("HH:mm:ss")).Append(" ");
                policyStr.Append(SelectedDatabase).Append(" ");

                if (IsTaskBackUpDataBase) 
                    policyStr.Append("全库备份");
                else 
                    policyStr.Append("业务表备份");

                return policyStr.ToString();
            }
        }
    }
}
