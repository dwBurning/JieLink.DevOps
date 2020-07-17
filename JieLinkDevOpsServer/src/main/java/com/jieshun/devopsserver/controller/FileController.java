package com.jieshun.devopsserver.controller;


import com.jieshun.devopsserver.service.FileStoreService;
import org.apache.commons.io.IOUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.util.StringUtils;


import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URLEncoder;
import java.net.URLDecoder;

@RestController
public class FileController {

    private final String UPLOAD_PREFIX = "/upload";
    private final String DOWNLOAD_PREFIX = "/download";

    @Autowired
    FileStoreService fileStoreService;

    @PostMapping("/upload/**")
    public ResponseEntity<String> upload(HttpServletRequest request, MultipartFile file) throws Exception {

        //上传路径格式为 /upload/bucktName/xxx/somepath...
        String requestUrl = URLDecoder.decode(request.getRequestURI(), request.getCharacterEncoding());
        System.out.println(requestUrl + "..." + file.getOriginalFilename());///upload/logs/20200716...云桌面账号.txt
        //去掉/upload
        requestUrl = requestUrl.substring(requestUrl.indexOf(UPLOAD_PREFIX) + UPLOAD_PREFIX.length(), requestUrl.length());///logs/2020071
        //去掉开头和结尾的/
        requestUrl = StringUtils.trimLeadingCharacter(requestUrl, '/');//logs/2020071
        requestUrl = StringUtils.trimTrailingCharacter(requestUrl, '/');

        System.out.println(requestUrl);

        boolean ok = fileStoreService.upload(requestUrl, file.getOriginalFilename(), file.getInputStream());

        return ResponseEntity.ok("上传成功");
    }

    @GetMapping("/download/**/{fileName}")
    public ResponseEntity<String> download(HttpServletRequest request, HttpServletResponse response, @PathVariable("fileName") String fileName) throws IOException {

        //下载路径格式为 /download/bucktName/xxx/somepath.../xxx.txt
        String requestUrl = URLDecoder.decode(request.getRequestURI(), request.getCharacterEncoding());
        //去掉/upload
        requestUrl = requestUrl.substring(requestUrl.indexOf(DOWNLOAD_PREFIX) + DOWNLOAD_PREFIX.length(), requestUrl.length());///logs/2020071
        //去掉开头和结尾的/
        requestUrl = StringUtils.trimLeadingCharacter(requestUrl, '/');//logs/2020071
        requestUrl = StringUtils.trimTrailingCharacter(requestUrl, '/');

        System.out.println(requestUrl);
        System.out.println(fileName);

        //获取文件流
        InputStream inStream = null;
        try {
            inStream = fileStoreService.download(requestUrl);
        } catch (Exception e) {
            System.out.println(e.getMessage());
            return ResponseEntity.notFound().build();
        }
        OutputStream outStream = response.getOutputStream();

        response.setCharacterEncoding("UTF-8");
        response.setHeader("Content-Disposition", "attachment;filename=" + URLEncoder.encode(fileName, "UTF-8"));
        //从输入写到输出
        IOUtils.copy(inStream, outStream);
        outStream.close();
        inStream.close();

        return ResponseEntity.ok("下载成功");
    }


}
