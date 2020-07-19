package com.jieshun.devopsserver.utils;

import java.util.LinkedList;
import java.util.Queue;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import com.jieshun.devopsserver.bean.ApplyInfo;

@Component
public class SendEmailTask {

	private static final Logger log = LoggerFactory.getLogger(SendEmailTask.class);

	private static final Object lock = new Object();

	@Autowired
	private JavaMailSender javaMailSender;

	static Queue<ApplyInfo> queue = new LinkedList<ApplyInfo>();

	public static void Enqueue(ApplyInfo applyInfo) {
		synchronized (lock) {
			queue.offer(applyInfo);
		}
	}

	/**
	 * 设置每5秒执行一次
	 */
	@Scheduled(cron = "*/5 * * * * ?")
	private void send() {
		while (queue.size() > 0) {
			synchronized (lock) {
				ApplyInfo applyInfo = queue.poll();
				SimpleMailMessage message = new SimpleMailMessage();
				message.setFrom("deadlineweismile@foxmail.com");
				message.setTo(applyInfo.getEmail());
				message.setSubject("JieLink运维平台推送消息");
				StringBuilder emailTextString = new StringBuilder();
				emailTextString.append("工号：").append(applyInfo.getJobNumber()).append("\r\n");
				emailTextString.append("姓名：").append(applyInfo.getName()).append("\r\n");
				emailTextString.append("工单：").append(applyInfo.getWorkOrderNo()).append("\r\n");
				emailTextString.append(applyInfo.getDownloadMsg());
				message.setText(emailTextString.toString());
				javaMailSender.send(message);
				log.info("发送工单{}给{}", applyInfo.getWorkOrderNo(), applyInfo.getName());
			}
		}
	}
}
