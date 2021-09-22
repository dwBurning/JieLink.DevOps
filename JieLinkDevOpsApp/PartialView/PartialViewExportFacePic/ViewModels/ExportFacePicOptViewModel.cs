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
            rbNo = false;
            rbjielink = true;
            rbG3 = false;
            rbjielink3 = false;
            SelectPathCommand = new DelegateCommand();
            SelectPathCommand.ExecuteAction = SelectPath;
            SelectPathCommand_jielink3 = new DelegateCommand();
            SelectPathCommand_jielink3.ExecuteAction = SelectPathjielink3;
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
                }else if (rbjielink3)
                {
                    if (!FilePath_jielink3.Trim().IsNullOrEmpty())
                    {
                        if(!FilePath_jielink3.Contains("files"))
                        { 
                            MessageBoxHelper.MessageBoxShowWarning("jielink3导出请选择文件服务器路径，选择到files路径，例如D:\\jielink\\files！");
                            return;
                        }
                    }
                    else { return; }
                    DbConfigJielink3 dbconfig = new DbConfigJielink3();
                    dbconfig.ShowDialog();
                    if (dbconfig.IsSqlCon == true)
                    {
                        DbSqlConStr = dbconfig.DbSQLConnString;
                        bgw.DoWork += ExportFacePicJielink3;
                    }
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
        /// jielink.x导出图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportFacePicJielink3(object sender, EventArgs e)
        {
            try
            {
                ExportJielink3();
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
        void ExportJielink3()
        {
            try
            {
                int success = 0;
                int total = 0;
                int fails = 0;

                //目标文件路径
                string dsFullFilePath = string.Empty;
                string dstPath = string.Empty;
                //多线程调用依赖对象
                this.Dispatcher.Invoke(new Action(() =>
                {
                    dsFullFilePath = FilePath_jielink3;
                    dstPath = FilePath;

                }));

                ShowMessage(string.Format("使用文件服务器地址为:{0}", dsFullFilePath));
                ShowMessage(string.Format("文件备份到:{0}", dstPath));
                
                List<PersonInfo> list = bllProcess.GetJielink3PersonImage();
                if (list != null && list.Count > 0)
                {
                    total = list.Count;
                    foreach (PersonInfo info in list)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(info.Photo))
                            {
                                if(total < 100)
                                    ShowMessage(string.Format("图片导出失败,姓名:{0},编号:{1},图片路径不存在", info.PersonName, info.PersonNO));
                                LogHelper.CommLogger.Info(string.Format("图片导出失败,姓名:{0},编号:{1},图片路径不存在", info.PersonName, info.PersonNO));
                                fails++;
                                continue;
                            }
                            info.Photo = info.Photo.Replace("/", "\\");
                            string photoUrl = dstPath + "\\" + info.Photo;
                            //源文件名
                            string sFilePath = info.Photo.Substring(0, 1) != @"\" ? @"\" + info.Photo : info.Photo;

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

                            //复制文件
                            if (!File.Exists(photoUrl))
                            {
                                if(total < 100)
                                    ShowMessage(string.Format("图片导出失败,姓名:{0},编号:{1},图片路径{2}不存在", info.PersonName, info.PersonNO, photoUrl));
                                LogHelper.CommLogger.Info(string.Format("图片导出失败,姓名:{0},编号:{1},图片路径{2}不存在", info.PersonName, info.PersonNO, photoUrl));
                                fails++;
                                continue;
                            }
                            if (File.Exists(dFullFileName))
                            {
                                if(total < 100)
                                    ShowMessage(string.Format("图片导出失败,姓名:{0},编号:{1},目标图片{2}已存在", info.PersonName, info.PersonNO, dFullFileName));
                                LogHelper.CommLogger.Info(string.Format("图片导出失败,姓名:{0},编号:{1},目标图片{2}已存在", info.PersonName, info.PersonNO, dFullFileName));
                                fails++;
                                continue;
                            }
                            File.Copy(photoUrl, dFullFileName);
                            if (File.Exists(dFullFileName))
                            {
                                success++;
                            }
                            else
                            {
                                if(total < 100)
                                    ShowMessage(string.Format("图片导出失败,姓名:{0},编号:{1},图片路径{2}", info.PersonName, info.PersonNO, info.Photo));
                                LogHelper.CommLogger.Info(string.Format("图片导出失败,姓名:{0},编号:{1},图片路径{2}", info.PersonName, info.PersonNO, info.Photo));
                                fails++;
                            }
                            System.Threading.Thread.Sleep(5);
                        }
                        catch (Exception ex)
                        {
                             if(total < 100)
                                ShowMessage(ex.ToString());
                            LogHelper.CommLogger.Error(ex.ToString());
                            //ShowMessage(string.Format("图片转换失败,姓名:{0},编号:{1},图片路径{2},路径:{3}", info.PersonName, info.PersonNO, info.Photo, info.Photo));
                            fails++;
                        }
                    }
                    if (total >= 100)
                        ShowMessage(string.Format("图片导出完成，本次导出人员总数：{0}，成功:{1},失败{2}，详见信息见日志", total, success, fails));
                    ShowMessage(string.Format("图片导出完成，本次导出人员总数：{0}，成功:{1},失败{2}", total, success, fails));
                }
                else
                {
                    ShowMessage("未检测到人事资料中的人脸图片路径，请检查人事资料表以及数据库连接！");
                }
            }
            catch(Exception ex)
            {
                ShowMessage(ex.ToString());
            }
            ///TODO
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
        public bool rbjielink3
        {
            get { return Convert.ToBoolean(GetValue(rbjielink3Property)); }
            set { SetValue(rbjielink3Property, value.ToString()); }
        }
        public static readonly DependencyProperty rbjielink3Property =
            DependencyProperty.Register("rbjielink3", typeof(string), typeof(ExportFacePicOptViewModel), new PropertyMetadata(""));
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
        string defaultjielink3filePath = "";
        public DelegateCommand SelectPathCommand { get; set; }
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(ExportFacePicOptViewModel), new PropertyMetadata(""));

        public DelegateCommand SelectPathCommand_jielink3 { get; set; }
        public string FilePath_jielink3
        {
            get { return (string)GetValue(FilePath_jielink3Property); }
            set { SetValue(FilePath_jielink3Property, value); }
        }
        public static readonly DependencyProperty FilePath_jielink3Property =
            DependencyProperty.Register("FilePath_jielink3", typeof(string), typeof(ExportFacePicOptViewModel), new PropertyMetadata(""));

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


        private void SelectPathjielink3(object parameter)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog fileDialog = new System.Windows.Forms.FolderBrowserDialog();

                if (!string.IsNullOrEmpty(defaultjielink3filePath))
                {
                    fileDialog.SelectedPath = defaultjielink3filePath;
                }

                System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    defaultjielink3filePath = fileDialog.SelectedPath;

                    FilePath_jielink3 = fileDialog.SelectedPath;
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
