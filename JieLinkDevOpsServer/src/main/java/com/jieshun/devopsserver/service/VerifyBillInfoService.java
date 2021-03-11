package com.jieshun.devopsserver.service;

import java.util.Collection;
import java.util.Iterator;
import java.util.List;
import java.util.ListIterator;
import java.util.stream.Collectors;

import com.jieshun.devopsserver.bean.*;
import com.jieshun.devopsserver.mapper.ApplyInfoMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.jieshun.devopsserver.bean.VerifyBillInfoExample.Criteria;
import com.jieshun.devopsserver.controller.VerifyBillInfoController.StatusCode;
import com.jieshun.devopsserver.mapper.VerifyBillInfoMapper;
/**
 * 版本管理Service
 * 
 * @author ggd
 *
 */
@Service
public class VerifyBillInfoService {

	private static final Logger log = LoggerFactory.getLogger(VerifyBillInfoService.class);

	@Autowired
	VerifyBillInfoMapper VerifyBillInfoMapper;
	@Autowired
	ApplyInfoMapper applyInfoMapper;

	/**
	 * 根据分页查询版本信息
	 * 
	 * @param orderNo 工单号
	 * @param start   起始索引
	 * @param end     结束索引
	 * @return 分页对象
	 */
	public PageSet<VerifyBillInfo> getVerifyBillInfoWithPages(String orderNo, int start, int end) {
		//String limitString = String.format("operator_date desc limit %d,%d", start, end - start);
		String limitString = String.format("add_date desc limit %d,%d", start, end - start);
		VerifyBillInfoExample example = new VerifyBillInfoExample();
		Criteria criteria = example.createCriteria();
		criteria.andIsdeleteEqualTo(0);
		criteria.andStatusNotEqualTo(StatusCode.Finsih.getValue());
		/*if (orderNo != null && orderNo != "") {

			criteria.andWorkOrderNoLike("%" + orderNo + "%");
		}*/
		int total = VerifyBillInfoMapper.countByExample(example);
		if (total == 0 && orderNo != null && orderNo != "") {
			criteria = example.or();
			criteria.andIsdeleteEqualTo(0);
			//criteria.andVersionDescribeLike("%" + orderNo + "%");
		}
		example.setOrderByClause(limitString);
		List<VerifyBillInfo> VerifyBillInfos = VerifyBillInfoMapper.selectByExample(example);
		PageSet<VerifyBillInfo> pageSet = new PageSet<>();
		pageSet.setItems(VerifyBillInfos);
		pageSet.setTotal(total);
		return pageSet;
	}

	
	/**
	 * 
	 * @param projectshoperno 商户号
	 * @param projectno
	 * @param projectname
	 * @return
	 */
	public String SearchSunloginByInfo(String projectshoperno,String projectno,String projectname)
	{
		//主要搜索对账表
		VerifyBillInfoExample example = new VerifyBillInfoExample();
		
		String limitString = String.format("add_date desc limit 1");
		example.setOrderByClause(limitString);
		
		//根据商户号搜索
		Criteria CriteriaShoperno = example.createCriteria();
		CriteriaShoperno.andIsdeleteEqualTo(0);
		CriteriaShoperno.andProjectShoperNoEqualTo(projectshoperno);
		List<VerifyBillInfo> VerifyBillInfosShopNo = VerifyBillInfoMapper.selectByExample(example);
		if(!VerifyBillInfosShopNo.isEmpty())
			return VerifyBillInfosShopNo.get(0).getProjectRemote() + "/" + VerifyBillInfosShopNo.get(0).getProjectRemotePassword();
		
		//根据项目编号搜索
		example.clear();
		example.setOrderByClause(limitString);
		Criteria CriteriaProNo = example.createCriteria();
		CriteriaProNo.andIsdeleteEqualTo(0);
		CriteriaProNo.andProjectNoEqualTo(projectno);
		List<VerifyBillInfo> VerifyBillInfosProNo = VerifyBillInfoMapper.selectByExample(example);
		if(!VerifyBillInfosProNo.isEmpty())
			return VerifyBillInfosProNo.get(0).getProjectRemote() + "/" + VerifyBillInfosProNo.get(0).getProjectRemotePassword();
		
		//根据项目名搜索
		example.clear();
		example.setOrderByClause(limitString);
		Criteria CriteriaProName = example.createCriteria();
		CriteriaProName.andIsdeleteEqualTo(0);
		CriteriaProName.andProjectNameEqualTo(projectname);
		List<VerifyBillInfo> VerifyBillInfosProName = VerifyBillInfoMapper.selectByExample(example);
		if(!VerifyBillInfosProName.isEmpty())
			return VerifyBillInfosProName.get(0).getProjectRemote() + "/" + VerifyBillInfosProName.get(0).getProjectRemotePassword();
		
		return "";
	}
	
	/**
	 * @param id 
	 * @return 增加一点紧急度
	 */
	public int addEmergencyById(int id) {
		VerifyBillInfo VerifyBillInfo = VerifyBillInfoMapper.selectByPrimaryKey(id);
		VerifyBillInfo.setEmergency(VerifyBillInfo.getEmergency()+1);
		return VerifyBillInfoMapper.updateByPrimaryKeySelective(VerifyBillInfo);
		//return 1;
	}

	/**
	 * 根据Id删除工单信息
	 * 
	 * @param id
	 * @return
	 */
	public int deleteVerifyBillInfoById(int id) {
		VerifyBillInfo VerifyBillInfo = new VerifyBillInfo();
		VerifyBillInfo.setId(id);
		VerifyBillInfo.setIsdelete(1);
		return VerifyBillInfoMapper.updateByPrimaryKeySelective(VerifyBillInfo);
	}
	
	/**
	 * 根据ID完成工单信息
	 * @param id
	 * @return
	 */
	public int finishVerifyBillInfoById(int id)
	{
		VerifyBillInfo VerifyBillInfo = new VerifyBillInfo();
		VerifyBillInfo.setId(id);
		VerifyBillInfo.setStatus(StatusCode.Finsih.getValue());
		return VerifyBillInfoMapper.updateByPrimaryKeySelective(VerifyBillInfo);
	}
	/**
	 * 研发确认，在备注后增加处理结果
	 * @param id
	 * @param remark
	 * @param user
	 * @return
	 */
	public int confirmVerifyBillInfoById(int id,String remark,String user)
	{
		VerifyBillInfo VerifyBillInfo = VerifyBillInfoMapper.selectByPrimaryKey(id);
		VerifyBillInfo.setProjectRemark(VerifyBillInfo.getProjectRemark() + "\r\n" + user + ":" + remark);
		VerifyBillInfo.setStatus(StatusCode.DeveloperDone.getValue());
		return VerifyBillInfoMapper.updateByPrimaryKeySelective(VerifyBillInfo);
	}
	
	/**
	 * 根据ID获取SQL语句
	 * @param id
	 * @return
	 */
	public String GetAutoSqlById(int id)
	{
		VerifyBillInfo VerifyBillInfo = VerifyBillInfoMapper.selectByPrimaryKey(id);
		String ret = VerifyBillInfo.getAutosql();
		if(ret != null )
			return ret;
		else
			return "";
	}
	
	/**
	 * 
	 * @param id
	 * @return
	 */
	public String GetUrlById(int id)
	{
		VerifyBillInfo VerifyBillInfo = VerifyBillInfoMapper.selectByPrimaryKey(id);
		String ret = VerifyBillInfo.getUploadfilename();
		if(ret != null)
			return ret;
		else
			return "";
	}
	
	/**
	 * 根据ID设置远程
	 * @return
	 */
	public int SetRemoteById(int id,String remote,String remote_password)
	{
		VerifyBillInfo VerifyBillInfo = VerifyBillInfoMapper.selectByPrimaryKey(id);
		VerifyBillInfo.setProjectRemote(remote);
		VerifyBillInfo.setProjectRemotePassword(remote_password);
		return VerifyBillInfoMapper.updateByPrimaryKeySelective(VerifyBillInfo);
	}
	
	/**
	 * 根据ID设置SQL语句
	 * @param id
	 * @param sql
	 * @return
	 */
	public int SetAutoSqlById(int id,String sql)
	{
		VerifyBillInfo VerifyBillInfo = VerifyBillInfoMapper.selectByPrimaryKey(id);
				
		//如果不需要查原因，以及不需要补推，就不需要研发处理了
		if(!VerifyBillInfo.getProjectTask().contains("查因") && !VerifyBillInfo.getProjectTask().contains("补推"))
		{
			VerifyBillInfo.setStatus(StatusCode.DoItYourSelf.getValue());
		}
		VerifyBillInfo.setAutosql(sql);

		return VerifyBillInfoMapper.updateByPrimaryKeySelective(VerifyBillInfo);
	}
	
	public String CheckSQL(String filename)
	{
		//根据文件名搜索ID
		VerifyBillInfoExample example = new VerifyBillInfoExample();
		
		String limitString = String.format("add_date desc limit 1");
		example.setOrderByClause(limitString);
		
		Criteria CriteriaShoperno = example.createCriteria();
		CriteriaShoperno.andUploadfilenameEqualTo(filename);
		
		List<VerifyBillInfo> VerifyBillInfo = VerifyBillInfoMapper.selectByExampleWithBLOBs(example);
		if(VerifyBillInfo.get(0).getAutosql().contains("没有检测到文件，如果文件较大可能还在上传中"))
		{
			//返回任务列表
			return VerifyBillInfo.get(0).getProjectTask();
		}
		return "";
	}
	
	/**
	 * 根据文件名设置SQL
	 * @param filename
	 * @param SQL
	 * @return
	 */
	public int SetSqlByFileName(String filename,String SQL)
	{
		//根据文件名搜索ID
		VerifyBillInfoExample example = new VerifyBillInfoExample();
		
		String limitString = String.format("add_date desc limit 1");
		example.setOrderByClause(limitString);
		
		Criteria CriteriaShoperno = example.createCriteria();
		CriteriaShoperno.andUploadfilenameEqualTo(filename);
		
		List<VerifyBillInfo> VerifyBillInfo = VerifyBillInfoMapper.selectByExampleWithBLOBs(example);
		
		VerifyBillInfo temp = new VerifyBillInfo();
		temp.setAutosql(SQL);
		return VerifyBillInfoMapper.updateByExampleWithBLOBs(temp, example);
	}
//	/**
//	 * 根据工单号查询版本信息
//	 * 
//	 * @param orderNo 工单号
//	 * @return 版本信息对象
//	 */
//	public VerifyBillInfo getVerifyBillInfoByOrderNo(String orderNo) {
//		VerifyBillInfoExample example = new VerifyBillInfoExample();
//		Criteria criteria = example.createCriteria();
//		//criteria.andWorkOrderNoEqualTo(orderNo);
//		return VerifyBillInfoMapper.selectByExample(example).get(0);
//	}
//
	/**
	 * 发布版本
	 * 
	 * @param VerifyBillInfo 版本信息对象
	 * @return 受影响行数
	 */
	public int addVerifyBillInfo(VerifyBillInfo VerifyBillInfo) {
		return VerifyBillInfoMapper.insertSelective(VerifyBillInfo);
	}

	/**
	 * 生成SQL语句 
	 * @return
	 */
//	public String VerifyBillMakeSQL()
//	{
//		VerifyBillInfo VerifyBillInfo = new VerifyBillInfo();
//		VerifyBillInfo.setAutosql("select \"这是测试生成sql的测试数据，看到此条说明文件上传，生成SQL，查看SQL功能正常\"");
//		//VerifyBillInfo.setStatus(4);
//		return "";
//	}
//
//	/**
//	 * 根据分页查询版本信息
//	 *
//	 * @param standVersion 版本号
//	 * @param startIndex   起始索引
//	 * @param endIndex     结束索引
//	 * @return 分页对象
//	 */
//	public PageSet<VerifyBillInfo> getVersionDownloadInfoWithPages(String standVersion, int startIndex, int endIndex) {
////		String limitString = String.format("operator_date desc limit %d,%d", startIndex, endIndex - startIndex);
////		VerifyBillInfoExample example = new VerifyBillInfoExample();
////		Criteria criteria = example.createCriteria();
////		criteria.andIsDeletedEqualTo(0);
////		if (standVersion != null && standVersion != "")
////		{
////			criteria.andStandVersionLike("%" + standVersion + "%");
////		}
////		int total = VerifyBillInfoMapper.countByExample(example);
////		if (total == 0 && standVersion != null && standVersion != "") {
////			criteria = example.or();
////			criteria.andIsDeletedEqualTo(0);
////			criteria.andVersionDescribeLike("%" + standVersion + "%");
////		}
////		total = VerifyBillInfoMapper.countByExample(example);
////		example.setOrderByClause(limitString);
////		List<VerifyBillInfo> VerifyBillInfoList = VerifyBillInfoMapper.selectByExample(example);
////		if(VerifyBillInfoList.size()>0) {
////			//List<String> workOrderNos = VerifyBillInfoList.stream().map(VerifyBillInfo::getWorkOrderNo).collect(Collectors.toList());
////			ApplyInfoExample applyInfoExample = new ApplyInfoExample();
////			ApplyInfoExample.Criteria applyInfoCriteria = applyInfoExample.createCriteria();
////			//applyInfoCriteria.andWorkOrderNoIn(workOrderNos);
////			applyInfoExample.setOrderByClause("apply_date desc");
////			List<ApplyInfo> applyInfoList = applyInfoMapper.selectByExample(applyInfoExample);
////			if (applyInfoList.size() > 0) {
////				for (VerifyBillInfo VerifyBillInfo : VerifyBillInfoList) {
////					List<ApplyInfo> currApplyList = applyInfoList.stream().filter(ApplyInfo -> ApplyInfo.getWorkOrderNo().equals(VerifyBillInfo.getWorkOrderNo())).collect(Collectors.toList());
////					if (currApplyList.size() > 0) {
////						VerifyBillInfo.setChildren(currApplyList);
////						VerifyBillInfo.setDownloadCount(currApplyList.size());
////						VerifyBillInfo.setHasChildren(true);
////					}
////					else{
////						VerifyBillInfo.setHasChildren(false);
////					}
////				}
////			}
////		}
//		PageSet<VerifyBillInfo> pageSet = new PageSet<>();
////		pageSet.setItems(VerifyBillInfoList);
////		pageSet.setTotal(total);
//		return pageSet;
//	}
//
//	/**
//	 * 根据分页查询版本下载情况
//	 *
//	 * @param VerifyBillInfoKeyId VerifyBillInfo的主键ID
//	 * @param startIndex   起始索引
//	 * @param endIndex     结束索引
//	 * @param enableAll	   是否查询出所有数据 0所有（不看start、end），1分页
//	 * @return 分页对象
//	 */
//	public PageSet<ApplyInfo> getApplyInfoByWorkOrderNoWithPages(int VerifyBillInfoKeyId, int startIndex, int endIndex, int enableAll) {
//		VerifyBillInfoExample example = new VerifyBillInfoExample();
//		Criteria criteria = example.createCriteria();
//		//criteria.andIsDeletedEqualTo(0);
//		criteria.andIdEqualTo(VerifyBillInfoKeyId);
//		int total = VerifyBillInfoMapper.countByExample(example);
//		VerifyBillInfo VerifyBillInfo = VerifyBillInfoMapper.selectByPrimaryKey(VerifyBillInfoKeyId);
//		PageSet<ApplyInfo> pageSet = new PageSet<>();
//		pageSet.setTotal(total);
//		if(VerifyBillInfo != null) {
//			if(enableAll == 0){
//				//展开项：查询所有，不分页
//				endIndex = Integer.MAX_VALUE;
//			}
//			String limitString = String.format("apply_date desc limit %d,%d", startIndex, endIndex);
//			ApplyInfoExample applyInfoExample = new ApplyInfoExample();
//			ApplyInfoExample.Criteria applyInfoCriteria = applyInfoExample.createCriteria();
//			//applyInfoCriteria.andWorkOrderNoEqualTo(VerifyBillInfo.getWorkOrderNo());
//			applyInfoExample.setOrderByClause(limitString);
//			List<ApplyInfo> applyInfoList = applyInfoMapper.selectByExample(applyInfoExample);
//			pageSet.setItems(applyInfoList);
//		}
//		return pageSet;
//	}
}

