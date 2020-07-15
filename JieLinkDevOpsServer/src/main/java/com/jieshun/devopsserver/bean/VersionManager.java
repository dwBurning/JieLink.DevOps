package com.jieshun.devopsserver.bean;

import java.util.Date;

public class VersionManager {
    private Integer id;

    private String workOrderNo;

    private String standVersion;

    private Date compileDate;

    private String versionDescribe;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getWorkOrderNo() {
        return workOrderNo;
    }

    public void setWorkOrderNo(String workOrderNo) {
        this.workOrderNo = workOrderNo == null ? null : workOrderNo.trim();
    }

    public String getStandVersion() {
        return standVersion;
    }

    public void setStandVersion(String standVersion) {
        this.standVersion = standVersion == null ? null : standVersion.trim();
    }

    public Date getCompileDate() {
        return compileDate;
    }

    public void setCompileDate(Date compileDate) {
        this.compileDate = compileDate;
    }

    public String getVersionDescribe() {
        return versionDescribe;
    }

    public void setVersionDescribe(String versionDescribe) {
        this.versionDescribe = versionDescribe == null ? null : versionDescribe.trim();
    }
}