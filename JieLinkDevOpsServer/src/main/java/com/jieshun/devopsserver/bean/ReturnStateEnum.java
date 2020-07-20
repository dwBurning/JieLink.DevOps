package com.jieshun.devopsserver.bean;

/**
 * 通用状态返回对象
 * @author Administrator
 *
 */
public enum ReturnStateEnum  {

	SUCCESS(0,"处理成功"),
	
	FAILD(1,"处理失败");
	
	/**
	 * 状态值
	 */
	private int state;
	
	/**
	 * 状态描述
	 */
	private String message;

	public int getState() {
		return state;
	}


	public String getMessage() {
		return message;
	}

	private ReturnStateEnum(int state, String message) {
		this.state = state;
		this.message = message;
	}
}
