package com.jieshun.devopsserver.utils;

import java.util.LinkedList;
import java.util.Queue;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.scheduling.annotation.Async;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import com.jieshun.devopsserver.bean.ApplyInfo;

@Component
@Async
public class SendEmailTask {

	private static final Logger log = LoggerFactory.getLogger(SendEmailTask.class);

	private static final Object lock = new Object();

	@Autowired
	private JavaMailSender javaMailSender;

	static Queue<SimpleMailMessage> queue = new LinkedList<SimpleMailMessage>();

	public static void Enqueue(SimpleMailMessage mailMessage) {
		synchronized (lock) {
			queue.offer(mailMessage);
		}
	}

	/**
	 * 设置每5秒执行一次
	 */
	@Scheduled(cron = "*/5 * * * * ?")
	private void send() throws Exception {
		synchronized (lock) {
			while (queue.size() > 0) {
				try {
					SimpleMailMessage mailMessage = queue.poll();
					mailMessage.setFrom("deadlineweismile@foxmail.com");
					mailMessage.setSubject("JieLink运维平台推送消息");
					javaMailSender.send(mailMessage);
					log.info("发送邮件:{}\r\n给{}", mailMessage.getText(), mailMessage.getTo());
					Thread.sleep(100);
				} catch (InterruptedException e) {
					e.printStackTrace();
				}
			}
		}
	}
}
