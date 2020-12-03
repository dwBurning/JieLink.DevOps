package com.jieshun.devopsserver.service;

import com.jieshun.devopsserver.bean.DevOpsEventFilterExample;
import com.jieshun.devopsserver.bean.DevOpsEventFilterExample.Criteria;
import com.jieshun.devopsserver.mapper.DevOpsEventFilterMapper;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.jieshun.devopsserver.bean.DevOpsEventFilter;

/**
 * 
 * @author 作者 ：dengwei
 * 
 * @version 创建时间：2020年12月3日 下午2:03:32
 * 
 * @Description 描述：
 * 
 */
@Service
public class DevOpsEventFilterService {

	@Autowired
	DevOpsEventFilterMapper devOpsEventFilterMapper;

	public DevOpsEventFilter getDevOpsEventFilterByCode(int code) {
		DevOpsEventFilterExample example = new DevOpsEventFilterExample();
		Criteria criteria = example.createCriteria();
		criteria.andEventCodeEqualTo(code);
		List<DevOpsEventFilter> devOpsEventFilters = devOpsEventFilterMapper.selectByExample(example);
		if (devOpsEventFilters != null && devOpsEventFilters.size() > 0) {
			return devOpsEventFilters.get(0);
		}
		return null;
	}
}
