<template>
  <div class="profit">
    <financial-template
      :tabData="tabData"
      @on-change="changeTab"
      @on-initial-hospital="getHospital"
      ref="template"
    >
      <div slot="content" class="table-box">
        <div class="table">
          <Form ref="formData" :label-width="75">
            <Row>
              <Col :lg="12" :md="12">
                <Row type="flex" justify="start">
                  <Col v-show="!hasLessParams">
                    <FormItem label="机构选择">
                      <Select
                        v-model="hospitalCheckedIds"
                        filterable
                        multiple
                        placeholder="请选择机构"
                        @on-change="changeHospitals"
                        :max-tag-count="1"
                        :max-tag-placeholder="maxTagPlaceholder"
                        style="width: 222px;"
                      >
                        <Option
                          v-for="item in hospitalList"
                          :key="item.dialysisId"
                          :value="item.dialysisId"
                        >{{item.dialysisName}}</Option>
                      </Select>
                    </FormItem>
                  </Col>
                  <Col>
                    <FormItem label="日期范围">
                      <DatePicker
                        :value="date"
                        @on-change="changeDate"
                        type="daterange"
                        :options="options"
                      ></DatePicker>
                    </FormItem>
                  </Col>
                </Row>
              </Col>
              <Col :lg="12" :md="12">
                <FormItem>
                  <Row type="flex" justify="end">
                    <Col>
                      <Button
                        @click="getTableData()"
                        v-permission="buttonRole.YLFX_DC"
                        type="primary"
                        style="margin-right: 10px;"
                      >查询</Button>
                      <Button
                        @click="exportTable()"
                        type="info"
                        style="margin-right: 10px;"
                      >导出</Button>
                      <Button @click="print" v-permission="buttonRole.YLFX_DY" type="primary" ghost>打印</Button>
                    </Col>
                  </Row>
                </FormItem>
              </Col>
            </Row>
          </Form>
          <div class="title">{{title}}</div>
          <!-- <div class="time">时间：{{this.params.beginTime}} 至 {{this.params.endTime}}</div> -->
          <Table
            ref="table"
            id="profit_table"
            class="default-table"
            border
            @on-drag-drop="dragTh"
            :draggable="true"
            :height="tableHeight"
            :data="tableData"
            :loading="table1Loading"
            :columns="columns"
            :row-class-name="rowClassName"
          ></Table>
        </div>
      </div>
    </financial-template>
  </div>
</template>

<script>
import financialTemplate from '@/components/financial-template'
import { getCurrentMonthFirst, getNowFormatDate, _debounce } from '@/libs/tools.js'
import { mapMutations } from 'vuex'
const BUTTONROLE = {
  YLFX_DY: 'YLFX_DY',
  YLFX_DC: 'YLFX_DC'
}
export default {
  name: 'profit',
  data () {
    return {
      tableHeight: 0,
      table1Loading: false,
      tableData: [],
      columns: [],

      options: {
        disabledDate (date) {
          return date && date.valueOf() > Date.now()
        }
      },
      content: '12312323',
      title: '',
      date: [],
      tabData: [],
      tabCheckedId: 1,

      monthFirst: '',
      monthLast: '',

      hospitalCheckedIds: ['0'],
      hospitalCheckedName: '',
      hospitalList: [],

      params: {
        // monthOrYear: 1,
        costChangeType: -1,
        centerId: ['0'],
        beginTime: '-',
        endTime: '-'
      },
      // 收入增减变动表
      constChangeColumn1: [
        { title: '序号', key: 'no', width: '55', align: 'center' },
        { title: '机构名称', key: 'centerName', minWidth: 100 },
        { title: '本期收入', key: 'nowIncome', minWidth: 100 },
        {
          title: '上期比',
          key: 'preIncomeRate',
          align: 'center',
          children: [
            { title: '上期收入', key: 'upIncome', align: 'center', minWidth: 100 },
            { title: '变动额', key: 'differIncome', align: 'center', minWidth: 100 },
            { title: '变动率', key: 'differRate', align: 'center', minWidth: 100 }
          ]
        },
        {
          title: '上年同期比',
          key: 'lastYearRate',
          align: 'center',
          children: [
            { title: '上年同期收入', key: 'upYearNowIncome', align: 'center', minWidth: 100 },
            { title: '变动额', key: 'upYearDifferIncome', align: 'center', minWidth: 100 },
            { title: '变动率', key: 'upYearDifferRate', align: 'center', minWidth: 100 }
          ]
        }
      ],
      // 成本增减变动表
      constChangeColumn2: [
        { title: '序号', key: 'no', width: '55', align: 'center' },
        { title: '机构名称', key: 'centerName', minWidth: 100 },
        { title: '本期成本', key: 'nowIncome', minWidth: 100 },
        {
          title: '上期比',
          key: 'preIncomeRate',
          align: 'center',
          children: [
            { title: '上期成本', key: 'upIncome', align: 'center', minWidth: 100 },
            { title: '变动额', key: 'differIncome', align: 'center', minWidth: 100 },
            { title: '变动率', key: 'differRate', align: 'center', minWidth: 100 }
          ]
        },
        {
          title: '上年同期比',
          key: 'lastYearRate',
          align: 'center',
          children: [
            { title: '上年同期成本', key: 'upYearNowIncome', align: 'center', minWidth: 100 },
            { title: '变动额', key: 'upYearDifferIncome', align: 'center', minWidth: 100 },
            { title: '变动率', key: 'upYearDifferRate', align: 'center', minWidth: 100 }
          ]
        }
      ],
      // 毛利增减变动表
      constChangeColumn3: [
        { title: '序号', key: 'no', width: '55', align: 'center' },
        { title: '机构名称', key: 'centerName', width: '110' },
        {
          title: '本期',
          key: 'preIncomeRate',
          align: 'center',
          children: [
            { title: '收入', key: 'nowIncome', align: 'center', width: '75' },
            { title: '成本', key: 'nowCost', align: 'center', width: '75' },
            { title: '毛利', key: 'nowProfit', align: 'center', width: '75' },
            { title: '毛利率', key: 'profitRate', align: 'center', width: '75' }
          ]
        },
        {
          title: '上期',
          key: 'preIncomeRate',
          align: 'center',
          children: [
            { title: '收入', key: 'upIncome', align: 'center', width: '75' },
            { title: '成本', key: 'upCost', align: 'center', width: '75' },
            { title: '毛利', key: 'upProfit', align: 'center', width: '75' },
            { title: '毛利率', key: 'upProfitRate', align: 'center', width: '75' }
          ]
        },
        {
          title: '上期比',
          key: 'preIncomeRate',
          align: 'center',
          children: [
            { title: '收入变动额', key: 'differIncome', align: 'center', width: '80' },
            { title: '成本变动额', key: 'differCost', align: 'center', width: '80' },
            { title: '毛利变动额', key: 'differProfit', align: 'center', width: '80' },
            { title: '毛利率变动', key: 'profitRateRate', align: 'center', width: '80' }
          ]
        },
        {
          title: '上年同期',
          key: 'lastYearRate',
          align: 'center',
          children: [
            { title: '收入', key: 'upYearNowIncome', align: 'center', width: '75' },
            { title: '成本', key: 'upYearNowCost', align: 'center', width: '75' },
            { title: '毛利', key: 'upYearNowProfit', align: 'center', width: '75' },
            { title: '毛利率', key: 'upYearProfitRate', align: 'center', width: '75' }
          ]
        },
        {
          title: '上年同期比',
          key: 'lastYearRate',
          align: 'center',
          children: [
            { title: '收入变动额', key: 'upYearDifferIncome', align: 'center', width: '80' },
            { title: '成本变动额', key: 'upYearDifferCost', align: 'center', width: '80' },
            { title: '毛利变动额', key: 'upYearDifferProfit', align: 'center', width: '80' },
            { title: '毛利率变动', key: 'upYearDifferProfitRate', align: 'center', width: '80' }
          ]
        }
      ],

      // 医护人员人均月创收表
      docNurInColumns: [
        { title: '机构名称', key: 'centerName' },
        { title: '总创收', key: 'revenueTotal' },
        { title: '医护人员数量', key: 'medicalCount' },
        { title: '人均创收', key: 'perCapita' }
      ],
      // 医护人员人均月创利表
      docNurProColumns: [
        { title: '机构名称', key: 'centerName' },
        { title: '总创收', key: 'revenueTotal' },
        { title: '总成本', key: 'earningsTotal' },
        { title: '医护人员数量', key: 'medicalCount' },
        { title: '人均创利', key: 'perProfit' }
      ],
      // 单台机器月均收入表
      MachInColumns: [
        { title: '机构名称', key: 'centerName' },
        { title: '透析机总创收', key: 'revenueTotal' },
        { title: '透析机数量', key: 'machineCount' },
        { title: '单台透析机创收', key: 'machineCapita' },
        { title: '月透析次数', key: 'curePattern' }
      ],
      buttonRole: BUTTONROLE,

      printDatas: {
        api: '',
        title: '',
        columns: [],
        params: {}
      }
    }
  },
  computed: {
    UISettings () {
      let params = {
        beginReportDate: this.params.beginTime,
        endReportDate: this.params.endTime
      }
      let target = {
        '1': {
          columns: this.constChangeColumn1,
          api: ''
        },
        '2': {
          columns: this.constChangeColumn2,
          api: ''
        },
        '3': {
          columns: this.constChangeColumn3,
          api: ''
        },
        '4': {
          columns: this.docNurInColumns,
          api: 'BusinessTargetClntroller/BusinessTarget/MedicalRevenue'
        },
        '5': {
          columns: this.docNurProColumns,
          api: 'BusinessTargetClntroller/BusinessTarget/MedicalRevenue'
        },
        '6': {
          columns: this.MachInColumns,
          api: 'BusinessTargetClntroller/BusinessTarget/DialysisMachineRevenue'
        },
        '7': {
          columns: [],
          api: ''
        }
      }
      for (let key in target) {
        target[key].params = params
      }
      return target
    },
    hasLessParams () {
      return [4, 5, 6, 7].includes(this.tabCheckedId)
    },
    exportOrPrintTitle () {
      return `${this.title}（${this.params.beginTime}~${this.params.endTime}）`
    }
  },
  mounted () {
    this.tableHeight = this.$refs.template.$el.clientHeight - 140
    this.loadTabData()
      .then(_ => {
        let dateRange = [getCurrentMonthFirst(), getNowFormatDate(false)]
        this.date = dateRange
        this.params.beginTime = dateRange[0]
        this.params.endTime = dateRange[1]
        this.changeTab(this.tabCheckedId)
      })
      .catch(e => {})
  },
  methods: {
    ...mapMutations(['setColumns', 'setTitle', 'setParams', 'setApi']),
    dragTh(aa,bb) {
      console.log('lalalalal',aa,bb)
    },
    maxTagPlaceholder (num) {
      return `+${num}机构`
    },
    changeHospitals (ids) {
      this.params.centerId = ids
      if (ids.length === 0) return false
      if (ids.length === 1 && ids[0] !== '0') {
        this.hospitalCheckedName = this.hospitalList.filter(
          v => v.dialysisId === ids[0]
        )[0].dialysisName
      } else {
        this.hospitalCheckedName = '透析机构'
      }
      // cy 如果多选了全选+其他机构，则去掉全部
      if (ids.length > 1 && ids[0] === '0') {
        this.hospitalCheckedIds.splice(ids.indexOf('0'), 1)
      }
      // 如果多选，再选中了全选，则清空ids，并显示全选
      if (ids.length > 1 && ids[ids.length - 1] === '0') {
        this.hospitalCheckedIds = ['0']
      }
      // cy 防抖
      // _debounce(this.changeTab, this.tabCheckedId, 500)
    },
    // 加载表格名字
    loadTabData () {
      return this.swsApi
        .swsPost(`BusinessTargetClntroller/BusinessTarget/1/3`)
        .then(res => {
          if (res.data.code === 200) {
            this.tabData = res.data.result
          }
        })
        .catch(e => {})
    },
    // 表格合计行加粗
    rowClassName (row, index) {
      if (row.centerName === '合计') {
        return 'total'
      }
    },
    changeTab (id) {
      this.tabCheckedId = id
      this.params.costChangeType = id
      this.title = `${this.hospitalCheckedName}${this.tabData[id - 1].childValue}`
      // this.getTableData()
      this.tableData = []
      this.columns = []
    },
    changeDate (e) {
      if (!e[0]) return
      this.params.beginTime = e[0]
      this.params.endTime = e[1]
      // this.getTableData()
    },
    getTableData () {
      this.table1Loading = true
      let { api, params, columns } = this.UISettings[this.tabCheckedId]
      this.columns = columns
      if (this.hasLessParams) {
        this.loadSpecialTable(api, params)
      } else {
        this.loadDefaultTable()
      }
    },
    loadDefaultTable () {
      let api = 'BusinessTargetClntroller/BusinessTarget/CostChange'
      // 缓存打印数据
      this.printDatas = Object.assign(this.printDatas, { api, params: this.params })
      this.swsApi
        .swsPost(api, this.params)
        .then(res => {
          let data = res.data
          this.table1Loading = false
          if (data.success) {
            this.tableData = data.result
          }
        })
        .catch(e => {
          this.table1Loading = false
        })
    },
    loadSpecialTable (api, params) {
      // 缓存打印数据
      this.printDatas = Object.assign(this.printDatas, { api, params })
      api &&
        this.swsApi
          .swsPost(api, params)
          .then(res => {
            let data = res.data
            this.table1Loading = false
            if (data.success) {
              // this.columns = data.result.tableHeaders.map(item => {
              //   delete item.children
              //   return item
              // })
              this.tableData = data.result
            }
          })
          .catch(e => {
            this.table1Loading = false
          })
    },
    changeTimeType () {
      this.getTableData()
    },
    exportTable () {
      // cy 多表头有不同的导出方式
      if ([1, 2, 3].includes(this.tabCheckedId)) {
        let tableId = 'profit_table'
        let name = this.exportOrPrintTitle
        tableExport(tableId, name, 'xlsx')
      } else {
        this.$refs.table.exportCsv({
          filename: this.exportOrPrintTitle,
          columns: this.columns,
          data: this.tableData
        })
      }
    },
    // 获取机构
    getHospital (hospitalList) {
      this.hospitalList = hospitalList
      this.hospitalCheckedIds = [hospitalList[0].dialysisId]
    },
    print () {
      // 缓存打印数据
      this.printDatas = Object.assign(this.printDatas, {
        columns: this.columns,
        title: this.exportOrPrintTitle
      })
      const { api, columns, title, params } = this.printDatas
      this.setTitle(title)
      this.setApi(api)
      this.setColumns(columns)
      this.setParams(params)
      let url = window.location.href.split('#')[0]
      window.open(`${url}#/print/profit_analysis`)
    }
  },
  components: {
    financialTemplate
  }
}
</script>

<style scoped lang="less" scope="scoped">
.profit {
  height: 100%;
  .table-box {
    .table {
      padding: 0 10px;
      background: #ffffff;
    }
    .time {
      margin-bottom: 10px;
    }
    form {
      margin-top: 14px;
    }
  }
  /deep/ .ivu-form .ivu-form-item {
    margin-bottom: 10px;
    .ivu-form-item-label {
      color: #a3a3a3;
      font-size: 14px;
    }
  }
  // .ivu-form .ivu-form-item-label {
  //   color: #a3a3a3;
  //   font-size: 14px;
  // }
  .title {
    text-align: center;
    font-size: 18px;
    line-height: 1.8;
    color: #515a6e;
  }
  /deep/ .ivu-table .total td {
    font-weight: bold;
  }
}
</style>
