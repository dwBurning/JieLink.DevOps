package com.jieshun.devopsserver.controller;

import java.util.Date;

import com.jieshun.devopsserver.bean.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.jieshun.devopsserver.service.VersionInfoService;

/**
 * 版本管理Controller
 * 
 * @author wei
 *
 */
@RestController
@RequestMapping(value = "/version")
public class VersionInfoController {

	
	private static final Logger log = LoggerFactory.getLogger(VersionInfoController.class);

	
	@Autowired
	VersionInfoService versionInfoService;

	/**
	 * 根据分页查询版本信息
	 * 
	 * @param orderNo 工单号
	 * @param start   起始索引
	 * @param end     结束索引
	 * @return 分页对象
	 */
	@RequestMapping(value = "/getVersionInfoWithPages", method = RequestMethod.GET)
	public PageSet<VersionInfo> getVersionInfoWithPages(String orderNo, int start, int end) {
		return versionInfoService.getVersionInfoWithPages(orderNo, start, end);
	}

	/**
	 * 发布版本
	 * 
	 * @param workOrderNo
	 * @param standVersion
	 * @param versionType
	 * @param compileDate
	 * @param versionDescribe
	 * @param downloadMsg
	 * @return
	 */
	@RequestMapping(value = "/addVersionInfo", method = RequestMethod.POST)
	public ReturnData addVersionInfo(String workOrderNo, String standVersion, int versionType, Date compileDate,
			String versionDescribe, String downloadMsg) {
		VersionInfo versionInfo = new VersionInfo();
		versionInfo.setWorkOrderNo(workOrderNo);
		versionInfo.setStandVersion(standVersion);
		versionInfo.setVersionType(versionType);
		versionInfo.setCompileDate(compileDate);
		versionInfo.setVersionDescribe(versionDescribe);
		versionInfo.setDownloadMsg(downloadMsg);
		int result = versionInfoService.addVersionInfo(versionInfo);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}
	
	@RequestMapping(value = "/deleteVersionInfoById", method = RequestMethod.DELETE)
	public ReturnData deleteVersionInfoById(int id) {
		int result = versionInfoService.deleteVersionInfoById(id);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}

	/**
	 * 根据分页查询版本下载情况
	 *
	 * @param standVersion 工单号
	 * @param startIndex   起始索引
	 * @param endIndex     结束索引
	 * @return 分页对象
	 */
	@RequestMapping(value = "/getVersionDownloadInfoWithPages", method = RequestMethod.GET)
	public PageSet<VersionInfo> getVersionDownloadInfoWithPages(String standVersion, int startIndex, int endIndex) {
		return versionInfoService.getVersionDownloadInfoWithPages(standVersion, startIndex, endIndex);
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
	@RequestMapping(value = "/getApplyInfoByWorkOrderNoWithPages", method = RequestMethod.GET)
	public PageSet<ApplyInfo> getApplyInfoByWorkOrderNoWithPages(int versionInfoKeyId, int startIndex, int endIndex, int enableAll) {
		return versionInfoService.getApplyInfoByWorkOrderNoWithPages(versionInfoKeyId, startIndex, endIndex, enableAll);
	}
}
