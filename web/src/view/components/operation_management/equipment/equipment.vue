<template>
  <div id="equipment">
    <parent-view2 @on-change="changeHospital" ref="tree">
      <template
        slot-scope="slotProps"
      >{{`${slotProps.index !== 0 ? `${slotProps.index}.` : ''}${slotProps.dialysis.dialysisName}(${slotProps.dialysis.equipCount})`}}</template>
      <div slot="content">
        <div class="chart">
          <div class="btn-group">
            <span>统计类型</span>
            <Select
              v-model="statisticTypeModel"
              @on-change="setChart"
              placeholder="请选择"
              style="width:200px; margin-left: 10px;"
            >
              <Option
                :value="item.type"
                v-for="item in statisticType"
                :disabled=" item.type === 1 && hospitalCheckedId !== '0'"
                :key="item.type"
              >{{item.value}}</Option>
            </Select>
            <Button
              shape="circle"
              icon="md-cloud-upload"
              style="margin-left: 10px;"
              @click="download"
              v-permission="buttonRole.SBGL_DC"
            >导出</Button>
          </div>
          <div ref="chart" style="height: 100%"></div>
        </div>
        <div class="equipment-operate">
          <div class="operate">
            <div class="operate-search">
              <div style="margin-top: 10px;">
                <div class="inline-block">
                  <Form :label-width="80">
                    <FormItem label="设备类型" style="margin-bottom: 10px;">
                      <Select
                        v-model="selectMachineType"
                        @on-change="selectEquipmentT"
                        placeholder="请选择设备类型"
                        style="width: 200px"
                      >
                        <Option
                          :value="item.id"
                          v-for="item in equipmentDicList"
                          :key="item.id"
                        >{{item.name}}</Option>
                      </Select>
                    </FormItem>
                  </Form>
                </div>
                <div class="inline-block btn-groups">
                  <RadioGroup v-model="equipmentState" @on-change="selectEquipmentS" type="button">
                    <Radio :label="item.txt" :key="item.id" v-for="item in btnType"></Radio>
                  </RadioGroup>
                </div>
                <Input
                  v-model="searchKey"
                  @on-search="searchE"
                  search
                  enter-button
                  placeholder="请输入关键字"
                  style="width:240px;margin-left:20px;display:inline-table;"
                />
              </div>
            </div>
          </div>

          <swsTable
            class="sws-table"
            style="margin-top: 20px;"
            ref="table"
            :loading="loading"
            :columns="table_column"
            :data="equipment_data"
            :pagination="pagination"
            @on-change="changePage"
          />
        </div>
        <transition name="fade">
          <equipment-detail
            id="equipment-detail"
            :equipment="equipmentDetail"
            ref="equipmentDetail"
          ></equipment-detail>
        </transition>
      </div>
    </parent-view2>
  </div>
</template>

<script>
import parentView2 from '@/components/parent-view/parent-view2.vue'
import Operate from '@/components/operate'
import swsTable from '_c/sws-table/'
import chartSetting from './chartSetting.js'
import equipmentDetail from './equipment-detail.vue'
const BUTTONROLE = {
  SBGL_DC: 'SBGL_DC'
}
const equipmentStateArr = [
  {
    id: 0,
    txt: '全部'
  },
  {
    id: 1,
    txt: '正常使用'
  },
  {
    id: 2,
    txt: '维修'
  },
  {
    id: 3,
    txt: '报废'
  }
]
export default {
  data () {
    return {
      loading: false,
      // 透析机构列表
      hospitalCheckedId: '0',
      hospitalCheckedName: '',
      hospitalList: [],

      statisticTypeModel: 0,
      statisticType: [
        {
          value: '设备型号统计',
          type: 0
        },
        {
          value: '设备分布统计',
          type: 1
        },
        {
          value: '设备使用状态统计',
          type: 2
        },
        {
          value: '设备使用年限统计',
          type: 3
        }
      ],
      searchKey: '',
      selectMachineType: '0',

      btnType: [
        {
          id: 1,
          txt: '正常使用'
        },
        {
          id: 2,
          txt: '维修'
        },
        {
          id: 3,
          txt: '报废'
        }
      ],
      btnCheckedId: -1,
      equipmentState: '',
      eqState: equipmentStateArr[0].id,
      // 表格
      table_default: [
        {
          title: '设备名称',
          key: 'name',
          render: (h, params) => {
            return h('div', [h('strong', params.row.name)])
          }
        },
        {
          title: '设备型号',
          key: 'model',
          width: 120,
          render: (h, params) => {
            let txt = [0, 1].includes(params.row.validity)
              ? '数据正常' : [2, 3].includes(params.row.validity)
                ? '型号异常' : ''
            if (txt !== '') {
              return (
                <div>
                  <tooltip content={txt} placement='top'>
                    <p style={![0, 1].includes(params.row.validity) ? 'color:red;' : ''}>{params.row.model}</p>
                  </tooltip>
                </div>
              )
            } else return <p>{params.row.model}</p>
          }
        },
        {
          title: '设备状态',
          key: 'dataState',
          width: 120,
          render: (h, params) => {
            let _class = ''
            let _txt = ''
            if (params.row.dataState === 1) {
              _txt = '正常使用'
            } else if (params.row.dataState === 2) {
              _class = 'txt-fix'
              _txt = '已维修'
            } else {
              _class = 'txt-down'
              _txt = '已报废'
            }
            return h(
              'span',
              {
                class: _class
              },
              [_txt]
            )
          }
        },
        {
          title: '设备编号',
          key: 'serialNumber',
          render: (h, params) => {
            let txt = [0, 2].includes(params.row.validity)
              ? '数据正常' : [1, 3].includes(params.row.validity)
                ? '编码异常 ' : ''
            if (txt !== '') {
              return (
                <div>
                  <tooltip content={txt} placement='top'>
                    <p style={![0, 2].includes(params.row.validity) ? 'color:red;' : ''}>{params.row.serialNumber}</p>
                  </tooltip>
                </div>
              )
            } else return <p>{params.row.serialNumber}</p>
          }
        },
        {
          title: '购买日期',
          key: 'purchaseDate',
          width: 110
        },
        {
          title: '保质期',
          key: 'maintenanceDate',
          width: 110
        },
        {
          title: '操作',
          key: 'action',
          width: 90,
          align: 'center',
          render: (h, params) => {
            return (
              <Operate
                showEdit={false}
                showDelete={false}
                handleWatch={() => this.handleWatch(params.row)}
              />
            )
          }
        }
      ],
      center_key: {
        title: '机构',
        key: 'center'
      },
      table_column: [],

      equipmentDetail: {},
      equipment_data: [],
      pageSize: 10,
      equipmentDataCount: 0,
      startPage: 1,
      // 设备类型
      equipmentDicList: [],

      buttonRole: BUTTONROLE
    }
  },
  created () {
    // 获取机构列表
    this.$nextTick(() => {
      // 获取设备类型数据字典
      this.swsApi
        .swsPost('SystemDictionary/DictionaryList', {
          typeId: 'b2db1cf2ec674e059031f54d58e00969'
        })
        .then(res => {
          let data = res.data
          let all = {
            id: '0',
            name: '全部'
          }

          if (data.success) {
            this.equipmentDicList = [all, ...data.result]
          }
        })
    })
  },
  computed: {
    chartTitle () {
      let name = null
      name = this.statisticType[this.statisticTypeModel].value
      return `${this.hospitalCheckedName}${name}`
    },
    chartApi () {
      let api = null
      if (this.statisticTypeModel === 0) {
        // 设备型号统计
        api = 'Equipment/GetEquiByModelList/' + this.hospitalCheckedId
      } else if (this.statisticTypeModel === 1) {
        // 设备分布统计
        api = 'Equipment/GetEquiByModelDistribution'
      } else if (this.statisticTypeModel === 2) {
        // 设备使用状态统计
        api = 'Equipment/GetEquiBySeate/' + this.hospitalCheckedId
      } else if (this.statisticTypeModel === 3) {
        // 设备使用年限统计
        api = 'Equipment/GetEquiByuseYear/' + this.hospitalCheckedId
      } else return false

      return api
    },
    pagination () {
      return { total: this.equipmentDataCount, current: this.startPage, pageSize: this.pageSize }
    }
  },
  mounted () { },
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
      sessionStorage.setItem('hospitalCheckedId', id)

      if (id === '0') {
        this.table_column = [...this.table_default]
        this.table_column.splice(1, 0, this.center_key)
      } else {
        this.table_column = [...this.table_default]
      }

      // 重置筛选条件
      this.startPage = 1
      this.eqState = equipmentStateArr[0].id
      this.selectMachineType = '0'
      this.equipmentState = ''

      // 针对设备分布做处理，全部机构时，设备分布选项显示，其他机构时不显示
      if (this.statisticTypeModel === 1 && id !== '0') {
        this.statisticTypeModel++
      }
      this.setPieChart(this.chartApi)

      this.getEquipmentList(this.startPage)
    },
    // 分页
    changePage (index) {
      this.startPage = index
      this.getEquipmentList(index)
    },
    // 获取设备列表
    getEquipmentList (i) {
      i = i || 1
      let args = {
        pageSize: this.pageSize,
        pageNum: i,
        name: this.searchKey,
        centerId: this.hospitalCheckedId,
        eqState: this.eqState,
        eqType: this.selectMachineType
      }
      this.loading = true
      this.swsApi
        .swsPost('Equipment/Equipment', args)
        .then(res => {
          this.loading = false
          if (res.data.result) {
            this.equipment_data = res.data.result
            this.equipmentDataCount = res.data.dataCount
          } else {
            this.$Message.info(res.data.error)
          }
        })
        .catch(e => {
          this.loading = false
        })
    },
    // 下载图片
    download () {
      let args = {
        centerId: this.hospitalCheckedId,
        pageSize: 10000,
        name: '',
        eqState: 0,
        pageNum: 1,
        eqType: '0'
      }
      this.swsApi
        .swsPost('Equipment/Equipment', args)
        .then(res => {
          if (res.data.result) {
            let data = res.data.result.map(item => {
              let dataState = item.dataState
              let _txt = ''

              if (dataState === 1) {
                _txt = '正常使用'
              } else if (dataState === 2) {
                _txt = '已维修'
              } else {
                _class = 'txt-down'
                _txt = '已报废'
              }
              item.dataState = _txt

              item.serialNumber += '\t'
              item.maintenanceDate += '\t'
              item.purchaseDate += '\t'
              return item
            })

            this.$refs.table.exportCsv({
              filename: `${this.hospitalCheckedName}设备统计表`,
              columns: this.table_column.slice(0, -1),
              data
            })
          } else {
            this.$Message.error('网络错误，请稍后再试')
          }
        })
    },
    // 筛选设备状态
    selectEquipmentS (value) {
      let s = 0
      equipmentStateArr.forEach(v => {
        if (v.txt === value) {
          s = v.id
        }
      })
      this.eqState = s
      this.startPage = 1
      this.getEquipmentList(this.startPage)
    },
    // 筛选设备类型
    selectEquipmentT (value) {
      this.selectMachineType = value
      this.getEquipmentList(this.startPage)
    },
    // 搜索设备
    searchE () {
      this.startPage = 1
      this.getEquipmentList(this.startPage)
    },
    setChart () {
      // 设置图表
      this.setPieChart(this.chartApi)
    },
    handleWatch ({ id }) {
      let equipmentDetail = this.equipment_data.filter(v => {
        return v.id === id
      })[0]
      this.equipmentDetail = equipmentDetail
      this.$refs.equipmentDetail.show()
    }
  },
  components: {
    parentView2,
    equipmentDetail,
    Operate,
    swsTable
  }
}
</script>

<style scoped lang="less">
// .fade-enter-active,
// .fade-leave-active {
//   transition: opacity 0.05s;
// }
// .fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
//   opacity: 0;
// }
#equipment {
  position: relative;
  width: 100%;
  height: 100%;
  .chart {
    height: 480px;
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
  .equipment-operate {
    margin-top: 20px;
    padding: 20px 0;
    background: #ffffff;
    .operate {
      padding: 0 20px;
      // .operate-search .ivu-row > * {
      //    margin-top: 10px;
      // }
    }
    .organization-list {
      margin-top: 20px;
    }
    // cy:调大table的字体
    .ivu-table-wrapper {
      & /deep/ .ivu-table td {
        font-size: 13px;
      }
    }
    /deep/ .ivu-table-cell {
      .txt-down {
        color: #fc4b4b;
      }
      .txt-fix {
        color: #4f95e8;
      }
    }
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
.inline-block + .inline-block {
  margin-left: 10px;
}
#equipment-detail {
  position: absolute;
  top: 0;
  right: 0;
  height: 100%;
  z-index: 10;
}
</style>
