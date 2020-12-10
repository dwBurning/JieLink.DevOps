package com.jieshun.devopsserver.mapper;

import com.jieshun.devopsserver.bean.DevOpsEventFilter;
import com.jieshun.devopsserver.bean.DevOpsEventFilterExample;
import java.util.List;
import org.apache.ibatis.annotations.Param;

public interface DevOpsEventFilterMapper {
    int countByExample(DevOpsEventFilterExample example);

    int deleteByExample(DevOpsEventFilterExample example);

    int deleteByPrimaryKey(Integer id);

    int insert(DevOpsEventFilter record);

    int insertSelective(DevOpsEventFilter record);

    List<DevOpsEventFilter> selectByExample(DevOpsEventFilterExample example);

    DevOpsEventFilter selectByPrimaryKey(Integer id);

    int updateByExampleSelective(@Param("record") DevOpsEventFilter record, @Param("example") DevOpsEventFilterExample example);

    int updateByExample(@Param("record") DevOpsEventFilter record, @Param("example") DevOpsEventFilterExample example);

    int updateByPrimaryKeySelective(DevOpsEventFilter record);

    int updateByPrimaryKey(DevOpsEventFilter record);
}