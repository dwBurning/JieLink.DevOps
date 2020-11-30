package com.jieshun.devopsserver.bean;

public class ProjectInfo {
    private Integer id;

    private String projectNo;

    private String devopsVersion;

    private Integer isFilter;
    
    private Integer productType;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
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
}