using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewEncrypter.Models;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewEncrypter.ViewModels
{
    class EncrypterViewModel : DependencyObject
    {
        #region 变量

        #region 加/解密数据库
        public DelegateCommand CreateEncryptSqlFileCommand { get; set; }

        public DelegateCommand EncryptDatabaseCommand { get; set; }

        public DelegateCommand CreateDecryptSqlFileCommand { get; set; }

        public DelegateCommand DecryptDatabseCommand { get; set; }

        public string Dbs
        {
            get { return (string)GetValue(DbsProperty); }
            set { SetValue(DbsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Dbs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DbsProperty =
            DependencyProperty.Register("Dbs", typeof(string), typeof(EncrypterViewModel), new PropertyMetadata(""));



        public string Remark
        {
            get { return (string)GetValue(RemarkProperty); }
            set { SetValue(RemarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Remark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemarkProperty =
            DependencyProperty.Register("Remark", typeof(string), typeof(EncrypterViewModel), new PropertyMetadata($"*【生成加密SQL】【生成解密SQL】功能是生成SQL脚本，需要手动执行到数据库；" +
                $"生成的SQL脚本在本工具安装目录\\tool\\Encrypter\\script目录下。{Environment.NewLine}*【执行加密】【执行解密】功能是直接操作数据库，对数据库进行加解密，请谨慎操作。"));



        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(EncrypterViewModel), new PropertyMetadata(""));
        #endregion

        #region 加/解密人脸

        public DelegateCommand OneKeyEncryptFaceCommand { get; set; }

        public DelegateCommand OneKeyDecryptFaceCommand { get; set; }

        public DelegateCommand GetFileServerPathCommand { get; set; }

        public string FaceDbs
        {
            get { return (string)GetValue(FaceDbsProperty); }
            set { SetValue(FaceDbsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FaceDbs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FaceDbsProperty =
            DependencyProperty.Register("FaceDbs", typeof(string), typeof(EncrypterViewModel), new PropertyMetadata("jielink"));



        public string HeadPath
        {
            get { return (string)GetValue(HeadPathProperty); }
            set { SetValue(HeadPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeadPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeadPathProperty =
            DependencyProperty.Register("HeadPath", typeof(string), typeof(EncrypterViewModel), new PropertyMetadata(""));



        #endregion

        #region 加/解密文件夹

        public DelegateCommand GetFileWindowCommand { get; set; }

        public DelegateCommand EncryptFileCommand { get; set; }

        public DelegateCommand DecryptFileCommand { get; set; }

        public DelegateCommand GetDirectoryWindowCommand { get; set; }

        public DelegateCommand EncryptDirectoryCommand { get; set; }

        public DelegateCommand DecryptDirectoryCommand { get; set; }

        public string EncryptFilePath
        {
            get { return (string)GetValue(EncryptFilePathProperty); }
            set { SetValue(EncryptFilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EncryptFilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EncryptFilePathProperty =
            DependencyProperty.Register("EncryptFilePath", typeof(string), typeof(EncrypterViewModel), new PropertyMetadata(""));



        public string EncryptDirectoryPath
        {
            get { return (string)GetValue(EncryptDirectoryPathProperty); }
            set { SetValue(EncryptDirectoryPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EncryptDirectoryPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EncryptDirectoryPathProperty =
            DependencyProperty.Register("EncryptDirectoryPath", typeof(string), typeof(EncrypterViewModel), new PropertyMetadata(""));



        #endregion


        #endregion

        public EncrypterViewModel()
        {
            CreateEncryptSqlFileCommand = new DelegateCommand();
            CreateEncryptSqlFileCommand.ExecuteAction = CreateEncryptSqlFile;
            EncryptDatabaseCommand = new DelegateCommand();
            EncryptDatabaseCommand.ExecuteAction = EncryptDatabase;
            CreateDecryptSqlFileCommand = new DelegateCommand();
            CreateDecryptSqlFileCommand.ExecuteAction = CreateDecryptSqlFile;
            DecryptDatabseCommand = new DelegateCommand();
            DecryptDatabseCommand.ExecuteAction = DecryptDatabse;

            OneKeyEncryptFaceCommand = new DelegateCommand();
            OneKeyEncryptFaceCommand.ExecuteAction = OneKeyEncryptFace;
            OneKeyDecryptFaceCommand = new DelegateCommand();
            OneKeyDecryptFaceCommand.ExecuteAction = OneKeyDecryptFace;
            GetFileServerPathCommand = new DelegateCommand();
            GetFileServerPathCommand.ExecuteAction = GetFileServerPath;


            GetFileWindowCommand = new DelegateCommand();
            GetFileWindowCommand.ExecuteAction = GetFileWindow;
            GetDirectoryWindowCommand = new DelegateCommand();
            GetDirectoryWindowCommand.ExecuteAction = GetDirectoryWindow;

            EncryptFileCommand = new DelegateCommand();
            EncryptFileCommand.ExecuteAction = EncryptFile;
            DecryptFileCommand = new DelegateCommand();
            DecryptFileCommand.ExecuteAction = DecryptFile;
            EncryptDirectoryCommand = new DelegateCommand();
            EncryptDirectoryCommand.ExecuteAction = EncryptDirectory;
            DecryptDirectoryCommand = new DelegateCommand();
            DecryptDirectoryCommand.ExecuteAction = DecryptDirectory;
        }
        private void CreateEncryptSqlFile(object parameter)
        {
            if (!CheckDbs(Dbs)) return;
            Params param = new Params
            {
                database = Dbs,
                cmd = (int)EnumCMD.EncryptToSQL,
                path = "",
                connStr = GetConnStr(),
            };
            if (!CreateParamsFile(param)) return;
            Start();
        }

        private void EncryptDatabase(object parameter)
        {
            if (!CheckDbs(Dbs)) return;
            Params param = new Params
            {
                database = Dbs,
                cmd = (int)EnumCMD.EncryptToDatabase,
                path = "",
                connStr = GetConnStr(),
            };
            if (!CreateParamsFile(param)) return;
            Start();
        }

        private void CreateDecryptSqlFile(object parameter)
        {
            if (!CheckDbs(Dbs)) return;
            Params param = new Params
            {
                database = Dbs,
                cmd = (int)EnumCMD.DecryptToSQL,
                path = "",
                connStr = GetConnStr(),
            };
            if (!CreateParamsFile(param)) return;
            Start();
        }

        private void DecryptDatabse(object parameter)
        {
            if (!CheckDbs(Dbs)) return;
            Params param = new Params
            {
                database = Dbs,
                cmd = (int)EnumCMD.DecryptToDataBase,
                path = "",
                connStr = GetConnStr(),
            };
            if (!CreateParamsFile(param)) return;
            Start();
        }

        private void OneKeyEncryptFace(object parameter)
        {
            //var fileServerPath = GetFileServcerPath();
            //if (string.IsNullOrEmpty(fileServerPath)) return;
            if (!CheckDbs(FaceDbs)) return;
            if (!Directory.Exists(HeadPath))
            {
                Notice.Show("路径不存在!", "通知", 3, MessageBoxIcon.Warning);
                return;
            }
            Params param = new Params
            {
                database = FaceDbs,
                cmd = (int)EnumCMD.EncryptFileOneKey,
                path = HeadPath,
                connStr = GetConnStr(),
            };
            if (!CreateParamsFile(param)) return;
            Start();
        }

        private void OneKeyDecryptFace(object parameter)
        {
            //var fileServerPath = GetFileServcerPath();
            //if (string.IsNullOrEmpty(fileServerPath)) return;
            if (!CheckDbs(FaceDbs)) return;
            if (!Directory.Exists(HeadPath))
            {
                Notice.Show("路径不存在!", "通知", 3, MessageBoxIcon.Warning);
                return;
            }
            Params param = new Params
            {
                database = FaceDbs,
                cmd = (int)EnumCMD.DecryptFileOneKey,
                path = HeadPath,
                connStr = GetConnStr(),
            };
            if (!CreateParamsFile(param)) return;
            Start();
        }

        private void EncryptFile(object parameter)
        {
            if (!File.Exists(EncryptFilePath))
            {
                Notice.Show("文件不存在!", "通知", 3, MessageBoxIcon.Warning);
                return;
            }
            Params param = new Params
            {
                database = "",
                cmd = (int)EnumCMD.EncryptFile,
                path = EncryptFilePath,
            };
            if (!CreateParamsFile(param)) return;
            Start();
        }

        private void DecryptFile(object parameter)
        {
            if (!File.Exists(EncryptFilePath))
            {
                Notice.Show("文件不存在!", "通知", 3, MessageBoxIcon.Warning);
                return;
            }
            Params param = new Params
            {
                database = "",
                cmd = (int)EnumCMD.DecryptFile,
                path = EncryptFilePath,
            };
            if (!CreateParamsFile(param)) return;
            Start();
        }

        private void EncryptDirectory(object parameter)
        {
            if (!Directory.Exists(EncryptDirectoryPath))
            {
                Notice.Show("路径不存在!", "通知", 3, MessageBoxIcon.Warning);
                return;
            }
            Params param = new Params
            {
                database = "",
                cmd = (int)EnumCMD.EncryptFolder,
                path = EncryptDirectoryPath,
            };
            if (!CreateParamsFile(param)) return;
            Start();
        }

        private void DecryptDirectory(object parameter)
        {
            if (!Directory.Exists(EncryptDirectoryPath))
            {
                Notice.Show("路径不存在!", "通知", 3, MessageBoxIcon.Warning);
                return;
            }
            Params param = new Params
            {
                database = "",
                cmd = (int)EnumCMD.DecryptFolder,
                path = EncryptDirectoryPath,
            };
            if (!CreateParamsFile(param)) return;
            Start();
        }


        private void GetFileWindow(object parameter)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                EncryptFilePath = fileDialog.FileName.Trim();
            }
        }

        private void GetDirectoryWindow(object parameter)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                EncryptDirectoryPath = folderBrowserDialog.SelectedPath.Trim();
            }
        }

        private void GetFileServerPath(object parameter)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var path = folderBrowserDialog.SelectedPath.Trim();
                if (!path.EndsWith("files"))
                {
                    Notice.Show("文件服务器地址应选择files文件夹!", "通知", 3, MessageBoxIcon.Warning);
                    return;
                }
                HeadPath = path;
            }
        }

        private bool CheckDbs(string dbsStr)
        {
            if (string.IsNullOrWhiteSpace(dbsStr))
            {
                Notice.Show("请输入数据库名！", "通知", 3, MessageBoxIcon.Warning);
                return false;
            }
            string[] dbs = dbsStr.Replace('；',';').Split(';');
            for (int i = 0; i < dbs.Length; i++)
            {
                dbs[i] = dbs[i].Trim();
            }
            foreach (var item in dbs)
            {
                var connStr = GetConnStr().Replace("$db$", item);
                try
                {
                    MySqlHelper.ExecuteDataset(connStr, "select * from sc_operator limit 1");
                   
                }
                catch (Exception)
                {
                    Notice.Show("数据库连接失败，请确认数据库配置信息、数据库名是否正确！", "通知", 3, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 启动Encrypter.exe
        /// </summary>
        private void Start()
        {
            string executePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tool\\Encrypter\\JieShun.JieLink.DevOps.Encrypter.exe");
           ProcessHelper.StartProcessDotNet(executePath, null);
        }

        /// <summary>
        /// 生成配置文件
        /// </summary>
        /// <param name="param">参数</param>
        private bool CreateParamsFile(Params param)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tool\\Encrypter\\params.json");
            try
            {
                if (File.Exists(path)) File.Delete(path); 
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(JsonHelper.SerializeObject(param));
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Notice.Show("创建配置文件失败!", "通知", 3, MessageBoxIcon.Warning);
                return false;
            }
            return File.Exists(path);
        }

        private string GetFileServcerPath()
        {
            var path = string.Empty;
            try
            {
                //非管理员权限可能会异常
                //var process = Process.GetProcessesByName("JieShun.FileServer.Web").FirstOrDefault();
                var process = Process.GetProcessesByName("JieShun.FileServer.Web").FirstOrDefault();
                if (process != null)
                {
                    path = new FileInfo(process.MainModule.FileName).Directory.FullName;
                }
                else
                {
                    Notice.Show("请以管理员权限运行!", "通知", 3, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Notice.Show("请以管理员权限运行!", "通知", 3, MessageBoxIcon.Warning);
            }
            return path;
        }

        private string GetConnStr()
        {
            return $"Data Source={EnvironmentInfo.DbConnEntity.Ip};port={EnvironmentInfo.DbConnEntity.Port};User ID={EnvironmentInfo.DbConnEntity.UserName};Password={EnvironmentInfo.DbConnEntity.Password};Initial Catalog=$db$;Pooling=true;charset=utf8;";
        }
    }
}
