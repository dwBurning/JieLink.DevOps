package com.jieshun.devopsserver.service;

import com.jieshun.devopsserver.service.impl.FileStore;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Service;

import java.io.InputStream;

@Service
public class FileStoreService {

    @Autowired
    FileStore fileStore;

    public boolean upload(String relativePath, String fileName, InputStream inStream) throws Exception {

        boolean ok = fileStore.upload(relativePath, fileName, inStream);
        //做其他事
        return ok;
    }

    public InputStream download(String requestUrl) throws Exception {

        return fileStore.download(requestUrl);
    }
}
