using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PartialViewInterface.Commands;
using System.Diagnostics;
using PartialViewFacePicBackUp;
using System.Configuration;
using PartialViewInterface;
using System.Windows;
using PartialViewInterface.Utils;
using System.Data;
using Panuon.UI.Silver;
using System.ComponentModel;

namespace PartialViewFacePicBackUp.ViewModels
{
    public class FacePicBackUpOptViewModel : DependencyObject
    {
        public FacePicBackUpOptViewModel()
        {
            FacePicBackUpCommand = new DelegateCommand();
            FacePicBackUpCommand.ExecuteAction = DoFacePicBackUp;
            SelectPathCommand = new DelegateCommand();
            SelectPathCommand.ExecuteAction = SelectPath;
            CheckPicCommand = new DelegateCommand();
            CheckPicCommand.ExecuteAction = DoCheckPic;
        }

        /// <summary>
        /// 后台运行
        /// </summary>
        private void DoFacePicBackUp(object sender)
        {
            try
            {
                BackgroundWorker bgw = new BackgroundWorker();
                Notice.Show("开始备份人脸图片", "通知", 3, MessageBoxIcon.Info);
                bgw.DoWork += FacePicBackUp;
                bgw.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning(ex.ToString());
            }
        }
        private void DoCheckPic(object sender)
        {
            try
            {
                BackgroundWorker bgw = new BackgroundWorker();
                Notice.Show("开始检测人脸图片", "通知", 3, MessageBoxIcon.Info);
                bgw.DoWork += CheckPic;
                bgw.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning(ex.ToString());
            }
        }

        /// <summary>
        /// 选择路径
        /// </summary>
        public DelegateCommand SelectPathCommand { get; set; }
        /// <summary>
        /// 检测人脸图片
        /// </summary>
        public DelegateCommand CheckPicCommand { get; set; }

        /// <summary>
        /// 执行备份
        /// </summary>
        public DelegateCommand FacePicBackUpCommand { get; set; }

        /// <summary>
        /// 统计人事备份情况
        /// </summary>
        public static int CountPersonAll = 0;
        public static int CountPersonNotExists = 0;
        public static int CountPersonSuccess = 0;
        public static int CountPersonFail = 0;
        public static int CountFeatureAll = 0;
        public static int CountFeatureNotExists = 0;
        public static int CountFeatureSuccess = 0;
        public static int CountFeatureFail = 0;

        /// <summary>
        /// 文件服务器地址
        /// </summary>
        public string FileServerPath = string.Empty;

        /// <summary>
        /// 根据路径备份
        /// </summary>
        /// <param name="parameter"></param>
        public void FacePicBackUp(object sender, DoWorkEventArgs e)
        {
            try
            {
                ReadFileServerPath();

                if (FileServerPath == "")
                {
                    MessageBoxHelper.MessageBoxShowWarning("未获取到文件服务器路径！");
                    return;
                }
                if (FilePath == "")
                {
                    MessageBoxHelper.MessageBoxShowWarning("请选择有效的备份路径！");
                    return;
                }

                #region 初始化变量
                CountPersonAll = 0;
                CountPersonNotExists = 0;
                CountPersonSuccess = 0;
                CountPersonFail = 0;
                CountFeatureAll = 0;
                CountFeatureNotExists = 0;
                CountFeatureSuccess = 0;
                CountFeatureFail = 0;
                #endregion

                //读取人事资料
                string sqlstr = "select photopath,personno,PersonName from control_person where status = 0 and LENGTH(photopath) > 0";
                //MySqlDataReader reader 
                DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sqlstr).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    CountPersonAll++;
                    string photopath = dr["photopath"].ToString();

                    // 集中管控下发时保存的人脸路径不在head
                    if (photopath.StartsWith(@"down/pic"))
                    {
                        string temp = photopath.Split('/')[2];
                        string dsttemp = temp.Insert(6, "/");
                        photopath = photopath.Replace(temp, dsttemp);
                    }

                    CopyPersonFile(photopath, dr, true);
                }

                //读取人脸特征
                string sqlstrFeature = "select cpf.feature,cp.PersonName,cp.personno from control_person_face cpf inner join control_person cp on cpf.pguid = cp.pguid and LENGTH(cpf.feature) > 0 ";
                DataTable dtfeature = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sqlstrFeature).Tables[0];
                foreach(DataRow dr in dtfeature.Rows)
                {
                    CountFeatureAll++;
                    string photopath = dr["feature"].ToString();

                    // 集中管控下发时保存的人脸路径不在head
                    if (photopath.StartsWith(@"down/pic"))
                    {
                        string temp = photopath.Split('/')[2];
                        string dsttemp = temp.Insert(6, "/");
                        photopath = photopath.Replace(temp, dsttemp);
                    }
                    CopyPersonFile(photopath, dr, false);
                }

                ShowMessage(string.Format("获取到的文件服务器路径为：{0}", FileServerPath));
                ShowMessage(string.Format("备份人事图片共{0}个，其中成功{1}个，人事图片不存在{2}个，失败{3}个", CountPersonAll, CountPersonSuccess, CountPersonNotExists, CountPersonFail));
                ShowMessage(string.Format("备份人脸特征共{0}个，其中成功{1}个，特征图片不存在{2}个，失败{3}个", CountFeatureAll, CountFeatureSuccess, CountFeatureNotExists, CountFeatureFail));
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning(ex.ToString());
            }
        }

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(FacePicBackUpOptViewModel), new PropertyMetadata(""));

        string defaultfilePath = "";

        /// <summary>
        /// 路径
        /// </summary>
        /// <param name="parameter"></param>
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

        /// <summary>
        /// 检测图片是否存在
        /// </summary>
        /// <param name="parameter"></param>
        private void CheckPic(object sender, DoWorkEventArgs e)
        {
            try
            {
                ReadFileServerPath();

                if (FileServerPath == "")
                {
                    MessageBoxHelper.MessageBoxShowWarning("未获取到文件服务器路径！");
                    return;
                }
                

                ShowMessage("开始检测人脸图片完整性！");

                int CheckPersonAll = 0;
                int CheckPersonNotExist = 0;
                int CheckFeatureAll = 0;
                int CheckFeatureNotExist = 0;

                //读取人事资料
                string sqlstr = "select photopath,personno,PersonName from control_person where status = 0 and LENGTH(photopath) > 0";
                DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sqlstr).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    string photopath = dr["photopath"].ToString();
                    string personno = dr["personno"].ToString();
                    string personName = dr["PersonName"].ToString();
                    CheckPersonAll++;

                    // 集中管控下发时保存的人脸路径不在head
                    if (photopath.StartsWith(@"down/pic"))
                    {
                        string temp = photopath.Split('/')[2];
                        string dsttemp = temp.Insert(6, "/");
                        photopath = photopath.Replace(temp, dsttemp);
                    }
                    string faceFilePath = Path.Combine(FileServerPath, photopath.Replace(@"down/", ""));
                    if (!File.Exists(faceFilePath))
                    {
                        CheckPersonNotExist++;
                        ShowMessage(string.Format("人事图片检查：姓名【{1}】人事编号为【{0}】的人脸图片不存在！", personno, personName));
                    }
                }

                // 读取人脸特征
                string sqlstrFeature = "select cpf.feature,cp.PersonName,cp.personno from control_person_face cpf inner join control_person cp on cpf.pguid = cp.pguid and cp.status = 0 and LENGTH(cpf.feature) > 0 and LENGTH(cp.PhotoPath)";
                DataTable dtf = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sqlstrFeature).Tables[0];
                foreach(DataRow dr in dtf.Rows)
                {
                    CheckFeatureAll++;
                    string photopath = dr["feature"].ToString();
                    string personno = dr["personno"].ToString();
                    string personName = dr["PersonName"].ToString();

                    #region 人脸特征文件夹路径日期加一个
                    //// 2.8.1E1以后的版本不用特殊处理
                    if (photopath.StartsWith(@"down/pic"))
                    {
                        string temp = photopath.Split('/')[2];
                        string dsttemp = temp.Insert(6, "/");
                        photopath = photopath.Replace(temp, dsttemp);
                    }
                    #endregion

                    string featureFilePath = Path.Combine(FileServerPath, photopath.Replace(@"down/", ""));
                    if (!File.Exists(featureFilePath))
                    {
                        CheckFeatureNotExist++;
                        ShowMessage(string.Format("人事特征检查：姓名【{1}】人事编号为【{0}】的人脸特征不存在！", personno, personName));
                    }
                }

                ShowMessage(string.Format("检测人事图片共{0}个，其中图片不存在{1}个", CheckPersonAll, CheckPersonNotExist));
                ShowMessage(string.Format("检测人脸特征共{0}个，其中特征不存在{1}个", CheckFeatureAll, CheckFeatureNotExist));
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning(ex.ToString());
            }
        }

        /// <summary>
        /// 复制文件到备份路径
        /// </summary>
        /// <param name="sourcePath">数据库中的文件路径</param>
        public void CopyPersonFile(string sourcePath, DataRow dr, bool IsPerson)
        {
            try
            {
                string personno = dr["personno"].ToString();
                string personName = dr["PersonName"].ToString();

                sourcePath = sourcePath.Replace("/", "\\");

                //完整目标文件夹路径
                string FactSavePath = Path.GetDirectoryName(FilePath + "\\FileSavePath" + sourcePath.Replace("down", ""));
                //完整源文件路径
                string FaceSourcePath = sourcePath.Replace("down", FileServerPath);
                //完整目标文件路径
                string FaceDestPath = FilePath + "\\FileSavePath" + sourcePath.Replace("down", "");
                //如果指定的目标文件夹路径不存在，则创建该存储路径
                if (!Directory.Exists(FactSavePath))
                {
                    //创建
                    Directory.CreateDirectory(FactSavePath);
                }

                #region 保存文件
                if (!File.Exists(FaceSourcePath))
                {
                    if (IsPerson)
                    {
                        CountPersonNotExists++;
                        ShowMessage(string.Format("警告：姓名【{1}】人事编号为【{0}】的图片不存在！", personno, personName));
                    }
                    else
                    {
                        CountFeatureNotExists++;
                        ShowMessage(string.Format("警告：姓名【{1}】人事编号为【{0}】的特征文件不存在！", personno, personName));
                    }
                    return;
                }

                // 复制成功不打印，否则打印太多会滚动覆盖
                if (File.Exists(FaceDestPath))
                {
                    if (IsPerson)
                    {
                        CountPersonSuccess++;
                        //ShowMessage(string.Format("备份姓名【{1}】人事编号为【{0}】的图片时，目标文件已存在", personno, personName));
                    }
                    else
                    {
                        CountFeatureSuccess++;
                        //ShowMessage(string.Format("备份姓名【{1}】人事编号为【{0}】的特征文件时，目标文件已存在", personno, personName));
                    }
                }
                else
                {
                    File.Copy(FaceSourcePath, FaceDestPath, false);
                    if (IsPerson)
                    {
                        CountPersonSuccess++;
                        //ShowMessage(string.Format("备份姓名【{1}】人事编号为【{0}】的图片成功", personno, personName));
                    }
                    else
                    {
                        CountFeatureSuccess++;
                        //ShowMessage(string.Format("备份姓名【{1}】人事编号为【{0}】的特征文件成功", personno, personName));
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                //如果一个文件无法正常备份那么所有的都会无法正常备份，只弹窗提示一次报错
                if (CountFeatureFail == 0 && CountPersonFail == 0)
                    MessageBoxHelper.MessageBoxShowWarning(ex.ToString());

                if (IsPerson) CountPersonFail++;
                else CountFeatureFail++;
            }
        }

        /// <summary>
        /// 读取文件服务器中文件存储路径到FileServerPath
        /// </summary>
        private void ReadFileServerPath()
        {
            try
            {
                #region 根据门禁服务进程或者中心进程获取文件服务器路径

                string configpath = string.Empty;
                //System.Windows.Forms.FolderBrowserDialog fileDialog = new System.Windows.Forms.FolderBrowserDialog();
                var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer").FirstOrDefault();
                if (process == null)
                {
                    ShowMessage("未发现运行门禁服务");
                    process = Process.GetProcessesByName("SmartCenter.Host").FirstOrDefault();
                }

                if (process != null)
                {
                    configpath = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.FullName, @"SmartFile\down\Config\AppSettings.config");
                    configpath = configpath.Replace("SmartBoxDoor\\", "");
                    configpath = configpath.Replace("SmartCenter\\", "");
                }
                else
                {
                    ShowMessage("未发现运行中心服务");
                    return;
                }
                #endregion

                #region 根据文件服务器config读取文件存储路径:不要写死，万一配置文件增加结点，则取不到配置

                FileStream fs = new FileStream(configpath, FileMode.Open, FileAccess.Read);

                StreamReader sr = new StreamReader(fs, Encoding.Default);
                while (!sr.EndOfStream)
                {
                    string tmp = sr.ReadLine();
                    if (tmp.Contains("DownFilePath"))
                    {
                        List<string> lst = tmp.Split('=').ToList() ;
                        FileServerPath = lst[2].Replace(@"/>", "");
                        break;
                    }
                }

                sr.Close();
                sr.Dispose();
                fs.Close();
                fs.Dispose();


                FileServerPath = FileServerPath.Replace("\\\\", "\\").Replace("\"", "");//TrimStart("\"".ToCharArray()).TrimEnd("\"".ToCharArray());

                ShowMessage("文件服务器文件保存路径：" + FileServerPath);
                #endregion

            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning(ex.ToString());
            }
        }


        //写日志变量和函数
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(FacePicBackUpOptViewModel));
        public void ShowMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                //if (Message != null && Message.Length > 5000)
                //{
                //    Message = string.Empty;
                //}
                if (message.Length > 0)
                {
                    Message += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
                }
            }));
        }
    }
}