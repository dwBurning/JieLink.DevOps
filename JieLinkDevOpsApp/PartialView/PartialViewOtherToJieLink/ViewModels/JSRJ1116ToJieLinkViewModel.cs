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
    public class JSRJ1116ToJieLinkViewModel : DependencyObject
    {

        private readonly string GROUPROOTPARENTID = "00000000-0000-0000-0000-000000000000";

        private readonly string REMARK = "JSRJ1116";

        private readonly string PARKNO = "00000000-0000-0000-0000-000000000000";

        public DelegateCommand TestConnCommand { get; set; }

        public DelegateCommand UpgradeCommand { get; set; }

        public bool CanExecute { get; set; }

        public JSRJ1116ToJieLinkViewModel()
        {
            TestConnCommand = new DelegateCommand();
            TestConnCommand.ExecuteAction = TestConn;

            UpgradeCommand = new DelegateCommand();
            UpgradeCommand.ExecuteAction = Upgrade;
            UpgradeCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return CanExecute; });
        }

        private void TestConn(object parameter)
        {
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
                if (MSSqlHelper.TestDbConnection(conn.ConnectionStr))
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
                MessageBoxHelper.MessageBoxShowWarning("连接错误，请重新输入JSDS数据库连接信息");
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
                    StartUpgradeJSRJ();
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

        private bool VoucherSaveJSRJ(TBaseHrPersonModel hrPerson, TBaseJSRJCardInfoModel tcCardInfoModel, TBaseJSRJCardInfoModel tcCardEndDateMax)
        {
            bool completeFlag = true;
            try
            {
                string voucherGuid = Guid.NewGuid().ToString();
                int voucherType = 163;
                string physicsNum = string.Empty;
                string remark = REMARK;
                string sql = string.Format("INSERT INTO control_voucher(Guid,PGUID,LGUID,PersonNo,PersonName,VoucherType,VoucherNo,CardNum,PhysicsNum,AddOperatorNo,AddTime,`Status`,LastTime,Remark,StatusFromPerson) VALUE('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{6}','{7}','9999','{8}',1,'{8}','{9}',1);",
                          voucherGuid, new Guid(hrPerson.GUID).ToString(), new Guid(tcCardEndDateMax.GUID).ToString(), hrPerson.NO, hrPerson.NAME, voucherType, tcCardInfoModel.CarNO.Replace("-", ""), physicsNum, tcCardInfoModel.OptDate, remark);
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
                           Guid.NewGuid().ToString(), voucherGuid, voucherType, tcCardInfoModel.CarNO.Replace("-", ""), new Guid(hrPerson.GUID).ToString(), hrPerson.NO, tcCardInfoModel.OptDate);
                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);

                        sql = string.Format("INSERT into control_voucher_record(id,VoucherGuid,OperateType,OperateNo,OperateTime) VALUE('{0}','{1}',1,'9999','{2}');",
                            Guid.NewGuid().ToString(), voucherGuid, tcCardInfoModel.OptDate);
                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                    }
                }

            }
            catch (Exception o1)
            {
                completeFlag = false;
                ShowMessage(string.Format("处理凭证【{0}】信息异常", tcCardInfoModel.ID));
            }
            return completeFlag;
        }


        private void StartUpgradeJSRJ()
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
                ShowMessage("正在迁移组织");
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

                ShowMessage("正在迁移人事");
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
                            string currentGuid = "";

                            try
                            {
                                currentGuid = new Guid((G3GroupList.FirstOrDefault(e => e.ID == currentDeptId)).GUID).ToString();
                            }
                            catch (Exception ex)
                            {
                                MessageBoxHelper.MessageBoxShowWarning("JSRJ1116数据库中的组织Guid异常：" + ex.ToString());
                                return;
                            }
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
                    DataTable JSRJCardInfoDts = MSSqlHelper.ExecuteQuery("SELECT *,b.GUID as LGUID from tc.MonthCarInfo a inner join mc.cardType b on a.cardTypeId=b.ID where status = 1  ORDER BY a.id asc;", null);
                    List<TBaseJSRJCardInfoModel> JSRJCardInfoList = new List<TBaseJSRJCardInfoModel>();
                    if (JSRJCardInfoDts != null)
                    {
                        JSRJCardInfoList = CommonHelper.DataTableToList<TBaseJSRJCardInfoModel>(JSRJCardInfoDts).OrderBy(x => x.ID).ToList();
                    }
                    ShowMessage("正在迁移车场服务");
                    //车场服务
                    if (JSRJCardInfoList.Count > 0)
                    {
                        List<TBaseJSRJCardInfoModel> remove = new List<TBaseJSRJCardInfoModel>();
                        foreach (TBaseJSRJCardInfoModel tcCardInfo in JSRJCardInfoList)
                        {

                            if (remove.Where(e => e.GUID == tcCardInfo.GUID).ToList().Count > 0)
                            {
                                continue;
                            }
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                bool completeFlag = true;  //事务提交标识
                                try
                                {
                                    TBaseHrPersonModel hrPerson1 = personList.FirstOrDefault(e => e.ID == tcCardInfo.PersonId);
                                    if (hrPerson1 == null)
                                    {
                                        continue;
                                    }
                                    if (hrPerson1.CarCount <= 1)
                                    {
                                        string lguid = new Guid(tcCardInfo.GUID).ToString();
                                        string startTime = CommonHelper.GetDateTimeValue(tcCardInfo.StartDate, DateTime.Now).ToString("yyyy-MM-dd 00:00:00");
                                        string endTime = CommonHelper.GetDateTimeValue(tcCardInfo.EndDate, DateTime.Now).ToString("yyyy-MM-dd 23:59:59");
                                        string stopServiceTime = CommonHelper.GetDateTimeValue(tcCardInfo.EndDate, DateTime.Now).ToString("yyyy-MM-dd");
                                        string uniqueServiceNo = CommonHelper.GetUniqueId();

                                        string sql = string.Format("INSERT INTO control_lease_stall(LGUID,PGUID,SetmealNo,StartTime,EndTime,OperatorNO,OperatorName,OperateTime,`Status`,PersonName,PersonNo,NisspId,CarNumber,VehiclePosCount,StopServiceTime,UniqueServiceNo,`Timestamp`) VALUE('{0}','{1}',50,'{2}','{3}','9999','超级管理员','{4}',0,'{5}','{6}','{0}','{7}','{7}','{8}','{9}',0)",
                                                    lguid, new Guid(hrPerson1.GUID).ToString(), startTime, endTime, DateTime.Now, hrPerson1.NAME, hrPerson1.NO, 1, stopServiceTime, uniqueServiceNo);
                                        int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                        if (flag > 0)
                                        {
                                            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsParkService=1 WHERE PGUID='{0}';", new Guid(hrPerson1.GUID).ToString()));
                                            bool saveFlag = VoucherSaveJSRJ(hrPerson1, tcCardInfo, tcCardInfo);
                                            if (!saveFlag)
                                            {
                                                isContinue = false;
                                                break;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        //获取这个人的所有卡的信息
                                        List<TBaseJSRJCardInfoModel> tcCardinfoList1 = JSRJCardInfoList.Where(e => e.PersonId == hrPerson1.ID).Distinct().ToList();
                                        if (tcCardinfoList1.Count > 0)
                                        {
                                            //根据卡类分组
                                            var tcCardMap = tcCardinfoList1.GroupBy(e => e.CardTypeID).ToList();
                                            int size;
                                            foreach (var item in tcCardMap)
                                            {
                                                TBaseJSRJCardInfoModel tcCardEndDateMax = item.OrderByDescending(e => e.EndDate).FirstOrDefault();
                                                switch (tcCardEndDateMax.CardTypeID)
                                                {
                                                    case 5:
                                                        size = 50;
                                                        break;
                                                    case 6:
                                                        size = 59;
                                                        break;
                                                    case 7:
                                                        size = 60;
                                                        break;
                                                    case 8:
                                                        size = 61;
                                                        break;
                                                    default:
                                                        size = 64;
                                                        break;
                                                }
                                                string lguid = new Guid(tcCardEndDateMax.GUID).ToString();
                                                string startTime = CommonHelper.GetDateTimeValue(tcCardEndDateMax.StartDate, DateTime.Now).ToString("yyyy-MM-dd 00:00:00");
                                                string endTime = CommonHelper.GetDateTimeValue(tcCardEndDateMax.EndDate, DateTime.Now).ToString("yyyy-MM-dd 23:59:59");
                                                string stopServiceTime = CommonHelper.GetDateTimeValue(tcCardEndDateMax.EndDate, DateTime.Now).ToString("yyyy-MM-dd");
                                                string uniqueServiceNo = CommonHelper.GetUniqueId();

                                                string sql = string.Format("INSERT INTO control_lease_stall(LGUID,PGUID,SetmealNo,StartTime,EndTime,OperatorNO,OperatorName,OperateTime,`Status`,PersonName,PersonNo,NisspId,CarNumber,VehiclePosCount,StopServiceTime,UniqueServiceNo,`Timestamp`) VALUE('{0}','{1}','{10}','{2}','{3}','9999','超级管理员','{4}',0,'{5}','{6}','{0}','{7}','{7}','{8}','{9}',0)",
                                                            lguid, new Guid(hrPerson1.GUID).ToString(), startTime, endTime, DateTime.Now, hrPerson1.NAME, hrPerson1.NO, hrPerson1.CarCount, stopServiceTime, uniqueServiceNo, size);
                                                int flag = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                                                if (flag > 0)
                                                {
                                                    MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, string.Format("UPDATE control_person SET IsParkService=1 WHERE PGUID='{0}';", new Guid(hrPerson1.GUID).ToString()));


                                                }
                                                //移除已经处理的车场卡信息
                                                foreach (var a in item)
                                                {
                                                    bool saveFlag = VoucherSaveJSRJ(hrPerson1, a, tcCardEndDateMax);
                                                    if (!saveFlag)
                                                    {
                                                        isContinue = false;
                                                        break;
                                                    }
                                                    remove.Add(a);
                                                }


                                            }
                                        }
                                    }

                                }
                                catch (Exception o)
                                {
                                    completeFlag = false;
                                    ShowMessage(string.Format("处理MC_Card_Id【{0}】的车场凭证服务信息异常：{1}", tcCardInfo.ID, o.ToString()));
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
                ShowMessage("正在迁移场内记录");
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

        public string IpAddr
        {
            get { return (string)GetValue(IpAddrProperty); }
            set { SetValue(IpAddrProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IpAddr.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IpAddrProperty =
            DependencyProperty.Register("IpAddr", typeof(string), typeof(JSRJ1116ToJieLinkViewModel));


        public string DbName
        {
            get { return (string)GetValue(DbNameProperty); }
            set { SetValue(DbNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DbName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DbNameProperty =
            DependencyProperty.Register("DbName", typeof(string), typeof(JSRJ1116ToJieLinkViewModel));


        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(JSRJ1116ToJieLinkViewModel));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(JSRJ1116ToJieLinkViewModel));



        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(JSRJ1116ToJieLinkViewModel));


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
