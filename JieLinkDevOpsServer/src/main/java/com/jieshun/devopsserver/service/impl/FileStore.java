package com.jieshun.devopsserver.service.impl;

import io.minio.errors.*;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.util.*;

public interface FileStore {

    boolean upload(String relativePath, String fileName, InputStream inStream) throws Exception;

    InputStream download(String requestUrl) throws Exception;
}
