package com.jieshun.devopsserver.utils;

import org.springframework.security.core.context.SecurityContextHolder;

import com.jieshun.devopsserver.bean.SysUser;

public class Util {

	/**
	 * 获取当前用户
	 * @return SysUser
	 */
	public static SysUser getCurrentUser() {
		SysUser user = (SysUser) SecurityContextHolder.getContext().getAuthentication().getPrincipal();
		return user;
	}
}
