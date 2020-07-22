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
		SimpleMailMessage message = new SimpleMailMessage();
		message.setTo(applyInfo.getEmail());
		StringBuilder emailTextString = new StringBuilder();
		emailTextString.append("工号：").append(applyInfo.getJobNumber()).append("\r\n");
		emailTextString.append("姓名：").append(applyInfo.getName()).append("\r\n");
		emailTextString.append("工单：").append(applyInfo.getWorkOrderNo()).append("\r\n");
		emailTextString.append(versionInfo.getDownloadMsg());
		message.setText(emailTextString.toString());
		SendEmailTask.Enqueue(message);
		return result;
	}
}
