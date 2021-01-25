package com.jieshun.devopsserver.mapper;

import com.jieshun.devopsserver.bean.DevOpsEventConcern;
import com.jieshun.devopsserver.bean.DevOpsEventConcernExample;
import java.util.List;
import org.apache.ibatis.annotations.Param;

public interface DevOpsEventConcernMapper {
    int countByExample(DevOpsEventConcernExample example);

    int deleteByExample(DevOpsEventConcernExample example);

    int deleteByPrimaryKey(Integer id);

    int insert(DevOpsEventConcern record);

    int insertSelective(DevOpsEventConcern record);

    List<DevOpsEventConcern> selectByExample(DevOpsEventConcernExample example);

    DevOpsEventConcern selectByPrimaryKey(Integer id);

    int updateByExampleSelective(@Param("record") DevOpsEventConcern record, @Param("example") DevOpsEventConcernExample example);

    int updateByExample(@Param("record") DevOpsEventConcern record, @Param("example") DevOpsEventConcernExample example);

    int updateByPrimaryKeySelective(DevOpsEventConcern record);

    int updateByPrimaryKey(DevOpsEventConcern record);
}