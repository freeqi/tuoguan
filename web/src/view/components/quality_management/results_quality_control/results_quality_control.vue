<template>
  <div id="control">
    <parent-view2 @on-change="changeHospital" ref="tree">
      <template
        slot-scope="slotProps"
      >{{`${slotProps.index !== 0 ? `${slotProps.index}.` : ''}${slotProps.dialysis.dialysisName}`}}</template>
      <div slot="content">
        <div class="chart" ref="chart">
          <div class="btn-group">
            <Select
              class
              v-model="statisticTypeModel"
              @on-change="changeChartType"
              style="width:200px;"
            >
              <Option
                :value="item.type"
                v-for="item in statisticType"
                :key="item.type"
              >{{item.name}}</Option>
            </Select>
            <div class="right">
              <!-- <DatePicker
                v-model="dateModel"
                type="month"
                format="yyyy-MM"
                style="width: 160px;margin-right: 10px"
                placeholder="请选择月份"
                @on-change="changeDate"
              ></DatePicker>-->
              <DatePicker
                v-model="dateModel"
                type="daterange"
                style="margin-right: 10px"
                placeholder="请选择时间段"
                @on-change="changeDate"
              ></DatePicker>
              <Button
                shape="circle"
                icon="md-cloud-upload"
                v-permission="buttonRole.JGZK_DC"
                @click="download"
              >导出</Button>
            </div>
          </div>
          <div ref="dom" style="height: 100%;"></div>
        </div>
        <div class="operate-box">
          <h3 class="operate-box-title">{{operateTitle}}</h3>
          <div class="table">
            <Table
              :loading="loading"
              ref="table"
              :columns="table_column"
              :data="table_data"
              no-data-text="无数据"
            ></Table>
          </div>
          <div class="pagination" v-if="dataCount > 10">
            <div>
              <Page
                :total="dataCount"
                :page-size="pageSize"
                :current.sync="startPage"
                @on-change="changePage"
              ></Page>
            </div>
          </div>
        </div>
      </div>
    </parent-view2>
  </div>
</template>

<script>
import parentView2 from '@/components/parent-view/parent-view2.vue'
import chartSetting from '../chartSetting.js'
const BUTTONROLE = {
  JGZK_DC: 'JGZK_DC'
}
export default {
  data () {
    return {
      statisticTypeModel: 1,
      chartTitle: '',
      operateTitle: '',
      statisticType: [],

      tagA: '',

      // dateRangeModel: [],
      dateModel: '',

      loading: false,
      table_data: [{}],
      table_column: [],
      table_api: '',
      dataCount: 0,
      pageSize: 9,
      startPage: 1,
      // cy 时间段筛选
      beginTime: '',
      endTime: '',

      legendTitle: {},

      hospitalCheckedId: '',
      hospitalCheckedName: '',
      hospitalList: '',
      // cy:高血压质控率统计
      gxyType: {
        type: 1,
        name: '高血压质控率',
        title: '高血压质控率详情',
        column: [
          {
            title: '患者人数',
            key: 'panterCount',
            width: 120
          },
          {
            title: '检测截止日期',
            key: 'times',
            width: 120
          },
          {
            title: '血压＜140/90MMHG的60岁以下患者',
            key: 'lcheckCount'
          },
          {
            title: '血压＜150/90MMHG的60岁以上患者',
            key: 'rcheckCount'
          }
        ],
        tableApi: 'ResultControl/QualityHypertension/Data',
        chartApi: 'ResultControl/QualityHypertension/Chart'
      },
      // cy:肾性贫血控制率统计
      sxpxType: {
        type: 2,
        name: '肾性贫血控制率',
        title: '肾性贫血控制率详情',
        column: [
          {
            title: '患者人数',
            key: 'panterCount'
          },
          {
            title: '检测截止日期',
            key: 'times'
          },
          {
            title: '检查人数',
            key: 'checkCount'
          },
          {
            title: '血红蛋白≥100G/L的患者人数',
            key: 'rcheckCount'
          }
        ],
        tableApi: 'ResultControl/Hemoglobin/Data',
        chartApi: 'ResultControl/Hemoglobin/Chart'
      },
      // cy:全段甲状旁腺素（IPTH）控制率统计
      ipthType: {
        type: 3,
        name: '全段甲状旁腺素（IPTH）控制率',
        title: '全段甲状旁腺素（IPTH）控制率详情',
        column: [
          {
            title: '患者人数',
            key: 'panterCount'
          },
          {
            title: '检测人数',
            key: 'checkCount'
          },
          {
            title: '检测截止日期',
            key: 'times'
          },
          {
            title: '控制率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'ResultControl/IPTH/Data',
        chartApi: 'ResultControl/IPTH/Chart'
      },
      // cy:血磷控制率统计
      pType: {
        type: 4,
        name: '血磷控制率',
        title: '血磷控制率详情',
        column: [
          {
            title: '患者人数',
            key: 'panterCount'
          },
          {
            title: '检测人数',
            key: 'checkCount'
          },
          {
            title: '检测截止日期',
            key: 'times'
          },
          {
            title: '控制率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'ResultControl/P/Data',
        chartApi: 'ResultControl/P/Chart'
      },
      // cy:血钙控制率统计
      caType: {
        type: 5,
        name: '血钙控制率',
        title: '血钙控制率详情',
        column: [
          {
            title: '患者人数',
            key: 'panterCount'
          },
          {
            title: '检测人数',
            key: 'checkCount'
          },
          {
            title: '检测截止日期',
            key: 'times'
          },
          {
            title: '控制率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'ResultControl/Ca/Data',
        chartApi: 'ResultControl/Ca/Chart'
      },
      // 血清蛋白控制率统计
      xqdbType: {
        type: 6,
        name: '血清蛋白控制率',
        title: '血清蛋白控制率详情',
        column: [
          {
            title: '患者人数',
            key: 'panterCount'
          },
          {
            title: '检测人数',
            key: 'checkCount'
          },
          {
            title: '检测截止日期',
            key: 'times'
          },
          {
            title: '控制率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'ResultControl/Serum/Data',
        chartApi: 'ResultControl/Serum/Chart'
      },
      // Kt/V和URR控制率统计
      urrType: {
        type: 7,
        name: 'Kt/V和URR控制率',
        title: 'Kt/V和URR控制率详情',
        column: [
          {
            title: '患者人数',
            key: 'panterCount'
          },
          {
            title: '检测人数',
            key: 'checkCount'
          },
          {
            title: '检测截止日期',
            key: 'times'
          },
          {
            title: '控制率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'ResultControl/URR/Data',
        chartApi: 'ResultControl/URR/Chart'
      },
      // 透析期间体重增长控制率
      weightType: {
        type: 8,
        name: '透析期间体重增长控制率',
        title: '透析期间体重增长控制率详情',
        column: [
          {
            title: '患者人数',
            key: 'panterCount'
          },
          {
            title: '检测人数',
            key: 'checkCount'
          },
          {
            title: '检测截止日期',
            key: 'times'
          },
          {
            title: '控制率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'ResultControl/WeightGain/Data',
        chartApi: 'ResultControl/WeightGain/Chart'
      },
      // 乙型肝炎和丙型肝炎的发病率
      gyType: {
        type: 9,
        name: '乙型肝炎和丙型肝炎的发病率',
        title: '乙型肝炎和丙型肝炎的发病率详情',
        column: [
          {
            title: '患者人数',
            key: 'panterCount'
          },
          {
            title: '检测人数',
            key: 'checkCount'
          },
          {
            title: '检测截止日期',
            key: 'times'
          },
          {
            title: '发病率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'ResultControl/HepatitisB/Data',
        chartApi: 'ResultControl/HepatitisB/Chart'
      },
      // 血液生化定时检验完成率统计
      xyshjcType: {
        type: 4,
        name: '血液生化定时检验完成率统计',
        title: '血液生化定时检验完成详情',
        column: [
          {
            title: '维持性患者数量',
            key: 'partitionName'
          },
          {
            title: '电解质(标准>=80%)',
            key: 'disinfectionTime'
          },
          {
            title: '肾功能(标准>=50%)',
            key: 'partitionName'
          },
          {
            title: '肝功能(标准>=30%)',
            key: 'disinfectionTime'
          },
          {
            title: '血脂(标准>=30%)',
            key: 'partitionName'
          },
          {
            title: '统计时间',
            key: 'disinfectionTime'
          }
        ],
        tableApi: 'HospitalFeeling/DisinfectionRoom/List',
        chartApi: 'HospitalFeeling/EndotoxinByMonth/List',
        monthApi: 'HospitalFeeling/DisinfectionRoomStatisticsByCenter/List/'
      },
      buttonRole: BUTTONROLE
    }
  },
  created () {
    this.statisticType.push(
      // this.xyshjcType,
      // this.xqdbType,
      this.gxyType,
      this.sxpxType,
      this.ipthType,
      this.pType,
      this.caType,
      this.xqdbType,
      this.urrType,
      this.weightType,
      this.gyType
    )
  },
  mounted () {
    this.$nextTick(() => { })
  },
  mixins: [chartSetting],
  methods: {
    changeHospital (id, list) {
      this.hospitalCheckedId = id
      this.dateRangeModel = ''
      list.length && (this.hospitalList = list)
      this.hospitalCheckedName = this.hospitalList.filter(
        v => v.dialysisId === id
      )[0].dialysisName
      this.operatechangeType(this.statisticTypeModel, false)
    },
    changeChartType (v) {
      this.operatechangeType(v, true)
    },
    operatechangeType (_type, flage = false) {
      let organizationName = {
        title: '机构名称',
        key: 'centerName',
        width: 240
      }
      let {
        title,
        name,
        column,
        tableApi,
        chartApi
      } = this.statisticType.filter(item => item.type === _type)[0]
      this.hospitalCheckedId === '0' &&
        (column = [...[organizationName], ...column])
      this.operateTitle = title
      this.chartTitle = name
      this.table_api = tableApi
      this.table_column = column
      this.table_data = []
      this.secondStateModel = ''
      this.startPage = 1
      this.legendTitle = {}

      this.dateRangeModel = ''
      this.table_api && this.getTableList(this.startPage)
      this.setChart(_type, chartApi, this.beginTime)
    },
    getTableList (i) {
      i = i || 1
      let args = {
        centerId: this.hospitalCheckedId,
        // isQualified: this.secondStateModel,
        pageSize: this.pageSize,
        beginTime: this.beginTime,
        // endTime: this.endTime,
        pageNum: i,
        title: this.legendTitle
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
    changePage (page) {
      this.getTableList(page)
    },
    changeDate (date) {
      this.beginTime = date[0]
      let type = this.statisticTypeModel
      this.startPage = 1
      this.getTableList(this.startPage)
      // if (this.hospitalCheckedId === '0') {
      let { chartApi } = this.statisticType.filter(
        item => item.type === type
      )[0]
      this.setChart(type, chartApi, date[0])
      // }
    },
    secondStateChange (v) {
      this.startPage = 1
      this.getTableList(this.startPage)
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
  .chart {
    height: 436px;
    padding: 20px;
    background: #ffffff;
    position: relative;
    .btn-group {
      position: absolute;
      left: 0;
      width: 100%;
      padding: 0 20px;
      display: flex;
      justify-content: space-between;
      z-index: 10;
      /deep/ .ivu-btn-primary {
        margin-left: 10px;
        background: #4f95e8;
        border-color: #4f95e8;
        box-shadow: 2px 2px 6px rgba(79, 149, 232, 0.35);
      }
    }
  }
  .operate-box {
    margin-top: 20px;
    padding-bottom: 20px;
    background: #ffffff;
    &-title {
      padding-left: 20px;
      height: 42px;
      line-height: 42px;
      font-size: 14px;
      border-bottom: 1px solid #f3f3f4;
    }
    .operate {
      padding: 20px 20px 10px;
      .options {
        display: inline-block;
        & + .options {
          margin-left: 10px;
        }
      }
      .btn-groups {
        color: #999;
        font-size: 13px;
        margin-bottom: 10px;
        /deep/ .ivu-radio-group-button .ivu-radio-wrapper-checked {
          color: #ffffff;
          background: #4f95e8;
        }
      }
    }
    .table {
      margin-top: 20px;
      padding: 0 20px;
    }
    .ivu-table-wrapper {
      & /deep/ .ivu-table th {
        background: #f8f8f9;
      }
      /deep/ .success {
        color: #36cd9a;
      }
      /deep/ .error {
        color: #f06767;
      }
    }
  }
}
</style>
