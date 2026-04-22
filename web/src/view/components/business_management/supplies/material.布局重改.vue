<template>
  <div id="material">
    <financial-template
      :tabData="ListArr"
      @on-change="chooseArr"
      @on-initial-hospital="getHospital"
      ref="template"
    >
      <template slot="content">
        <div class="chart1" v-show="isChart1">
          <div>
            <p>
              <span>
                <span style="margin-right: 8px;">机构</span>
                <Select
                  v-model="centerId"
                  filterable
                  placeholder
                  @on-change="changeHospital"
                  style="width: 190px"
                >
                  <Option
                    v-for="item in hospitalList"
                    :key="item.dialysisId"
                    :value="item.dialysisId"
                  >{{item.dialysisName}}</Option>
                </Select>
              </span>
            </p>
            <p class="search-box">
              <span v-if="type1">
                <span style="margin-right: 8px;">类别</span>
                <Select
                  v-model="modelType1_M"
                  v-show="typeMultiple"
                  style="width:150px;margin-right:8px;"
                  @on-change="chooseType_M"
                  :max-tag-count="1"
                  multiple
                >
                  <Option :value="1">药品</Option>
                  <Option :value="2">耗材</Option>
                  <Option :value="3" v-if="recLi1!=3">固定资产</Option>
                  <Option :value="4" v-if="recLi1!=3">低值易耗品</Option>
                </Select>
                <Select
                  v-show="!typeMultiple && this.recLi !== 12"
                  v-model="modelType1"
                  style="width:150px;margin-right:8px;"
                  @on-change="chooseType1"
                >
                  <Option :value="1">药品</Option>
                  <Option :value="2">耗材</Option>
                  <Option :value="3" v-if="recLi1!=3">固定资产</Option>
                  <Option :value="4" v-if="recLi1!=3">低值易耗品</Option>
                </Select>
              </span>
              <span v-if="recLi1!=3">
                日期
                <DatePicker
                  type="daterange"
                  placeholder="请选择日期"
                  style="width: 180px;margin-left: 8px;"
                  @on-change="chooseDate"
                ></DatePicker>
              </span>
              <span v-if="recLi1==3">
                近效期时间段
                <Select
                  v-model="month"
                  style="width:120px;margin-left:8px;"
                  @on-change="chooseType3"
                >
                  <Option :value="1">1个月</Option>
                  <Option :value="3">3个月</Option>
                  <Option :value="6">6个月</Option>
                </Select>
              </span>
              <span style="margin-left: 8px;" v-if="isSearch">
                查询
                <Input
                  v-model="search"
                  style="width: 180px;margin-left:8px;"
                  placeholder="搜索..."
                />
              </span>
              <span class="out">
                <Button
                  type="primary"
                  v-permission="buttonRole.WZBB_DC"
                  style="margin-right: 10px;"
                  @click="importTab(1)"
                >导出</Button>
                <Button
                  type="primary"
                  ghost
                  style="margin-right: 10px;"
                  @click="print"
                >打印</Button>
              </span>
            </p>
            <div class="cont">
              <h2>{{centerId === '0' ? '透析机构' : hospitalCheckedName}}{{title}}</h2>
              <Table
                class="default-table"
                ref="Tab1"
                border
                highlight-row
                :columns="columnsTop"
                :data="filterData"
                style="margin-top: 10px;clear: right;"
                :height="tableHeight"
                :loading="tabLoad"
                @on-row-dblclick="getTableDetailData"
              ></Table>
              <!--<Page :total="filterData.length" show-total style="float: right;margin-top: 10px;" @on-change="changePage2" :page-size="pageSize"/>-->
            </div>
          </div>
        </div>
        <div class="chart2" v-show="!isChart1">
          <div>
            <p>
              <span style="margin-right: 8px;" v-if="recLi === 12">
                物品
                <Input
                  v-model="goodsSearchKey"
                  style="width: 180px;margin-left:8px;"
                  placeholder="请选择物品..."
                  @on-focus="loadGoods"
                />
              </span>
              <span>
                机构
                <Select
                  v-model="centerId"
                  filterable
                  placeholder
                  @on-change="changeHospital"
                  style="width: 190px;margin-left: 8px;"
                >
                  <Option
                    v-for="item in hospitalList"
                    :key="item.dialysisId"
                    :value="item.dialysisId"
                  >{{item.dialysisName}}</Option>
                </Select>
              </span>
            </p>
            <p class="search-box">
              <span v-if="type2 && recLi !== 12">
                <span style="margin-right: 8px;">类别</span>
                <Select
                  v-model="modelType2"
                  style="width:150px;margin-right:8px;"
                  @on-change="chooseType2"
                >
                  <Option :value="1">药品</Option>
                  <Option :value="2">耗材</Option>
                </Select>
              </span>
              <span>
                日期
                <DatePicker
                  type="daterange"
                  placeholder="请选择日期"
                  style="width: 180px;margin-left: 8px;"
                  @on-change="chooseDate2"
                ></DatePicker>
              </span>
              <span style="margin-left: 8px;" v-if="isSearch">
                查询
                <Input
                  v-model="search"
                  style="width: 180px;margin-left:8px;"
                  placeholder="搜索名称/单号..."
                />
              </span>
              <span class="out">
                <Button
                  type="primary"
                  v-permission="buttonRole.WZBB_DC"
                  style="margin-right: 10px;"
                  @click="importTab(2)"
                >导出</Button>
                <Button
                  type="primary"
                  ghost
                  style="margin-right: 10px;"
                  @click="print"
                >打印</Button>
              </span>
            </p>
            <div class="cont">
              <h2>{{centerId === '0' ? '透析机构' : hospitalCheckedName}}{{title}}</h2>
              <!-- <span v-if="type2">库房：{{houseName2}}</span> -->
              <!-- <span v-if="dateData2[0]">日期：{{dateData2[0]}}至{{dateData2[1]}}</span> -->
              <div style="position:relative;">
                <Spin fix v-show="showTzTable && recLi2==12">请选择物品...</Spin>
                <Table
                  class="default-table"
                  ref="Tab2"
                  border
                  highlight-row
                  :columns="columnsTop"
                  :data="filterData"
                  style="margin-top: 10px;"
                  :loading="tabLoad"
                  :height="tableHeight"
                  @on-row-dblclick="getTableDetailData"
                ></Table>
                <Page
                  v-if="recLi2==7"
                  :total="dataCount"
                  show-total
                  style="float: right;margin-top: 10px;"
                  @on-change="changePage2"
                  :page-size="pageSize"
                  :current.sync="startPage"
                />
              </div>
            </div>
          </div>
        </div>
        <div style="padding: 0 15px 20px;background:#fff;" v-show="detailTableFlag">
          <div class="cont">
            <h2>{{title}}明细</h2>
            <Table
              class="default-table"
              ref="detai_table"
              border
              height="300"
              :columns="columnsDetail"
              :data="detailTableData"
              style="margin-top: 10px;"
            ></Table>
          </div>
        </div>

        <!-- 加载所有物品 -->
        <Modal v-model="selectModal" width="800">
          <p slot="header" style="text-align: center;">选择项目</p>
          <Row>
            <Col span="24">
              <Input placeholder="检索..." v-model="modalSearchKey" ref="searchInput"/>
              <Spin size="large" fix v-if="listShow || !goodsFilter"></Spin>
              <ul class="liList">
                <li v-for="item in goodsFilter" :key="item.id" @click="selectOneInfo(item)">
                  <div>{{item.itemName}}</div>
                </li>
              </ul>
            </Col>
          </Row>
          <div slot="footer">
            <Button @click="selectModal=false;modalSearchKey=''">关闭</Button>
          </div>
        </Modal>
      </template>
    </financial-template>
  </div>
</template>

<script>
import financialTemplate from '@/components/financial-template'
import { toFilterKey } from '@/libs/tools'
import { mapMutations } from 'vuex'
import column from './column'
const BUTTONROLE = {
  WZBB_DC: 'WZBB_DC'
}
export default {
  name: 'material',
  components: {
    financialTemplate
  },
  data () {
    return {
      showTzTable: false, // cy 物品台账查询时未选择物品显示加载中的标识
      listShow: false,
      medicalItemId: '', // 选中的物品id
      modalSearchKey: '', // cy 搜索模态框的绑定关键词
      selectModal: false, // cy 加载物品
      goodsList: [], //
      goodsSearchKey: '', // cy 台账物品搜索的绑定关键词

      columnsDetail: [],
      detailTableData: [], // cy 明细table
      detailTableFlag: false, // cy 显示明细table
      tableHeight: 0,
      modelType1: 1,
      modelType1_M: [1],
      hospitalCheckedName: '',

      title1: '',
      houseName1: '全部',

      title: '',
      houseName: '全部',
      ListArr: [],
      recLi: 1,
      hospitalList: [],
      tabLoad: false,

      ListArr1: [],
      recLi1: 1,
      columnsTop: [],
      dateData: [],
      data1: [],
      search: '',
      type1: true,

      ListArr2: [],
      houseName2: '全部',
      recLi2: 6,
      // columnsBot: [],
      data2: [],
      title2: '',
      modelType2: 1,
      type2: true,
      startPage: 1,

      month: null,
      centerId: null,
      actionCode: '',
      pageIndex2: 1,
      pageSize: 20,
      dateData2: [],
      jsonStr: {},
      dataCount: 0,
      columnsSP: [
        // 进销存汇总表导出表头
        { title: '序号', key: 'serialNumber', width: 65 },
        { title: '名称', key: 'medicalItemName', width: 180 },
        { title: '规格', key: 'specifications', width: 180 },
        { title: '厂家', key: 'manufacturer', width: 240 },
        { title: '计算单位', key: 'unitName', width: 80 },
        { title: '成本价', key: 'inPrice', width: 85 },
        { title: '材质', key: 'itemTypeName', width: 120 },
        { title: '初期_数量', key: 'beginCount', align: 'center', width: 70 },
        {
          title: '初期_销售金额',
          key: 'beginSalePrice',
          align: 'center',
          width: 100
        },
        {
          title: '初期_成本金额',
          key: 'beginCostPrice',
          align: 'center',
          width: 100
        },
        {
          title: '初期_差价',
          key: 'beginDifference',
          align: 'center',
          width: 70
        },
        {
          title: '入库_数量',
          key: 'putInStorageCount',
          align: 'center',
          width: 70
        },
        {
          title: '入库_销售金额',
          key: 'putInStorageSalePrice',
          align: 'center',
          width: 100
        },
        {
          title: '入库_成本金额',
          key: 'salesCostPrice',
          align: 'center',
          width: 100
        },
        {
          title: '入库_差价',
          key: 'putInStorageDifference',
          align: 'center',
          width: 70
        },
        {
          title: '出库_数量',
          key: 'outboundCount',
          align: 'center',
          width: 70
        },
        {
          title: '出库_销售金额',
          key: 'outboundSalePrice',
          align: 'center',
          width: 100
        },
        {
          title: '出库_成本金额',
          key: 'outboundCostPrice',
          align: 'center',
          width: 100
        },
        {
          title: '出库_差价',
          key: 'outboundDifference',
          align: 'center',
          width: 70
        },
        { title: '销售_数量', key: 'salesCount', align: 'center', width: 70 },
        {
          title: '销售_销售金额',
          key: 'salesSalePrice',
          align: 'center',
          width: 100
        },
        {
          title: '销售_成本金额',
          key: 'salesCostPrice',
          align: 'center',
          width: 100
        },
        {
          title: '销售_差价',
          key: 'salesDifference',
          align: 'center',
          width: 70
        },
        { title: '期末_数量', key: 'endCount', align: 'center', width: 70 },
        {
          title: '期末_销售金额',
          key: 'endSalePrice',
          align: 'center',
          width: 100
        },
        {
          title: '期末_成本金额',
          key: 'endCostPrice',
          align: 'center',
          width: 100
        },
        { title: '期末_差价', key: 'endDifference', align: 'center', width: 70 }
      ],
      SPTabData: [],
      buttonRole: BUTTONROLE,
      printDatas: {
        api: '',
        title: '',
        columns: [],
        params: {}
      }
    }
  },
  async mounted () {
    await this.getList()
    await this.chooseArr(this.recLi1)
    this.tableHeight = this.$refs.template.$el.clientHeight - 200
  },
  mixins: [column],
  computed: {
    isSearch () {
      return this.recLi !== 2 && this.recLi !== 12
    },
    filterData () {
      let mdata = []
      let tableData = this.isChart1 ? this.data1 : this.data2
      if (this.search) {
        mdata = toFilterKey(
          tableData,
          'medicalItemName,manufacturer,supplierName,inStorageNo,batchNo,outboundNo',
          this.search
        )
      } else {
        mdata = tableData
      }
      return mdata
    },
    isChart1 () {
      return this.recLi <= 5
    },
    // 接口汇总
    UISetting () {
      return {
        '1': {
          // 进销存汇总表
          columnsTop: this.columns1,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/EntersSellsSaves'
        },
        '2': {
          // 入出库汇总表
          columnsTop: this.columns2,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/Confluence'
        },
        '3': {
          // 近效期查询表
          columnsTop: this.columns3,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/RecentValidityStatistics'
        },
        '4': {
          // 外购入库统计
          columnsTop: this.columns4,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/PutInStorage'
        },
        '5': {
          // 外购入库明细
          columnsTop: this.columns5,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/PutInStorageDetail'
        },
        '6': {
          // 划价出库统计
          columnsTop: this.columns6,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/AccuratelyOutbound'
        },
        '7': {
          // 划价出库明细
          columnsTop: this.columns7,
          // actionCode: 'MaterialsStatistica/MaterialsStatistica/AccuratelyOutboundDetail'
          actionCode: 'MaterialsStatistica/MaterialsStatistica/AccuratelyOutboundDetailById'
        },
        '8': {
          // 领用出库统计
          columnsTop: this.columns8,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/Recipients'
        },
        '9': {
          // 领用出库明细
          columnsTop: this.columns9,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/RecipientsDetail'
        },
        '10': {
          // 其他出库统计
          columnsTop: this.columns10,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/OtherOutbound'
        },
        '11': {
          // 其他出库明细统计
          columnsTop: this.columns11,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/OtherOutboundDtail'
        },
        '12': {
          // 物品台账
          columnsTop: this.columns12,
          actionCode: 'MaterialsStatistica/MaterialsStatistica/ItemStandingBook'
        }
      }
    },
    typeMultiple () {
      return [1, 2, 4, 5].includes(this.recLi)
    },
    goodsFilter () {
      let data = []
      if (this.goodsList) {
        data = toFilterKey(
          this.goodsList,
          'medicalItemName,aliasName,manufacturer,packaging,mnemonic',
          this.modalSearchKey)
      } else {
        data = []
      }
      return data
    }
  },
  methods: {
    ...mapMutations(['setColumns', 'setTitle', 'setParams', 'setApi']),
    selectOneInfo (item) {
      this.selectModal = false
      this.goodsSearchKey = item.medicalItemName
      this.medicalItemId = item.id
      this.chooseArr(this.recLi)
    },
    // cy 加载所有物品
    loadGoods () {
      let that = this
      this.selectModal = true
      let params = {
        pageSize: 9999,
        pageNum: 1
      }
      // 查询物品
      if (!this.goodsList.length) {
        this.listShow = true
        this.swsApi
          .swsPost('Data/MedicalItemRecord/list', params)
          .then(function (res) {
            if (res.data.success) {
              let arr = []
              // 过滤非药品、耗材
              let result = res.data.result.filter(res => {
                return [1, 2].includes(res.medicalItemType)
              })
              for (let i in result) {
                let item = {}
                // 物品名 + 别名 + 生产厂家 + 包装规格 +助记码
                item.itemName = `${result[i].medicalItemName} ${result[i].brand
                  ? '(' + result[i].brand + ')' : ''} ${result[i].aliasName
                  ? '(' + result[i].aliasName + ')' : ''} ${result[i].manufacturer
                  ? '(' + result[i].manufacturer + ')' : ''} ${result[i].packaging
                  ? result[i].packaging : ''} [${result[i].mnemonic}]`
                item.id = result[i].id
                // cy 用于模糊搜素
                item.medicalItemName = result[i].medicalItemName // 物品名称
                item.aliasName = result[i].aliasName // 别名
                item.manufacturer = result[i].manufacturer // 生产厂家
                item.packaging = result[i].packaging // 包装规格
                item.packageUnit = result[i].packageUnit // 包装单位ID
                item.mnemonic = result[i].mnemonic // 助记码
                item.medicalItemType = result[i].medicalItemType // 类型
                arr.push(item)
              }
              that.goodsList = arr
              that.listShow = false
            }
          })
      } else {
        that.listShow = false
      }
    },
    // 获取机构
    getHospital (hospitalList) {
      this.hospitalList = hospitalList
      this.centerId = hospitalList[0].dialysisId
    },
    // 切换医院
    changeHospital (id) {
      this.centerId = id
      this.hospitalCheckedName = this.hospitalList.filter(
        v => v.dialysisId === id
      )[0].dialysisName
      this.chooseArr(this.recLi)
    },
    getList () {
      let that = this
      return this.swsApi
        .swsPost('BusinessTargetClntroller/BusinessTarget/2/1')
        .then(function (response) {
          // 获取列表
          if (response.data.code === 200) {
            that.ListArr = response.data.result
            that.ListArr1 = response.data.result.slice(0, 5)
            that.ListArr2 = response.data.result.slice(5)
          }
        })
    },
    chooseType1 (value) {
      this.modelType1 = value
      this.chooseArr(this.recLi1)
      let types = ['药品', '耗材', '固定资产', '低值易耗品']
      this.houseName1 = types[value - 1]
    },
    // 多选
    chooseType_M (value) {
      this.modelType1_M = value
      this.chooseArr(this.recLi1)
    },
    // 近效期
    chooseType3 (value) {
      this.chooseArr(this.recLi1)
    },
    chooseType2 (value) {
      this.pageIndex2 = 1
      this.startPage = 1
      if (value === 1) {
        this.houseName2 = '药品'
      } else if (value === 2) {
        this.houseName2 = '耗材'
      }
      this.chooseArr(this.recLi2)
    },
    chooseDate (value) {
      this.dateData = value
      this.chooseArr(this.recLi1)
    },
    chooseDate2 (value) {
      this.dateData2 = value
      this.pageIndex2 = 1
      this.startPage = 1
      this.chooseArr(this.recLi2)
    },
    chooseArr (id) {
      this.search = ''
      this.detailTableFlag = false
      if (!this.ListArr.length) return
      this.actionCode = ''
      this.recLi = id
      this.title = this.ListArr[id - 1].childValue
      if (id <= 5) {
        this.recLi1 = id
        this.type1 = true
        this.columnsTop = []
        this.data1 = []
        this.jsonStr = {
          medicalItemType: this.typeMultiple
            ? this.modelType1_M.join(',')
            : this.modelType1, // 多选
          centerId: this.centerId,
          beginTime: this.dateData[0],
          endTime: this.dateData[1]
        }
      } else {
        this.recLi2 = id
        this.columnsTop = []
        this.data2 = []
        this.type2 = true
        this.jsonStr = {
          medicalItemType: this.modelType2,
          centerId: this.centerId,
          beginTime: this.dateData2[0],
          endTime: this.dateData2[1]
        }
      }
      let { columnsTop, actionCode } = this.UISetting[id]
      this.columnsTop = columnsTop
      this.actionCode = actionCode
      // 3、7特殊需要
      if (this.recLi === 3) {
        this.jsonStr.month = this.month
      } else if (this.recLi === 7) {
        this.actionCode = 'MaterialsStatistica/MaterialsStatistica/AccuratelyOutboundDetail'
        this.jsonStr.pageIndex = this.pageIndex2
        this.jsonStr.pageSize = this.pageSize
      } else if (this.recLi === 12) {
        if (this.goodsSearchKey === '') {
          this.showTzTable = true
          // console.log(this.showTzTable = true, 'this.showTzTable')
          return false
        } else {
          this.showTzTable = false
        }
        this.jsonStr = {
          id: this.medicalItemId,
          centerId: this.centerId,
          beginTime: this.dateData2[0],
          endTime: this.dateData2[1]
        }
      }
      this.printDatas = Object.assign(this.printDatas, {
        params: JSON.parse(JSON.stringify(this.jsonStr)),
        api: this.actionCode
      })
      this.getTabData(id)
    },
    getTableDetailData (row, index) {
      if (!row.id) {
        return false
      }
      // cy 根据入库、出库单的id查询明细表单
      if ([4, 6, 8, 10].includes(this.recLi)) {
        this.detailTableFlag = true
        let { columnsTop, actionCode } = this.UISetting[this.recLi + 1]
        this.columnsDetail = columnsTop
        this.swsApi.swsPost(actionCode, { id: row.id, centerId: this.centerId })
          .then(res => {
            if (res.data.success) {
              this.detailTableData = res.data.result
              this.$nextTick(() => {
                let dom = document.querySelector('.financial-content .content')
                dom.scrollTo({
                  top: dom.scrollHeight,
                  left: 0,
                  behavior: 'smooth'
                })
              })
            }
          })
          .catch(e => {
            console.log(e)
          })
      }
    },
    changePage2 (value) {
      this.pageIndex2 = value
      this.chooseArr(this.recLi2)
    },
    getTabData (index) {
      let that = this
      that.tabLoad = true
      this.swsApi
        .swsPost(this.actionCode, this.jsonStr)
        .then(function (response) {
          if (response.data.code === 200) {
            if (index <= 5) {
              that.data1 = response.data.result
            } else {
              that.data2 = response.data.result
              that.dataCount = response.data.dataCount
            }
          }
          that.tabLoad = false
        })
        .catch(e => {
          that.tabLoad = false
          console.log(e)
        })
    },
    importTab (index) {
      if (index === 1) {
        let exportColumn = []
        let filename = ''
        let TabData = []
        let ColumnOne = {}
        let Date = ''
        let houseName = ''
        let title = {}
        if (this.recLi1 === 1) {
          exportColumn = this.columnsSP
        } else {
          exportColumn = this.columnsTop
        }
        exportColumn.forEach(item => {
          ColumnOne[item.key] = item.title
        })
        TabData = JSON.parse(JSON.stringify(this.data1))
        filename = this.hospitalCheckedName + this.title
        if (this.dateData.length === 0) {
          Date = ''
        } else {
          Date = '日期：' + this.dateData[0] + '至' + this.dateData[1]
        }
        if (this.houseName1) {
          houseName = '库房：' + this.houseName1
        } else {
          houseName = ''
        }
        if (this.recLi1 === 1) {
          title = {
            medicalItemName: houseName,
            manufacturer: Date,
            putInStorageSalePrice: filename
          }
        } else if (this.recLi1 === 2) {
          title = {
            parentBusinessType: houseName,
            childBusinessType: Date,
            salePriceSum: filename
          }
        } else if (this.recLi1 === 3) {
          if (this.month === 1) {
            Date = '近1个月'
          } else if (this.month === 3) {
            Date = '近3个月'
          } else {
            Date = '近6个月'
          }
          title = { itemTypeName: Date, inPrice: filename }
        } else if (this.recLi1 === 4) {
          title = { rowNumber: Date, difference: filename }
        } else if (this.recLi1 === 5) {
          title = { rowNumber: Date, medicalItemName: filename }
        }
        TabData.unshift(title, ColumnOne)
        this.$refs.Tab1.exportCsv({
          filename: filename,
          columns: exportColumn,
          data: TabData.filter((data, index) => {
            for (let i in data) {
              // 解决日期格式问题
              if (typeof data[i] === 'string') {
                if (
                  data[i].indexOf('T') !== -1 &&
                  data[i].indexOf('T') === 10
                ) {
                  data[i] =
                    data[i].substring(0, 10) + ' ' + data[i].substring(11, 16)
                }
              }
            }
            return index < TabData.length
          }),
          noHeader: true
        })
      } else if (index === 2) {
        let exportColumn2 = []
        let filename2 = ''
        let TabData2 = []
        let ColumnOne2 = {}
        let Date2 = ''
        let houseName2 = ''
        let title2 = {}
        exportColumn2 = this.columnsTop
        filename2 = this.hospitalCheckedName + this.title
        if (this.dateData2.length === 0) {
          Date2 = ''
        } else {
          Date2 = '日期：' + this.dateData2[0] + '至' + this.dateData2[1]
        }
        if (this.houseName1) {
          houseName2 = '库房：' + this.houseName2
        } else {
          houseName2 = ''
        }
        if (this.recLi2 === 6) {
          title2 = {
            serialNumber: houseName2,
            outboundNo: Date2,
            salesAmount: filename2
          }
        } else if (this.recLi2 === 7) {
          title2 = {
            parentBusinessType: houseName2,
            childBusinessType: Date2,
            salePriceSum: filename2
          }
          this.data2 = this.SPTabData
        } else if (this.recLi2 === 8) {
          title2 = {
            serialNumber: houseName2,
            outboundNo: Date2,
            differenceAmount: filename2
          }
        } else if (this.recLi2 === 9) {
          title2 = {
            serialNumber: houseName2,
            outboundNo: Date2,
            founderDate: filename2
          }
        } else if (this.recLi2 === 10) {
          title2 = { serialNumber: Date2, differenceAmount: filename2 }
        } else if (this.recLi2 === 11) {
          title2 = { serialNumber: Date2, address: filename2 }
        } else if (this.recLi2 === 12) {
          title2 = {
            specifications: Date2,
            putInStorageSalePrice: filename2
          }
          exportColumn2 = this.columns12Export
        }
        exportColumn2.forEach(item => {
          ColumnOne2[item.key] = item.title
        })
        TabData2 = JSON.parse(JSON.stringify(this.data2))
        TabData2.unshift(title2, ColumnOne2)
        this.$refs.Tab2.exportCsv({
          filename: filename2,
          columns: exportColumn2,
          data: TabData2.filter((data, index) => {
            for (let i in data) {
              // 解决日期格式问题
              if (typeof data[i] === 'string') {
                if (
                  data[i].indexOf('T') !== -1 &&
                  data[i].indexOf('T') === 10
                ) {
                  data[i] =
                    data[i].substring(0, 10) + ' ' + data[i].substring(11, 16)
                }
              }
            }
            return index < TabData2.length
          }),
          noHeader: true
        })
      }
    },
    print () {
      let url = window.location.href.split('#')[0]
      // 缓存打印数据
      this.printDatas = Object.assign(this.printDatas, {
        columns: JSON.parse(JSON.stringify(this.columnsTop)),
        title: (this.centerId === '0' ? '透析机构' : this.hospitalCheckedName) + this.title
      })

      const { api, columns, title, params } = this.printDatas
      this.setTitle(title)
      this.setApi(api)
      this.setColumns(columns)
      this.setParams(params)

      window.open(`${url}#/print/material_statement`)
    }
  }
}
</script>

<style scoped lang="less">
#material {
  position: relative;
  width: 100%;
  height: 100%;
  font-size: 14px;
  .chart1,
  .chart2,
  .detai_table {
    height: 100%;
    padding: 20px 15px;
    background: #ffffff;
    position: relative;
    // .btn-group {
    //   position: absolute;
    //   right: 40px;
    //   z-index: 10;
    //   /deep/ .ivu-btn-primary {
    //     background: #4f95e8;
    //     border-color: #4f95e8;
    //     box-shadow: 2px 2px 6px rgba(79, 149, 232, 0.35);
    //   }
    // }
  }
  .search-box {
    & > span {
      display: inline-block;
      margin-top: 10px;
    }
  }
}
.inner-content .main .content {
  background: #ffffff !important;
}
.tab-panel {
  position: absolute;
  top: -32px;
  left: 0;
  font-size: 14px;
}
.out {
  float: right;
  margin-right: 10px;
}
.cont {
  h2 {
    padding-top: 10px;
    text-align: center;
    font-weight: normal;
    font-size: 18px;
  }
  span {
    margin-right: 30px;
  }
}
.liList {
  max-height: 450px;
  overflow-y: scroll;
  li {
    height: 30px;
    line-height: 30px;
    font-size: 12px;
    border-bottom: 1px solid #eaeaea;
    cursor: pointer;
    white-space: nowrap;
    text-overflow: ellipsis;
    overflow: hidden;
    i {
      color: red;
    }
  }
}
</style>
