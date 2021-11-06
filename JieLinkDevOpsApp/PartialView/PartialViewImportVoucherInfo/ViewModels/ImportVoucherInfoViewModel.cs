using MySql.Data.MySqlClient;
using PartialViewImportInfoVoucher.Models;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewImportVoucherInfo.ViewModels
{
    public class ImportVoucherInfoViewModel : DependencyObject
    {
        public DelegateCommand CheckDataCommand { get; set; }

        public DelegateCommand ImportVoucherInfoCommand { get; set; }

        private bool canExecute = false;

        public ImportVoucherInfoViewModel()
        {
            CheckDataCommand = new DelegateCommand();
            CheckDataCommand.ExecuteAction = CheckData;

            ImportVoucherInfoCommand = new DelegateCommand();
            ImportVoucherInfoCommand.ExecuteAction = ImportVoucherInfo;
            ImportVoucherInfoCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return canExecute; });

            Message = "说明：本工具是为共享汽车导入凭证信息而制作，在用工具导入之前，至少要通过界面发行一个凭证信息，且开通了服务。\r\n1)将需要导入的车牌粘贴到txt文件中,一个车牌一行，比如VoucherInfos.txt\r\n2)将记事本文件另存为UTF8编码，必须为UTF8，否则汉字会乱码\r\n";

        }


        //ControlVoucher voucher = new ControlVoucher();

        Person person = new Person();
        Credential credential = new Credential();
        List<CredentialChannelRel> credentialChannelRels = new List<CredentialChannelRel>();
        /// <summary>
        /// 点击 “校验” 触发事件
        /// </summary>
        /// <param name="parameter"></param>
        private void CheckData(object parameter)
        {
            if (string.IsNullOrEmpty(PersonNo))
            {
                MessageBoxHelper.MessageBoxShowWarning("请先输入人事编号！");
                return;
            }

            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBoxHelper.MessageBoxShowWarning("请先指定文件路径！");
                return;
            }


            bool result = false;

            #region 检测是否存在这个人
            string selectPreson = string.Format("select * from  sc_person where PersonNo='{0}' and status=1 and IsDeleted=0 limit 1 ", PersonNo);

            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, selectPreson))
            {
                if (reader.Read())
                {
                    person.Id = reader["Id"].ToString();
                    person.NumberType = (Int32)reader["NumberType"];
                    person.IDNumber = reader["IDNumber"].ToString();
                    person.PersonNo = reader["PersonNo"].ToString();
                    person.Name = reader["Name"].ToString();
                    person.Gender = reader["Gender"].ToString();
                    person.GroupId = reader["GroupId"].ToString();
                }
                else
                {
                    ShowMessage("没有查询到该人事信息，请先填入人员信息！");
                    result = false;
                    return;
                }
            }
            #endregion

            #region 检测是否存在凭证
            string selectVocher = string.Format("select * from sc_credential where status=1 and personno='{0}' limit 1", PersonNo);
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, selectVocher))
            {
                if (reader.Read())
                {
                    credential.Id = reader["Id"].ToString();
                    credential.PersonNo = reader["PersonNo"].ToString();
                    credential.PersonName = reader["PersonName"].ToString();
                    credential.CredentialNo = reader["CredentialNo"].ToString();
                    credential.CredentialType = reader["CredentialType"].ToString();



                    result = true;
                }
                else
                {
                    ShowMessage("没有查询到该人事相关的凭证信息，请先至少发行一个凭证！");
                    result = false;
                    return;
                }
            }
            #endregion

            #region 检测是否存在已经开通过一个服务服务，且数据库的  凭证-通道关系表  中存在了  该对应的凭证  数据
            string selectCredentialChannel = string.Format("select * from pms_credential_channel_rel where CredentialId='{0}' ", credential.Id);
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, selectCredentialChannel))
            {
                while (reader.Read())
                {
                    CredentialChannelRel credentialChannelRel = new CredentialChannelRel();

                    credentialChannelRel.Id = reader["Id"].ToString();
                    credentialChannelRel.CredentialId = reader["CredentialId"].ToString();
                    credentialChannelRel.CredentialNo = reader["CredentialNo"].ToString();
                    credentialChannelRel.CredentialType = (Int32)reader["CredentialType"];
                    credentialChannelRel.CredentialStatus = (Int32)reader["CredentialStatus"];
                    credentialChannelRel.ChannelId = reader["ChannelId"].ToString();
                    credentialChannelRel.LeaseStallId = reader["LeaseStallId"].ToString();

                    credentialChannelRels.Add(credentialChannelRel);

                    //voucher.VGUID = reader["guid"].ToString();
                    //voucher.PGUID = reader["pguid"].ToString();
                    //voucher.LGUID = reader["lguid"].ToString();
                    //voucher.PersonNo = reader["personno"].ToString();
                    //voucher.VoucherType = int.Parse(reader["vouchertype"].ToString());
                    //voucher.VoucherNo = reader["voucherno"].ToString();
                    //voucher.CardNum = reader["cardnum"].ToString();
                    //voucher.AddOperatorNo = reader["addoperatorno"].ToString();
                    //voucher.AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //voucher.Status = 1;
                    //voucher.LastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //voucher.Remark = "add by program";
                    //voucher.StatusFromPerson = 1;
                    result = true;
                }


                if (credentialChannelRels.Count == 0)
                {
                    ShowMessage("没有查询到该人事对应的凭证所关联的服务，请先手动生成服务！");
                    result = false;
                    return;
                }
            }


            #endregion

            //string cmd = "select * from control_voucher where status!=4 and personno='{0}' limit 1";
            //using (MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, string.Format(cmd, PersonNo)))
            //{
            //    if (reader.Read())
            //    {
            //        //voucher.VGUID = reader["guid"].ToString();
            //        //voucher.PGUID = reader["pguid"].ToString();
            //        //voucher.LGUID = reader["lguid"].ToString();
            //        //voucher.PersonNo = reader["personno"].ToString();
            //        //voucher.VoucherType = int.Parse(reader["vouchertype"].ToString());
            //        //voucher.VoucherNo = reader["voucherno"].ToString();
            //        //voucher.CardNum = reader["cardnum"].ToString();
            //        //voucher.AddOperatorNo = reader["addoperatorno"].ToString();
            //        //voucher.AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //        //voucher.Status = 1;
            //        //voucher.LastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //        //voucher.Remark = "add by program";
            //        //voucher.StatusFromPerson = 1;
            //        result = true;
            //    }
            //    else
            //    {
            //        ShowMessage("没有查询到该人事相关的凭证信息，请先至少发行一个凭证！");
            //        result = false;
            //        return;
            //    }
            //}

            if (result)
            {
                if (!File.Exists(FilePath))
                {
                    ShowMessage("文件不存在！");
                    return;
                }

                if (!FilePath.EndsWith(".txt"))
                {
                    ShowMessage("非文本文件！");
                    return;
                }

                if (GetType(FilePath) != System.Text.Encoding.UTF8)
                {
                    ShowMessage("非UTF8编码！");
                    return;
                }
            }

            canExecute = true;
            ShowMessage("校验通过！");
        }

        /// <summary>
        /// 点击 “导入” 触发方法
        /// </summary>
        /// <param name="parameter"></param>
        private void ImportVoucherInfo(object parameter)
        {
            string[] arryVoucherInfo = File.ReadAllLines(FilePath);
            arryVoucherInfo = arryVoucherInfo.Distinct().ToArray();
            if (arryVoucherInfo.Length == 0)
            {
                ShowMessage("没有可导入的车牌");
                return;
            }
            var personNoP = PersonNo;
            if (credential == null || string.IsNullOrEmpty(credential.Id))
            {
                ShowMessage("请先校验！");
                return;
            }


            Task.Factory.StartNew(() =>
            {
                //List<string> pcsSqls = new List<string>();

                //List<string> insertVouchers = new List<string>();
                //List<string> insertVehicles = new List<string>();
                //List<string> insertCredentialChannelRels = new List<string>();
                foreach (var item in arryVoucherInfo)
                {
                    List<string> insertSqls = new List<string>();
                    //新增 凭证  (根据从数据库中获取的一个例子为主，主要替换掉ID ,凭证号，创建时间，更新时间，备注，车牌)
                    Credential insertCredential = new Credential()
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        CredentialNo = item,
                        CreateTime = DateTime.Now,
                        UpdatedTime = DateTime.Now,
                        Remark = "工具导入",
                        Plate = item
                    };

                    insertSqls.Add(string.Format(@" insert into sc_credential
                                                            (Id,PersonId,PersonNo,PersonName,CredentialNo,CredentialType,
                                                             StartTime,EndTime,                      
                                                             Status,IsDeleted,CreatedTime,CreatedOperatorId,CreatedOperatorNo,CreatedOperatorName,
                                                             UpdatedTime,UpdatedOperatorId,UpdatedOperatorNo,UpdatedOperatorName,
                                                             CardNum,  PhysicsNum,CardKey,Password,PhotoPath,DataPath,TxtData,BlobData,FileMd5,
                                                             IsVisitor,FingerNo,
                                                             Remark,
                                                             IsAntiFlag,
                                                             Plate,PlateColor,VehicleType)
                                                        select 
                                                             '{0}',PersonId,PersonNo,PersonName,'{1}',CredentialType,
                                                             StartTime,EndTime,                      
                                                             Status,IsDeleted,'{2}',CreatedOperatorId,CreatedOperatorNo,CreatedOperatorName,
                                                             '{3}',UpdatedOperatorId,UpdatedOperatorNo,UpdatedOperatorName,
                                                             CardNum,  PhysicsNum,CardKey,Password,PhotoPath,DataPath,TxtData,BlobData,FileMd5,
                                                             IsVisitor,FingerNo,
                                                             '{4}',
                                                             IsAntiFlag,
                                                             '{5}',PlateColor,VehicleType
                                                         from sc_credential
                                                         where PersonNo='{6}' limit 1
                                                          ", insertCredential.Id, insertCredential.CredentialNo, insertCredential.CreateTime, insertCredential.UpdatedTime, insertCredential.Remark, insertCredential.Plate,
                                                             personNoP));

                    //新增车辆信息
                    insertSqls.Add(string.Format(@"insert into pms_Vehicle 
                                                            ( Id,CredentialId,Plate,
                                                              PlateColor,VehicleBrand,VehicleColor,VehicleType,
                                                              VehicleSituation,PersonId,Status,OpenId,
                                                              Remark,CreatedTime,
                                                              FaceImageUrl
                                                            )  
                                                    select   replace(uuid(),'-','') ,'{0}','{1}',
                                                             PlateColor,VehicleBrand,VehicleColor,VehicleType,
                                                             VehicleSituation,PersonId,Status,OpenId,
                                                             '{2}','{3}',
                                                             FaceImageUrl
                                                    from pms_Vehicle 
                                                    where CredentialId='{4}' limit 1 ",
                                                        insertCredential.Id, item,
                                                        "工具导入", DateTime.Now,
                                                        credential.Id
                                                    ));

                    //新增  凭证- 通道 关系   信息
                    insertSqls.Add(string.Format(@" insert into pms_credential_channel_rel 
                                                                        (Id,CredentialId,CredentialNo,
                                                                        CredentialType,ChannelId,LeaseStallId,CredentialStatus
                                                                        ) 
                                                                    select 
                                                                        replace(uuid(),'-',''),'{0}','{1}',
                                                                        CredentialType,ChannelId,LeaseStallId,CredentialStatus
                                                                    from pms_credential_channel_rel 
                                                                    where CredentialId='{2}'  ",
                                                                        insertCredential.Id, item,
                                                                        credential.Id
                                                 ));




                    insertSqls.Add(string.Format(@" insert into {3}.pms_credential_channel_rel 
                                                                        (Id,CredentialId,CredentialNo,
                                                                        CredentialType,ChannelId,LeaseStallId,CredentialStatus
                                                                        ) 
                                                                    select 
                                                                        replace(uuid(),'-',''),'{0}','{1}',
                                                                        CredentialType,ChannelId,LeaseStallId,CredentialStatus
                                                                    from pms_credential_channel_rel 
                                                                    where CredentialId='{2}'  ",
                                                                        insertCredential.Id, item,
                                                                        credential.Id,
                                                                        EnvironmentInfo.DbConnEntity.DbName + "_pcs"
                                                 ));

                    insertSqls.Add(string.Format(@" update {1}.pms_credential_channel_rel 
                                                    set  ChannelId=(select LeaseStallId  from pms_credential_channel_rel where CredentialId='{0}' and  ChannelId ='11111111111111111111111111111111' )  
                                                    where CredentialId='{0}' and  ChannelId ='11111111111111111111111111111111'   ", insertCredential.Id, EnvironmentInfo.DbConnEntity.DbName + "_pcs"));

                    //insertSqls.Add(string.Format(@" update jielink_pcs.pms_credential_channel_rel 
                    //                                set  channel=(select LeaseStallId  from jielink_pcs.pms_credential_channel_rel where CredentialId='{0}' and  ChannelId ='11111111111111111111111111111111' )  
                    //                                where CredentialId='{0}' and  ChannelId ='11111111111111111111111111111111'   ", insertCredential.Id));

                    //执行sql 语句
                    try
                    {
                        using (MySqlConnection conn = new MySqlConnection(EnvironmentInfo.ConnectionString))
                        {
                            conn.Open();
                            MySqlTransaction transaction = conn.BeginTransaction();
                            MySqlCommand cmd = conn.CreateCommand();
                            cmd.CommandTimeout = int.MaxValue;//超时时间设置60分钟
                            cmd.Transaction = transaction;

                            try
                            {
                                foreach (var insertSql in insertSqls)
                                {
                                    LogHelper.CommLogger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "导入凭证信息执行sql：" + insertSql);
                                    cmd.CommandText = insertSql;
                                    int x = cmd.ExecuteNonQuery();
                                    if (x <= 0)
                                    {
                                        throw new Exception("新增失败!sql语句为: " + insertSql);
                                    }

                                }

                                transaction.Commit();
                                ShowMessage($"导入凭证 {item} 成功！");
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                LogHelper.CommLogger.Error("数据新增遇到些问题，事务回滚：" + ex.ToString());
                                ShowMessage("数据新增遇到些问题，事务回滚：" + ex.ToString());
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LogHelper.CommLogger.Error("数据新增遇到些问题" + ex.ToString());
                        ShowMessage("数据新增遇到些问题" + ex.ToString());
                    }
                }

            });
        }


        public void ShowMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (Message != null && Message.Length > 5000)
                {
                    Message = string.Empty;
                }

                if (message.Length > 0)
                {
                    Message += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
                }
            }));
        }

        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// <param name="fileName">文件路径</param>
        /// <returns>文件的编码类型</returns>

        public static Encoding GetType(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static Encoding GetType(FileStream fs)
        {
            Encoding reVal = Encoding.Default;
            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;  //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }




        public string PersonNo
        {
            get { return (string)GetValue(PersonNoProperty); }
            set { SetValue(PersonNoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PersonNo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PersonNoProperty =
            DependencyProperty.Register("PersonNo", typeof(string), typeof(ImportVoucherInfoViewModel));


        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(ImportVoucherInfoViewModel));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ImportVoucherInfoViewModel));

    }
}
