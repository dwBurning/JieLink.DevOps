package com.jieshun.devopsserver;

import org.mybatis.spring.annotation.MapperScan;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling // 开启定时任务支持
@MapperScan(basePackages = "com.jieshun.devopsserver.mapper")
public class DevOpsServerApplication {

	public static void main(String[] args) {
		SpringApplication.run(DevOpsServerApplication.class, args);
	}

}
