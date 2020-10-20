package com.jieshun.devopsserver.bean;

import java.util.Date;
import java.util.List;

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

    @JSONField(format = "yyyy-MM-dd HH:mm:ss")
    private Date operatorDate;

    private Integer isDeleted;

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

    public Date getOperatorDate() {
        return operatorDate;
    }

    public void setOperatorDate(Date operatorDate) {
        this.operatorDate = operatorDate;
    }

    public Integer getIsDeleted() {
        return isDeleted;
    }

    public void setIsDeleted(Integer isDeleted) {
        this.isDeleted = isDeleted;
    }

    /**
     * 下载次数
     */
    private Integer downloadCount;

    public Integer getDownloadCount() {
        return downloadCount;
    }

    public void setDownloadCount(Integer downloadCount) {
        this.downloadCount = downloadCount;
    }

    /**
     * 补丁下载情况
     */
    private List<ApplyInfo> children;

    public List<ApplyInfo> getChildren() {
        return children;
    }

    public void setChildren(List<ApplyInfo> applyInfoList) {
        this.children = applyInfoList;
    }

    private boolean hasChildren;

    public boolean getHasChildren() {
        return hasChildren;
    }

    public void setHasChildren(boolean hasChildren) {
        this.hasChildren = hasChildren;
    }

}