package com.jieshun.devopsserver.mapper;

import com.jieshun.devopsserver.bean.VersionManager;
import com.jieshun.devopsserver.bean.VersionManagerExample;
import java.util.List;
import org.apache.ibatis.annotations.Param;

public interface VersionManagerMapper {
    int countByExample(VersionManagerExample example);

    int deleteByExample(VersionManagerExample example);

    int deleteByPrimaryKey(Integer id);

    int insert(VersionManager record);

    int insertSelective(VersionManager record);

    List<VersionManager> selectByExampleWithBLOBs(VersionManagerExample example);

    List<VersionManager> selectByExample(VersionManagerExample example);

    VersionManager selectByPrimaryKey(Integer id);

    int updateByExampleSelective(@Param("record") VersionManager record, @Param("example") VersionManagerExample example);

    int updateByExampleWithBLOBs(@Param("record") VersionManager record, @Param("example") VersionManagerExample example);

    int updateByExample(@Param("record") VersionManager record, @Param("example") VersionManagerExample example);

    int updateByPrimaryKeySelective(VersionManager record);

    int updateByPrimaryKeyWithBLOBs(VersionManager record);

    int updateByPrimaryKey(VersionManager record);
}