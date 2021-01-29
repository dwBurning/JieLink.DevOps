using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Panuon.UI.Silver;
using System.ComponentModel;
using PartialViewInterface.Commands;
using System.Windows;
using System.Diagnostics;
using PartialViewInterface.Utils;
using System.IO;
using DBUtility;
using PartialViewInterface;

namespace PartialViewExportFacePic.ViewModels
{
    class ExportFacePicOptViewModel : DependencyObject
    {
        public ExportFacePicOptViewModel()
        {
            rbNoAndName = true;
            rbjielink = true;
            SelectPathCommand = new DelegateCommand();
            SelectPathCommand.ExecuteAction = SelectPath;
            ExportCommand = new DelegateCommand();
            ExportCommand.ExecuteAction = DoExportFacePic;
        }
        BllProcess bllProcess = new BllProcess();

        public DelegateCommand ExportCommand { get; set; }
        private string DbSqlConStr = "";
        private void DoExportFacePic(object sender)
        {
            try
            {
                if (FilePath.Trim().IsNullOrEmpty())
                {
                    MessageBoxHelper.MessageBoxShowWarning("请选择图片导出路径！");
                    return;
                }


                BackgroundWorker bgw = new BackgroundWorker();
                if (rbjielink)
                {
                    bgw.DoWork += ExportFacePicJielink; 
                }
                else if (rbG3)
                {
                    //MessageBoxHelper.MessageBoxShowWarning("G3导出功能暂未完成");
                    //return;
                    //填写SQLSERVER数据库内容
                    DbConfig dbconfig = new DbConfig();
                    dbconfig.ShowDialog();
                    if (dbconfig.IsSqlCon == true)
                    {
                        DbSqlConStr = dbconfig.DbSQLConnString;
                        bgw.DoWork += ExportFacePicG3;
                    }
                    else
                        return;
                }
                bgw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning(ex.ToString());
            }
        }
        /// <summary>
        /// jielink导出图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportFacePicJielink(object sender, EventArgs e)
        {
            try
            {
                ExportJielink();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning(ex.ToString());
            }
        }
        /// <summary>
        /// G3导出图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportFacePicG3(object sender, EventArgs e)
        {
            try
            {
                ExportG3();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning(ex.ToString());
            }
        }


        void ExportJielink()
        {
            int success = 0;
            int total = 0;
            int fails = 0;

            //自动设置数据库连接，获取文件服务器地址
            //1为jielink数据库
            bllProcess.TestConnect(EnvironmentInfo.ConnectionString, 1);
            string downUrl = bllProcess.GetDownServerUrl();
            ShowMessage(string.Format("获取到文件服务器地址为:{0}",downUrl));

            List<PersonInfo> list = bllProcess.GetJielinkPersonImage();
            if (list != null && list.Count > 0)
            {
                total = list.Count;
                foreach (PersonInfo info in list)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(info.Photo))
                        {
                            ShowMessage(string.Format("图片导出失败,姓名:{0},编号:{1},图片路径不存在", info.PersonName, info.PersonNO));
                            fails++;
                            continue;
                        }
                        string photoUrl = downUrl + "/" + info.Photo;
                        //源文件名
                        string sFilePath = info.Photo.Substring(0, 1) != @"\" ? @"\" + info.Photo : info.Photo;

                        string dsFullFilePath = string.Empty;
                        //多线程调用依赖对象
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            //目标文件全路径
                            dsFullFilePath = FilePath;
                        }));

                        //目标文件路径
                        //string dDirectory = Path.GetDirectoryName(dsFullFilePath);
                        //创建文件夹
                        CreateDirectory(dsFullFilePath);
                        //转换后文件名
                        string dFielName = "";
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            if (rbNoAndName)
                            {
                                dFielName = @"\" + info.PersonNO + "-" + info.PersonName + ".jpg";
                            }
                            else if (rbName)
                            {
                                dFielName = @"\" + info.PersonName + ".jpg";
                            }
                            else if (rbNo)
                            {
                                dFielName = @"\" + info.PersonNO + ".jpg";
                            }
                        }));

                        //目标文件全路径
                        string dFullFileName = dsFullFilePath + dFielName;
                        bool ret = bllProcess.DownloadPicture(photoUrl, dFullFileName);
                        if (ret)
                        {
                            success++;
                        }
                        else
                        {
                            ShowMessage(string.Format("图片导出失败,姓名:{0},编号:{1},图片路径{2}", info.PersonName, info.PersonNO, info.Photo));
                            fails++;
                        }
                        System.Threading.Thread.Sleep(5);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "服务器提交了协议冲突. Section=ResponseStatusLine")
                        {
                            ShowMessage(string.Format("姓名:【{0}】,编号:【{1}】的人脸图片不存在！", info.PersonName, info.PersonNO));
                        }
                        else
                            ShowMessage(ex.ToString());
                        //ShowMessage(string.Format("图片转换失败,姓名:{0},编号:{1},图片路径{2},路径:{3}", info.PersonName, info.PersonNO, info.Photo, info.Photo));
                        fails++;
                    }
                }
                ShowMessage(string.Format("图片导出完成，本次导出人员总数：{0}，成功:{1},失败{2}", total, success, fails));
            }
            else
            { 
                ShowMessage("未检测到人事资料中的人脸图片路径，请检查人事资料表以及数据库连接！");
            }
        }

        void ExportG3()
        {
            int success = 0;
            int total = 0;
            int fails = 0;
            //获取人脸图片保存路径
            string sNotFullFilePath = bllProcess.GetPersonImageSavePath();

            //输入正确数据库信息后测试连接时已建立SQL连接
            List<PersonInfo> list = bllProcess.GetFileFullPath();
            if (list != null && list.Count > 0)
            {
                total = list.Count;
                foreach (PersonInfo info in list)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(info.Photo))
                        {
                            fails++;
                            ShowMessage(string.Format("图片导出失败,姓名:{0},编号:{1},图片路径不存在", info.PersonName, info.PersonNO));
                            System.Threading.Thread.Sleep(5);
                            continue;
                        }

                        //源文件名
                        string sFilePath = info.Photo.Substring(0, 1) != @"\" ? @"\" + info.Photo : info.Photo;
                        string sFullFilePath = sNotFullFilePath + sFilePath;

                        string dsFullFilePath = string.Empty;
                        //多线程调用依赖对象
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            //目标文件全路径
                            dsFullFilePath = FilePath;
                        }));

                        //目标文件路径
                        //string dDirectory = Path.GetDirectoryName(dsFullFilePath);
                        //创建文件夹
                        CreateDirectory(dsFullFilePath);
                        //转换后文件名
                        string dFielName = "";
                        Dispatcher.Invoke(new Action(() =>
                        {
                            if (rbNoAndName)
                            {
                                dFielName = @"\" + info.PersonNO + "-" + info.PersonName + ".jpg";
                            }
                            else if (rbName)
                            {
                                dFielName = @"\" + info.PersonName + ".jpg";
                            }
                            else if (rbNo)
                            {
                                dFielName = @"\" + info.PersonNO + ".jpg";
                            }
                        }));

                        //目标文件全路径
                        string dFullFileName = dsFullFilePath + dFielName;

                        //bool ret = bllProcess.DownloadPicture(sFullFilePath, dFullFileName);
                        //if (ret)
                        //{
                        //    success++;
                        //}
                        //else
                        //{
                        //    ShowMessage(string.Format("图片转换失败,姓名:{0},编号:{1},图片路径{2}", info.PersonName, info.PersonNO, info.Photo));
                        //    fails++;
                        //}

                        if (File.Exists(sFullFilePath))
                        {
                            File.Move(sFullFilePath, dFullFileName);
                            success++;
                        }
                        else
                        {
                            fails++;
                            ShowMessage(string.Format("姓名:【{0}】,编号:【{1}】的人脸图片未找到！，图片路径【{2}】", info.PersonName, info.PersonNO, info.Photo));
                            //ShowMessage(string.Format("图片转换失败,姓名:{0},编号:{1},图片路径{2}", info.PersonName, info.PersonNO, info.Photo));
                        }
                    }
                    catch (Exception ex)
                    {
                        fails++;
                        ShowMessage(string.Format("图片转换失败,姓名:{0},编号:{1},图片路径{2}", info.PersonName, info.PersonNO, info.Photo));
                    }
                }
                ShowMessage(string.Format("图片导出完成，本次导出人员总数：{0}，成功:{1},失败{2}", total, success, fails));
            }
        }

        #region 定义依赖对象
        public bool rbNoAndName
        {
            get { return Convert.ToBoolean(GetValue(rbNoAndNameProperty)); }
            set { SetValue(rbNoAndNameProperty, value.ToString()); }
        }
        public static readonly DependencyProperty rbNoAndNameProperty =
            DependencyProperty.Register("rbNoAndName", typeof(string), typeof(ExportFacePicOptViewModel), new PropertyMetadata(""));
        public bool rbNo
        {
            get { return Convert.ToBoolean(GetValue(rbNoProperty)); }
            set { SetValue(rbNoProperty, value.ToString()); }
        }
        public static readonly DependencyProperty rbNoProperty =
            DependencyProperty.Register("rbNo", typeof(string), typeof(ExportFacePicOptViewModel), new PropertyMetadata(""));
        public bool rbName
        {
            get { return Convert.ToBoolean(GetValue(rbNameProperty)); }
            set { SetValue(rbNameProperty, value.ToString()); }
        }
        public static readonly DependencyProperty rbNameProperty =
            DependencyProperty.Register("rbName", typeof(string), typeof(ExportFacePicOptViewModel), new PropertyMetadata(""));
        public bool rbjielink
        {
            get { return Convert.ToBoolean(GetValue(rbjielinkProperty)); }
            set { SetValue(rbjielinkProperty, value.ToString()); }
        }
        public static readonly DependencyProperty rbjielinkProperty =
            DependencyProperty.Register("rbjielink", typeof(string), typeof(ExportFacePicOptViewModel), new PropertyMetadata(""));
        public bool rbG3
        {
            get { return Convert.ToBoolean(GetValue(rbG3Property)); }
            set { SetValue(rbG3Property, value.ToString()); }
        }
        public static readonly DependencyProperty rbG3Property =
            DependencyProperty.Register("rbG3", typeof(string), typeof(ExportFacePicOptViewModel), new PropertyMetadata(""));
        #endregion

        void CreateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        #region 选择路径
        /// <summary>
        /// 选择路径
        /// </summary>
        string defaultfilePath = "";
        public DelegateCommand SelectPathCommand { get; set; }
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(ExportFacePicOptViewModel), new PropertyMetadata(""));
        private void SelectPath(object parameter)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog fileDialog = new System.Windows.Forms.FolderBrowserDialog();

                var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer").FirstOrDefault();
                if (process != null)
                {
                    fileDialog.SelectedPath = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.FullName, "para");
                }
                if (!string.IsNullOrEmpty(defaultfilePath))
                {
                    fileDialog.SelectedPath = defaultfilePath;
                }

                System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    defaultfilePath = fileDialog.SelectedPath;

                    FilePath = fileDialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning(ex.ToString());
            }
        }
        #endregion

        #region 打印日志
        //写日志变量和函数
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ExportFacePicOptViewModel));
        public void ShowMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (message.Length > 0)
                {
                    Message += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
                }
            }));
        }
        #endregion
    }
}
