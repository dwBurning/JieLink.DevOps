package com.jieshun.devopsserver.controller;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.jieshun.devopsserver.bean.ReturnData;
import com.jieshun.devopsserver.bean.ReturnStateEnum;
import com.jieshun.devopsserver.bean.SysUser;
import com.jieshun.devopsserver.service.SysUserService;
import com.jieshun.devopsserver.utils.Util;

@RestController
@RequestMapping(value = "/user")
public class SysUserController {

	@Autowired
	SysUserService sysUserService;

	/**
	 * 获取当前用户
	 * 
	 * @return 用户名
	 */
	@RequestMapping(value = "/currentUserName")
	public String currentUserName() {
		return Util.getCurrentUser().getUsername();
	}

	/**
	 * 获取所有用户信息
	 * 
	 * @return 用户名
	 */
	@RequestMapping(value = "/getSysUsers", method = RequestMethod.GET)
	public List<SysUser> getSysUsers(String userName) {
		return sysUserService.getSysUsers(userName);
	}

	/**
	 * 新增用户
	 * 
	 * @return
	 */
	@RequestMapping(value = "/addSysUser", method = RequestMethod.POST)
	public ReturnData addSysUser(String userName, String password, String cellPhone, String email) {
		SysUser sysUser = new SysUser();
		sysUser.setUserName(userName);
		sysUser.setPassword(password);
		if (cellPhone == null) {
			cellPhone = "";
		}
		sysUser.setCellPhone(cellPhone);
		sysUser.setEmail(email);
		int result = sysUserService.addSysUser(sysUser);

		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), "用户名已存在！");
		}
	}
}
