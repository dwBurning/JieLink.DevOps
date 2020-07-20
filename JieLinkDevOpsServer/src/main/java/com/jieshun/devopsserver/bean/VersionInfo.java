package com.jieshun.devopsserver.bean;

import java.util.Date;

import com.alibaba.fastjson.annotation.JSONField;

public class VersionInfo {
    private Integer id;

    private String workOrderNo;

    private String standVersion;

    private Integer versionType;

    @JSONField(format = "yyyy-MM-dd HH:mm:ss")
    private Date compileDate;

    private String versionDescribe;

    private String downloadMsg;

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

    public Integer getVersionType() {
        return versionType;
    }

    public void setVersionType(Integer versionType) {
        this.versionType = versionType;
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

    public String getDownloadMsg() {
        return downloadMsg;
    }

    public void setDownloadMsg(String downloadMsg) {
        this.downloadMsg = downloadMsg == null ? null : downloadMsg.trim();
    }
}