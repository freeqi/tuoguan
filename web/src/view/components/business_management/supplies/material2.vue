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
                机构
                <Select
                  v-model="centerId"
                  filterable
                  placeholder
                  @on-change="changeHospital"
                  style="width: 190px;margin-left: 5px;"
                >
                  <Option
                    v-for="item in hospitalList"
                    :key="item.dialysisId"
                    :value="item.dialysisId"
                  >{{item.dialysisName}}</Option>
                </Select>
              </span>
              <span v-if="type1">
                <span style="margin:0 8px;">类别</span>
                <Select
                  v-model="modelType1"
                  style="width:150px;"
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
                  style="width: 190px;margin-left: 5px;"
                  @on-change="chooseDate"
                ></DatePicker>
              </span>
              <span v-if="recLi1==3">
                近效期时间段
                <Select
                  v-model="month"
                  style="width:120px;margin-left:8px;"
                  @on-change="chooseType1"
                >
                  <Option :value="1">1个月</Option>
                  <Option :value="3">3个月</Option>
                  <Option :value="6">6个月</Option>
                </Select>
              </span>
            </p>
            <p class="search-box">
              <span style="margin-left: 8px;" v-if="isSearch">
                查询
                <Input
                  v-model="search"
                  style="width: 180px;margin-left:8px;"
                  placeholder="搜索名称/厂家/供应商/单号..."
                />
              </span>
              <span class="out">
                <Button
                  type="primary"
                  v-permission="buttonRole.WZBB_DC"
                  style="margin-right: 10px;"
                  @click="importTab(1)"
                >导出</Button>
              </span>
            </p>
            <div class="cont">
              <h2>{{centerId === '0' ? '透析机构' : hospitalCheckedName}}{{title}}</h2>
              <!-- <span v-if="recLi1!=3" v-show="type1">库房：{{houseName1}}</span> -->
              <!-- <span v-if="dateData[0]">日期：{{dateData[0]}}至{{dateData[1]}}</span> -->
              <Table
                class="default-table"
                ref="Tab1"
                border
                :columns="columnsTop"
                :data="filterData"
                style="margin-top: 10px;clear: right;"
                :height="tableHeight"
                :loading="tabLoad"
              ></Table>
              <!--<Page :total="filterData.length" show-total style="float: right;margin-top: 10px;" @on-change="changePage2" :page-size="pageSize"/>-->
            </div>
          </div>
        </div>
        <div class="chart2" v-show="!isChart1">
          <div>
            <p>
              <span>
                机构
                <Select
                  v-model="centerId"
                  filterable
                  placeholder
                  @on-change="changeHospital"
                  style="width: 190px;margin-left: 5px;"
                >
                  <Option
                    v-for="item in hospitalList"
                    :key="item.dialysisId"
                    :value="item.dialysisId"
                  >{{item.dialysisName}}</Option>
                </Select>
              </span>
              <span v-if="type2">
                <span style="margin: 0 8px;">类别</span>
                <Select
                  v-model="modelType2"
                  style="width:150px;"
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
                  style="width: 180px;margin-left: 5px;"
                  @on-change="chooseDate2"
                ></DatePicker>
              </span>
            </p>
            <p class="search-box">
              <span style="margin-left: 8px;" v-if="isSearch">
                查询
                <Input
                  v-model="search"
                  style="width: 180px;margin-left:8px;"
                  placeholder="搜索名称/厂家/供应商/单号..."
                />
              </span>
              <span class="out">
                <Button
                  type="primary"
                  v-permission="buttonRole.WZBB_DC"
                  style="margin-right: 10px;"
                  @click="importTab(2)"
                >导出</Button>
              </span>
            </p>
            <div class="cont">
              <h2>{{centerId === '0' ? '透析机构' : hospitalCheckedName}}{{title}}</h2>
              <!-- <span v-if="type2">库房：{{houseName2}}</span> -->
              <!-- <span v-if="dateData2[0]">日期：{{dateData2[0]}}至{{dateData2[1]}}</span> -->
              <Table
                class="default-table"
                ref="Tab2"
                border
                :columns="columnsBot"
                :data="filterData"
                style="margin-top: 10px;"
                :loading="tabLoad"
                :height="tableHeight"
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
      </template>
    </financial-template>
  </div>
</template>

<script>
import financialTemplate from '@/components/financial-template'
import { toFilterKey } from '@/libs/tools'
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
      tableHeight: 0,
      modelType1: 1,
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
      columnsBot: [],
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
      buttonRole: BUTTONROLE
    }
  },
  async mounted () {
    await this.getList()
    this.chooseArr(this.recLi1)
    this.tableHeight = this.$refs.template.$el.clientHeight - 200
  },
  mixins: [column],
  computed: {
    isSearch () {
      return this.recLi !== 2
    },
    filterData () {
      let mdata = []
      let tableData = this.isChart1 ? this.data1 : this.data2
      if (this.search) {
        mdata = toFilterKey(tableData, 'medicalItemName,manufacturer,supplierName,inStorageNo,batchNo,outboundNo', this.search)
      } else {
        mdata = tableData
      }
      return mdata
    },
    isChart1 () {
      return this.recLi <= 5
    }
  },
  methods: {
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
            that.ListArr2 = response.data.result.slice(
              5,
              response.data.result.length
            )
          }
        })
    },
    chooseType1 (value) {
      this.chooseArr(this.recLi1)
      // eslint-disable-next-line eqeqeq
      if (value == 1) {
        this.houseName1 = '药品'
      } else if (value === 2) {
        this.houseName1 = '耗材'
      } else if (value === 3) {
        this.houseName1 = '固定资产'
      } else if (value === 4) {
        this.houseName1 = '低值易耗品'
      }
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
      this.actionCode = ''
      this.recLi = id
      console.log(id)
      this.title = this.ListArr[id - 1].childValue
      if (id <= 5) {
        this.recLi1 = id
        this.type1 = true
        this.columnsTop = []
        this.data1 = []
        this.jsonStr = {
          medicalItemType: this.modelType1,
          centerId: this.centerId,
          beginTime: this.dateData[0],
          endTime: this.dateData[1]
        }
      } else {
        this.recLi2 = id
        this.columnsBot = []
        this.data2 = []
        this.type2 = true
        this.jsonStr = {
          medicalItemType: this.modelType2,
          centerId: this.centerId,
          beginTime: this.dateData2[0],
          endTime: this.dateData2[1]
        }
      }
      switch (id) {
        case 1: // 进销存汇总表
          this.columnsTop = this.columns1
          this.actionCode =
            'MaterialsStatistica/MaterialsStatistica/EntersSellsSaves'
          break
        case 2: // 入出库汇总表
          this.columnsTop = this.columns2
          this.actionCode = 'MaterialsStatistica/MaterialsStatistica/Confluence'
          break
        case 3: // 近效期查询表
          this.columnsTop = this.columns3
          this.actionCode =
            'MaterialsStatistica/MaterialsStatistica/RecentValidityStatistics'
          this.jsonStr = {
            centerId: this.centerId,
            month: this.month,
            medicalItemType: this.modelType1
          }
          break
        case 4: // 外购入库统计
          this.columnsTop = this.columns4
          this.actionCode =
            'MaterialsStatistica/MaterialsStatistica/PutInStorage'
          break
        case 5: // 外购入库明细
          this.columnsTop = this.columns5
          this.actionCode =
            'MaterialsStatistica/MaterialsStatistica/PutInStorageDetail'
          break
        case 6: // 划价出库统计
          this.columnsBot = this.columns6
          this.actionCode =
            'MaterialsStatistica/MaterialsStatistica/AccuratelyOutbound'
          break
        case 7: // 划价出库明细
          this.columnsBot = this.columns7
          this.actionCode =
            'MaterialsStatistica/MaterialsStatistica/AccuratelyOutboundDetail'
          this.jsonStr = {
            medicalItemType: this.modelType2,
            centerId: this.centerId,
            beginTime: this.dateData2[0],
            endTime: this.dateData2[1],
            pageIndex: this.pageIndex2,
            pageSize: this.pageSize
          }
          break
        case 8: // 领用出库统计
          this.columnsBot = this.columns8
          this.actionCode = 'MaterialsStatistica/MaterialsStatistica/Recipients'
          break
        case 9: // 领用出库明细
          this.columnsBot = this.columns9
          this.actionCode =
            'MaterialsStatistica/MaterialsStatistica/RecipientsDetail'
          break
        case 10: // 其他出库统计
          this.columnsBot = this.columns10
          this.actionCode =
            'MaterialsStatistica/MaterialsStatistica/OtherOutbound'
          break
        case 11: // 其他出库明细统计
          this.columnsBot = this.columns11
          this.actionCode =
            'MaterialsStatistica/MaterialsStatistica/OtherOutboundDtail'
          break
      }
      this.getTabData(id)
    },
    changePage2 (value) {
      this.pageIndex2 = value
      this.chooseArr(this.recLi2)
    },
    getTabData (index) {
      let that = this
      if (index <= 5) {
        that.tabLoad = true
      } else {
        that.tabLoad = true
      }
      this.swsApi
        .swsPost(this.actionCode, this.jsonStr)
        .then(function (response) {
          if (response.data.code === 200) {
            if (index <= 5) {
              that.tabLoad = false
              that.data1 = response.data.result
            } else {
              that.tabLoad = false
              that.data2 = response.data.result
              that.dataCount = response.data.dataCount
            }
          } else {
            that.tabLoad = false
            that.tabLoad = false
          }
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
        exportColumn2 = this.columnsBot
        exportColumn2.forEach(item => {
          ColumnOne2[item.key] = item.title
        })
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
        }
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
  .chart2 {
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
  // .chart1 {
  //   height: 540px;
  // }
  // .chart2 {
  //   height: 600px;
  // }
  .search-box {
    & > span {
      display: inline-block;
      margin-top: 10px;
    }
  }
}
.inner-content .main .content {
  margin-left: 0 !important;
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
    margin-top: 10px;
    text-align: center;
    font-weight: normal;
    font-size: 18px;
  }
  span {
    margin-right: 30px;
  }
}
</style>
