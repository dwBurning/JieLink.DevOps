package com.jieshun.devopsserver.service;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.jieshun.devopsserver.bean.PageSet;
import com.jieshun.devopsserver.bean.VersionInfo;
import com.jieshun.devopsserver.bean.VersionInfoExample;
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

}
