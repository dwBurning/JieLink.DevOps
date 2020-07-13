<template>
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

      <el-table v-loading="loading" :data="sessions" border style="width: 100%">
        <el-table-column v-if="sessionIdVisible" prop="id" label="主键ID" width="300"></el-table-column>
        <el-table-column prop="projectId" label="项目ID" width="120"></el-table-column>
        <el-table-column prop="sessionId" label="会话ID" width="300"></el-table-column>
        <el-table-column prop="sessionStatus" :formatter="stateFormat" label="会话状态" width="120"></el-table-column>
        <el-table-column prop="createAuthor" label="创建者" width="120"></el-table-column>
        <el-table-column prop="closeAuthor" label="挂断者" width="120"></el-table-column>
        <el-table-column prop="createTime" label="创建时间" width="180"></el-table-column>
        <el-table-column prop="updateTime" label="更新时间" width="180"></el-table-column>
        <el-table-column prop="remark" label="备注" width="180"></el-table-column>
        <el-table-column fixed="right" label="操作" width="100">
          <template slot-scope="scope">
            <el-button @click="handleClick(scope.row)" type="text" size="medium">详情</el-button>
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
      this.loadSessions();
    },

    stateFormat(row, column) {
      if (row.sessionStatus == 0) {
        return "开始呼叫";
      } else if (row.sessionStatus == 1) {
        return "接通";
      } else if (row.sessionStatus == 2) {
        return "开始挂断";
      } else if (row.sessionStatus == 3) {
        return "正常挂断";
      } else if (row.sessionStatus == 4) {
        return "异常挂断";
      }
    },
    timelineFormat(state) {
      if (state == 0) {
        return "开始呼叫";
      } else if (state == 1) {
        return "接通";
      } else if (state == 2) {
        return "开始挂断";
      } else if (state == 3) {
        return "正常挂断";
      } else if (state == 4) {
        return "异常挂断";
      }
    },
    //加载会话数据
    loadSessions() {
      var start = (this.page - 1) * this.limit;
      var end = this.page * this.limit;
      let _this = this;
      _this.loading = true;
      getRequest("/session/getSessionsWithPages", {
        projectId:this.keywords,
        start: start,
        end: end
      }).then(
        resp => {
          this.sessions = resp.data.items;
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
  mounted: function() {
    this.loadSessions();
  },
  data() {
    return {
      loading: false,
      dialogLoading: false,
      tabLoading: false,
      tabPosition: "left",
      devices: [],
      dialogVisible: false,
      sessionIdVisible: false,
      keywords: null,
      selItems: "",
      sessions: [],
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