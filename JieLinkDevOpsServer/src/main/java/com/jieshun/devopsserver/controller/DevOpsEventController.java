package com.jieshun.devopsserver.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.alibaba.fastjson.JSON;
import com.jieshun.devopsserver.bean.DevOpsEvent;
import com.jieshun.devopsserver.bean.PageSet;
import com.jieshun.devopsserver.bean.ReturnData;
import com.jieshun.devopsserver.bean.ReturnStateEnum;
import com.jieshun.devopsserver.bean.VersionInfo;
import com.jieshun.devopsserver.service.DevOpsEventService;

@RestController
@RequestMapping(value = "/devops")
public class DevOpsEventController {

	@Autowired
	DevOpsEventService devOpsEventService;

	@RequestMapping(value = "/addApplyInfo", method = RequestMethod.POST)
	public ReturnData reportDevOpsEvent(String params) {
		DevOpsEvent devOpsEvent = JSON.parseObject(params, DevOpsEvent.class);
		int result = devOpsEventService.reportDevOpsEvent(devOpsEvent);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}

	/**
	 * 根据分页查询版本信息
	 * 
	 * @param orderNo 工单号
	 * @param start   起始索引
	 * @param end     结束索引
	 * @return 分页对象
	 */
	@RequestMapping(value = "/getDevOpsEventWithPages", method = RequestMethod.GET)
	public PageSet<DevOpsEvent> getDevOpsEventWithPages(int eventCode, int start, int end) {
		return devOpsEventService.getDevOpsEventWithPages(eventCode, start, end);
	}

	@RequestMapping(value = "/processed", method = RequestMethod.PUT)
	public ReturnData processed(int id) {
		int result = devOpsEventService.processed(id);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}

}
