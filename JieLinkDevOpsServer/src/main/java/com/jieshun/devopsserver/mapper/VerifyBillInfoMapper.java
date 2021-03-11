package com.jieshun.devopsserver.mapper;

import com.jieshun.devopsserver.bean.VerifyBillInfo;
import com.jieshun.devopsserver.bean.VerifyBillInfoExample;
import java.util.List;
import org.apache.ibatis.annotations.Param;

public interface VerifyBillInfoMapper {
    int countByExample(VerifyBillInfoExample example);

    int deleteByExample(VerifyBillInfoExample example);

    int deleteByPrimaryKey(Integer id);

    int insert(VerifyBillInfo record);

    int insertSelective(VerifyBillInfo record);

    List<VerifyBillInfo> selectByExampleWithBLOBs(VerifyBillInfoExample example);

    List<VerifyBillInfo> selectByExample(VerifyBillInfoExample example);

    VerifyBillInfo selectByPrimaryKey(Integer id);

    int updateByExampleSelective(@Param("record") VerifyBillInfo record, @Param("example") VerifyBillInfoExample example);

    int updateByExampleWithBLOBs(@Param("record") VerifyBillInfo record, @Param("example") VerifyBillInfoExample example);

    int updateByExample(@Param("record") VerifyBillInfo record, @Param("example") VerifyBillInfoExample example);

    int updateByPrimaryKeySelective(VerifyBillInfo record);

    int updateByPrimaryKeyWithBLOBs(VerifyBillInfo record);

    int updateByPrimaryKey(VerifyBillInfo record);
}