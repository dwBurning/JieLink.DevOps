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
    </el-header>
    <el-main class="report_main">
      <el-dialog title="申请人信息" :visible.sync="dialogVisible">
        <el-form
          :model="ruleForm"
          :rules="rules"
          ref="ruleForm"
          label-width="100px"
          class="demo-ruleForm"
        >
          <el-form-item label="工号" prop="jobNumber">
            <el-input v-model="ruleForm.jobNumber"></el-input>
          </el-form-item>
          <el-form-item label="姓名" prop="name">
            <el-input v-model="ruleForm.name"></el-input>
          </el-form-item>
          <el-form-item label="手机" prop="cellPhone">
            <el-input v-model="ruleForm.cellPhone"></el-input>
          </el-form-item>
          <el-form-item label="邮箱" prop="email">
            <el-input v-model="ruleForm.email"></el-input>
          </el-form-item>

          <el-form-item>
            <el-button type="primary" @click="submitForm('ruleForm')">申请</el-button>
            <el-button @click="resetForm('ruleForm')">重置</el-button>
          </el-form-item>
        </el-form>
      </el-dialog>

      <el-table v-loading="loading" :data="versionInfos" border style="width: 100%">
        <el-table-column v-if="idVisible" prop="id" label="主键ID" width="100"></el-table-column>
        <el-table-column fixed="left" prop="workOrderNo" label="工单号" width="300"></el-table-column>
        <el-table-column prop="standVersion" label="版本号" width="80"></el-table-column>
        <el-table-column prop="versionType" :formatter="versionTypeFormat" label="版本类型" width="120"></el-table-column>
        <el-table-column prop="compileDate" label="编译时间" width="200"></el-table-column>
        <el-table-column prop="versionDescribe" label="版本描述" width="400"></el-table-column>
        <el-table-column fixed="right" label="操作" width="100">
          <template slot-scope="scope">
            <el-button
              @click="handleClick(scope.row)"
              icon="el-icon-unlock"
              type="primary"
              size="small"
            >申请</el-button>
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
import Pagination from "@/components/Pagination";
export default {
  components: { Pagination },
  methods: {
    //打开对话窗 请求对话参与方数据
    handleClick(row) {
      this.selItems = row;
      this.dialogVisible = true;
    },

    //关键字搜索会话
    searchClick() {
      this.loadVsersionInfo();
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
          postRequest("/apply/addApplyInfo", {
            workOrderNo: this.selItems.workOrderNo,
            jobNumber: this.ruleForm.jobNumber,
            name: this.ruleForm.name,
            cellPhone: this.ruleForm.cellPhone,
            email: this.ruleForm.email
          }).then(
            resp => {
              this.$notify({
                title: "成功",
                message: "申请成功，请注意查收邮件",
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
    var checkPhone = (rule, value, callback) => {
      if (!value) {
        return callback(new Error("手机号不能为空"));
      } else {
        const reg = /^1[3|4|5|7|8][0-9]\d{8}$/;
        console.log(reg.test(value));
        if (reg.test(value)) {
          callback();
        } else {
          return callback(new Error("请输入正确的手机号"));
        }
      }
    };

    return {
      Height: 0,
      loading: false,
      dialogLoading: false,
      dialogVisible: false,
      idVisible: false,
      keywords: null,
      selItems: "",
      versionInfos: [],
      total: 0, //数据总条数
      page: 1, //默认显示第1页
      limit: 5, //默认一次显示5条数据

      ruleForm: {
        jobNumber: "",
        name: "",
        cellPhone: "",
        email: ""
      },

      rules: {
        jobNumber: [{ required: true, message: "请输入工号", trigger: "blur" }],
        name: [{ required: true, message: "请输入姓名", trigger: "blur" }],
        cellPhone: [
          { required: true, message: "请输入手机号", trigger: "blur" },
          { validator: checkPhone, trigger: "blur" }
        ],
        email: [
          { required: true, message: "请输入邮箱地址", trigger: "blur" },
          {
            type: "email",
            message: "请输入正确的邮箱地址",
            trigger: ["blur", "change"]
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