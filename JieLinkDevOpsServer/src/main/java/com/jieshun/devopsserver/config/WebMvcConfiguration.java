package com.jieshun.devopsserver.config;

import com.jieshun.devopsserver.config.properties.MinioProperties;
import com.jieshun.devopsserver.service.impl.DiskFileStore;
import com.jieshun.devopsserver.service.impl.FileStore;
import com.jieshun.devopsserver.service.impl.MinioFileStore;
import io.minio.MinioClient;
import io.minio.errors.InvalidEndpointException;
import io.minio.errors.InvalidPortException;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnMissingBean;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

@EnableConfigurationProperties(MinioProperties.class)
@Configuration
public class WebMvcConfiguration {

    @Autowired
    private MinioProperties minioProperties;

    /**
     * 全局CORS设置
     * @return
     */
    @Bean
    public WebMvcConfigurer corsConfigurer(){
        return new WebMvcConfigurer(){
            @Override
            public void addCorsMappings(CorsRegistry registry){
                registry.addMapping("/**").allowedOrigins("*")
                        .allowedMethods("GET", "HEAD", "POST","PUT", "DELETE", "OPTIONS")
                        .allowCredentials(false).maxAge(3600);

            }
        };
    }

    /**
     * 注册mini客户端
     * @return
     * @throws InvalidPortException
     * @throws InvalidEndpointException
     */

    @Bean
    public MinioClient minioClient() throws InvalidPortException, InvalidEndpointException {

        return MinioClient.builder()
                .endpoint(minioProperties.getEndpoint())
                .credentials(minioProperties.getAccesskey(),minioProperties.getSecretKey())
                .build();
    }

    /**
     * fileserver.type=minio时才生效
     * @return
     */
    @ConditionalOnProperty(name = "fileserver.type",havingValue="minio")
    @Bean
    public MinioFileStore minioFileStore(){
        return new MinioFileStore();
    }

    /**
     * 容器中没有这个bean时才注入
     * @return
     */
    @ConditionalOnMissingBean(FileStore.class)
    @Bean
    public FileStore diskFileStore(){
        return new DiskFileStore();
    }
}