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
	 * @param orderNo 工单号
	 * @param start 起始索引
	 * @param end 结束索引
	 * @return 分页对象
	 */
	public PageSet<VersionInfo> getVersionInfoWithPages(String orderNo, int start, int end) {
		String limitString = String.format("compile_date desc limit %d,%d", start, end - start);
		VersionInfoExample example = new VersionInfoExample();
		Criteria criteria = example.createCriteria();
		if (orderNo != null && orderNo != "") {

			criteria.andWorkOrderNoLike("%" + orderNo + "%");
		}
		int total = versionInfoMapper.countByExample(example);
		example.setOrderByClause(limitString);
		List<VersionInfo> versionInfos = versionInfoMapper.selectByExample(example);
		PageSet<VersionInfo> pageSet = new PageSet<>();
		pageSet.setItems(versionInfos);
		pageSet.setTotal(total);
		return pageSet;
	}
}
