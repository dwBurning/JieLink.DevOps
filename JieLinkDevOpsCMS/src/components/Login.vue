<template>
  <el-form
    :model="loginForm"
    :rules="rules"
    ref="loginForm"
    class="login-container"
    label-position="left"
    label-width="0px"
    v-loading="loading"
  >
    <h3 class="login_title">JieLink运维平台</h3>
    <el-form-item prop="username">
      <el-input type="text" v-model="loginForm.username" auto-complete="off" placeholder="用户名"></el-input>
    </el-form-item>
    <el-form-item prop="password">
      <el-input type="password" v-model="loginForm.password" auto-complete="off" placeholder="密码"></el-input>
    </el-form-item>

    <el-checkbox class="login_remember" v-model="loginForm.checked" label-position="left">记住密码</el-checkbox>
    <el-form-item style="width: 100%">
      <el-button type="primary" @click="submitClick('loginForm')" style="width: 100%">登录</el-button>
    </el-form-item>
  </el-form>
</template>
<script>
import { postRequest } from "../utils/api";
import { putRequest } from "../utils/api";
export default {
  data() {
    return {
      loading: false,
      loginForm: {
        username: "burning",
        password: "js*168",
        checked: true
      },
      rules: {
        username: [
          { required: true, message: "请输入用户名", trigger: "blur" }
        ],
        password: [{ required: true, message: "请输入密码", trigger: "blur" }]
      }
    };
  },

  methods: {
    submitClick(formName) {
      this.$refs[formName].validate(valid => {
        if (valid) {
          var _this = this;
          this.loading = true;
          postRequest("/login", {
            username: this.loginForm.username,
            password: this.loginForm.password
          }).then(
            resp => {
              _this.loading = false;
              if (resp.status == 200) {
                //成功
                var json = resp.data;
                if (json.status == 0) {
                  localStorage.setItem('username', this.loginForm.username);
                  _this.$router.replace({ path: "/versionmanager" });
                } else {
                  _this.$alert("登录失败!", "失败!");
                }
              } else {
                //失败
                _this.$alert("登录失败!", "失败!");
              }
            },
            resp => {
              _this.loading = false;
              _this.$alert("找不到服务器⊙﹏⊙∥!", "失败!");
            }
          );
        }
      });
    }
  }
};
</script>
<style>
.login-container {
  border-radius: 15px;
  background-clip: padding-box;
  margin: 180px auto;
  width: 350px;
  padding: 35px 35px 15px 35px;
  background: #fff;
  border: 1px solid #eaeaea;
  box-shadow: 0 0 25px #cac6c6;
}

.login_title {
  margin: 0px auto 40px auto;
  text-align: center;
  color: #505458;
}

.login_remember {
  margin: 0px 0px 35px 0px;
  text-align: left;
}
</style>
