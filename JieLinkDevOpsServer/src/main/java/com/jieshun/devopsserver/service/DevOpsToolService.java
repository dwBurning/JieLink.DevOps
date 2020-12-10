package com.jieshun.devopsserver.service;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.jieshun.devopsserver.bean.DevOpsProductExample.Criteria;
import com.jieshun.devopsserver.bean.DevOpsProduct;
import com.jieshun.devopsserver.bean.DevOpsProductExample;
import com.jieshun.devopsserver.bean.PageSet;
import com.jieshun.devopsserver.bean.VersionInfo;
import com.jieshun.devopsserver.mapper.DevOpsProductMapper;

@Service
public class DevOpsToolService {

	@Autowired
	DevOpsProductMapper devOpsProductMapper;
	
	/**
	 * 根据分页查询版本信息
	 * 
	 * @param version 版本号
	 * @param start   起始索引
	 * @param end     结束索引
	 * @return 分页对象
	 */
	public PageSet<DevOpsProduct> getDevOpsProductWithPages(String version, int start, int end) {
		String limitString = String.format("operator_date limit %d,%d", start, end - start);
		DevOpsProductExample example = new DevOpsProductExample();
		Criteria criteria = example.createCriteria();
		criteria.andIsDeletedEqualTo(0);
		if (version != null && version != "") {

			criteria.andProductVersionLike("%" + version + "%");
		}
		int total = devOpsProductMapper.countByExample(example);
		example.setOrderByClause(limitString);
		List<DevOpsProduct> devOpsProducts = devOpsProductMapper.selectByExample(example);
		PageSet<DevOpsProduct> pageSet = new PageSet<>();
		pageSet.setItems(devOpsProducts);
		pageSet.setTotal(total);
		return pageSet;
	}
	
	
	public int addDevOpsProduct(DevOpsProduct devOpsProduct) {
		return devOpsProductMapper.insertSelective(devOpsProduct);
	}
	
	
	/**
	 * 根据Id删除版本信息
	 * 
	 * @param id
	 * @return
	 */
	public int deleteDevOpsProductById(int id) {
		DevOpsProduct devOpsTool = new DevOpsProduct();
		devOpsTool.setId(id);
		devOpsTool.setIsDeleted(1);
		return devOpsProductMapper.updateByPrimaryKeySelective(devOpsTool);
	}
	
	
	public DevOpsProduct getTheLastVersion(int productType) {
		String limitString = "operator_date desc limit 1";
		DevOpsProductExample example = new DevOpsProductExample();
		Criteria criteria = example.createCriteria();
		criteria.andProductTypeEqualTo(productType);
		criteria.andIsDeletedEqualTo(0);
		example.setOrderByClause(limitString);
		List<DevOpsProduct> devOpsTools = devOpsProductMapper.selectByExample(example);
		if (devOpsTools.size()>0) {
			return devOpsTools.get(0);
		}
		return null;
	}
}
