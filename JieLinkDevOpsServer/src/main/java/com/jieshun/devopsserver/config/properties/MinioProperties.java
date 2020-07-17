package com.jieshun.devopsserver.config.properties;

import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.stereotype.Component;

@ConfigurationProperties(prefix="minio")
@Component
public class MinioProperties {
    private String endpoint;//连接url
    private String accesskey;//用户名
    private String secretKey;//密码

    public String getEndpoint() {
        return endpoint;
    }

    public void setEndpoint(String endpoint) {
        this.endpoint = endpoint;
    }

    public String getAccesskey() {
        return accesskey;
    }

    public void setAccesskey(String accesskey) {
        this.accesskey = accesskey;
    }

    public String getSecretKey() {
        return secretKey;
    }

    public void setSecretKey(String secretKey) {
        this.secretKey = secretKey;
    }
}
