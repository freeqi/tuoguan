/* eslint-disable eqeqeq */
<template>
  <detail-info ref="detail" :title="title" :class="longW ? 'longW' : ''">
    <Tabs :value="currentTabPanel" ref="tab" class="tabs" @on-click="changeTab()">
      <TabPane label="基本信息" name="tab1">
        <div class="info-box">
          <div class="main-info"><div><Icon type="ios-build-outline" size="24"/><span>设备信息</span></div></div>
          <div class="detail">
            <p>
              <span class="field"><label>设备名称：</label>{{equipment.name}}</span>
            </p>
            <p>
              <span class="field"><label>设备类型：</label>{{equipment.equipType}}</span>
            </p>
            <p>
              <span class="field"><label>机器型号：</label>{{equipment.model}}</span>
              <span class="field"><label>设备分区：</label>{{equipment.treatmentRegion}}</span>
            </p>
            <p>
              <span class="field"><label>设备编号：</label>{{equipment.serialNumber}}</span>
              <span class="field"><label>血源性疾病：</label>{{equipment.bloodBorneDisease}}</span>
            </p>
            <p>
              <span class="field"><label>IP地址：</label>{{equipment.ipAddress}}</span>
              <span class="field"><label>床位号：</label>{{equipment.bedNo}}</span>
            </p>
            <p>
              <span class="field"><label>治疗模式：</label>{{equipment.treatmentModelsK | toString }}</span>
            </p>
          </div>
          <img v-if="equipment.dataState == 1" src="@/assets/images/equipment_normal.png" class="status" alt="" srcset="">
          <img v-else-if="equipment.dataState == 2" src="@/assets/images/equipment_fix.png" class="status" alt="" srcset="">
          <img v-else src="@/assets/images/equipment_down.png" class="status" alt="" srcset="">
        </div>
        <div class="info-box">
          <div class="main-info"><div><Icon type="ios-information-circle-outline" size="23" /><span>其他信息</span></div></div>
          <div class="detail">
            <p>
              <span class="field"><label>购买日期：</label>{{equipment.purchaseDate}}</span>
              <span class="field"><label>采购价：</label><span class="special">{{equipment.purchaseMoney | formatNumber}}元</span></span>
            </p>
            <p>
              <span class="field"><label>供货商：</label>{{equipment.supplier}}</span>
            </p>
            <p>
              <span class="field"><label>供货商电话：</label>{{equipment.supplierTelphone}}</span>
            </p>
            <p>
              <span class="field"><label>生厂商：</label>{{equipment.producer}}</span>
            </p>
            <p>
              <span class="field"><label>生厂商电话：</label>{{equipment.producerTelphone}}</span>
            </p>
            <p>
              <span class="field"><label>售后工程师：</label>{{equipment.engineerName}}</span>
              <span class="field"><label>工程师电话：</label>{{equipment.engineerPhone}}</span>
            </p>
            <p>
              <span class="field"><label>生产日期：</label>{{equipment.produceDate}}</span>
              <span class="field"><label>保质期：</label>{{equipment.maintenanceDate}}</span>
            </p>
          </div>
        </div>
      </TabPane>
      <TabPane label="维修记录" name="require">
        <div class="info-box">
          <div class="main-info"><div><Icon type="ios-build-outline" size="24"/><span>维修记录</span></div></div>
          <div class="detail timeLine" v-if="!requireLoading">
            <Timeline v-if="requireData.length">
              <TimelineItem v-for="item in requireData" :key="item.id">
                <i slot="dot"></i>
                <div class="left">
                  <p class="t-status"
                    :class="(item.equipmentState==1||item.equipmentState ==2)? 'color-info' : 'color-error'">
                    {{item.equipmentState | eqState}}
                  </p>
                  <p class="time">{{item.maintenanceDate}}</p>
                </div>
                <div class="content">
                  <p class="people">申请人：{{item.applicant}}</p>
                  <p class="list">{{item.maintenanceReason}}</p>
                  <p class="people">维修人：{{item.maintenancePerson}}</p>
                  <p class="money">维修费用：{{item.maintenanceCost}}元</p>
                  <p class="solve">解决方案：{{item.solution}}</p>
                </div>
              </TimelineItem>
            </Timeline>
            <div class="empty-text" v-else>暂无数据</div>
          </div>
          <div class="demo-spin-container" v-else>
            <Spin fix></Spin>
          </div>
        </div>
      </TabPane>
      <TabPane label="维保记录" name="maintenance">
        <div class="info-box">
          <div class="main-info"><div><Icon type="ios-build-outline" size="24"/><span>维保记录</span></div></div>
          <div class="detail timeLine"  v-if="!MLoading">
            <Timeline  v-if="MData.length">
              <TimelineItem v-for="item in MData" :key="item.id">
                <i slot="dot"></i>
                <div class="left">
                  <p class="time">{{item.maintenanceDate}}</p>
                </div>
                <div class="content">
                  <p class="people color-info">维保人：{{item.maintenancePerson}}</p>
                  <p>维保内容：</p>
                  <p class="list">{{item.maintenanceContent}}</p>
                  <p>备注：</p>
                  <p class="list">{{item.remark}}</p>
                </div>
              </TimelineItem>
            </Timeline>
            <div class="empty-text" v-else>暂无数据</div>
          </div>
          <div class="demo-spin-container" v-else>
            <Spin fix></Spin>
          </div>
        </div>
      </TabPane>
      <TabPane label="生化检测" name="check">
        <div class="info-box">
          <div class="main-info"><div><Icon type="ios-flask-outline" size="24"/><span>生化检测</span></div></div>
          <div class="detail">
            <RadioGroup v-model="checkType" @on-change="changeT" style="margin-right: 15px;">
              <Radio v-for="item in dicBioDetection" :key="item.id" :label="item.value">{{item.name}}</Radio>
            </RadioGroup>
            <div class="selectBox">
              <span style="display:inline-block;">检测类别：</span>
              <Select v-model="selectBioType" size="small" style="width:120px;" @on-change="changeBioType">
                <Option v-for="item in detectionTypeLists[comptIndex]" :key="item.index" :value="item.index">{{item.name}}</Option>
              </Select>
            </div>
          </div>
        </div>
        <Table height="600" :loading="tloading" :columns="table_column" :data="checkData"></Table>
      </TabPane>
    </Tabs>
    <!-- 数据请求完成前显示加载 -->
    <!-- <spin size="large" fix v-if="loading"></spin> -->
  </detail-info>
</template>

<script>
import {formateDateToString} from '@/libs/tools'
import detailInfo from '@/components/detail-info/'
const [BIODETECTION] = ['2db5af576d3b4417b6b024c4909f3e98']
export default {
  data () {
    return {
      // 数据字典
      dicBioDetection: [],

      loading: false,
      tloading: false,
      title: '设备详情',
      longW: false,
      currentTabPanel: 'tab1',
      checkType: '2',
      selectBioType: 1, // 选中的生化检测对应的类别
      // cy 生化检测类别数组 (数组顺序对应生化检测的数据字典value顺序：
      // 透析液1，反渗水后端2，反渗水前端3，透析相关用水4, 污水5，置换液6)
      detectionTypeLists: [
        [{index: 1, name: '电解质'}, {index: 2, name: '电导'}, {index: 3, name: '内毒素'}, {index: 4, name: '细菌'}],
        [{index: 1, name: '细菌培养'}, {index: 2, name: '内毒素'}, {index: 3, name: '化学污染物'}],
        [{index: 1, name: '细菌培养'}, {index: 2, name: '内毒素'}],
        [{index: 1, name: '余氯'}, {index: 2, name: 'PH值'}, {index: 3, name: '硬度'}],
        [{index: 4, name: '粪大肠菌群素'}, {index: 1, name: '余氯'}, {index: 2, name: 'PH值'}, {index: 3, name: '化学需氧量'}],
        [{index: 1, name: '电解质'}, {index: 2, name: '内毒素'}, {index: 3, name: '细菌'}]
      ],
      table_column: [],
      table_common_column: [
        {
          title: `类别`,
          key: 'testType',
          width: 100
        },
        {
          title: `检测日期`,
          key: 'testDate',
          render: (h, params) => {
            return (
              <span>{formateDateToString(new Date(params.row.testDate), 'yyyy-MM-dd hh:mm')}</span>
            )
          }
        },
        {
          title: `检测结果`,
          key: 'values'
        },
        {
          title: `备注`,
          key: 'remark'
        }
      ],
      table_electrolyte_column: [
        {
          title: `检测日期`,
          key: 'testDate',
          width: 120,
          fixed: 'left',
          render: (h, params) => {
            return (
              <span>{formateDateToString(new Date(params.row.testDate), 'yyyy-MM-dd hh:mm')}</span>
            )
          }
        },
        {
          title: `类别`,
          key: 'testType',
          width: 100
        },
        {
          title: '钠(Na)',
          key: 'ionNa',
          align: 'center',
          width: 80
        },
        {
          title: '钾(K)',
          key: 'ionK',
          align: 'center',
          width: 80
        },
        {
          title: '钙(Ca)',
          key: 'ionGa',
          align: 'center',
          width: 80
        },
        {
          title: '镁(Mg)',
          key: 'ionMg',
          align: 'center',
          width: 80
        },
        {
          title: '氯(Cl)',
          key: 'ionCl',
          align: 'center',
          width: 80
        },
        {
          title: '碳酸氢根(HCO3-)',
          key: 'ionHCO3',
          align: 'center',
          width: 115
        },
        {
          title: '备注',
          key: 'remark',
          align: 'center',
          width: 100
        }
      ],
      // 判断是否已获得维保或维修记录
      getEkrFlage: false,
      getMrFlage: false,
      getEbtFlage: false,
      requireLoading: true,
      MLoading: true,
      // 维修维保记录
      requireData: [],
      MData: [],
      checkData: [],
      comptIndex: 1
    }
  },
  props: {
    equipment: {
      require: true,
      default: () => {
        return {}
      }
    }
  },
  created () {
    this.table_column = this.table_common_column
    let types = [BIODETECTION]
    types = types.map(id => {
      let da = {
        url: 'SystemDictionary/DictionaryList',
        params: { typeId: `${id}` }
      }
      return da
    })
    this.swsApi.swsAllPost(types)
      .then(res => {
        this.dicBioDetection = res[0].data.result
      })
      .catch(e => {
        console.log(e)
      })
  },
  mounted () {
  },
  computed: {
    // 所选设备id
    equipmentCheckedId () {
      return this.equipment.id
    },
    typeName () {
      return this.detectionTypeLists[this.comptIndex].filter(res => { return res.index === this.selectBioType })[0].name
    }
  },
  watch: {
    checkType () {
      this.comptIndex = parseInt(this.checkType) - 1
      this.changeBioType()
    }
  },
  methods: {
    changeTab () {
      let tabName = this.$refs.tab.activeKey
      let api = ''
      this.longW = tabName === 'check'
      if (tabName === 'maintenance' && !this.getEkrFlage) {
        // 维保
        api = `Equipment/GetEkr/${this.equipmentCheckedId}`
        this.MLoading = true
        this.getDataMethod(api, (res) => {
          if (res.data.success) {
            this.getEkrFlage = true
            this.MLoading = false
            this.MData = res.data.result
          } else {
            this.MData = []
          }
        })
      } else if (tabName === 'require' && !this.getMrFlage) {
        // 维修
        api = `Equipment/GetMr/${this.equipmentCheckedId}`
        this.requireLoading = true
        this.getDataMethod(api, (res) => {
          if (res.data.success) {
            this.getMrFlage = true
            this.requireLoading = false
            this.requireData = res.data.result
          } else {
            this.requireData = []
          }
        })
      } else if (tabName === 'check' && !this.getEbtFlage) {
        this.changeT()
      } else {
        return false
      }
    },
    // 获取维修或维保记录
    getDataMethod (api, fn) {
      this.swsApi.swsGet(api)
        .then(fn)
        .catch(e => {
          this.loading = false
          console.log(e)
        })
    },
    show () {
      this.longW = false
      this.resetData()
      this.$refs.detail.show()
    },
    hide () {
      this.$refs.detail.hide()
      this.resetData()
    },
    resetData () {
      this.getMrFlage = false
      this.getEkrFlage = false
      this.MLoading = false
      this.requireLoading = false
      this.requireData = []
      this.MData = []
    },
    changeT () {
      this.selectBioType = 1
      this.getBioData()
    },
    getBioData () {
      let id = this.dicBioDetection.filter(v => {
        return v.value === this.checkType
      })[0].id
      let api = `Equipment/GetEbt/${this.equipmentCheckedId}/${id}/${this.typeName}`
      this.tloading = true
      this.getDataMethod(api, (res) => {
        if (res.data.success) {
          this.getEbtFlage = true
          this.tloading = false
          this.checkData = res.data.result
        } else {
          this.checkData = []
        }
      })
    },
    changeBioType () {
      if (this.checkType === '1' && this.selectBioType === 1) {
        this.table_column = this.table_electrolyte_column
      } else {
        this.table_column = this.table_common_column
      }
      this.getBioData()
    }
  },
  components: {
    detailInfo
  },
  filters: {
    formatNumber (value) {
      if (!value) return '0'
      return value.toLocaleString('en-US')
    },
    toString (value) {
      if (!value) return false
      return value.join('，')
    },
    eqState (value) {
      let _txt = ''
      if (value === 1) {
        _txt = '已正常'
      } else if (value === 2) {
        _txt = '维修中'
      } else if (value === 3) {
        _txt = '待处理'
      } else {
        _txt = '已报废'
      }
      return _txt
    }
  }
}
</script>

<style scoped lang="less">
#equipment-detail {
  width: 500px;
  &.longW{
    width: 654px;
  }
  & /deep/ .ivu-tabs-nav .ivu-tabs-tab-active {
    color: #4f95e8;
  }
  & /deep/ .ivu-tabs-bar {
    margin-bottom: 0;
  }
  .info-box {
    position: relative;
    padding: 10px;
    color: #333333;
    .main-info {
      margin-bottom: 6px;
      padding-left: 10px;
      display: flex;
      align-items: center;
      height: 36px;
      // justify-content: center;
      & > div {
        margin-right: 16px;
      }
      & i {
        margin-right: 4px;
      }
    }
    .detail {
      padding-left: 14px;
      .selectBox {
        display: inline-block;
        font-size: 12px;
      }
      &>p {
        display: flex;
        align-items: center;
        min-height: 28px;
        .field {
          margin-right: 24px;
          label {
            margin-right: 3px;
            color: #999;
          }
          .special {
            color: #fc4e4e;
          }
        }
      }
      .empty-text {
        line-height: 80px;
        padding-left: 70px;
      }
      &.timeLine {
        padding-left: 140px;
        i {
          display: inline-block;
          width: 8px;
          height: 8px;
          border-radius: 8px;
          background: #c5c5c5;
        }
        /deep/ .ivu-timeline-item-head-custom {
          padding: 0;
          line-height: 8px;
        }
        & /deep/ .ivu-timeline-item-content{
          padding-bottom: 20px;
        }
        .color-info {
          color: #5c9dea;
        }
        .color-error {
          color: #fc4b4b;
        }
        .left {
          text-align: right;
          position: absolute;
          left: -10px;
          transform: translateX(-100%);
          .time {
            margin-top: 2px;
          }
          & p:first-child {
            margin-top: 0
          }
        }
        .content {
          &>* {
            margin-bottom: 2px;
          }
          .list, .solve {
            color: #999;
          }
          .list + .people {
            margin-top: 10px;
          }
          .money {
            color: #fc4b4b;
          }
        }
      }
    }
    .status {
      position: absolute;
      top: 20px;
      right: 40px;
    }
  }
  // .tabs {
  //   display: flex;
  //   flex-direction: column;
  //   height: 100%;
  //   overflow-y: auto;
  //   /deep/ .ivu-tabs-content {
  //     flex: 1;
  //     .ivu-tabs-tabpane {
  //       overflow-y: auto;
  //       .ivu-table-wrapper {
  //         border: none !important;
  //         & /deep/ .ivu-table {
  //           &::before, &::after {
  //             display: none !important;
  //           }
  //         }
  //         & /deep/ .ivu-table th {
  //           font-size: 13px;
  //           background: #f2f8ff;
  //           border-bottom: none;
  //         }
  //       }
  //     }
  //   }
  // }
}
.demo-spin-container{
  margin-top: 100px
}
</style>
