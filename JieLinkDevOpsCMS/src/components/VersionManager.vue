<template>
  <div id="app">
    <Header></Header>
    <div id="v-content" v-bind:style="{minHeight: Height+'px'}">
      <el-container>
        <el-header class="report_header">
          <el-input
            placeholder="请输入项目编号..."
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
          <el-dialog title="会话参与方状态" @opened="dialogOpened" :visible.sync="dialogVisible">
            <el-tabs
              :tab-position="tabPosition"
              v-model="activeTab"
              @tab-click="tabClick"
              style="height: 260px;"
            >
              <el-tab-pane
                v-for="device in devices"
                :name="device.mediaId"
                :key="device.id"
                :label="device.deviceName"
              >
                <el-timeline :reverse="reverse">
                  <el-timeline-item
                    v-for="(activity, index) in activities"
                    :key="index"
                    :timestamp="activity.createTime"
                    size="large"
                  >{{timelineFormat(activity.messageType)}}</el-timeline-item>
                </el-timeline>
              </el-tab-pane>
            </el-tabs>
          </el-dialog>

          <el-table v-loading="loading" :data="versionInfos" border style="width: 100%">
            <el-table-column v-if="idVisible" prop="id" label="主键ID" width="100"></el-table-column>
            <el-table-column fixed="left" prop="workOrderNo" label="工单号" width="120"></el-table-column>
            <el-table-column prop="standVersion" label="版本号" width="120"></el-table-column>
            <el-table-column prop="versionType" :formatter="versionTypeFormat" label="版本类型" width="120"></el-table-column>
            <el-table-column prop="compileDate" label="编译时间" width="200"></el-table-column>
            <el-table-column prop="versionDescribe" label="版本描述" width="300"></el-table-column>
            <el-table-column  label="操作" width="100">
              <template slot-scope="scope">
                <el-button @click="handleClick(scope.row)" type="text" size="medium">申请</el-button>
              </template>
            </el-table-column>
          </el-table>
          <pagination
            v-show="total>0"
            :total="total"
            :page.sync="page"
            :limit.sync="limit"
            @pagination="loadSessions"
          />
        </el-main>
      </el-container>
    </div>
    <Footer>由jielink2.*团队提供技术支持</Footer>
  </div>
</template>

<script>import { postRequest } from "../utils/api";
import { getRequest } from "../utils/api";
import Pagination from "@/components/Pagination";
export default {
  components: { Pagination },
  methods: {
    //打开对话窗 请求对话参与方数据
    handleClick(row) {
      this.selItems = row;
      this.dialogVisible = true;
      let _this = this;
      _this.dialogLoading = true;
      getRequest("/sessionDevices/getDevicesBySessionId/" + row.id).then(
        resp => {
          _this.devices = resp.data;

          _this.dialogLoading = false;
        },
        resp => {
          if (resp.response.status == 403) {
            _this.$message({
              type: "error",
              message: resp.response.data
            });
          }
          _this.dialogLoading = false;
        }
      );
    },

    tabClick(tab) {
      this.loadMessageHistory(tab.name); //获取到当前设备ID
    },

    dialogOpened() {
      setTimeout(() => {
        let _this = this;
        _this.loadMessageHistory(_this.devices[0].mediaId);
        _this.activeTab = _this.devices[0].mediaId;
      }, 200);
    },

    loadMessageHistory(mid) {
      console.log("loadMessageHistory" + mid);
      let _this = this;
      _this.tabLoading = true;
      getRequest("/messageHistroy/getMessageHistoryBySIdAndMid", {
        sid: _this.selItems.id,
        mid: mid
      }).then(
        resp => {
          _this.activities = resp.data;
          _this.tabLoading = false;
        },
        resp => {
          if (resp.response.status == 403) {
            _this.$message({
              type: "error",
              message: resp.response.data
            });
          }
          _this.tabLoading = false;
        }
      );
    },
    //关键字搜索会话
    searchClick() {
      this.loadVsersionInfo();
    },

    versionTypeFormat(row, column) {
      if (row.versionType == 0) {
        return "工单";
      } else if (row.sessionStatus == 1) {
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
        orderNo:this.keywords,
        start: start,
        end: end
      }).then(
        resp => {
          this.versionInfos = resp.data.items;
          this.total = resp.data.total;
          _this.loading = false;
        },
        resp => {
          if (resp.response.status == 403) {
            _this.$message({
              type: "error",
              message: resp.response.data
            });
          }
          _this.loading = false;
        }
      );
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
      Height:0,
      loading: false,
      dialogLoading: false,
      tabLoading: false,
      tabPosition: "left",
      devices: [],
      dialogVisible: false,
      idVisible: false,
      keywords: null,
      selItems: "",
      versionInfos: [],
      reverse: true,
      activities: [],
      activeTab: "",
      total: 0, //数据总条数
      page: 1, //默认显示第1页
      limit: 10 //默认一次显示10条数据
    };
  }
};
</script>
<style>
@import "../assets/common.css";
</style>>