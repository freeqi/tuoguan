<template>
  <div id="patient">
    <parent-view2 @on-change="changeHospital" ref="tree">
      <template
        slot-scope="slotProps"
      >{{`${slotProps.index !== 0 ? `${slotProps.index}.` : ''}${slotProps.dialysis.dialysisName}(${slotProps.dialysis.patientCount})`}}</template>
      <div slot="content">
        <div class="chart" ref="chart">
          <div class="btn-group">
            <Select
              v-model="statisticTypeModel"
              @on-change="setChart"
              style="width:200px; margin-left: 10px;"
            >
              <Option
                :value="item.type"
                v-for="item in statisticType"
                :key="item.type"
              >按{{item.name}}分类</Option>
            </Select>
            <Select
              v-if="statisticTypeModel==3"
              v-model="statisticYearTypeModel"
              @on-change="setChartByYear"
              style="width:120px; margin-left: 10px;"
            >
              <Option
                :value="item.type"
                v-for="item in statisticYearType"
                :key="item.type"
              >{{item.value}}</Option>
            </Select>
            <Button
              shape="circle"
              icon="md-cloud-upload"
              v-permission="buttonRole.HZXX_DC"
              style="margin-left: 10px;"
              @click="download"
            >导出</Button>
          </div>
          <div ref="dom" style="height: 100%;"></div>
        </div>
        <div class="patients-operate">
          <div class="operate">
            <div class="operate-search">
              <div>
                <div class="inline-block">
                  <Form :label-width="80">
                    <FormItem label="在院状态" style="margin-bottom: 0;">
                      <Select
                        v-model="hospitalState"
                        @on-change="selectHospitalState"
                        placeholder="请选择患者在院状态"
                        style="width: 200px"
                      >
                        <Option
                          :value="item.id"
                          v-for="item in hospitalStateType"
                          :key="item.id"
                        >{{item.name}}</Option>
                      </Select>
                    </FormItem>
                  </Form>
                </div>
                <div class="inline-block">
                  <Input
                    v-model="searchKey"
                    search
                    enter-button
                    style="width: 220px"
                    @on-search="searchPatient"
                    placeholder="请输入关键字"
                  />
                </div>
              </div>
            </div>
          </div>
          <swsTable
            class="sws-table"
            style="margin-top: 20px;"
            ref="table"
            size="default"
            :loading="loading"
            :columns="table_column"
            :data="patient_data"
            :pagination="pagination"
            @on-change="changePage"
          />
        </div>

        <transition name="fade">
          <patient-detail id="patient-detail" :patient="patientDetail" ref="patientDetail"></patient-detail>
        </transition>
      </div>
    </parent-view2>
  </div>
</template>

<script>
import parentView2 from '@/components/parent-view/parent-view2.vue'
import Operate from '@/components/operate'
import swsTable from '_c/sws-table/'
import { mapState } from 'vuex'
import chartSetting from './chartSetting.js'
import patientDetail from './patient-detail.vue'
const TYPEID = '396ee46a6c6b486a9d4a790b7be22577'
const BUTTONROLE = {
  HZXX_DC: 'HZXX_DC'
}
export default {
  name: 'pat_info',
  data () {
    return {
      // 图表
      statisticTypeModel: 0,
      statisticType: [
        {
          type: 0,
          name: '年龄段'
        },
        {
          type: 1,
          name: '性别'
        },
        {
          type: 2,
          name: '血源性疾病'
        },
        {
          type: 3,
          name: '患者增长情况'
        },
        {
          type: 4,
          name: '医保'
        },
        {
          type: 5,
          name: '在院状态'
        }
      ],
      // 患者增长情况按年计算
      statisticYearTypeModel: 1,
      statisticYearType: [
        {
          type: 1,
          value: '近1年'
        },
        {
          type: 2,
          value: '近2年'
        },
        {
          type: 3,
          value: '近3年'
        }
      ],
      // 透析机构列表
      hospitalCheckedId: '1',
      hospitalCheckedName: '',
      hospitalList: [],
      loading: false,
      // 搜索
      searchKey: '',

      // 表格
      table_column: [
        {
          title: '姓名',
          key: 'name',
          render: (h, params) => {
            return h('div', [
              h('Icon', {
                props: {
                  type: 'person'
                }
              }),
              h('strong', params.row.name)
            ])
          }
        },
        {
          title: '性别',
          key: 'sex'
        },
        {
          title: '年龄',
          key: 'age'
        },
        {
          title: '治疗编号',
          key: 'patientNo'
        },
        {
          title: '病人状态',
          key: 'sHospitalState',
          align: 'center',
          render: (h, params) => {
            const { sHospitalState } = params.row
            let color = ''
            switch (sHospitalState) {
              case '转入':
                color = '#1890ff'
                break
              case '转出':
                color = '#ff9901'
                break
              case '离院':
                color = 'red'
                break
              case '死亡':
                color = '#999'
                break
            }
            return <span style={{ color }}>{sHospitalState}</span>
          }
        },
        {
          title: '接诊医生',
          key: 'receiveDoctor'
        },
        {
          title: '接诊日期',
          key: 'receiveDate'
        },
        {
          title: '医保类型',
          key: 'ssiInsuredType',
          align: 'center'
        },
        {
          title: '联系电话',
          key: 'contactPhone'
        },
        {
          title: '操作',
          key: 'action',
          width: 45,
          align: 'center',
          render: (h, params) => {
            return (
              <Operate
                showEdit={false}
                showDelete={false}
                handleWatch={() => {
                  let patientDetail = this.patient_data.filter(v => {
                    return v.id === params.row.id
                  })[0]

                  this.patientDetail = patientDetail
                  this.$refs.patientDetail.show()
                }}
              />
            )
          }
        }
      ],
      patientDetail: {},
      patient_data: [],
      pageSize: 9,
      patientDataCount: 0,
      startPage: 1,

      // a标签
      hospitalState: '0', // 患者在院状态
      hospitalStateType: [],
      buttonRole: BUTTONROLE
    }
  },
  created () {
    this.$nextTick(() => {
      this.swsApi
        .swsPost('SystemDictionary/DictionaryList', { typeId: TYPEID })
        .then(res => {
          let data = res.data
          if (data.success) {
            this.hospitalStateType = data.result
          }
        })
    })
  },
  computed: {
    chartTitle () {
      let name = null
      if (this.statisticTypeModel >= 0 && this.statisticTypeModel !== 3) {
        // 按性别，性别，血源性疾病，年龄段
        name = this.statisticType[this.statisticTypeModel].name
      } else if (this.statisticTypeModel === 3) {
        // 按患者增长情况
        name = '患者增长情况'
        return `${this.hospitalCheckedName}${name}比例统计`
      } else return false

      return `${this.hospitalCheckedName}患者${name}比例统计`
    },
    chartApi () {
      let api = null
      if (this.statisticTypeModel === 0) {
        // 按性别
        api = 'Patients/GetPatientsByAgeList/' + this.hospitalCheckedId
      } else if (this.statisticTypeModel === 1) {
        // 按职位
        api = 'Patients/GetPatientsBySexList/' + this.hospitalCheckedId
      } else if (this.statisticTypeModel === 2) {
        // 按职称
        api = 'Patients/GetPatientsByBloodBorneList/' + this.hospitalCheckedId
      } else if (this.statisticTypeModel === 3) {
        // 按患者增长情况
        api =
          'Patients/GetPatientsByDateList/' +
          `${this.hospitalCheckedId}/${this.statisticYearTypeModel}`
      } else if (this.statisticTypeModel === 4) {
        // 按医保
        api =
          'Patients/GetPatientsBySIInsuredTypeList/' + this.hospitalCheckedId
      } else if (this.statisticTypeModel === 5) {
        // 按医保
        api = 'Patients/GetPatientByHospitalState/' + this.hospitalCheckedId
      } else return false

      return api
    },
    menuCollapsed () {
      return this.collasped
    },
    pagination () {
      return { total: this.patientDataCount, current: this.startPage, pageSize: this.pageSize }
    },
    ...mapState({
      collasped: state => state.app.menuCollapsed
    })
  },
  watch: {
    menuCollapsed () {
      setTimeout(() => {
        this.dom.resize()
      }, 160)
    }
  },
  mixins: [chartSetting],
  methods: {
    // 切换医院
    changeHospital (id, list) {
      this.searchKey = ''
      list.length && (this.hospitalList = list)
      this.hospitalCheckedId = id
      this.hospitalCheckedName = this.hospitalList.filter(
        v => v.dialysisId === id
      )[0].dialysisName
      this.startPage = 1
      // 设置图表
      // this.setPieChart(this.chartApi)
      this.setChart()
      this.getPatientList(this.startPage)
      sessionStorage.setItem('hospitalCheckedId', id)
    },
    // 分页
    changePage (index) {
      this.startPage = index
      this.getPatientList(index)
    },
    // 获取患者列表
    getPatientList (i) {
      i = i || 1
      let args = {
        name: this.searchKey,
        centerId: this.hospitalCheckedId,
        pageSize: this.pageSize,
        hospitalStateId: this.hospitalState,
        pageNum: i
      }
      this.loading = true
      this.swsApi
        .swsPost('Patients/Patients', args)
        .then(res => {
          this.loading = false
          if (res.data.result) {
            this.patient_data = res.data.result
            this.patientDataCount = res.data.dataCount
          } else {
            this.$Notice.error({
              title: '请求错误',
              desc: '网络错误，请稍后再试'
            })
          }
        })
        .catch(e => {
          this.loading = false
        })
    },
    searchPatient () {
      this.startPage = 1
      this.getPatientList(this.startPage)
    },
    // 下载图表
    download () {
      let args = {
        centerId: this.hospitalCheckedId,
        pageSize: 10000,
        name: '',
        hospitalStateId: '',
        pageNum: 1
      }
      this.swsApi
        .swsPost('Patients/Patients', args)
        .then(res => {
          if (res.data.result) {
            let data = res.data.result.map(item => {
              item.contactPhone += '\t'
              item.receiveDate += '\t'
              return item
            })
            this.$refs.table.exportCsv({
              filename: `${this.hospitalCheckedName}患者统计表`,
              columns: this.table_column,
              data
            })
          } else {
            this.$Message.error('网络错误，请稍后再试')
          }
        })
    },
    //
    setChart () {
      // 设置图表
      if (this.statisticTypeModel === 3) {
        this.statisticYearTypeModel = 1
      }
      this.setPieChart(this.chartApi)
    },
    // 患者增长情况按年计算
    setChartByYear () {
      let url = this.chartApi
      this.setPieChart(url)
    },
    selectHospitalState () {
      this.startPage = 1
      this.getPatientList(this.startPage)
    }
  },
  components: {
    parentView2,
    patientDetail,
    Operate,
    swsTable
  }
}
</script>
<style lang="less" scoped>
#patient {
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
      right: 40px;
      z-index: 10;
      /deep/ .ivu-btn-primary {
        background: #4f95e8;
        border-color: #4f95e8;
        box-shadow: 2px 2px 6px rgba(79, 149, 232, 0.35);
      }
    }
  }
  .patients-operate {
    margin-top: 20px;
    padding: 20px 0;
    background: #ffffff;
    .operate {
      padding: 0 20px;
      .operate-search .ivu-row > * {
        margin-top: 10px;
      }
    }
    .organization-list {
      margin-top: 20px;
    }
    .ivu-table-wrapper {
      & /deep/ .ivu-table .ivu-table-cell {
        padding-right: 5px;
        padding-left: 5px;
      }
    }
  }
}

#patient-detail {
  position: absolute;
  top: 0;
  right: 0;
  width: 500px;
  height: 100%;
  z-index: 10;
}
.inline-block {
  vertical-align: middle;
  & + .inline-block {
    margin-left: 10px;
  }
}
</style>
