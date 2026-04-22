<template>
  <div id="control">
    <parent-view2 @on-change="changeHospital" ref="tree">
      <template slot-scope="slotProps">{{`${slotProps.dialysis.dialysisName}`}}</template>
      <div slot="content">
        <div class="chart" ref="chart">
          <div class="btn-group">
            <div class="left">
              <Select
                class
                v-model="statisticTypeModel"
                @on-change="changeChartType"
                style="width:160px;margin-right:10px;"
              >
                <Option
                  :value="item.type"
                  v-for="item in statisticType"
                  :key="item.type"
                >按{{item.name}}分类</Option>
              </Select>
              <Select
                v-model="secondStateModel"
                placeholder="请选择监测类型"
                @on-change="secondStateChange"
                style="width: 160px"
              >
                <Option
                  :value="item.id"
                  v-for="item in secondBtnType.detail"
                  :key="item.id"
                >{{item.txt}}</Option>
              </Select>
            </div>
            <div class="right">
              <!-- <DatePicker
                v-model="dateModel"
                type="month"
                format="yyyy-MM"
                style="width: 160px;margin-right: 10px"
                placeholder="请选择月份"
                @on-change="changeDate"
              ></DatePicker> -->

              <DatePicker
                v-model="dateRangeModel"
                type="daterange"
                format="yyyy-MM"
                style="width: 160px;margin-right: 10px"
                placeholder="请选择时间段"
                @on-change="changeDate"
              ></DatePicker>
              <Button
                shape="circle"
                v-permission="buttonRole.YGKZ_DC"
                icon="md-cloud-upload"
                @click="download"
              >导出</Button>
            </div>
          </div>
          <div ref="dom" style="height: 100%;"></div>
        </div>
        <div class="operate-box">
          <h3 class="operate-box-title">{{operateTitle}}</h3>
          <!-- <div class="operate">
            <Form :label-width="60">
              <div class="btn-groups options">
                <FormItem
                  label="筛选："
                  style="margin-bottom: 0;"
                  v-if="secondBtnType.type === 'radio'"
                >
                  <RadioGroup
                    v-model="secondStateModel"
                    @on-change="secondStateChange"
                    type="button"
                  >
                    <Radio
                      :label="item.id"
                      :key="item.id"
                      v-for="item in secondBtnType.detail"
                    >{{item.txt}}</Radio>
                  </RadioGroup>
                </FormItem>
                <FormItem label="筛选：" style="margin-bottom: 0;" v-else>
                  <Select
                    v-model="secondStateModel"
                    placeholder="请选择监测类型"
                    @on-change="secondStateChange"
                    style="width: 160px"
                  >
                    <Option
                      :value="item.id"
                      v-for="item in secondBtnType.detail"
                      :key="item.id"
                    >{{item.txt}}</Option>
                  </Select>
                </FormItem>
              </div>
            </Form>
          </div> -->
          <div class="table">
            <Table :loading="loading" ref="table" :columns="table_column" :data="table_data" no-data-text="无数据"></Table>
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
/* eslint-disable camelcase */
import parentView2 from '@/components/parent-view/parent-view2.vue'
import chartSetting from './chartSetting.js'
const BUTTONROLE = {
  YGKZ_DC: 'YGKZ_DC'
}
export default {
  data () {
    let date = new Date()
    let currentMonth = `${date.getFullYear()}-${date.getMonth() + 1}`

    return {
      buttonRole: BUTTONROLE,
      statisticTypeModel: 1,
      chartTitle: '',
      operateTitle: '',
      statisticType: [],

      dateRangeModel: [currentMonth, currentMonth],
      dateModel: '',

      loading: false,
      table_data: [{}],
      table_column: [],
      table_api: '',
      dataCount: 0,
      pageSize: 10,
      startPage: 1,
      beginTime: currentMonth,
      endTime: currentMonth,

      secondBtnType: [],
      secondStateModel: 0, // 查询参数 -- 是否合格
      hospitalCheckedId: '',
      hospitalCheckedName: '',
      hospitalList: [],
      // 消毒类型
      xdType: {
        type: 1,
        name: '治疗室消毒合格率统计',
        title: '治疗室消毒合格率明细',
        column: [
          {
            title: '治疗室',
            key: 'partitionName'
          },
          {
            title: '消毒时间',
            key: 'disinfectionTime'
          },
          {
            title: '是否合格',
            key: 'isQualified',
            align: 'center',
            render: (h, params) => {
              let text = params.row.isQualified
              if (text === '合格') {
                return <span class="success">{text}</span>
              } else {
                return <span class="error">{text}</span>
              }
            }
          }
        ],
        btnType: {
          type: 'select',
          detail: [
            {
              id: 2,
              txt: '全部'
            },
            {
              id: 1,
              txt: '合格'
            },
            {
              id: 0,
              txt: '不合格'
            }
          ]
        },
        tableApi: 'HospitalFeeling/DisinfectionRoom/List',
        chartApi: 'HospitalFeeling/DisinfectionRoomStatistics/List'
      },
      // 新入患者传染病发病率统计
      xrhzType: {
        type: 2,
        name: '新入患者传染病发病率统计',
        title: '传染病发病率明细',
        column: [
          {
            title: '姓名',
            key: 'patentName'
          },
          {
            title: '性别',
            key: 'patentSex'
          },
          {
            title: '治疗编号',
            key: 'cureCode'
          },
          {
            title: '发病时间',
            key: 'morbidityTime'
          },
          {
            title: '传染病类型',
            key: 'infectiousTypeName'
          }
        ],
        btnType: {
          type: 'select',
          detail: [
            {
              id: 1,
              txt: '全部'
            },
            {
              id: 2,
              txt: '乙型肝炎'
            },
            {
              id: 3,
              txt: '丙型肝炎'
            }
          ]
        },
        tableApi: 'HospitalFeeling/GetPatientInfectiousCheck/List',
        chartApi:
          'HospitalFeeling/PatientInfectiousChecksStatisticsByCenter/List'
      },
      // 新入患者传染病监测完成率统计
      jcwclType: {
        type: 3,
        name: '新入患者传染病监测完成率统计',
        title: '传染病监测合格率明细',
        column: [
          {
            title: '姓名',
            key: 'patentName'
          },
          {
            title: '性别',
            key: 'patentSex'
          },
          {
            title: '治疗编号',
            key: 'cureCode'
          },
          {
            title: '传染病类型',
            key: 'infectiousTypeName'
          },
          {
            title: '监测时间',
            key: 'disinfectionTime'
          },
          {
            title: '监测是否完成',
            key: 'isQualified',
            align: 'center',
            render: (h, params) => {
              let text = params.row.isQualified
              if (text === '完成') {
                return <span class="success">{text}</span>
              } else {
                return <span class="error">{text}</span>
              }
            }
          }
        ],
        btnType: {
          type: 'select',
          detail: [
            {
              id: 2,
              txt: '全部'
            },
            {
              id: 1,
              txt: '完成'
            },
            {
              id: 0,
              txt: '未完成'
            }
          ]
        },
        tableApi: 'HospitalFeeling/InfectiousDiseasesRecordCheck/List',
        chartApi:
          'HospitalFeeling/GetInfectiousDiseasesRecordChartCheckQueryable/List'
      },
      // 标本菌落数检验合格率统计
      btjlsType: {
        type: 4,
        name: '标本菌落数检验合格率统计',
        title: '标本菌落数检验合格率明细',
        column: [
          {
            title: '标本',
            key: 'specimen',
            width: 120
          },
          {
            title: '菌落数监测',
            key: 'waterColonyCount',
            renderHeader: (h, params) => {
              let styles = {
                fontSize: '12px',
                color: 'red'
              }
              return (
                <div>
                  菌落数监测
                <span style={styles}>(标准&lt;=100CFU/ML)</span>
                </div>
              )
            }
          },
          {
            title: '监测时间',
            key: 'disinfectionTime',
            width: 150
          },
          {
            title: '是否合格',
            key: 'isQualified',
            align: 'center',
            width: 100,
            render: (h, params) => {
              let text = params.row.isQualified
              if (text === '合格') {
                return <span class="success">{text}</span>
              } else {
                return <span class="error">{text}</span>
              }
            }
          },
          {
            title: '操作',
            key: 'address',
            width: 140
          }
        ],
        btnType: {
          type: 'select',
          detail: [
            {
              id: 1,
              txt: '全部'
            },
            {
              id: 2,
              txt: '合格'
            },
            {
              id: 3,
              txt: '不合格'
            }
          ]
        },
        tableApi: 'HospitalFeeling/GetInspectionWaterPollutionQueryableTable/List/1',
        chartApi: 'HospitalFeeling/GetInspectionWaterPollutionsStatisticsChart/List/1'
      },
      // 内毒素检验合格率统计
      ndsjyType: {
        type: 5,
        name: '内毒素检验合格率统计',
        title: '内毒素检验合格率明细',
        column: [
          {
            title: '标本',
            key: 'specimen',
            width: 120
          },
          {
            title: '内毒素监测',
            key: 'endotoxin',
            renderHeader: (h, params) => {
              let styles = {
                fontSize: '12px',
                color: 'red'
              }
              return (
                <div>
                  内毒素监测
                <span style={styles}>(标准&lt;=0.25EU/ML)</span>
                </div>
              )
            }
          },
          {
            title: '监测时间',
            key: 'disinfectionTime',
            width: 150
          },
          {
            title: '是否合格',
            key: 'isQualified',
            align: 'center',
            width: 100,
            render: (h, params) => {
              let text = params.row.isQualified
              if (text === '合格') {
                return <span class="success">{text}</span>
              } else {
                return <span class="error">{text}</span>
              }
            }
          },
          {
            title: '操作',
            key: 'address',
            width: 140
          }
        ],
        btnType: {
          type: 'select',
          detail: [
            {
              id: 1,
              txt: '全部'
            },
            {
              id: 2,
              txt: '合格'
            },
            {
              id: 3,
              txt: '不合格'
            }
          ]
        },
        tableApi: 'HospitalFeeling/GetInspectionWaterPollutionQueryableTable/List/2',
        chartApi: 'HospitalFeeling/GetInspectionWaterPollutionsStatisticsChart/List/2'
      }
    }
  },
  created () {
    this.statisticType.push(
      this.xdType,
      this.xrhzType,
      this.jcwclType,
      this.btjlsType,
      this.ndsjyType
    )
  },
  mounted () {
    this.$nextTick(_ => {
    })
  },
  mixins: [chartSetting],
  methods: {
    changeHospital (id, list) {
      this.hospitalCheckedId = id
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
        btnType,
        tableApi,
        chartApi
      } = this.statisticType.filter(item => item.type === _type)[0]
      this.hospitalCheckedId === '0' &&
        (column = [...[organizationName], ...column])
      this.operateTitle = title
      this.chartTitle = name
      this.secondBtnType = btnType
      this.table_api = tableApi
      this.table_column = column
      this.table_data = []
      this.secondStateModel = ''
      this.startPage = 1

      this.table_api && this.getTableList(this.startPage)
      if (flage) {
        this.dateModel = ''
        this.setChart(_type, chartApi)
      } else {
        // eslint-disable-next-line no-unused-expressions
        this.dateModel ? '' : this.setChart(_type, chartApi)
      }
    },
    getTableList (i) {
      i = i || 1
      let args = {
        centerId: this.hospitalCheckedId,
        isQualified: this.secondStateModel,
        pageSize: this.pageSize,
        beginTime: this.beginTime,
        endTime: this.endTime,
        pageNum: i
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
      if (typeof date === 'object') {
        [this.beginTime, this.endTime] = date
        this.startPage = 1

        this.getTableList(this.startPage)

        let type = this.statisticTypeModel
        let { chartApi } = this.statisticType.filter(
          item => item.type === type
        )[0]
        this.setChart(type, chartApi)
      }
      // else {
      //   let type = this.statisticTypeModel
      //   this.dateModel = date
      //   let { chartApi, monthApi } = this.statisticType.filter(
      //     item => item.type === type
      //   )[0]
      //   if (date) {
      //     this.setChart(type, monthApi, true, date)
      //   } else {
      //     this.setChart(type, chartApi)
      //   }
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
    height: 486px;
    padding: 50px 20px 20px;
    background: #ffffff;
    position: relative;
    .btn-group {
      position: absolute;
      top: 20px;
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
    padding-bottom: 10px;
    background: #ffffff;
    &-title {
      margin-bottom: 10px;
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
