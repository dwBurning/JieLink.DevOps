using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
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
    public class JSOCT2017ToJieLinkViewModel : DependencyObject
    {
        private readonly string GROUPROOTPARENTID = "00000000-0000-0000-0000-000000000000";

        private readonly string REMARK = "JSOCT2017";

        private readonly string PARKNO = "00000000-0000-0000-0000-000000000000";

        public DelegateCommand TestConnCommand { get; set; }

        public DelegateCommand UpgradeCommand { get; set; }

        public bool CanExecute { get; set; }

        public DbConnectionModel conn { get; set; }

        public JSOCT2017ToJieLinkViewModel()
        {
            TestConnCommand = new DelegateCommand();
            TestConnCommand.ExecuteAction = TestConn;

            UpgradeCommand = new DelegateCommand();
            UpgradeCommand.ExecuteAction = Upgrade;
            UpgradeCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return CanExecute; });
        }

        private void TestConn(object parameter)
        {
            DbConnectionModel dbConnectionModel = this.GetDbConnectionModel();

            try
            {
                PasswordBox passwordBox = (PasswordBox)parameter;
                this.Password = passwordBox.Password;

                if (!CheckValid())
                {
                    MessageBoxHelper.MessageBoxShowWarning("请填写完整的数据库配置信息！");
                    return;
                }

                DbConnectionModel conn = this.GetDbConnectionModel();
                if (MSSqlHelper.TestDbConnection(dbConnectionModel.ConnectionStr))
                {
                    CanExecute = true;
                    ShowMessage("数据库连接成功");
                    Notice.Show("数据库连接成功", "通知", 3, MessageBoxIcon.Success);
                }
                else
                {
                    CanExecute = false;
                    MessageBoxHelper.MessageBoxShowWarning("连接错误，请重新输入数据库连接信息");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning("连接错误，请重新输入数据库连接信息");
            }
        }

        private bool CheckValid()
        {
            if (string.IsNullOrEmpty(IpAddr)
                || string.IsNullOrEmpty(Password)
                || string.IsNullOrEmpty(UserName)
                || string.IsNullOrEmpty(DbName))
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
                Task.Factory.StartNew(() =>
                {
                    CanExecute = false;
                    StartUpgradeG3();
                    CanExecute = true;
                });
            }
            catch (Exception ex)
            {
                ShowMessage("一键升级错误，详情请分析日志：" + ex);
                MessageBoxHelper.MessageBoxShowWarning("一键升级错误，详情请分析日志");
            }
            finally
            {

            }
        }


        private void StartUpgradeG3()
        {
            try
            {
                List<TBaseDeptModel> G3GroupList = null;
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
                DataTable groupDt = MSSqlHelper.ExecuteQuery("SELECT * from hr.dept WHERE deleteFlag = '0' ORDER BY id ASC;", null);
                if (groupDt != null)
                {
                    List<TBaseDeptModel> tempGroupList = CommonHelper.DataTableToList<TBaseDeptModel>(groupDt).OrderBy(x => x.PARENTID).ToList();

                    List<TBaseDeptModel> groupList = GetHrDeptChildren(tempGroupList, "0");
                    G3GroupList = groupList;
                    if (groupList.Count > 0)
                    {
                        //存储jsds的根节点GUID：保持根节点原先的RGGUID不变，处理jsds根节点下的子组织时，将ORG_ID与jsdsRootId比较
                        //因为jielink中组织guid涉及的表结构很多，因此组织根节点的guid保持不动
                        string jsdsRootId = "0";
                        foreach (TBaseDeptModel group in groupList)
                        {
                            if (string.IsNullOrWhiteSpace(group.REMARK))
                            {
                                group.REMARK = REMARK;
                            }
                            if (group.PARENTID == "0")
                            {
                                jsdsRootId = group.ID;
                                //根组织节点
                                if (groupRoot == null)
                                {
                                    string orgCode = new Random().ToString().Substring(2, 8);
                                    string sql = string.Format("INSERT INTO control_role_group(RGGUID,RGName,RGCode,ParentId,RGType,`Status`,CreatedOnUtc,Remark,RGFullPath) VALUE('{0}','{1}','{2}','{3}',1,0,'{4}','{5}','{6}');", new Guid(group.GUID).ToString(), group.NAME, orgCode, GROUPROOTPARENTID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), group.REMARK, group.NAME + ";");
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
                                        ShowMessage("组织根节点插入失败：" + o.Message);
                                        MessageBoxHelper.MessageBoxShowWarning("组织根节点插入失败");
                                        return;
                                    }
                                }
                                else
                                {
                                    string sql = string.Format("UPDATE control_role_group SET Remark='{0}'  WHERE RGGUID='{1}' AND ParentId='{2}';", group.REMARK, groupRoot.RGGUID, GROUPROOTPARENTID);
                                    try
                                    {
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag <= 0)
                                        {
                                            ShowMessage("组织根节点更新失败");
                                            MessageBoxHelper.MessageBoxShowWarning("组织根节点更新失败");
                                            return;
                                        }
                                    }
                                    catch (Exception o)
                                    {
                                        ShowMessage("组织根节点更新失败：" + o.Message);
                                        MessageBoxHelper.MessageBoxShowWarning("组织根节点更新失败");
                                        return;
                                    }

                                }

                            }
                            else
                            {
                                string currentGuid = new Guid(group.GUID).ToString();
                                //寻找当前组织是否已存在：判断是否重复升级
                                DataSet currentGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_role_group WHERE `Status`=0 AND RGGUID='{0}'", currentGuid));
                                ControlRoleGroup currentGroup = null;
                                if (currentGroupDs != null && currentGroupDs.Tables[0] != null)
                                {
                                    currentGroup = CommonHelper.DataTableToList<ControlRoleGroup>(currentGroupDs.Tables[0]).FirstOrDefault();
                                }
                                if (currentGroup != null)
                                {
                                    ShowMessage("不可重复一键升级");
                                    MessageBoxHelper.MessageBoxShowWarning("不可重复一键升级");
                                    return;
                                }

                                //找上一节点组织：因为要对全路径赋值
                                DataSet parentGroupDs = new DataSet();
                                if (group.PARENTID == jsdsRootId)
                                {
                                    parentGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_role_group WHERE `Status`=0 AND RGGUID='{0}'", new Guid(groupRoot.RGGUID).ToString()));
                                }
                                else
                                {
                                    parentGroupDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_role_group WHERE `Status`=0 AND RGGUID='{0}'", new Guid(groupList.FirstOrDefault(e => e.ID == group.PARENTID).GUID)));
                                }
                                ControlRoleGroup parentGroup = null;

                                if (parentGroupDs != null && parentGroupDs.Tables[0] != null)
                                {
                                    parentGroup = CommonHelper.DataTableToList<ControlRoleGroup>(parentGroupDs.Tables[0]).FirstOrDefault();
                                }
                                if (parentGroup == null)
                                {
                                    ShowMessage("父节点组织不存在，跳过");
                                    continue;
                                }
                                string rgFullPath = string.Format("{0}|{1};", parentGroup.RGFullPath.Trim(';'), group.NAME);
                                //其他子组织
                                string orgCode = new Random().ToString().Substring(2, 8);
                                string sql = string.Format("INSERT INTO control_role_group(RGGUID,RGName,RGCode,ParentId,RGType,`Status`,CreatedOnUtc,Remark,RGFullPath) VALUE('{0}','{1}','{2}','{3}',1,0,'{4}','{5}','{6}');", currentGuid, group.NAME, orgCode, parentGroup.RGGUID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), group.REMARK, rgFullPath);
                                try
                                {
                                    int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                    if (flag <= 0)
                                    {
                                        ShowMessage("组织" + group.NAME + "插入失败");
                                        continue;
                                    }
                                }
                                catch (Exception o)
                                {
                                    ShowMessage("组织" + group.NAME + "插入失败：" + o.Message);
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
                    ShowMessage("组织根节点不存在，请确认数据库是否正确");
                    MessageBoxHelper.MessageBoxShowWarning("组织根节点不存在，请确认数据库是否正确");
                    return;
                }

                //3、人事
                DataTable personDts = MSSqlHelper.ExecuteQuery("SELECT * from hr.person where deleteflag = 0  ORDER BY id asc;", null);
                List<TBaseHrPersonModel> personList = new List<TBaseHrPersonModel>();

                if (personDts != null)
                {
                    personList = CommonHelper.DataTableToList<TBaseHrPersonModel>(personDts).OrderBy(x => x.ID).ToList();
                }
                bool isContinue = false;
                if (personList.Count > 0)
                {
                    string idNumber = "311311194910010099";
                    int userType = 2;   //业主
                    int relationship = 0;   //与户主关系：本人
                    foreach (TBaseHrPersonModel personModel in personList)
                    {
                        string personGuid = new Guid(personModel.GUID).ToString();
                        //判断是否已升级
                        DataSet currentPersonDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, string.Format("SELECT * from control_person WHERE `Status`=0 AND PGUID='{0}'", personGuid));
                        ControlPerson currentPerson = null;
                        if (currentPersonDs != null && currentPersonDs.Tables[0] != null)
                        {
                            currentPerson = CommonHelper.DataTableToList<ControlPerson>(currentPersonDs.Tables[0]).FirstOrDefault();
                        }
                        if (currentPerson != null)
                        {
                            ShowMessage("不可重复一键升级");
                            MessageBoxHelper.MessageBoxShowWarning("不可重复一键升级");
                            return;
                        }
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            ControlRoleGroup currentGroup = null;
                            string currentDeptId = personModel.DeptId;
                            string currentGuid = new Guid((G3GroupList.FirstOrDefault(e => e.ID == currentDeptId)).GUID).ToString();
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

                            if (string.IsNullOrWhiteSpace(personModel.Mobile) || personModel.Mobile.Length <= 7)
                            {
                                //虚构
                                personModel.Mobile = "189" + new Random().Next(10000000, 99999999).ToString().PadLeft(8, '0');
                            }
                            if (string.IsNullOrWhiteSpace(personModel.Remark))
                            {
                                personModel.Remark = REMARK;
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
                            string sql = string.Format("INSERT INTO control_person(PGUID,PersonNo,PersonName,Gender,Mobile,Email,Relationship,RID,EnterTime,Type,`Status`,Remark,CreateTime,CurKey,LastKey,RFullPath,PersonId,CanInOut,IsTKService,IsParkService,IsDoorService,IsIssueCard,IDNumber) VALUE('{0}','{1}','{2}',{3},'{4}','{5}',{6},'{7}','{8}',{9},0,'{10}','{11}','{12}','{13}','{14}','{15}',0,0,0,0,0,'{16}')",
                              personGuid, personModel.NO, personModel.NAME, personModel.SEX, personModel.Mobile, personModel.Email, relationship, currentGroup.RGGUID,
                              personModel.OptDate, userType, personModel.Remark, personModel.OptDate, personModel.NewKeyCode, personModel.OldKeyCode, currentGroup.RGFullPath, personId, idNumber);
                            try
                            {
                                int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                if (flag <= 0)
                                {
                                    ShowMessage(string.Format("用户{0}【{1}】新增失败", personModel.NAME, personModel.ID));
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
                                            ShowMessage(string.Format("用户与组织关系{0}【{1}】新增失败", currentGroup.RGGUID, personModel.ID));
                                        }

                                        completeFlag = true;
                                    }
                                    catch (Exception o)
                                    {
                                        ShowMessage(string.Format("用户与组织关系{0}【{1}】新增失败：{2}", currentGroup.RGGUID, personModel.ID, o.ToString()));
                                    }
                                }
                            }
                            catch (Exception o)
                            {
                                ShowMessage(string.Format("用户{0}【{1}】新增失败：{2}", personModel.NAME, personModel.ID, o.ToString()));
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
                DataSet jielinkPersonDs = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "SELECT * from control_person WHERE `Status`=0 ORDER BY id ASC;");
                if (jielinkPersonDs != null && jielinkPersonDs.Tables[0] != null)
                {
                    jielinkPersonList = CommonHelper.DataTableToList<ControlPerson>(jielinkPersonDs.Tables[0]);
                }
                if (isContinue)
                {
                    DataTable mcCardInfoDts = MSSqlHelper.ExecuteQuery("SELECT * from mc.cardinfo where status = 1 and isplan = 0 ORDER BY id asc;", null);
                    List<TBaseMcCardInfoModel> mcCardInfoList = new List<TBaseMcCardInfoModel>();
                    if (mcCardInfoDts != null)
                    {
                        mcCardInfoList = CommonHelper.DataTableToList<TBaseMcCardInfoModel>(mcCardInfoDts).OrderBy(x => x.ID).ToList();
                    }
                    DataTable mjDaKqCardDts = MSSqlHelper.ExecuteQuery("select * from mj.da_kq_card", null);
                    List<TBaseMjDaKqCard> mjCardList = new List<TBaseMjDaKqCard>();
                    if (mjDaKqCardDts != null)
                    {
                        mjCardList = CommonHelper.DataTableToList<TBaseMjDaKqCard>(mjDaKqCardDts).OrderBy(x => x.Mc_Cardinfo_Id).ToList();
                    }
                    DataTable tcCardDts = MSSqlHelper.ExecuteQuery("select * from tc.cardinfo", null);
                    List<TBaseTcCardInfoModel> tcCardList = new List<TBaseTcCardInfoModel>();
                    if (mjDaKqCardDts != null)
                    {
                        tcCardList = CommonHelper.DataTableToList<TBaseTcCardInfoModel>(tcCardDts).OrderBy(x => x.IssueID).ToList();
                    }

                    DataTable hrRoomPosDts = MSSqlHelper.ExecuteQuery("select * from hr.RoomPos", null);
                    List<TBaseHrRoomPos> hrRoomPosList = new List<TBaseHrRoomPos>();
                    if (hrRoomPosDts != null)
                    {
                        hrRoomPosList = CommonHelper.DataTableToList<TBaseHrRoomPos>(hrRoomPosDts).OrderBy(x => x.ParsonId).ToList();
                    }
                    //门禁、车场服务
                    if (mcCardInfoList.Count > 0)
                    {
                        if (mjCardList.Count > 0)
                        {
                            foreach (TBaseMjDaKqCard mjCardInfo in mjCardList)
                            {
                                using (TransactionScope transaction = new TransactionScope())
                                {
                                    bool completeFlag = true;  //事务提交标识
                                    try
                                    {
                                        TBaseMcCardInfoModel mcCardinfo = mcCardInfoList.FirstOrDefault(e => e.ID == mjCardInfo.Mc_Cardinfo_Id);
                                        if (mcCardinfo == null)
                                        {
                                            continue;
                                        }
                                        TBaseHrPersonModel hrPerson = personList.FirstOrDefault(e => e.ID == mcCardinfo.PersonID);
                                        if (hrPerson == null)
                                        {
                                            continue;
                                        }
                                        string startTime = CommonHelper.GetDateTimeValue(mjCardInfo.Start_Date, DateTime.Now).ToString("yyyy-MM-dd");
                                        string endTime = CommonHelper.GetDateTimeValue(mjCardInfo.End_Date, DateTime.Now).ToString("yyyy-MM-dd");
                                        string sql = string.Format("INSERT INTO system_set_door_service_to_person(SGUID,PersonNo,StartTime,EndTime,Start1,End1,`Week`,WeekText,CardRightFirst,`Status`,OperNo,OperName,OperTime) VALUE('{0}','{1}','{2}','{3}','00:00','23:59','1111111','星期一,星期二,星期三,星期四,星期五,星期六,星期日',0,0,'9999','超级管理员','{4}')",
                                                    Guid.NewGuid().ToString(), hrPerson.NO, startTime, endTime, DateTime.Now);
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag > 0)
                                        {
                                            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsDoorService=1 WHERE PGUID='{0}';", hrPerson.GUID));
                                        }
                                    }
                                    catch (Exception o)
                                    {
                                        completeFlag = false;
                                        ShowMessage(string.Format("处理MC_Card_Id【{0}】的门禁凭证服务信息异常：{1}", mjCardInfo.Mc_Cardinfo_Id, o.ToString()));
                                    }
                                    if (completeFlag)
                                    {
                                        transaction.Complete();

                                        isContinue = true;
                                    }


                                }
                            }
                        }

                        if (tcCardList.Count > 0)
                        {
                            List<TBaseTcCardInfoModel> remove = new List<TBaseTcCardInfoModel>();
                            foreach (TBaseTcCardInfoModel tcCardInfo in tcCardList)
                            {
                                if (remove.Contains(tcCardInfo))
                                {
                                    continue;
                                }
                                using (TransactionScope transaction = new TransactionScope())
                                {
                                    bool completeFlag = true;  //事务提交标识
                                    try
                                    {
                                        TBaseMcCardInfoModel mcCardinfo = mcCardInfoList.FirstOrDefault(e => e.ID == tcCardInfo.IssueID);
                                        if (mcCardinfo == null)
                                        {
                                            continue;
                                        }
                                        TBaseHrPersonModel hrPerson = personList.FirstOrDefault(e => e.ID == mcCardinfo.PersonID);
                                        if (hrPerson == null)
                                        {
                                            continue;
                                        }
                                        TBaseHrRoomPos hrRoomPos = hrRoomPosList.FirstOrDefault(e => e.ParsonId == hrPerson.ID);
                                        if (hrRoomPos == null || hrRoomPos.MaxPos <= 0)
                                        {
                                            string lguid = new Guid(mcCardinfo.GUID).ToString();
                                            string startTime = CommonHelper.GetDateTimeValue(tcCardInfo.StartDate, DateTime.Now).ToString("yyyy-MM-dd 00:00:00");
                                            string endTime = CommonHelper.GetDateTimeValue(tcCardInfo.EndDate, DateTime.Now).ToString("yyyy-MM-dd 23:59:59");
                                            string stopServiceTime = CommonHelper.GetDateTimeValue(tcCardInfo.EndDate, DateTime.Now).ToString("yyyy-MM-dd");
                                            string uniqueServiceNo = CommonHelper.GetUniqueId();

                                            string sql = string.Format("INSERT INTO control_lease_stall(LGUID,PGUID,SetmealNo,StartTime,EndTime,OperatorNO,OperatorName,OperateTime,`Status`,PersonName,PersonNo,NisspId,CarNumber,VehiclePosCount,StopServiceTime,UniqueServiceNo,`Timestamp`) VALUE('{0}','{1}',50,'{2}','{3}','9999','超级管理员','{4}',0,'{5}','{6}','{0}','{7}','{7}','{8}','{9}',0)",
                                                        lguid, new Guid(hrPerson.GUID).ToString(), startTime, endTime, DateTime.Now, hrPerson.NAME, hrPerson.NO, 1, stopServiceTime, uniqueServiceNo);
                                            int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                            if (flag > 0)
                                            {
                                                MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsParkService=1 WHERE PGUID='{0}';", new Guid(hrPerson.GUID).ToString()));
                                                bool saveFlag = VoucherSaveG3(hrPerson, mcCardinfo, tcCardInfo);
                                                if (!saveFlag)
                                                {
                                                    isContinue = false;
                                                    break;
                                                }

                                            }
                                        }
                                        //多位多车，同卡类置为一个服务，去时间最长的；不同卡类分开服务
                                        else
                                        {
                                            //获取这个人的所有卡的信息
                                            List<TBaseMcCardInfoModel> mcCardInfoList1 = mcCardInfoList.Where(e => e.PersonID == hrPerson.ID).Distinct().ToList();
                                            List<TBaseTcCardInfoModel> tcCardinfoList1 = new List<TBaseTcCardInfoModel>();
                                            //获取车场卡数据
                                            foreach (TBaseMcCardInfoModel mcCard in mcCardInfoList1)
                                            {
                                                TBaseTcCardInfoModel tcCard = tcCardList.FirstOrDefault(e => e.IssueID == mcCard.ID);
                                                if (tcCard != null)
                                                {
                                                    tcCardinfoList1.Add(tcCard);
                                                }
                                            }
                                            if (tcCardinfoList1.Count > 0)
                                            {
                                                //根据卡类分组
                                                var tcCardMap = tcCardinfoList1.GroupBy(e => e.CardTypeID).ToList();
                                                foreach (var item in tcCardMap)
                                                {
                                                    TBaseTcCardInfoModel tcCardEndDateMax = item.OrderByDescending(e => e.EndDate).FirstOrDefault();

                                                    string lguid = new Guid(mcCardinfo.GUID).ToString();
                                                    string startTime = CommonHelper.GetDateTimeValue(tcCardEndDateMax.StartDate, DateTime.Now).ToString("yyyy-MM-dd 00:00:00");
                                                    string endTime = CommonHelper.GetDateTimeValue(tcCardEndDateMax.EndDate, DateTime.Now).ToString("yyyy-MM-dd 23:59:59");
                                                    string stopServiceTime = CommonHelper.GetDateTimeValue(tcCardEndDateMax.EndDate, DateTime.Now).ToString("yyyy-MM-dd");
                                                    string uniqueServiceNo = CommonHelper.GetUniqueId();

                                                    string sql = string.Format("INSERT INTO control_lease_stall(LGUID,PGUID,SetmealNo,StartTime,EndTime,OperatorNO,OperatorName,OperateTime,`Status`,PersonName,PersonNo,NisspId,CarNumber,VehiclePosCount,StopServiceTime,UniqueServiceNo,`Timestamp`) VALUE('{0}','{1}',50,'{2}','{3}','9999','超级管理员','{4}',0,'{5}','{6}','{0}','{7}','{7}','{8}','{9}',0)",
                                                                lguid, new Guid(hrPerson.GUID).ToString(), startTime, endTime, DateTime.Now, hrPerson.NAME, hrPerson.NO, hrRoomPos.MaxPos, stopServiceTime, uniqueServiceNo);
                                                    int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                                    if (flag > 0)
                                                    {
                                                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsParkService=1 WHERE PGUID='{0}';", new Guid(hrPerson.GUID).ToString()));


                                                    }
                                                    //移除已经处理的车场卡信息
                                                    foreach (var temp in item)
                                                    {
                                                        bool saveFlag = VoucherSaveG3(hrPerson, mcCardinfo, temp);
                                                        if (!saveFlag)
                                                        {
                                                            isContinue = false;
                                                            break;
                                                        }
                                                        remove.Add(temp);
                                                    }

                                                }
                                            }
                                        }

                                    }
                                    catch (Exception o)
                                    {
                                        completeFlag = false;
                                        ShowMessage(string.Format("处理MC_Card_Id【{0}】的车场凭证服务信息异常：{1}", tcCardInfo.IssueID, o.ToString()));
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
                }

                //入场记录
                if (isContinue)
                {
                    List<TBaseTcRecordModel> recordList = null;
                    DataTable recordDts = MSSqlHelper.ExecuteQuery("select * from tc.record where outid = 0;", null);
                    if (recordDts != null)
                    {
                        recordList = CommonHelper.DataTableToList<TBaseTcRecordModel>(recordDts).OrderBy(x => x.InId).ToList();
                    }
                    foreach (TBaseTcRecordModel enterRecord in recordList)
                    {
                        try
                        {
                            TBaseHrPersonModel personinfo = null;
                            //获取用户
                            if (enterRecord.PersonId > 1)
                            {
                                personinfo = personList.FirstOrDefault(e => e.ID == enterRecord.PersonId);
                            }

                            string plate = enterRecord.CarNo.Replace("-", "");
                            string intime = enterRecord.Intime.ToString();
                            int sealId = 54;
                            string sealName = "临时用户A";
                            //默认卡类ID大于4的都为月卡(临时卡E、F之类也会设置为月卡)
                            if (enterRecord.CardTypeId > 4)
                            {
                                sealId = 50;
                                sealName = "月租用户A";
                            }
                            string enterDeviceId = "188766208";
                            string enterDeviceName = "虚拟车场入口";
                            string eguid = new Guid(enterRecord.GUID).ToString();
                            int wasgone = 0;
                            if (enterRecord.OutId == 0)
                            {
                                wasgone = 0;
                            }
                            string personNo = "";
                            string personName = "临时车主";
                            if (personinfo != null)
                            {
                                string personGuid = new Guid(personinfo.GUID).ToString();
                                ControlPerson person = jielinkPersonList.FirstOrDefault(x => x.PGUID == personGuid);
                                if (person != null)
                                {
                                    personNo = person.PersonNo;
                                    personName = person.PersonName;
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(enterRecord.Remark))
                            {
                                enterRecord.Remark = REMARK;
                            }
                            string sql = $"insert into box_enter_record (CredentialType,CredentialNO,Plate,CarNumOrig,EnterTime,SetmealType,SealName,EGuid,EnterRecordID,EnterDeviceID,EnterDeviceName,WasGone,EventType,EventTypeName,ParkNo,OperatorNo,OperatorName,PersonNo,PersonName,Remark,InDeviceEnterType,OptDate) " +
                                $"VALUES(163,'{plate}','{plate}','{plate}','{intime}',{sealId},'{sealName}','{eguid}','{enterRecord.GUID}','{enterDeviceId}','{enterDeviceName}','{wasgone}',1,'一般正常记录','{PARKNO}','9999','超级管理员','{personNo}','{personName}','{enterRecord.Remark}',1,'{intime}');";
                            int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                            if (result > 0)
                            {
                                ShowMessage($"车牌：{plate}-{enterRecord.InId} 补录入场记录成功！");
                            }

                        }
                        catch (Exception o)
                        {
                            ShowMessage($"车牌：{enterRecord.CarNo} 补录收费记录异常！");
                        }

                    }

                }
                ShowMessage("升级完成！");
                MessageBoxHelper.MessageBoxShowWarning("升级完成！");
            }
            catch (Exception ex)
            {
            }
            finally
            {

            }
        }

        /// <summary>
        /// 递归获取G3组织列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<TBaseDeptModel> GetHrDeptChildren(List<TBaseDeptModel> list, string parentId)
        {
            var result = new List<TBaseDeptModel>();
            var deptList = new List<TBaseDeptModel>();
            if (parentId == "0")
            {
                deptList = list.Where(e => e.PARENTID == "0").ToList();
            }
            else
            {
                deptList = list.Where(e => e.PARENTID == parentId).ToList();
            }
            if (deptList != null)
            {
                result.AddRange(deptList);
                foreach (var dept in deptList)
                {
                    result.AddRange(GetHrDeptChildren(list, dept.ID));
                }
            }

            return result;
        }

        private bool VoucherSaveG3(TBaseHrPersonModel hrPerson, TBaseMcCardInfoModel mcCardinfo, TBaseTcCardInfoModel tcCardInfoModel)
        {
            bool completeFlag = true;
            try
            {
                string voucherGuid = Guid.NewGuid().ToString();
                int voucherType = 163;
                string physicsNum = string.Empty;
                if (mcCardinfo.PhysicalCardType == 55)
                {
                    voucherType = 55;
                    physicsNum = mcCardinfo.IDNO;
                }
                string remark = REMARK;
                string sql = string.Format("INSERT INTO control_voucher(Guid,PGUID,LGUID,PersonNo,PersonName,VoucherType,VoucherNo,CardNum,PhysicsNum,AddOperatorNo,AddTime,`Status`,LastTime,Remark,StatusFromPerson) VALUE('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{6}','{7}','9999','{8}',1,'{8}','{9}',1);",
                          voucherGuid, new Guid(hrPerson.GUID).ToString(), new Guid(mcCardinfo.GUID).ToString(), hrPerson.NO, hrPerson.NAME, voucherType, tcCardInfoModel.CarNO.Replace("-", ""), physicsNum, mcCardinfo.OptDate, remark);
                int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                if (flag > 0)
                {
                    MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsIssueCard=1 WHERE PGUID='{0}';", new Guid(hrPerson.GUID).ToString()));

                    if (voucherType == 163)
                    {
                        sql = string.Format("INSERT INTO control_vehicle_info(VGUID,PGUID,Plate,VehicleType,`Status`,PlateColor) VALUE('{0}','{1}','{2}',0,1,3);",
                            Guid.NewGuid().ToString(), new Guid(hrPerson.GUID).ToString(), tcCardInfoModel.CarNO.Replace("-", ""));
                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);

                        sql = string.Format("INSERT INTO control_voucher_issue(GUID,VoucherGuid,VoucherType,VoucherNo,PGUID,PersonNo,OperatorNo,AddTime,IsBACKED) VALUE('{0}','{1}',{2},'{3}','{4}','{5}','9999','{6}',0);",
                           Guid.NewGuid().ToString(), voucherGuid, voucherType, tcCardInfoModel.CarNO.Replace("-", ""), new Guid(hrPerson.GUID).ToString(), hrPerson.NO, mcCardinfo.OptDate);
                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);

                        sql = string.Format("INSERT into control_voucher_record(id,VoucherGuid,OperateType,OperateNo,OperateTime) VALUE('{0}','{1}',1,'9999','{2}');",
                            Guid.NewGuid().ToString(), voucherGuid, mcCardinfo.OptDate);
                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                    }
                }

            }
            catch (Exception o1)
            {
                completeFlag = false;
                ShowMessage(string.Format("处理凭证【{0}】信息异常", mcCardinfo.IDNO));
            }
            return completeFlag;
        }

        public string IpAddr
        {
            get { return (string)GetValue(IpAddrProperty); }
            set { SetValue(IpAddrProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IpAddr.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IpAddrProperty =
            DependencyProperty.Register("IpAddr", typeof(string), typeof(JSOCT2017ToJieLinkViewModel));


        public string DbName
        {
            get { return (string)GetValue(DbNameProperty); }
            set { SetValue(DbNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DbName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DbNameProperty =
            DependencyProperty.Register("DbName", typeof(string), typeof(JSOCT2017ToJieLinkViewModel));


        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(JSOCT2017ToJieLinkViewModel));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(JSOCT2017ToJieLinkViewModel));



        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(JSOCT2017ToJieLinkViewModel));


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
    }
}
