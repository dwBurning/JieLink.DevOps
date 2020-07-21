package com.jieshun.devopsserver.mapper;

import com.jieshun.devopsserver.bean.DevOpsEvent;
import com.jieshun.devopsserver.bean.DevOpsEventExample;
import java.util.List;
import org.apache.ibatis.annotations.Param;

public interface DevOpsEventMapper {
    int countByExample(DevOpsEventExample example);

    int deleteByExample(DevOpsEventExample example);

    int deleteByPrimaryKey(Integer id);

    int insert(DevOpsEvent record);

    int insertSelective(DevOpsEvent record);

    List<DevOpsEvent> selectByExample(DevOpsEventExample example);

    DevOpsEvent selectByPrimaryKey(Integer id);

    int updateByExampleSelective(@Param("record") DevOpsEvent record, @Param("example") DevOpsEventExample example);

    int updateByExample(@Param("record") DevOpsEvent record, @Param("example") DevOpsEventExample example);

    int updateByPrimaryKeySelective(DevOpsEvent record);

    int updateByPrimaryKey(DevOpsEvent record);
}