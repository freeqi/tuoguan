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
          <div class="table-content">
            <div class="pdf-box">

              <Select v-model="medicalType" style="width:160px; margin-right: 10px">
                <Option v-for="item in medicalTypeList" :value="item.value" :key="item.value">{{ item.label }}</Option>
              </Select>

              <Button icon="md-download" type="primary" @click="downLoadPDF()">下载文档</Button>
              <div class="right-group">
                <a
                  ref="pdfADom"
                  :href="ducumentApi"
                  target="blank"
                >全屏预览</a>
              </div>
              <Divider style="margin:10px 0 15px;"/>
              <div class="pdf-view">
                <object
                  :data="ducumentApi"
                  type="application/pdf"
                  width="100%"
                  height="600"
                >
                  <p>
                    抱歉，您的浏览器不支持PDF预览，请点击链接下载PDF文件
                    <a
                      :href="ducumentApi"
                    ></a>
                  </p>
                </object>
              </div>
            </div>
          </div>
        </div>
        <div class="table2" v-else>
          <h2>{{medicalCheckedInfo.hospitalName}}{{tabCheckedInfo.name}}{{`-${medicalCheckedInfo.patientName}`}}</h2>
          <div>
            <div class="btn-groups">
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
      medicalRecordChecked: {
        i: 0,
        id: '',
        recordName: ''
      },
      tabCheckedInfo: {
        id: 1,
        name: ''
      },
      tabData: [
        {
          id: 1,
          name: '医疗文书'
        },
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
      ]

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
  mounted () {
    this.$nextTick(() => {
      this.changeTab()
    })
  },
  methods: {
    // cy 下载文档
    downLoadPDF () {
      let src = this.$refs['pdfADom'].getAttribute('href')
      // 下载文件
      this.swsApi.swsDownload({
        methods: 'get',
        url: `${src}?date=${new Date().valueOf()}`,
        responseType: 'blob'
      }).then(res => {
        let blob = new Blob([res.data], {type: 'application/pdf'})
        let elink = document.createElement('a')
        elink.download = '病历.pdf'
        elink.style.display = 'none'
        let href = URL.createObjectURL(blob)
        elink.href = href
        document.body.appendChild(elink)
        elink.click()
        URL.revokeObjectURL(href)
        document.body.removeChild(elink)
      })
    },
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
          money = accAdd(
            money,
            subtract(item.receivablePrice, item.refundPrice)
          )
        } else {
          money = accAdd(money, item.receivablePrice)
        }
      })
      return parseFloat(money.toFixed(3))
    },
    changeDate (arr) {
      this.beginTime = arr[0]
      this.endTime = arr[1]
      this.pageIndex = 1
      this.getTableData()
    },
    // cy 切换标签
    changeTab (id) {
      id = id || 2
      this.tabCheckedInfo = {
        id: id,
        name: this.tabData.filter(item => item['id'] === id)[0].name
      }
      this.hide()
      this.pageIndex = 1
      this.getTableData(id)
    },
    changePatient (info) {
      this.medicalCheckedInfo = info
      this.hide()
      this.pageIndex = 1
      this.getTableData()
    },
    getTableData (index) {
      index = index || this.tabCheckedInfo.id
      let url = ''
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
      switch (index) {
        case 1:
          this.medicalRecordChecked.id = this.medicalCheckedInfo.patientId
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
      this.loading = true
      index !== 1 &&
        this.swsApi
          .swsPost(url, params)
          .then(res => {
            if (res.data.success) {
              this.table_data = res.data.result
              this.dataCount = res.data.dataCount
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
