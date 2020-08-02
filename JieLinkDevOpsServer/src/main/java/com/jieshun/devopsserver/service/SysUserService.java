package com.jieshun.devopsserver.service;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;

import com.jieshun.devopsserver.bean.SysUser;
import com.jieshun.devopsserver.bean.SysUserExample;
import com.jieshun.devopsserver.bean.SysUserExample.Criteria;
import com.jieshun.devopsserver.mapper.SysUserMapper;


@Service
public class SysUserService implements UserDetailsService {
	
	private static final Logger log = LoggerFactory.getLogger(SysUserService.class);

	@Autowired
	SysUserMapper sysUserMapper;
	
	@Override
	public UserDetails loadUserByUsername(String username) throws UsernameNotFoundException {
		// TODO Auto-generated method stub
		SysUserExample example = new SysUserExample();
		Criteria criteria = example.createCriteria();
		criteria.andUserNameEqualTo(username);
		List<SysUser> users = sysUserMapper.selectByExample(example);
		if (users == null || users.size() <= 0) {
			// 避免返回null，这里返回一个不含有任何值的User对象，在后期的密码比对过程中一样会验证失败
			log.info(String.format("没有找到用户名为:%s 的用户", username));
			SysUser user=new SysUser();
			user.setUserName("");
			user.setPassword("");
			return user;
		}

		SysUser systemUser = users.get(0);

		return systemUser;
	}
	
	public List<SysUser> getAllUser() {
		SysUserExample example = new SysUserExample();
		return sysUserMapper.selectByExample(example);
	}
}
