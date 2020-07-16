package com.jieshun.devopsserver.mapper;

import com.jieshun.devopsserver.bean.ApplyInfo;
import com.jieshun.devopsserver.bean.ApplyInfoExample;
import java.util.List;
import org.apache.ibatis.annotations.Param;

public interface ApplyInfoMapper {
    int countByExample(ApplyInfoExample example);

    int deleteByExample(ApplyInfoExample example);

    int deleteByPrimaryKey(Integer id);

    int insert(ApplyInfo record);

    int insertSelective(ApplyInfo record);

    List<ApplyInfo> selectByExample(ApplyInfoExample example);

    ApplyInfo selectByPrimaryKey(Integer id);

    int updateByExampleSelective(@Param("record") ApplyInfo record, @Param("example") ApplyInfoExample example);

    int updateByExample(@Param("record") ApplyInfo record, @Param("example") ApplyInfoExample example);

    int updateByPrimaryKeySelective(ApplyInfo record);

    int updateByPrimaryKey(ApplyInfo record);
}