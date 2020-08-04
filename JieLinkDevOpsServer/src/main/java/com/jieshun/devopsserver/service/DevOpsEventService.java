package com.jieshun.devopsserver.service;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.stereotype.Service;

import com.jieshun.devopsserver.bean.DevOpsEvent;
import com.jieshun.devopsserver.bean.DevOpsEventEnum;
import com.jieshun.devopsserver.bean.DevOpsEventExample;
import com.jieshun.devopsserver.bean.DevOpsEventExample.Criteria;
import com.jieshun.devopsserver.bean.PageSet;
import com.jieshun.devopsserver.bean.SysUser;

import com.jieshun.devopsserver.mapper.DevOpsEventMapper;
import com.jieshun.devopsserver.utils.SendEmailTask;

@Service
public class DevOpsEventService {

	@Autowired
	DevOpsEventMapper devOpsEventMapper;

	@Autowired
	SysUserService sysUserService;

	public int reportDevOpsEvent(DevOpsEvent devOpsEvent) {
		int result = devOpsEventMapper.insertSelective(devOpsEvent);
		if (result > 0) {
			List<SysUser> sysUsers = sysUserService.getSysUsers(null);
			sysUsers.forEach((user) -> {
				SimpleMailMessage message = new SimpleMailMessage();
				StringBuilder emailTextString = new StringBuilder();
				String eventMessage = DevOpsEventEnum.getDevOpsEventEnumByCode(devOpsEvent.getEventType()).getMessage();
				emailTextString.append("事件类型：").append(eventMessage).append("\r\n");
				emailTextString.append("远程账号：").append(devOpsEvent.getRemoteAccount()).append("\r\n");
				emailTextString.append("联系人姓名：").append(devOpsEvent.getContactName()).append("\r\n");
				emailTextString.append("联系人电话：").append(devOpsEvent.getContactPhone()).append("\r\n");
				message.setText(emailTextString.toString());
				message.setTo(user.getEmail());
				SendEmailTask.Enqueue(message);
			});
		}
		return result;
	}

	/**
	 * 根据分页查询版本信息
	 * 
	 * @param orderNo 工单号
	 * @param start   起始索引
	 * @param end     结束索引
	 * @return 分页对象
	 */
	public PageSet<DevOpsEvent> getDevOpsEventWithPages(int eventCode, int start, int end) {
		String limitString = String.format("operator_date desc limit %d,%d", start, end - start);
		DevOpsEventExample example = new DevOpsEventExample();
		Criteria criteria = example.createCriteria();
		if (eventCode >= 0) {
			criteria.andEventTypeEqualTo(eventCode);
		}
		int total = devOpsEventMapper.countByExample(example);
		example.setOrderByClause(limitString);
		List<DevOpsEvent> devOpsEvents = devOpsEventMapper.selectByExample(example);
		PageSet<DevOpsEvent> pageSet = new PageSet<>();
		pageSet.setItems(devOpsEvents);
		pageSet.setTotal(total);
		return pageSet;
	}

	public int processed(int id) {
		DevOpsEvent devOpsEvent = new DevOpsEvent();
		devOpsEvent.setId(id);
		devOpsEvent.setIsProcessed(1);
		return devOpsEventMapper.updateByPrimaryKeySelective(devOpsEvent);
	}
}
