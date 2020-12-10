<template>
  <div id="app">
    <Header></Header>
    <el-container>
      <div style="heigh:200p;disply:block">{{message}}</div>
    </el-container>
  </div>
</template>

<script>
import { postRequest } from "../utils/api";
import { getRequest } from "../utils/api";
export default {
  data() {
    return {
      message: "建设中..."
    };
  }
};
</script>
<style>
@import "../assets/common.css";
</style>>