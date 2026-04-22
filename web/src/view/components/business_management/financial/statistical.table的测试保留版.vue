<template>
  <div class="statistical">
    <financial-template
      :tabData="tabData"
      @on-change="changeTab"
      @on-initial-hospital="getHospital"
    >
      <div class="table-box" slot="content" ref="tables">
        <div class="table">
          <Form ref="formData" :label-width="75" :model="formData">
            <Row>
              <Col :lg="18" :md="18">
                <Row type="flex" justify="start">
                  <Col v-show="tabCheckedId===1">
                    <FormItem label="机构选择">
                      <Select
                        v-model="hospitalCheckedId"
                        multiple
                        :max-tag-count="1"
                        :max-tag-placeholder="maxTagPlaceholder"
                        filterable
                        clearable
                        placeholder="请选择机构"
                        @on-change="changeHospital"
                        style="width: 222px"
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
                    <FormItem label="日期选择">
                      <DatePicker
                        :value="formData.date"
                        @on-change="changeDate"
                        type="daterange"
                        :options="options"
                      ></DatePicker>
                    </FormItem>
                  </Col>
                </Row>
              </Col>
              <Col :lg="6" :md="6" class="button">
                <FormItem>
                  <Row type="flex" justify="end">
                    <Col>
                      <Button
                        v-permission="buttonRole.TJFX_DC"
                        @click="exportTable"
                        type="primary"
                      >导出</Button>
                      <Button
                        v-permission="buttonRole.TJFX_DY"
                        @click="print"
                        type="primary"
                        ghost
                      >打印</Button>
                    </Col>
                  </Row>
                </FormItem>
              </Col>
            </Row>
          </Form>
          <p class="title">{{title}}</p>
          <Table
            v-show="tabCheckedId!==8"
            id="statistical_table"
            class="default-table"
            :data="tableData"
            border
            ref="table"
            :loading="tableLoading"
            :columns="columns"
            :row-class-name="rowClassName"
            :height="tableHeight"
          ></Table>
          <table id="customer_table" v-show="tabCheckedId===8">
            <tr>
              <td style="height: 60px;font-size: 20px;display: none;" v-bind:colspan="columnLength">{{`${this.title} - ${this.monthFirst}-${this.monthLast}`}}</td>
            </tr>
            <thead>
              <tr class="tablehead" v-for="theadDatals in patientOncePayColumns">
                <td
                  v-for="(val) in theadDatals"
                  v-if="(val.split('#'))[0] && (val.split('#'))[0]!='empty'"
                  v-bind:colspan="(val.split('#'))[1]"
                  v-bind:rowspan="(val.split('#'))[2]">
                  {{val?(val.split('#'))[0]:1}}
                </td>
              </tr>
            </thead>
            <tbody>
              <template v-for="(item) in customerTableData">
                <tr>
                  <td :rowspan="item.perCapitaDialysisDetail.length">{{item.centerName}}</td>
                  <td v-for="val in item.perCapitaDialysisDetail[0]" v-html="val"></td>
                </tr>
                <tr v-for="(_item, _index) in item.perCapitaDialysisDetail" v-if="_index !== 0" >
                  <td v-for="val in _item" v-html="val"></td>
                </tr>
              </template>
            </tbody>
          </table>
        </div>
      </div>
    </financial-template>
  </div>
</template>

<script>
import financialTemplate from '@/components/financial-template'
import { getCurrentMonthFirst, getNowDate } from '@/libs/tools.js'
import { mapMutations } from 'vuex'
const BUTTONROLE = {
  TJFX_DY: 'TJFX_DY',
  TJFX_DC: 'TJFX_DC'
}
export default {
  name: 'statistical',
  data () {
    return {
      columnLength: 0, // 自定义表头的长度

      hospitalCheckedId: ['0'],
      hospitalCheckedName: '',
      hospitalList: [],

      selectedBxType: 1, // cy 医保报销类型
      tableHeight: 0,
      buttonRole: BUTTONROLE,
      options: {
        disabledDate (date) {
          return date && date.valueOf() > Date.now()
        }
      },
      tableLoading: false,
      title: '',
      monthFirst: '',
      search: '',
      monthLast: '',
      tabData: [],
      tabCheckedId: 1,
      formData: {
        date: []
      },
      tableData: [],
      columns: [],
      // 患者人数统计
      patienyColumns: [
        { title: '序号', key: 'no', width: 50 },
        { title: '机构名称', key: 'centerName' },
        { title: '全部患者人数', key: 'total' },
        { title: '职工医保人数', key: 'workerHealthCount' },
        { title: '居民医保人数', key: 'residentsHealthCount' },
        { title: '新增患者人数', key: 'newCount' },
        { title: '流失患者人数', key: 'lossCount' },
        { title: '流失率', key: 'turnoverRate' }
      ],
      // 医护人数统计
      docNurColumns: [
        { title: '序号', key: 'no', width: 50 },
        { title: '机构名称', key: 'centerName' },
        { title: '医生人员数', key: 'doctorCount' },
        { title: '护士人员数', key: 'rnurseCount' },
        { title: '其他人员数', key: 'otherCount' },
        { title: '合计', key: 'total' }
      ],
      // 血透机器数量统计
      dialysisMachineColumns: [
        { title: '序号', key: 'no', width: 50 },
        { title: '机构名称', key: 'centerName' },
        { title: 'SWS-4000', key: 'swS4000Count' },
        { title: 'SWS-4000A', key: 'swS4000ACount' },
        { title: 'SWS-6000', key: 'swS6000Count' },
        { title: 'SWS-6000A', key: 'swS6000ACount' },
        {
          title: '4000+6000',
          key: 'total1',
          render: (h, params) => {
            return h('div', params.row.swS4000Count + params.row.swS6000Count)
          }
        },
        {
          title: '4000A+6000A',
          key: 'total2',
          render: (h, params) => {
            return h('div', params.row.swS4000ACount + params.row.swS6000ACount)
          }
        },
        { title: '合计', key: 'total' }
      ],
      // 床位日均使用次数
      bedColumns: [
        { title: '序号', key: 'no', width: 50 },
        { title: '机构名称', key: 'centerName' },
        { title: '床位总数', key: 'bedsCount' },
        { title: '透析总次数', key: 'curePattern' },
        { title: '床位日均使用次数', key: 'dayCout' },
        { title: '床位日均使用率', key: 'usageRate' }
      ],
      // 患者人均月收费及成本统计表
      patientFeeColumns: [
        { title: '序号', key: 'no', width: 50 },
        { title: '机构名称', key: 'centerName' },
        { title: '耗材收入(元)', key: 'materialCharge' },
        { title: '药品收入', key: 'drugCharge' },
        { title: '诊疗收入', key: 'treatmentCharge' },
        { title: '耗材成本', key: 'materialEarnings' },
        { title: '药品成本', key: 'drugEarnings' },
        { title: '诊疗成本', key: 'treatmentEarnings' },
        {
          title: '合计',
          align: 'center',
          children: [
            { title: '收入', key: 'chargeTotal', align: 'center' },
            { title: '成本', key: 'earningsTotal', align: 'center' }
          ]
        }
      ],
      // 医护人员人均月创收表
      docNurInColumns: [
        { title: '序号', key: 'no', width: 50 },
        { title: '机构名称', key: 'centerName' },
        { title: '总创收', key: 'revenueTotal' },
        { title: '医护人员数量', key: 'medicalCount' },
        { title: '人均创收', key: 'perCapita' }
      ],
      // 医护人员人均月创利表
      docNurProColumns: [
        { title: '序号', key: 'no', width: 50 },
        { title: '机构名称', key: 'centerName' },
        { title: '总创收（万元）', key: 'revenueTotal' },
        { title: '总成本（万元）', key: 'earningsTotal' },
        { title: '医护人员数量', key: 'medicalCount' },
        { title: '人均创利', key: 'perProfit' }
      ],
      // 单台机器月均收入表
      MachInColumns: [
        { title: '序号', key: 'no', width: 50 },
        { title: '机构名称', key: 'centerName' },
        { title: '透析机总创收（万元）', key: 'machineCapita' },
        { title: '透析机数量', key: 'machineCount' },
        { title: '单台透析机创收', key: 'machineCapita' },
        { title: '月透析次数', key: 'curePattern' }
      ],
      // 医护人员利用率
      docNorUseRaColumns: [
        { title: '序号', key: 'no', width: 50 },
        { title: '机构名称', key: 'centerName' },
        { title: '医护数量', key: 'machineCount' },
        { title: '透析次数', key: 'curePattern' },
        { title: '理论工作时间', key: 'machineCapita' },
        { title: '透析工作时间', key: 'revenueTotal' },
        { title: '饱和度', key: 'saturation' }
      ],
      patientOncePayAndCost: [
        { title: '机构', key: 'centerName' },
        { title: '项目', key: 'itemName' },
        {
          title: 'HD',
          align: 'center',
          key: 'jc',
          children: [
            { title: '成本', align: 'center', key: 'hdCost', width: 70 },
            { title: '收入', align: 'center', key: 'hdIncome', width: 70 }
          ]
        },
        {
          title: 'HDF',
          align: 'center',
          key: 'jc',
          children: [
            { title: '成本', align: 'center', key: 'hdfCost', width: 70 },
            { title: '收入', align: 'center', key: 'hdfIncome', width: 70 }
          ]
        },
        {
          title: 'HFHD',
          align: 'center',
          key: 'jc',
          children: [
            { title: '成本', align: 'center', key: 'hfhdfCost', width: 70 },
            { title: '收入', align: 'center', key: 'hfhdfIncome', width: 70 }
          ]
        },
        {
          title: 'HD+HP',
          align: 'center',
          key: 'jc',
          children: [
            { title: '成本', align: 'center', key: 'hdhpfCost', width: 70 },
            { title: '收入', align: 'center', key: 'hdhpfIncome', width: 70 }
          ]
        },
        {
          title: 'SUCF',
          align: 'center',
          key: 'jc',
          children: [
            { title: '成本', align: 'center', key: 'sucffCost', width: 70 },
            { title: '收入', align: 'center', key: 'sucffIncome', width: 70 }
          ]
        },
        {
          title: '平均',
          align: 'center',
          key: 'jc',
          children: [
            { title: '成本平均', align: 'center', key: 'averageCost', width: 70 },
            { title: '收入平均', align: 'center', key: 'averageIncome', width: 70 }
          ]
        }
      ],
      customerTableData: [],
      cus_tabel_data1: [
        {
          'centerName': '康美透析中心',
          'itemName': '药品',
          'hdCost': '8',
          'hdIncome': '9',
          'hdfCost': '10',
          'hdfIncome': '11',
          'hfhdfCost': '12',
          'hfhdfIncome': '13',
          'hdhpfCost': '14',
          'hdhpfIncome': '15',
          'sucffCost': '16',
          'sucffIncome': '17'
        },
        {
          'centerName': '秀山透析中心',
          'itemName': '药品',
          'hdCost': '8',
          'hdIncome': '9',
          'hdfCost': '10',
          'hdfIncome': '11',
          'hfhdfCost': '12',
          'hfhdfIncome': '13',
          'hdhpfCost': '14',
          'hdhpfIncome': '15',
          'sucffCost': '16',
          'sucffIncome': '17'
        }
      ],
      cus_tabel_data2: [
        {
          'centerName': '康美透析中心',
          'perCapitaDialysisDetail':
          [
            {'itemName': '药品', 'hdCost': 0, 'hdIncome': 0, 'hdfCost': 0, 'hdfIncome': 0, 'hfhdCost': 0, 'hfhdIncome': 0, 'hdhpCost': 0, 'hdhpIncome': 0, 'sucfCost': 0, 'sucfIncome': 0},
            {'itemName': '耗材', 'hdCost': 0, 'hdIncome': 0, 'hdfCost': 0, 'hdfIncome': 0, 'hfhdCost': 0, 'hfhdIncome': 0, 'hdhpCost': 0, 'hdhpIncome': 0, 'sucfCost': 0, 'sucfIncome': 0},
            {'itemName': '诊疗', 'hdCost': 0, 'hdIncome': 0, 'hdfCost': 0, 'hdfIncome': 0, 'hfhdCost': 0, 'hfhdIncome': 0, 'hdhpCost': 0, 'hdhpIncome': 0, 'sucfCost': 0, 'sucfIncome': 0}
          ]
        },
        {
          'centerName': '111透析中心',
          'perCapitaDialysisDetail':
          [
            {'itemName': '药品11', 'hdCost': 111, 'hdIncome': 0, 'hdfCost': 0, 'hdfIncome': 0, 'hfhdCost': 0, 'hfhdIncome': 0, 'hdhpCost': 0, 'hdhpIncome': 0, 'sucfCost': 0, 'sucfIncome': 0},
            {'itemName': '耗材11', 'hdCost': 111, 'hdIncome': 0, 'hdfCost': 0, 'hdfIncome': 0, 'hfhdCost': 0, 'hfhdIncome': 0, 'hdhpCost': 0, 'hdhpIncome': 0, 'sucfCost': 0, 'sucfIncome': 0},
            {'itemName': '诊疗11', 'hdCost': 111, 'hdIncome': 0, 'hdfCost': 0, 'hdfIncome': 0, 'hfhdCost': 0, 'hfhdIncome': 0, 'hdhpCost': 0, 'hdhpIncome': 0, 'sucfCost': 0, 'sucfIncome': 0}
          ]
        },
        {
          'centerName': '222透析中心',
          'perCapitaDialysisDetail':
          [
            {'itemName': '药品22', 'hdCost': 222, 'hdIncome': 0, 'hdfCost': 0, 'hdfIncome': 0, 'hfhdCost': 0, 'hfhdIncome': 0, 'hdhpCost': 0, 'hdhpIncome': 0, 'sucfCost': 0, 'sucfIncome': 0},
            {'itemName': '耗材22', 'hdCost': 222, 'hdIncome': 0, 'hdfCost': 0, 'hdfIncome': 0, 'hfhdCost': 0, 'hfhdIncome': 0, 'hdhpCost': 0, 'hdhpIncome': 0, 'sucfCost': 0, 'sucfIncome': 0},
            {'itemName': '诊疗22', 'hdCost': 222, 'hdIncome': 0, 'hdfCost': 0, 'hdfIncome': 0, 'hfhdCost': 0, 'hfhdIncome': 0, 'hdhpCost': 0, 'hdhpIncome': 0, 'sucfCost': 0, 'sucfIncome': 0}
          ]
        }
      ],
      // cy 患者人均每例收费及成本
      patientOncePayColumns: [
        {
          'projectName': '项目#2#2',
          'HD': 'HD#2#1',
          'HDF': 'HDF#2#1',
          'HFHD': 'HFHD#2#1',
          'HD+HP': 'HD+HP#2#1',
          'SUCF': 'SUCF#2#1',
          // 'average': '平均#2#1',
          'hdCost': '',
          'hdIncome': '',
          'hdfCost': '',
          'hdfIncome': '',
          'hfhdfCost': '',
          'hfhdfIncome': '',
          'hdhpfCost': '',
          'hdhpfIncome': '',
          'sucffCost': '',
          'sucffIncome': ''
        },
        {
          'projectName': 'empty',
          'HD': 'empty',
          'HDF': 'empty',
          'HFHD': 'empty',
          'HD+HP': 'empty',
          'SUCF': 'empty',
          // 'average': 'empty',
          'hdCost': '成本',
          'hdIncome': '收入',
          'hdfCost': '成本',
          'hdfIncome': '收入',
          'hfhdfCost': '成本',
          'hfhdfIncome': '收入',
          'hdhpfCost': '成本',
          'hdhpfIncome': '收入',
          'sucffCost': '成本',
          'sucffIncome': '收入'
          // 'averageCost': '成本平均',
          // 'averageIncome': '收入平均',
        }
      ],
      exportColumns: [],
      aaa: {
        title: '短文',
        align: 'center',
        width: 120,
        className: 'table-green-index',
        fixed: 'left',
        render: (h, params) => {
          let a = params.row.essay
          let b = []
          a.map((val, index) => {
            b.push(
              h(
                'div',
                {
                  on: {
                    click: () => {
                      let status = null
                      if (a[index].status === 'Completed') {
                        status = true
                      } else {
                        status = false
                      }
                    }
                  }
                },
                val.title
              )
            )
            if (a.length !== index + 1) {
              b.push(h('hr', {}))
            }
          })
          return b
        }
      },
      printDatas: {
        api: '',
        title: '',
        columns: [],
        params: {}
      }
    }
  },
  created () {
    this.loadTabData().then(_ => {
      let d = getNowDate().substring(0, 10)
      this.formData.date.push(getCurrentMonthFirst(), d)
      this.monthFirst = getCurrentMonthFirst()
      this.monthLast = d
      this.changeTab(this.tabCheckedId)
    })
  },
  mounted () {
    this.$nextTick(_ => {
      this.tableHeight = this.$refs.tables.clientHeight - 115
    })
  },
  computed: {
    // 接口，表格汇总
    UISettings () {
      return {
        '1': {
          columns: [],
          actionCode: 'BusinessTargetClntroller/BusinessTarget/GetSIRatioAsync'
        },
        '2': {
          columns: this.patienyColumns,
          actionCode: 'BusinessTargetClntroller/BusinessTarget/PatientsReport'
        },
        '3': {
          columns: this.docNurColumns,
          actionCode: 'BusinessTargetClntroller/BusinessTarget/Employee'
        },
        '4': {
          columns: this.dialysisMachineColumns,
          actionCode: 'BusinessTargetClntroller/BusinessTarget/Equipment'
        },
        '5': {
          columns: [],
          actionCode: 'BusinessTargetClntroller/BusinessTarget/CurePatternCheck'
        },
        '6': {
          columns: this.bedColumns,
          actionCode: 'BusinessTargetClntroller/BusinessTarget/BedsUseReport'
        },
        '7': {
          columns: this.patientFeeColumns,
          actionCode: 'BusinessTargetClntroller/BusinessTarget/PatientSaverage'
        },
        '8': {
          columns: [],
          actionCode:
            'BusinessTargetClntroller/BusinessTarget/PerCapitaDialysis'
        },
        '9': {
          columns: this.docNorUseRaColumns,
          actionCode: 'BusinessTargetClntroller/BusinessTarget/SaturatedMedical'
        },
        '10': {
          columns: [],
          actionCode: ''
        }
      }
    },
    exportAndPrintTitle () {
      if (this.tabCheckedId !== 7) {
      // if ([1, 2, 3, 4, 5, 6].includes(this.tabCheckedId)) {
        return `${this.title}（${this.monthFirst}~${this.monthLast}）`
      } else {
        return this.title
      }
    }
  },
  methods: {
    ...mapMutations(['setColumns', 'setTitle', 'setParams', 'setApi']),
    // 获取机构
    getHospital (hospitalList) {
      this.hospitalList = hospitalList
      this.hospitalCheckedId = hospitalList[0].dialysisId
    },
    // 切换医院
    changeHospital (id) {
      // console.log(id)
      // cy 多选只有一个时则需要获取该机构名称
      if (id.length === 1) {
        this.hospitalCheckedName = this.hospitalList.filter(
          v => v.dialysisId === id[0]
        )[0].dialysisName
      }
      // cy 如果多选了全选+其他机构，则去掉全部
      if (id.length > 1 && id.includes('0')) {
        id.splice(id.indexOf('0'), 1)
      }
      this.hospitalCheckedId = id
      this.columns = []
      this.tableData = []
      this.getTableDataByKey(this.tabCheckedId)
    },
    maxTagPlaceholder (num) {
      return `+${num}机构`
    },
    // 医保报销类型选择
    changeBxType () {},
    // 加载表格名字
    loadTabData () {
      return this.swsApi
        .swsPost(`BusinessTargetClntroller/BusinessTarget/1/2`)
        .then(res => {
          if (res.data.success) {
            this.tabData = res.data.result
          } else {
            this.tabData = []
          }
        })
        .catch(e => {
          console.log(e)
        })
    },
    // 选择第一个表格日期
    changeDate (e) {
      if (e[0]) {
        this.monthFirst = e[0]
        this.monthLast = e[1]
        this.changeTab(this.tabCheckedId)
        this.formData.date = e
      }
    },
    // 点击表格类别
    changeTab (index) {
      this.columns = []
      this.tableData = []
      this.tabCheckedId = index
      this.title = this.tabData[index - 1].childValue
      this.getTableDataByKey(index)
    },
    getTableDataByKey (index) {
      let jsonStr = {}
      let baseJsonStr = {
        beginReportDate: this.monthFirst,
        endReportDate: this.monthLast
      }
      // cy 医保报销表单增加 类型筛选
      if (index === 1) {
        jsonStr = {
          selectedBxType: this.selectedBxType,
          centerId: this.hospitalCheckedId,
          beginTime: this.monthFirst,
          endTime: this.monthLast,
          model: 0
        }
      } else {
        jsonStr = baseJsonStr
      }
      let { actionCode, columns } = this.UISettings[index]
      this.columns = columns
      this.loadTable(actionCode, jsonStr, index)
    },
    // 加载表格
    loadTable (actionCode, jsonStr, index) {
      this.tableLoading = true
      this.printDatas.api = actionCode
      this.printDatas.params = JSON.parse(JSON.stringify(jsonStr))
      this.swsApi
        .swsPost(`${actionCode}`, jsonStr)
        .then(res => {
          if (res.data.success) {
            if ([1, 5].includes(index)) {
              let data = res.data.result.tableHeaders
              // cy 处理表头
              for (let item of data) {
                item['minWidth'] = 70
                if (item.key === null) {
                  delete item.key
                }
                if (item.children === null) {
                  delete item.children
                } else {
                  for (let fitem of item.children) {
                    fitem['minWidth'] = 65
                    if (fitem.children === null) {
                      delete fitem.children
                    }
                    if (item.key === null) {
                      delete item.key
                    }
                  }
                }
              }
              this.columns = JSON.parse(JSON.stringify(data))
              if (index === 1) {
                this.tableData = res.data.result.siRatioOutPuts
              } else if (index === 5) {
                this.tableData = res.data.result.curePatternDatas
              }
            } else if (index === 8) {
              this.customerTableData = res.data.result
              if (this.customerTableData.length !== 0 && this.customerTableData[0].perCapitaDialysisDetail.length !== 0) {
                this.columnLength = Object.keys(this.customerTableData[0].perCapitaDialysisDetail[0]).length + 1
                // console.log(this.columnLength)
              }
            } else {
              // console.log(this.tableData)
              this.tableData = res.data.result
            }
            this.tableLoading = false
          }
        })
        .catch(e => {
          this.tableLoading = false
          console.log(e)
        })
    },
    // 表格首行加粗
    rowClassName (row, index) {
      if (row.centerName === '合计') {
        return 'total'
      }
    },
    // 打印
    print () {
      let url = window.location.href.split('#')[0]
      // 缓存打印数据
      this.printDatas = Object.assign(this.printDatas, {
        columns: JSON.parse(JSON.stringify(this.columns)),
        title: this.exportAndPrintTitle
      })

      const { api, columns, title, params } = this.printDatas
      this.setTitle(title)
      this.setApi(api)
      this.setColumns(columns)
      this.setParams(params)

      window.open(`${url}#/print/statistic_analysis`)
    },
    // cy 处理多表头导出问题
    handleMultiTableHeader (columns, noSubTitle) {
      noSubTitle = noSubTitle || false
      return columns
        .map(res => {
          if (res.hasOwnProperty('children')) {
            return res.children.map(item => {
              let colData = Object.assign({}, item)
              colData.title = `${
                noSubTitle
                  ? ''
                  : `${res.title.substring(res.title.indexOf('_') + 1)}_`
              }${colData.title}`
              return colData
            })
          } else {
            return res
          }
        })
        .flat()
    },
    // 导出
    exportTable () {
      let tableId = this.tabCheckedId === 8 ? 'customer_table' : 'statistical_table'
      let name = this.exportAndPrintTitle
      tableExport(tableId, name, 'xlsx')
      // let tableTitle = this.handleMultiTableHeader(this.columns)
      // this.$refs.table.exportCsv({
      //   filename: this.exportAndPrintTitle,
      //   columns: tableTitle,
      //   data: this.tableData.filter((data, index) => {
      //     return index < this.tableData.length
      //   })
      // })
    }
  },
  components: {
    financialTemplate
  }
}
</script>

<style lang="less" scope="scoped">
.statistical {
  height: 100%;
  .table-box {
    background: #fff;
    height: 100%;
    .title {
      text-align: center;
      font-size: 18px;
      color: #515a6e;
      line-height: 37px;
    }
  }
  form {
    padding-top: 14px;
  }
  .ivu-form .ivu-form-item-label {
    color: #a3a3a3;
    font-size: 14px;
  }
  .ivu-form .ivu-form-item {
    margin-bottom: 10px;
  }
  .table {
    padding: 0 10px;
    padding-bottom: 20px;
    background: #ffffff;
  }
  .time {
    font-size: 13px;
    color: #333333;
    margin-bottom: 10px;
  }
  .ivu-table .total td {
    font-weight: bold;
  }
  .button {
    text-align: right !important;
    button {
      margin-left: 10px;
    }
  }
  #customer_table {
    width: 100%;
    border-collapse: collapse;
    margin-bottom: 10px;
    td {
      text-align: center;
      border: 1px solid #aaaaaa;
      padding: 10px 20px;
    }
  }
}
</style>
