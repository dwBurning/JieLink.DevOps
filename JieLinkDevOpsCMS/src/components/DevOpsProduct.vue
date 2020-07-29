<template>
  <div id="app">
    <Header></Header>
    <div id="v-content" v-bind:style="{minHeight: Height+'px'}">
      <el-container>
        <el-header class="report_header">
          <el-input
            placeholder="请输入版本号..."
            prefix-icon="el-icon-search"
            v-model="keywords"
            style="width: 400px"
            size="medium"
          ></el-input>
          <el-button
            type="primary"
            icon="el-icon-search"
            size="small"
            style="margin-left: 3px"
            @click="searchClick"
          >搜索</el-button>
          <el-button
            type="primary"
            icon="el-icon-document-add"
            size="small"
            style="margin-left: 3px"
            @click="addVersionInfo"
          >发布</el-button>
        </el-header>
        <el-main class="report_main">
          <el-dialog title="版本信息" :visible.sync="dialogVisible">
            <el-form
              :model="ruleForm"
              :rules="rules"
              ref="ruleForm"
              label-width="100px"
              class="demo-ruleForm"
            >
              <el-form-item label="产品类型">
                <el-select
                  style="width: 100%;"
                  v-model="ruleForm.productType"
                  placeholder="请选择产品类型"
                >
                  <el-option label="运维工具" value="0"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item label="版本号" prop="productVersion">
                <el-input v-model="ruleForm.productVersion" placeholder="V1.0.0"></el-input>
              </el-form-item>

              <el-form-item label="版本描述" prop="versionDescribe">
                <el-input type="textarea" v-model="ruleForm.versionDescribe" placeholder="新增了*功能"></el-input>
              </el-form-item>
              <el-form-item label="上传文件" prop="downloadUrl">
                <el-upload
                  class="upload-demo"
                  :action="action"
                  :on-preview="handlePreview"
                  :on-remove="handleRemove"
                  :before-remove="beforeRemove"
                  multiple
                  :limit="3"
                  :on-exceed="handleExceed"
                  :file-list="fileList"
                >
                  <el-button size="small" type="primary">点击上传</el-button>
                  <!-- <div slot="tip" class="el-upload__tip">只能上传jpg/png文件，且不超过500kb</div> -->
                </el-upload>
                <el-input type="textarea" v-model="ruleForm.downloadUrl"></el-input>
              </el-form-item>

              <el-form-item>
                <el-button type="primary" @click="submitForm('ruleForm')">发布</el-button>
                <el-button @click="resetForm('ruleForm')">重置</el-button>
              </el-form-item>
            </el-form>
          </el-dialog>

          <el-table v-loading="loading" :data="versionInfos" border style="width: 100%">
            <el-table-column v-if="idVisible" prop="id" label="主键ID" width="50"></el-table-column>
            <el-table-column
              fixed="left"
              prop="productType"
              :formatter="productTypeFormat"
              label="产品类型"
              width="120"
            ></el-table-column>
            <el-table-column prop="productVersion" label="版本号" width="80"></el-table-column>

            <el-table-column prop="operatorDate" label="上传时间" width="160"></el-table-column>
            <el-table-column prop="versionDescribe" label="版本描述" width="300"></el-table-column>
            <el-table-column prop="downloadUrl" label="下载信息" width="300"></el-table-column>
            <el-table-column label="操作" width="100">
              <template slot-scope="scope">
                <el-button
                  @click="handleClick(scope.row)"
                  type="danger"
                  icon="el-icon-delete"
                  size="small"
                >删除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <pagination
            v-show="total>0"
            :total="total"
            :page.sync="page"
            :limit.sync="limit"
            @pagination="loadVsersionInfo"
          />
        </el-main>
      </el-container>
    </div>
    <Footer>
      <a href="http://106.53.255.16:8090/" target="_blank">由JieLink+V2.*团队提供技术支持</a>
    </Footer>
  </div>
</template>

<script>
import { postRequest } from "../utils/api";
import { getRequest } from "../utils/api";
import { deleteRequest } from "../utils/api";
import Pagination from "@/components/Pagination";
console.log(process.env.BASE_URL)
export default {
  components: { Pagination },
  methods: {
    //打开对话窗 请求对话参与方数据
    handleClick(row) {
      this.selItems = row;
      this.$confirm("此操作将永久删除该文件, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      })
        .then(() => {
          deleteRequest("/devops/deleteDevOpsProductById", {
            id: this.selItems.id
          }).then(
            resp => {
              this.$notify({
                title: "成功",
                type: "success",
                message: "删除成功!"
              });
              this.loadVsersionInfo();
            },
            resp => {
              if (resp.status == 403) {
                this.$notify({
                  title: "错误",
                  type: "error",
                  message: resp.data.msg
                });
              }
              this.loading = false;
            }
          );
        })
        .catch(() => {
          this.$notify({
            title: "取消",
            type: "info",
            message: "已取消删除"
          });
        });
    },

    //关键字搜索会话
    searchClick() {
      this.loadVsersionInfo();
    },

    addVersionInfo() {
      this.dialogVisible = true;
    },

    productTypeFormat(row, column) {
      if (row.deviceType == 0) {
        return "运维工具";
      }
    },
    //加载会话数据
    loadVsersionInfo() {
      var start = (this.page - 1) * this.limit;
      var end = this.page * this.limit;
      let _this = this;
      _this.loading = true;
      getRequest("/devops/getDevOpsProductWithPages", {
        version: this.keywords,
        start: start,
        end: end
      }).then(
        resp => {
          this.versionInfos = resp.data.items;
          this.total = resp.data.total;
          _this.loading = false;
        },
        resp => {
          if (resp.status == 403) {
            _this.$notify({
              title: "错误",
              type: "error",
              message: resp.data.msg
            });
          }
          _this.loading = false;
        }
      );
    },

    submitForm(formName) {
      this.$refs[formName].validate(valid => {
        if (valid) {
          postRequest("/devops/addDevOpsProduct", {
            productType: this.ruleForm.productType,
            productVersion: this.ruleForm.productVersion,
            versionDescribe: this.ruleForm.versionDescribe,
            downloadUrl: this.ruleForm.downloadUrl
          }).then(
            resp => {
              this.$notify({
                title: "成功",
                message: "发布成功",
                type: "success"
              });
              this.dialogVisible = false;
              this.$refs[formName].resetFields();
              this.loadVsersionInfo();
            },
            resp => {
              if (resp.status == 403) {
                _this.$notify({
                  title: "错误",
                  type: "error",
                  message: resp.data.msg
                });
              }
              _this.loading = false;
            }
          );
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
    resetForm(formName) {
      this.$refs[formName].resetFields();
    },
    handleRemove(file, fileList) {
      console.log(file, fileList);
    },
    handlePreview(file) {
      console.log(file);
    },
    handleExceed(files, fileList) {
      this.$message.warning(`当前限制选择 3 个文件，本次选择了 ${files.length} 个文件，共选择了 ${files.length + fileList.length} 个文件`);
    },
    beforeRemove(file, fileList) {
      return this.$confirm(`确定移除 ${ file.name }？`);
    }
  },
  mounted() {
    //动态设置内容高度 让footer始终居底   header+footer的高度是200
    this.Height = document.documentElement.clientHeight - 200; //监听浏览器窗口变化
    window.onresize = () => {
      this.Height = document.documentElement.clientHeight - 200;
    };

    
    this.loadVsersionInfo();
  },
  data() {
    return {
      fileList:[],
      action: "",
      Height: 0,
      loading: false,
      dialogLoading: false,
      dialogVisible: false,
      idVisible: false,
      keywords: null,
      versionInfos: [],
      total: 0, //数据总条数
      page: 1, //默认显示第1页
      limit: 10, //默认一次显示10条数据

      ruleForm: {
        productType: "",
        productVersion: "",
        versionDescribe: "",
        downloadUrl: ""
      },

      rules: {
        productType: [
          { required: true, message: "请选择产品类型", trigger: "blur" }
        ],
        productVersion: [
          { required: true, message: "请输入版本信息", trigger: "blur" }
        ],
        versionDescribe: [
          { required: true, message: "请输入描述信息", trigger: "blur" }
        ],
        downloadUrl: [
          { required: true, message: "请输入下载链接", trigger: "blur" }
        ]
      },
      pickerOptions: {
        disabledDate(time) {
          return time.getTime() > Date.now();
        },
        shortcuts: [
          {
            text: "今天",
            onClick(picker) {
              picker.$emit("pick", new Date());
            }
          },
          {
            text: "昨天",
            onClick(picker) {
              const date = new Date();
              date.setTime(date.getTime() - 3600 * 1000 * 24);
              picker.$emit("pick", date);
            }
          },
          {
            text: "一周前",
            onClick(picker) {
              const date = new Date();
              date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
              picker.$emit("pick", date);
            }
          }
        ]
      }
    };
  }
};
</script>
<style>
@import "../assets/common.css";
</style>>