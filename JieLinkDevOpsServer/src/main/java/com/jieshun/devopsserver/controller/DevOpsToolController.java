package com.jieshun.devopsserver.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.jieshun.devopsserver.bean.DevOpsProduct;
import com.jieshun.devopsserver.bean.PageSet;
import com.jieshun.devopsserver.bean.ReturnData;
import com.jieshun.devopsserver.bean.ReturnStateEnum;
import com.jieshun.devopsserver.service.DevOpsToolService;

@RestController
@RequestMapping(value = "/devops")
public class DevOpsToolController {

	@Autowired
	DevOpsToolService devOpsToolService;

	/**
	 * 根据分页查询版本信息
	 * 
	 * @param version 版本号
	 * @param start   起始索引
	 * @param end     结束索引
	 * @return 分页对象
	 */
	@RequestMapping(value = "/getDevOpsProductWithPages", method = RequestMethod.GET)
	public PageSet<DevOpsProduct> getDevOpsProductWithPages(String version, int start, int end) {
		return devOpsToolService.getDevOpsProductWithPages(version, start, end);
	}

	@RequestMapping(value = "/addDevOpsProduct", method = RequestMethod.POST)
	public ReturnData addDevOpsProduct(int productType, String productVersion, String versionDescribe, String downloadUrl) {
		DevOpsProduct devOpsTool = new DevOpsProduct();
		devOpsTool.setProductType(productType);
		devOpsTool.setProductVersion(productVersion);
		devOpsTool.setDownloadUrl(downloadUrl);
		devOpsTool.setVersionDescribe(versionDescribe);
		int result = devOpsToolService.addDevOpsProduct(devOpsTool);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}

	@RequestMapping(value = "/deleteDevOpsProductById", method = RequestMethod.DELETE)
	public ReturnData deleteDevOpsProductById(int id) {
		int result = devOpsToolService.deleteDevOpsProductById(id);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}

	@RequestMapping(value = "/getTheLastVersion", method = RequestMethod.GET)
	public DevOpsProduct getTheLastVersion(int productType) {
		return devOpsToolService.getTheLastVersion(productType);
	}

}
