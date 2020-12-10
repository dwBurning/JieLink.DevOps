<template>
    <div>
        <div>
            <el-upload
            class="upload-demo"
            action="http://localhost:8080/upload/logs/20200716"
            :on-preview="handlePreview"
            :on-remove="handleRemove"
            :before-remove="beforeRemove"
            multiple
            :limit="3"
            :on-success="handleSuccess"
            :file-list="fileList">
             <el-button size="small" type="primary">点击上传</el-button>
            <!-- <div slot="tip" class="el-upload__tip">只能上传jpg/png文件，且不超过500kb</div> -->
            </el-upload>
        </div>
    </div>
</template>

<script>
export default {
  name: 'App',
  data() {
     return {
       Height: 0,
       fileList:[]
     }
  },
  methods: {
    handleRemove(file, fileList) {
      console.log(file,fileList);
    },
    handlePreview(file) {
      console.log(file);
    },

    beforeRemove(file, fileList) {
      return this.$confirm(`确定移除 ${ file.name }？`);
    },
    handleSuccess(response, file, fileList){
      console.log('上传成功：下载url='+response);
    }
  }
}
</script>