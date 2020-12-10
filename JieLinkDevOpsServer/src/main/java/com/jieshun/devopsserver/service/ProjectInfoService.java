package com.jieshun.devopsserver.service;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.jieshun.devopsserver.bean.ProjectInfo;
import com.jieshun.devopsserver.bean.ProjectInfoExample;
import com.jieshun.devopsserver.bean.ProjectInfoExample.Criteria;
import com.jieshun.devopsserver.mapper.ProjectInfoMapper;

/**
 * 
 * @author 作者 ：dengwei
 * 
 * @version 创建时间：2020年11月28日 下午5:33:28
 * 
 * @Description 描述：
 * 
 */
@Service
public class ProjectInfoService {

	@Autowired
	ProjectInfoMapper projectInfoMapper;

	public int reportProjectInfo(ProjectInfo projectInfo) {
		ProjectInfo info = getProjectInfoByProjectNo(projectInfo.getProjectNo());
		if (info != null) {
			projectInfo.setIsFilter(info.getIsFilter());
			projectInfo.setId(info.getId());
			return projectInfoMapper.updateByPrimaryKeySelective(projectInfo);
		} else {
			projectInfo.setIsFilter(0);
			return projectInfoMapper.insertSelective(projectInfo);
		}
	}

	
	public int updateProjectInfo(ProjectInfo projectInfo) {
		ProjectInfo info = getProjectInfoByProjectNo(projectInfo.getProjectNo());
		if (info != null) {
			projectInfo.setId(info.getId());
			return projectInfoMapper.updateByPrimaryKeySelective(projectInfo);
		} else {
			return projectInfoMapper.insertSelective(projectInfo);
		}
	}
	
	
	public ProjectInfo getProjectInfoByProjectNo(String projectNo) {
		ProjectInfoExample example = new ProjectInfoExample();
		Criteria criteria = example.createCriteria();
		criteria.andProjectNoEqualTo(projectNo);
		List<ProjectInfo> projectInfos = projectInfoMapper.selectByExample(example);
		if (projectInfos != null && projectInfos.size() > 0) {
			return projectInfos.get(0);
		}
		return null;
	}

	public int filter(String projectNo) {
		ProjectInfo projectInfo = getProjectInfoByProjectNo(projectNo);
		if (projectInfo != null) {
			projectInfo.setIsFilter(1);
			return projectInfoMapper.updateByPrimaryKey(projectInfo);
		} else {
			ProjectInfo info = new ProjectInfo();
			info.setProjectNo(projectNo);
			info.setIsFilter(1);
			info.setDevopsVersion("V1.0.0");
			return projectInfoMapper.insert(info);
		}
	}

}
