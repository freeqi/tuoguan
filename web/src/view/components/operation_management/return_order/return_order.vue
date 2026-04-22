<template>
  <div id="purchase_requisition" :class="{ overflow : detailShowFlage}">
    <div class="btn-groups">
      <span>机构</span>
      <Select v-model="hospitalCheckedId" @on-change="changeHospital" style="width: 160px;">
        <Option v-for="item in hospitalList" :key="item.id" :value="item.dialysisId">{{item.dialysisName}}</Option>
      </Select>
      <span>订单状态</span>
      <Select transfer
        v-model="orderCheckedId"
        placeholder="请选择订单状态"
        style="width: 120px;"
        @on-change="changeOrderType"
        filterable
        >
        <Option :value="item.id" v-for="item in orderType" :key="item.id">{{item.txt}}</Option>
      </Select>
      <span>时间范围</span>
      <DatePicker
        v-model="time"
        format="yyyy-MM-dd"
        type="daterange"
        placement="bottom-end"
        placeholder="请选择时间段"
        style="width: 200px;"
        @on-change="changeDate">
      </DatePicker>
      <Input
        v-model="searchKey"
        @on-search="search"
        search
        enter-button
        placeholder="请输入关键字"
        style="width: 200px; display: inline-table;"
      />
      <Button type="primary" @click="refresh" :loading="refreshLoading">刷新</Button>
    </div>
    <Divider></Divider>
    <div class="detail_table">
      <Table :columns="table_columns" :data="table_data" :loading="loading">

        <template slot-scope="{ row, index }" slot="action">
          <!-- <template v-if="row.dataState===1"> -->
            <div class="button-group">
              <Tooltip content="查看" placement="top" transfer>
                <Icon
                  type="md-list"
                  size="22"
                  color="#4f95e8"
                  @click="showDetail(row)"
                  style="cursor: pointer"
                ></Icon>
              </Tooltip>
            </div>
          <!-- </template> -->
        </template>
      </Table>
      <div class="pagination" v-if="dataCount > pageSize">
        <Page
          :total="dataCount"
          :page-size="pageSize"
          :current.sync="startPage"
          @on-change="handleChangePage"
        />
      </div>
    </div>

    <detail ref="detail" :form-detail="formDetail" @on-hide="afterHide" @on-save="getFormList"></detail>
  </div>
</template>

<script>
import detail from './detail.vue'
import {
  getCurrentMonthFirst,
  getNowDate
} from '@/libs/tools.js'
export default {
  name: '',
  data () {
    return {
      refreshLoading: false, // 刷新按钮loading
      time: [],
      // 表格
      table_columns: [
        {
          title: '序列',
          key: 'no',
          align: 'center',
          width: 50,
          render: (h, {index}) => {
            let No = (index + 1) + this.pageSize * (this.startPage - 1)
            return <span>{No}</span>
          }
        },
        {
          title: '机构',
          key: 'centerName',
          minWidth: 125
        },
        {
          title: '退货单号',
          key: 'returnNo',
          minWidth: 125
        },
        {
          title: '种类数',
          key: 'itemType',
          width: 65,
          align: 'center'
        },
        {
          title: '退货数量',
          key: 'totalQty',
          width: 80,
          align: 'center'
        },
        {
          title: '中心审核人',
          key: 'auditor',
          align: 'center',
          minWidth: 90
        },
        {
          title: '创建时间',
          key: ' auditDate',
          minWidth: 80,
          render: (h, params) => {
            return <span>{new Date(params.row.auditDate).toLocaleDateString().replace(/\//g, '-')}</span>
          }
        },
        {
          title: '供货商',
          key: 'supplierName',
          minWidth: 65
        },
        {
          title: '备注',
          key: 'remark',
          minWidth: 110,
          tooltip: true
        },
        {
          title: '审核状态',
          key: 'remark',
          width: 110,
          render: (h, params) => {
            let status = params.row.groupAuditStatus
            if (status === '1' || !status) {
              return <tag color='default'>未审批</tag>
            } else if (status === '2') {
              return <tag color='success'>已同意</tag>
            } else {
              return <tag color='error'>已拒绝</tag>
            }
          }
        },
        {
          title: '操作',
          slot: 'action',
          fixed: 'right',
          align: 'center',
          width: 110
        }
      ],
      table_data: [],
      dataCount: 0,
      loading: false,
      pageSize: 10,
      startPage: 1,

      // 清单id
      formDetail: '',
      // 时间
      beginTime: '',
      endTime: '',
      searchKey: '',

      orderType: [
        {
          id: 0,
          txt: '全部'
        },
        {
          id: 1,
          txt: '未审批'
        },
        {
          id: 2,
          txt: '已同意'
        },
        {
          id: 3,
          txt: '已拒绝'
        },
        {
          id: 6,
          txt: '上级待审'
        }
      ],
      orderCheckedId: 0,
      formEdit: {
        id: '',
        dataState: 1,
        orderNo: '',
        applyDate: '',
        remarks: '',
        applyMain: ''
      },
      // 详情页显示
      detailShowFlage: false,
      // 缓存当前scrollTop值
      scrollTop: 0,
      // 选中订单id
      checkedListId: -1,
      // 血透中心
      hospitalList: [],
      hospitalCheckedId: '0'

    }
  },
  created () {
  },
  async mounted () {
    let d = getNowDate().substring(0, 10)
    let f = getCurrentMonthFirst()
    this.time = [f, d]
    this.beginTime = f
    this.endTime = d
    await this.getHospital()
    await this.getFormList()
  },
  methods: {
    refresh () {
      let params = {
        centerId: this.hospitalCheckedId,
        auditStatus: this.orderCheckedId,
        beginTime: this.beginTime,
        endTime: this.endTime,
        pageSize: this.pageSize,
        pageNum: this.startPage
      }
      this.refreshLoading = true
      this.swsApi
        .swsPost('CenterDocking/ReturnRequestNow/list', params)
        .then(res => {
          if (res.data.success) {
            this.table_data = res.data.result
            this.dataCount = res.data.dataCount
          }
          this.refreshLoading = false
        })
        .catch(e => {
          this.refreshLoading = false
          console.log(e)
        })
    },
    changeOrderType () {
      this.getFormList()
    },
    // 获取申请单
    getFormList (pageNum = 1) {
      let params = {
        centerId: this.hospitalCheckedId,
        auditStatus: this.orderCheckedId,
        remarks: this.searchKey,
        beginTime: this.beginTime,
        endTime: this.endTime,
        pageSize: this.pageSize,
        pageNum: pageNum
      }
      this.loading = true
      this.swsApi
        .swsPost('CenterDocking/ReturnRequest/list', params)
        .then(res => {
          if (res.data.success) {
            this.table_data = res.data.result
            this.dataCount = res.data.dataCount
          }
          this.loading = false
        })
        .catch(e => {
          console.log(e)
        })
    },
    showDetail (row) {
      this.formDetail = row
      this.detailShowFlage = true
      let dom = document.querySelector('#purchase_requisition')
      this.scrollTop = dom.scrollTop
      this.$refs.detail.show()
      this.$nextTick(() => {
        dom.scrollTo(0, 0)
      })
    },
    handleChangePage (i) {
      this.getFormList(i)
    },
    changeHospital () {
      this.startPage = 1
      this.getFormList(1)
    },
    search () {
      this.startPage = 1
      this.getFormList(1)
    },
    changeDate (v) {
      if (!v) return
      this.beginTime = this.time[0]
      this.endTime = this.time[1]

      this.startPage = 1
      this.getFormList(this.startPage)
    },
    // 获取机构
    getHospital () {
      this.swsApi
        .swsPost('Employee/GetEmplyeeByDialysisList')
        .then(res => {
          if (res.data.success) {
            this.hospitalList = res.data.result
          }
        })
        .catch(e => {
        })
    },
    // 解决小屏幕样式问题
    afterHide () {
      let dom = document.querySelector('#purchase_requisition')
      dom.scrollTo(0, this.scrollTop)
      this.detailShowFlage = false
    }
  },
  components: {
    detail
  }
}
</script>

<style scoped lang="less">
#purchase_requisition {
  position: relative;
  // width: calc(~'100% - 20px');
  height: 100%;
  overflow-y: auto;
  padding: 20px;
  background: #ffffff;
  &.overflow {
    overflow: hidden;
  }
  .btn-groups {
    color: #999;
    font-size: 13px;
    span {
      margin-right: 10px;
      display: inline-block;
      // width: 60px;
      text-align: right;
    }
    & + .btn-groups {
      margin: 20px 0;
    }
    & > * {
      margin-right: 20px;
    }
    /deep/ .ivu-radio-group-button .ivu-radio-wrapper-checked {
      color: #ffffff;
      background: #4f95e8;
    }
  }
  & > .btn-groups:last-child {
    margin-right: 0;
  }

  .button-group {
    button + button {
      margin-left: 10px;
    }
  }
  .detail_table {
    margin-top: 20px;
  }
  .ivu-table-wrapper {
    border: none !important;
    & /deep/ .ivu-table-tip {
      overflow: hidden;
    }
    & /deep/ .ivu-table .ivu-table-cell {
        padding-right: 0px;
        padding-left: 10px;
      }
    & /deep/ .ivu-table {
      td {
        color: #666;
      }
      &::before,
      &::after {
        display: none !important;
      }
    }
    & /deep/ .ivu-table th {
      font-size: 14px;
      background: #f9f9f9;
      border-bottom: none;
    }
  }
}
</style>
