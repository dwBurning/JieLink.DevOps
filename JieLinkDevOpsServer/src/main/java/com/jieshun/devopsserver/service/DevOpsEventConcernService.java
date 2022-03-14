package com.jieshun.devopsserver.service;

/** 

* @author 作者 ：dengwei

* @version 创建时间：2020年12月29日 上午9:24:10 

* @Description 描述：

*/

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.jieshun.devopsserver.bean.DevOpsEventConcern;
import com.jieshun.devopsserver.bean.DevOpsEventConcernExample;
import com.jieshun.devopsserver.bean.DevOpsEventConcernExample.Criteria;
import com.jieshun.devopsserver.mapper.DevOpsEventConcernMapper;

@Service
public class DevOpsEventConcernService {

	@Autowired
	DevOpsEventConcernMapper devOpsEventConcernMapper;

	public DevOpsEventConcern getdevopsEventConcernByProjectNo(String projectNo, int eventType) {

		DevOpsEventConcernExample example = new DevOpsEventConcernExample();
		Criteria criteria = example.createCriteria();
		criteria.andProjectNoEqualTo(projectNo);
		criteria.andEventTypeEqualTo(0);

		List<DevOpsEventConcern> devOpsEventConcerns = devOpsEventConcernMapper.selectByExample(example);
		if (devOpsEventConcerns != null && devOpsEventConcerns.size() > 0) {
			return devOpsEventConcerns.get(0);
		} else {
			criteria.andEventTypeEqualTo(eventType);
			devOpsEventConcerns = devOpsEventConcernMapper.selectByExample(example);
			if (devOpsEventConcerns != null && devOpsEventConcerns.size() > 0) {
				return devOpsEventConcerns.get(0);
			}
		}

		return null;
	}
}
