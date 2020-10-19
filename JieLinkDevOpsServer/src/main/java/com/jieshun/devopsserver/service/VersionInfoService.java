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

import com.jieshun.devopsserver.bean.VersionInfoExample.Criteria;
import com.jieshun.devopsserver.mapper.VersionInfoMapper;

/**
 * 版本管理Service
 * 
 * @author wei
 *
 */
@Service
public class VersionInfoService {

	private static final Logger log = LoggerFactory.getLogger(VersionInfoService.class);

	@Autowired
	VersionInfoMapper versionInfoMapper;
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
	public PageSet<VersionInfo> getVersionInfoWithPages(String orderNo, int start, int end) {
		String limitString = String.format("operator_date desc limit %d,%d", start, end - start);
		VersionInfoExample example = new VersionInfoExample();
		Criteria criteria = example.createCriteria();
		criteria.andIsDeletedEqualTo(0);
		if (orderNo != null && orderNo != "") {

			criteria.andWorkOrderNoLike("%" + orderNo + "%");
		}
		int total = versionInfoMapper.countByExample(example);
		if (total == 0 && orderNo != null && orderNo != "") {
			criteria = example.or();
			criteria.andIsDeletedEqualTo(0);
			criteria.andVersionDescribeLike("%" + orderNo + "%");
		}
		total = versionInfoMapper.countByExample(example);
		example.setOrderByClause(limitString);
		List<VersionInfo> versionInfos = versionInfoMapper.selectByExample(example);
		PageSet<VersionInfo> pageSet = new PageSet<>();
		pageSet.setItems(versionInfos);
		pageSet.setTotal(total);
		return pageSet;
	}

	/**
	 * 根据工单号查询版本信息
	 * 
	 * @param orderNo 工单号
	 * @return 版本信息对象
	 */
	public VersionInfo getVersionInfoByOrderNo(String orderNo) {
		VersionInfoExample example = new VersionInfoExample();
		Criteria criteria = example.createCriteria();
		criteria.andWorkOrderNoEqualTo(orderNo);
		return versionInfoMapper.selectByExample(example).get(0);
	}

	/**
	 * 发不版本
	 * 
	 * @param versionInfo 版本信息对象
	 * @return 受影响行数
	 */
	public int addVersionInfo(VersionInfo versionInfo) {
		return versionInfoMapper.insertSelective(versionInfo);
	}

	/**
	 * 根据Id删除版本信息
	 * 
	 * @param id
	 * @return
	 */
	public int deleteVersionInfoById(int id) {
		VersionInfo versionInfo = new VersionInfo();
		versionInfo.setId(id);
		versionInfo.setIsDeleted(1);
		return versionInfoMapper.updateByPrimaryKeySelective(versionInfo);
	}


	/**
	 * 根据分页查询版本信息
	 *
	 * @param standVersion 版本号
	 * @param startIndex   起始索引
	 * @param endIndex     结束索引
	 * @return 分页对象
	 */
	public PageSet<VersionInfo> getVersionDownloadInfoWithPages(String standVersion, int startIndex, int endIndex) {
		String limitString = String.format("operator_date desc limit %d,%d", startIndex, endIndex - startIndex);
		VersionInfoExample example = new VersionInfoExample();
		Criteria criteria = example.createCriteria();
		criteria.andIsDeletedEqualTo(0);
		if (standVersion != null && standVersion != "")
		{
			criteria.andStandVersionLike("%" + standVersion + "%");
		}
		int total = versionInfoMapper.countByExample(example);
		if (total == 0 && standVersion != null && standVersion != "") {
			criteria = example.or();
			criteria.andIsDeletedEqualTo(0);
			criteria.andVersionDescribeLike("%" + standVersion + "%");
		}
		total = versionInfoMapper.countByExample(example);
		example.setOrderByClause(limitString);
		List<VersionInfo> versionInfoList = versionInfoMapper.selectByExample(example);
		if(versionInfoList.size()>0) {
			List<String> workOrderNos = versionInfoList.stream().map(VersionInfo::getWorkOrderNo).collect(Collectors.toList());
			ApplyInfoExample applyInfoExample = new ApplyInfoExample();
			ApplyInfoExample.Criteria applyInfoCriteria = applyInfoExample.createCriteria();
			applyInfoCriteria.andWorkOrderNoIn(workOrderNos);
			applyInfoExample.setOrderByClause("apply_date desc");
			List<ApplyInfo> applyInfoList = applyInfoMapper.selectByExample(applyInfoExample);
			if (applyInfoList.size() > 0) {
				for (VersionInfo versionInfo : versionInfoList) {
					List<ApplyInfo> currApplyList = applyInfoList.stream().filter(ApplyInfo -> ApplyInfo.getWorkOrderNo().equals(versionInfo.getWorkOrderNo())).collect(Collectors.toList());
					if (currApplyList.size() > 0) {
						versionInfo.setChildren(currApplyList);
						versionInfo.setDownloadCount(currApplyList.size());
						versionInfo.setHasChildren(true);
					}
					else{
						versionInfo.setHasChildren(false);
					}
				}
			}
		}
		PageSet<VersionInfo> pageSet = new PageSet<>();
		pageSet.setItems(versionInfoList);
		pageSet.setTotal(total);
		return pageSet;
	}

	/**
	 * 根据分页查询版本下载情况
	 *
	 * @param versionInfoKeyId VersionInfo的主键ID
	 * @param startIndex   起始索引
	 * @param endIndex     结束索引
	 * @param enableAll	   是否查询出所有数据 0所有（不看start、end），1分页
	 * @return 分页对象
	 */
	public PageSet<ApplyInfo> getApplyInfoByWorkOrderNoWithPages(int versionInfoKeyId, int startIndex, int endIndex, int enableAll) {
		VersionInfoExample example = new VersionInfoExample();
		Criteria criteria = example.createCriteria();
		criteria.andIsDeletedEqualTo(0);
		criteria.andIdEqualTo(versionInfoKeyId);
		int total = versionInfoMapper.countByExample(example);
		VersionInfo versionInfo = versionInfoMapper.selectByPrimaryKey(versionInfoKeyId);
		PageSet<ApplyInfo> pageSet = new PageSet<>();
		pageSet.setTotal(total);
		if(versionInfo != null) {
			if(enableAll == 0){
				//展开项：查询所有，不分页
				endIndex = Integer.MAX_VALUE;
			}
			String limitString = String.format("apply_date desc limit %d,%d", startIndex, endIndex);
			ApplyInfoExample applyInfoExample = new ApplyInfoExample();
			ApplyInfoExample.Criteria applyInfoCriteria = applyInfoExample.createCriteria();
			applyInfoCriteria.andWorkOrderNoEqualTo(versionInfo.getWorkOrderNo());
			applyInfoExample.setOrderByClause(limitString);
			List<ApplyInfo> applyInfoList = applyInfoMapper.selectByExample(applyInfoExample);
			pageSet.setItems(applyInfoList);
		}
		return pageSet;
	}
}
