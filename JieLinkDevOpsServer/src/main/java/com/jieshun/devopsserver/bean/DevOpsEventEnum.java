package com.jieshun.devopsserver.bean;

import java.util.HashMap;
import java.util.Map;

public enum DevOpsEventEnum {

	OUTOFMEMORY(1, "系统内存预警"), 
	CPU(2, "系统CPU预警"), 
	THREAD(3, "进程线程预警"), 
	DISKSPACE(4, "系统磁盘预警"), 
	PROCESSOUTOFMEMORY(5, "进程内存溢出"),
	PROCESSCPU(6, "进程CPU预警");

	private int code;

	private String message;

	public int getCode() {
		return code;
	}

	public void setCode(int code) {
		this.code = code;
	}

	public String getMessage() {
		return message;
	}

	public void setMessage(String message) {
		this.message = message;
	}

	private DevOpsEventEnum(int code, String message) {
		this.code = code;
		this.message = message;
	}

	/**
	 * key:枚举值name，即业务标识。 value：枚举值对象。
	 */
	private static Map<Integer, DevOpsEventEnum> mapping = new HashMap<>(16);

	static {
		for (DevOpsEventEnum eventEnum : values()) {
			mapping.put(eventEnum.getCode(), eventEnum);
		}
	}

	public static DevOpsEventEnum getDevOpsEventEnumByCode(Integer code) {
		return mapping.get(code);
	}
}
