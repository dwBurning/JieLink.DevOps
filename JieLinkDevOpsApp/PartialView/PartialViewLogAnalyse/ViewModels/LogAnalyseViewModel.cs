using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using PartialViewInterface.Commands;
using PartialViewLogAnalyse.Models;
using PartialViewLogAnalyse.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewLogAnalyse.ViewModels
{
    public class LogAnalyseViewModel : DependencyObject
    {

        public DelegateCommand SelectPathCommand { get; set; }
        public DelegateCommand AnalyseCommand { get; set; }
        public DelegateCommand SelectXmppPathCommand { get; set; }
        public DelegateCommand AnalyseXmppCommand { get; set; }
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(LogAnalyseViewModel), new PropertyMetadata(""));


        public string Plate
        {
            get { return (string)GetValue(PlateProperty); }
            set { SetValue(PlateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Plate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlateProperty =
            DependencyProperty.Register("Plate", typeof(string), typeof(LogAnalyseViewModel), new PropertyMetadata(""));



        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeviceName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeviceNameProperty =
            DependencyProperty.Register("DeviceName", typeof(string), typeof(LogAnalyseViewModel), new PropertyMetadata(""));



        public string Result
        {
            get { return (string)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Result.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(string), typeof(LogAnalyseViewModel), new PropertyMetadata(""));

        public string XmppFilePath
        {
            get { return (string)GetValue(XmppFilePathProperty); }
            set
            {
                SetValue(XmppFilePathProperty, value);
                xmppFilePath = value;
            }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XmppFilePathProperty =
            DependencyProperty.Register("XmppFilePath", typeof(string), typeof(LogAnalyseViewModel), new PropertyMetadata(""));

        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register("FilterText", typeof(string), typeof(LogAnalyseViewModel), new PropertyMetadata("jsg3-of"));


        public string XmppResult
        {
            get { return (string)GetValue(XmppResultProperty); }
            set { SetValue(XmppResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Result.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XmppResultProperty =
            DependencyProperty.Register("XmppResult", typeof(string), typeof(LogAnalyseViewModel), new PropertyMetadata(""));

        private List<string> filePathList = new List<string>();
        private string plate = string.Empty;
        private string deviceName = string.Empty;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private IPendingHandler pendingHandler = null;

        private List<string> xmppFilePathList = new List<string>();
        private BackgroundWorker backgroundWorkerXmpp = new BackgroundWorker();
        private string filterText = "";
        private string xmppFilePath = "";
        public LogAnalyseViewModel()
        {
            SelectPathCommand = new DelegateCommand();
            SelectPathCommand.ExecuteAction = SelectPath;
            AnalyseCommand = new DelegateCommand();
            AnalyseCommand.ExecuteAction = Analyse;
            AnalyseCommand.CanExecuteFunc = o => filePathList.Count > 0;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

            SelectXmppPathCommand = new DelegateCommand();
            SelectXmppPathCommand.ExecuteAction = SelectXmppPath;
            AnalyseXmppCommand = new DelegateCommand();
            AnalyseXmppCommand.ExecuteAction = AnalyseXmpp;
            AnalyseXmppCommand.CanExecuteFunc = o => !string.IsNullOrEmpty(XmppFilePath);
            backgroundWorkerXmpp.WorkerSupportsCancellation = true;
            backgroundWorkerXmpp.WorkerReportsProgress = true;
            backgroundWorkerXmpp.DoWork += BackgroundWorkerXmpp_DoWork;
            backgroundWorkerXmpp.RunWorkerCompleted += BackgroundWorkerXmpp_RunWorkerCompleted;
            backgroundWorkerXmpp.ProgressChanged += BackgroundWorkerXmpp_ProgressChanged;
        }



        private void SelectPath(object parameter)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Multiselect = true;
            var process = Process.GetProcessesByName("SmartCenter.Host").FirstOrDefault();
            if (process != null)
            {
                fileDialog.InitialDirectory = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.FullName, "logs");
            }
            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = string.Join(";", fileDialog.FileNames);
                filePathList = fileDialog.FileNames.ToList();
                filePathList = filePathList.FindAll(x => x.Contains("JieLink_Center_Comm") || x.Contains("JieLink_CENTER_2"));
                //按时间排序
                filePathList.Sort((a, b) =>
                {
                    return int.Parse(b.Substring(b.LastIndexOf('.') + 1).Replace("log", "0"))
                             - int.Parse(a.Substring(a.LastIndexOf('.') + 1).Replace("log", "0"));
                });

            }
        }
        private void Analyse(object parameter)
        {
            if (backgroundWorker.IsBusy)
            {
                return;
            }
            plate = Plate;//后台线程访问会有问题，赋值个临时字段解决
            deviceName = DeviceName;
            backgroundWorker.RunWorkerAsync();
            PendingBoxConfigurations configurations = new PendingBoxConfigurations();
            configurations.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            pendingHandler = PendingBox.Show(string.Format("正在分析日志文件...({0}%)", 0), "请等待", false, Application.Current.MainWindow, configurations);
        }
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                RecordContext context = new RecordContext();
                context.OrderRecords = new List<JspayOrderRecord>();
                context.ParkRecords = new List<ParkRecord>();
                context.DeviceCache = new List<DeviceInfo>();
                List<string> lastLines = new List<string>();
                int totalLines = AnalyseUtils.GetTotalLines(filePathList);
                if (totalLines == 0)
                {
                    throw new Exception("日志内容为空");
                }
                int currentLines = 0;
                int currentPencent = 0;
                foreach (var filePath in filePathList)
                {
                    using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("gb2312")))
                    {
                        while (!sr.EndOfStream)
                        {

                            string line = sr.ReadLine();
                            AnalyseUtils.ParseRecord(context, line, lastLines);
                            AnalyseUtils.ParseOrderRecord(context, line, lastLines);
                            if (lastLines.Count > 100)
                                lastLines.RemoveAt(0);
                            lastLines.Add(line);

                            //更新进度
                            currentLines++;
                            int pencent = (currentLines * 100) / totalLines;
                            if (pencent > 99)
                                pencent = 99;
                            if (pencent != currentPencent)
                            {
                                backgroundWorker.ReportProgress(pencent);
                            }
                            currentPencent = pencent;
                        }
                    }
                }
                AnalyseUtils.MergeParkRecordAndOrder(context);

                StringBuilder sb = new StringBuilder();
                foreach (var parkRecord in context.ParkRecords)
                {
                    if ((string.IsNullOrEmpty(plate) || AnalyseUtils.FuzzyCompare(parkRecord.CredentialNo, plate) || parkRecord.HistoryCredentialNo.Any(y => AnalyseUtils.FuzzyCompare(y, plate)))
                        && (string.IsNullOrEmpty(deviceName) || parkRecord.DeviceName.Contains(deviceName))
                        )
                    {
                        sb.Append("=======================");
                        sb.Append(parkRecord.CredentialNo);
                        sb.Append("=======================");
                        sb.Append(Environment.NewLine);
                        foreach (var logNode in parkRecord.LogNodes)
                        {
                            sb.Append(logNode.LogTime.ToString("yyyy-MM-dd HH:mm:ss,fff "));
                            sb.Append(logNode.Message);
                            sb.Append(Environment.NewLine);

                        }
                    }

                }
                e.Result = sb.ToString();
            }
            catch (Exception ex)
            {
                e.Result = "日志分析异常：" + ex.ToString();
            }

        }
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (pendingHandler != null)
                pendingHandler.UpdateMessage(string.Format("正在分析日志文件...({0}%)", e.ProgressPercentage));
        }
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (pendingHandler != null)
                pendingHandler.Close();
            string log = e.Result as string;
            if (!string.IsNullOrEmpty(log))
            {
                this.Result = log;
            }
            else
            {
                this.Result = "无搜索结果";
            }

        }


        private void SelectXmppPath(object parameter)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            //fileDialog.Multiselect = true;
            var process = Process.GetProcessesByName("SmartCenter.Host").FirstOrDefault();
            if (process != null)
            {
                fileDialog.InitialDirectory = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.FullName, "XmppLog");
            }
            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                XmppFilePath = fileDialog.FileName;

            }
        }
        private void AnalyseXmpp(object parameter)
        {
            if (backgroundWorkerXmpp.IsBusy)
            {
                return;
            }
            this.XmppResult = "";
            filterText = FilterText;

            backgroundWorkerXmpp.RunWorkerAsync();
            PendingBoxConfigurations configurations = new PendingBoxConfigurations();
            configurations.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            pendingHandler = PendingBox.Show(string.Format("正在分析日志文件...({0}%)", 0), "请等待", false, Application.Current.MainWindow, configurations);
        }
        private void BackgroundWorkerXmpp_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                List<ReceiveThreadContext> contexts = new List<ReceiveThreadContext>();
                int totalLines = AnalyseUtils.GetTotalLines(new List<string>() { xmppFilePath });
                int currentLines = 0;
                int currentPencent = 0;
                using (StreamReader sr = new StreamReader(xmppFilePath, Encoding.GetEncoding("gb2312")))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        int firstIndex = line.IndexOf('[');
                        if (firstIndex > 0)
                        {
                            string strLogTime = line.Substring(line.IndexOf('|') + 2, 23);
                            DateTime logTime = DateTime.Now;
                            try
                            {
                                logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }

                            string threadNum = line.Substring(firstIndex + 1, line.IndexOf(']') - line.IndexOf('[') - 1);

                            if (line.IndexOf("读取流--开始") >= 0)
                            {
                                ReceiveThreadContext context = contexts.FindLast(x => x.ThreadNum == threadNum);
                                if (context == null || context.EndTime.Year > 2000)
                                {
                                    context = new ReceiveThreadContext();
                                    context.ThreadNum = threadNum;
                                    context.StartTime = logTime;
                                    contexts.Add(context);
                                }
                                if (context.LastLines.Count > 1)
                                {
                                    context.LastLines.RemoveAt(1);
                                }
                                ReceiveData receiveData = new ReceiveData();
                                receiveData.Content = line;
                                receiveData.LogTime = logTime;
                                context.LastLines.Add(receiveData);
                            }
                            if (line.IndexOf("断开") >= 0)
                            {
                                //Console.WriteLine(threadNum);
                                ReceiveThreadContext context = contexts.FindLast(x => x.ThreadNum == threadNum);
                                if (context != null && context.EndTime.Year < 2000)
                                {
                                    context.EndTime = DateTime.Now;
                                    ReceiveData receiveData = new ReceiveData();
                                    receiveData.Content = line;
                                    receiveData.LogTime = logTime;
                                    context.LastLines.Add(receiveData);
                                }
                                else
                                {
                                    Console.WriteLine(line);
                                }
                            }
                            if (line.IndexOf("接收到数据：<iq id") > 0 && line.IndexOf(" type=\"get") > 0)
                            {
                                string name = line.Substring(line.IndexOf("to=") + 4, line.IndexOf("\"", line.IndexOf("to=") + 4) - line.IndexOf("to=") - 4);
                                ReceiveThreadContext context = contexts.FindLast(x => x.ThreadNum == threadNum);
                                if (context != null && context.EndTime.Year < 2000)
                                {
                                    context.Name = name;
                                    //ReceiveData receiveData = new ReceiveData();
                                    //receiveData.Content = line;
                                    //receiveData.LogTime = logTime;
                                    //context.LastLines.Add(receiveData);
                                }
                            }


                        }
                        //更新进度
                        currentLines++;
                        int pencent = (currentLines * 100) / totalLines;
                        if (pencent > 99)
                            pencent = 99;
                        if (pencent != currentPencent)
                        {
                            backgroundWorkerXmpp.ReportProgress(pencent);
                        }
                        currentPencent = pencent;
                    }
                }
                StringBuilder sb = new StringBuilder();
                foreach (var context in contexts)
                {
                    if (string.IsNullOrEmpty(filterText) || (context.Name != null && !context.Name.Contains(filterText)))
                    {
                        sb.Append(string.Format("======={0}=======\n", context.Name));
                        foreach (var data in context.LastLines)
                        {
                            sb.Append(data.Content.Substring(data.Content.IndexOf('|')));
                            sb.Append(Environment.NewLine);
                        }
                        sb.Append(Environment.NewLine);
                    }
                }
                string result = sb.ToString();
                e.Result = string.IsNullOrEmpty(result) ? "无结果" : result;
            }
            catch (Exception ex)
            {
                e.Result = "日志分析异常：" + ex.ToString();
            }

        }

        private void BackgroundWorkerXmpp_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (pendingHandler != null)
                pendingHandler.UpdateMessage(string.Format("正在分析日志文件...({0}%)", e.ProgressPercentage));
        }
        private void BackgroundWorkerXmpp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (pendingHandler != null)
                pendingHandler.Close();
            string log = e.Result as string;
            if (!string.IsNullOrEmpty(log))
            {
                this.XmppResult = log;
            }
            else
            {
                this.XmppResult = "无搜索结果";
            }

        }
    }
}
