using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Utils;
using PartialViewJsdsOneClickUpgradeToJieLink.ViewModels;

namespace PartialViewJsdsOneClickUpgradeToJieLink
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class JsdsOneClickUpgradeToJieLink : UserControl, IPartialView
    {
        public string MenuName
        {
            get { return "改造升级"; }
        }

        public string TagName
        {
            get { return "PartialViewOtherToJieLink"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        JsdsOneClickUpgradeViewModel viewModel;
        private static readonly object continueLocker = new object();

        public JsdsOneClickUpgradeToJieLink()
        {
            InitializeComponent();
            viewModel = JsdsOneClickUpgradeViewModel.Instance();
            DataContext = viewModel;    //上下文赋值

            viewModel.EnableContinueToClickUpgradeButton = true;
            viewModel.UpgradeResult = "说明：本工具用于其他软件一键切换到jielink软件，迁移数据包括【组织、人事、凭证、车场服务、门禁服务、入场记录、出场记录、收费记录】。\r\n欢迎加入jielink阵营!!!\r\n";
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select * from sys_user limit 1");
                this.IsEnabled = true;
            }
            catch (Exception)
            {
                MessageBoxHelper.MessageBoxShowWarning("请先在【设置】菜单中配置数据库连接");
                this.IsEnabled = false;
            }
        }

        public string JsdsIp { get; set; }

        public string JsdsDbConnString { get; private set; }

        /// <summary>
        /// 组织根节点
        /// </summary>

        private readonly string GROUPROOTPARENTID = "00000000-0000-0000-0000-000000000000";

        private readonly string DEFAULTPERSON = "DEFAULTPERSON";

        private readonly string REMARK = "jsds";
        /// <summary>
        /// 迁移这个时间之后的入出场收费记录
        /// </summary>

        private readonly string INTIME = "2020-01-01 ";

        private readonly string PARKNO = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// 数据字典：包含折扣类型
        /// </summary>
        //private List<TSyncDictionary> syncDictionaryList = new List<TSyncDictionary>();
        /// <summary>
        /// 数据字典：包含支付方式
        /// </summary>
        //private List<TSysDictionary> sysDictionaryList = new List<TSysDictionary>();

        private void btnTestConn_Click(object sender, RoutedEventArgs e)
        {
            string connStr = $"Data Source={txtJsdsDbIp.Text};port={txtJsdsDbPort.Text};User ID={txtJsdsDbUser.Text};Password={txtJsdsDbPwd.Password};Initial Catalog={txtJsdsDbName.Text};";

            try
            {
                string cmd = "select * from t_cac_account";
                MySqlHelper.ExecuteDataset(connStr, cmd);
                JsdsDbConnString = connStr;

                viewModel.ShowMessage("JSDS数据库连接成功");
                Notice.Show("JSDS数据库连接成功", "通知", 3);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning("连接错误，请重新输入JSDS数据库连接信息");
            }
        }

        private void btnOneClickUpgrade_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(JsdsDbConnString))
                {
                    viewModel.ShowMessage("请先配置JSDS数据库连接信息");
                    MessageBoxHelper.MessageBoxShowWarning("请先配置JSDS数据库连接信息");
                }
                //判断数据库是否jsds
                string cmd = "select * from t_cac_account";
                MySqlHelper.ExecuteDataset(JsdsDbConnString, cmd);

                lock (continueLocker)
                {
                    if (!viewModel.EnableContinueToClickUpgradeButton)
                    {
                        viewModel.ShowMessage("正在进行升级，请勿重复操作");
                        MessageBoxHelper.MessageBoxShowWarning("正在进行升级，请勿重复操作");
                    }
                    Task.Factory.StartNew(() =>
                    {
                        viewModel.EnableContinueToClickUpgradeButton = false;
                        StartUpgrade();
                    });
                }
            }
            catch (Exception ex)
            {
                viewModel.ShowMessage("一键升级错误，详情请分析日志：" + ex);
                MessageBoxHelper.MessageBoxShowWarning("一键升级错误，详情请分析日志");
            }
            finally
            {

            }
        }

        /// <summary>
        /// 开始升级
        /// </summary>
        private void StartUpgrade()
        {
            try
            {
                //开始进行数据转换
                //1、组织control_role_group
                DataSet dbGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_role_group ORDER BY id ASC;");
                List<ControlRoleGroup> dbGroupList = new List<ControlRoleGroup>();
                ControlRoleGroup groupRoot = null;
                if (dbGroupDs != null && dbGroupDs.Tables[0] != null)
                {
                    dbGroupList = CommonHelper.DataTableToList<ControlRoleGroup>(dbGroupDs.Tables[0]).OrderBy(x => x.ID).ToList();
                    groupRoot = dbGroupList.FirstOrDefault(x => x.ParentId == GROUPROOTPARENTID);
                }
                DataSet groupDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_base_organize WHERE STATE='normal' ORDER BY ORG_ID ASC;");
                if (groupDs != null && groupDs.Tables[0] != null)
                {
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
                                    string sql = string.Format("INSERT INTO control_role_group(RGGUID,RGName,RGCode,ParentId,RGType,`Status`,CreatedOnUtc,Remark,RGFullPath) VALUE('{0}','{1}','{2}','{3}',1,0,'{4}','{5}','{6}');", new Guid(group.ID), group.ORG_NAME, orgCode, GROUPROOTPARENTID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), group.REMARK, group.ORG_NAME + ";");
                                    try
                                    {
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag <= 0)
                                        {
                                            viewModel.ShowMessage("组织根节点插入失败");
                                            MessageBoxHelper.MessageBoxShowWarning("组织根节点插入失败");
                                            return;
                                        }
                                    }
                                    catch (Exception o)
                                    {
                                        viewModel.ShowMessage("组织根节点插入失败：" + o.Message);
                                        MessageBoxHelper.MessageBoxShowWarning("组织根节点插入失败");
                                        return;
                                    }
                                }
                                else
                                {
                                    string sql = string.Format("UPDATE control_role_group SET Remark='{0}' WHERE RGGUID='{1}' AND ParentId='{2}';", group.REMARK, groupRoot.RGGUID, GROUPROOTPARENTID);
                                    try
                                    {
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag <= 0)
                                        {
                                            viewModel.ShowMessage("组织根节点更新失败");
                                            MessageBoxHelper.MessageBoxShowWarning("组织根节点更新失败");
                                            return;
                                        }
                                    }
                                    catch (Exception o)
                                    {
                                        viewModel.ShowMessage("组织根节点更新失败：" + o.Message);
                                        MessageBoxHelper.MessageBoxShowWarning("组织根节点更新失败");
                                        return;
                                    }

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
                                    viewModel.ShowMessage("不可重复一键升级");
                                    MessageBoxHelper.MessageBoxShowWarning("不可重复一键升级");
                                    return;
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
                                    viewModel.ShowMessage("父节点组织不存在，跳过");
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
                                        viewModel.ShowMessage("组织" + group.ORG_NAME + "插入失败");
                                        continue;
                                    }
                                }
                                catch (Exception o)
                                {
                                    viewModel.ShowMessage("组织" + group.ORG_NAME + "插入失败：" + o.Message);
                                    continue;
                                }
                            }
                        }
                    }
                }
                //2、重新查询组织
                dbGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_role_group ORDER BY id ASC;");
                dbGroupList = new List<ControlRoleGroup>();
                groupRoot = null;
                if (dbGroupDs != null && dbGroupDs.Tables[0] != null)
                {
                    dbGroupList = CommonHelper.DataTableToList<ControlRoleGroup>(dbGroupDs.Tables[0]).OrderBy(x => x.ID).ToList();
                    groupRoot = dbGroupList.FirstOrDefault(x => x.ParentId == GROUPROOTPARENTID);
                }
                if (groupRoot == null)
                {
                    viewModel.ShowMessage("组织根节点不存在，请确认数据库是否正确");
                    MessageBoxHelper.MessageBoxShowWarning("组织根节点不存在，请确认数据库是否正确");
                    return;
                }
                //3、人事
                DataSet personDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_base_person WHERE STATE='NORMAL' AND ID in (SELECT PERSON_ID from t_cac_account where `STATUS`='NORMAL' ORDER BY CREATE_DATE ASC) ORDER BY PERSON_SN asc;");
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
                bool isContinue = false;
                if (personList.Count > 0)
                {
                    string idNumber = "311311194910010099";
                    int userType = 2;   //业主
                    int relationship = 0;   //与户主关系：本人
                    int gender = 1; //男                    
                    foreach (TBasePersonModel personModel in personList)
                    {
                        if (personModel.ID.ToUpper().Equals(DEFAULTPERSON))
                        {
                            //默认用户：跳过
                            continue;
                        }
                        string personGuid = new Guid(personModel.ID).ToString();
                        //判断是否已升级
                        DataSet currentPersonDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_person WHERE `Status`=0 AND PGUID='{0}'", personGuid));
                        ControlPerson currentPerson = null;
                        if (currentPersonDs != null && currentPersonDs.Tables[0] != null)
                        {
                            currentPerson = CommonHelper.DataTableToList<ControlPerson>(currentPersonDs.Tables[0]).FirstOrDefault();
                        }
                        if (currentPerson != null)
                        {
                            viewModel.ShowMessage("不可重复一键升级");
                            MessageBoxHelper.MessageBoxShowWarning("不可重复一键升级");
                            return;
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
                                DataSet currentGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_role_group WHERE RGGUID='{0}'", currentGuid));
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
                                    viewModel.ShowMessage(string.Format("用户{0}【{1}】新增失败", personModel.NAME, personModel.ID));
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
                                            viewModel.ShowMessage(string.Format("用户与组织关系{0}【{1}】新增失败", currentGroup.RGGUID, personModel.ID));
                                        }

                                        completeFlag = true;
                                    }
                                    catch (Exception o)
                                    {
                                        viewModel.ShowMessage(string.Format("用户与组织关系{0}【{1}】新增失败：{2}", currentGroup.RGGUID, personModel.ID, o.ToString()));
                                    }
                                }
                            }
                            catch (Exception o)
                            {
                                viewModel.ShowMessage(string.Format("用户{0}【{1}】新增失败：{2}", personModel.NAME, personModel.ID, o.ToString()));
                            }

                            if (completeFlag)
                            {
                                transaction.Complete();

                                isContinue = true;
                            }
                        }
                    }
                }

                List<ControlPerson> jielinkPersonList = new List<ControlPerson>();  //jielink数据库的用户列表
                if (isContinue)
                {
                    List<TCacAccountModel> accountList = new List<TCacAccountModel>();
                    List<TcacVoucherBizParamModel> cacVoucherServiceList = new List<TcacVoucherBizParamModel>();
                    List<TCacVoucherModel> voucherList = new List<TCacVoucherModel>();
                    List<TCacAuthServiceModel> parkServiceList = new List<TCacAuthServiceModel>();
                    List<TCacAuthServiceModel> doorServiceList = new List<TCacAuthServiceModel>();
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
                    DataSet cacVoucherServiceDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * FROM t_cac_voucher_biz_param WHERE ACCOUNT_ID in (SELECT ID from t_cac_account WHERE `STATUS`='NORMAL') ORDER BY CREATE_TIME ASC;");
                    if (cacVoucherServiceDs != null && cacVoucherServiceDs.Tables[0] != null)
                    {
                        cacVoucherServiceList = CommonHelper.DataTableToList<TcacVoucherBizParamModel>(cacVoucherServiceDs.Tables[0]);
                    }
                    DataSet voucherDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "SELECT * from t_cac_voucher WHERE `STATUS`='NORMAL' OR `STATUS`='BLANK' ORDER BY CREATE_TIME ASC;");
                    if (voucherDs != null && voucherDs.Tables[0] != null)
                    {
                        voucherList = CommonHelper.DataTableToList<TCacVoucherModel>(voucherDs.Tables[0]);
                    }
                    DataSet parkServiceDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "select * from t_cac_auth_service WHERE state='NORMAL' and BUSINESS_CODE='PARK' ORDER BY CREATE_TIME DESC;");
                    if (parkServiceDs != null && parkServiceDs.Tables[0] != null)
                    {
                        parkServiceList = CommonHelper.DataTableToList<TCacAuthServiceModel>(parkServiceDs.Tables[0]);
                    }
                    DataSet doorServiceDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, "select * from t_cac_auth_service WHERE state='NORMAL' and BUSINESS_CODE='DOOR' ORDER BY CREATE_TIME DESC;");
                    if (doorServiceDs != null && doorServiceDs.Tables[0] != null)
                    {
                        doorServiceList = CommonHelper.DataTableToList<TCacAuthServiceModel>(doorServiceDs.Tables[0]);
                    }
                    List<TCacVoucherModel> voucherNonServiceList = new List<TCacVoucherModel>(); //不关联车场服务的凭证信息，凭证中的Lguid无需赋值：如纯门禁凭证……     
                    string[] parkServiceGuidList = parkServiceList.Select(x => x.ID).ToArray();
                    List<string> voucherGuidWithServceList = cacVoucherServiceList.Where(x => parkServiceGuidList.Contains(x.AUTH_SERVICE_ID)).Select(x => x.VOUCHER_ID).ToList();
                    if (voucherGuidWithServceList.Count > 0)
                    {
                        voucherNonServiceList = voucherList.Where(x => !voucherGuidWithServceList.Contains(x.ID)).ToList();
                    }
                    else
                    {
                        voucherNonServiceList.AddRange(voucherList);
                    }
                    //4、凭证、门禁服务、车场服务
                    if (accountList.Count > 0)
                    {
                        foreach (TCacAccountModel account in accountList)
                        {
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                bool completeFlag = true;  //事务提交标识
                                List<TCacAuthServiceModel> parkServiceSuccessList = new List<TCacAuthServiceModel>();
                                try
                                {
                                    if (account.PERSON_ID.ToUpper().Equals(DEFAULTPERSON))
                                    {
                                        continue;
                                    }
                                    string personGuid = new Guid(account.PERSON_ID).ToString();
                                    ControlPerson jieLinkPerson = jielinkPersonList.FirstOrDefault(x => x.PGUID == personGuid);
                                    if (jieLinkPerson == null)
                                    {
                                        continue;
                                    }
                                    //插入门禁服务
                                    TCacAuthServiceModel doorService = doorServiceList.FirstOrDefault(x => x.ACCOUNT_ID == account.ID);
                                    if (doorService != null)
                                    {
                                        string startTime = CommonHelper.GetDateTimeValue(doorService.START_TIME, DateTime.Now).ToString("yyyy-MM-dd");
                                        string endTime = CommonHelper.GetDateTimeValue(doorService.END_TIME, DateTime.Now).ToString("yyyy-MM-dd");
                                        string sql = string.Format("INSERT INTO system_set_door_service_to_person(SGUID,PersonNo,StartTime,EndTime,Start1,End1,`Week`,WeekText,CardRightFirst,`Status`,OperNo,OperName,OperTime) VALUE('{0}','{1}','{2}','{3}','00:00','23:59','1111111','星期一,星期二,星期三,星期四,星期五,星期六,星期日',0,0,'9999','超级管理员','{4}')",
                                                Guid.NewGuid().ToString(), jieLinkPerson.PersonNo, startTime, endTime, doorService.CREATE_TIME);
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag > 0)
                                        {
                                            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsDoorService=1 WHERE PGUID='{0}';", personGuid));
                                        }
                                    }
                                    //车场服务+凭证
                                    List<TCacAuthServiceModel> parkServices = parkServiceList.Where(x => x.ACCOUNT_ID == account.ID).ToList();
                                    isContinue = true;
                                    foreach (TCacAuthServiceModel parkService in parkServices)
                                    {
                                        string lguid = new Guid(parkService.ID).ToString();
                                        string startTime = CommonHelper.GetDateTimeValue(parkService.START_TIME, DateTime.Now).ToString("yyyy-MM-dd 00:00:00");
                                        string endTime = CommonHelper.GetDateTimeValue(parkService.END_TIME, DateTime.Now).ToString("yyyy-MM-dd 23:59:59");
                                        string stopServiceTime = CommonHelper.GetDateTimeValue(parkService.END_TIME, DateTime.Now).ToString("yyyy-MM-dd");
                                        string uniqueServiceNo = CommonHelper.GetUniqueId();
                                        string sql = string.Format("INSERT INTO control_lease_stall(LGUID,PGUID,SetmealNo,StartTime,EndTime,OperatorNO,OperatorName,OperateTime,`Status`,PersonName,PersonNo,NisspId,CarNumber,VehiclePosCount,StopServiceTime,UniqueServiceNo,`Timestamp`) VALUE('{0}','{1}',50,'{2}','{3}','9999','超级管理员','{4}',0,'{5}','{6}','{0}','{7}','{7}','{8}','{9}',0)",
                                                lguid, personGuid, startTime, endTime, parkService.CREATE_TIME, jieLinkPerson.PersonName, jieLinkPerson.PersonNo, parkService.PARK_SEAT_NUM, stopServiceTime, uniqueServiceNo);
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag > 0)
                                        {
                                            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsParkService=1 WHERE PGUID='{0}';", personGuid));
                                            //关联了车场服务的凭证
                                            List<TcacVoucherBizParamModel> relations = cacVoucherServiceList.Where(x => x.ACCOUNT_ID == account.ID && x.AUTH_SERVICE_ID == parkService.ID).ToList();
                                            List<string> voucherGuids = relations.Select(x => x.VOUCHER_ID).ToList();
                                            List<TCacVoucherModel> vouchers = voucherList.Where(x => voucherGuids.Contains(x.ID)).ToList();
                                            bool saveFlag = VoucherSave(vouchers, jieLinkPerson, lguid, account.ID);
                                            if (!saveFlag)
                                            {
                                                isContinue = false;
                                                break;
                                            }
                                        }
                                    }
                                    if (isContinue)
                                    {
                                        //纯凭证
                                        List<TCacVoucherModel> vouchersNonService = voucherNonServiceList.Where(x => x.ACCOUNT_ID == account.ID).ToList();
                                        completeFlag = VoucherSave(vouchersNonService, jieLinkPerson, "", account.ID);
                                    }
                                }
                                catch (Exception o)
                                {
                                    completeFlag = false;
                                    viewModel.ShowMessage(string.Format("处理账号【{0}】的凭证服务信息异常：{1}", account.ID, o.ToString()));
                                }
                                if (completeFlag)
                                {
                                    transaction.Complete();

                                    isContinue = true;
                                }
                            }
                        }
                    }
                }

                //两个isContinue不可以合并，因为前一个里面有对isContinue赋值：迁移入出场、收费记录
                if (isContinue)
                {
                    List<TParkRecordInModel> recordInList = new List<TParkRecordInModel>();
                    List<TParkRecordOutModel> recordOutList = new List<TParkRecordOutModel>();
                    List<TParkPayModel> recordBillList = new List<TParkPayModel>();
                    DataSet recordInDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, string.Format("SELECT * FROM t_park_record_in WHERE IN_TIME>='{0}' ORDER BY IN_TIME ASC;", INTIME));
                    if (recordInDs != null && recordInDs.Tables[0] != null && recordInDs.Tables[0].Rows.Count > 0)
                    {
                        recordInList = CommonHelper.DataTableToList<TParkRecordInModel>(recordInDs.Tables[0]);
                        DataSet recordOutDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, string.Format("SELECT * FROM t_park_record_out WHERE IN_TIME>='{0}' ORDER BY IN_TIME ASC;", INTIME));
                        if (recordOutDs != null && recordOutDs.Tables[0] != null && recordOutDs.Tables[0].Rows.Count > 0)
                        {
                            recordOutList = CommonHelper.DataTableToList<TParkRecordOutModel>(recordOutDs.Tables[0]);
                            DataSet recordBillDs = MySqlHelper.ExecuteDataset(JsdsDbConnString, string.Format("SELECT * FROM t_park_pay WHERE IN_TIME>='{0}' ORDER BY IN_TIME ASC;", INTIME));
                            if (recordBillDs != null && recordBillDs.Tables[0] != null && recordBillDs.Tables[0].Rows.Count > 0)
                            {
                                recordBillList = CommonHelper.DataTableToList<TParkPayModel>(recordBillDs.Tables[0]);
                            }
                        }
                    }
                    if (recordInList.Count > 0)
                    {
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
                        foreach (TParkRecordInModel recordIn in recordInList)
                        {
                            try
                            {
                                string plate = recordIn.PHYSICAL_NO;
                                string intime = recordIn.IN_TIME;
                                int sealId = 54;
                                string sealName = "临时用户A";
                                if (!string.IsNullOrWhiteSpace(recordIn.AUTHSERVICE_ID))
                                {
                                    sealId = 50;
                                    sealName = "月租用户A";
                                }
                                string enterDeviceId = "188766208";
                                string enterDeviceName = "虚拟车场入口";
                                string eguid = new Guid(recordIn.ID).ToString();
                                int wasgone = 0;
                                if (recordIn.OUT_FLAG.Equals("OUT"))
                                {
                                    wasgone = 1;
                                }
                                string personNo = "";
                                string personName = "临时车主";
                                if (!string.IsNullOrWhiteSpace(recordIn.PERSON_ID) && !recordIn.PERSON_ID.Equals(DEFAULTPERSON))
                                {
                                    string personGuid = new Guid(recordIn.PERSON_ID).ToString();
                                    ControlPerson person = jielinkPersonList.FirstOrDefault(x => x.PGUID == personGuid);
                                    if (person != null)
                                    {
                                        personNo = person.PersonNo;
                                        personName = person.PersonName;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(recordIn.REMARK))
                                {
                                    recordIn.REMARK = REMARK;
                                }
                                string sql = $"insert into box_enter_record (CredentialType,CredentialNO,Plate,CarNumOrig,EnterTime,SetmealType,SealName,EGuid,EnterRecordID,EnterDeviceID,EnterDeviceName,WasGone,EventType,EventTypeName,ParkNo,OperatorNo,OperatorName,PersonNo,PersonName,Remark,InDeviceEnterType,OptDate) " +
                                    $"VALUES(163,'{plate}','{plate}','{plate}','{intime}',{sealId},'{sealName}','{eguid}','{recordIn.ID}','{enterDeviceId}','{enterDeviceName}','{wasgone}',1,'一般正常记录','{PARKNO}','9999','超级管理员','{personNo}','{personName}','{recordIn.REMARK}',1,'{intime}');";
                                int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                if (result > 0)
                                {
                                    viewModel.ShowMessage($"车牌：{plate}-{recordIn.ID} 补录入场记录成功！");
                                }
                            }
                            catch (Exception o)
                            {
                                viewModel.ShowMessage($"车牌：{recordIn.PHYSICAL_NO}-{recordIn.IN_TIME}-{recordIn.ID} 补录入场记录异常！");
                            }
                        }
                        foreach (TParkPayModel recordBill in recordBillList)
                        {
                            try
                            {
                                string plate = recordBill.PHYSICAL_NO;
                                string intime = recordBill.IN_TIME;
                                int sealId = 54;
                                string sealName = "临时用户A";
                                if (!string.IsNullOrWhiteSpace(recordBill.VOUCHER_ID))
                                {
                                    sealId = 50;
                                    sealName = "月租用户A";
                                }
                                //string enterDeviceId = "188766208";
                                string enterDeviceName = "虚拟车场入口";
                                string outDeviceID = "120201030";
                                string outDeviceName = "虚拟车场出口";
                                string bguid = new Guid(recordBill.ID).ToString();
                                string enterRecordId = recordBill.RECORDIN_ID;
                                string outRecordId = recordBill.RECORDOUT_ID;
                                if (string.IsNullOrWhiteSpace(recordBill.ORDER_NO))
                                {
                                    recordBill.ORDER_NO = recordBill.ID;
                                }
                                string personNo = "";
                                string personName = "临时车主";
                                //找出场记录
                                TParkRecordOutModel recordOut = recordOutList.FirstOrDefault(x => x.ID == outRecordId);
                                if (recordOut != null && !string.IsNullOrWhiteSpace(recordOut.AUTHSERVICE_ID))
                                {
                                    string serviceGuid = new Guid(recordOut.AUTHSERVICE_ID).ToString();
                                    ControlLeaseStall service = parkServiceList.FirstOrDefault(x => x.LGUID == serviceGuid);
                                    if (service != null)
                                    {
                                        ControlPerson person = jielinkPersonList.FirstOrDefault(x => x.PGUID == service.PGUID);
                                        if (person != null)
                                        {
                                            personNo = person.PersonNo;
                                            personName = person.PersonName;
                                        }
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(recordBill.REMARK))
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
                                string sql = $"INSERT INTO box_bill(BGUID, CredentialType, CredentialNO, Plate, OrderId, PayTime, FeesTime, CreateTime, InTime, EnterRecordID, Money, Fees, Benefit, Derate, AccountReceivable, Paid, ActualPaid, Exchange, FreeMoney, PayTypeID, PayTypeName, ChargeType, ChargeDeviceID,ChargeDeviceName, OperatorID, OperatorName,Cashier,CashierName, OrderType,`Status`, SealTypeId, SealTypeName, Remark, ReplaceDeduct, EventType, PayFrom, DeviceID, PersonNo, PersonName, ParkNo) " +
                                    $"VALUE('{bguid}', 163, '{plate}', '{plate}', '{recordBill.ORDER_NO}', '{recordBill.PAY_TIME}', '{recordBill.PAY_TIME}', '{recordBill.CREATE_TIME}', '{recordBill.IN_TIME}', '{enterRecordId}', {money}, {fees}, {benefit}, {derate}, {accountReceivable}, {paid}, {actualPaid}, {exchange},{freeMoney}, {payType}, '{payTypeName}', {chargeType}, '{outDeviceID}','{outDeviceName}', '9999', '超级管理员', '9999', '超级管理员', {orderType}, {status}, {sealId}, '{sealName}', '{REMARK}', {replaceDeduct}, 1, '{payFrom}', '{outDeviceID}', '{personNo}', '{personName}', '{PARKNO}');";
                                int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                if (result > 0)
                                {
                                    viewModel.ShowMessage($"车牌：{plate}-{recordBill.ID} 补录收费记录成功！");
                                }
                            }
                            catch (Exception o)
                            {
                                viewModel.ShowMessage($"车牌：{recordBill.PHYSICAL_NO}-{recordBill.PAY_TIME}-{recordBill.ID} 补录收费记录异常！");
                            }
                        }
                        foreach (TParkRecordOutModel recordOut in recordOutList)
                        {
                            try
                            {
                                string plate = recordOut.PHYSICAL_NO;
                                string intime = recordOut.IN_TIME;
                                int sealId = 54;
                                string sealName = "临时用户A";
                                if (!string.IsNullOrWhiteSpace(recordOut.AUTHSERVICE_ID))
                                {
                                    sealId = 50;
                                    sealName = "月租用户A";
                                }
                                //string enterDeviceId = "188766208";
                                string enterDeviceName = "虚拟车场入口";
                                string outDeviceID = "120201030";
                                string outDeviceName = "虚拟车场出口";
                                string oguid = new Guid(recordOut.ID).ToString();
                                string outRecordId = recordOut.ID;
                                string enterRecordId = recordOut.RECORDIN_ID;
                                string personNo = "";
                                string personName = "临时车主";
                                if (!string.IsNullOrWhiteSpace(recordOut.AUTHSERVICE_ID))
                                {
                                    string serviceGuid = new Guid(recordOut.AUTHSERVICE_ID).ToString();
                                    ControlLeaseStall service = parkServiceList.FirstOrDefault(x => x.LGUID == serviceGuid);
                                    if (service != null)
                                    {
                                        ControlPerson person = jielinkPersonList.FirstOrDefault(x => x.PGUID == service.PGUID);
                                        if (person != null)
                                        {
                                            personNo = person.PersonNo;
                                            personName = person.PersonName;
                                        }
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(recordOut.REMARK))
                                {
                                    recordOut.REMARK = REMARK;
                                }
                                int payType = 21;   //其他，通过收费记录赋值    
                                string payTypeName = "其它";
                                TParkPayModel recordBill = recordBillList.FirstOrDefault(x => x.RECORDOUT_ID == outRecordId);
                                if (recordBill != null)
                                {
                                    GetPayType(recordBill.PAY_TYPE, out payType, out payTypeName);
                                }
                                //AccountReceivable,Charging,HgMoney,YhMoney,FreeMoney
                                decimal accountReceivable = recordOut.TOTAL_CHARGE_ACTUAL;  //实收金额
                                decimal charging = recordOut.TOTAL_CHARGE_AVAILABLE;   //应收金额
                                decimal hgMoney = 0;    //回滚金额
                                decimal yhMoney = recordOut.TOTAL_CHARGE_REFUND;    //优惠金额
                                decimal freeMoney = 0;  //免费金额
                                string sql = $"INSERT INTO box_out_record(OGUID,OutRecordID,OutDeviceID,OutDeviceName,EnterRecordID,InDeviceName,InTime,CredentialType,CredentialNO,Plate,CarNumOrig,OperatorNo,OperatorName,OutTime,OptTime,PersonNo,PersonName,SetmealType,SetmealName,Remark,PayType,AccountReceivable,Charging,HgMoney,YhMoney,FreeMoney,IoType,ExtendFlag,EventType,EventTypeName,ParkNo) " +
                                    $"VALUE('{oguid}','{outRecordId}','{outDeviceID}','{outDeviceName}','{enterRecordId}','{enterDeviceName}','{recordOut.IN_TIME}',163,'{plate}','{plate}','{plate}','9999','超级管理员','{recordOut.OUT_TIME}','{recordOut.OUT_TIME}','{personNo}','{personName}',{sealId},'{sealName}','{recordOut.REMARK}','{payType}',{accountReceivable},{charging},{hgMoney},{yhMoney},{freeMoney},2,1,1,'一般正常记录','{PARKNO}');";
                                int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                if (result > 0)
                                {
                                    viewModel.ShowMessage($"车牌：{plate}-{recordOut.ID} 补录出场记录成功！");
                                }
                            }
                            catch (Exception o)
                            {
                                viewModel.ShowMessage($"车牌：{recordOut.PHYSICAL_NO}-{recordOut.OUT_TIME}-{recordOut.ID} 补录出场记录异常！");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                viewModel.EnableContinueToClickUpgradeButton = true;
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
        private bool VoucherSave(List<TCacVoucherModel> voucherList, ControlPerson jieLinkPerson, string lguid, string accountID)
        {
            bool completeFlag = true;
            foreach (TCacVoucherModel voucher in voucherList)
            {
                try
                {
                    string voucherGuid = new Guid(voucher.ID).ToString();
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
                    string sql = string.Format("INSERT INTO control_voucher(Guid,PGUID,LGUID,PersonNo,PersonName,VoucherType,VoucherNo,CardNum,PhysicsNum,AddOperatorNo,AddTime,`Status`,LastTime,Remark,StatusFromPerson) VALUE('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{6}','{7}','9999','{8}',1,'{8}','{9}',1);",
                             voucherGuid, jieLinkPerson.PGUID, lguid, jieLinkPerson.PersonNo, jieLinkPerson.PersonName, voucherType, voucher.PHYSICAL_NO, physicsNum, voucher.CREATE_TIME, remark);
                    int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                    if (flag > 0)
                    {
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
                    completeFlag = false;
                    viewModel.ShowMessage(string.Format("处理账号【{0}】的凭证【{1}】信息异常：{1}", accountID, voucher.ID, o1.ToString()));
                    break;
                }
            }
            return completeFlag;
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
    }
}