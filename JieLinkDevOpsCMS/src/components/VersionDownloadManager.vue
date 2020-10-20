<template>
    <el-container>
        <el-header class="report_header">
            <el-input placeholder="请输入版本号或者工单号关键字" 
                prefix-icon="el-icon-search" 
                v-model="keywords"
                style="width: 400px"
                size="medium">
            </el-input>
            <el-button type="primary" icon="el-icon-search" size="small" style="margin-left: 3px"  @click="searchClick">搜索</el-button>
        </el-header>
        <el-main class="report_main">
            <el-dialog :visible.sync="dialogVisible">
                <el-table border :data="applyInfoList" v-loading="dialogLoading">
                    <el-table-column prop="jobNumber" label="工号" min-width="80"></el-table-column>
                    <el-table-column prop="name" label="姓名" min-width="100"></el-table-column>
                    <el-table-column prop="cellPhone" label="手机号"  min-width="100"></el-table-column>
                    <el-table-column prop="email" label="邮箱" min-width="150"></el-table-column> 
                    <el-table-column prop="applyDate" label="申请日期" min-width="200"></el-table-column>          
                </el-table>
                <pagination v-show="dialogTotal>0" :total="dialogTotal" :page.sync="dialogPage" :limit.sync="dialogLimit" @pagination="loadApplyInfoList"></pagination>   
            </el-dialog>
            <el-table v-loading="loading"
                ref="versionInfoTable"  
                :data="versionInfoList"
                row-key="id"
                border
                style="width: 100%">               
                <el-table-column fixed="left" prop="children" type="expand">
                    <template slot-scope="slots">
                        <el-table border :data="slots.row.children">
                            <el-table-column prop="jobNumber" label="工号" min-width="80"></el-table-column>
                            <el-table-column prop="name" label="姓名" min-width="100"></el-table-column>
                            <el-table-column prop="cellPhone" label="手机号"  min-width="100"></el-table-column>
                            <el-table-column prop="email" label="邮箱" min-width="150"></el-table-column> 
                            <el-table-column prop="applyDate" label="申请日期" min-width="200"></el-table-column>          
                        </el-table>
                    </template>
                </el-table-column>
                <el-table-column fixed="left" v-if="idVisible" prop="id" label="主键ID" width="100"></el-table-column>
                <el-table-column prop="workOrderNo" label="工单号" width="300"></el-table-column>
                <el-table-column prop="standVersion" label="版本号" width="80"></el-table-column>
                <el-table-column prop="versionType" :formatter="versionTypeFormat" label="版本类型" width="120"></el-table-column>
                <el-table-column prop="downloadCount" label="下载次数" width="80"></el-table-column>
                <el-table-column prop="compileDate" label="编译时间" width="200"></el-table-column>
                <el-table-column prop="versionDescribe" label="版本描述" width="400"></el-table-column>    
                <el-table-column label="操作" width="100">
                    <template slot-scope="scope">
                        <el-button icon="el-icon-user" type="primary" size="small" @click="getDetails(scope.row)">详情</el-button>
                    </template>
                </el-table-column>                 
            </el-table>
            <pagination v-show="total>0" :total="total" :page.sync="page" :limit.sync="limit" @pagination="loadVsersionInfoList"></pagination>
        </el-main>
    </el-container>
</template>
<style>
@import '../assets/common.css';
</style>
<script>
import { postRequest } from "../utils/api";
import { getRequest } from "../utils/api";
import Pagination from "@/components/Pagination";
export default {
    components: { Pagination },
    methods:{
        versionTypeFormat(row, column) {
            if (row.versionType == 0) {
                return "工单";
            } else if (row.versionType == 1) {
                return "补丁";
            }else if(row.versionType == 2) {
                return "文档";
            }
        },
        //关键字搜索会话
        searchClick() {
            this.loadVsersionInfoList();
        },
        getDetails(row){                   
            this.loadApplyInfoList(row);           
            this.dialogVisible = true;
        },        
        loadVsersionInfoList(){
            var startIndex = (this.page - 1) * this.limit;
            var endIndex = this.page * this.limit;  
            this.loading = true;
            getRequest("/version/getVersionDownloadInfoWithPages", {
                standVersion: this.keywords,
                startIndex: startIndex,
                endIndex: endIndex
            })
            .then(
                response=>{
                    this.versionInfoList = response.data.items;
                    this.total = response.data.total;
                    this.loading = false;
                },
                response => {
                    if (response.status == 403) {
                        this.$notify({
                            title: "错误",
                            type: "error",
                            message: response.data.msg
                        });
                    }
                    this.loading = false;
                }
            )
        },
        loadApplyInfoList(row){
            var startIndex = (this.dialogPage - 1) * this.dialogLimit;
            var endIndex = this.dialogPage * this.dialogLimit;  
            this.dialogLoading = true;
            getRequest("/version/getApplyInfoByWorkOrderNoWithPages", {
                versionInfoKeyId: row.id,
                startIndex: startIndex,
                endIndex: endIndex,
                enableAll: 1,   //是否查询出所有数据 0所有（不看start、end），1分页
            })
            .then(
                response=>{
                    this.applyInfoList = response.data.items;
                    this.dialogTotal = response.data.total;
                    this.dialogLoading = false;
                },
                response => {
                    if (response.status == 403) {
                        this.$notify({
                            title: "错误",
                            type: "error",
                            message: response.data.msg
                        });
                    }
                    this.dialogLoading = false;
                }
            )
        }
    },
    mounted(){   //页面初始化方法
		this.loadVsersionInfoList();
	},
    data(){
        return{
            Height: 0,
            loading: false,
            idVisible: false,
            keywords: null,
            versionInfoList: [],
            total: 0, //数据总条数
            page: 1, //默认显示第1页
            limit: 5, //默认一次显示5条数据   

            dialogVisible: false,
            dialogLoading: false,
            applyInfoList:[],   
            dialogTotal: 0, //数据总条数
            dialogPage: 1, //默认显示第1页
            dialogLimit: 5, //默认一次显示5条数据      
        };
    }
}
</script>
