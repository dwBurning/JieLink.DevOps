package com.jieshun.devopsserver.bean;

import java.util.List;

/**
 * 分页参数设置
 * @author wei
 *
 * @param <T>
 */
public class PageSet<T> {
	
	/**
	 * 总页数
	 */
	private int total;
	
	/**
	 * 当前分页对象列表
	 */
	private List<T> items;

	public int getTotal() {
		return total;
	}

	public void setTotal(int total) {
		this.total = total;
	}

	public List<T> getItems() {
		return items;
	}

	public void setItems(List<T> items) {
		this.items = items;
	}
}
