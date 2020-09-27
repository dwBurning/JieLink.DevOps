package com.jieshun.devopsserver;

import org.mybatis.spring.annotation.MapperScan;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.builder.SpringApplicationBuilder;
import org.springframework.boot.web.servlet.support.SpringBootServletInitializer;
import org.springframework.scheduling.annotation.EnableAsync;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling // 开启定时任务支持
@EnableAsync //开启异步事件的支持
@MapperScan(basePackages = "com.jieshun.devopsserver.mapper")
public class DevOpsServerApplication extends SpringBootServletInitializer implements CommandLineRunner {

	private static final Logger log = LoggerFactory.getLogger(DevOpsServerApplication.class);

	public static void main(String[] args) {
		try {
			SpringApplication.run(DevOpsServerApplication.class, args);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	@Override
	public void run(String... args) throws Exception {
		log.info("start");
	}

	@Override
	protected SpringApplicationBuilder configure(SpringApplicationBuilder builder) {
		return builder.sources(this.getClass());
	}

}
