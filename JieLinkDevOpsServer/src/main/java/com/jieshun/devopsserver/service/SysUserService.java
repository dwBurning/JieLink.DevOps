package com.jieshun.devopsserver.service;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.jieshun.devopsserver.bean.SysUser;
import com.jieshun.devopsserver.bean.SysUserExample;
import com.jieshun.devopsserver.mapper.SysUserMapper;

@Service
public class SysUserService {

	@Autowired
	SysUserMapper sysUserMapper;

	
	public List<SysUser> getAllUser() {
		SysUserExample example = new SysUserExample();
		return sysUserMapper.selectByExample(example);
	}
}
