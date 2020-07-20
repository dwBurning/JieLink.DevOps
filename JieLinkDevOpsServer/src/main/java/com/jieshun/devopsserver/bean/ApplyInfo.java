package com.jieshun.devopsserver.bean;

import java.util.Date;

public class ApplyInfo {
    private Integer id;

    private String workOrderNo;

    private String jobNumber;

    private String name;

    private String cellPhone;

    private String email;

    private Date applyDate;

    private Integer isSendEmail;
    
    private String DownloadMsg;

    public String getDownloadMsg() {
		return DownloadMsg;
	}

	public void setDownloadMsg(String downloadMsg) {
		DownloadMsg = downloadMsg;
	}

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

    public String getJobNumber() {
        return jobNumber;
    }

    public void setJobNumber(String jobNumber) {
        this.jobNumber = jobNumber == null ? null : jobNumber.trim();
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name == null ? null : name.trim();
    }

    public String getCellPhone() {
        return cellPhone;
    }

    public void setCellPhone(String cellPhone) {
        this.cellPhone = cellPhone == null ? null : cellPhone.trim();
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email == null ? null : email.trim();
    }

    public Date getApplyDate() {
        return applyDate;
    }

    public void setApplyDate(Date applyDate) {
        this.applyDate = applyDate;
    }

    public Integer getIsSendEmail() {
        return isSendEmail;
    }

    public void setIsSendEmail(Integer isSendEmail) {
        this.isSendEmail = isSendEmail;
    }
}