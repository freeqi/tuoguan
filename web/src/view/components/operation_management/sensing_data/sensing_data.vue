<template>
    <div id="medical">
        <div  class="sensingContent">
          <ul class="tab-panel">
            <li
              class="tab-panel-item"
              v-for="item in tabData"
              :key="item.id"
              @click="changeTab(item.id,item.index,item.name)"
              :class="tabCheckedInfo.name == item.name ? 'active' : ''"
            >{{item.name}}</li>
          </ul>
        
          <div class="table2">
            <div>
              <div class="btn-groups">

                <Form 
                inline 
                :label-width="80" 
                label-position="left"
                :modal="sensingData">
                    <FormItem  label="日期选择:">
                        <DatePicker
                        confirm 
                  :value="dateRange"
                  placeholder="按创建时间筛选"
                  @on-change="changeDate"
                  @on-ok="changeDate1"
                  @on-clear="clearDate"
                  type="datetimerange"
                  :options="dateRangeOptions"
                ></DatePicker>
                    </FormItem>

                    <FormItem label="机构选择:">

                        <Select v-model="sensingData.centerId" style="width:200px" @on-change="changeCenter">
                        <Option v-for="item in centerIdList" :value="item.value" :key="item.label">{{ item.label }}</Option>
                        </Select>

                    </FormItem>

                </Form>

                <!-- <span>日期选择</span>
                <DatePicker
                  :value="dateRange"
                  placeholder="按创建时间筛选"
                  @on-change="changeDate"
                  type="datetimerange"
                  :options="dateRangeOptions"
                ></DatePicker> -->

                <!-- <Button type="primary" style="margin-left: 10px;" @click="exportTable">导出</Button> -->
              </div>
              <div class="table2-content" v-show="!table_detail_flag">
                <Table fixed :columns="table_columns" height="700" ref="table" :data="table_data" :loading="loading"></Table>
                <!-- <div class="pagination" v-if="dataCount > pageSize">
                  <Page
                    transfer
                    :total="dataCount"
                    :page-size="pageSize"
                    :current.sync="pageIndex"
                    @on-change="handleChangePage"
                  />
                </div> -->
              </div>
            </div>
          
          </div>
        </div>
    </div>
  </template>
  
  <script>
//   import { accAdd, subtract } from '@/libs/tools'
//   import columns from '../medical/columns.js'
import {handleTime} from "@/libs/tools"
  export default {
    // mixins: [columns],
    data () {
      return {

        // centerId:"abc3d60b474c4fef946a66887348c41a",
        centerIdList:[],
        formVal:[],
        tableHeight: 0,
        // dataCount: 0,
        // pageSize: 9,
        // pageIndex: 1,

        sensingData:{
            projectName: "",
            centerId: "abc3d60b474c4fef946a66887348c41a",
            startTime: "",
            endTime: ""
        },

  
        tableDetailLoading: false,
        table_detail_flag: false,
  
        prescriptionTotalMoney: 0,
  
        dateRange: [], // cy时间段筛选
        beginTime: '',
        endTime: '',
        dataFilterType: 1, // cy时间快捷筛选
        dataFilterTypeList: [
          { id: 0, label: '全部' },
          { id: 1, label: '近一个月' },
          { id: 2, label: '近三个月' }
        ],
        dateRangeOptions: {
          disabledDate (date) {
            return date && date.valueOf() > Date.now()
          }
        },
        medicalRecordChecked: {
          i: 0,
          id: '',
          recordName: ''
        },
        tabCheckedInfo: {
          id: 1,
          name: ''
        },
        tabData: [],
        // -------------------
        // cy被选择的机构、患者信息
        medicalCheckedInfo: {
          hospitalId: '',
          hospitalName: '',
          patientId: '',
          patientName: ''
        },
  
        table_columns: [],
        table_data: [],
        formDetail: [],
  
        loading: false,
        // pageSize: 12,
        // startPage: 1,
        // searchKey: '',
        api: 'Patients/Patients',
        // 病历类型，及选中id
        medicalType: 1,
        medicalTypeList: [
          {label: '病历首页', value: 1},
          {label: '专用病历', value: 2},
          {label: '门诊病历/透析记录单', value: 3},
          {label: '透析方案调整', value: 4},
          {label: '阶段小结', value: 5},
          {label: '护理评估记录', value: 6},
          {label: '健康宣教', value: 7},
          {label: '营养评估', value: 8}
        ],
  
      }
    },
    computed: {
      ducumentApi () {
        // 解决缓存问题
        let baseUrl = process.env.NODE_ENV === 'development'
          ? this.$config.baseURL.dev
          : this.$config.baseURL.pro
        return `${baseUrl}Document/Patientsfileview/${this.medicalRecordChecked.id}/${this.medicalType}?date=${new Date().getTime()}`
      }
    },
    created(){
        this.getcenterIdList()

    // this.GetCategoryLook()
    // this.changeTab('def9da41kS5HI3874150768152023040',1,'电解质七项')
    },
    mounted () {
      this.$nextTick(() => {
        this.GetCategoryList();

        // this.changeTab()
        // this.changeTab(this.tabData[0].id,this.tabData[0].index,this.tabData[0].name)

      })
    },
    methods: {

        /* 
            new
        */

        //获取所有检验项目分类配置
        GetCategoryList(){
            this.swsApi.swsGet('Patients/GetSensingDataGroupByProjectName').then(res=>{
                console.log(res);
                if(res.status == 200){
                    this.tabData = res.data.result.map((item,index)=>{
                        return {id:index+1,name:item.projectName,index:index+1}
                    })
                    console.log(this.tabData);

                    setTimeout(() => {
                        this.changeTab(this.tabData[0].id,this.tabData[0].index,this.tabData[0].name)
                    }, 100);
                }
            })
        },
        //根据检验项目查询  Patients/GetLaboratoryCategorySetting
        // GetCategoryLook(id){
        //     this.swsApi.swsGet(`Patients/GetLaboratoryCategorySetting/${id}`).then(res=>{
        //         if(res.data.success){
        //             console.log(res);
        //             this.table_columns = res.data.result.map(item=>{
        //                 return {title:item.displayName,key:item.id,minWidth:80,align:'center'}
        //             })

        //             this.table_columns.unshift( { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center",fixed:"left" })
        //             // this.table_columns.push( { title: "操作", key: "handle", minWidth: 110, align: "center",fixed: "right"})
        //             console.log(this.table_columns);
        //         }
                
        //     })
        // },
    //     exportTable() {
    //   this.$refs.table.exportCsv({
    //     filename: this.medicalCheckedInfo.hospitalName+this.tabCheckedInfo.name+'-'+this.medicalCheckedInfo.patientName,
    //     columns: this.table_columns.filter((col, index) => {
    //       return index < this.table_columns.length-1;
    //     }),
    //     data: this.table_data.filter((data, index) => {
    //       if(data.CheckDate){
	// 		  data.CheckDate = data.CheckDate.substring(0,10);
	// 	  }
    //       return index < this.table_data.length;
    //     })
    //   });
    // },

      // cy 隐藏详情
      hide () {
        this.table_detail_flag = false
      },
      changeDate (arr) {
        // console.log(arr);
        if(!arr[0] && !arr[1]){
            // this.beginTime = '';
            // this.endTime = '';
            this.sensingData.startTime = '';
            this.sensingData.endTime = '';
            // console.log( this.sensingData);
            // this.getTableData()
        }else{
            // this.beginTime = new Date(arr[0]).toISOString()
            // this.endTime = new Date(arr[1]).toISOString()
            this.sensingData.startTime = new Date(handleTime(arr[0])).toISOString()
            this.sensingData.endTime = new Date(handleTime(arr[1])).toISOString()
            // console.log( this.sensingData);

            // this.getTableData()
        }
      },
      changeDate1(){
        console.log(this.sensingData.startTime);
        console.log(this.sensingData.endTime);
        this.getTableData()

      },
      clearDate(){
        this.sensingData.startTime = '';
        this.sensingData.endTime = '';
        this.getTableData()
      },
      // cy 切换标签
      changeTab (id,index,name) {
        this.sensingData.projectName = name;
        name = name || "SC-细菌内毒素(临检)"
        this.tabCheckedInfo = {
          id: id,
          name: name
        }
        this.getTableData();
      },
   
      getTableData () {
        this.table_data = []
        this.loading = true

            this.swsApi
            .swsPost('Patients/GetSensingDataByProjectName',this.sensingData)
            .then(res => {
                console.log(res);

              if (res.data.code == 200 && res.data.result.length>0) {
                console.log(res);
                this.table_data = res.data.result
                this.formVal = Object.keys(this.table_data[0]);
                console.log(this.formVal);
                this.formVal.shift();
                let columns = [];
                this.formVal.forEach((item,i)=>{
            let obj = {
              key: item,
              title: item,
              minWidth: 120
            }
            if(i<3){
              if(item == 'CheckDate'){
                obj.title = '送检时间';
                columns[0] = obj;
              }
              if(item == 'Region'){
                obj.title = '区域';
                columns[1] = obj;
              }
              if(item == 'ReportNo'){
                obj.title = '条码号';
                columns[2] = obj;
              }
            }else columns.push(obj);

            console.log(obj);

          })
          console.log(columns);
        //   columns.push({ title: '操作', key: 'action', slot: 'action', align: 'center',fixed: 'right',width:65 });
          this.table_columns = columns;


                // this.dataCount = res.data.dataCount
                console.log(this.table_data);
              }
              this.loading = false
            })
            .catch(e => {
              this.$Notice.error({
                title: '请求错误,请稍后再试',
                desc: e
              })
              this.loading = false
            })
      },
      handleChangePage (i) {
        this.pageIndex = i
        this.getTableData()
      },


      //new
      //获取机构的信息
      getcenterIdList(){
        this.swsApi.swsPost("Employee/GetEmplyeeByDialysisList").then(res=>{
            console.log(res);
            if(res.status == 200){
                let { result } = res.data;
                result.shift();//删除第一个
               result.forEach(item=>{
                let obj = {"value":item.dialysisId,"label":item.dialysisName}
                this.centerIdList.push(obj)
                })
                console.log(this.centerIdList);
            }
        })
      },
      //机构改变
      changeCenter(value){
        console.log(value);
        this.sensingData.centerId = value;
        console.log(this.sensingData);
        this.getTableData();
      }

    }
  }
  </script>
  
  <style scoped lang="less">
  #medical {
    position: relative;
    height: 100%;
    padding: 0 20px 20px 20px;
    /deep/ .content {
      // margin-right: 20px;
      width: 100%;
      // background: #ffffff;
      overflow-y: auto;
    }
    .tab-panel {
      position: sticky;
      top: 0;
      z-index: 4;
      &-item {
        position: relative;
        display: inline-block;
        font-size: 11px;
        // width: 90px;
        height: 32px;
        line-height: 32px;
        text-align: center;
        background-color: #ffffff;
        border: solid 1px #eaeaea;
  
        border-left: none;
        cursor: pointer;
        &:first-child {
          border-left: solid 1px #eaeaea;
          &.active {
            border-left-color: transparent;
          }
        }
        &.active {
          color: #ee5151;
          border-bottom: none;
          border-top: solid 1px #ff6e5c;
        }
        &:hover {
          color: #ee5151;
        }
      }
    }
    .table {
      background: #ffffff;
      overflow-y: auto;
      padding: 20px 20px 40px;
      .right-group {
        float: right;
        cursor: pointer;
        line-height: 37px;
        font-size: 14px;
        margin-right: 30px;
        color: #5a9be9;
      }
      .top-btn-group {
        span {
          margin-right: 20px;
          width: 60px;
          text-align: right;
        }
      }
      .table-content {
        // margin-top: 20px;
        .pdf-box {
          
          h4 {
            display: inline-block;
            padding-left: 10px;
            border-left: 3px solid #3399ff;
            margin: 15px 10px 0 0;
            font-size: 13px;
            line-height: 13px;
          }
          .pdf-list {
            float: right;
            color: #333333;
            width: 183px;
            font-size: 14px;
            text-align: center;
            .pdf-list-box {
              margin-top: 20px;
              padding: 10px 15px;
              border-radius: 4px;
              border: solid 1px #eaeaea;
              font-size: 13px;
              text-align: left;
              .pdf-list-text {
                color: #555555;
                // font-weight: 600;
                border-bottom: 1px dashed #ddd;
                line-height: 33px;
                &:hover {
                  background: #f0f0f0;
                  cursor: pointer;
                }
              }
              .active {
                background: #f0f0f0;
              }
            }
            &.disable + * {
              .default-table {
                margin-left: 0 !important;
              }
            }
          }
        }
      }
    }
    .table2 {
      flex:1;
      position: relative;
      padding: 20px 10px 1px;
      background: #ffffff;
      .ivu-table-wrapper {
        & /deep/ .ivu-table .ivu-table-cell {
          padding-right: 8px;
          padding-left: 8px;
        }
      }
      h2 {
        text-align: center;
        font-size: 18px;
        font-weight: 500;
        margin-bottom: 20px;
      }
      .btn-groups {
        color: #999;
        font-size: 13px;
        span {
          margin-right: 20px;
          display: inline-block;
          // width: 60px;
          text-align: right;
        }
        & + .btn-groups {
          margin: 20px 0;
        }
        & > * {
          margin-right: 20px;
        }
        /deep/ .ivu-radio-group-button .ivu-radio-wrapper-checked {
          color: #ffffff;
          background: #4f95e8;
        }
      }
    //   .table2-content {
    //     margin-top: 20px;
    //   }
      .table_detail {
        position: absolute;
        background: #ffffff;
        top: 42px;
        left: 0;
        overflow-y: auto;
        padding: 0 20px;
        z-index: 4;
        width: 100%;
        .head_content {
          .record {
            margin: 20px 0 10px;
            color: #333333;
            font-size: 16px;
            span {
              margin-left: 10px;
            }
            .right {
              float: right;
              .approval {
                background: #f90;
                color: #ffffff;
                border-color: #f90;
              }
            }
            &::after {
              display: block;
              content: '';
              height: 0;
              visibility: hidden;
              clear: both;
            }
          }
          .items-box {
            font-size: 14px;
            .field {
              color: #666666;
              & + .red {
                color: #fc4b4b;
              }
            }
            span:not(.field) {
              font-weight: bold;
              margin-right: 50px;
            }
          }
        }
      }
    }
  }
  .color-gray {
    color: #999;
  }
  .empty {
    margin-top: 20px;
    font-size: 14px;
    line-height: 80px;
    text-align: center;
  }
  .status-text-success {
    color: #19be6b;
  }
  .status-text-error {
    color: #ed4014;
  }
  /deep/ .ivu-table {
    .status-text-success {
      .status-text-success;
    }
    .status-text-error {
      .status-text-error;
    }
  }
  /deep/ .tab-panel{
    background: #fff;
  }
  .sensingContent{
    height:100%;
    background: #fff;
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
  }
  /deep/ .ivu-form-inline .ivu-form-item{
    margin-right: 30px;
    margin-left: 10px;
  }
  /deep/ .ivu-form .ivu-form-item-label{
    font-size: 14px;
    font-weight: 600;
  }
  </style>
  