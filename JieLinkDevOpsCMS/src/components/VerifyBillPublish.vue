<template>
  <el-container>
    <el-header class="report_header">
      <el-input
        placeholder="请输入工单号或者版本描述关键字..."
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
      >新增对账任务</el-button>
      <!--<el-button
        type="primary"
        icon="el-icon-document-add"
        size="small"
        style="margin-left: 3px"
        @click="Test"
      >测试按钮</el-button> -->
      <!-- <el-upload
       action=""
  class="upload-demo"
  :on-preview="handlePreview"
  :on-remove="handleRemove"
  :before-remove="beforeRemove"
  multiple
  :limit="3"
              :auto-upload="false" 
  :file-list="fileList">
  <el-button size="small" type="primary">点击上传</el-button>
</el-upload> -->
    </el-header>

    <el-main class="report_main">

      <el-dialog
        title="补录语句"
        :visible.sync="SQLDialog"
      >
        <el-input
          type="textarea"
          :rows="30"
          v-model="textarea"
        >
        </el-input>
        <el-button
          style="margin-top :10px"
          type="primary"
          @click="SQLDialog=false"
        >确定</el-button>
      </el-dialog>

      <el-dialog
        title="补充远程"
        :visible.sync="AddRemoteDialog"
      >
        <el-form
          :model="ruleForm"
          :rules="rules"
          ref="ruleForm"
          label-width="100px"
          class="demo-ruleForm"
        >
          <el-form-item
            label="向日葵远程"
            prop="set_project_remote"
          >
            <el-input v-model="set_project_remote"></el-input>
          </el-form-item>
          <el-form-item
            label="远程密码"
            prop="set_project_remote_password"
          >
            <el-input v-model="set_project_remote_password"></el-input>
          </el-form-item>
          <el-form-item>
            <el-button @click="AddRemoteDialog = false">取消</el-button>
            <el-button
              type="primary"
              @click="setRemote('ruleForm')"
            >确定</el-button>
          </el-form-item>
        </el-form>
      </el-dialog>
      <el-dialog
        title="对账信息"
        :visible.sync="dialogVisible"
      >
        <el-form
          :model="ruleForm"
          :rules="rules"
          ref="ruleForm"
          label-width="100px"
          class="demo-ruleForm"
        >

          <el-form-item
            label="商户号"
            prop="project_shoper_no"
          >
            <el-input v-model="ruleForm.project_shoper_no"></el-input>
          </el-form-item>
          <el-form-item
            label="停车场名称"
            prop="project_name"
          >
            <el-input v-model="ruleForm.project_name"></el-input>
          </el-form-item>
          <el-form-item
            label="停车场编号"
            prop="project_no"
          >
            <el-input v-model="ruleForm.project_no"></el-input>
          </el-form-item>
          <el-form-item
            label="版本类型"
            prop="versionType"
          >
            <el-select
              style="width: 100%;"
              v-model="ruleForm.versionType"
              placeholder="请选择产品类型"
            >
              <el-option
                label="JieLink"
                value="0"
              ></el-option>
              <el-option
                label="G3-标准版"
                value="1"
              ></el-option>
              <el-option
                label="G3-速通版"
                value="2"
              ></el-option>
            </el-select>
          </el-form-item>
          <el-form-item
            label="版本号"
            prop="project_version"
          >
            <el-input
              v-model="ruleForm.version"
              placeholder="Jielink版本号例如2.7.1,2.8.1E1, G3版本号例如1.6.2SP10,1.0.3等, 可不填"
            ></el-input>
          </el-form-item>
          <el-form-item
            label="是否非标"
            prop="nonstandard"
          >
            <el-select
              style="width: 100%;"
              v-model="ruleForm.nonstandard"
              placeholder="是否非标 默认填否"
            >
              <el-option
                label="是"
                value="1"
              ></el-option>
              <el-option
                label="否"
                value="0"
              ></el-option>
            </el-select>
          </el-form-item>
          <el-form-item
            label="向日葵远程"
            prop="project_remote"
          >
            <el-input v-model="ruleForm.project_remote"></el-input>
          </el-form-item>
          <el-form-item
            label="远程密码"
            prop="project_remote_password"
          >
            <el-input v-model="ruleForm.project_remote_password"></el-input>
          </el-form-item>
          <el-form-item
            label="问题描述"
            prop="describe"
          >
            <el-input
              type="textarea"
              v-model="ruleForm.describe"
              placeholder="例如2.1号订单平台有车场无，开机密码，敏感现场等"
            ></el-input>
          </el-form-item>

          <el-upload
            class="upload-demo"
            ref="upload"
            accept=".xlsx,.xls"
            action=""
            :on-remove="handleRemove"
            :on-change="onUploadChange"
            :before-upload="beforeUpload"
            :auto-upload="false"
            :limit="1"
            :file-list="fileList"
          >

            <el-button
              size="small"
              type="primary"
              style="margin-left:100px;"
            >选择excel文件上传</el-button>
            <label>只能上传一个不超过1M的excel,jielink需要补录或者删除的数据必须放在第一个Sheet中,多余统计数据需删除</label>
          </el-upload>

          <el-checkbox-group v-model="ruleForm.checkList">
            <el-checkbox
              style="margin-left:100px;margin-top:20px;margin-bottom:20px"
              label="平台有车场无 补录"
            ></el-checkbox>
            <el-checkbox label="平台无车场有 补推"></el-checkbox>
            <el-checkbox label="平台无车场有 删除"></el-checkbox>
            <el-checkbox label="退款问题"></el-checkbox>
            <el-checkbox label="查原因"></el-checkbox>
          </el-checkbox-group>

          <el-form-item>
            <el-button
              type="primary"
              @click="submitForm('ruleForm')"
            >发布</el-button>
            <!--@click="submitUpload()" -->
            <!-- @click="submitForm('ruleForm')" -->
            <el-button
              type="primary"
              @click="sunloginClick('ruleForm')"
            >一键搜索向日葵</el-button>
            <el-button @click="resetForm('ruleForm')">重置</el-button>
          </el-form-item>
        </el-form>
      </el-dialog>
      <el-table
        v-loading="loading"
        :data="versionInfos"
        border
        style="width: 100%"
      >
        <el-table-column
          v-if="idVisible"
          prop="id"
          label="主键ID"
          width="50"
        ></el-table-column>
        <el-table-column
          fixed="left"
          prop="projectName"
          label="项目名"
          width="160"
        ></el-table-column>
        <el-table-column
          prop="emergency"
          :formatter="EmergencyFormat"
          label="紧急程度"
          width="80"
        ></el-table-column>
        <el-table-column
          prop="status"
          :formatter="statusFormat"
          label="工单状态"
          width="120"
        ></el-table-column>
        <el-table-column
          prop="projectBigVersion"
          :formatter="VersionFormat"
          label="版本类型"
          width="80"
        ></el-table-column>
        <el-table-column
          prop="projectRemark"
          label="项目信息"
          width="400"
        ></el-table-column>
        <el-table-column
          prop="projectVersion"
          label="版本号"
          width="80"
        ></el-table-column>
        <el-table-column
          prop="projectIsNonstandard"
          :formatter="IsNonstandardFormat"
          label="是否非标"
          width="80"
        ></el-table-column>
        <el-table-column
          prop="projectNo"
          label="项目编号"
          width="160"
        ></el-table-column>
        <el-table-column
          prop="projectTask"
          label="对账需求"
          width="160"
        ></el-table-column>
        <el-table-column
          prop="projectRemote"
          label="项目远程"
          width="160"
        ></el-table-column>
        <el-table-column
          prop="projectRemotePassword"
          label="项目远程密码"
          width="100"
        ></el-table-column>
        <el-table-column
          fixed="right"
          label="操作"
          width="450"
        >
          <template slot-scope="scope">
            <el-button
              style="margin-left: 10px"
              @click="finishClick(scope.row)"
              type="success"
              icon="el-icon-check"
              size="small"
            >验收</el-button>
            <el-button
              @click="ConfirmClick(scope.row)"
              type="success"
              icon="el-icon-chat-dot-square"
              size="small"
            >研发确认</el-button>
            <el-button
              @click="SQLClick(scope.row)"
              type="link"
              icon="el-icon-document-copy"
              size="small"
            >补录语句</el-button>
            <el-button
              @click="emergencyClick(scope.row)"
              type="link"
              icon="el-icon-star-off"
              size="small"
            >加急</el-button>
            <el-button
              @click="ReUpload(scope.row)"
              style="margin-top: 10px"
              type="primary"
              icon="el-icon-upload2"
              size="small"
            >重传</el-button>
            <el-button
              @click="DownExcelClick(scope.row)"
              type="primary"
              icon="el-icon-download"
              size="small"
            >下载表格</el-button>
            <el-button
              @click="AddRemoteTemp(scope.row)"
              type="primary"
              icon="el-icon-thumb"
              size="small"
            >补充远程</el-button>
            <el-button
              @click="deleteClick(scope.row)"
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

<script src="https://cdn.jsdelivr.net/npm/vue"></script>
<script src="https://unpkg.com/element-ui@2.6.1/lib/index.js"></script>
<script src="https://unpkg.com/axios/dist/axios.min.js"></script>
<script>
import { postRequest } from "../utils/api";
import { putRequest } from "../utils/api";
import { getRequest } from "../utils/api";
import { deleteRequest } from "../utils/api";
import Pagination from "@/components/Pagination";
const axios = require("axios");
var AddRemoteId;
var m_filename;
export default {
  components: { Pagination },
  methods: {
    Test() {},

    //打开对话窗 请求对话参与方数据
    deleteClick(row) {
      this.selItems = row;
      this.$confirm("此操作将删除该工单, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(() => {
          deleteRequest("/VerifyBill/deleteVerifyBillInfoById", {
            id: this.selItems.id,
          }).then(
            (resp) => {
              this.$notify({
                title: "成功",
                type: "success",
                message: "删除成功!",
              });
              this.loadVsersionInfo();
            },
            (resp) => {
              if (resp.status == 403) {
                this.$notify({
                  title: "错误",
                  type: "error",
                  message: resp.data.msg,
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
            message: "已取消删除",
          });
        });
    },

    //增加紧急度
    emergencyClick(row) {
      this.selItems = row;
      putRequest("/VerifyBill/addEmergencyById", {
        id: this.selItems.id,
      }).then((resp) => {
        this.$notify({
          title: "加急",
          type: "success",
          message: "紧急度+1",
        });
        this.loadVsersionInfo();
      });
    },

    //重新上传 重传
    ReUpload(row)
    {
      this.selItems = row;
      this.$confirm(
        "该操作将重新上传文件，重新生成补录语句，是否继续?",
        "提示",
        {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
        }
      )
    },

    //验收
    finishClick(row) {
      this.selItems = row;
      this.$confirm(
        "该操作将从该界面隐藏该订单，请确认该对账任务已完成?",
        "提示",
        {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
        }
      )
        .then(() => {
          putRequest("/VerifyBill/finishVerifyBillInfoById", {
            id: this.selItems.id,
          }).then(
            (resp) => {
              this.$notify({
                title: "成功",
                type: "success",
                message: "验收成功!",
              });
              this.loadVsersionInfo();
            },
            (resp) => {
              if (resp.status == 403) {
                this.$notify({
                  title: "错误",
                  type: "error",
                  message: resp.data.msg,
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
            message: "已取消验收",
          });
        });
    },

    //研发确认按钮
    ConfirmClick(row) {
      this.$prompt("输入处理结果", "研发确认", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        inputErrorMessage: "错误",
      })
        .then(({ value }) => {
          //获取登录用户名
          var username = "";
          getRequest("user/currentUserName").then((resp) => {
            username = resp.data;

            this.selItems = row;
            putRequest("/VerifyBill/ConfirmVerifyBillById", {
              id: this.selItems.id,
              remark: value,
              user: username,
            });

            clearTimeout(this.timer); //清除延迟执行
            this.timer = setTimeout(() => {
              //有时候刷新页面太快，数据还没提交完成，设置刷新延迟执行700ms
              this.$message({
                type: "success",
                message: "已提交！" + value,
              });
              this.loadVsersionInfo();
            }, 700);

          });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "取消输入",
          });
        });
    },

    //搜索向日葵
    sunloginClick(row) {
      getRequest("/VerifyBill/SearchSunloginByInfo", {
        project_shoper_no: this.ruleForm.project_shoper_no,
        project_no: this.ruleForm.project_no,
        project_name: this.ruleForm.project_name,
        versiontype: this.ruleForm.versionType,
      }).then((resp) => {
        if (resp.data.code == 0) {
          var temp = resp.data.msg.split("/");
          this.ruleForm.project_remote = temp[0];
          this.ruleForm.project_remote_password = temp[1];
          this.$notify({
            title: "成功",
            type: "success",
            message: "搜索到同项目的过往远程记录，请确认能否连接成功",
          });
        } else {
          if (this.ruleForm.project_remote == "")
            this.ruleForm.project_remote = "无";
          if (this.ruleForm.project_remote_password == "")
            this.ruleForm.project_remote_password = "无";
          this.$notify({
            title: "未找到",
            type: "error",
            message: "未在数据库中找到远程，请联系朱海涛获取项目远程",
          });
        }
      });
      this.loading = false;
    },

    //补录语句
    SQLClick(row) {
      this.selItems = row;
      getRequest("/VerifyBill/GetAutoSqlById", {
        id: this.selItems.id,
      }).then((resp) => {
        if (resp.data.code == 0) {
          // //换个行容易吗
          // var SQLText = resp.data.msg.split("\r\n");
          // const newDatas = [];
          // const h = this.$createElement;
          // for (const i in SQLText) {
          //   newDatas.push(h("p", null, SQLText[i]));
          // }

          //换textarea了 能放的多了还方便复制
          this.SQLDialog = true;
          this.textarea = resp.data.msg;
          // this.$alert(h("div", null, newDatas), "补录语句", {
          //   confirmButtonText: "确定",
          // });
        } else {
          this.$notify({
            title: "错误",
            type: "error",
            message: "未查到该条的SQL语句！",
          });
        }
      });
    },

    //下载文件按钮
    DownExcelClick(row) {
      this.selItems = row;
      let fd = new FormData();
      fd.append("id", this.selItems.id);
      axios({
        method: "post",
        url: "/VerifyBill/GetFile/" + new Date().getTime(),
        data: fd,
        responseType: "blob",
        //responseType: "FileOutputStream",
        headers: {
          "Content-Type":
            "multipart/form-data;boundary=" + new Date().getTime(),
        },
      })
        .then((response) => {
          this.download(response);
        })
        .catch((error) => {
          error.toString();
        });
    },
    // 下载文件
    download(data) {
      if (!data) {
        return;
      }
      //无文件流返回
      if (data.data.size == 0 || data.size == 0 || data.data == "") {
        this.$notify({
          title: "失败",
          type: "error",
          message: "未找到文件!",
        });
        return;
      }
      let url = window.URL.createObjectURL(new Blob([data.data]));
      //let url = window.URL.createObjectURL(new File([data],"excel.xlsx"));
      let link = document.createElement("a");
      link.style.display = "none";
      link.href = url;

      link.setAttribute("download", decodeURIComponent(data.headers.filename));
      document.body.appendChild(link);
      link.click();
    },

    //补充远程用，临时当修改用
    AddRemoteTemp(row) {
      AddRemoteId = row.id;
      this.AddRemoteDialog = true;
    },

    //设置远程
    setRemote(formName) {
      if (
        this.set_project_remote != "" &&
        this.set_project_remote_password != ""
      ) {
        putRequest("/VerifyBill/SetRemoteById", {
          id: AddRemoteId,
          remote: this.set_project_remote,
          remote_password: this.set_project_remote_password,
        }).then((resp) => {
          this.$notify({
            title: "成功",
            type: "success",
            message: "设置成功!",
          });
          this.loadVsersionInfo();
        });
      }
      this.AddRemoteDialog = false;
    },
    //选择上传文件后触发
    onUploadChange(file) {
      var fileName = file.name.substring(file.name.lastIndexOf(".") + 1);
      if (fileName != "xls" && fileName != "xlsx") {
        this.$message({
          type: "error",
          showClose: true,
          duration: 3000,
          message: "文件类型不是excel文件!",
        });
        this.fileList.pop();
        return false;
      }
      // const isEXCEL =
      //   file.raw.type ===
      //     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ||
      //   file.raw.type === "application/vnd.ms-excel";
      const isLt1M = file.size / 1024 / 1024 < 1;
      // if (!isEXCEL) {
      //   this.$message.error("上传文件只能是excel格式!");
      //   return false;
      // }
      if (!isLt1M) {
        this.$message.error("上传文件大小不能超过1MB!请删掉不必要的统计数据");
        return false;
      }
      this.fileList.pop();
      this.fileList.push(file.name);
      if (file.raw.name != "") this.hasFileUpload = true;
      else this.hasFileUpload = false;
    },
    handleRemove(file, fileList) {
      this.hasFileUpload = false;
      //this.fileList.pop();
      this.fileList = fileList;
    },
    //关键字搜索会话
    searchClick() {
      this.loadVsersionInfo();
    },

    addVersionInfo() {
      this.dialogVisible = true;
    },
    VersionFormat(row, column) {
      if (row.projectBigVersion == 0) {
        return "JieLink";
      } else if (row.projectBigVersion == 1) {
        return "G3-标准版";
      } else if (row.projectBigVersion == 2) {
        return "G3-速通版";
      }
    },
    IsNonstandardFormat(row, column) {
      if (row.projectIsNonstandard == 0) {
        return "否";
      } else if (row.projectIsNonstandard == 1) {
        return "是";
      }
    },
    statusFormat(row, column) {
      if (row.status == 0) {
        return "需要提供远程";
      } else if (row.status == 1) {
        return "等待研发处理";
      } else if (row.status == 2) {
        return "生成语句,自助处理";
      } else if (row.status == 3) {
        return "研发已处理";
      } else if (row.status == 4) {
        return "已完成";
      }
    },
    EmergencyFormat(row, column) {
      var temp = row.emergency;
      var ret = "";
      if (temp > 10) temp = 10;
      while (temp > 5) {
        ret += "★";
        temp -= 1;
      }
      temp -= ret.length;
      while (temp > 0) {
        ret += "☆";
        temp -= 1;
      }
      if (ret == "") ret = "♫";
      return ret;
    },
    //加载会话数据
    loadVsersionInfo() {
      var start = (this.page - 1) * this.limit;
      var end = this.page * this.limit;
      let _this = this;
      _this.loading = true;
      getRequest("/VerifyBill/getVerifyBillInfoWithPages", {
        orderNo: this.keywords,
        start: start,
        end: end,
      }).then(
        (resp) => {
          this.versionInfos = resp.data.items;
          this.total = resp.data.total;
          _this.loading = false;
        },
        (resp) => {
          if (resp.status == 403) {
            _this.$notify({
              title: "错误",
              type: "error",
              message: resp.data.msg,
            });
          }
          _this.loading = false;
        }
      );
    },

    beforeUpload(file) {
      // m_filename = file.name;
      var that = this;
      that.downloadLoading = that.$loading({
        lock: true,
        text: "文件上传中...",
        spinner: "el-icon-loading",
        background: "rgba(0,0,0,0.7)",
      });
      let fd = new FormData();
      fd.append("file", file);
      fd.append("_t1", new Date());
      axios({
        method: "post",
        url: "/VerifyBill/upload/" + new Date().getTime(),
        data: fd,
        headers: {
          "Content-Type":
            "multipart/form-data;boundary=" + new Date().getTime(),
        },
      })
        .then((rsp) => {
          that.downloadLoading.close();
          that.uploadLoading = false;
          let resp = rsp.data;
          if (resp.resultCode == 200) {
            that.uploadTemplateDialog = false;
            that.$message.success(resp.resultMsg);
            //that.queryData();//更新数据
          } else {
            that.uploadTemplateDialog = false;
            // that.$message({
            //   type: "error",
            //   showClose: true,
            //   duration: 60000,
            //   message: resp.resultMsg,
            // });
          }
        })
        .catch((error) => {
          that.downloadLoading.close();
          that.uploadLoading = false;
          that.uploadTemplateDialog = false;
          that.$message({
            type: "error",
            showClose: true,
            duration: 60000,
            message: "请求失败! error:" + error,
          });
        });
      return false;
    },

    submitUpload() {
      this.uploadLoading = true;
    },
    //提交数据
    submitForm(formName) {
      if (this.ruleForm.checkList.length == 0) {
        this.$message({
          showClose: true,
          message: "请至少勾选一项任务！",
          type: "error",
        });
        return;
      }
      //要有选择上传文件 TODO 这里好像出问题了
      if (this.hasFileUpload == false) {
        this.$message({
          showClose: true,
          message: "请选择对账差异excel表格！",
          type: "error",
        });
        return;
      }

      //获取登录用户名
      var username = "";
      getRequest("user/currentUserName").then(
        (msg) => {
          username = msg.data;

          //上传表单
          this.$refs[formName].validate((valid) => {
            if (valid) {
              //this.$refs.upload.submit();
              postRequest("/VerifyBill/addVerifyBillInfo", {
                publisher_name: username,
                project_shoper_no: this.ruleForm.project_shoper_no,
                project_no: this.ruleForm.project_no,
                project_name: this.ruleForm.project_name,
                versionType: this.ruleForm.versionType,
                version: this.ruleForm.version,
                nonstandard: this.ruleForm.nonstandard,
                project_remote: this.ruleForm.project_remote,
                project_remote_password: this.ruleForm.project_remote_password,
                describe: this.ruleForm.describe,
                project_task: this.ruleForm.checkList.toString(),
                uploadfilename: this.fileList.toString(),
              }).then(
                (resp) => {
                  //上传文件  上传完表单上传文件
                  this.$refs.upload.submit();

                  this.$notify({
                    title: "成功",
                    message: "发布成功",
                    type: "success",
                  });
                  this.dialogVisible = false;
                  this.$refs[formName].resetFields();
                  this.loadVsersionInfo();
                },
                (resp) => {
                  if (resp.status == 403) {
                    _this.$notify({
                      title: "错误",
                      type: "error",
                      message: resp.data.msg,
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
        (msg) => {
          this.$message({
            showClose: true,
            message: "登录已过期，需要重新登录",
            type: "error",
          });
          return;
        }
      );
    },

    resetForm(formName) {
      this.$refs[formName].resetFields();
    },
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
      AddRemoteDialog: false,
      SQLDialog: false,
      SQLdialogVisible: false,
      idVisible: false,
      keywords: null,
      versionInfos: [],
      total: 0, //数据总条数
      page: 1, //默认显示第1页
      limit: 5, //默认一次显示5条数据
      hasFileUpload: false, //是否选择了excel文件上传
      fileList: [],
      set_project_remote: "",
      set_project_remote_password: "",
      //AutoSQL:"",
      textarea: "",
      ruleForm: {
        project_shoper_no: "",
        project_no: "",
        project_name: "",
        versionType: "",
        version: "",
        nonstandard: "",
        project_remote: "",
        project_remote_password: "",
        describe: "",
        taskstring: "",
        uploadFileName: "",
        checkList: [],
        //TODO
      },

      rules: {
        project_shoper_no: [
          { required: true, message: "请输入商户号", trigger: "blur" },
        ],
        project_no: [
          { required: true, message: "请输入项目编号", trigger: "blur" },
        ],
        project_name: [
          { required: true, message: "请输入项目名", trigger: "blur" },
        ],
        versionType: [
          { required: true, message: "请选择项目类型", trigger: "blur" },
        ],
        version: [{ required: false, trigger: "blur" }],
        nonstandard: [
          {
            required: true,
            message: "请选择是否非标，不知道选否即可！",
            trigger: "blur",
          },
        ],
        project_remote: [
          {
            required: true,
            message: "请输入向日葵远程，无远程可以一键搜索数据库",
            trigger: "blur",
          },
        ],
        project_remote_password: [
          { required: true, message: "请输入远程密码", trigger: "blur" },
        ],
        describe: [{ required: true, message: "请输入描述", trigger: "blur" }],
        //项目任务判断 TODO
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
            },
          },
          {
            text: "昨天",
            onClick(picker) {
              const date = new Date();
              date.setTime(date.getTime() - 3600 * 1000 * 24);
              picker.$emit("pick", date);
            },
          },
          {
            text: "一周前",
            onClick(picker) {
              const date = new Date();
              date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
              picker.$emit("pick", date);
            },
          },
        ],
      },
    };
  },
};
</script>
<style>
@import "../assets/common.css";
</style>>