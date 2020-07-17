package com.jieshun.devopsserver.service.impl;

import io.minio.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnMissingBean;
import org.springframework.boot.autoconfigure.condition.ConditionalOnMissingClass;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Component;
import org.springframework.util.StringUtils;

import java.io.*;
import java.util.*;


public class MinioFileStore implements FileStore {

    @Autowired
    MinioClient minioClient;

    @Override
    public boolean upload(String relativePath, String fileName, InputStream inStream) throws Exception {

        List<String> pathItems = new ArrayList<>(Arrays.asList(relativePath.split("/")));
        pathItems.removeIf(x -> StringUtils.isEmpty(x));


        //取出bucketName
        String bucketName = "default";
        if (pathItems.size() > 0) {
            bucketName = pathItems.get(0);
            pathItems.remove(0);//移除bucketName，剩下的都是相对路径了
        }
        System.out.println("bucketName:" + bucketName);
        //剩下的路径作为相对路径拼在一起
        String objectName = fileName;
        if (pathItems.size() > 0) {
            objectName = StringUtils.arrayToDelimitedString(pathItems.toArray(), "/") + "/" + fileName;
        }
        System.out.println("objectName:" + objectName);
        //如果bucketName不存在则创建
        try {
            if (!minioClient.bucketExists(BucketExistsArgs.builder().bucket(bucketName).build())) {

                minioClient.makeBucket(
                        MakeBucketArgs.builder()
                                .bucket(bucketName)
                                .build());
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        try {
            PutObjectArgs args = PutObjectArgs.builder()
                    .bucket(bucketName)
                    .object(objectName)
                    .stream(inStream, inStream.available(), -1)
                    .build();
            minioClient.putObject(args);
        } catch (Exception e) {
            throw new Exception("上传文件错误：" + e.getMessage());
        }
        return true;
    }

    @Override
    public InputStream download(String requestUrl) throws Exception {

        List<String> pathItems = new ArrayList<>(Arrays.asList(requestUrl.split("/")));
        pathItems.removeIf(x -> StringUtils.isEmpty(x));
        String fileName = pathItems.get(pathItems.size() - 1);
        pathItems.remove(pathItems.size() - 1);//移除fileName
        //取出bucketName
        String bucketName = "default";
        if (pathItems.size() > 0) {
            bucketName = pathItems.get(0);
            pathItems.remove(0);//移除bucketName，剩下的都是相对路径了
        }
        System.out.println("bucketName:" + bucketName);
        System.out.println(pathItems);
        //剩下的路径作为相对路径拼在一起
        String objectName = fileName;
        if (pathItems.size() > 0) {
            objectName = StringUtils.arrayToDelimitedString(pathItems.toArray(), "/") + "/" + fileName;
        }
        System.out.println("objectName:" + objectName);

        try {
            return minioClient.getObject(GetObjectArgs
                    .builder()
                    .bucket(bucketName)
                    .object(objectName)
                    .build());
        } catch (Exception e) {
            throw new Exception("下载文件失败:" + e.getMessage());

        }


    }
}
