package com.jieshun.devopsserver.controller;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.jieshun.devopsserver.utils.Util;

@RestController
@RequestMapping(value = "/user")
public class SysUserController {

	
	/**
	 * 获取当前用户
	 * 
	 * @return 用户名
	 */
	@RequestMapping(value = "/currentUserName")
	public String currentUserName() {
		return Util.getCurrentUser().getUsername();
	}
}
