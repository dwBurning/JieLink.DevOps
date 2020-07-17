package com.jieshun.devopsserver.config;

import com.jieshun.devopsserver.config.properties.MinioProperties;
import io.minio.MinioClient;
import io.minio.errors.InvalidEndpointException;
import io.minio.errors.InvalidPortException;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
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


}