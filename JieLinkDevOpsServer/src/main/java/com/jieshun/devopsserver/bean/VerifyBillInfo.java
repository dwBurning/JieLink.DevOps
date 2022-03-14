package com.jieshun.devopsserver.bean;

import java.util.Date;

public class VerifyBillInfo {
    private Integer id;

    private String publisherName;

    private String projectShoperNo;

    private String projectNo;

    private String projectName;

    private String projectBigVersion;

    private String projectVersion;

    private Integer projectIsNonstandard;

    private String projectRemote;

    private String projectRemotePassword;

    private String projectRemark;

    private String projectTask;

    private Date addDate;

    private Integer status;

    private Integer emergency;

    private Integer isdelete;

    private Date finishDate;

    private String uploadfilename;

    private String autosql;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getPublisherName() {
        return publisherName;
    }

    public void setPublisherName(String publisherName) {
        this.publisherName = publisherName == null ? null : publisherName.trim();
    }

    public String getProjectShoperNo() {
        return projectShoperNo;
    }

    public void setProjectShoperNo(String projectShoperNo) {
        this.projectShoperNo = projectShoperNo == null ? null : projectShoperNo.trim();
    }

    public String getProjectNo() {
        return projectNo;
    }

    public void setProjectNo(String projectNo) {
        this.projectNo = projectNo == null ? null : projectNo.trim();
    }

    public String getProjectName() {
        return projectName;
    }

    public void setProjectName(String projectName) {
        this.projectName = projectName == null ? null : projectName.trim();
    }

    public String getProjectBigVersion() {
        return projectBigVersion;
    }

    public void setProjectBigVersion(String projectBigVersion) {
        this.projectBigVersion = projectBigVersion == null ? null : projectBigVersion.trim();
    }

    public String getProjectVersion() {
        return projectVersion;
    }

    public void setProjectVersion(String projectVersion) {
        this.projectVersion = projectVersion == null ? null : projectVersion.trim();
    }

    public Integer getProjectIsNonstandard() {
        return projectIsNonstandard;
    }

    public void setProjectIsNonstandard(Integer projectIsNonstandard) {
        this.projectIsNonstandard = projectIsNonstandard;
    }

    public String getProjectRemote() {
        return projectRemote;
    }

    public void setProjectRemote(String projectRemote) {
        this.projectRemote = projectRemote == null ? null : projectRemote.trim();
    }

    public String getProjectRemotePassword() {
        return projectRemotePassword;
    }

    public void setProjectRemotePassword(String projectRemotePassword) {
        this.projectRemotePassword = projectRemotePassword == null ? null : projectRemotePassword.trim();
    }

    public String getProjectRemark() {
        return projectRemark;
    }

    public void setProjectRemark(String projectRemark) {
        this.projectRemark = projectRemark == null ? null : projectRemark.trim();
    }

    public String getProjectTask() {
        return projectTask;
    }

    public void setProjectTask(String projectTask) {
        this.projectTask = projectTask == null ? null : projectTask.trim();
    }

    public Date getAddDate() {
        return addDate;
    }

    public void setAddDate(Date addDate) {
        this.addDate = addDate;
    }

    public Integer getStatus() {
        return status;
    }

    public void setStatus(Integer status) {
        this.status = status;
    }

    public Integer getEmergency() {
        return emergency;
    }

    public void setEmergency(Integer emergency) {
        this.emergency = emergency;
    }

    public Integer getIsdelete() {
        return isdelete;
    }

    public void setIsdelete(Integer isdelete) {
        this.isdelete = isdelete;
    }

    public Date getFinishDate() {
        return finishDate;
    }

    public void setFinishDate(Date finishDate) {
        this.finishDate = finishDate;
    }

    public String getUploadfilename() {
        return uploadfilename;
    }

    public void setUploadfilename(String uploadfilename) {
        this.uploadfilename = uploadfilename == null ? null : uploadfilename.trim();
    }

    public String getAutosql() {
        return autosql;
    }

    public void setAutosql(String autosql) {
        this.autosql = autosql == null ? null : autosql.trim();
    }
}