package com.jieshun.devopsserver.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.jieshun.devopsserver.bean.PageSet;
import com.jieshun.devopsserver.bean.VersionInfo;
import com.jieshun.devopsserver.service.VersionInfoService;

/**
 * 版本管理Controller
 * @author wei
 *
 */
@RestController
@RequestMapping(value = "/version")
public class VersionInfoController {

	@Autowired
	VersionInfoService versionInfoService;
	
	/**
	 * 根据分页查询版本信息
	 * @param orderNo 工单号
	 * @param start 起始索引
	 * @param end 结束索引
	 * @return 分页对象
	 */
	@RequestMapping(value = "/getVersionInfoWithPages", method = RequestMethod.GET)
	public PageSet<VersionInfo> getVersionInfoWithPages(String orderNo, int start, int end) {
		return versionInfoService.getVersionInfoWithPages(orderNo, start, end);
	}

}
