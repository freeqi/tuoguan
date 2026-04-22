<template>
  <div id="incoming_outgoings">
    <financial-template
      :tabData="tabData"
      @on-change="changeTab"
      @on-initial-hospital="getHospital"
    >
      <div class="tab-content" slot="content">
        <div class="table">
          <div style="margin: 10px;">
            <span>机构选择</span>
            <Select
              v-show="!hasPage"
              v-model="hospitalCheckedId"
              multiple
              :max-tag-count="1"
              :max-tag-placeholder="maxTagPlaceholder"
              filterable
              clearable
              placeholder="请选择机构"
              @on-change="changeHospital"
              style="width: 222px;margin-left:10px;"
            >
              <Option
                v-for="item in hospitalList"
                :key="item.dialysisId"
                :value="item.dialysisId"
              >{{item.dialysisName}}</Option>
            </Select>
            <Select
              v-show="hasPage"
              v-model="hospitalCheckedId"
              filterable
              clearable
              placeholder="请选择机构"
              @on-change="changeHospital"
              style="width: 190px;margin-left:10px;"
            >
              <Option
                v-for="item in hospitalList"
                :key="item.dialysisId"
                :value="item.dialysisId"
              >{{item.dialysisName}}</Option>
            </Select>
            <span style="padding: 0 19px;font-size: 18px;" v-show="tabCheckedId == 1">
              <RadioGroup v-model="typeCheckedId" @on-change="changeType">
                <Radio v-for="item of type" :key="item.index" :label="item.index">{{item.title}}</Radio>
              </RadioGroup>
            </span>
          </div>
          <div class="table-operate">
            <div class="button-group">
              <span>
                <span class="span_title">日期选择</span>
                <DatePicker
                  :value="formData.date"
                  @on-change="changeDate"
                  type="daterange"
                  :options="options"
                  placeholder="请选择时间段"
                ></DatePicker>
              </span>
              <span v-show="hasPage">
                <Input
                  v-model="searchKey"
                  search
                  enter-button
                  @on-search="keySearch"
                  placeholder="请输入姓名或单号查询"
                />
              </span>
            </div>
            <div class="operate-right">
              <Button @click="exportTable()" type="primary" v-permission="buttonRole.SZBB_DC">导出</Button>
              <Button
                @click="print('table')"
                type="primary"
                ghost
                v-permission="buttonRole.SZBB_DC"
              >打印</Button>
            </div>
          </div>
          <div class="table_title">
            <p class="p" v-if="!hasPage">
              {{isMulti ? '透析中心' : hospitalCheckedName}}{{title}}
              <span
                v-show="tabCheckedId === 1"
              >（{{this.type[this.typeCheckedId-1].title}}）</span>
            </p>
            <p class="p" v-else>
              {{hospitalCheckedId === '0' ? '透析中心' : hospitalCheckedName}}{{title}}
              <span
                v-show="tabCheckedId ==2"
              >（按收费项目）</span>
            </p>
          </div>
        </div>
        <div class="table">
          <Table
            id="table_box"
            ref="table"
            class="default-table"
            @on-row-dblclick="dbClick"
            :height="tableHeight"
            :highlight-row="[4,5].includes(tabCheckedId)"
            :data="tableData"
            :row-class-name="rowClassName"
            :loading="tableLoading"
            :columns="columns"
          ></Table>
          <div class="pagination">
            <Page
              class="page"
              v-show="hasPage"
              :total="dataCount"
              show-total
              :current="current"
              :page-size="pageSize"
              @on-change="changePage"
            ></Page>
          </div>
          <p class="p" v-show="detailTableFlag">{{detailTableCheckedInfo.name}}{{title}}明细表</p>
          <Table
            v-show="detailTableFlag"
            class="default-table"
            ref="detailTable"
            height="300"
            :data="detailTableData"
            :row-class-name="rowClassName"
            :loading="detailTableLoading"
            :columns="detailTableColumns"
          ></Table>
          <div v-show="isTab3">
            <div class="time">
              <!-- <span>操作员：{{data.name}}</span> -->
              <span>
                时间：
                <label v-show="isTab3">{{this.monthFirst}} 至 {{this.monthLast}}</label>
                <!-- <label v-show="isTab3">{{this.formData.singleDate}}</label> -->
              </span>
            </div>
            <table class="total-table">
              <tr v-for="(item,index) in tableTotal" :key="index">
                <td>{{item.title}}</td>
                <td>{{item.desc}}</td>
              </tr>
            </table>
          </div>
          <!-- <div class="button">
              <Button @click="exportTable()" type="primary" v-permission="buttonRole.SZBB_DC">导出</Button>
              <Button @click="print" type="primary" ghost v-permission="buttonRole.SZBB_DC">打印</Button>
          </div>-->
        </div>
      </div>
    </financial-template>
  </div>
</template>

<script>
import financialTemplate from '@/components/financial-template'
import { mapMutations } from 'vuex'
import {
  getCurrentMonthFirst,
  getNowDate,
  getNowFormatDate,
  convertCurrency
} from '@/libs/tools.js'
const BUTTONROLE = {
  SZBB_DC: 'SZBB_DC',
  SZBB_DY: 'SZBB_DY'
}
export default {
  name: 'incoming_outgoings',
  components: {
    financialTemplate
  },
  data () {
    return {
      searchKey: '', // cy 查询关键字

      detailTableColumns: [], // cy 双击指定table的行显示的明细表单
      detailTableData: [],
      detailTableFlag: false, // 是否显示明细表单
      detailTableCheckedInfo: {
        name: ''
      },
      detailTableLoading: false,
      headerFlag: false, // cy 收入总汇表和收入日报表的选择全部机构时的标志
      tableHeight: 0, // table高度
      pageSize: 11,
      current: 1,
      options: {
        disabledDate (date) {
          return date && date.valueOf() > Date.now()
        }
      },
      type: [
        { title: '按收费项目', index: 1 },
        { title: '按结算方式', index: 2 },
        { title: '按回款方式', index: 3 }
      ],
      hospitalCheckedId: '0',
      hospitalCheckedName: '',
      hospitalList: [],
      data: {
        outPrice: 5000,
        price: 4000,
        name: '张三'
      },
      tabData: [],
      tabCheckedId: 1,
      typeCheckedId: 1,
      formData: {
        type: '1',
        date: [],
        singleDate: getNowFormatDate(false)
      },
      title: '收入总汇表',
      monthFirst: '',
      monthLast: '',
      columns: [],
      allColumns: [],
      tableData: [],
      feeColumns: [
        { title: '序号', key: 'no', width: 65, align: 'center' },
        { title: '项目', key: 'itemName' },
        { title: '门诊内科', key: 'totalMoney' },
        { title: '合计', key: 'totalMoney' }
      ],
      allFeeColumns: [
        { title: '透析中心', key: 'itemName' },
        { title: '药品费', key: 'yp' },
        { title: '检查费', key: 'jc' },
        { title: '治疗费', key: 'zl' },
        { title: '透析费', key: 'tx' },
        { title: '护理费', key: 'hl' },
        { title: '氧气费', key: 'yq' },
        { title: '材料费', key: 'cl' },
        { title: '其他费', key: 'qt' },
        { title: '合计', key: 'totalMoney' }
      ],
      allCostColumns: [
        { title: '透析中心', key: 'itemName' },
        { title: '药品费', key: 'yp' },
        { title: '耗材费', key: 'cl' },
        { title: '透析费', key: 'tx' },
        { title: '其他费', key: 'qt' },
        { title: '合计', key: 'totalMoney' }
      ],
      tableTotal: [],
      listColumns: [
        { title: '序号', key: 'no', align: 'center', minWidth: 65 },
        { title: '患者姓名', key: 'pName', minWidth: 65 },
        { title: '性别', key: 'sex', align: 'center', minWidth: 40 },
        { title: '年龄', key: 'age', align: 'center', minWidth: 40 },
        { title: '医保类别', key: 'healthCareType', minWidth: 70 },
        { title: '类别', key: 'mzType', align: 'center', minWidth: 70 },
        { title: '结算单号', key: 'prescriptionNo', minWidth: 110 },
        { title: '总计', key: 'sumPrice', minWidth: 70 },
        { title: '医保基金', key: 'ybjj', align: 'center', minWidth: 70 },
        { title: '大额统筹', key: 'detc', align: 'center', minWidth: 70 },
        { title: '民政救助', key: 'mzjz', align: 'center', minWidth: 70 },
        { title: '帐户支付', key: 'zhzf', align: 'center', minWidth: 70 },
        { title: '现金', key: 'xj', minWidth: 65 },
        {
          title: '医院超标',
          key: 'yycb',
          align: 'center',
          minWidth: 70,
          render: (h, params) => {
            return (
              <div
                style={params.row.yycb > 0 ? 'color:red;font-size:18px;' : ''}
              >
                {params.row.yycb}
              </div>
            )
          }
        },
        { title: '误差', key: 'wx', minWidth: 65 },
        {
          title: '',
          key: 'cancelDate',
          width: 112,
          renderHeader: (h, params) => {
            return h('div', (params.columns = this.timeTitle))
          },
          render: (h, params) => {
            if (params.row.cancelDate) {
              return (
                <div>
                  {params.row.cancelDate.substring(0, 10)}{' '}
                  {params.row.cancelDate.substring(11, 16)}
                </div>
              )
            }
          }
        }
      ],
      listDetailColumns: [
        { title: '序号', key: 'no', align: 'center', minWidth: 65 },
        { title: '姓名', key: 'pName', minWidth: 65 },
        { title: '门诊号', key: 'mzh', minWidth: 90 },
        {
          title: '收费时间',
          width: 112,
          key: 'openDate'
        },
        { title: '处方号', key: 'prescriptionNo', minWidth: 110 },
        { title: '名称', key: 'itemName', minWidth: 110 },
        { title: '单价', key: 'unitPrice', minWidth: 70 },
        { title: '数量', key: 'qty', minWidth: 70 },
        { title: '金额', key: 'totalPrice', minWidth: 70 },
        { title: '等级', key: 'level', minWidth: 60 },
        {
          title: '支付比例',
          key: 'payProportion',
          align: 'center',
          minWidth: 70
        },
        { title: '自费金额', key: 'payAmount', align: 'center', minWidth: 70 },
        {
          title: '医保支付金额',
          key: 'isPayAmount',
          align: 'center',
          minWidth: 90
        },
        {
          title: '医保上传数量',
          key: 'isUpQty',
          align: 'center',
          minWidth: 90
        },
        { title: '操作员', key: 'operationMan', minWidth: 65 }
      ],
      outListDetailColumns: [
        { title: '序号', key: 'no', minWidth: 65, align: 'center' },
        { title: '姓名', key: 'pName', minWidth: 65 },
        { title: '门诊号', key: 'mzh', minWidth: 110 },
        {
          title: '退费时间',
          key: 'cancelDate',
          minWidth: 110
        },
        { title: '处方号', key: 'prescriptionNo', minWidth: 110 },
        { title: '名称', key: 'itemName', minWidth: 110 },
        { title: '单价', key: 'unitPrice', minWidth: 65 },
        { title: '数量', key: 'qty', minWidth: 65 },
        { title: '金额', key: 'totalPrice', minWidth: 65 },
        { title: '操作员', key: 'operationMan', minWidth: 65 }
      ],
      timeTitle: '收费时间',
      tableLoading: false,
      table3Loading: false,
      dataCount: 0,
      buttonRole: BUTTONROLE,

      printJson: {},
      printDatas: {
        api: '',
        title: '',
        columns: [],
        params: {}
      }
    }
  },
  computed: {
    isMulti () {
      return (
        this.hospitalCheckedId === '0' ||
        this.hospitalCheckedId[0] === '0' ||
        this.hospitalCheckedId.length > 1
      )
    },
    hasPage () {
      return [4, 5, 6, 7].includes(this.tabCheckedId)
    },
    isTab3 () {
      return this.tabCheckedId === 3
    }
  },
  created () {
    let data = this.loadMenu()
    data.then(() => {
      let d = getNowDate().substring(0, 10)
      this.formData.date.push(getCurrentMonthFirst(), d)
      this.monthFirst = this.formData.date[0]
      this.monthLast = this.formData.date[1]
      // this.changeDate(this.formData.date)
    })
  },
  mounted () {
    this.tableHeight = document.documentElement.clientHeight - 360
  },
  methods: {
    ...mapMutations(['setColumns', 'setTitle', 'setParams', 'setApi']),
    keySearch () {
      this.getTableDataByKey(this.tabCheckedId)
    },
    maxTagPlaceholder (num) {
      return `+${num}机构`
    },
    // cy 双击table某一行触发【用于某些统计表单查看明细】
    dbClick (row, index) {
      if (!row.prescriptionNo) {
        return false
      }
      if (this.tabCheckedId === 4) {
        this.detailTableCheckedInfo.name = row.pName
        this.detailTableFlag = true
        this.detailTableColumns = this.listDetailColumns
      } else if (this.tabCheckedId === 5) {
        this.detailTableCheckedInfo.name = row.pName
        this.detailTableFlag = true
        this.detailTableColumns = this.outListDetailColumns
      }
      this.swsApi
        .swsGet(
          `BusinessTargetClntroller/BusinessTarget/ChargeDetailByNo/${
            row.prescriptionNo
          }`
        )
        .then(res => {
          this.detailTableData = res.data.result
          this.$nextTick(() => {
            let dom = document.querySelector('.financial-content .content')
            dom.scrollTo({
              top: dom.scrollHeight,
              left: 0,
              behavior: 'smooth'
            })
          })
        })
        .catch(e => {
          console.log(e)
        })
    },
    // 选择日期
    changeDate (e) {
      if (typeof e === 'object' && e[0]) {
        this.monthFirst = e[0]
        this.monthLast = e[1]
        this.formData.date = e
        this.changeTab(this.tabCheckedId)
      } else if (e && typeof e === 'string') {
        this.formData.singleDate = e
        this.changeTab(this.tabCheckedId)
      } else {
        this.formData.singleDate = getNowFormatDate(false)
      }
    },
    // 切换医院
    changeHospital (id) {
      if (typeof id === 'string') {
        this.hospitalCheckedId = id
        this.hospitalCheckedName = this.hospitalList.filter(
          v => v.dialysisId === id
        )[0].dialysisName
        this.getTableDataByKey(this.tabCheckedId)
      } else if (typeof id === 'object' && id.length !== 0) {
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
      }
    },
    // 获取机构
    getHospital (hospitalList) {
      this.hospitalList = hospitalList
      this.hospitalCheckedId = hospitalList[0].dialysisId
    },
    // 导出
    exportTable () {
      let filename = ''
      let pDom = document.querySelector('.table_title .p')
      filename = pDom.innerText

      this.$refs.table.exportCsv({
        filename: `${filename}（${this.formData.date[0]}~${
          this.formData.date[1]
        }）`,
        columns: this.columns,
        data: this.tableData
      })
    },
    // 打印
    print () {
      let routers = [
        'cost_summary',
        'cost_summary',
        'income_daily',
        'cost_summary',
        'cost_summary',
        'cost_summary',
        'cost_summary'
      ]
      let url = window.location.href.split('#')[0]
      let pDom = document.querySelector('.p')
      let filename = `${pDom.innerText}（${this.formData.date[0]}~${
        this.formData.date[1]
      }）`

      // 缓存打印数据
      this.printDatas = Object.assign(this.printDatas, {
        columns: JSON.parse(JSON.stringify(this.columns)),
        title: filename
      })
      // 针对收费清单和退费清单收费时间退费时间问题
      if ([4, 5].includes(this.tabCheckedId)) {
        let dateHeader = {}
        if (this.tabCheckedId === 4) {
          dateHeader = {
            key: 'cancelDate',
            title: '收费清单'
          }
        }
        if (this.tabCheckedId === 5) {
          dateHeader = {
            key: 'cancelDate',
            title: '退费清单'
          }
        }
        this.printDatas.columns.splice(-1, 1, dateHeader)
      }
      const { api, columns, title, params } = this.printDatas
      this.setTitle(title)
      this.setApi(api)
      this.setColumns(columns)
      this.setParams(params)

      window.open(`${url}#/print/${routers[this.tabCheckedId - 1]}`)
    },
    // 加载表格类型
    loadMenu () {
      return new Promise((resolve, reject) => {
        this.swsApi
          .swsPost(`BusinessTargetClntroller/BusinessTarget/1/1`)
          .then(res => {
            if (res.data.success) {
              this.tabData = res.data.result
              resolve('成功')
            }
          })
          .catch(e => {
            reject(e)
            console.log(e)
          })
      })
    },
    // 选择表格类型
    changeTab (index) {
      // console.log(index)
      this.tabCheckedId = index
      this.title = this.tabData.filter(
        item => item.childKey === index
      )[0].childValue
      this.columns = []
      this.tableData = []
      this.detailTableFlag = false // cy 重置显隐明细表单的flag
      this.current = 1
      this.getTableDataByKey(index)
    },
    // 选择收入总汇表类型
    changeType (index) {
      this.typeCheckedId = index
      this.columns = []
      this.tableData = []
      this.getTableDataByKey(this.tabCheckedId)
    },
    // cy 获取日报表
    getDayReportData () {
      let params = {
        centerId: this.hospitalCheckedId,
        beginTime: this.formData.date[0],
        endTime: this.formData.date[1],
        model: 1
      }
      this.swsApi
        .swsPost(
          '/BusinessTargetClntroller/BusinessTarget/DayReportAsync',
          params
        )
        .then(res => {
          if (res.data.success) {
            let result = res.data.result
            let data = []
            let xjObj = {}
            for (let item in result) {
              let obj = {}
              switch (item) {
                case 'total':
                  obj.title = '收费情况'
                  obj.desc = `合计：￥${result[item]}/大写：${convertCurrency(
                    result[item]
                  )}`
                  break
                case 'xj':
                  xjObj.title = '现金'
                  xjObj.desc = `收：￥${result[item]}/大写：${convertCurrency(
                    result[item]
                  )}`
                  break
                case 'individual':
                  obj.title = '个人帐户'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(
                    result[item]
                  )}`
                  break
                case 'mzbz':
                  obj.title = '民政补助'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(
                    result[item]
                  )}`
                  break
                case 'detc':
                  obj.title = '大额统筹'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(
                    result[item]
                  )}`
                  break
                case 'yycb':
                  obj.title = '医院超标'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(
                    result[item]
                  )}`
                  break
                case 'ybjj':
                  obj.title = '医保基金'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(
                    result[item]
                  )}`
                  break
                case 'pjst':
                  obj.title = '票据收退'
                  obj.desc = `收费：${result[item] ? result[item] : '暂无'}`
                  break
                case 'hmfw':
                  obj.title = '号码范围'
                  obj.desc = result[item] ? result[item] : '暂无'
                  break
                case 'sjph':
                  obj.title = '实际票号'
                  obj.desc = result[item] ? result[item] : '暂无'
                  break
              }
              if (JSON.stringify(obj) !== '{}') {
                data.push(obj)
              }
            }
            // let da = data.splice(data.indexOf('现金'), 1)
            data.splice(1, 0, xjObj)
            this.tableTotal = data
          }
        })
        .catch(e => {
          console.log(e)
        })
    },
    // 按值请求表格数据
    getTableDataByKey (index) {
      let actionCode = ''
      let jsonStr = {}
      switch (index) {
        case 1:
          // cy 当多选时长度为1且id为0，或者多选长度大于1时，展示后台传的表结构，单选id为0时同理
          if (this.isMulti) {
            this.headerFlag = true
          } else {
            this.columns = this.feeColumns
          }
          actionCode = 'BusinessTargetClntroller/BusinessTarget/ALL'
          jsonStr = {
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            model: this.typeCheckedId
          }
          break
        case 2:
          // cy 当多选时长度为1且id为0，或者多选长度大于1时，展示后台传的表结构，单选id为0时同理
          if (this.isMulti) {
            this.headerFlag = true
          } else {
            this.columns = this.feeColumns
          }
          actionCode = 'BusinessTargetClntroller/BusinessTarget/CostsAsync'
          jsonStr = {
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            model: 1
          }
          break
        case 3:
          // cy 当多选时长度为1且id为0，或者多选长度大于1时，展示后台传的表结构，单选id为0时同理
          if (this.isMulti) {
            this.headerFlag = true
          } else {
            this.columns = this.feeColumns
          }
          actionCode = 'BusinessTargetClntroller/BusinessTarget/ALL'
          jsonStr = {
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            model: 1
          }
          // 获取日报表
          this.getDayReportData()
          break
        case 4:
          this.timeTitle = '收费时间'
          this.columns = this.listColumns
          actionCode = 'BusinessTargetClntroller/BusinessTarget/Charge'
          jsonStr = {
            pageNum: this.current,
            pageSize: this.pageSize,
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            keyword: this.searchKey
          }
          break
        case 5:
          this.timeTitle = '退费时间'
          this.columns = this.listColumns
          actionCode = 'BusinessTargetClntroller/BusinessTarget/Refund'
          jsonStr = {
            pageNum: this.current,
            pageSize: this.pageSize,
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            keyword: this.searchKey
          }
          break
        case 6:
          this.columns = this.outListDetailColumns
          actionCode = 'BusinessTargetClntroller/BusinessTarget/RefundDetail'
          jsonStr = {
            pageNum: this.current,
            pageSize: this.pageSize,
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            keyword: this.searchKey
          }
          break
        case 7:
          this.columns = this.listDetailColumns
          actionCode = 'BusinessTargetClntroller/BusinessTarget/ChargeDetail'
          jsonStr = {
            pageNum: this.current,
            pageSize: this.pageSize,
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            keyword: this.searchKey
          }
          break
      }
      this.loadTable(actionCode, jsonStr)
    },
    loadTable (actionCode, jsonStr) {
      this.printJson = jsonStr
      this.tableLoading = true
      this.tableData = []
      this.dataCount = 0
      this.printDatas.api = actionCode
      // cy api接口的centerId类型更改为Array 在这里统一处理
      if (typeof jsonStr.centerId === 'string') {
        jsonStr.centerId = [jsonStr.centerId]
      }
      this.printDatas.params = jsonStr
      this.swsApi
        .swsPostCouldCancel(actionCode, jsonStr)
        .then(res => {
          if (res.data.success) {
            if (this.headerFlag) {
              let tableData = res.data.result.centerDayIncomeOutPuts
              let header = res.data.result.tableHeaders
              for (let item of header) {
                if (item.key === null) {
                  delete item.key
                }
                if (item.children === null) {
                  delete item.children
                } else {
                  for (let fitem of item.children) {
                    if (fitem.children === null) {
                      delete fitem.children
                    }
                    if (item.key === null) {
                      delete item.key
                    }
                  }
                }
              }
              this.columns = header
              this.tableData = tableData
              // cy 关闭该标志
              this.headerFlag = false
            } else {
              if (this.tabCheckedId === 2) {
                this.handleTable2Data(res.data.result)
              } else {
                this.tableData = res.data.result
                this.dataCount = res.data.dataCount
              }
            }
          } else {
            this.$Notice.error({
              title: '网络出错！',
              desc: res.data.error
            })
          }
          this.tableLoading = false
        })
        .catch(e => {
          console.log(e)
          if (!e.message.includes('取消上一次请求')) {
            this.tableLoading = false
          }
        })
    },
    handleTable2Data (data) {
      let columnData = data.shift().title.split(',')
      columnData.unshift('序号')
      // console.log(columnData)
      var columns = []
      for (let index in columnData) {
        if (columnData[index] === '序号') {
          columns.push({ title: columnData[index], key: index, width: '65' })
        } else {
          columns.push({ title: columnData[index], key: index, align: 'center' })
        }
      }
      this.columns = columns
      // console.log(columns)
      let index = 0
      let tableData = data.map(item => {
        index++
        let arr = item.title.split(',')
        let da = {}
        for (let i in arr) {
          if (i == 0) {
            da[`${i}`] = index
          } else {
            da[`${i}`] = arr[i]
          }
        }
        // console.log(da, item)
        return da
      })
      this.tableData = tableData
    },
    // 表格首行加粗
    rowClassName ({ itemName }, index) {
      // if (itemName === '合计') {
      //   return 'total'
      // }
    },
    // 分页
    changePage (page) {
      this.current = page
      this.getTableDataByKey(this.tabCheckedId)
    }
  }
}
</script>

<style scope="scoped" lang="less">
#incoming_outgoings {
  height: 100%;
  .ivu-table .total td {
    font-weight: bold;
  }
  .type {
    position: sticky;
    top: 0;
    z-index: 999;
    background: #fff;
    // margin: 20px 0;
    padding: 14px 0;
    border-bottom: 2px solid #f6f6f6;
    li {
      display: inline-block;
      color: #a8a8a8;
      font-size: 16px;
      width: 110px;
      text-align: center;
      transition: color 0.3s;
      &::before {
        position: absolute;
        display: block;
        content: '';
        top: -1px;
        left: 0;
        width: 100%;
        height: 2px;
        background-color: transparent;
        transition: all 0.3s;
      }
      &.typeActive {
        color: #4f95e8 !important;
        border-bottom: 2px solid #4f95e8;
      }
      &:hover {
        color: #4f95e8 !important;
      }
    }
  }
  .typeActive {
    color: #4f95e8 !important;
    border-bottom: 2px solid #4f95e8;
  }
  .content {
    background: #ffffff;
    .tab-wrapper {
      display: flex;
      flex-direction: column;
      height: 100%;
    }
    .tab-panel {
      height: 32px;
      // position: sticky;
      // top: 0;
      // z-index: 100;
    }
    .tab-content {
      flex: 1;
      overflow-y: auto;
      // margin-top: 10px;
    }
  }
  .ivu-form .ivu-form-item-label {
    color: #a3a3a3;
  }
  .p {
    text-align: center;
    font-size: 18px;
    color: #515a6e;
    line-height: 37px;
  }
  form {
    padding-top: 14px;
  }
  .ivu-form .ivu-form-item-label {
    color: #a3a3a3;
    font-size: 14px;
  }
  .ivu-form-item {
    margin-bottom: 10px;
  }
  .table {
    padding: 10px;
    background: #ffffff;
    .table-operate {
      padding: 10px 0;
      display: flex;
      justify-content: space-between;
      .operate-right {
        margin-right: 20px;
      }
      .button-group {
        display: flex;
        justify-content: flex-start;
        .search_box {
          margin: 0 5px;
        }
        & > span .span_title {
          margin: 0 10px;
        }
        span + span {
          margin-left: 10px;
        }
      }
      button + button {
        margin-left: 10px;
      }
    }
  }
  .time {
    margin-top: 10px;
    font-size: 13px;
    color: #333333;
    margin-bottom: 10px;
    span {
      margin-right: 20px;
    }
  }
  .time-right {
    float: right;
  }
  .button {
    text-align: right;
    button {
      margin: 2px 10px;
    }
  }
  .total-table {
    width: 100%;
    border-collapse: collapse;
    margin-bottom: 10px;
    td {
      border: 1px solid #aaaaaa;
      padding: 10px 20px;
    }
  }
  .page {
    float: right;
    margin-top: 10px;
  }
}
</style>
