package com.jieshun.devopsserver.service.impl;

import org.apache.commons.io.IOUtils;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.autoconfigure.condition.ConditionalOnBean;
import org.springframework.stereotype.Component;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStream;

public class DiskFileStore implements FileStore {

    @Value("${fileserver.savepath}")
    String fileSavePath;
    final String DEFAULT_BUCKTNAME_PREFIX="default/";
    @Override
    public boolean upload(String relativePath, String fileName, InputStream inStream) throws Exception {

        String dirPath = "";
        if (fileSavePath.endsWith("/") || fileSavePath.endsWith("\\")) {
            dirPath = fileSavePath + relativePath;
        } else {
            dirPath = fileSavePath + File.separator + relativePath;
        }
        File dir = new File(dirPath);
        if (!dir.exists()) {
            dir.mkdirs();
        }
        String fullPath = dirPath + File.separator + fileName;
        System.out.println("保存文件到磁盘..." + fullPath);
        File file = new File(fullPath);
        FileOutputStream outStream = new FileOutputStream(file);
        IOUtils.copy(inStream, outStream);
        outStream.close();
        //inStream.close();//inStream不用管，外层释放吧

        return true;
    }

    @Override
    public InputStream download(String requestUrl) throws Exception {

        String fullPath = "";
        if (fileSavePath.endsWith("/") || fileSavePath.endsWith("\\")) {
            fullPath = fileSavePath + requestUrl;
        } else {
            fullPath = fileSavePath + File.separator + requestUrl;
        }
        System.out.println("从磁盘获取文件..." + fullPath);
        File file = new File(fullPath);
        if (file.exists() && file.isFile()) {

            return new FileInputStream(file);

        } else {
            //如果是default文件夹，尝试从根目录再找下(兼容)
            throw new Exception("文件不存在：" + fullPath);
        }

    }

}
