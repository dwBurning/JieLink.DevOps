package com.jieshun.devopsserver.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.stereotype.Service;

import com.jieshun.devopsserver.bean.ApplyInfo;
import com.jieshun.devopsserver.bean.VersionInfo;
import com.jieshun.devopsserver.mapper.ApplyInfoMapper;
import com.jieshun.devopsserver.utils.SendEmailTask;

@Service
public class ApplyInfoService {

	@Autowired
	ApplyInfoMapper applyInfoMapper;

	@Autowired
	VersionInfoService versionInfoService;

	public int addApplyInfo(ApplyInfo applyInfo) {
		int result = applyInfoMapper.insertSelective(applyInfo);
		VersionInfo versionInfo = versionInfoService.getVersionInfoByOrderNo(applyInfo.getWorkOrderNo());
		applyInfo.setDownloadMsg(versionInfo.getDownloadMsg());
		SendEmailTask.Enqueue(applyInfo);
		return result;

	}
}
