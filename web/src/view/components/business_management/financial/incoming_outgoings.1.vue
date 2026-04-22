<template>
  <div id="incoming_outgoings">
    <financial-template
      :tabData="tabData"
      @on-change="changeTab"
      @on-initial-hospital="getHospital"
    >
      <div class="tab-content" slot="content">
        <Form :label-width="90">
          <Row>
            <FormItem label="机构选择">
              <Select
                v-model="hospitalCheckedId"
                :multiple="[1,2,3].includes(tabCheckedId)"
                :max-tag-count="2"
                :max-tag-placeholder="maxTagPlaceholder"
                filterable
                placeholder="请选择机构"
                @on-change="changeHospital"
                style="width: 190px"
              >
                <Option
                  v-for="item in hospitalList"
                  :key="item.dialysisId"
                  :value="item.dialysisId"
                >{{item.dialysisName}}</Option>
              </Select>
            </FormItem>
          </Row>
        </Form>
        <div v-show="tabCheckedId == 1">
          <!-- <ul class="type">
              <li
                :class="typeCheckedId == item.index ? 'typeActive' : ''"
                v-for="(item) of type"
                :key="item.index"
                @click="changeType(item.index)"
              >{{item.title}}</li>
          </ul>-->
          <div style="padding: 0 19px;font-size: 18px;">
            <RadioGroup v-model="typeCheckedId" @on-change="changeType">
              <Radio v-for="item of type" :key="item.index" :label="item.index">{{item.title}}</Radio>
            </RadioGroup>
          </div>
          <div class="table">
            <Form ref="formData" :model="formData" :label-width="80">
              <Row>
                <Col :lg="8" :md="8">
                  <FormItem label="日期选择">
                    <DatePicker
                      :value="formData.date"
                      @on-change="changeDate"
                      type="daterange"
                      :options="options"
                    ></DatePicker>
                  </FormItem>
                </Col>
                <Col :lg="8" :md="8">
                  <FormItem label="查询" v-show="[4,5,6,7].includes(tabCheckedId)">
                    <Input v-model="searchKey" placeholder="请输入姓名或单号"/>
                  </FormItem>
                </Col>
                <Col :lg="7" :md="7" class="button">
                  <Button @click="exportTable()" type="primary" v-permission="buttonRole.SZBB_DC">导出</Button>
                  <Button @click="print" type="primary" ghost v-permission="buttonRole.SZBB_DC">打印</Button>
                </Col>
              </Row>
            </Form>
            <p
              class="p"
            >{{hospitalCheckedId === '0' ? '透析中心' : hospitalCheckedName}}{{title}}（{{this.type[this.typeCheckedId-1].title}}）</p>
            <!-- <div class="time">
                <span>时间：{{this.monthFirst}} 至 {{this.monthLast}}</span>
            </div>-->
          </div>
        </div>
        <div v-show="tabCheckedId != 1">
          <div class="table">
            <Form ref="formData" :model="formData" :label-width="80">
              <Row>
                <Col :lg="8" :md="10">
                  <FormItem label="日期选择">
                    <DatePicker
                      :value="formData.date"
                      @on-change="changeDate"
                      v-show="!isTab3"
                      type="daterange"
                      :options="options"
                    ></DatePicker>
                    <DatePicker
                      :value="formData.singleDate"
                      @on-change="changeDate"
                      v-show="isTab3"
                      type="date"
                    ></DatePicker>
                  </FormItem>
                </Col>
                <Col :lg="15" :md="11" class="button">
                  <Button @click="exportTable()" type="primary" v-permission="buttonRole.SZBB_DC">导出</Button>
                  <Button @click="print" type="primary" ghost v-permission="buttonRole.SZBB_DC">打印</Button>
                </Col>
              </Row>
            </Form>
            <p class="p">
              {{hospitalCheckedId === '0' ? '透析中心' : hospitalCheckedName}}{{title}}
              <span
                v-show="tabCheckedId ==2"
              >（按收费项目）</span>
            </p>
            <!-- <div class="time">
                <span>操作员：{{data.name}}</span>
                <span>
                  时间：
                  <label v-show="!isTab3">{{this.monthFirst}} 至 {{this.monthLast}}</label>
                  <label v-show="isTab3">{{this.formData.singleDate}}</label>
                </span>

                <div class="time-right" v-show="isTab3">
                  <span>
                    借款：
                    <label class="red">{{data.price}}元</label>
                  </span>
                  <span>借出：{{data.outPrice}}元</span>
                </div>
            </div>-->
          </div>
        </div>
        <div class="table">
          <Table
            @on-row-dblclick="dbClick"
            :height="tableHeight"
            :data="tableData"
            :row-class-name="rowClassName"
            ref="table"
            :loading="tableLoading"
            :columns="columns"
            class="default-table"
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
            ref="detailTable"
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
                <label v-show="!isTab3">{{this.monthFirst}} 至 {{this.monthLast}}</label>
                <label v-show="isTab3">{{this.formData.singleDate}}</label>
              </span>
              <div class="time-right">
                <span>
                  借款：
                  <label class="red">{{data.price}}元</label>
                </span>
                <span>借出：{{data.outPrice}}元</span>
              </div>
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
import {
  getCurrentMonthFirst,
  getNowDate,
  getNowFormatDate
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
      pageSize: 10,
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
      title: '',
      monthFirst: '',
      monthLast: '',
      columns: [],
      allColumns: [],
      tableData: [],
      feeColumns: [
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
      tableTotal: [
        {
          title: '收费情况',
          desc: '合计:428372.90/大写:肆拾贰万捌仟叁佰柒拾贰圆玖角整'
        },
        {
          title: '个人帐户',
          desc: '收：￥1523.31/大写：壹仟伍佰贰拾叁圆叁角壹分'
        },
        {
          title: '民政补助',
          desc: '收：￥29007.22/大写：贰万玖仟零柒圆贰角贰分'
        },
        {
          title: '大额统筹',
          desc: '收：￥112522.74/大写：壹拾壹万贰仟伍佰贰拾贰圆柒角肆分'
        },
        {
          title: '现金',
          desc: '收：￥39329.98/大写：叁万玖仟叁佰贰拾玖圆玖角捌分'
        },
        {
          title: '医院超标',
          desc: '收：￥35.65/大写：叁拾伍圆陆角伍分'
        },
        {
          title: '医保基金',
          desc: '收：￥245954.00/大写：贰拾肆万伍仟玖佰伍拾肆圆整'
        },
        { title: '票据收退', desc: '收费:1605张,退费:19张,重打:11张' },
        { title: '号码范围', desc: 'S0010648～S0013403' },
        { title: '实际票号', desc: 'SF17975～SF19590' }
      ],
      listColumns: [
        { title: '患者姓名', key: 'pName' },
        { title: '性别', key: 'sex', width: 60 },
        { title: '年龄', key: 'age', width: 60, align: 'center' },
        { title: '医保类别', key: 'healthCareType' },
        { title: '类别', key: 'mzType' },
        { title: '处方号', key: 'prescriptionNo' },
        { title: '总计', key: 'sumPrice' },
        { title: '医保基金', key: 'ybjj' },
        { title: '大额统筹', key: 'detc' },
        { title: '民政救助', key: 'mzjz' },
        { title: '帐户支付', key: 'zhzf' },
        { title: '现金', key: 'xj' },
        { title: '医院超标', key: 'yycb' },
        { title: '误差', key: 'wx' },
        {
          title: '',
          key: 'cancelDate',
          width: 110,
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
        { title: '姓名', key: 'pName' },
        { title: '门诊号', key: 'mzh' },
        {
          title: '收费时间',
          key: 'openDate',
          render: (h, params) => {
            if (params.row.openDate) {
              return (
                <div>
                  {params.row.openDate.substring(0, 10)}{' '}
                  {params.row.openDate.substring(11, 16)}
                </div>
              )
            }
          }
        },
        { title: '处方号', key: 'prescriptionNo' },
        { title: '名称', key: 'itemName' },
        { title: '单价', key: 'unitPrice' },
        { title: '数量', key: 'qty' },
        { title: '金额', key: 'totalPrice' },
        { title: '等级', key: 'level' },
        { title: '支付比例', key: 'payProportion' },
        { title: '支付金额', key: 'payAmount' },
        { title: '操作员', key: 'operationMan' }
      ],
      outListDetailColumns: [
        { title: '姓名', key: 'pName' },
        { title: '门诊号', key: 'mzh' },
        {
          title: '退费时间',
          key: 'cancelDate',
          width: 110,
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
        },
        { title: '处方号', key: 'prescriptionNo' },
        { title: '名称', key: 'itemName' },
        { title: '单价', key: 'unitPrice' },
        { title: '数量', key: 'qty' },
        { title: '金额', key: 'totalPrice' },
        { title: '操作员', key: 'operationMan' }
      ],
      // inListDetailColumns: [
      //   { title: '姓名', key: 'name' },
      //   { title: '门诊号', key: 'name' },
      //   { title: '收费时间', key: 'name' },
      //   { title: '处方号', key: 'name' },
      //   { title: '名称', key: 'name' },
      //   { title: '单价', key: 'name' },
      //   { title: '数量', key: 'name' },
      //   { title: '金额', key: 'name' },
      //   { title: '等级', key: 'name' },
      //   { title: '支付比例', key: 'name' },
      //   { title: '支付金额', key: 'name' },
      //   { title: '操作员', key: 'name' }
      // ],
      timeTitle: '收费时间',
      tableLoading: false,
      table3Loading: false,
      dataCount: 0,

      buttonRole: BUTTONROLE
    }
  },
  computed: {
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
      this.changeDate(this.formData.date)
    })
  },
  mounted () {
    this.tableHeight = document.documentElement.clientHeight - 360
  },
  methods: {
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
        // console.log('查看明细!', row.prescriptionNo)
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
              behavior: 'smooth'})
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
        // console.log('string', id)
        this.hospitalCheckedId = id
        this.hospitalCheckedName = this.hospitalList.filter(
          v => v.dialysisId === id
        )[0].dialysisName
        this.getTableDataByKey(this.tabCheckedId)
      } else if (typeof id === 'object' && id.length !== 0) {
        this.hospitalCheckedId = id
        // console.log('object', id)
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
      if (this.tabCheckedId === 1) {
        filename = `${
          this.hospitalCheckedId === '0' ? '透析中心' : this.hospitalCheckedName
        }${this.title}（${this.type[this.typeCheckedId - 1].title}）`
      } else {
        filename = `${
          this.hospitalCheckedId === '0' ? '透析中心' : this.hospitalCheckedName
        }${this.title}`
        filename += this.tabCheckedId === 2 ? '（按收费项目）' : ''
      }
      this.$refs.table.exportCsv({
        filename,
        columns: this.columns,
        data: this.tableData
      })
    },
    // 打印
    print () {},
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
      this.hospitalCheckedId = '0' // cy 重置选中的机构 不然会有小bug
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
    // 按值请求表格数据
    getTableDataByKey (index) {
      let actionCode = ''
      let jsonStr = {}
      switch (index) {
        case 1:
          // cy 当多选时，为全部，或者长度大于1时展示后台传的表结构，单选为全部时同理
          if (this.hospitalCheckedId === '0' || this.hospitalCheckedId[0] === '0' || this.hospitalCheckedId.length > 1) {
            // this.columns = this.allFeeColumns
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
          if (this.hospitalCheckedId === '0' || this.hospitalCheckedId.length > 1) {
            this.columns = this.allCostColumns
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
          if (this.hospitalCheckedId === '0' || this.hospitalCheckedId.length > 1) {
            // this.columns = this.allFeeColumns
            this.headerFlag = true
          } else {
            this.columns = this.feeColumns
          }
          actionCode = 'BusinessTargetClntroller/BusinessTarget/ALL'
          jsonStr = {
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.singleDate,
            endTime: this.formData.singleDate,
            model: 1
          }
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
            endTime: this.formData.date[1]
          }
          break
        case 5:
          this.timeTitle = '退费清单'
          this.columns = this.listColumns
          actionCode = 'BusinessTargetClntroller/BusinessTarget/Refund'
          jsonStr = {
            pageNum: this.current,
            pageSize: this.pageSize,
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1]
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
            endTime: this.formData.date[1]
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
            endTime: this.formData.date[1]
          }
          break
      }
      this.loadTable(actionCode, jsonStr)
    },
    loadTable (actionCode, jsonStr) {
      this.tableLoading = true
      this.tableData = []
      // cy api接口的centerId类型更改为Array 在这里统一处理
      if (typeof jsonStr.centerId === 'string') {
        jsonStr.centerId = [jsonStr.centerId]
      }
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
            } else {
              this.tableData = res.data.result
              this.dataCount = res.data.dataCount
            }
            // cy 关闭该标志
            this.headerFlag = false
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
        })
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
    padding: 0 10px;
    // padding-bottom: 20px;
    background: #ffffff;
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
