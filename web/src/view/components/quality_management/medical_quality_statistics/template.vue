<template>
  <div id="control">
    <parent-view2 @on-change="changeHospital" ref="tree">
      <template
        slot-scope="slotProps"
      >{{slotProps.dialysis.dialysisId!='0'?`${slotProps.index}.${slotProps.dialysis.dialysisName}`:`${slotProps.dialysis.dialysisName}(${hospitalCount})`}}</template>
      <div slot="content">
        <ul class="tab-panel">
          <li
            class="tab-panel-item"
            v-for="item in tabData"
            :key="item.id"
            @click="changeTab(item.id)"
            :class="tabCheckedId == item.id ? 'active' : ''"
          >{{item.name}}</li>
        </ul>
        <div class="table">
          <h2
            class="title"
          >{{hospitalCheckedId === '0'?'医疗质量和安全管理指标统计':hospitalCheckedName}}-{{$route.meta.title}}-{{title}}({{isYear?`${Year}年`:`${Year}年${Month}月`}})</h2>
          <div class="flex-table">
            <div class="operate" :class="hospitalCheckedId === '0' ? 'margin200' : ''">
              <div class="datepick">
                <Select
                  v-if="isYear"
                  v-model="dateChecked"
                  placeholder="请选择年份"
                  @on-change="selectDate"
                >
                  <Option :value="item" v-for="item in YEAR_RANGE" :key="item">{{item}}年</Option>
                </Select>
                <DatePicker
                  v-else
                  v-model="dateChecked"
                  type="month"
                  format="yyyy-MM"
                  placeholder="请选择月份"
                  @on-change="selectDate"
                ></DatePicker>
              </div>
              <div class="download">
                <Button type="default" @click="exportData" v-permission="buttonPermission">导出</Button>
              </div>
            </div>
            <div>
              <Table
                class="default-table"
                border
                ref="table"
                size="small"
                highlight-row
                :height="tableHeight"
                :loading="tableLoading"
                :columns="table_column"
                :data="table_data"
                :class="hospitalCheckedId === '0' ? 'margin200' : ''"
                no-data-text="无数据"
              ></Table>
            </div>
          </div>
          <aside
            class="sec-nav"
            v-show="hospitalCheckedId === '0'"
            :class="hospitalCheckedId !== '0' ? 'disable' : ''"
          >
            <span>指标</span>
            <div class="sec-nav-box" :loading="loading">
              <p
                class="sec-nav-text"
                :class="indicatorCheckedId === item.enumValue ? 'active': ''"
                v-for="item in monthIndicatorList"
                :key="item.enumValue"
                :value="item.enumValue"
                @click="changeIndicatorType(item)"
              >&nbsp;&nbsp;{{item.desction}}</p>
            </div>
          </aside>
        </div>
      </div>
    </parent-view2>
  </div>
</template>

<script>
import parentView2 from '@/components/parent-view/parent-view2.vue'
export default {
  data () {
    return {
      tableHeight: 0,
      statisticTypeModel: 1,
      dateChecked: '',

      loading: false,
      tableLoading: false,
      table_data: [{}],
      table_column: [],

      hospitalCount: 0,
      hospitalCheckedId: '', // 选中的医院id
      hospitalCheckedName: '', // 选中的医院名称
      title: '', // 标题显示的指标

      tabCheckedId: 1, // 选择的tab标签id
      monthIndicatorList: [],
      indicatorCheckedId: 1, // 选择的月度指标id
      indicatorCheckedName: '', // 选中的月度指标名称
      Month: new Date().getMonth() + 1, // 当前日期
      Year: new Date().getFullYear(),

      dataCount: 0,
      table_column_detail: [],
      organizationName: {
        title: '机构名称',
        key: 'centerName',
        align: 'left',
        minWidth: 110
      },
      YEAR_RANGE: new Array(5)
        .fill(new Date().getFullYear())
        .map((item, index) => new Date().getFullYear() - index)
    }
  },
  props: {
    isYear: {
      type: Boolean,
      default: false
    },
    tabData: {
      type: Array,
      default: () => []
    },
    api: {
      type: String,
      default: ''
    },
    singleApi: {
      type: String,
      default: ''
    },
    tableColumnDetail: {
      type: Array,
      default: () => []
    },
    buttonPermission: {
      type: String,
      default: ''
    }
  },
  created () {
    this.$nextTick(() => {
      this.table_column_detail = this.tableColumnDetail
      this.dateChecked = this.isYear ? new Date().getFullYear() : `${new Date().getFullYear()}-${new Date().getMonth() + 1}`
    })
  },
  mounted () {
    this.$nextTick(() => {
      // cy：初始化当前的日期
      this.setCurrentDate()
      // 计算机构数量
      this.hospitalCount = this.$refs['tree'].hospitalList.lengt
      // cy 调整table高度
      this.tableHeight = document.documentElement.clientHeight - 290
      // console.log('tableHeight', document.documentElement.clientHeight, this.tableHeight)
    })
  },
  methods: {
    // cy：iview原生导出，未实现自定义居中表头
    exportData () {
      let date = this.isYear
        ? `${this.Year}年`
        : `${this.Year}年${this.Month}月`
      // let meteTitle = this.isYear ? this.$route.meta.title : '指标'
      this.$refs.table.exportCsv({
        filename: `${
          this.hospitalCheckedId === '0' ? '' : this.hospitalCheckedName
        }${date}${this.title}统计表`
      })
    },
    // cy:选择日期
    selectDate (v) {
      if (!v) return false
      this.setCurrentDate(v)
      this.getCenterData()
    },
    // cy:指标点击事件
    changeIndicatorType (item) {
      this.indicatorCheckedId = item.enumValue
      this.title = item.desction
      // this.getCenterData(item.enumValue)
      this.getCenterData()
    },
    changeHospital (id) {
      // cy：将医院名称查询出来并保存
      this.hospitalCheckedId = id
      // console.log('changeHospita0------', id)
      if (id !== '0') {
        this.hospitalCheckedName = this.$refs['tree'].hospitalList.filter(
          item => item.dialysisId === id
        )[0].dialysisName
      }

      this.changeTab(this.tabCheckedId)

      this.$nextTick(() => {
        // 计算机构数量
        this.hospitalCount = this.$refs['tree'].hospitalList.length
      })
    },
    // cy：获取时间
    setCurrentDate (...args) {
      let date = new Date()
      if (args.length) {
        let [year, month] = this.isYear ? args : args[0].split('-')
        this.Year = year
        this.Month = month ? parseInt(month) : date.getMonth() + 1
      } else {
        this.Month = date.getMonth() + 1
        this.Year = date.getFullYear()
      }
      // console.log('year,month------------', this.Year, this.Month)
      return `${this.Year}-${this.Month}`
    },
    // cy:获取单个指标对应的透析中心的数据
    getCenterData () {
      // getCenterData(id,...args){
      // id = id || 1
      // let dateObj = new Date()
      let params = {
        queryDateTime: `${this.Year}-${this.Month}`
      }
      // if (args.length!=0){
      //   let [date] = args
      //   let da = this.setCurrentDate(date)
      //   params.queryDateTime = da
      // } else{
      //   // params.queryDateTime = `${dateObj.getFullYear()}-${dateObj.getMonth()+1}`
      //   params.queryDateTime = `${this.Year}-${this.Month}`
      // }

      this.tableLoading = true
      let singleApi = ''
      if (this.hospitalCheckedId === '0') {
        singleApi = `${this.singleApi}/AllCenter`
        if (this.isYear) {
          params.medicalIndicatorsYear = this.indicatorCheckedId
        } else {
          params.medicalIndicatorsMonth = this.indicatorCheckedId
        }
      } else {
        singleApi = this.singleApi
        params.centerId = this.hospitalCheckedId
        params.medicalStatisticalType = this.tabCheckedId
      }
      this.swsApi
        .swsPost(singleApi, params)
        .then(res => {
          this.tableLoading = false
          if (res.data.success) {
            this.table_data = res.data.result
            this.dataCount = res.data.dataCount
          } else {
            this.$Message.error('网络错误，请稍后再试')
          }
        })
        .catch(e => {
          console.log(e)
        })
    },
    // cy获取月度指标列表
    getMonthIndicatorList (id) {
      // console.log('cy获取月度指标列表-')
      this.swsApi
        .swsPost(`${this.api}/${id}`)
        .then(res => {
          if (res.data.success) {
            this.monthIndicatorList = res.data.result
            // cy：这里需要将获取到的指标列表的第一个指标数据的id和名称赋值给indicatorCheckedId，name
            this.indicatorCheckedId = this.monthIndicatorList[0].enumValue
            this.title = this.monthIndicatorList[0].desction
            // console.log(this.indicatorCheckedId, this.title)
            // this.getCenterData(this.indicatorCheckedId)
            this.getCenterData()
          }
        })
        .catch(e => {
          console.log(e)
        })
    },
    getTableList (i) {
      i = i || 1
      let args = {
        centerId: this.hospitalCheckedId
      }
      this.loading = true
      this.swsApi
        .swsPost(this.table_api, args)
        .then(res => {
          this.loading = false
          if (res.data.success) {
            this.table_data = res.data.result
            this.dataCount = res.data.dataCount
          } else {
            this.$Message.error('网络错误，请稍后再试')
          }
        })
        .catch(e => {
          this.loading = false
        })
    },
    changeTab (id) {
      this.tabCheckedId = id
      // this.table_column = (this.hospitalCheckedId === '0') ? this.table_column_all : this.table_column_detail
      // cy将判断统一
      if (this.hospitalCheckedId === '0') {
        this.getMonthIndicatorList(id)
        // cy：全部显示时table新增机构列
        this.table_column = [
          ...[this.organizationName],
          ...this.table_column_detail
        ]
      } else {
        this.title = this.tabData.filter(item => item.id === id)[0].name
        this.table_column = this.table_column_detail
        // this.getCenterData(this.tabCheckedId)
        this.getCenterData()
      }
    }
  },
  components: {
    parentView2
  }
}
</script>

<style scoped lang="less">
#control {
  position: relative;
  width: 100%;
  height: 100%;
  /deep/ .content {
    margin-right: 20px;
    width: 100%;
    background: #fff;
    overflow-x: hidden;
  }
  .tab-panel {
    font-size: 0;
    display: inline-block;
    height: 32px;
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
      transition: color 0.3s;
      &::before {
        position: absolute;
        display: block;
        content: '';
        top: -1px;
        left: 0;
        width: 100%;
        height: 2px;
        // background-color: transparent;
        transition: all 0.3s;
      }
      &:first-child {
        border-left: solid 1px #eaeaea;
        &.active {
          border-left-color: transparent;
        }
      }
      &.active {
        color: #ee5151;
        // border-bottom-color: transparent;
        border-bottom: none;
        border-top: solid 2px #ff6e5c;
        // &::before {
        //   background-color: #ee5151;
        // }
      }
      &:hover {
        color: #ee5151;
      }
    }
  }
  .table {
    .ivu-table-wrapper {
      & /deep/ .ivu-table .ivu-table-cell {
        padding-right: 8px;
        padding-left: 8px;
      }
    }
    h2 {
      margin-bottom: 10px;
      margin-left: 20px;
    }
    &::before,
    &::after {
      display: block;
      height: 0;
      content: '';
      visibility: hidden;
      clear: both;
    }
    padding: 10px;
    background: #ffffff;
    .sec-nav {
      float: left;
      margin-right: -100%;
      margin-top: -31px;
      color: #333333;
      width: 195px;
      font-size: 12px;
      span {
        // display: inline-block;
        line-height: 20px;
        font-size: 16px;
      }
      .sec-nav-box {
        margin-top: 11px;
        padding: 10px 0;
        border-radius: 4px;
        border: solid 1px #eaeaea;
        font-size: 13px;
        .sec-nav-text {
          color: #555555;
          // font-weight: 600;
          line-height: 28px;
          &:hover {
            background: #c7ebfa;
            cursor: pointer;
          }
        }
        .active {
          background: #c7ebfa;
        }
      }
      &.disable + * {
        .default-table {
          margin-left: 0 !important;
        }
      }
    }
    .title {
      text-align: center;
    }
    .flex-table {
      float: right;
      width: 100%;
      .operate {
        width: 100%;
        text-align: left;
        display: inline-block;
        margin-bottom: 10px;
        .datepick {
          display: inline-block;
          text-align: left;
          width: 160px;
        }
        .download {
          // float: right;
          display: inline-block;
          text-align: right;
          margin-left: 10px;
        }
      }
      .margin200 {
        margin-left: 206px;
      }
      // .ivu-table-wrapper {
      //   border: transparent;
      // }
      // .ivu-table .big-font-cell{
      //   font-size: 15px;
      //   font-weight: 600;
      // }
      // .ivu-table .small-font-cell {
      //   text-align: right;
      //   font-size: 10px;
      //   font-weight: 400;
      // }
    }
  }
}
</style>
