using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using PartialViewOtherToJieLink.JSDSViewModels;
using PartialViewOtherToJieLink.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;

namespace PartialViewOtherToJieLink.ViewModels
{
    public class JSDSToJieLinkViewModel : DependencyObject
    {
        private readonly string REMARK = "JSDS";

        public DelegateCommand TestConnCommand { get; set; }

        public DelegateCommand UpgradeCommand { get; set; }

        public bool CanExecute { get; set; }

        public string JsdsIp { get; set; }

        public string JsdsDbConnString { get; private set; }

        JsdsUpgradePolicy policy = new JsdsUpgradePolicy();
        /// <summary>
        /// key为jsds不能转换为guid的字符串，value为映射是guid的字符串
        /// </summary>
        Dictionary<string, string> guidMapDic = new Dictionary<string, string>();
        /// <summary>
        /// 匹配不到入场记录的出场记录，入场id可能为空或者0，生成新的入场id
        /// </summary>
        Dictionary<string, string> recordOutIdMapDic = new Dictionary<string, string>();
        /// <summary>
        /// jsds区域迁移到jielink时，生成车场和门禁区域，区域若没有关联设备，可直接删除
        /// 门禁区域的guid映射好，后面可能用到
        /// </summary>
        Dictionary<string, string> areaGuidMapDic = new Dictionary<string, string>();

        public JSDSToJieLinkViewModel()
        {
            TestConnCommand = new DelegateCommand();
            TestConnCommand.ExecuteAction = TestConn;

            UpgradeCommand = new DelegateCommand();
            UpgradeCommand.ExecuteAction = Upgrade;
            UpgradeCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return CanExecute; });

            ImportTimeDataSource = new Dictionary<int, string>();
            ImportTimeDataSource.Add(1, "最近1个月");
            ImportTimeDataSource.Add(3, "最近3个月");
            ImportTimeDataSource.Add(6, "最近6个月");
            ImportTimeDataSource.Add(9, "最近9个月");
            ImportTimeDataSource.Add(12, "最近12个月");
            this.SelectMonth = 3;

            this.Port = 3306;
        }

        private void TestConn(object parameter)
        {
            PasswordBox passwordBox = (PasswordBox)parameter;
            this.Password = passwordBox.Password;

            if (!CheckValid())
            {
                MessageBoxHelper.MessageBoxShowWarning("请填写完整的数据库配置信息！");
                return;
            }

            string connStr = $"Data Source={this.IpAddr};port={this.Port};User ID={this.UserName};Password={this.Password};Initial Catalog={this.DbName};";

            try
            {
                string cmd = "select * from t_cac_account";
                MySqlHelper.ExecuteDataset(connStr, cmd);
                JsdsDbConnString = connStr;
                CanExecute = true;
                ShowMessage("JSDS数据库连接成功");
                Notice.Show("JSDS数据库连接成功", "通知", 3, MessageBoxIcon.Success);
            }
            catch (Exception ex)
            {
                CanExecute = false;
                MessageBoxHelper.MessageBoxShowWarning("连接错误，请重新输入JSDS数据库连接信息");
            }
        }

        private bool CheckValid()
        {
            if (string.IsNullOrEmpty(IpAddr)
                || string.IsNullOrEmpty(Password)
                || string.IsNullOrEmpty(UserName)
                || string.IsNullOrEmpty(DbName)
                || this.SelectMonth == 0)
            {
                return false;
            }
            return true;
        }

        //缓存sql配置信息
        private DbConnectionModel GetDbConnectionModel()
        {
            return new DbConnectionModel
            {
                Server = this.IpAddr,
                DbName = this.DbName,
                UserName = this.UserName,
                Pwd = this.Password
            };
        }

        private void Upgrade(object parameter)
        {
            try
            {
                //判断数据库是否jsds
                string cmd = "select * from t_cac_account";
                MySqlHelper.ExecuteDataset(JsdsDbConnString, cmd);

                policy.EnterRecordSelect = this.EnterRecord;
                policy.OutRecordSelect = this.OutRecord;
                policy.BillRecordSelect = this.BillRecord;
                policy.ParkSelect = this.Park;
                policy.DoorSelect = this.Door;
                policy.ControlDeviceSelect = this.ControlDevice;
                policy.DeviceRightSelect = this.DeviceRight;
                if (!policy.DeviceRightSelect && policy.ControlDeviceSelect)
                {
                    ShowMessage("没有选择设备的情况下，请勿选择设备权限");
                    return;
                }
                if (!policy.EnterRecordSelect && (policy.OutRecordSelect || policy.BillRecordSelect))
                {
                    ShowMessage("没有选择入场记录的情况下，请勿选择出场记录或收费记录");
                    return;
                }
                else if (!policy.OutRecordSelect && policy.BillRecordSelect)
                {
                    ShowMessage("没有选择出场记录的情况下，请勿选择收费记录");
                    return;
                }
                policy.SelectedTimeIndex = CommonHelper.GetIntValue(this.SelectMonth, 3);
                Task.Factory.StartNew(() =>
                {
                    CanExecute = false;
                    StartUpgrade();
                    CanExecute = true;
                });
            }
            catch (Exception ex)
            {
                ShowMessage("一键升级错误，详情请分析日志");
                MessageBoxHelper.MessageBoxShowWarning("一键升级错误，详情请分析日志");
            }
            finally
            {
                CanExecute = true;
            }
        }


        /// <summary>
        /// 开始升级
        /// </summary>
        private void StartUpgrade()
        {
            try
            {
                //string defaultServiceGuid = Guid.NewGuid().ToString();  //对应jsds默认用户开通的车场服务
                //开始进行数据转换
                //1、组织control_role_group
                DataSet dbGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_role_group WHERE `Status`=0 ORDER BY id ASC;");
                List<ControlRoleGroup> dbGroupList = new List<ControlRoleGroup>();
                ControlRoleGroup groupRoot = null;
                if (dbGroupDs != null && dbGroupDs.Tables[0] != null)
                {
                    dbGroupList = CommonHelper.DataTableToList<ControlRoleGroup>(dbGroupDs.Tables[0]).OrderBy(x => x.ID).ToList();
                    groupRoot = dbGroupList.FirstOrDefault(x => x.ParentId == ConstantHelper.ROOTPARENTID);
                }
                DataSet groupDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_base_organize WHERE STATE='normal' ORDER BY ORG_ID ASC;");
                List<string> groupSuccessImportList = new List<string>();
                if (groupDs != null && groupDs.Tables[0] != null)
                {
                    ShowMessage("01.迁移组织");
                    List<TBaseOrganizeModel> tempGroupList = CommonHelper.DataTableToList<TBaseOrganizeModel>(groupDs.Tables[0]).OrderBy(x => x.ORG_ID).ToList();
                    List<TBaseOrganizeModel> groupList = GetOrganizeChildren(tempGroupList, "");
                    if (groupList.Count > 0)
                    {
                        //存储jsds的根节点GUID：保持根节点原先的RGGUID不变，处理jsds根节点下的子组织时，将ORG_ID与jsdsRootId比较
                        //因为jielink中组织guid涉及的表结构很多，因此组织根节点的guid保持不动
                        string jsdsRootId = string.Empty;
                        foreach (TBaseOrganizeModel group in groupList)
                        {
                            if (string.IsNullOrWhiteSpace(group.REMARK))
                            {
                                group.REMARK = REMARK;
                            }
                            if (string.IsNullOrWhiteSpace(group.ORG_ID))
                            {
                                jsdsRootId = group.ID;
                                //根组织节点
                                if (groupRoot == null)
                                {
                                    string orgCode = new Random().ToString().Substring(2, 8);
                                    string rgguid = new Guid(group.ID).ToString();
                                    string sql = string.Format("INSERT INTO control_role_group(RGGUID,RGName,RGCode,ParentId,RGType,`Status`,CreatedOnUtc,Remark,RGFullPath) VALUE('{0}','{1}','{2}','{3}',1,0,'{4}','{5}','{6}');", rgguid, group.ORG_NAME, orgCode, ConstantHelper.ROOTPARENTID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), group.REMARK, group.ORG_NAME + ";");
                                    try
                                    {
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag <= 0)
                                        {
                                            ShowMessage("组织根节点插入失败");
                                            MessageBoxHelper.MessageBoxShowWarning("组织根节点插入失败");
                                            return;
                                        }
                                    }
                                    catch (Exception o)
                                    {
                                        ShowMessage("组织根节点插入异常：详情请分析日志");
                                        LogHelper.CommLogger.Error(o, "组织根节点插入异常：");
                                        return;
                                    }
                                    groupSuccessImportList.Add(rgguid);
                                }
                                else
                                {
                                    string sql = string.Format("UPDATE control_role_group SET Remark='{0}' WHERE RGGUID='{1}' AND ParentId='{2}';", group.REMARK, groupRoot.RGGUID, ConstantHelper.ROOTPARENTID);
                                    try
                                    {
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag <= 0)
                                        {
                                            ShowMessage("组织根节点更新失败");
                                            return;
                                        }
                                    }
                                    catch (Exception o)
                                    {
                                        ShowMessage("组织根节点更新异常：详情请分析日志");
                                        LogHelper.CommLogger.Error(o, "组织根节点更新异常：");
                                        return;
                                    }
                                    groupSuccessImportList.Add(groupRoot.RGGUID);
                                }
                            }
                            else
                            {
                                string currentGuid = new Guid(group.ID).ToString();
                                //寻找当前组织是否已存在：判断是否重复升级
                                DataSet currentGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_role_group WHERE `Status`=0 AND RGGUID='{0}'", currentGuid));
                                ControlRoleGroup currentGroup = null;
                                if (currentGroupDs != null && currentGroupDs.Tables[0] != null)
                                {
                                    currentGroup = CommonHelper.DataTableToList<ControlRoleGroup>(currentGroupDs.Tables[0]).FirstOrDefault();
                                }
                                if (currentGroup != null)
                                {
                                    ShowMessage("组织" + group.ORG_NAME + "已存在，跳过");
                                    continue;
                                }

                                //找上一节点组织：因为要对全路径赋值
                                if (group.ORG_ID.Equals(jsdsRootId))
                                {
                                    group.ORG_ID = groupRoot.RGGUID;
                                }
                                DataSet parentGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_role_group WHERE `Status`=0 AND RGGUID='{0}'", new Guid(group.ORG_ID).ToString()));
                                ControlRoleGroup parentGroup = null;
                                if (parentGroupDs != null && parentGroupDs.Tables[0] != null)
                                {
                                    parentGroup = CommonHelper.DataTableToList<ControlRoleGroup>(parentGroupDs.Tables[0]).FirstOrDefault();
                                }
                                if (parentGroup == null)
                                {
                                    ShowMessage("组织" + group.ORG_NAME + "的父节点组织不存在，跳过");
                                    continue;
                                }
                                string rgFullPath = string.Format("{0}|{1};", parentGroup.RGFullPath.Trim(';'), group.ORG_NAME);
                                //其他子组织
                                string orgCode = new Random().ToString().Substring(2, 8);
                                string sql = string.Format("INSERT INTO control_role_group(RGGUID,RGName,RGCode,ParentId,RGType,`Status`,CreatedOnUtc,Remark,RGFullPath) VALUE('{0}','{1}','{2}','{3}',1,0,'{4}','{5}','{6}');", currentGuid, group.ORG_NAME, orgCode, parentGroup.RGGUID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), group.REMARK, rgFullPath);
                                try
                                {
                                    int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                    if (flag <= 0)
                                    {
                                        ShowMessage("组织" + group.ORG_NAME + "插入失败");
                                        continue;
                                    }
                                }
                                catch (Exception o)
                                {
                                    ShowMessage("组织" + group.ORG_NAME + "插入异常：详情请分析日志");
                                    LogHelper.CommLogger.Error(o, "组织" + group.ORG_NAME + "插入异常：");

                                    continue;
                                }
                                groupSuccessImportList.Add(currentGuid);
                            }
                        }
                    }
                    string msg = string.Format("01.迁移组织结束，合计处理{0}条组织数据，成功迁移{1}条组织", groupList.Count, groupSuccessImportList.Count);
                    ShowMessage(msg);
                    LogHelper.CommLogger.Info(msg);

                    groupList.Clear();
                    groupSuccessImportList.Clear();
                }
                else
                {
                    ShowMessage("01.迁移组织结束：groupDs == null || groupDs.Tables[0] == null");
                    LogHelper.CommLogger.Info("01.迁移组织结束：groupDs == null || groupDs.Tables[0] == null");
                }

                //2、重新查询组织
                dbGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_role_group WHERE `Status`=0 ORDER BY id ASC;");
                dbGroupList = new List<ControlRoleGroup>();
                groupRoot = null;
                if (dbGroupDs != null && dbGroupDs.Tables[0] != null)
                {
                    dbGroupList = CommonHelper.DataTableToList<ControlRoleGroup>(dbGroupDs.Tables[0]).OrderBy(x => x.ID).ToList();
                    groupRoot = dbGroupList.FirstOrDefault(x => x.ParentId == ConstantHelper.ROOTPARENTID);
                }
                if (groupRoot == null)
                {
                    ShowMessage("组织根节点不存在，请确认数据库是否正确");
                    MessageBoxHelper.MessageBoxShowWarning("组织根节点不存在，请确认数据库是否正确");
                    return;
                }
                //3、人事
                //DataSet personDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_base_person WHERE STATE='NORMAL' AND ID in (SELECT PERSON_ID from t_cac_account where `STATUS`='NORMAL' ORDER BY CREATE_DATE ASC) ORDER BY PERSON_SN asc;");
                DataSet personDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_base_person WHERE STATE='NORMAL' ORDER BY CREATE_TIME asc;");
                List<TBasePersonModel> personList = new List<TBasePersonModel>();
                List<TBasePersonKeyModel> personKeyList = new List<TBasePersonKeyModel>();
                if (personDs != null && personDs.Tables[0] != null)
                {
                    personList = CommonHelper.DataTableToList<TBasePersonModel>(personDs.Tables[0]).OrderBy(x => x.PERSON_SN).ToList();
                    DataSet personKeyDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_base_person_key WHERE ID in (SELECT ID from t_base_person WHERE STATE='NORMAL' ORDER BY PERSON_SN asc);");
                    if (personKeyDs != null && personKeyDs.Tables[0] != null)
                    {
                        personKeyList = CommonHelper.DataTableToList<TBasePersonKeyModel>(personKeyDs.Tables[0]);
                    }
                }
                List<string> personSuccessImportList = new List<string>();   //用户成功导入数
                if (personList.Count > 0)
                {
                    ShowMessage("02.迁移用户");
                    string idNumber = "311311194910010099";
                    int userType = 2;   //业主
                    int relationship = 0;   //与户主关系：本人
                    int gender = 1; //男         
                    foreach (TBasePersonModel personModel in personList)
                    {
                        if (string.IsNullOrWhiteSpace(personModel.CODE))
                        {
                            //跳过
                            continue;
                        }
                        string personGuid = GetGuidString(personModel.ID);
                        DataSet currentPersonDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_person WHERE `Status`=0 AND PGUID='{0}'", personGuid));
                        ControlPerson currentPerson = null;
                        if (currentPersonDs != null && currentPersonDs.Tables[0] != null)
                        {
                            currentPerson = CommonHelper.DataTableToList<ControlPerson>(currentPersonDs.Tables[0]).FirstOrDefault();
                        }
                        if (currentPerson != null)
                        {
                            //重复，直接跳过，以第一条为准
                            continue;
                            //ShowMessage("不可重复一键升级");
                            //MessageBoxHelper.MessageBoxShowWarning("不可重复一键升级");
                            //return;
                        }

                        using (TransactionScope transaction = new TransactionScope())
                        {
                            ControlRoleGroup currentGroup = null;
                            if (string.IsNullOrWhiteSpace(personModel.ORG_ID))
                            {
                                //组织为空时，放到根组织下
                                currentGroup = new ControlRoleGroup()
                                {
                                    RGFullPath = groupRoot.RGFullPath,
                                    RGGUID = groupRoot.RGGUID
                                };
                            }
                            else
                            {
                                string currentGuid = new Guid(personModel.ORG_ID).ToString();
                                //寻找当前组织是否已存在：判断是否重复升级
                                DataSet currentGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_role_group WHERE `Status`=0 AND RGGUID='{0}'", currentGuid));
                                if (currentGroupDs != null && currentGroupDs.Tables[0] != null)
                                {
                                    currentGroup = CommonHelper.DataTableToList<ControlRoleGroup>(currentGroupDs.Tables[0]).FirstOrDefault();
                                }
                                if (currentGroup == null)
                                {
                                    //组织为空时，放到根组织下
                                    currentGroup = new ControlRoleGroup()
                                    {
                                        RGFullPath = groupRoot.RGFullPath,
                                        RGGUID = groupRoot.RGGUID
                                    };
                                }
                            }
                            TBasePersonKeyModel personkeyModel = personKeyList.FirstOrDefault(x => x.ID == personModel.ID);
                            string curKey = string.Empty;
                            if (personkeyModel != null)
                            {
                                curKey = personkeyModel.DYNAMIC_KEY;
                            }
                            else
                            {
                                curKey = new Random().Next(10000000, 80000000).ToString().PadLeft(8, '0');
                            }
                            if (!string.IsNullOrWhiteSpace(personModel.SEX) && personModel.SEX.ToUpper().Equals("FEMALE"))
                            {
                                gender = 0;
                            }
                            if (string.IsNullOrWhiteSpace(personModel.TEL) || personModel.TEL.Length <= 7)
                            {
                                //虚构
                                personModel.TEL = "189" + new Random().Next(10000000, 99999999).ToString().PadLeft(8, '0');
                            }
                            if (string.IsNullOrWhiteSpace(personModel.REMARK))
                            {
                                personModel.REMARK = REMARK;
                            }
                            string personId = "";   //不可能为空
                            do
                            {
                                DataSet personIdDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from sys_parameter WHERE ParaName='Person';");
                                if (personIdDs != null && personIdDs.Tables[0] != null)
                                {
                                    SysParameter parameter = CommonHelper.DataTableToList<SysParameter>(personIdDs.Tables[0]).FirstOrDefault();
                                    if (parameter != null)
                                    {
                                        personId = parameter.ParaValue.ToString().PadLeft(6, '0');
                                        //更新ParaValue
                                        MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("UPDATE sys_parameter SET ParaValue={0} WHERE ParaName='Person';", parameter.ParaValue + 1));

                                        currentPersonDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_person WHERE `Status`=0 AND PersonId='{0}'", personId));
                                        currentPerson = null;
                                        if (currentPersonDs != null && currentPersonDs.Tables[0] != null)
                                        {
                                            currentPerson = CommonHelper.DataTableToList<ControlPerson>(currentPersonDs.Tables[0]).FirstOrDefault();
                                        }
                                    }
                                }
                            } while (currentPerson != null);
                            bool completeFlag = false;  //事务提交标识
                            string sql = string.Format("INSERT INTO control_person(PGUID,PersonNo,PersonName,Gender,Mobile,Email,Relationship,RID,EnterTime,Type,`Status`,Remark,CreateTime,CurKey,LastKey,RFullPath,PersonId,CanInOut,IsTKService,IsParkService,IsDoorService,IsIssueCard,IDNumber) VALUE('{0}','{1}','{2}',{3},'{4}','{5}',{6},'{7}','{8}',{9},0,'{10}','{11}',{12},{13},'{14}','{15}',0,0,0,0,0,'{16}')",
                                personGuid, personModel.CODE, personModel.NAME, gender, personModel.TEL, personModel.EMAIL, relationship, currentGroup.RGGUID,
                                personModel.CREATE_TIME, userType, personModel.REMARK, personModel.CREATE_TIME, curKey, curKey, currentGroup.RGFullPath, personId, idNumber);
                            try
                            {
                                int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                if (flag <= 0)
                                {
                                    ShowMessage(string.Format("02.迁移用户，用户{0}【{1}】新增失败", personModel.NAME, personModel.ID));
                                }
                                else
                                {
                                    //组织与用户关系
                                    sql = string.Format("INSERT control_person_group(PGGUID,RGGUID,PGUID,`Status`) VALUE('{0}','{1}','{2}',0)", Guid.NewGuid().ToString(), currentGroup.RGGUID, personGuid);
                                    try
                                    {
                                        flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag <= 0)
                                        {
                                            ShowMessage(string.Format("02.迁移用户，用户与组织关系{0}【{1}】新增失败", currentGroup.RGGUID, personModel.CODE));
                                        }
                                        else
                                        {
                                            completeFlag = true;
                                        }
                                    }
                                    catch (Exception o)
                                    {
                                        ShowMessage(string.Format("02.迁移用户，用户与组织关系{0}【{1}】新增异常：详情请分析日志", currentGroup.RGGUID, personModel.CODE));
                                        LogHelper.CommLogger.Error(o, string.Format("02.迁移用户，用户与组织关系{0}【{1}】新增异常：", currentGroup.RGGUID, personModel.CODE));
                                    }
                                }
                            }
                            catch (Exception o)
                            {
                                ShowMessage(string.Format("02.迁移用户，用户{0}【{1}】新增异常：详情请分析日志", personModel.NAME, personModel.CODE));
                                LogHelper.CommLogger.Error(o, string.Format("02.迁移用户，用户{0}【{1}】新增异常：", personModel.NAME, personModel.CODE));
                            }

                            if (completeFlag)
                            {
                                transaction.Complete();
                                ShowMessage(string.Format("02.迁移用户：用户编号={0}", personModel.CODE));
                                personSuccessImportList.Add(personModel.CODE);
                            }
                        }
                    }
                    string msg = string.Format("02.迁移用户结束，合计处理{0}条用户数据，成功迁移{1}条用户", personList.Count, personSuccessImportList.Count);
                    ShowMessage(msg);
                    LogHelper.CommLogger.Info(msg);

                    personList.Clear();
                    personKeyList.Clear();
                }
                else
                {
                    ShowMessage("02.迁移用户结束：personList.Count == 0");
                    LogHelper.CommLogger.Info("02.迁移用户结束：personList.Count == 0");
                }

                List<ControlPerson> jielinkPersonList = new List<ControlPerson>();  //jielink数据库的用户列表
                guidMapDic = new Dictionary<string, string>();
                ShowMessage("03.迁移凭证服务");
                if (personSuccessImportList.Count > 0)
                {
                    List<TCacAccountModel> accountList = new List<TCacAccountModel>();
                    List<TCacVoucherServiceModel> cacVoucherServiceList = new List<TCacVoucherServiceModel>();
                    List<TCacVoucherModel> voucherList = new List<TCacVoucherModel>();
                    //JSDS的凭证关系查询方式（2021-02-02与JSDS同事沟通，他们的设计方式）：账号找服务，服务找凭证，最后生成账号与凭证的关系 
                    List<TCacAuthServiceModel> parkServiceList = new List<TCacAuthServiceModel>();  //关联车场服务的凭证，迁移后凭证中的Lguid必需赋值
                    List<TCacAuthServiceModel> doorServiceList = new List<TCacAuthServiceModel>();  //关联门禁服务的凭证，迁移后凭证中的Lguid无需赋值
                    DataSet jielinkPersonDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_person WHERE `Status`=0 ORDER BY id ASC;");
                    if (jielinkPersonDs != null && jielinkPersonDs.Tables[0] != null)
                    {
                        jielinkPersonList = CommonHelper.DataTableToList<ControlPerson>(jielinkPersonDs.Tables[0]);
                    }
                    DataSet accountDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_cac_account WHERE `STATUS`='NORMAL' ORDER BY CREATE_TIME ASC;");
                    if (accountDs != null && accountDs.Tables[0] != null)
                    {
                        accountList = CommonHelper.DataTableToList<TCacAccountModel>(accountDs.Tables[0]);
                    }
                    DataSet cacVoucherServiceDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * FROM t_cac_voucher_service WHERE STATE='NORMAL' ORDER BY CREATE_TIME DESC;");  //服务与凭证关系
                    if (cacVoucherServiceDs != null && cacVoucherServiceDs.Tables[0] != null)
                    {
                        cacVoucherServiceList = CommonHelper.DataTableToList<TCacVoucherServiceModel>(cacVoucherServiceDs.Tables[0]);
                    }
                    //可查询出白卡，没有绑定账号没有绑定服务的白卡，相当于录入系统（迁移数据不单纯做卡入库）
                    DataSet voucherDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_cac_voucher WHERE (`STATUS`='NORMAL' OR `STATUS`='BLANK') ORDER BY ACCOUNT_ID asc, CREATE_TIME DESC;");
                    if (voucherDs != null && voucherDs.Tables[0] != null)
                    {
                        voucherList = CommonHelper.DataTableToList<TCacVoucherModel>(voucherDs.Tables[0]);
                    }
                    DataSet parkServiceDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "select * from t_cac_auth_service WHERE state='NORMAL' and BUSINESS_CODE='PARK' ORDER BY CREATE_TIME DESC;");  //车场服务
                    if (parkServiceDs != null && parkServiceDs.Tables[0] != null)
                    {
                        parkServiceList = CommonHelper.DataTableToList<TCacAuthServiceModel>(parkServiceDs.Tables[0]);
                    }
                    DataSet doorServiceDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "select * from t_cac_auth_service WHERE state='NORMAL' and BUSINESS_CODE='DOOR' ORDER BY CREATE_TIME DESC;");  //门禁服务
                    if (doorServiceDs != null && doorServiceDs.Tables[0] != null)
                    {
                        doorServiceList = CommonHelper.DataTableToList<TCacAuthServiceModel>(doorServiceDs.Tables[0]);
                    }
                    //4、凭证、门禁服务、车场服务
                    if (accountList.Count > 0)
                    {
                        List<string> vsSuccessImportList = new List<string>();
                        int doorServiceSuccessImportCount = 0;
                        int parkServiceSuccessImportCount = 0;
                        int voucherSuccessImportCount = 0;
                        foreach (TCacAccountModel account in accountList)
                        {
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                if (string.IsNullOrWhiteSpace(account.ID))
                                {
                                    continue;
                                }
                                if (string.IsNullOrWhiteSpace(account.PERSON_ID))
                                {
                                    continue;
                                }
                                string personGuid = GetGuidString(account.PERSON_ID);
                                ControlPerson jieLinkPerson = jielinkPersonList.FirstOrDefault(x => x.PGUID == personGuid);
                                if (jieLinkPerson == null)
                                {
                                    continue;
                                }
                                bool completeFlag = true;
                                List<TCacAuthServiceModel> parkServiceSuccessList = new List<TCacAuthServiceModel>();
                                List<string> messages = new List<string>();
                                try
                                {
                                    //插入门禁服务
                                    TCacAuthServiceModel doorService = doorServiceList.FirstOrDefault(x => x.ACCOUNT_ID == account.ID);
                                    if (doorService != null)
                                    {
                                        string startTime = CommonHelper.GetDateTimeValue(doorService.START_TIME, DateTime.Now).ToString("yyyy-MM-dd");
                                        string endTime = CommonHelper.GetDateTimeValue(doorService.END_TIME, DateTime.Now).ToString("yyyy-MM-dd");

                                        DataSet currentDoorServiceDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, $"SELECT * from system_set_door_service_to_person WHERE `Status`=0 and PersonNo='{jieLinkPerson.PersonNo}'");
                                        SystemSetDoorServiceToPerson currentDoorService = null;
                                        if (currentDoorServiceDs != null && currentDoorServiceDs.Tables[0] != null)
                                        {
                                            currentDoorService = CommonHelper.DataTableToList<SystemSetDoorServiceToPerson>(currentDoorServiceDs.Tables[0]).FirstOrDefault();
                                        }
                                        string sql = string.Empty;
                                        if (currentDoorService == null)
                                        {
                                            sql = string.Format("INSERT INTO system_set_door_service_to_person(SGUID,PersonNo,StartTime,EndTime,Start1,End1,`Week`,WeekText,`Status`,OperNo,OperName,OperTime) VALUE('{0}','{1}','{2}','{3}','00:00','23:59','1111111','星期一,星期二,星期三,星期四,星期五,星期六,星期日',0,'9999','超级管理员','{4}')",
                                                   Guid.NewGuid().ToString(), jieLinkPerson.PersonNo, startTime, endTime, doorService.CREATE_TIME);
                                        }
                                        else
                                        {
                                            sql = $"update system_set_door_service_to_person set StartTime='{startTime}',EndTime='{endTime}',OperTime='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' where `Status`=0 and PersonNo='{jieLinkPerson.PersonNo}'";
                                        }
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag > 0)
                                        {
                                            doorServiceSuccessImportCount++;
                                            messages.Add("门禁服务");
                                            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsDoorService=1 WHERE PGUID='{0}';", personGuid));

                                            //纯凭证
                                            List<TCacVoucherServiceModel> relations = cacVoucherServiceList.Where(x => x.AUTHSERVICE_ID == doorService.ID).ToList();
                                            List<string> voucherNonServiceGuids = relations.Select(x => x.VOUCHER_ID).ToList();
                                            List<TCacVoucherModel> vouchersNonServiceList = voucherList.Where(x => voucherNonServiceGuids.Contains(x.ID)).ToList();
                                            if (vouchersNonServiceList.Count > 0)
                                            {
                                                List<string> voucherSuccessImportList = VoucherSave(vouchersNonServiceList, jieLinkPerson, "", account.ID);
                                                if (voucherSuccessImportList.Count > 0)
                                                {
                                                    voucherSuccessImportCount = voucherSuccessImportCount + voucherSuccessImportList.Count;
                                                    messages.Add(string.Format("没有关联服务的凭证Guid=【{0}】", string.Join(",", voucherSuccessImportList)));
                                                    voucherSuccessImportList.Clear();
                                                }
                                            }
                                            if (messages.Count > 0)
                                            {
                                                ShowMessage(string.Format("03.迁移用户={0}凭证服务：{1}", jieLinkPerson.PersonNo, string.Join("；", messages)));
                                                vsSuccessImportList.Add(jieLinkPerson.PersonNo);
                                            }
                                            else
                                            {
                                                ShowMessage(string.Format("03.迁移用户={0}凭证服务：无记录", jieLinkPerson.PersonNo));
                                            }
                                            relations.Clear();
                                            voucherNonServiceGuids.Clear();
                                            vouchersNonServiceList.Clear();
                                        }
                                    }
                                    //车场服务+凭证
                                    List<TCacAuthServiceModel> parkServices = parkServiceList.Where(x => x.ACCOUNT_ID == account.ID).ToList();
                                    foreach (TCacAuthServiceModel parkService in parkServices)
                                    {
                                        string lguid = string.Empty;
                                        Guid tempGuid = new Guid();
                                        if (Guid.TryParse(parkService.ID, out tempGuid))
                                        {
                                            lguid = new Guid(parkService.ID).ToString();
                                        }
                                        else
                                        {
                                            if (guidMapDic.ContainsKey(parkService.ID))
                                            {
                                                continue;
                                            }
                                            guidMapDic.Add(parkService.ID, lguid);
                                            lguid = Guid.NewGuid().ToString();
                                        }
                                        string startTime = CommonHelper.GetDateTimeValue(parkService.START_TIME, DateTime.Now).ToString("yyyy-MM-dd 00:00:00");
                                        string endTime = CommonHelper.GetDateTimeValue(parkService.END_TIME, DateTime.Now).ToString("yyyy-MM-dd 23:59:59");
                                        string stopServiceTime = CommonHelper.GetDateTimeValue(parkService.END_TIME, DateTime.Now).ToString("yyyy-MM-dd");
                                        string uniqueServiceNo = CommonHelper.GetUniqueId();
                                        if (parkService.PARK_SEAT_NUM < 1)
                                        {
                                            parkService.PARK_SEAT_NUM = 1;
                                        }
                                        DataSet currentParkServiceDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, $"SELECT * from control_lease_stall WHERE LGUID='{lguid}' AND `Status`=0;");
                                        ControlLeaseStall currentParkService = null;
                                        if (currentParkServiceDs != null && currentParkServiceDs.Tables[0] != null)
                                        {
                                            currentParkService = CommonHelper.DataTableToList<ControlLeaseStall>(currentParkServiceDs.Tables[0]).FirstOrDefault();
                                        }
                                        string sql = string.Empty;
                                        if (currentParkService == null)
                                        {
                                            sql = string.Format("INSERT INTO control_lease_stall(LGUID,PGUID,SetmealNo,StartTime,EndTime,OperatorNO,OperatorName,OperateTime,`Status`,PersonName,PersonNo,NisspId,CarNumber,VehiclePosCount,StopServiceTime,UniqueServiceNo,`Timestamp`) VALUE('{0}','{1}',50,'{2}','{3}','9999','超级管理员','{4}',0,'{5}','{6}','{0}','{7}','{7}','{8}','{9}',0)",
                                                    lguid, personGuid, startTime, endTime, parkService.CREATE_TIME, jieLinkPerson.PersonName, jieLinkPerson.PersonNo, parkService.PARK_SEAT_NUM, stopServiceTime, uniqueServiceNo);
                                        }
                                        else
                                        {
                                            sql = $"update control_lease_stall set StartTime='{startTime}',EndTime='{endTime}',StopServiceTime='{stopServiceTime}',CarNumber={parkService.PARK_SEAT_NUM},VehiclePosCount={parkService.PARK_SEAT_NUM},OperateTime='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'";
                                        }
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag > 0)
                                        {
                                            parkServiceSuccessImportCount++;
                                            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsParkService=1 WHERE PGUID='{0}';", personGuid));
                                            //关联了车场服务的凭证
                                            List<TCacVoucherServiceModel> relations = cacVoucherServiceList.Where(x => x.AUTHSERVICE_ID == parkService.ID).ToList();
                                            List<string> voucherGuids = relations.Select(x => x.VOUCHER_ID).ToList();
                                            List<TCacVoucherModel> vouchers = voucherList.Where(x => voucherGuids.Contains(x.ID)).ToList();
                                            if (vouchers.Count > 0)
                                            {
                                                List<string> voucherSuccessImportList = VoucherSave(vouchers, jieLinkPerson, lguid, account.ID);
                                                if (voucherSuccessImportList.Count > 0)
                                                {
                                                    voucherSuccessImportCount = voucherSuccessImportCount + voucherSuccessImportList.Count;
                                                    messages.Add(string.Format("车场服务lguid={0}，关联凭证Guid=【{1}】", lguid, string.Join(",", voucherSuccessImportList)));
                                                    voucherSuccessImportList.Clear();
                                                }
                                                else
                                                {
                                                    messages.Add(string.Format("车场服务lguid={0}", lguid));
                                                }
                                            }
                                            else
                                            {
                                                messages.Add(string.Format("车场服务lguid={0}", lguid));
                                            }
                                            relations.Clear();
                                            voucherGuids.Clear();
                                            vouchers.Clear();
                                        }
                                    }
                                }
                                catch (Exception o)
                                {
                                    completeFlag = false;
                                    ShowMessage(string.Format("03.迁移凭证服务，处理用户={0}的凭证服务信息异常：详情请分析日志", jieLinkPerson.PersonNo));
                                    LogHelper.CommLogger.Error(o, string.Format("03.迁移凭证服务，处理用户={0}的凭证服务信息异常：", jieLinkPerson.PersonNo));
                                }
                                if (completeFlag)
                                {
                                    transaction.Complete();
                                }
                            }
                        }
                        string msg = string.Format("03.迁移凭证服务结束，合计处理{0}条账户数据，成功迁移{1}条：凭证{2}条，车场服务{3}，门禁服务{4}",
                            accountList.Count, vsSuccessImportList.Count, voucherSuccessImportCount, parkServiceSuccessImportCount, doorServiceSuccessImportCount);
                        ShowMessage(msg);
                        LogHelper.CommLogger.Info(msg);

                        accountList.Clear();
                        cacVoucherServiceList.Clear();
                        voucherList.Clear();
                        parkServiceList.Clear();
                        doorServiceList.Clear();
                        vsSuccessImportList.Clear();
                        personSuccessImportList.Clear();
                    }
                    else
                    {
                        ShowMessage("03.迁移凭证服务结束：accountList.Count==0");
                        LogHelper.CommLogger.Info("03.迁移凭证服务结束：accountList.Count==0");
                    }
                }
                else
                {
                    ShowMessage("03.迁移凭证服务结束：personSuccessImportList.Count == 0");
                    LogHelper.CommLogger.Info("03.迁移凭证服务结束：personSuccessImportList.Count == 0");
                }
                //设备：日志写在方法体内
                ControlDeviceImport();
                //设备权限：区域
                DeviceRightImport();
                ShowMessage("06.迁移入出场收费记录");
                List<TParkRecordInModel> recordInList = new List<TParkRecordInModel>();
                List<TParkRecordOutModel> recordOutList = new List<TParkRecordOutModel>();
                List<TParkPayModel> highVersionRecordBillList = new List<TParkPayModel>();
                List<TParkpayrecordModel> lowVersionPayRecordList = new List<TParkpayrecordModel>();
                bool enableTParkPay = false;    //是否存在t_park_pay，不存在的话通过出场记录生成收费记录
                string miniTime = DateTime.Now.Date.AddMonths(-3).ToString("yyyy-MM-dd 00:00:00");
                switch (policy.SelectedTimeIndex)
                {
                    case 1:
                        miniTime = DateTime.Now.Date.AddMonths(-1).ToString("yyyy-MM-dd 00:00:00");
                        break;
                    case 6:
                        miniTime = DateTime.Now.Date.AddMonths(-6).ToString("yyyy-MM-dd 00:00:00");
                        break;
                    case 9:
                        miniTime = DateTime.Now.Date.AddMonths(-9).ToString("yyyy-MM-dd 00:00:00");
                        break;
                    case 12:
                        miniTime = DateTime.Now.Date.AddMonths(-12).ToString("yyyy-MM-dd 00:00:00");
                        break;
                    default:
                        break;
                }
                DataSet recordInDs = null;
                if (policy.EnterRecordSelect)
                {
                    //勾选了入场记录
                    recordInDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, string.Format("SELECT * FROM t_park_record_in WHERE IN_TIME>='{0}' ORDER BY IN_TIME ASC;", miniTime));
                }
                else
                {
                    //没有勾选入场记录：默认场内记录
                    recordInDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, string.Format("SELECT * FROM t_park_record_in WHERE IN_TIME>='{0}' AND OUT_FLAG='NO_OUT' ORDER BY IN_TIME ASC;", miniTime));
                }
                if (recordInDs != null && recordInDs.Tables[0] != null && recordInDs.Tables[0].Rows.Count > 0)
                {
                    recordInList = CommonHelper.DataTableToList<TParkRecordInModel>(recordInDs.Tables[0]);
                }
                if (recordInList.Count > 0 && policy.OutRecordSelect)
                {
                    DataSet recordOutDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, string.Format("SELECT * FROM t_park_record_out WHERE IN_TIME>='{0}' ORDER BY IN_TIME ASC;", miniTime));
                    if (recordOutDs != null && recordOutDs.Tables[0] != null && recordOutDs.Tables[0].Rows.Count > 0)
                    {
                        recordOutList = CommonHelper.DataTableToList<TParkRecordOutModel>(recordOutDs.Tables[0]);
                    }
                }
                if (recordOutList.Count > 0 && policy.BillRecordSelect)
                {
                    try
                    {
                        DataSet recordBillDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, string.Format("SELECT * FROM t_park_pay WHERE IN_TIME>='{0}' ORDER BY IN_TIME ASC;", miniTime));
                        if (recordBillDs != null && recordBillDs.Tables[0] != null && recordBillDs.Tables[0].Rows.Count > 0)
                        {
                            highVersionRecordBillList = CommonHelper.DataTableToList<TParkPayModel>(recordBillDs.Tables[0]);
                        }
                        enableTParkPay = true;
                    }
                    catch (Exception o)
                    {
                        LogHelper.CommLogger.Error(o, "查询收费表t_park_pay（低版本收费记录在出场表t_park_record_out）异常：");
                        enableTParkPay = false;
                        DataSet recordBillDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, string.Format($"SELECT * from t_park_visitorpayrecord WHERE IN_TIME>='{miniTime}' ORDER BY IN_TIME ASC;"));
                        if (recordBillDs != null && recordBillDs.Tables[0] != null && recordBillDs.Tables[0].Rows.Count > 0)
                        {
                            lowVersionPayRecordList = CommonHelper.DataTableToList<TParkpayrecordModel>(recordBillDs.Tables[0]);
                        }
                    }
                }
                if (recordInList.Count > 0)
                {
                    if (jielinkPersonList == null)
                    {
                        jielinkPersonList = new List<ControlPerson>();
                    }
                    if (jielinkPersonList.Count == 0)
                    {
                        //获取用户
                        DataSet jielinkPersonDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_person WHERE `Status`=0 ORDER BY id ASC;");
                        if (jielinkPersonDs != null && jielinkPersonDs.Tables[0] != null)
                        {
                            jielinkPersonList = CommonHelper.DataTableToList<ControlPerson>(jielinkPersonDs.Tables[0]);
                        }
                    }
                    DataSet serviceDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_lease_stall WHERE `Status`=0 ORDER BY OperateTime DESC;");
                    List<ControlLeaseStall> parkServiceList = new List<ControlLeaseStall>();
                    if (serviceDs != null && serviceDs.Tables[0] != null)
                    {
                        parkServiceList = CommonHelper.DataTableToList<ControlLeaseStall>(serviceDs.Tables[0]);
                    }
                    DataSet voucherDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_voucher WHERE `Status` in (1,2) ORDER BY AddTime ASC;");
                    List<ControlVoucher> voucherList = new List<ControlVoucher>();
                    if (voucherDs != null && voucherDs.Tables[0] != null)
                    {
                        voucherList = CommonHelper.DataTableToList<ControlVoucher>(voucherDs.Tables[0]);
                    }
                    List<ControlDevices> controlDevicesList = new List<ControlDevices>();
                    if (policy.ControlDeviceSelect)
                    {
                        //迁移了设备
                        DataSet controlDeviceDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_devices ORDER BY id ASC;");
                        if (controlDeviceDs != null && controlDeviceDs.Tables[0] != null && controlDeviceDs.Tables[0].Rows.Count > 0)
                        {
                            controlDevicesList = CommonHelper.DataTableToList<ControlDevices>(controlDeviceDs.Tables[0]).OrderBy(x => x.ID).ToList();
                        }
                    }
                    List<string> enterSuccessImportList = new List<string>();
                    List<string> outSuccessImportList = new List<string>();
                    List<string> billSuccessImportList = new List<string>();
                    foreach (TParkRecordInModel recordIn in recordInList)
                    {
                        try
                        {
                            string plate = recordIn.PHYSICAL_NO;
                            string intime = CommonHelper.GetDateTimeValue(recordIn.IN_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                            string enterDeviceId = "188766208";
                            string enterDeviceName = "虚拟车场入口";
                            GetEnterDeviceInfo(controlDevicesList, recordIn, ref enterDeviceId, ref enterDeviceName);
                            string eguid = new Guid(recordIn.ID).ToString();
                            int wasgone = 0;
                            if (recordIn.OUT_FLAG.Equals("OUT"))
                            {
                                wasgone = 1;
                            }
                            string personNo = "";
                            string personName = "临时车主";
                            int sealId = 54;
                            string sealName = "临时用户A";
                            if (!string.IsNullOrWhiteSpace(recordIn.AUTHSERVICE_ID))
                            {
                                string serviceGuid = GetGuidString(recordIn.AUTHSERVICE_ID);
                                ControlLeaseStall parkService = parkServiceList.FirstOrDefault(x => x.LGUID == serviceGuid);
                                if (parkService != null)
                                {
                                    sealId = parkService.SetmealNo;
                                    sealName = "月租用户A";
                                    ControlPerson person = jielinkPersonList.FirstOrDefault(x => x.PGUID == parkService.PGUID);
                                    if (person != null)
                                    {
                                        personNo = person.PersonNo;
                                        personName = person.PersonName;
                                    }
                                }
                            }
                            if (string.IsNullOrWhiteSpace(recordIn.REMARK))
                            {
                                recordIn.REMARK = REMARK;
                            }
                            string sql = $"insert into box_enter_record (CredentialType,CredentialNO,Plate,CarNumOrig,EnterTime,SetmealType,SealName,EGuid,EnterRecordID,EnterDeviceID,EnterDeviceName,WasGone,EventType,EventTypeName,PARKNO,OperatorNo,OperatorName,PersonNo,PersonName,Remark,InDeviceEnterType,OptDate) " +
                                $"VALUES(163,'{plate}','{plate}','{plate}','{intime}',{sealId},'{sealName}','{eguid}','{recordIn.ID}','{enterDeviceId}','{enterDeviceName}','{wasgone}',1,'一般正常记录','{ConstantHelper.PARKNO}','9999','超级管理员','{personNo}','{personName}','{recordIn.REMARK}',1,'{intime}');";
                            int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                            if (result > 0)
                            {
                                enterSuccessImportList.Add(string.Format($"{plate}-{recordIn.ID}"));
                                ShowMessage($"06.迁移入出场收费记录，车牌：{plate}-{recordIn.ID} 补录入场记录成功！");
                            }
                        }
                        catch (Exception o)
                        {
                            ShowMessage($"06.迁移入出场收费记录，车牌：{recordIn.PHYSICAL_NO}-{recordIn.IN_TIME}-{recordIn.ID} 补录入场记录异常：{o.Message}");
                            LogHelper.CommLogger.Error(o, $"06.迁移入出场收费记录，车牌：{recordIn.PHYSICAL_NO}-{recordIn.IN_TIME}-{recordIn.ID} 补录入场记录异常：");
                        }
                    }
                    foreach (TParkRecordOutModel recordOut in recordOutList)
                    {
                        try
                        {
                            string plate = recordOut.PHYSICAL_NO;
                            recordOut.IN_TIME = CommonHelper.GetDateTimeValue(recordOut.IN_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                            recordOut.OUT_TIME = CommonHelper.GetDateTimeValue(recordOut.OUT_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                            string enterDeviceId = "188766208";
                            string enterDeviceName = "虚拟车场入口";
                            bool newRecordInId = false; //是否重新生成的入场id，是的话不会匹配到入场记录，即不找入场记录了
                            if (string.IsNullOrWhiteSpace(recordOut.RECORDIN_ID) || recordOut.RECORDIN_ID == "0")
                            {
                                //找不到入场记录的出场记录 重新生成RECORDIN_ID，并映射好出场id
                                recordOut.RECORDIN_ID = GetOutEnterRecordIn(recordOut.ID);
                                newRecordInId = true;
                            }
                            if (policy.ControlDeviceSelect && !newRecordInId)
                            {
                                TParkRecordInModel recordIn = recordInList.FirstOrDefault(x => x.ID == recordOut.RECORDIN_ID);
                                if (recordIn != null)
                                {
                                    GetEnterDeviceInfo(controlDevicesList, recordIn, ref enterDeviceId, ref enterDeviceName);
                                }
                            }
                            string outDeviceId = "120201030";
                            string outDeviceName = "虚拟车场出口";
                            ControlDevices chargeBox = null;
                            GetOutDeviceInfo(controlDevicesList, recordOut, ref outDeviceId, ref outDeviceName, ref chargeBox);
                            string oguid = new Guid(recordOut.ID).ToString();
                            string outRecordId = recordOut.ID;
                            string personNo = "";
                            string personName = "临时车主";
                            int sealId = 54;
                            string sealName = "临时用户A";
                            if (!string.IsNullOrWhiteSpace(recordOut.AUTHSERVICE_ID))
                            {
                                string serviceGuid = GetGuidString(recordOut.AUTHSERVICE_ID);
                                ControlLeaseStall service = parkServiceList.FirstOrDefault(x => x.LGUID == serviceGuid);
                                if (service != null)
                                {
                                    sealId = service.SetmealNo;
                                    sealName = "月租用户A";
                                    ControlPerson person = jielinkPersonList.FirstOrDefault(x => x.PGUID == service.PGUID);
                                    if (person != null)
                                    {
                                        personNo = person.PersonNo;
                                        personName = person.PersonName;
                                    }
                                }
                            }
                            if (string.IsNullOrWhiteSpace(recordOut.REMARK))
                            {
                                recordOut.REMARK = REMARK;
                            }
                            int payType = 21;   //其他，通过收费记录赋值    
                            string payTypeName = "其它";
                            //AccountReceivable,Charging,HgMoney,YhMoney,FreeMoney
                            decimal accountReceivable = 0;  //实收金额
                            decimal charging = 0;   //应收金额
                            decimal hgMoney = 0;    //回滚金额
                            decimal yhMoney = 0;    //优惠金额
                            decimal freeMoney = 0;  //免费金额
                            string billsql = string.Empty;
                            string billguid = string.Empty;
                            if (!enableTParkPay)
                            {
                                //收费记录t_park_visitorpayrecord
                                TParkpayrecordModel payrecord = lowVersionPayRecordList.FirstOrDefault(x => x.RECORDOUT_ID == outRecordId);
                                if (payrecord != null)
                                {
                                    if (sealId == 54 || payrecord.ACTUAL_BALANCE > 0)
                                    {
                                        //临时套餐或者实收大于0
                                        accountReceivable = payrecord.ACTUAL_BALANCE;  //实收金额
                                        GetPayType(payrecord.PAY_TYPE, out payType, out payTypeName);
                                        decimal money = payrecord.ACTUAL_BALANCE;  //实收金额
                                        decimal fees = payrecord.AVAILABLE_BALANCE;   //优惠前金额（应收金额）
                                        decimal benefit = payrecord.ABATE_BALANCE;    //优惠金额
                                        decimal derate = 0;    //滚动收费的减免金额
                                        decimal paid = 0;   //已支付金额
                                        decimal actualPaid = payrecord.ACTUAL_BALANCE;
                                        decimal exchange = payrecord.REFUND_BALANCE;   //找零金额                                    
                                        decimal smallChange = 0;    //抹零金额，主要用于存储自助缴费机的抹零金额
                                        int orderType = 0;  //订单类型：1云支付订单 2盒子订单 3中央收费订单 4 缴费机
                                        int status = 1; //订单状态 0未支付 1已支付 2已退款 3代扣中 
                                        int replaceDeduct = 0;  //代扣标识（0：无代扣，1：支付宝代扣）
                                        string payFrom = "捷顺";    //支付来源，如：捷顺、第三方   
                                        int chargeType = 0; //正常收费
                                        billguid = new Guid(payrecord.ID).ToString();
                                        payrecord.CREATE_TIME = CommonHelper.GetDateTimeValue(payrecord.CREATE_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                                        payrecord.BRUSHCARD_TIME = CommonHelper.GetDateTimeValue(payrecord.BRUSHCARD_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                                        payrecord.IN_TIME = CommonHelper.GetDateTimeValue(payrecord.IN_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                                        string chargeDeviceID = outDeviceId;
                                        string chargeDeviceName = outDeviceName;
                                        if (chargeBox != null)
                                        {
                                            chargeDeviceID = chargeBox.DeviceID;
                                            chargeDeviceName = chargeBox.DeviceName;
                                        }
                                        billsql = $"INSERT INTO box_bill(BGUID, CredentialType, CredentialNO, Plate, OrderId, PayTime, FeesTime, CreateTime, InTime, EnterRecordID, Money, Fees, Benefit, Derate, AccountReceivable, Paid, ActualPaid, Exchange, FreeMoney, PayTypeID, PayTypeName, ChargeType, ChargeDeviceID,ChargeDeviceName, OperatorID, OperatorName,Cashier,CashierName, OrderType,`Status`, SealTypeId, SealTypeName, Remark, ReplaceDeduct, EventType, PayFrom, DeviceID, PersonNo, PersonName, PARKNO) " +
                                            $"VALUE('{billguid}', 163, '{plate}', '{plate}', '{payrecord.ID}', '{payrecord.BRUSHCARD_TIME}', '{payrecord.BRUSHCARD_TIME}', '{payrecord.CREATE_TIME}', '{payrecord.IN_TIME}', '{recordOut.RECORDIN_ID}', {money}, {fees}, {benefit}, {derate}, {accountReceivable}, {paid}, {actualPaid}, {exchange},{freeMoney}, {payType}, '{payTypeName}', {chargeType}, '{chargeDeviceID}','{chargeDeviceName}', '9999', '超级管理员', '9999', '超级管理员', {orderType}, {status}, {sealId}, '{sealName}', '{REMARK}', {replaceDeduct}, 1, '{payFrom}', '{outDeviceId}', '{personNo}', '{personName}', '{ConstantHelper.PARKNO}');";
                                    }
                                    else
                                    {
                                        ShowMessage($"06.迁移入出场收费记录，车牌：{plate}-{payrecord.ID} 补录收费记录，!(sealId == 54 || payrecord.ACTUAL_BALANCE > 0) 跳过！");
                                    }
                                }
                            }
                            else
                            {
                                //收费记录t_park_pay
                                TParkPayModel recordBill = highVersionRecordBillList.FirstOrDefault(x => x.RECORDOUT_ID == outRecordId);
                                if (recordBill != null)
                                {
                                    GetPayType(recordBill.PAY_TYPE, out payType, out payTypeName);
                                }
                                //AccountReceivable,Charging,HgMoney,YhMoney,FreeMoney
                                accountReceivable = recordOut.TOTAL_CHARGE_ACTUAL;  //实收金额
                                charging = recordOut.TOTAL_CHARGE_AVAILABLE;   //应收金额
                                hgMoney = 0;    //回滚金额
                                yhMoney = recordOut.TOTAL_CHARGE_REFUND;    //优惠金额
                                freeMoney = 0;  //免费金额
                            }
                            string sql = $"INSERT INTO box_out_record(OGUID,OutRecordID,OutDeviceID,OutDeviceName,EnterRecordID,InDeviceName,InTime,CredentialType,CredentialNO,Plate,CarNumOrig,OperatorNo,OperatorName,OutTime,OptTime,PersonNo,PersonName,SetmealType,SetmealName,Remark,PayType,AccountReceivable,Charging,HgMoney,YhMoney,FreeMoney,IoType,ExtendFlag,EventType,EventTypeName,PARKNO) " +
                                $"VALUE('{oguid}','{outRecordId}','{outDeviceId}','{outDeviceName}','{recordOut.RECORDIN_ID}','{enterDeviceName}','{recordOut.IN_TIME}',163,'{plate}','{plate}','{plate}','9999','超级管理员','{recordOut.OUT_TIME}','{recordOut.OUT_TIME}','{personNo}','{personName}',{sealId},'{sealName}','{recordOut.REMARK}','{payType}',{accountReceivable},{charging},{hgMoney},{yhMoney},{freeMoney},2,1,1,'一般正常记录','{ConstantHelper.PARKNO}');";
                            int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                            if (result > 0)
                            {
                                outSuccessImportList.Add(string.Format($"{plate}-{recordOut.ID}"));
                                ShowMessage($"06.迁移入出场收费记录，车牌：{plate}-{recordOut.ID} 补录出场记录成功！");

                                if (!enableTParkPay && !string.IsNullOrWhiteSpace(billsql))
                                {
                                    result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, billsql);
                                    if (result > 0)
                                    {
                                        billSuccessImportList.Add(string.Format($"{plate}-{billguid}"));
                                        ShowMessage($"06.迁移入出场收费记录，车牌：{plate}-{billguid} 补录收费记录成功！");
                                    }
                                }
                            }
                        }
                        catch (Exception o)
                        {
                            ShowMessage($"06.迁移入出场收费记录，车牌：{recordOut.PHYSICAL_NO}-{recordOut.OUT_TIME}-{recordOut.ID} 补录出场记录异常：{o.Message}");
                            LogHelper.CommLogger.Error(o, $"06.迁移入出场收费记录，车牌：{recordOut.PHYSICAL_NO}-{recordOut.OUT_TIME}-{recordOut.ID} 补录出场记录异常：");
                        }
                    }
                    if (enableTParkPay && highVersionRecordBillList.Count > 0)
                    {
                        foreach (TParkPayModel recordBill in highVersionRecordBillList)
                        {
                            try
                            {
                                string plate = recordBill.PHYSICAL_NO;
                                recordBill.IN_TIME = CommonHelper.GetDateTimeValue(recordBill.IN_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                                recordBill.PAY_TIME = CommonHelper.GetDateTimeValue(recordBill.PAY_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                                recordBill.CREATE_TIME = CommonHelper.GetDateTimeValue(recordBill.CREATE_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                                string enterDeviceId = "188766208";
                                string enterDeviceName = "虚拟车场入口";
                                bool newRecordInId = false; //是否重新生成的入场id，是的话不会匹配到入场记录，即不找入场记录了
                                if (string.IsNullOrWhiteSpace(recordBill.RECORDIN_ID) || recordBill.RECORDIN_ID == "0")
                                {
                                    //找不到入场记录的出场记录 重新生成RECORDIN_ID，并映射好出场id
                                    recordBill.RECORDIN_ID = GetOutEnterRecordIn(recordBill.RECORDOUT_ID);
                                    newRecordInId = true;
                                }
                                string outDeviceId = "120201030";
                                string outDeviceName = "虚拟车场出口";
                                string chargeDeviceID = outDeviceId;
                                string chargeDeviceName = outDeviceName;
                                string bguid = new Guid(recordBill.ID).ToString();
                                ControlDevices chargeBox = null;
                                if (policy.ControlDeviceSelect)
                                {
                                    if (!newRecordInId)
                                    {
                                        TParkRecordInModel recordIn = recordInList.FirstOrDefault(x => x.ID == recordBill.RECORDIN_ID);
                                        if (recordIn != null)
                                        {
                                            GetEnterDeviceInfo(controlDevicesList, recordIn, ref enterDeviceId, ref enterDeviceName);
                                        }
                                    }
                                    TParkRecordOutModel recordOut = recordOutList.FirstOrDefault(x => x.ID == recordBill.RECORDOUT_ID);
                                    if (recordOut != null)
                                    {
                                        GetOutDeviceInfo(controlDevicesList, recordOut, ref outDeviceId, ref outDeviceName, ref chargeBox);
                                        if (chargeBox != null)
                                        {
                                            chargeDeviceID = chargeBox.DeviceID;
                                            chargeDeviceName = chargeBox.DeviceName;
                                        }
                                    }
                                }
                                if (string.IsNullOrWhiteSpace(recordBill.ORDER_NO))
                                {
                                    recordBill.ORDER_NO = recordBill.ID;
                                }
                                string personNo = "";
                                string personName = "临时车主";
                                int sealId = 54;
                                string sealName = "临时用户A";
                                if (!string.IsNullOrWhiteSpace(recordBill.VOUCHER_ID))
                                {
                                    string voucherGuid = GetGuidString(recordBill.VOUCHER_ID);
                                    ControlVoucher voucher = voucherList.FirstOrDefault(x => x.Guid == voucherGuid);
                                    if (voucher != null && !string.IsNullOrWhiteSpace(voucher.LGuid))
                                    {
                                        ControlLeaseStall service = parkServiceList.FirstOrDefault(x => x.LGUID == voucher.LGuid);
                                        if (service != null)
                                        {
                                            sealId = service.SetmealNo;
                                            sealName = "月租用户A";
                                            ControlPerson person = jielinkPersonList.FirstOrDefault(x => x.PGUID == service.PGUID);
                                            if (person != null)
                                            {
                                                personNo = person.PersonNo;
                                                personName = person.PersonName;
                                            }
                                        }
                                    }
                                }
                                if (!(sealId == 54 || recordBill.ACTUAL_BALANCE > 0))
                                {
                                    //临时套餐或者实收大于0：才生成收费记录
                                    ShowMessage($"06.迁移入出场收费记录，车牌：{plate}-{recordBill.ID} 补录收费记录，!(sealId == 54 || recordBill.ACTUAL_BALANCE > 0) 跳过！");
                                    continue;
                                }
                                if (string.IsNullOrWhiteSpace(recordBill.REMARK))
                                {
                                    recordBill.REMARK = REMARK;
                                }
                                int payType = 21;
                                string payTypeName = "其它";
                                GetPayType(recordBill.PAY_TYPE, out payType, out payTypeName);
                                decimal money = recordBill.ACTUAL_BALANCE;  //实收金额
                                decimal fees = recordBill.AVAILABLE_BALANCE;   //优惠前金额（应收金额）
                                decimal benefit = recordBill.ABATE_BALANCE;    //优惠金额
                                decimal derate = 0;    //滚动收费的减免金额
                                decimal accountReceivable = recordBill.ACTUAL_BALANCE;  //实收金额
                                decimal paid = 0;   //已支付金额
                                decimal actualPaid = recordBill.ACTUAL_BALANCE;
                                decimal exchange = recordBill.REFUND_BALANCE;   //找零金额
                                decimal freeMoney = 0;  //免费金额
                                decimal smallChange = 0;    //抹零金额，主要用于存储自助缴费机的抹零金额
                                int orderType = 0;  //订单类型：1云支付订单 2盒子订单 3中央收费订单 4 缴费机
                                int status = 1; //订单状态 0未支付 1已支付 2已退款 3代扣中 
                                int replaceDeduct = 0;  //代扣标识（0：无代扣，1：支付宝代扣）
                                string payFrom = "捷顺";    //支付来源，如：捷顺、第三方   
                                int chargeType = 0; //正常收费
                                if (string.IsNullOrWhiteSpace(recordBill.PAY_TIME))
                                {
                                    recordBill.PAY_TIME = recordBill.BRUSHCARD_TIME;
                                }
                                string sql = $"INSERT INTO box_bill(BGUID, CredentialType, CredentialNO, Plate, OrderId, PayTime, FeesTime, CreateTime, InTime, EnterRecordID, Money, Fees, Benefit, Derate, AccountReceivable, Paid, ActualPaid, Exchange, FreeMoney, PayTypeID, PayTypeName, ChargeType, ChargeDeviceID,ChargeDeviceName, OperatorID, OperatorName,Cashier,CashierName, OrderType,`Status`, SealTypeId, SealTypeName, Remark, ReplaceDeduct, EventType, PayFrom, DeviceID, PersonNo, PersonName, PARKNO) " +
                                    $"VALUE('{bguid}', 163, '{plate}', '{plate}', '{recordBill.ORDER_NO}', '{recordBill.PAY_TIME}', '{recordBill.PAY_TIME}', '{recordBill.CREATE_TIME}', '{recordBill.IN_TIME}', '{recordBill.RECORDIN_ID}', {money}, {fees}, {benefit}, {derate}, {accountReceivable}, {paid}, {actualPaid}, {exchange},{freeMoney}, {payType}, '{payTypeName}', {chargeType}, '{chargeDeviceID}','{chargeDeviceName}', '9999', '超级管理员', '9999', '超级管理员', {orderType}, {status}, {sealId}, '{sealName}', '{REMARK}', {replaceDeduct}, 1, '{payFrom}', '{outDeviceId}', '{personNo}', '{personName}', '{ConstantHelper.PARKNO}');";
                                int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                if (result > 0)
                                {
                                    billSuccessImportList.Add(string.Format($"{plate}-{recordBill.ID}"));
                                    ShowMessage($"06.迁移入出场收费记录，车牌：{plate}-{recordBill.ID} 补录收费记录成功！");
                                }
                            }
                            catch (Exception o)
                            {
                                ShowMessage($"06.迁移入出场收费记录，车牌：{recordBill.PHYSICAL_NO}-{recordBill.PAY_TIME}-{recordBill.ID} 补录收费记录异常：{o.Message}");
                                LogHelper.CommLogger.Error(o, $"06.迁移入出场收费记录，车牌：{recordBill.PHYSICAL_NO}-{recordBill.PAY_TIME}-{recordBill.ID} 补录收费记录异常：");
                            }
                        }
                    }
                    string msg = string.Format("06.迁移入出场收费记录，合计将解析入场记录{0}条，出场记录{1}条，收费记录{2}条；实际迁移入场记录{3}条，出场记录{4}条，收费记录{5}条",
                        recordInList.Count, recordOutList.Count, enableTParkPay ? highVersionRecordBillList.Count : lowVersionPayRecordList.Count, enterSuccessImportList.Count, outSuccessImportList.Count, billSuccessImportList.Count);
                    ShowMessage(msg);
                    LogHelper.CommLogger.Info(msg);

                    recordInList.Clear();
                    recordOutList.Clear();
                    lowVersionPayRecordList.Clear();
                    highVersionRecordBillList.Clear();
                    enterSuccessImportList.Clear();
                    outSuccessImportList.Clear();
                    billSuccessImportList.Clear();
                    jielinkPersonList.Clear();
                    parkServiceList.Clear();
                    voucherList.Clear();
                }
                else
                {
                    ShowMessage("06.迁移入出场收费记录结束：recordInList.Count == 0");
                    LogHelper.CommLogger.Info("06.迁移入出场收费记录结束：recordInList.Count == 0");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("JSDS一键升级到JieLink异常：详情请分析日志");
                LogHelper.CommLogger.Error(ex, "JSDS一键升级到JieLink异常：");
            }
            finally
            {

            }
        }

        /// <summary>
        /// 保持凭证信息
        /// </summary>
        /// <param name="voucherList"></param>
        /// <param name="jieLinkPerson"></param>
        /// <param name="lguid"></param>
        /// <param name="accountID"></param>
        /// <returns></returns>
        private List<string> VoucherSave(List<TCacVoucherModel> voucherList, ControlPerson jieLinkPerson, string lguid, string accountID)
        {
            List<string> voucherSuccessImportList = new List<string>();
            foreach (TCacVoucherModel voucher in voucherList)
            {
                try
                {
                    string voucherGuid = string.Empty;
                    Guid tempGuid = new Guid();
                    if (Guid.TryParse(voucher.ID, out tempGuid))
                    {
                        voucherGuid = new Guid(voucher.ID).ToString();
                    }
                    else
                    {
                        if (guidMapDic.ContainsKey(voucher.ID))
                        {
                            continue;
                        }
                        guidMapDic.Add(voucher.ID, lguid);
                        voucherGuid = Guid.NewGuid().ToString();
                    }
                    int voucherType = 163;
                    string physicsNum = string.Empty;
                    if (voucher.VOUCHER_TYPE.Equals("ECARD"))
                    {
                        voucherType = 55;
                        physicsNum = voucher.PHYSICAL_NO;
                    }
                    string remark = REMARK;
                    if (!string.IsNullOrWhiteSpace(voucher.REMARK))
                    {
                        remark = voucher.REMARK;
                    }
                    DataSet currentVoucherDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, $"SELECT * from control_voucher WHERE `Status` in (1,2) AND VoucherNo='{voucher.PHYSICAL_NO}' AND PGUID='{jieLinkPerson.PGUID}';");
                    ControlVoucher currentControlVoucher = null;
                    if (currentVoucherDs != null && currentVoucherDs.Tables[0] != null)
                    {
                        currentControlVoucher = CommonHelper.DataTableToList<ControlVoucher>(currentVoucherDs.Tables[0]).FirstOrDefault();
                    }
                    if (currentControlVoucher != null)
                    {
                        continue;
                    }
                    string sql = string.Format("INSERT INTO control_voucher(Guid,PGUID,LGUID,PersonNo,PersonName,VoucherType,VoucherNo,CardNum,PhysicsNum,AddOperatorNo,AddTime,`Status`,LastTime,Remark,StatusFromPerson) VALUE('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{6}','{7}','9999','{8}',1,'{8}','{9}',1);",
                             voucherGuid, jieLinkPerson.PGUID, lguid, jieLinkPerson.PersonNo, jieLinkPerson.PersonName, voucherType, voucher.PHYSICAL_NO, physicsNum, voucher.CREATE_TIME, remark);
                    int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                    if (flag > 0)
                    {
                        voucherSuccessImportList.Add(voucherGuid);
                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsIssueCard=1 WHERE PGUID='{0}';", jieLinkPerson.PGUID));

                        if (voucherType == 163)
                        {
                            sql = string.Format("INSERT INTO control_vehicle_info(VGUID,PGUID,Plate,VehicleType,`Status`,PlateColor) VALUE('{0}','{1}','{2}',0,1,3);",
                                Guid.NewGuid().ToString(), jieLinkPerson.PGUID, voucher.PHYSICAL_NO);
                            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                        }

                        sql = string.Format("INSERT INTO control_voucher_issue(GUID,VoucherGuid,VoucherType,VoucherNo,PGUID,PersonNo,OperatorNo,AddTime,IsBACKED) VALUE('{0}','{1}',{2},'{3}','{4}','{5}','9999','{6}',0);",
                            Guid.NewGuid().ToString(), voucherGuid, voucherType, voucher.PHYSICAL_NO, jieLinkPerson.PGUID, jieLinkPerson.PersonNo, voucher.CREATE_TIME);
                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);

                        sql = string.Format("INSERT into control_voucher_record(id,VoucherGuid,OperateType,OperateNo,OperateTime) VALUE('{0}','{1}',1,'9999','{2}');",
                            Guid.NewGuid().ToString(), voucherGuid, voucher.CREATE_TIME);
                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                    }
                }
                catch (Exception o1)
                {
                    ShowMessage(string.Format("处理账号【{0}】的凭证【{1}】信息异常：{1}", accountID, voucher.ID, o1.ToString()));
                    break;
                }
            }
            return voucherSuccessImportList;
        }
        /// <summary>
        /// 支付方式转换
        /// </summary>
        /// <param name="payTypeStr"></param>
        /// <returns></returns>
        private void GetPayType(string payTypeStr, out int payType, out string payTypeName)
        {
            payType = 21;   //其它
            payTypeName = "其它";
            if (payTypeStr == "XJ")
            {
                payType = 13;
                payTypeName = "现金";
            }
            //else if (payTypeStr == "CLOUDPAY")
            //{
            //    //云支付
            //    payType = 13;
            //}
            else if (payTypeStr == "WX")
            {
                payType = 2;
                payTypeName = "微信";
            }
            else if (payTypeStr == "ZFB")
            {
                payType = 1;
                payTypeName = "支付宝";
            }
            else if (payTypeStr == "YDK")
            {
                //云代扣：无感支付
                payType = 27;
                payTypeName = "无感支付";
            }
            else if (payTypeStr == "YHJM")
            {
                payType = 28;
                payTypeName = "优惠减免";
            }
            else if (payTypeStr == "JSJK")
            {
                payType = 22;
                payTypeName = "捷顺金科";
            }
            else if (payTypeStr == "JST")
            {
                payType = 4;
                payTypeName = "捷顺通支付";
            }
            else if (payTypeStr == "JSTK")
            {
                payType = 5;
                payTypeName = "捷顺通卡支付";
            }
        }

        /// <summary>
        /// 递归获取组织列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<TBaseOrganizeModel> GetOrganizeChildren(List<TBaseOrganizeModel> list, string parentId)
        {
            var result = new List<TBaseOrganizeModel>();
            var subordinate = new List<TBaseOrganizeModel>();
            if (string.IsNullOrWhiteSpace(parentId))
            {
                subordinate = list.Where(e => e.ORG_ID == "" || e.ORG_ID == null).ToList();
            }
            else
            {
                subordinate = list.Where(e => e.ORG_ID == parentId).ToList();
            }
            if (subordinate != null)
            {
                result.AddRange(subordinate);
                foreach (var subo in subordinate)
                {
                    result.AddRange(GetOrganizeChildren(list, subo.ID));
                }
            }

            return result;
        }

        /// <summary>
        /// 判断字符串是否可转guid
        /// </summary>
        /// <param name="jsdsID"></param>
        /// <param name="jielinkGuid"></param>
        private string GetGuidString(string jsdsID)
        {
            string jielinkGuid = string.Empty;
            Guid tempGuid = new Guid();
            if (Guid.TryParse(jsdsID, out tempGuid))
            {
                jielinkGuid = new Guid(jsdsID).ToString();
            }
            else
            {
                if (guidMapDic.ContainsKey(jsdsID))
                {
                    jielinkGuid = guidMapDic[jsdsID];
                }
                else
                {
                    jielinkGuid = Guid.NewGuid().ToString();
                    guidMapDic.Add(jsdsID, jielinkGuid);
                }
            }
            return jielinkGuid;
        }

        /// <summary>
        /// 迁移设备
        /// </summary>
        private void ControlDeviceImport()
        {
            if (!policy.ControlDeviceSelect)
            {
                return;
            }
            DataSet equipmentDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_base_equipment WHERE EQUIP_STATE='NORMAL' ORDER BY EQUIPMENT_ID asc;");
            if (equipmentDs == null || equipmentDs.Tables[0] == null || equipmentDs.Tables[0].Rows.Count == 0)
            {
                ShowMessage("04.迁移设备结束：deviceDs空");
                return;
            }
            List<TBaseEquipmentModel> equipmentList = CommonHelper.DataTableToList<TBaseEquipmentModel>(equipmentDs.Tables[0]).OrderBy(x => x.EQUIPMENT_ID).ToList();
            if (equipmentList.Count == 0)
            {
                ShowMessage("04.迁移设备结束：equipmentList.Count == 0");
                return;
            }
            //设备的MAC、IP、网关地址等在t_base_equipment_param
            DataSet equipmentparamDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_base_equipment_param ORDER BY EQUIPMENT_ID ASC;");
            if (equipmentparamDs == null || equipmentparamDs.Tables[0] == null || equipmentparamDs.Tables[0].Rows.Count == 0)
            {
                ShowMessage("04.迁移设备结束：equipmentparamDs空");
                return;
            }
            List<TBaseEquipmentParamModel> equipmentParamList = CommonHelper.DataTableToList<TBaseEquipmentParamModel>(equipmentparamDs.Tables[0]).OrderBy(x => x.EQUIPMENT_ID).ToList();
            if (equipmentParamList.Count == 0)
            {
                ShowMessage("04.迁移设备结束：equipmentParamList.Count == 0");
                return;
            }
            DataSet controlDeviceDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_devices ORDER BY id ASC;");
            List<ControlDevices> controlDevicesList = new List<ControlDevices>();
            if (controlDeviceDs != null && controlDeviceDs.Tables[0] != null && controlDeviceDs.Tables[0].Rows.Count > 0)
            {
                controlDevicesList = CommonHelper.DataTableToList<ControlDevices>(controlDeviceDs.Tables[0]).OrderBy(x => x.ID).ToList();
            }
            if (controlDevicesList.Count == 0)
            {
                ShowMessage("04.迁移设备结束：controlDevicesList.Count == 0");
                return;
            }
            ControlDevices mjDeivce = controlDevicesList.FirstOrDefault(x => x.DeviceType == 32);
            ControlDevices parkBoxDeivce = controlDevicesList.FirstOrDefault(x => x.DeviceType == 25);
            List<string> deviceSuccessImportList = new List<string>();
            foreach (TBaseEquipmentModel equipment in equipmentList)
            {
                if (equipment.PRODUCT_MODEL != ConstantHelper.JSMJY08
                    //&& equipment.PRODUCT_MODEL != ConstantHelper.JSMJY08_Locker
                    //&& equipment.PRODUCT_MODEL != ConstantHelper.JSMJY08_Reader
                    //&& equipment.PRODUCT_MODEL != ConstantHelper.JSMJY08_OpenDoorButton
                    && equipment.PRODUCT_MODEL != ConstantHelper.JSMJK0220A
                    && equipment.PRODUCT_MODEL != ConstantHelper.JSMJK0240A
                    && equipment.PRODUCT_MODEL != ConstantHelper.JSC8ST)
                {
                    ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：不适配JieLink，跳过");
                    continue;
                    // equipment.PRODUCT_MODEL != ConstantHelper.JSMJK022040A_Reader    //领御III二门读卡器 等价于门：直接在插入领御设备的时候解析即可
                }
                TBaseEquipmentParamModel macItem = null;
                TBaseEquipmentParamModel ipItem = null;
                TBaseEquipmentParamModel devIDItem = null;
                TBaseEquipmentParamModel gatewayItem = null;
                TBaseEquipmentParamModel maskItem = null;
                TBaseEquipmentParamModel macNoItem = null;
                if (equipment.PRODUCT_MODEL == ConstantHelper.JSMJK0220A
                    || equipment.PRODUCT_MODEL == ConstantHelper.JSMJK0240A
                    || equipment.PRODUCT_MODEL == ConstantHelper.JSC8ST)
                {
                    //看jsds数据库，领御设备、速通通过EQUIPMENT_ID找MAC、devId、IP等设备信息
                    macItem = equipmentParamList.FirstOrDefault(x => x.EQUIPMENT_ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMMAC);
                    ipItem = equipmentParamList.FirstOrDefault(x => x.EQUIPMENT_ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMIP);
                    devIDItem = equipmentParamList.FirstOrDefault(x => x.EQUIPMENT_ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMDEVID);
                    gatewayItem = equipmentParamList.FirstOrDefault(x => x.EQUIPMENT_ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMGATEWAY);
                    maskItem = equipmentParamList.FirstOrDefault(x => x.EQUIPMENT_ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMMASK);
                    macNoItem = equipmentParamList.FirstOrDefault(x => x.EQUIPMENT_ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMMACNO);
                }
                else
                {
                    //看jsds数据库，Y08通过ID找MAC、devId、IP等设备信息
                    macItem = equipmentParamList.FirstOrDefault(x => x.ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMMAC);
                    ipItem = equipmentParamList.FirstOrDefault(x => x.ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMIP);
                    devIDItem = equipmentParamList.FirstOrDefault(x => x.ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMDEVID);
                    gatewayItem = equipmentParamList.FirstOrDefault(x => x.ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMGATEWAY);
                    maskItem = equipmentParamList.FirstOrDefault(x => x.ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMMASK);
                    macNoItem = equipmentParamList.FirstOrDefault(x => x.ID == equipment.ID && x.PARAM_CODE == ConstantHelper.JSDSPARAMMACNO);
                }
                if (macItem == null || string.IsNullOrWhiteSpace(macItem.PARAM_VALUE))
                {
                    ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：无MAC，跳过");
                    continue;
                }
                string mac = macItem.PARAM_VALUE.ToUpper().Replace('-', ':');
                if (devIDItem == null || string.IsNullOrWhiteSpace(devIDItem.PARAM_VALUE))
                {
                    ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：无devID，跳过");
                    continue;
                }
                //如果是速通，mac后3位加00与devId比较，若是不一样，也跳过：因为设备注册到jielink时 设备id由mac后3位加00生成
                if (equipment.PRODUCT_MODEL == ConstantHelper.JSC8ST)
                {
                    string deviceId = CommonHelper.ConvertDevicesId(mac);
                    if (devIDItem.PARAM_VALUE != deviceId)
                    {
                        ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：devID与jielink生成规则不符，跳过");
                        continue;
                    }
                }
                if (ipItem == null || string.IsNullOrWhiteSpace(ipItem.PARAM_VALUE))
                {
                    ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：无IP，跳过");
                    continue;
                }
                string[] ips = ipItem.PARAM_VALUE.Split('.');
                if (ips.Length != 4)
                {
                    ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：IP不对，跳过");
                    continue;
                }
                string gateway = string.Empty;
                if (gatewayItem != null && !string.IsNullOrWhiteSpace(gatewayItem.PARAM_VALUE))
                {
                    gateway = gatewayItem.PARAM_VALUE;
                }
                else
                {
                    //ip地址最后一位改为1
                    gateway = $"{ips[0]}.{ips[1]}.{ips[2]}.1";
                }
                string mask = string.Empty;
                if (maskItem != null && !string.IsNullOrWhiteSpace(maskItem.PARAM_VALUE))
                {
                    mask = maskItem.PARAM_VALUE;
                }
                else
                {
                    mask = "255.255.255.0";
                }
                string macNo = string.Empty;
                if (macNoItem != null && string.IsNullOrWhiteSpace(macNoItem.PARAM_VALUE))
                {
                    macNo = macNoItem.PARAM_VALUE;
                }
                string dguid = GetGuidString(equipment.ID);
                ControlDevices currentDevice = controlDevicesList.FirstOrDefault(x => x.DGUID == dguid || x.DeviceID == devIDItem.PARAM_VALUE);
                if (currentDevice != null)
                {
                    ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：设备已存在，跳过");
                    continue;
                }
                string parentGuid = equipment.EQUIPMENT_ID;
                if (string.IsNullOrWhiteSpace(equipment.REMARK))
                {
                    equipment.REMARK = REMARK;
                }
                int deviceType = 0;
                string model = string.Empty;
                if (equipment.PRODUCT_MODEL == ConstantHelper.JSMJY08)
                {
                    deviceType = ConstantHelper.JIELINK_JSMJY08A_OLD;    //老Y08
                    model = "JSMJY08A";
                }
                else if (equipment.PRODUCT_MODEL == ConstantHelper.JSMJK0220A)
                {
                    deviceType = ConstantHelper.JIELINK_JSMJK02_20;   //领御III型二门
                    model = "JSMJK02_20";
                }
                else if (equipment.PRODUCT_MODEL == ConstantHelper.JSMJK0240A)
                {
                    deviceType = ConstantHelper.JIELINK_JSMJK02_40;   //领御III型四门
                    model = "JSMJK02_40";
                }
                else if (equipment.PRODUCT_MODEL == ConstantHelper.JSC8ST)
                {
                    deviceType = ConstantHelper.JIELINK_JSTC1801_01;    //速通
                    model = "JSTC1801-01";
                }
                if (deviceType == 0)
                {
                    ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：不适配JieLink，跳过");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(parentGuid))
                {
                    string parentId = string.Empty;
                    string parentIp = string.Empty;
                    if (equipment.PRODUCT_MODEL == ConstantHelper.JSMJY08
                    || equipment.PRODUCT_MODEL == ConstantHelper.JSMJK0220A
                    || equipment.PRODUCT_MODEL == ConstantHelper.JSMJK0240A)
                    {
                        //控制器，找门将服务
                        if (mjDeivce == null)
                        {
                            ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：没有门禁服务，跳过");
                            continue;
                        }
                        parentId = mjDeivce.DeviceID;
                        parentIp = mjDeivce.IP;
                    }
                    else if (equipment.PRODUCT_MODEL == ConstantHelper.JSC8ST)
                    {
                        //车场控制器
                        if (parkBoxDeivce == null)
                        {
                            ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：没有车场盒子，跳过");
                            continue;
                        }
                        parentId = parkBoxDeivce.DeviceID;
                        parentIp = parkBoxDeivce.IP;
                    }
                    else
                    {
                        continue;
                    }
                    int deviceStatus = 0;
                    int authStatus = 0;
                    string qrCodeLink = string.Empty;
                    equipment.CREATE_TIME = CommonHelper.GetDateTimeValue(equipment.CREATE_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                    equipment.UPDATE_TIME = CommonHelper.GetDateTimeValue(equipment.UPDATE_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                    string sql = $"INSERT INTO control_devices(DGUID,DeviceID,IP,Mac,ParentID,Net_Mask,Gateway_IP,DeviceStatus,DeviceType,IoType,DeviceName,Model,DeviceClass,Mac2," +
                            $"Remark,InTime,UpdateTime,MasterIp,AuthStatus,MacNo,QrCodeLink,SpeakTecType,SpeakVideoType) VALUE('{dguid}','{devIDItem.PARAM_VALUE}','{ipItem.PARAM_VALUE}'," +
                            $"'{mac}','{parentId}','{mask}','{gateway}',{deviceStatus},{deviceType},0,'{equipment.EQUIP_NAME}','{model}',0,'{mac}','{equipment.REMARK}','{equipment.CREATE_TIME}'," +
                            $"'{equipment.UPDATE_TIME}','{parentIp}',{authStatus},'{macNo}','{qrCodeLink}',1,1)";
                    try
                    {
                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                        if (flag <= 0)
                        {
                            continue;
                        }
                    }
                    catch (Exception o)
                    {
                        ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：异常");
                        LogHelper.CommLogger.Error(o, $"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'异常：");
                        continue;
                    }
                    ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'：成功");
                    deviceSuccessImportList.Add(dguid);
                    if (deviceType == ConstantHelper.JIELINK_JSMJK02_20 || deviceType == ConstantHelper.JIELINK_JSMJK02_40)
                    {
                        //领御
                        string macToDeviceId = MACConvertDevicesId(mac);
                        if (string.IsNullOrWhiteSpace(macToDeviceId))
                        {
                            continue;
                        }
                        //找读卡器，将读卡器转化为门，有读卡器权限即有该门权限
                        List<TBaseEquipmentModel> doorEquipmentList = equipmentList.Where(x => x.PRODUCT_MODEL == ConstantHelper.JSMJK022040A_Reader
                            && x.EQUIPMENT_ID == equipment.ID).OrderBy(x => x.EQUIP_NAME).ToList();
                        int maxDoorCount = 0;
                        if (deviceType == ConstantHelper.JIELINK_JSMJK02_20)
                        {
                            //加2个门
                            maxDoorCount = 2;
                        }
                        else if (deviceType == ConstantHelper.JIELINK_JSMJK02_40)
                        {
                            //加4个门
                            maxDoorCount = 4;
                        }
                        for (int index = 0; index < doorEquipmentList.Count; index++)
                        {
                            if (index == maxDoorCount)
                            {
                                //已达门数量上限
                                break;
                            }
                            string doorGuid = GetGuidString(doorEquipmentList[index].ID);
                            string doorId = (CommonHelper.GetUIntValue(macToDeviceId) + index + 1).ToString();
                            string doorName = doorId + "门";
                            if (string.IsNullOrWhiteSpace(doorEquipmentList[index].REMARK))
                            {
                                doorEquipmentList[index].REMARK = REMARK;
                            }
                            sql = $"INSERT INTO control_devices(DGUID,DeviceID,Mac,ParentID,DeviceStatus,DeviceType,IoType,DeviceName,Model,DeviceClass,Mac2," +
                            $"Remark,InTime,UpdateTime,MasterIp,AuthStatus,SpeakTecType,SpeakVideoType) VALUE('{doorGuid}','{doorId}','{ConstantHelper.JIELINKDOORMAC}'," +
                            $"'{devIDItem.PARAM_VALUE}',{deviceStatus},{ConstantHelper.JIELINKDOORTYPE},0,'{doorName}','',0,'{ConstantHelper.JIELINKDOORMAC}','{equipment.REMARK}'," +
                            $"'{equipment.CREATE_TIME}','{equipment.UPDATE_TIME}','{ipItem.PARAM_VALUE}',{authStatus},1,1)";
                            try
                            {
                                int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                if (flag > 0)
                                {
                                    ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'，门='{doorId}'：成功");
                                    deviceSuccessImportList.Add(doorGuid);
                                }
                            }
                            catch (Exception o)
                            {
                                ShowMessage($"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'，门='{doorId}'：异常");
                                LogHelper.CommLogger.Error(o, $"04.迁移设备ID='{equipment.ID}'，设备类型='{equipment.PRODUCT_MODEL}'，门='{doorId}'异常：");
                            }
                        }
                    }
                }
            }
            string msg = string.Format($"04.迁移设备结束，合计处理{equipmentList.Count}条设备数据，成功迁移{deviceSuccessImportList.Count}个设备");
            ShowMessage(msg);
            LogHelper.CommLogger.Info(msg);
        }
        /// <summary>
        /// MAC提取设备ID
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string MACConvertDevicesId(string mac, string index = "")
        {
            string deviceId = string.Empty;
            if (string.IsNullOrWhiteSpace(mac))
            {
                return deviceId;
            }
            mac = mac.Replace(":", "").Substring(6, 6);
            mac = (string.IsNullOrEmpty(index) ? (mac + "00") : (mac + index));
            return Convert.ToUInt32(mac, 16).ToString();
        }

        /// <summary>
        /// 递归获取组织列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<TBaseDefendDistrictModel> GetDefendDistrictChildren(List<TBaseDefendDistrictModel> list, string parentId)
        {
            var result = new List<TBaseDefendDistrictModel>();
            var subareas = new List<TBaseDefendDistrictModel>();
            if (string.IsNullOrWhiteSpace(parentId))
            {
                subareas = list.Where(e => e.DISTRICT_ID == "" || e.DISTRICT_ID == null).ToList();
            }
            else
            {
                subareas = list.Where(e => e.DISTRICT_ID == parentId).ToList();
            }
            if (subareas != null)
            {
                result.AddRange(subareas);
                foreach (var subo in subareas)
                {
                    result.AddRange(GetDefendDistrictChildren(list, subo.ID));
                }
            }

            return result;
        }

        private void ControlAccessPointGroupImport(List<TBaseDefendDistrictModel> districtList)
        {
            if (districtList.Count == 0)
            {
                ShowMessage("05.迁移区域结束：districtList.Count == 0");
                LogHelper.CommLogger.Info("05.迁移区域结束：districtList.Count == 0");
                return;
            }
            //区域
            DataSet areaDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_access_point_group WHERE `Status`=0 ORDER BY id asc;");
            List<ControlAccessPointGroup> areaList = new List<ControlAccessPointGroup>();
            ControlAccessPointGroup areaRoot = null;
            if (areaDs != null && areaDs.Tables[0] != null && areaDs.Tables[0].Rows.Count > 0)
            {
                areaList = CommonHelper.DataTableToList<ControlAccessPointGroup>(areaDs.Tables[0]).OrderBy(x => x.Id).ToList();
                areaRoot = areaList.FirstOrDefault(x => x.ParentId == ConstantHelper.ROOTPARENTID);
            }
            List<string> areaSuccessImportList = new List<string>();
            if (areaList.Count > 0)
            {
                //parkNo不赋值的话，显示门禁区域，就可以选择车场和门禁设备了
                foreach (TBaseDefendDistrictModel district in districtList)
                {
                    if (string.IsNullOrWhiteSpace(district.REMARK))
                    {
                        district.REMARK = REMARK;
                    }
                    if (district.ID == ConstantHelper.JSDSDISTRICTROOT)
                    {
                        //根区域
                        if (areaRoot == null)
                        {
                            string jsdsRootId = GetGuidString(district.ID);
                            string areaCode = "A" + CommonHelper.GetTimeStamp().ToString(); //A+时间戳
                            string sql = $"INSERT INTO control_access_point_group(APGUID,ParkNo,APName,AreaCode,DefaultType,APType,ParentId,`Status`,CreatedOnUtc,Remark) VALUE('{jsdsRootId}',NULL,'{district.DISTRICT_NAME}','{areaCode}',1,1,'{ConstantHelper.ROOTPARENTID}',0,'{district.CREATE_TIME}','{district.REMARK}');";
                            try
                            {
                                int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                if (flag <= 0)
                                {
                                    ShowMessage("区域根节点插入失败");
                                    MessageBoxHelper.MessageBoxShowWarning("区域根节点插入失败");
                                    return;
                                }
                            }
                            catch (Exception o)
                            {
                                ShowMessage("区域根节点插入异常：详情请分析日志");
                                LogHelper.CommLogger.Error(o, "区域根节点插入异常：");
                                return;
                            }
                            areaSuccessImportList.Add(district.ID);
                        }
                        else
                        {
                            if (guidMapDic.ContainsKey(district.ID))
                            {
                                guidMapDic[district.ID] = areaRoot.APGUID;
                            }
                            else
                            {
                                guidMapDic.Add(district.ID, areaRoot.APGUID);
                            }
                            areaSuccessImportList.Add(district.ID);
                        }
                    }
                    else
                    {
                        string currentGuid = GetGuidString(district.ID);
                        //寻找当前组织是否已存在：判断是否重复升级
                        DataSet currentAreaDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, $"SELECT * from control_access_point_group WHERE `Status`=0 AND APGUID='{currentGuid}' ORDER BY id asc;");
                        ControlAccessPointGroup currentArea = null;
                        if (currentAreaDs != null && currentAreaDs.Tables[0] != null && currentAreaDs.Tables[0].Rows.Count > 0)
                        {
                            currentArea = CommonHelper.DataTableToList<ControlAccessPointGroup>(currentAreaDs.Tables[0]).FirstOrDefault();
                        }
                        if (currentArea != null)
                        {
                            ShowMessage("区域" + currentArea.APName + "已存在，跳过");
                            continue;
                        }
                        string parentGuid = string.Empty;
                        if (string.IsNullOrWhiteSpace(district.DISTRICT_ID) && district.ID != ConstantHelper.JSDSDISTRICTROOT)
                        {
                            //父节点为空的其他区域：如EXTERIOR
                            if (areaRoot == null)
                            {
                                continue;
                            }
                            parentGuid = areaRoot.APGUID;
                        }
                        else
                        {
                            parentGuid = GetGuidString(district.DISTRICT_ID);
                            DataSet parentAreaDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, $"SELECT * from control_access_point_group WHERE `Status`=0 AND APGUID='{parentGuid}' ORDER BY id asc;");
                            ControlAccessPointGroup parentArea = null;
                            if (parentAreaDs != null && parentAreaDs.Tables[0] != null && parentAreaDs.Tables[0].Rows.Count > 0)
                            {
                                parentArea = CommonHelper.DataTableToList<ControlAccessPointGroup>(parentAreaDs.Tables[0]).FirstOrDefault();
                            }
                            if (parentArea == null)
                            {
                                ShowMessage("区域" + district.DISTRICT_NAME + "的父节点区域不存在，跳过");
                                continue;
                            }
                        }
                        //车场区域
                        AreaInsert(currentGuid, ConstantHelper.PARKNO, district.DISTRICT_NAME, parentGuid, CommonHelper.GetDateTimeValue(district.CREATE_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"), district.REMARK, ref areaSuccessImportList);
                        //门禁区域
                        string doorGuid = GetAreaGuidString(district.ID);
                        string doorApName = string.Format("{0}-门禁", district.DISTRICT_NAME);
                        AreaInsert(doorGuid, null, doorApName, parentGuid, CommonHelper.GetDateTimeValue(district.CREATE_TIME, DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"), district.REMARK, ref areaSuccessImportList);
                    }
                }
            }
            string msg = $"05.迁移区域结束，合计处理{districtList.Count}条区域数据，成功迁移{areaSuccessImportList.Count}条区域";
            ShowMessage(msg);
            LogHelper.CommLogger.Info(msg);

            areaList.Clear();
            areaSuccessImportList.Clear();
        }
        /// <summary>
        /// jsds的区域迁移到jielink后：生成车场区域和门禁区域
        /// </summary>
        /// <param name="district"></param>
        /// <param name="apName"></param>
        /// <param name="currentGuid"></param>
        /// <param name="parentGuid"></param>
        /// <param name="areaSuccessImportList"></param>
        private void AreaInsert(string currentGuid, string parkNo, string apName, string parentGuid, string createTime, string remark, ref List<string> areaSuccessImportList)
        {
            string areaCode = "A" + CommonHelper.GetTimeStamp().ToString(); //A+时间戳
            string sql = string.Empty;
            if (string.IsNullOrWhiteSpace(parkNo))
            {
                sql = $"INSERT INTO control_access_point_group(APGUID,ParkNo,APName,AreaCode,DefaultType,APType,ParentId,`Status`,CreatedOnUtc,Remark) VALUE('{currentGuid}',null,'{apName}','{areaCode}',1,1,'{parentGuid}',0,'{createTime}','{remark}');";
            }
            else
            {
                sql = $"INSERT INTO control_access_point_group(APGUID,ParkNo,APName,AreaCode,DefaultType,APType,ParentId,`Status`,CreatedOnUtc,Remark) VALUE('{currentGuid}','{parkNo}','{apName}','{areaCode}',1,1,'{parentGuid}',0,'{createTime}','{remark}');";
            }
            try
            {
                int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                if (flag <= 0)
                {
                    ShowMessage("区域" + apName + "插入失败");
                }
                else
                {
                    areaSuccessImportList.Add(currentGuid);
                }
            }
            catch (Exception o)
            {
                ShowMessage("区域" + apName + "插入异常：详情请分析日志");
                LogHelper.CommLogger.Error(o, "区域" + apName + "插入异常：");
            }
        }

        /// <summary>
        /// jsds区域迁移到jielink时，生成车场和门禁区域，区域若没有关联设备，可直接删除
        /// 门禁区域的guid映射好，后面可能用到
        /// </summary>
        /// <param name="jsdsID"></param>
        /// <param name="jielinkGuid"></param>
        private string GetAreaGuidString(string jsdsDistrictId)
        {
            string jielinkGuid = string.Empty;
            if (areaGuidMapDic.ContainsKey(jsdsDistrictId))
            {
                jielinkGuid = areaGuidMapDic[jsdsDistrictId];
            }
            else
            {
                jielinkGuid = Guid.NewGuid().ToString();
                areaGuidMapDic.Add(jsdsDistrictId, jielinkGuid);
            }
            return jielinkGuid;
        }

        /// <summary>
        /// 迁移设备权限
        /// </summary>
        private void DeviceRightImport()
        {
            if (!policy.DeviceRightSelect)
            {
                return;
            }
            List<TBaseDefendDistrictModel> districtList = new List<TBaseDefendDistrictModel>();
            DataSet districtDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "select* from t_base_defend_district WHERE STATE = 'NORMAL' ORDER BY DISTRICT_ID ASC;");
            if (districtDs != null && districtDs.Tables[0] != null && districtDs.Tables[0].Rows.Count > 0)
            {
                List<TBaseDefendDistrictModel> tempDistrictList = CommonHelper.DataTableToList<TBaseDefendDistrictModel>(districtDs.Tables[0]).OrderBy(x => x.DISTRICT_ID).ToList();
                districtList = GetDefendDistrictChildren(tempDistrictList, "");
                tempDistrictList.Clear();
            }
            ControlAccessPointGroupImport(districtList);
            //设备
            DataSet controlDeviceDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_devices ORDER BY id ASC;");
            List<ControlDevices> controlDevicesList = new List<ControlDevices>();
            if (controlDeviceDs != null && controlDeviceDs.Tables[0] != null && controlDeviceDs.Tables[0].Rows.Count > 0)
            {
                controlDevicesList = CommonHelper.DataTableToList<ControlDevices>(controlDeviceDs.Tables[0]).OrderBy(x => x.ID).ToList();
            }
            if (controlDevicesList.Count == 0)
            {
                //已经迁移过，但迁移不成功
                ShowMessage("05.迁移设备权限结束：controlDevicesList空");
                return;
            }
            DataSet personDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_person where `Status`=0 ORDER BY Id DESC;");
            List<ControlPerson> personList = new List<ControlPerson>();
            if (personDs != null && personDs.Tables[0] != null && personDs.Tables[0].Rows.Count > 0)
            {
                personList = CommonHelper.DataTableToList<ControlPerson>(personDs.Tables[0]).OrderByDescending(x => x.Id).ToList();
            }
            if (personList.Count == 0)
            {
                //已经迁移过，但迁移不成功
                ShowMessage("04.迁移设备权限结束：personList.Count == 0");
                return;
            }
            DataSet equipmentDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_base_equipment WHERE EQUIP_STATE='NORMAL' ORDER BY EQUIPMENT_ID asc;");
            List<TBaseEquipmentModel> equipmentList = new List<TBaseEquipmentModel>();
            if (equipmentDs != null && equipmentDs.Tables[0] != null && equipmentDs.Tables[0].Rows.Count > 0)
            {
                equipmentList = CommonHelper.DataTableToList<TBaseEquipmentModel>(equipmentDs.Tables[0]).OrderBy(x => x.EQUIPMENT_ID).ToList();
            }
            if (equipmentList.Count == 0)
            {
                ShowMessage("04.迁移设备权限结束：equipmentList.Count == 0");
                return;
            }
            //车场服务、门禁服务
            DataSet parkServiceDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT* from control_lease_stall WHERE `Status`= 0 ORDER BY OperateTime DESC;");
            List<ControlLeaseStall> parkServiceList = new List<ControlLeaseStall>();
            if (parkServiceDs != null && parkServiceDs.Tables[0] != null && parkServiceDs.Tables[0].Rows.Count > 0)
            {
                parkServiceList = CommonHelper.DataTableToList<ControlLeaseStall>(parkServiceDs.Tables[0]).OrderByDescending(x => x.OperateTime).ToList();
            }
            DataSet doorServiceDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * FROM system_set_door_service_to_person WHERE `Status`=0 ORDER BY OperTime DESC;");
            List<SystemSetDoorServiceToPerson> doorServiceList = new List<SystemSetDoorServiceToPerson>();
            if (doorServiceDs != null && doorServiceDs.Tables[0] != null && doorServiceDs.Tables[0].Rows.Count > 0)
            {
                doorServiceList = CommonHelper.DataTableToList<SystemSetDoorServiceToPerson>(doorServiceDs.Tables[0]).OrderBy(x => x.OperTime).ToList();
            }
            if (parkServiceList.Count == 0 && doorServiceList.Count == 0)
            {
                ShowMessage("04.迁移设备权限结束：parkServiceList.Count == 0 && doorServiceList.Count == 0");
                return;
            }
            //区域
            List<ControlAccessPointGroup> areaList = new List<ControlAccessPointGroup>();
            DataSet areaDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_access_point_group WHERE `Status`=0 ORDER BY id ASC;");
            if (areaDs != null && areaDs.Tables[0] != null && areaDs.Tables[0].Rows.Count > 0)
            {
                areaList = CommonHelper.DataTableToList<ControlAccessPointGroup>(areaDs.Tables[0]).OrderBy(x => x.Id).ToList();
            }
            //服务
            DataSet cacauthserviceDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "select * from t_cac_auth_service WHERE STATE='NORMAL' AND ID in (SELECT AUTHSERVICE_ID FROM t_cac_auth_establish WHERE `STATUS`='NORMAL') ORDER BY CREATE_TIME DESC;");
            List<TCacAuthServiceModel> cacauthserviceList = new List<TCacAuthServiceModel>();
            if (cacauthserviceDs != null && cacauthserviceDs.Tables[0] != null && cacauthserviceDs.Tables[0].Rows.Count > 0)
            {
                cacauthserviceList = CommonHelper.DataTableToList<TCacAuthServiceModel>(cacauthserviceDs.Tables[0]).OrderByDescending(x => x.CREATE_TIME).ToList();
            }
            if (cacauthserviceList.Count == 0)
            {
                ShowMessage("05.迁移设备权限结束：cacauthserviceList.Count == 0");
                return;
            }
            //账号、用户
            List<string> accountIdList = cacauthserviceList.Select(x => x.ACCOUNT_ID).Distinct().ToList();
            string accountIdstr = string.Format("'{0}'", string.Join("','", accountIdList));
            DataSet accountDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, $"SELECT * from t_cac_account where `STATUS`= 'NORMAL' AND ID in ({accountIdstr});");
            List<TCacAccountModel> accountList = new List<TCacAccountModel>();
            List<string> personIdList = new List<string>();
            if (accountDs != null && accountDs.Tables[0] != null && accountDs.Tables[0].Rows.Count > 0)
            {
                accountList = CommonHelper.DataTableToList<TCacAccountModel>(accountDs.Tables[0]).OrderByDescending(x => x.CREATE_TIME).ToList();
            }
            if (accountList.Count == 0)
            {
                ShowMessage("05.迁移设备权限结束：accountList.Count == 0");
                return;
            }
            personIdList = CommonHelper.DataTableToList<TCacAccountModel>(accountDs.Tables[0]).OrderByDescending(x => x.CREATE_TIME).Select(x => x.PERSON_ID).ToList();
            if (personIdList.Count == 0)
            {
                ShowMessage("05.迁移设备权限结束：personIdList.Count == 0");
                return;
            }
            int doorRightSuccessImportCount = 0;
            int parkRightSuccessImportCount = 0;
            int parkServiceAreaSuccessImportCount = 0;
            foreach (TBaseDefendDistrictModel districtModel in districtList)
            {
                string apguid = GetGuidString(districtModel.ID);
                ControlAccessPointGroup area = areaList.FirstOrDefault(x => x.APGUID == apguid);
                if (area == null)
                {
                    continue;
                }
                //区域+通道
                DataSet establishDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, $"select * from t_base_establish WHERE STATE='NORMAL' AND DISTRICT_ID='{districtModel.ID}' ORDER BY ESTABLISH_ID ASC;");
                List<TBaseEstablishModel> establishList = new List<TBaseEstablishModel>();
                if (establishDs != null && establishDs.Tables[0] != null && establishDs.Tables[0].Rows.Count > 0)
                {
                    establishList = CommonHelper.DataTableToList<TBaseEstablishModel>(establishDs.Tables[0]).OrderBy(x => x.ESTABLISH_ID).ToList();
                }
                if (establishList.Count == 0)
                {
                    continue;
                }
                List<string> estaIdList = establishList.Select(x => x.ID).ToList();
                string estaIdStr = string.Format("'{0}'", string.Join("','", estaIdList));
                //通道+设备
                List<TBaseEstablishREquipmentModel> establishEquipmentList = IsEstablishREquipmentGoon(string.Empty, estaIdStr);
                if (establishEquipmentList.Count == 0)
                {
                    continue;
                }
                List<TBaseEquipmentModel> tempEquipmentList = IsEquipmentGoon(establishEquipmentList, equipmentList, null);
                if (tempEquipmentList.Count == 0)
                {
                    continue;
                }
                foreach (TBaseEquipmentModel equipmentModel in tempEquipmentList)
                {
                    string dguid = string.Empty;
                    string insertAreaGuid = area.APGUID;
                    if (equipmentModel.PRODUCT_MODEL == ConstantHelper.JSMJY08_Reader)
                    {
                        //Y08的话，读卡器关联通道：有读卡器找到Y08，即有Y08权限
                        dguid = GetGuidString(equipmentModel.EQUIPMENT_ID);
                    }
                    else
                    {
                        //1. 领御，直接对应门 equipmentModel.PRODUCT_MODEL == ConstantHelper.JSMJK022040A_Reader，有读卡器即有门权限
                        //2. 车场控制器：直接关联通道
                        dguid = GetGuidString(equipmentModel.ID);
                    }
                    ControlDevices device = controlDevicesList.FirstOrDefault(x => x.DGUID == dguid);
                    if (device == null)
                    {
                        continue;
                    }
                    if (device.DeviceType == ConstantHelper.JIELINK_JSMJY08A_OLD
                        || device.DeviceType == ConstantHelper.JIELINK_JSMJK02_20
                        || device.DeviceType == ConstantHelper.JIELINK_JSMJK02_40
                        || device.DeviceType == ConstantHelper.JIELINKDOORTYPE)
                    {
                        //找门禁区域
                        string doorAreaGuid = GetAreaGuidString(districtModel.ID);
                        ControlAccessPointGroup doorArea = areaList.FirstOrDefault(x => x.APGUID == doorAreaGuid);
                        if (doorArea == null)
                        {
                            continue;
                        }
                        insertAreaGuid = doorArea.APGUID;
                    }
                    try
                    {
                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, $"INSERT INTO control_ap_devcies(AGUID,APGUID,DGUID,`Status`,CreateTime) VALUE('{Guid.NewGuid().ToString()}','{insertAreaGuid}','{device.DGUID}',0,'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');");
                        if (flag <= 0)
                        {
                            ShowMessage($"05.迁移区域='{area.APName}'【'{area.AreaCode}'】的设备关系：'{device.DeviceName}'【{device.DeviceID}】失败");
                        }
                        else
                        {
                            ShowMessage($"05.迁移区域='{area.APName}'【'{area.AreaCode}'】的设备关系：'{device.DeviceName}'【{device.DeviceID}】成功");
                        }
                    }
                    catch (Exception o)
                    {
                        ShowMessage($"05.迁移区域='{area.APName}'【'{area.AreaCode}'】的设备关系：'{device.DeviceName}'【{device.DeviceID}】异常，详情请分析日志");
                        LogHelper.CommLogger.Error(o, $"05.迁移区域='{area.APName}'【'{area.AreaCode}'】的设备关系：'{device.DeviceName}'【{device.DeviceID}】异常，");
                    }
                }
            }
            foreach (string personId in personIdList)
            {
                string personGuid = GetGuidString(personId);
                ControlPerson person = personList.FirstOrDefault(x => x.PGUID == personGuid);
                if (person == null)
                {
                    ShowMessage($"04.迁移personNo='{personId}'的设备权限：用户不存在，不迁移权限，跳过");
                    continue;
                }
                if (person.CanInOut == 1)
                {
                    ShowMessage($"04.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的设备权限：用户权限已存在，不重复迁移权限，跳过");
                    continue;
                }
                TCacAccountModel account = accountList.FirstOrDefault(x => x.PERSON_ID == personId);
                if (account == null)
                {
                    ShowMessage($"04.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的设备权限：用户账号不存在，不迁移权限，跳过");
                    continue;
                }
                List<TCacAuthServiceModel> cacAuthServices = cacauthserviceList.Where(x => x.ACCOUNT_ID == account.ID).ToList();
                if (cacAuthServices.Count == 0)
                {
                    ShowMessage($"04.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的设备权限：未开通服务，不迁移权限，跳过");
                    continue;
                }
                List<string> parkServiceIdList = cacAuthServices.Where(x => x.BUSINESS_CODE == "PARK").Select(x => x.ID).ToList();
                List<string> doorServiceIdList = cacAuthServices.Where(x => x.BUSINESS_CODE == "DOOR").Select(x => x.ID).ToList();
                bool canInOut = false;  //用户的设备权限
                if (doorServiceIdList.Count > 0)
                {
                    SystemSetDoorServiceToPerson doorservice = doorServiceList.FirstOrDefault(x => x.PersonNo == person.PersonNo);
                    if (doorservice == null)
                    {
                        ShowMessage($"04.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的设备权限：门禁服务迁移不成功，不迁移门禁权限，跳过");
                    }
                    else
                    {
                        string doorServiceIdStr = string.Format("'{0}'", string.Join("','", doorServiceIdList));
                        //服务+通道      
                        List<TCacauthEstablishModel> currDoorServiceEstablishList = IsServiceEstablishGoon(personGuid, doorServiceIdStr);
                        if (currDoorServiceEstablishList.Count > 0)
                        {
                            List<string> estaIdList = currDoorServiceEstablishList.Select(x => x.ESTABLISH_ID).Distinct().ToList();
                            string estaIdStr = string.Format("'{0}'", string.Join("','", estaIdList));
                            //通道+设备
                            List<TBaseEstablishREquipmentModel> currDoorEstablishEquipmentList = IsEstablishREquipmentGoon(personGuid, estaIdStr);
                            if (currDoorEstablishEquipmentList.Count > 0)
                            {
                                List<TBaseEquipmentModel> currEquipmentList = IsEquipmentGoon(currDoorEstablishEquipmentList, equipmentList, person);
                                if (currEquipmentList.Count > 0)
                                {
                                    foreach (TBaseEquipmentModel equipmentModel in currEquipmentList)
                                    {
                                        string dguid = string.Empty;
                                        if (equipmentModel.PRODUCT_MODEL == ConstantHelper.JSMJY08_Reader)
                                        {
                                            //对应控制器
                                            dguid = GetGuidString(equipmentModel.EQUIPMENT_ID);
                                        }
                                        else
                                        {
                                            //领御，直接对应门
                                            dguid = GetGuidString(equipmentModel.ID);
                                        }
                                        ControlDevices device = controlDevicesList.FirstOrDefault(x => x.DGUID == dguid);
                                        if (device == null)
                                        {
                                            continue;
                                        }
                                        if (!(device.DeviceType == ConstantHelper.JIELINK_JSMJY08A_OLD
                                            || device.DeviceType == ConstantHelper.JIELINK_JSMJK02_20
                                            || device.DeviceType == ConstantHelper.JIELINK_JSMJK02_40
                                            || device.DeviceType == ConstantHelper.JIELINKDOORTYPE))
                                        {
                                            //不是门禁设备
                                            continue;
                                        }
                                        try
                                        {
                                            int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, $"INSERT INTO control_person_device_relation(PDGUID, PGUID, DGUID, ISDeleted, Type, UserType) VALUE(UUID(), '{personGuid}', '{dguid}', 0, 2, 0);");
                                            if (flag <= 0)
                                            {
                                                ShowMessage($"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的设备权限：'{device.DeviceName}'【{device.DeviceID}】门禁设备权限失败");
                                            }
                                            else
                                            {
                                                ShowMessage($"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的设备权限：'{device.DeviceName}'【{device.DeviceID}】门禁设备权限成功");
                                                doorRightSuccessImportCount++;
                                                canInOut = true;
                                            }
                                        }
                                        catch (Exception o)
                                        {
                                            ShowMessage($"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的设备权限：'{device.DeviceName}'【{device.DeviceID}】门禁设备权限异常，详情请分析日志");
                                            LogHelper.CommLogger.Error(o, $"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的设备权限：'{device.DeviceName}'【{device.DeviceID}】门禁设备权限异常，");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (parkServiceIdList.Count > 0)
                {
                    string parkServiceIdStr = string.Format("'{0}'", string.Join("','", parkServiceIdList));
                    //服务+通道      
                    List<TCacauthEstablishModel> currParkServiceEstablishList = IsServiceEstablishGoon(personGuid, parkServiceIdStr);
                    if (currParkServiceEstablishList.Count > 0)
                    {
                        List<string> estaIdList = currParkServiceEstablishList.Select(x => x.ESTABLISH_ID).Distinct().ToList();
                        string estaIdStr = string.Format("'{0}'", string.Join("','", estaIdList));
                        //通道+设备
                        List<TBaseEstablishREquipmentModel> currParkEstablishEquipmentList = IsEstablishREquipmentGoon(personGuid, estaIdStr);
                        List<TBaseEquipmentModel> currEquipmentList = new List<TBaseEquipmentModel>();
                        if (currParkEstablishEquipmentList.Count > 0)
                        {
                            currEquipmentList = IsEquipmentGoon(currParkEstablishEquipmentList, equipmentList, person);
                        }
                        if (currEquipmentList.Count > 0)
                        {
                            foreach (string serviceId in parkServiceIdList)
                            {
                                string serviceGuid = GetGuidString(serviceId);
                                ControlLeaseStall parkService = parkServiceList.FirstOrDefault(x => x.LGUID == serviceGuid);
                                if (parkService == null)
                                {
                                    continue;
                                }
                                DataSet serviceDistrictDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, $"SELECT * from t_cac_auth_service_district WHERE `STATUS`= 'NORMAL' AND AUTH_SERVICE_ID = '{serviceId}' ORDER BY CREATE_TIME asc;");
                                List<TCacauthServiceDistrictModel> serviceDistrictList = new List<TCacauthServiceDistrictModel>();
                                if (serviceDistrictDs != null && serviceDistrictDs.Tables[0] != null && serviceDistrictDs.Tables[0].Rows.Count > 0)
                                {
                                    serviceDistrictList = CommonHelper.DataTableToList<TCacauthServiceDistrictModel>(serviceDistrictDs.Tables[0]);
                                }
                                if (serviceDistrictList.Count > 0)
                                {
                                    List<string> districtIdList = serviceDistrictList.Select(x => x.PARK_DISTRICT_ID).ToList();
                                    foreach (string districtId in districtIdList)
                                    {
                                        string apguid = GetGuidString(districtId);
                                        ControlAccessPointGroup area = areaList.FirstOrDefault(x => x.APGUID == apguid);
                                        if (area == null)
                                        {
                                            continue;
                                        }
                                        try
                                        {
                                            int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, $"INSERT INTO control_area_service(Guid,APGuid,PGuid,LGuid) VALUE('{Guid.NewGuid().ToString()}','{area.APGUID}','{person.PGUID}','{parkService.LGUID}');");
                                            if (flag <= 0)
                                            {
                                                ShowMessage($"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的服务区域关系：'{serviceId}'-'{districtId}'失败");
                                            }
                                            else
                                            {
                                                ShowMessage($"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的服务区域关系：'{serviceId}'-'{districtId}'成功");
                                                parkServiceAreaSuccessImportCount++;
                                            }
                                        }
                                        catch (Exception o)
                                        {
                                            ShowMessage($"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的服务区域关系：'{serviceId}'-'{districtId}'异常，详情请分析日志");
                                            LogHelper.CommLogger.Error(o, $"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的服务区域关系：'{serviceId}'-'{districtId}'异常，");
                                            return;
                                        }
                                    }
                                }

                                DataSet voucherDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, $"SELECT * from control_voucher WHERE `Status` in (1,2) AND LGUID in ('{serviceGuid}') ORDER BY AddTime ASC;");
                                List<ControlVoucher> currVoucherList = new List<ControlVoucher>();
                                if (voucherDs != null && voucherDs.Tables[0] != null && voucherDs.Tables[0].Rows.Count > 0)
                                {
                                    currVoucherList = CommonHelper.DataTableToList<ControlVoucher>(voucherDs.Tables[0]);
                                }
                                if (currVoucherList.Count == 0)
                                {
                                    continue;
                                }
                                List<TCacauthEstablishModel> tempParkServiceEstablishList = currParkServiceEstablishList.Where(x => x.AUTHSERVICE_ID == serviceId).ToList();
                                if (tempParkServiceEstablishList.Count == 0)
                                {
                                    continue;
                                }
                                List<string> tempEstaIdList = tempParkServiceEstablishList.Select(x => x.ESTABLISH_ID).ToList();
                                List<TBaseEstablishREquipmentModel> tempParkEstablishEquipmentList = currParkEstablishEquipmentList.Where(x => tempEstaIdList.Contains(x.ESTA_ID)).ToList();
                                if (tempParkEstablishEquipmentList.Count == 0)
                                {
                                    continue;
                                }
                                List<string> tempEquipIdList = tempParkEstablishEquipmentList.Select(x => x.EQUI_ID).ToList();
                                List<TBaseEquipmentModel> tempEquipmentList = currEquipmentList.Where(x => tempEquipIdList.Contains(x.ID)).ToList();
                                foreach (TBaseEquipmentModel equipment in tempEquipmentList)
                                {
                                    string dguid = GetGuidString(equipment.ID);
                                    ControlDevices device = controlDevicesList.FirstOrDefault(x => x.DGUID == dguid);
                                    if (device == null)
                                    {
                                        continue;
                                    }
                                    if (device.DeviceType != ConstantHelper.JIELINK_JSTC1801_01)
                                    {
                                        //不是车场控制器：目前只支持速通1810-01
                                        continue;
                                    }
                                    foreach (ControlVoucher voucher in currVoucherList)
                                    {
                                        try
                                        {
                                            int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, $"INSERT INTO control_voucher_device(Id, VGuid, DGuid, DeviceId,`Status`) VALUE('{Guid.NewGuid().ToString()}', '{voucher.Guid}', '{device.DGUID}', '{device.DeviceID}', 0);");
                                            if (flag <= 0)
                                            {
                                                ShowMessage($"05.迁移VoucherNo='{voucher.VoucherNo}'的设备权限：'{device.DeviceName}'【{device.DeviceID}】车场设备权限失败");
                                            }
                                            else
                                            {
                                                ShowMessage($"05.迁移VoucherNo='{voucher.VoucherNo}'的设备权限：'{device.DeviceName}'【{device.DeviceID}】车场设备权限成功");
                                                parkRightSuccessImportCount++;
                                                canInOut = true;
                                            }
                                        }
                                        catch (Exception o)
                                        {
                                            ShowMessage($"05.迁移VoucherNo='{voucher.VoucherNo}'的设备权限：'{device.DeviceName}'【{device.DeviceID}】车场设备权限异常，详情请分析日志");
                                            LogHelper.CommLogger.Error(o, $"05.迁移VoucherNo='{voucher.VoucherNo}'的设备权限：：'{device.DeviceName}'【{device.DeviceID}】车场设备权限异常，");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (canInOut)
                {
                    try
                    {
                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, $"UPDATE control_person SET CanInOut=1 WHERE PGUID='{person.PGUID}';");
                        if (flag <= 0)
                        {
                            ShowMessage($"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的有权限标识失败");
                        }
                        else
                        {
                            ShowMessage($"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的有权限标识成功");
                        }
                    }
                    catch (Exception o)
                    {
                        ShowMessage($"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的有权限标识异常，详情请分析日志");
                        LogHelper.CommLogger.Error(o, $"05.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的有权限标识异常，");
                        return;
                    }
                }
            }
            string msg = string.Format($"05.迁移设备权限结束，成功迁移{parkRightSuccessImportCount}条凭证设备关系数据，{doorRightSuccessImportCount}条人与设备关系数据," +
                $"{parkServiceAreaSuccessImportCount}条服务区域数据");
            ShowMessage(msg);
            LogHelper.CommLogger.Info(msg);

            districtList.Clear();
            controlDevicesList.Clear();
            equipmentList.Clear();
            cacauthserviceList.Clear();
            areaList.Clear();
            personList.Clear();
            personIdList.Clear();
            accountList.Clear();
            parkServiceList.Clear();
            doorServiceList.Clear();
        }
        /// <summary>
        /// 判断服务是否绑定通道
        /// </summary>
        /// <param name="personGuid"></param>
        /// <param name="serviceIdStr">车场服务、门禁服务分开判断</param>
        /// <returns></returns>
        private List<TCacauthEstablishModel> IsServiceEstablishGoon(string personGuid, string serviceIdStr)
        {
            //服务+通道
            DataSet currServiceEstablishDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, $"SELECT * FROM t_cac_auth_establish WHERE `STATUS`='NORMAL' AND AUTHSERVICE_ID in ({serviceIdStr}) ORDER BY authservice_id ASC;");
            List<TCacauthEstablishModel> currServiceEstablishList = new List<TCacauthEstablishModel>();
            if (currServiceEstablishDs != null && currServiceEstablishDs.Tables[0] != null && currServiceEstablishDs.Tables[0].Rows.Count > 0)
            {
                currServiceEstablishList = CommonHelper.DataTableToList<TCacauthEstablishModel>(currServiceEstablishDs.Tables[0]).OrderBy(x => x.ESTABLISH_ID).ToList();
            }
            if (currServiceEstablishList.Count == 0)
            {
                ShowMessage($"04.迁移personId='{personGuid}'的设备权限：未关联通道，跳过");
            }
            return currServiceEstablishList;
        }

        /// <summary>
        /// 判断通道是否绑定设备
        /// </summary>
        /// <param name="personGuid"></param>
        /// <param name="estaIdStr">车场、门禁分开处理</param>
        /// <returns></returns>
        private List<TBaseEstablishREquipmentModel> IsEstablishREquipmentGoon(string personGuid, string estaIdStr, bool enableShowMsg = true)
        {
            //通道+设备
            DataSet currEstablishEquipmentDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, $"SELECT * from t_base_establish_r_equi WHERE STATE='NORMAL' AND ESTA_ID in ({estaIdStr}) ORDER BY ESTA_ID ASC,CREATE_TIME ASC;");
            List<TBaseEstablishREquipmentModel> currEstablishEquipmentList = new List<TBaseEstablishREquipmentModel>();
            if (currEstablishEquipmentDs != null && currEstablishEquipmentDs.Tables[0] != null && currEstablishEquipmentDs.Tables[0].Rows.Count > 0)
            {
                currEstablishEquipmentList = CommonHelper.DataTableToList<TBaseEstablishREquipmentModel>(currEstablishEquipmentDs.Tables[0]).OrderBy(x => x.ESTA_ID).ToList();
            }
            if (enableShowMsg && currEstablishEquipmentList.Count == 0)
            {
                ShowMessage($"04.迁移personId='{personGuid}'的设备权限：未关联设备，跳过");
            }
            return currEstablishEquipmentList;
        }

        /// <summary>
        /// 判断是否有设备
        /// </summary>
        /// <param name="currEstablishEquipmentList"></param>
        /// <param name="equipmentList"></param>
        /// <param name="person"></param>
        /// <returns></returns>
        private List<TBaseEquipmentModel> IsEquipmentGoon(List<TBaseEstablishREquipmentModel> currEstablishEquipmentList, List<TBaseEquipmentModel> equipmentList, ControlPerson person)
        {
            List<string> equipIdList = currEstablishEquipmentList.Select(x => x.EQUI_ID).Distinct().ToList();
            List<TBaseEquipmentModel> currEquipmentList = equipmentList.Where(x => equipIdList.Contains(x.ID)).ToList();
            if (person != null && currEquipmentList.Count == 0)
            {
                ShowMessage($"04.迁移PersonName='{person.PersonName}'【'{person.PersonNo}'】的设备权限：没有设备，跳过");
            }
            return currEquipmentList;
        }

        /// <summary>
        /// 获取入场设备信息
        /// </summary>
        /// <param name="controlDevicesList"></param>
        /// <param name="recordIn"></param>
        /// <param name="enterDeviceId"></param>
        /// <param name="enterDeviceName"></param>
        private void GetEnterDeviceInfo(List<ControlDevices> controlDevicesList, TParkRecordInModel recordIn, ref string enterDeviceId, ref string enterDeviceName)
        {
            if (policy.ControlDeviceSelect)
            {
                string dguid = GetGuidString(recordIn.EQUIPMENT_ID);
                ControlDevices device = controlDevicesList.FirstOrDefault(x => x.DGUID == dguid);
                if (device != null)
                {
                    enterDeviceId = device.DeviceID;
                    enterDeviceName = device.DeviceName;
                }
            }
        }
        /// <summary>
        /// 获取出场设备信息
        /// </summary>
        /// <param name="controlDevicesList"></param>
        /// <param name="recordIn"></param>
        /// <param name="enterDeviceId"></param>
        /// <param name="enterDeviceName"></param>
        private void GetOutDeviceInfo(List<ControlDevices> controlDevicesList, TParkRecordOutModel recordOut, ref string outDeviceId, ref string outDeviceName, ref ControlDevices chargeBox)
        {
            if (policy.ControlDeviceSelect)
            {
                string dguid = GetGuidString(recordOut.EQUIPMENT_ID);
                ControlDevices device = controlDevicesList.FirstOrDefault(x => x.DGUID == dguid);
                if (device != null)
                {
                    outDeviceId = device.DeviceID;
                    outDeviceName = device.DeviceName;

                    chargeBox = controlDevicesList.FirstOrDefault(x => x.DeviceID == device.ParentID);
                }
            }
        }

        /// <summary>
        /// 匹配不到入场记录的出场记录，入场id可能为空或者0，生成新的入场id
        /// </summary>
        /// <param name="recordOutId">出场记录Id</param>
        /// <returns></returns>
        private string GetOutEnterRecordIn(string recordOutId)
        {
            string recordInId = string.Empty;
            if (recordOutIdMapDic.ContainsKey(recordOutId))
            {
                recordInId = recordOutIdMapDic[recordOutId];
            }
            else
            {
                recordInId = Guid.NewGuid().ToString().Replace("-", "");
            }
            return recordInId;
        }

        public string IpAddr
        {
            get { return (string)GetValue(IpAddrProperty); }
            set { SetValue(IpAddrProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IpAddr.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IpAddrProperty =
            DependencyProperty.Register("IpAddr", typeof(string), typeof(JSDSToJieLinkViewModel));


        public string DbName
        {
            get { return (string)GetValue(DbNameProperty); }
            set { SetValue(DbNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DbName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DbNameProperty =
            DependencyProperty.Register("DbName", typeof(string), typeof(JSDSToJieLinkViewModel));


        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(JSDSToJieLinkViewModel));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(JSDSToJieLinkViewModel));



        public int Port
        {
            get { return (int)GetValue(PortProperty); }
            set { SetValue(PortProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Port.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PortProperty =
            DependencyProperty.Register("Port", typeof(int), typeof(JSDSToJieLinkViewModel));




        public bool EnterRecord
        {
            get { return (bool)GetValue(EnterRecordProperty); }
            set { SetValue(EnterRecordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnterRecord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnterRecordProperty =
            DependencyProperty.Register("EnterRecord", typeof(bool), typeof(JSDSToJieLinkViewModel));



        public bool OutRecord
        {
            get { return (bool)GetValue(OutRecordProperty); }
            set { SetValue(OutRecordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutRecord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutRecordProperty =
            DependencyProperty.Register("OutRecord", typeof(bool), typeof(JSDSToJieLinkViewModel));




        public bool BillRecord
        {
            get { return (bool)GetValue(BillRecordProperty); }
            set { SetValue(BillRecordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BillRecord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BillRecordProperty =
            DependencyProperty.Register("BillRecord", typeof(bool), typeof(JSDSToJieLinkViewModel));



        public bool Park
        {
            get { return (bool)GetValue(ParkProperty); }
            set { SetValue(ParkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Park.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParkProperty =
            DependencyProperty.Register("Park", typeof(bool), typeof(JSDSToJieLinkViewModel));




        public bool Door
        {
            get { return (bool)GetValue(DoorProperty); }
            set { SetValue(DoorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Door.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoorProperty =
            DependencyProperty.Register("Door", typeof(bool), typeof(JSDSToJieLinkViewModel));




        public Dictionary<int, string> ImportTimeDataSource
        {
            get { return (Dictionary<int, string>)GetValue(ImportTimeDataSourceProperty); }
            set { SetValue(ImportTimeDataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImportTimeDataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImportTimeDataSourceProperty =
            DependencyProperty.Register("ImportTimeDataSource", typeof(Dictionary<int, string>), typeof(JSDSToJieLinkViewModel));



        public int SelectMonth
        {
            get { return (int)GetValue(SelectMonthProperty); }
            set { SetValue(SelectMonthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectMonth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectMonthProperty =
            DependencyProperty.Register("SelectMonth", typeof(int), typeof(JSDSToJieLinkViewModel));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(JSDSToJieLinkViewModel));


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

        /// <summary>
        /// 设备
        /// </summary>
        public bool ControlDevice
        {
            get { return (bool)GetValue(ControlDeviceProperty); }
            set { SetValue(ControlDeviceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlDevice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlDeviceProperty =
            DependencyProperty.Register("ControlDevice", typeof(bool), typeof(JSDSToJieLinkViewModel));
        /// <summary>
        /// 设备权限
        /// </summary>

        public bool DeviceRight
        {
            get { return (bool)GetValue(DeviceRightProperty); }
            set { SetValue(DeviceRightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeviceRight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeviceRightProperty =
            DependencyProperty.Register("DeviceRight", typeof(bool), typeof(JSDSToJieLinkViewModel));
    }
}
