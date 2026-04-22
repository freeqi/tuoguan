<template>
  <div id="medical">
    <medical-record-list-temp :api="api" @on-change="changePatient">
      <div slot="content" style="height:95%;background: #fff;">
        <ul class="tab-panel">
          <li
            class="tab-panel-item"
            v-for="item in tabData"
            :key="item.id"
            @click="changeTab(item.id)"
            :class="tabCheckedInfo.id == item.id ? 'active' : ''"
          >{{item.name}}</li>
        </ul>
        <div class="table" v-if="tabCheckedInfo.id<=1">
          <div class="top-btn-group">
            <span>选择{{tabCheckedInfo.name}}类型</span>
            <RadioGroup v-model="medicalRecordChecked.i" @on-change="changeRecordType">
              <Radio
                v-for="item in medicalRecordList"
                :label="item.i"
                :key="item.i"
              >{{item.recordName}}</Radio>
            </RadioGroup>
            <div class="right-group">
              <Icon type="md-download" size="18"/>下载全部文档
            </div>
          </div>
          <div class="table-content">
            <div class="pdf-box">
              <h4>{{medicalRecordChecked.recordName}}</h4>
              <a
                :href="`${baseUrl}Document/fileview/${medicalRecordChecked.id}`"
                target="blank"
              >全屏预览</a>
              <Divider style="margin:10px 0 15px;"/>
              <div class="pdf-view">
                <object
                  :data="`${baseUrl}Document/fileview/${medicalRecordChecked.id}`"
                  type="application/pdf"
                  width="73%"
                  height="600"
                >
                  <p>
                    抱歉，您的浏览器不支持PDF预览，请点击链接下载PDF文件
                    <a
                      :href="`${baseUrl}Document/fileview/${medicalRecordChecked.id}`"
                    ></a>
                  </p>
                </object>

                <div class="pdf-list">
                  <span>历史记录</span>
                  <div class="pdf-list-box">
                    <p
                      class="pdf-list-text"
                      :class="medicalRecordChecked.id === item.id ? 'active': ''"
                      v-for="item in historyRecordList"
                      :key="item.id"
                      @click="changeRecord(item)"
                    >{{item.date}}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="table2" v-else>
          <h2>{{medicalCheckedInfo.hospitalName}}{{tabCheckedInfo.name}}{{`-${medicalCheckedInfo.patientName}`}}</h2>
          <div>
            <div class="btn-groups">
              <!-- <span>状态</span>
              <RadioGroup v-model="dataFilterType" type="button" @on-change="changeDateType">
                <Radio :label="item.id" :key="item.id" v-for="item in dataFilterTypeList">{{item.label}}</Radio>
              </RadioGroup>-->
              <span>日期选择</span>
              <DatePicker
                :value="dateRange"
                placeholder="按创建时间筛选"
                @on-change="changeDate"
                type="daterange"
                :options="dateRangeOptions"
              ></DatePicker>
            </div>
            <div class="table2-content" v-show="!table_detail_flag">
              <Table fixed :columns="table_columns" :data="table_data" :loading="loading"></Table>
              <div class="pagination" v-if="dataCount > pageSize">
                <Page
                  transfer
                  :total="dataCount"
                  :page-size="pageSize"
                  :current.sync="pageIndex"
                  @on-change="handleChangePage"
                />
              </div>
            </div>
          </div>
          <div class="table_detail" v-show="table_detail_flag">
            <div class="head_content">
              <Button type="default" @click="hide" icon="md-undo">返回</Button>
              <p class="record">
                {{tabCheckedInfo.name}}号：{{formDetail.prescriptionNo || formDetail.balanceNo}}
                <span
                  v-if="formDetail.chargeStatus === '1'"
                  class="default"
                >未收费</span>
                <span v-else-if="formDetail.chargeStatus === '2'" class="status-text-success">已收费</span>
                <span v-else-if="formDetail.chargeStatus === '3'" class="Success">部分退费</span>
                <span v-else-if="formDetail.chargeStatus === '4'" class="error">全部退费</span>
                <span v-else-if="formDetail.balanceState === 0" class="error">已退费</span>
                <span v-else-if="formDetail.balanceState === 1" class="status-text-success">正常结算</span>
                <span class="right button-group">
                  <!-- <Button type="primary" size="large" @click="">打印申请单</Button> -->
                </span>
              </p>
              <p class="items-box">
                <span class="field">划价时间：</span>
                <span>{{formateDateToString(new Date(formDetail.balanceDate ? formDetail.balanceDate: formDetail.chargeDate), 'yyyy-MM-dd hh:mm:ss')}}</span>
                <span class="field">姓名：</span>
                <span>{{formDetail.patientName}}</span>
                <span class="field">总金额：</span>
                <span class="red">￥{{ formDetail.sumPrice|| prescriptionTotalMoney || '/'}}</span>
              </p>
            </div>
            <Divider></Divider>
            <div ref="detailTable">
              <Table
                :columns="table_detail_columns"
                :data="table_detail_data"
                :loading="tableDetailLoading"
                :height="tableHeight"
              ></Table>
            </div>
          </div>
        </div>
      </div>
    </medical-record-list-temp>
  </div>
</template>

<script>
import { accAdd, subtract } from '@/libs/tools'
import medicalRecordListTemp from './medical_record_list.vue'
import columns from './columns.js'
export default {
  mixins: [columns],
  components: {
    medicalRecordListTemp
  },
  data () {
    return {
      tableHeight: 0,
      dataCount: 0,
      pageSize: 9,
      pageIndex: 1,

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
      // ---------------文档/病案 + 模拟数据
      medicalRecordChecked: {
        i: 0,
        id: '',
        recordName: ''
      },
      medicalRecordList: [],

      tabCheckedInfo: {
        id: 1,
        name: ''
      },
      tabData: [
        // {
        //   id: 1,
        //   name: '医疗文书'
        // },
        {
          id: 2,
          name: '收费清单'
        },
        {
          id: 3,
          name: '处方单'
        }
      ],
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
      baseUrl:
        process.env.NODE_ENV === 'development'
          ? this.$config.baseURL.dev
          : this.$config.baseURL.pro,
      pdfUrl: ''
    }
  },
  mounted () {
    this.$nextTick(() => {
      this.changeTab()
    })
  },
  methods: {
    // cy 隐藏详情
    hide () {
      this.table_detail_flag = false
    },
    // cy 查看表单详情
    showDetail (row) {
      this.table_detail_data = []
      this.table_detail_flag = true
      this.formDetail = row
      let params = {}
      let detailUrl = ''
      if (this.tabCheckedInfo.id === 2) {
        params = { balanceNo: row.balanceNo }
        detailUrl = 'MedicalRcord/MedicalRcord/FeesforListingDetail'
        this.table_detail_columns = this.table_charge_detail_columns
      } else if (this.tabCheckedInfo.id === 3) {
        params = { prescriptionId: row.id }
        detailUrl = 'MedicalRcord/MedicalRcord/PrescriptionDetail'
        this.table_detail_columns = this.table_prescription_detail_columns
      }
      params.centerId = this.medicalCheckedInfo.hospitalId
      this.tableDetailLoading = true
      this.swsApi
        .swsPost(detailUrl, params)
        .then(res => {
          if (res.data.success) {
            this.tableDetailLoading = false
            this.table_detail_data = res.data.result
            this.prescriptionTotalMoney = this.countMoney(res.data.result)
            this.$nextTick(res => {
              // cy 调整table高度
              this.tableHeight = document.documentElement.clientHeight - 365
            })
          }
        })
        .catch(e => {
          this.$Notice.error({
            title: '请求错误,请稍后再试',
            desc: e
          })
          this.tableDetailLoading = false
        })
    },
    // cy 计算处方单明细的总金额
    countMoney (data) {
      let money = 0
      data.forEach(item => {
        // cy 只有当状态为0且退费金额不为0时 才有必要让应收金额减去退费金额
        if (item.discountStatus === 0 && item.refundPrice) {
          money = accAdd(money, subtract(item.receivablePrice, item.refundPrice))
        } else {
          money = accAdd(money, item.receivablePrice)
        }
      })
      return parseFloat(money.toFixed(3))
    },
    changeDate (arr) {
      console.log('dateRange', this.dateRange, 'arr:', arr)
      this.beginTime = arr[0]
      this.endTime = arr[1]
      this.pageIndex = 1
      this.getTableData()
    },
    // cy 选择历史记录
    changeRecord (item) {
      console.log(item)
      let { i, id, recordName } = item
      this.medicalRecordChecked = {
        i,
        id,
        recordName
      }
    },
    // cy 切换文档/病案单选
    changeRecordType (index) {
      let recordId = this.medicalRecordList[0].i
      index = index || recordId
      console.log('index', index)

      let data = this.historyRecordListData.filter(item => item['i'] === index)
      console.log('data', data[0])
      let { i, id, recordName } = data[0]
      this.medicalRecordChecked = {
        i,
        id,
        recordName
      }
      this.historyRecordList = data
      console.log('changeRecordType2---------', this.medicalRecordChecked)
    },
    // cy 切换标签
    changeTab (id) {
      id = id || 2
      this.tabCheckedInfo = {
        id: id,
        name: this.tabData.filter(item => item['id'] === id)[0].name
      }
      console.log('changeTab1---------------------', id)
      this.hide()
      this.pageIndex = 1
      this.getTableData(id)
    },
    changePatient (info) {
      // console.log('子组件触发changePatient:', info)
      this.medicalCheckedInfo = info
      console.log('this.medicalCheckedInfo:', this.medicalCheckedInfo)
      this.hide()
      this.pageIndex = 1
      this.getTableData()
    },
    getTableData (index) {
      index = index || this.tabCheckedInfo.id
      let url = ''
      switch (index) {
        case 1:
          this.medicalRecordList = this.medicalRecordListData2
          this.changeRecordType()
          break
        case 2:
          this.table_columns = this.table_charge_columns
          url = 'MedicalRcord/MedicalRcord/ChargeList'
          break
        case 3:
          this.table_columns = this.table_prescription_columns
          url = 'MedicalRcord/MedicalRcord/Prescription'
          break
      }
      let params = {
        // centerId: 'abc3d60b474c4fef946a66887348c41a',
        // patientId: '94dc006a4cd54e78b152c500f99d81b4',
        centerId: this.medicalCheckedInfo.hospitalId,
        patientId: this.medicalCheckedInfo.patientId,
        beginTime: this.beginTime,
        endTime: this.endTime,
        pageSize: this.pageSize,
        pageIndex: this.pageIndex
      }
      this.loading = true
      this.swsApi.swsPost(url, params)
        .then(res => {
          if (res.data.success) {
            this.table_data = res.data.result
            this.dataCount = res.data.dataCount
            this.loading = false
          }
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
    }
  }
}
</script>

<style scoped lang="less">
#medical {
  position: relative;
  height: 100%;
  /deep/ .content {
    // margin-right: 20px;
    width: 100%;
    // background: #ffffff;
    overflow-y: auto;
  }
  .tab-panel {
    position: sticky;
    top: 0;
    z-index: 999;
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
      margin-top: 20px;
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
    .table2-content {
      margin-top: 20px;
    }
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
</style>
