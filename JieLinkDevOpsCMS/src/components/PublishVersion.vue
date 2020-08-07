<template>
  <el-container>
    <el-header class="report_header">
      <el-input
        placeholder="请输入工单号..."
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
          <el-form-item label="工单号" prop="workOrderNo">
            <el-input v-model="ruleForm.workOrderNo"></el-input>
          </el-form-item>
          <el-form-item label="版本号" prop="standVersion">
            <el-input v-model="ruleForm.standVersion" placeholder="V1.0.0"></el-input>
          </el-form-item>
          <el-form-item label="版本类型">
            <el-select style="width: 100%;" v-model="ruleForm.versionType" placeholder="请选择版本类型">
              <el-option label="工单" value="0"></el-option>
              <el-option label="补丁" value="1"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="操作方式">
            <el-select style="width: 100%;" v-model="ruleForm.operatorType" placeholder="请选择操作方式">
              <el-option label="替换盒子的文件" value="替换盒子的文件"></el-option>
              <el-option label="替换中心的文件" value="替换中心的文件"></el-option>
              <el-option label="替换Web的文件" value="替换Web的文件"></el-option>
            </el-select>
          </el-form-item>

          <el-form-item label="编译时间" prop="compileDate">
            <el-date-picker
              style="width: 100%;"
              v-model="ruleForm.compileDate"
              align="right"
              type="date"
              format="yyyy-MM-dd HH:mm:ss"
              value-format="yyyy-MM-dd HH:mm:ss"
              placeholder="选择日期"
              :picker-options="pickerOptions"
            ></el-date-picker>
          </el-form-item>

          <el-form-item label="版本描述" prop="versionDescribe">
            <el-input type="textarea" v-model="ruleForm.versionDescribe" placeholder="基于标准版本修改了*问题"></el-input>
          </el-form-item>
          <el-form-item label="下载信息" prop="downloadMsg">
            <el-input type="textarea" v-model="ruleForm.downloadMsg"></el-input>
          </el-form-item>

          <el-form-item>
            <el-button type="primary" @click="submitForm('ruleForm')">发布</el-button>
            <el-button @click="resetForm('ruleForm')">重置</el-button>
          </el-form-item>
        </el-form>
      </el-dialog>

      <el-table v-loading="loading" :data="versionInfos" border style="width: 100%">
        <el-table-column v-if="idVisible" prop="id" label="主键ID" width="50"></el-table-column>
        <el-table-column fixed="left" prop="workOrderNo" label="工单号" width="300"></el-table-column>
        <el-table-column prop="standVersion" label="版本号" width="80"></el-table-column>
        <el-table-column prop="versionType" :formatter="versionTypeFormat" label="版本类型" width="80"></el-table-column>
        <el-table-column prop="compileDate" label="编译时间" width="160"></el-table-column>
        <el-table-column prop="versionDescribe" label="版本描述" width="400"></el-table-column>
        <el-table-column prop="downloadMsg" label="下载信息" width="400"></el-table-column>
        <el-table-column fixed="right" label="操作" width="100">
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
</template>

<script>
import { postRequest } from "../utils/api";
import { getRequest } from "../utils/api";
import { deleteRequest } from "../utils/api";
import Pagination from "@/components/Pagination";
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
          deleteRequest("/version/deleteVersionInfoById", {
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

    versionTypeFormat(row, column) {
      if (row.versionType == 0) {
        return "工单";
      } else if (row.versionType == 1) {
        return "补丁";
      }
    },
    //加载会话数据
    loadVsersionInfo() {
      var start = (this.page - 1) * this.limit;
      var end = this.page * this.limit;
      let _this = this;
      _this.loading = true;
      getRequest("/version/getVersionInfoWithPages", {
        orderNo: this.keywords,
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
          postRequest("/version/addVersionInfo", {
            workOrderNo: this.ruleForm.workOrderNo,
            standVersion: this.ruleForm.standVersion,
            versionType: this.ruleForm.versionType,
            compileDate: this.ruleForm.compileDate,
            versionDescribe:
              "[" + this.ruleForm.operatorType + "]，" + this.ruleForm.versionDescribe,
            downloadMsg: this.ruleForm.downloadMsg
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
    }
  },
  mounted() {
    this.loadVsersionInfo();
  },
  data() {
    return {
      Height: 0,
      loading: false,
      dialogLoading: false,
      dialogVisible: false,
      idVisible: false,
      keywords: null,
      versionInfos: [],
      total: 0, //数据总条数
      page: 1, //默认显示第1页
      limit: 5, //默认一次显示5条数据

      ruleForm: {
        workOrderNo: "",
        standVersion: "",
        versionType: "",
        operatorType: "",
        compileDate: "",
        versionDescribe: "",
        downloadMsg: ""
      },

      rules: {
        workOrderNo: [
          { required: true, message: "请输入工单号", trigger: "blur" }
        ],
        standVersion: [
          { required: true, message: "请输入版本信息", trigger: "blur" }
        ],
        versionType: [
          { required: true, message: "请输入版本类型", trigger: "blur" }
        ],
        compileDate: [
          { required: true, message: "请选择编译日期", trigger: "blur" }
        ],
        versionDescribe: [
          { required: true, message: "请输入描述信息", trigger: "blur" }
        ],
        downloadMsg: [
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