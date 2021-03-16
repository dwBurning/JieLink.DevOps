package com.jieshun.devopsserver.bean;

import java.util.Date;

import com.alibaba.fastjson.annotation.JSONField;

public class ProjectInfo {
	private Integer id;

	private String projectVersion;

	private String projectName;

	private String projectNo;

	private String devopsVersion;

	private Integer isFilter;

	private Integer productType;
	
	@JSONField(format = "yyyy-MM-dd HH:mm:ss")
	private Date operatorDate;

	private String remark;

	public Integer getId() {
		return id;
	}

	public void setId(Integer id) {
		this.id = id;
	}

	public String getProjectVersion() {
		return projectVersion;
	}

	public void setProjectVersion(String projectVersion) {
		this.projectVersion = projectVersion == null ? null : projectVersion.trim();
	}

	public String getProjectName() {
		return projectName;
	}

	public void setProjectName(String projectName) {
		this.projectName = projectName == null ? null : projectName.trim();
	}

	public String getProjectNo() {
		return projectNo;
	}

	public void setProjectNo(String projectNo) {
		this.projectNo = projectNo == null ? null : projectNo.trim();
	}

	public String getDevopsVersion() {
		return devopsVersion;
	}

	public void setDevopsVersion(String devopsVersion) {
		this.devopsVersion = devopsVersion == null ? null : devopsVersion.trim();
	}

	public Integer getIsFilter() {
		return isFilter;
	}

	public void setIsFilter(Integer isFilter) {
		this.isFilter = isFilter;
	}

	public Integer getProductType() {
		return productType;
	}

	public void setProductType(Integer productType) {
		this.productType = productType;
	}

	public Date getOperatorDate() {
		return operatorDate;
	}

	public void setOperatorDate(Date operatorDate) {
		this.operatorDate = operatorDate;
	}

	public String getRemark() {
		return remark;
	}

	public void setRemark(String remark) {
		this.remark = remark == null ? null : remark.trim();
	}
}