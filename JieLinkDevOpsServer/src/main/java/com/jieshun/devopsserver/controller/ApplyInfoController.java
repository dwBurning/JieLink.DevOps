package com.jieshun.devopsserver.controller;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;

import org.springframework.web.bind.annotation.RestController;

import com.jieshun.devopsserver.bean.ApplyInfo;
import com.jieshun.devopsserver.bean.ReturnData;
import com.jieshun.devopsserver.bean.ReturnStateEnum;
import com.jieshun.devopsserver.service.ApplyInfoService;

/**
 * 版本管理Controller
 * 
 * @author wei
 *
 */
@RestController
@RequestMapping(value = "/apply")
public class ApplyInfoController {

	private static final Logger log = LoggerFactory.getLogger(ApplyInfoController.class);

	@Autowired
	ApplyInfoService applyInfoService;

	@RequestMapping(value = "/addApplyInfo", method = RequestMethod.POST)
	public ReturnData addApplyInfo(String workOrderNo, String jobNumber, String name, String cellPhone, String email) {
		ApplyInfo applyInfo = new ApplyInfo();
		applyInfo.setWorkOrderNo(workOrderNo);
		applyInfo.setJobNumber(jobNumber);
		applyInfo.setName(name);
		applyInfo.setCellPhone(cellPhone);
		applyInfo.setEmail(email);
		int result = applyInfoService.addApplyInfo(applyInfo);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}
}
