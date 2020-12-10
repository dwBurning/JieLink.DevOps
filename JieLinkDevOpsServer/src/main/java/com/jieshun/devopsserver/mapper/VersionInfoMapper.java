package com.jieshun.devopsserver.mapper;

import com.jieshun.devopsserver.bean.VersionInfo;
import com.jieshun.devopsserver.bean.VersionInfoExample;
import java.util.List;
import org.apache.ibatis.annotations.Param;

public interface VersionInfoMapper {
    int countByExample(VersionInfoExample example);

    int deleteByExample(VersionInfoExample example);

    int deleteByPrimaryKey(Integer id);

    int insert(VersionInfo record);

    int insertSelective(VersionInfo record);

    List<VersionInfo> selectByExample(VersionInfoExample example);

    VersionInfo selectByPrimaryKey(Integer id);

    int updateByExampleSelective(@Param("record") VersionInfo record, @Param("example") VersionInfoExample example);

    int updateByExample(@Param("record") VersionInfo record, @Param("example") VersionInfoExample example);

    int updateByPrimaryKeySelective(VersionInfo record);

    int updateByPrimaryKey(VersionInfo record);
}