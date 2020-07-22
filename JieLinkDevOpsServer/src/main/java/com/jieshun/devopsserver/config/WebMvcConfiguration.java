package com.jieshun.devopsserver.config;

import com.alibaba.fastjson.serializer.SerializerFeature;
import com.alibaba.fastjson.support.config.FastJsonConfig;
import com.alibaba.fastjson.support.spring.FastJsonHttpMessageConverter;
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
import org.springframework.boot.autoconfigure.http.HttpMessageConverters;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.converter.HttpMessageConverter;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

@EnableConfigurationProperties(MinioProperties.class)
@Configuration
public class WebMvcConfiguration implements WebMvcConfigurer {

	/**
	 * fileserver.type=minio时才生效
	 *
	 * @return
	 */
	@ConditionalOnProperty(name = "fileserver.type", havingValue = "minio")
	@Bean
	public MinioFileStore minioFileStore() {
		return new MinioFileStore();
	}

	@Autowired
	private MinioProperties minioProperties;

	/**
	 * 全局CORS设置
	 *
	 * @return
	 */
	@Bean
	public WebMvcConfigurer corsConfigurer() {
		return new WebMvcConfigurer() {
			@Override
			public void addCorsMappings(CorsRegistry registry) {
				registry.addMapping("/**").allowedOrigins("*")
						.allowedMethods("GET", "HEAD", "POST", "PUT", "DELETE", "OPTIONS").allowCredentials(false)
						.maxAge(3600);

			}
		};
	}

	/**
	 * 注册mini客户端
	 *
	 * @return
	 * @throws InvalidPortException
	 * @throws InvalidEndpointException
	 */

	@Bean
	public MinioClient minioClient() throws InvalidPortException, InvalidEndpointException {

		return MinioClient.builder().endpoint(minioProperties.getEndpoint())
				.credentials(minioProperties.getAccesskey(), minioProperties.getSecretKey()).build();
	}

	@Bean
	public HttpMessageConverters fastJsonHttpMessageConverters() {
		// 1.定义一个converters转换消息的对象
		FastJsonHttpMessageConverter fastConverter = new FastJsonHttpMessageConverter();
		// 2.添加fastjson的配置信息，比如: 是否需要格式化返回的json数据
		FastJsonConfig fastJsonConfig = new FastJsonConfig();
		fastJsonConfig.setSerializerFeatures(SerializerFeature.PrettyFormat, SerializerFeature.WriteNullStringAsEmpty,
				SerializerFeature.WriteNullBooleanAsFalse, SerializerFeature.WriteNullNumberAsZero,
				SerializerFeature.WriteNullListAsEmpty);
		// 3.在converter中添加配置信息
		fastConverter.setFastJsonConfig(fastJsonConfig);
		// 4.将converter赋值给HttpMessageConverter
		HttpMessageConverter<?> converter = fastConverter;
		// 5.返回HttpMessageConverters对象
		return new HttpMessageConverters(converter);
	}

	/**
	 * 容器中没有这个bean时才注入
	 *
	 * @return
	 */
	@ConditionalOnMissingBean(FileStore.class)
	@Bean
	public FileStore diskFileStore() {
		return new DiskFileStore();
	}

}