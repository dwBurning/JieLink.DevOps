<template>
  <el-container>
    <el-header class="report_header">
      <el-select
        style="width: 400px"
        prefix-icon="el-icon-search"
        v-model="keywords"
        placeholder="请选择事件类型"
      >
        <el-option label="系统内存预警" value="1"></el-option>
        <el-option label="系统CPU预警" value="2"></el-option>
        <el-option label="进程线程预警" value="3"></el-option>
        <el-option label="系统磁盘预警" value="4"></el-option>
        <el-option label="进程内存溢出" value="5"></el-option>
        <el-option label="进程CPU预警" value="6"></el-option>
      </el-select>
      <el-button
        type="primary"
        icon="el-icon-search"
        size="small"
        style="margin-left: 3px"
        @click="searchClick"
      >搜索</el-button>
    </el-header>
    <el-main class="report_main">
      <el-dialog title="项目信息" :visible.sync="dialogVisible">
        <el-form
          :model="ruleForm"
          :rules="rules"
          ref="ruleForm"
          label-width="100px"
          class="demo-ruleForm"
        >
          <el-form-item label="项目编号" prop="projectNo">
            <el-input :readonly="true" v-model="ruleForm.projectNo"></el-input>
          </el-form-item>
          <el-form-item label="当前版本" prop="devopsVersion">
            <el-input :readonly="true" v-model="ruleForm.devopsVersion"></el-input>
          </el-form-item>
          <el-form-item label="是否过滤" prop="boolenIsFilter">
            <el-switch v-model="ruleForm.boolenIsFilter"></el-switch>
          </el-form-item>
          <el-form-item label="注册时间" prop="operatorDate">
            <el-input :readonly="true" v-model="ruleForm.operatorDate"></el-input>
          </el-form-item>
          <el-form-item label="备注" prop="remark">
            <el-input v-model="ruleForm.remark"></el-input>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="submitForm('ruleForm')">确认</el-button>
          </el-form-item>
        </el-form>
      </el-dialog>

      <el-table v-loading="loading" :data="events" border style="width: 100%">
        <el-table-column v-if="idVisible" prop="id" label="主键ID" width="100"></el-table-column>
        <el-table-column
          fixed="left"
          prop="eventType"
          label="事件类型"
          :formatter="eventTypeFormat"
          width="120"
        ></el-table-column>

        <el-table-column label="项目编号" width="120">
          <template scope="scope">
            <el-button
              @click="getProjectInfoByProjectNo(scope.row)"
              type="text"
              size="small"
            >{{ scope.row.projectNo }}</el-button>
          </template>
        </el-table-column>
        <el-table-column prop="projectName" label="项目名称" width="150"></el-table-column>
        <el-table-column prop="projectVersion" label="项目版本" width="100"></el-table-column>
        <el-table-column prop="remoteAccount" label="远程账号" width="150"></el-table-column>
        <el-table-column prop="remotePassword" label="远程密码" width="100"></el-table-column>
        <el-table-column prop="contactName" label="联系人姓名" width="100"></el-table-column>
        <el-table-column prop="contactPhone" label="联系人电话" width="120"></el-table-column>
        <el-table-column prop="operatorDate" label="入库时间" width="200"></el-table-column>
        <el-table-column fixed="right" label="操作" width="150">
          <template slot-scope="scope">
            <el-button
              :disabled="scope.row.isProcessed==1"
              @click="handleClick(scope.row)"
              icon="el-icon-finished"
              type="primary"
              size="small"
            >标记为已处理</el-button>
          </template>
        </el-table-column>
        <el-table-column fixed="right" label="标记" width="150">
          <template slot-scope="scope">
            <el-button
              :disabled="scope.row.isFilter==1"
              @click="filterClick(scope.row)"
              icon="el-icon-finished"
              type="primary"
              size="small"
            >标记为过滤清单</el-button>
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
import { putRequest } from "../utils/api";
import { getRequest } from "../utils/api";
import Pagination from "@/components/Pagination";
export default {
  components: { Pagination },
  methods: {
    //打开对话窗
    handleClick(row) {
      this.selItems = row;
      putRequest("/devops/processed", {
        id: this.selItems.id
      }).then(
        resp => {
          this.$notify({
            title: "成功",
            message: "标记成功",
            type: "success"
          });
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
    },

    filterClick(row) {
      this.selItems = row;
      putRequest("/devops/filter", {
        projectNo: this.selItems.projectNo
      }).then(
        resp => {
          this.$notify({
            title: "成功",
            message: "标记成功",
            type: "success"
          });
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
    },

    //搜索
    searchClick() {
      this.loadVsersionInfo();
    },

    processStatusFormat(row, column) {
      if (row.isProcessed == 0) {
        return "未处理";
      } else if (row.isProcessed == 1) {
        return "已处理";
      }
    },

    eventTypeFormat(row, colume) {
      if (row.eventType == 1) {
        return "系统内存预警";
      } else if (row.eventType == 2) {
        return "系统CPU预警";
      } else if (row.eventType == 3) {
        return "进程线程预警";
      } else if (row.eventType == 4) {
        return "系统磁盘预警";
      } else if (row.eventType == 5) {
        return "进程内存溢出";
      } else if (row.eventType == 6) {
        return "进程CPU预警";
      } else {
        return "其他";
      }
    },

    loadVsersionInfo() {
      var start = (this.page - 1) * this.limit;
      var end = this.page * this.limit;
      let _this = this;
      _this.loading = true;
      getRequest("/devops/getDevOpsEventWithPages", {
        eventCode: this.keywords == null ? -1 : this.keywords,
        start: start,
        end: end
      }).then(
        resp => {
          this.events = resp.data.items;
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

    getProjectInfoByProjectNo(row) {
      this.selItems = row;
      let _this = this;
      _this.dialogLoading = true;
      getRequest("/devops/getProjectInfoByProjectNo", {
        projectNo: this.selItems.projectNo
      }).then(
        resp => {
          this.ruleForm = resp.data;
          _this.dialogVisible = true;
          _this.dialogLoading = false;
        },
        resp => {
          if (resp.status == 403) {
            _this.$notify({
              title: "错误",
              type: "error",
              message: resp.data.msg
            });
          }
          _this.dialogLoading = false;
        }
      );
    },

    submitForm(formName) {
      this.$refs[formName].validate(valid => {
        if (valid) {
          postRequest("/devops/updateProjectInfo", {
            projectNo: this.ruleForm.projectNo,
            isFilter: this.ruleForm.boolenIsFilter?1:0,
            remark: this.ruleForm.remark
          }).then(
            resp => {
              this.loadVsersionInfo();
              this.$notify({
                title: "成功",
                message: "项目信息更新成功",
                type: "success"
              });
              this.dialogVisible = false;
              this.$refs[formName].resetFields();
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
      selItems: "",
      projectInfo: null,
      events: [],
      total: 0, //数据总条数
      page: 1, //默认显示第1页
      limit: 5, //默认一次显示5条数据

      ruleForm: {
        projectNo: "",
        isFilter: 0,
        boolenIsFilter:false,
        operatorDate: "",
        remark: ""
      },
      rules: {

      }
    };
  }
};
</script>
<style>
@import "../assets/common.css";
</style>>