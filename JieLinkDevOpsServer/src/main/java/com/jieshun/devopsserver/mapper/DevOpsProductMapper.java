package com.jieshun.devopsserver.mapper;

import com.jieshun.devopsserver.bean.DevOpsProduct;
import com.jieshun.devopsserver.bean.DevOpsProductExample;
import java.util.List;
import org.apache.ibatis.annotations.Param;

public interface DevOpsProductMapper {
    int countByExample(DevOpsProductExample example);

    int deleteByExample(DevOpsProductExample example);

    int deleteByPrimaryKey(Integer id);

    int insert(DevOpsProduct record);

    int insertSelective(DevOpsProduct record);

    List<DevOpsProduct> selectByExample(DevOpsProductExample example);

    DevOpsProduct selectByPrimaryKey(Integer id);

    int updateByExampleSelective(@Param("record") DevOpsProduct record, @Param("example") DevOpsProductExample example);

    int updateByExample(@Param("record") DevOpsProduct record, @Param("example") DevOpsProductExample example);

    int updateByPrimaryKeySelective(DevOpsProduct record);

    int updateByPrimaryKey(DevOpsProduct record);
}