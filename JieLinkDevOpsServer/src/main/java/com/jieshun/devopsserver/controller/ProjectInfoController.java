package com.jieshun.devopsserver.controller;

import java.util.Date;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.alibaba.fastjson.JSON;
import com.jieshun.devopsserver.bean.DevOpsProduct;
import com.jieshun.devopsserver.bean.ProjectInfo;
import com.jieshun.devopsserver.bean.ReturnData;
import com.jieshun.devopsserver.bean.ReturnStateEnum;
import com.jieshun.devopsserver.service.DevOpsEventService;
import com.jieshun.devopsserver.service.DevOpsToolService;
import com.jieshun.devopsserver.service.ProjectInfoService;

/**
 * 
 * @author 作者 ：dengwei
 * 
 * @version 创建时间：2020年11月28日 下午5:33:10
 * 
 * @Description 描述：
 * 
 */
@RestController
@RequestMapping(value = "/devops")
public class ProjectInfoController {

	@Autowired
	ProjectInfoService projectinfoService;

	@Autowired
	DevOpsToolService devOpsToolService;

	@Autowired
	DevOpsEventService devOpsEventService;

	@RequestMapping(value = "/reportProjectInfo", method = RequestMethod.POST)
	public DevOpsProduct reportProjectInfo(@RequestBody String params) {
		ProjectInfo projectInfo = JSON.parseObject(params, ProjectInfo.class);
		projectinfoService.reportProjectInfo(projectInfo);
		return devOpsToolService.getTheLastVersion(projectInfo.getProductType());
	}

	@RequestMapping(value = "/updateProjectInfo", method = RequestMethod.POST)
	public ReturnData updateProjectInfo(String projectNo, Integer isFilter, String remark) {
		ProjectInfo projectInfo = new ProjectInfo();
		projectInfo.setProjectNo(projectNo);
		projectInfo.setIsFilter(isFilter);
		projectInfo.setRemark(remark);

		if (isFilter == 1) {
			devOpsEventService.filterByProjectNo(projectNo);
		}

		int result = projectinfoService.updateProjectInfo(projectInfo);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}

	@RequestMapping(value = "/filter", method = RequestMethod.PUT)
	public ReturnData filter(String projectNo) {

		devOpsEventService.filterByProjectNo(projectNo);

		int result = projectinfoService.filter(projectNo);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}

	@RequestMapping(value = "/getProjectInfoByProjectNo", method = RequestMethod.GET)
	public ProjectInfo getProjectInfoByProjectNo(String projectNo) {
		ProjectInfo projectInfo = projectinfoService.getProjectInfoByProjectNo(projectNo);
		if (projectInfo != null) {
			return projectInfo;
		}

		projectInfo = new ProjectInfo();
		projectInfo.setDevopsVersion("V1.0.0");
		projectInfo.setProjectNo(projectNo);
		projectInfo.setIsFilter(0);
		projectInfo.setRemark("没有收到该项目上报版本信息");
		projectinfoService.reportProjectInfo(projectInfo);
		return projectInfo;
	}

}
