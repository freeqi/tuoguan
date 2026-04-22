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
              <Select
                class
                v-model="dateModel"
                style="width: 140px;margin-right: 10px"
                placeholder="请选择年份"
                @on-change="changeDate"
              >
                <Option :value="item" v-for="item in YEAR_RANGE" :key="item">{{item}}年</Option>
              </Select>
              <Button
                shape="circle"
                icon="md-cloud-upload"
                v-permission="buttonRole.GCZK_DC"
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
  GCZK_DC: 'GCZK_DC'
}
export default {
  data () {
    return {
      hospitalCheckedName: '',
      hospitalList: [],

      statisticTypeModel: 1,
      chartTitle: '',
      operateTitle: '',
      statisticType: [],

      dateModel: new Date().getFullYear(),

      loading: false,
      table_data: [{}],
      table_column: [],
      table_api: '',
      dataCount: 0,
      pageSize: 10,
      startPage: 1,
      beginTime: new Date().getFullYear(),
      endTime: '',
      legendTitle: {},

      hospitalCheckedId: '',
      // 血常规定时检验完成率统计
      xcgType: {
        type: 1,
        name: '血常规定时检验完成率统计',
        title: '血常规定时检验完成详情',
        chartTitle: '血常规定时检验合格率',
        column: [
          {
            title: '患者人数',
            key: 'panterCount'
          },
          {
            title: '检测例次',
            key: 'checkCount'
          },
          {
            title: '检测截止日期',
            key: 'times'
          },
          {
            title: '完成率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'QualityControl/RoutineBloodRecord/Data',
        chartApi: 'QualityControl/RoutineBloodRecord/Chart'
      },
      // 全段甲状旁腺素（IPTH）定时检验完成率
      qdjzxType: {
        type: 2,
        name: '全段甲状旁腺素（IPTH）定时检验完成率统计',
        title: '全段甲状旁腺素（IPTH）定时检验完成详情',
        chartTitle: '全段甲状旁腺素（IPTH）定时检验合格率',
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
            title: '完成率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'QualityControl/IPTH/Data',
        chartApi: 'QualityControl/IPTH/Chart'
      },
      // 血液生化定时检测率
      xyshType: {
        type: 3,
        name: '血液生化定时检测完成率统计',
        title: '血液生化定时检测完成详情',
        chartTitle: '血液生化定时检测检验合格率',
        column: [
          {
            title: '患者人数',
            key: 'panterCount',
            width: 120
          },
          {
            title: '检测截止日期',
            key: 'times',
            width: 100
          },
          {
            title: '电解质(标准>=80%)',
            key: 'scheckCount'
          },
          {
            title: '肾功能(标准>=50%)',
            key: 'rcheckCount'
          },
          {
            title: '肝功能(标准>=30%)',
            key: 'lcheckCount'
          },
          {
            title: '血脂(标准>=30%)',
            key: 'scheckCount'
          }
        ],
        tableApi: 'QualityControl/BloodBiochemical/Data',
        chartApi: 'QualityControl/BloodBiochemical/Chart'
      },
      // 尿素清除指数（KT/V）、尿素下降率（URR）记录完成率
      nsqczsType: {
        type: 4,
        name: '尿素清除指数（KT/V）、尿素下降率（URR）记录完成率',
        title: '尿素清除指数（KT/V）、尿素下降率（URR）记录完成详情',
        chartTitle: '尿素清除指数（KT/V）、尿素下降率（URR）记录合格率',
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
            title: '完成率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'QualityControl/URR/Data',
        chartApi: 'QualityControl/URR/Chart'
      },
      // MHD患者的血清前白蛋白定时检验完成率
      mhdType: {
        type: 5,
        name: 'MHD患者的血清前白蛋白定时检验完成率',
        title: 'MHD患者的血清前白蛋白定时检验完成详情',
        chartTitle: 'MHD患者的血清前白蛋白定时检验合格率',
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
            title: '完成率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'QualityControl/PA/Data',
        chartApi: 'QualityControl/PA/Chart'
      },
      // MHD患者的C反映蛋白（CRP）定时检验完成率
      mhd2Type: {
        type: 6,
        name: 'MHD患者的C反映蛋白（CRP）定时检验完成率',
        title: 'MHD患者的C反映蛋白（CRP）定时检验完成详情',
        chartTitle: 'MHD患者的C反映蛋白（CRP）定时检验合格率',
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
            title: '完成率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'QualityControl/CRP/Data',
        chartApi: 'QualityControl/CRP/Chart'
      },
      // MHD患者的血清铁蛋白、转铁蛋白饱和度定时检验完成率
      mhd3Type: {
        type: 7,
        name: 'MHD患者的血清铁蛋白、转铁蛋白饱和度定时检验完成率',
        title: 'MHD患者的血清铁蛋白、转铁蛋白饱和度定时检验完成详情',
        chartTitle: 'MHD患者的血清铁蛋白、转铁蛋白饱和度定时检验合格率',
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
            title: '完成率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'QualityControl/Ferritin/Data',
        chartApi: 'QualityControl/Ferritin/Chart'
      },
      // B2微球蛋白定时检验完成率
      mhd4Type: {
        type: 8,
        name: 'MHD患者的B2微球蛋白定时检验完成率',
        title: 'MHD患者的B2微球蛋白定时检验完成详情',
        chartTitle: 'MHD患者的B2微球蛋白定时检验合格率',
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
            title: '完成率',
            key: 'percentOfPass'
          }
        ],
        tableApi: 'QualityControl/B2/Data',
        chartApi: 'QualityControl/B2/Chart'
      },

      YEAR_RANGE: new Array(5)
        .fill(new Date().getFullYear())
        .map((item, index) => new Date().getFullYear() - index),

      buttonRole: BUTTONROLE
    }
  },
  created () {
    this.statisticType.push(
      this.xcgType,
      this.xyshType,
      this.qdjzxType,
      this.nsqczsType,
      this.mhdType,
      this.mhd2Type,
      this.mhd3Type,
      this.mhd4Type
    )
  },
  mounted () {
    // this.$nextTick(() => {
    // })
  },
  mixins: [chartSetting],
  methods: {
    changeHospital (id, list) {
      this.hospitalCheckedId = id
      this.operatechangeType(this.statisticTypeModel, false)

      list.length && (this.hospitalList = list)
      this.hospitalCheckedName = this.hospitalList.filter(
        v => v.dialysisId === id
      )[0].dialysisName
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
        // eslint-disable-next-line no-unused-vars
        name,
        column,
        tableApi,
        chartApi,
        chartTitle
      } = this.statisticType.filter(item => item.type === _type)[0]
      this.hospitalCheckedId === '0' &&
        (column = [...[organizationName], ...column])
      // console.log('_type-----------------:',_type)
      // console.dir(column)
      this.operateTitle = title
      this.chartTitle = chartTitle
      this.table_api = tableApi
      this.table_column = column
      this.table_data = []
      this.secondStateModel = ''
      this.startPage = 1
      this.legendTitle = {}

      this.table_api && this.getTableList(this.startPage)
      // if(flage) {
      //   this.dateModel = ''
      //   this.setChart(_type, chartApi);
      // }else {
      //   this.dateModel ? '' : this.setChart(_type, chartApi)
      // }
      this.setChart(_type, chartApi, this.dateModel)
    },

    getTableList (i) {
      i = i || 1
      let args = {
        centerId: this.hospitalCheckedId,
        beginTime: this.dateModel,
        pageSize: this.pageSize,
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
      this.beginTime = date
      let type = this.statisticTypeModel
      let { chartApi } = this.statisticType.filter(
        item => item.type === type
      )[0]
      this.startPage = 1
      this.getTableList(this.startPage)
      this.setChart(type, chartApi, date)
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
