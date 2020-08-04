<template>
  <div id="app">
    <Header></Header>
    <el-container>
      <el-header class="report_header">
        <el-input
          placeholder="请输入用户名..."
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
          @click="addSysUser"
        >新增</el-button>
      </el-header>
      <el-main class="report_main">
        <el-dialog title="新增用户" :visible.sync="dialogVisible">
          <el-form
            :model="ruleForm"
            :rules="rules"
            ref="ruleForm"
            label-width="100px"
            class="demo-ruleForm"
          >
            <el-form-item label="用户名" prop="userName">
              <el-input v-model="ruleForm.userName"></el-input>
            </el-form-item>
            <el-form-item label="密码" prop="password">
              <el-input type="password" v-model="ruleForm.password"></el-input>
            </el-form-item>
            <el-form-item label="确认密码" prop="checkPass">
              <el-input type="password" v-model="ruleForm.checkPass" autocomplete="off"></el-input>
            </el-form-item>
            <el-form-item label="手机" prop="cellPhone">
              <el-input v-model="ruleForm.cellPhone"></el-input>
            </el-form-item>
            <el-form-item label="邮箱" prop="email">
              <el-input v-model="ruleForm.email"></el-input>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="submitForm('ruleForm')">添加</el-button>
              <el-button @click="resetForm('ruleForm')">重置</el-button>
            </el-form-item>
          </el-form>
        </el-dialog>

        <el-table v-loading="loading" :data="sysUsers" border style="width: 100%">
          <el-table-column v-if="idVisible" prop="id" label="主键ID" width="50"></el-table-column>
          <el-table-column fixed="left" prop="userName" label="用户名" width="120"></el-table-column>
          <el-table-column prop="cellPhone" label="手机" width="200"></el-table-column>
          <el-table-column prop="email" label="邮箱" width="200"></el-table-column>
        </el-table>
      </el-main>
    </el-container>
  </div>
</template>

<script>
import { postRequest } from "../utils/api";
import { getRequest } from "../utils/api";
export default {
  methods: {
    //关键字搜索会话
    searchClick() {
      this.loadSysUsers();
    },

    addSysUser() {
      this.dialogVisible = true;
    },

    //加载会话数据
    loadSysUsers() {
      let _this = this;
      _this.loading = true;
      getRequest("/user/getSysUsers", {
        userName: this.keywords
      }).then(
        resp => {
          this.sysUsers = resp.data;
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
          postRequest("/user/addSysUser", {
            userName: this.ruleForm.userName,
            password: this.ruleForm.password,
            cellPhone: this.ruleForm.cellPhone,
            email: this.ruleForm.email
          }).then(
            resp => {
              var json = resp.data;
              console.log(json)
              if (json.code == 0) {
                this.$notify({
                  title: "成功",
                  message: "添加成功",
                  type: "success"
                });
                this.dialogVisible = false;
                this.$refs[formName].resetFields();
                this.loadSysUsers();
              } else {
                this.$notify({
                  title: "失败",
                  message: json.msg,
                  type: "warning"
                });
              }
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
    this.loadSysUsers();
  },
  data() {
    var validatePass = (rule, value, callback) => {
      if (value === "") {
        callback(new Error("请输入密码"));
      } else {
        if (this.ruleForm.checkPass !== "") {
          this.$refs.ruleForm.validateField("checkPass");
        }
        callback();
      }
    };
    var validatePass2 = (rule, value, callback) => {
      if (value === "") {
        callback(new Error("请再次输入密码"));
      } else if (value !== this.ruleForm.password) {
        callback(new Error("两次输入密码不一致!"));
      } else {
        callback();
      }
    };
    return {
      Height: 0,
      loading: false,
      dialogLoading: false,
      dialogVisible: false,
      idVisible: false,
      keywords: null,
      sysUsers: [],

      ruleForm: {
        userName: "",
        password: "",
        checkPass: "",
        cellPhone: "",
        email: ""
      },

      rules: {
        userName: [
          { required: true, message: "请输入用户名", trigger: "blur" }
        ],
        password: [{ validator: validatePass, trigger: "blur" }],
        checkPass: [{ validator: validatePass2, trigger: "blur" }],

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