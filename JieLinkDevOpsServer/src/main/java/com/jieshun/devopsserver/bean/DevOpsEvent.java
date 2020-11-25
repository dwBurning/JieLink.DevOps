package com.jieshun.devopsserver.bean;

import java.util.Date;

import com.alibaba.fastjson.annotation.JSONField;

public class DevOpsEvent {
    private Integer id;

    private Integer eventType;

    private String projectNo;

    private String remoteAccount;

    private String remotePassword;

    private String contactName;

    private String contactPhone;

    @JSONField(format = "yyyy-MM-dd HH:mm:ss")
    private Date operatorDate;

    private Integer isProcessed;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public Integer getEventType() {
        return eventType;
    }

    public void setEventType(Integer eventType) {
        this.eventType = eventType;
    }

    public String getProjectNo() {
        return projectNo;
    }

    public void setProjectNo(String projectNo) {
        this.projectNo = projectNo == null ? null : projectNo.trim();
    }

    public String getRemoteAccount() {
        return remoteAccount;
    }

    public void setRemoteAccount(String remoteAccount) {
        this.remoteAccount = remoteAccount == null ? null : remoteAccount.trim();
    }

    public String getRemotePassword() {
        return remotePassword;
    }

    public void setRemotePassword(String remotePassword) {
        this.remotePassword = remotePassword == null ? null : remotePassword.trim();
    }

    public String getContactName() {
        return contactName;
    }

    public void setContactName(String contactName) {
        this.contactName = contactName == null ? null : contactName.trim();
    }

    public String getContactPhone() {
        return contactPhone;
    }

    public void setContactPhone(String contactPhone) {
        this.contactPhone = contactPhone == null ? null : contactPhone.trim();
    }

    public Date getOperatorDate() {
        return operatorDate;
    }

    public void setOperatorDate(Date operatorDate) {
        this.operatorDate = operatorDate;
    }

    public Integer getIsProcessed() {
        return isProcessed;
    }

    public void setIsProcessed(Integer isProcessed) {
        this.isProcessed = isProcessed;
    }
}