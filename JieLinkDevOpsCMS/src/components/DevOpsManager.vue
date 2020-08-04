<template>
  <div id="app">
    <Header></Header>
    <el-container>
      <el-header class="report_header">
        <el-select
          style="width: 400px"
          prefix-icon="el-icon-search"
          v-model="keywords"
          placeholder="请选择事件类型"
        >
          <el-option label="内存溢出" value="1"></el-option>
          <el-option label="CPU高" value="2"></el-option>
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
        <el-table v-loading="loading" :data="events" border style="width: 100%">
          <el-table-column v-if="idVisible" prop="id" label="主键ID" width="100"></el-table-column>
          <el-table-column
            fixed="left"
            prop="eventType"
            label="事件类型"
            :formatter="eventTypeFormat"
            width="120"
          ></el-table-column>
          <el-table-column prop="remoteAccount" label="远程账号" width="300"></el-table-column>
          <el-table-column prop="contactName" label="联系人姓名" width="100"></el-table-column>
          <el-table-column prop="contactPhone" label="联系人电话" width="120"></el-table-column>
          <el-table-column prop="operatorDate" label="入库时间" width="200"></el-table-column>
          <el-table-column
            prop="isProcessed"
            label="处理状态"
            :formatter="processStatusFormat"
            width="100"
          ></el-table-column>
          <el-table-column label="操作" width="150">
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
</template>

<script>
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
        return "内存溢出";
      } else if (row.eventType == 2) {
        return "CPU高";
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
      events: [],
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