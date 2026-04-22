<template>
  <div id="purchase_requisition_form" :class="{ overflow : detailShowFlage}">
    <div class="btn-groups">
      <span>透析中心</span>
      <Select
        v-model="hospitalCheckedId"
        @on-change="hostipalChange"
        placeholder="请选择"
        filterable
        style="width: 200px;"
      >
        <Option value="0">全部</Option>
        <Option v-for="item in hospitalList" :key="item.id" :value="item.id">{{item.dialysisName}}</Option>
      </Select>
      <span>物品类型</span>
      <Cascader
        style="width:170px;display: inline-table;"
        :data="catalogList"
        trigger="hover"
        @on-change="cascaderChange"
        placeholder="请选择物品类型">
      </Cascader>
      <span>审核状态</span>
      <Select transfer v-model="operateCheck" @on-change="changeState" placeholder="请选择状态" style="width: 125px;">
        <Option v-for="item in stateList" :value="item.id" :key="item.id">{{item.label}}</Option>
      </Select>
      <span>订单状态</span>
      <Select transfer v-model="orderTypeCheck" @on-change="changeOrderType" placeholder="请选择状态" style="width: 125px;">
        <Option v-for="item in orderStateList" :value="item.index" :key="item.index">{{item.label}}</Option>
      </Select>
    </div>
    <div class="btn-groups">
      <span>时间段</span>
      <DatePicker
        v-model="time"
        format="yyyy-MM-dd"
        type="daterange"
        placement="bottom-start"
        placeholder="请选择时间阶段"
        style="width: 200px;"
        @on-change="changeDate"
      ></DatePicker>
      <span>关键词</span>
      <Input v-model="keySearch" search enter-button @on-search="searchKey" placeholder="请输入关键字" style="width: 170px;display: inline-table;"/>
      <Button type="primary" @click="refresh" :loading="refreshLoading">刷新</Button>
      <!-- <Button type="primary" v-show="!mergeFlag && buttonRole.CGSQD_HBCGD" @click="mergeBtn(1)">合并生成订单</Button>
      <Button type="info" v-show="!mergeFlag && buttonRole.CGSQD_HBCGD" @click="mergeBtn(2)">合并采购单</Button> -->
    </div>
    <Divider></Divider>
    <div class="detail_table">
      <!--  -->
      <Table
        ref="purchaseTable"
        :columns="table_columns"
        :data="table_data"
        :loading="loading"
        @on-select-all="handleSelectAll"
        @on-select-all-cancel="handleSelectAll"
        @on-select="handleSelectRow"
        @on-select-cancel="handleCancelRow"
      ></Table>
      <div class="merge-btn-goups" v-show="mergeFlag">
        <Button type="primary" style="margin-right:10px;" @click="showMerge">确定合并</Button>
        <Button style="margin-left:10px;" type="default" @click="mergeCancel">取消</Button>
        <span style="margin-left:10px;">已经选中：{{selectedSum}} 项</span>
        <!-- <span style="margin-left:20px;color: red;">提示：合并采购单只能勾选未审批状态的采购单</span> -->
      </div>
      <div class="pagination" v-if="dataCount > pageSize">
        <Page
          :total="dataCount"
          :page-size="pageSize"
          :current.sync="startPage"
          @on-change="handleChangePage"
        />
      </div>
    </div>

    <detail
      ref="detail"
      :form-detail="formDetail"
      :ids-list="Array.from(selectedIds)"
      :supplier-info="supplierInfo"
      :supplier-list="Supplier"
      :catalog-list="catalogList"
      :is-merge="mergeDetailShow"
      :selected-type-id="cataId"
      :merge-type="mergeType"
      @on-save="getFormList"
      @on-hide="afterHide"
      @on-refresh="refresh"
    ></detail>
  </div>
</template>

<script>
import detail from './detail.vue'
import merge from './merge.js'
import {
  getXMonthFirst,
  getNowDate
} from '@/libs/tools.js'
// import { getNowDate } from '@/libs/tools.js'
const BUTTONROLE = {
  CGSQD_HBCGD: 'CGSQD_HBCGD'
}
export default {
  name: 'purchase_requisition_form',
  data () {
    let _this = this
    return {
      orderTypeCheck: 3, // cy 订单状态筛选
      catalogList: [], // cy 档案目录list
      selectedcCataId: '', // cy 单选项 物品类型id
      cataId: '', // cy 查看的该行数据的物品类型id
      refreshLoading: false, // 刷新按钮loading
      keySearch: '', // 关键词搜索
      supplierInfo: {
        supplierName: '', // cy 供应商名称
        supplierCheckedId: '' // cy 选中的供应商id
      },
      Supplier: [], // cy 供应商list
      mergeType: 0, // cy 合并类型
      // cy 合并状态控制
      mergeDetailShow: false,
      mergeFlag: false,
      mergeModal: false,
      selectedIds: new Set(), // 选中的合并项
      selectedSum: 0,
      totalMergeDetail: {}, // 合并的总金额等合计详情数据
      totalMergeData: [], // 合并的所有table数据

      time: [],
      operateCheck: '0',
      operateCheckName: '0',
      hospitalList: [],
      hospitalCheckedId: '0',
      // 表格
      table_columns: [
        {
          title: '序列',
          key: 'no',
          width: 50
        },
        {
          title: '所在机构',
          key: 'centerName',
          minWidth: 110
        },
        {
          title: '物品类型',
          key: 'catalogueName',
          minWidth: 70
          // render: (h, params) => {
          //   return <p>{this.findCatelogName(params.row.catalogue)}</p>
          // }
        },
        {
          title: '采购申请单',
          key: 'purchaseNo',
          minWidth: 125
        },
        {
          title: '物品种类',
          key: 'itemType',
          align: 'center',
          minWidth: 50,
          maxWidth: 100
        },
        {
          title: '采购总金额',
          key: 'actualPrice',
          align: 'center',
          minWidth: 60,
          maxWidth: 100
        },
        {
          title: '合计总数量',
          key: 'totalQty',
          align: 'center',
          minWidth: 60,
          maxWidth: 100
        },
        {
          title: '中心端审核人',
          key: 'auditor',
          align: 'center',
          minWidth: 60,
          maxWidth: 110
        },
        {
          title: '中心端审核时间',
          key: 'auditDates',
          minWidth: 138
        },
        {
          title: '备注',
          key: 'remark',
          tooltip: true,
          minWidth: 145
        },
        {
          title: '订单状态',
          key: 'purchaseToOrder',
          minWidth: 95,
          maxWidth: 120,
          align: 'center',
          render: (h, params) => {
            let color, text, content
            if (params.row.orderType === 0) {
              text = '全部未生成'
              color = 'red'
            } else if (params.row.orderType === 1) {
              text = '部分已生成'
              color = 'green'
            } else if (params.row.orderType === 2) {
              text = '全部已生成'
              color = 'blue'
            }
            if(params.row.catalogueName=='低值易耗'){
              text = '无需生成'
              color = 'green'
            }
            content = params.row.orderType === 0 ? [] : params.row.purchaseToOrder.map(res => {
              return h(
                'div',
                `${
                  res.medicaItemType === 1
                    ? '药品'
                    : res.medicaItemType === 2
                      ? '耗材'
                      : '其他'
                }类型订单号：${res.ordreNO}`
              )
            })
            if (content.length !== 0) {
              return h('div', [
                h(
                  'Poptip',
                  {
                    props: {
                      wordWrap: true,
                      width: 250,
                      trigger: 'hover',
                      placement: 'left'
                    }
                  },
                  [
                    h(
                      'Tag',
                      {
                        props: {
                          color: color
                        }
                      },
                      text
                    ),
                    h(
                      'div',
                      {
                        slot: 'content'
                      },
                      content
                    )
                  ]
                )
              ])
            } else {
              return <tag color={color}>{text}</tag>
            }
          }
        },
        {
          title: '审核状态',
          key: 'groupAuditStatus',
          align: 'center',
          minWidth: 80,
          maxWidth: 110,
          render: (h, params) => {
            let state = params.row.groupAuditStatus
            let arr, color, label
            arr = this.stateList.filter(item => {
              return item.id === state
            })
            if (arr.length) {
              // [color, label] = arr[0]
              color = arr[0].color
              label = arr[0].label
            }
            return <tag color={color}>{label}</tag>
          }
        },
        {
          title: '查看',
          key: 'action',
          align: 'center',
          fixed: 'right',
          minWidth: 50,
          maxWidth: 100,
          render: (h, params) => {
            return (
              <div>
                <tooltip content="查看" transfer placement="top">
                  <icon
                    type="md-list"
                    size="22"
                    color="#4f95e8"
                    onClick={() => {
                      this.showDetail(params.row)
                    }}
                    style={{ cursor: 'pointer' }}
                  />
                </tooltip>
              </div>
            )
          }
        }
      ],
      table_data: [],
      dataCount: 0,
      loading: false,
      pageSize: 9,
      startPage: 1,

      formDetail: {},
      // 时间
      beginTime: null,
      endTime: null,
      // 状态
      stateList: [
        {
          id: '0',
          label: '全部'
        },
        {
          id: '2',
          label: '未审批',
          color: 'default'
        },
        {
          id: '3',
          label: '已同意',
          color: 'success'
        },
        {
          id: '4',
          label: '已拒绝',
          color: 'error'
        },
        {
          id: '6',
          label: '上级待审',
          color: 'primary'
        },
        {
          id: '7',
          label: '暂 缓',
          color: 'warning'
        }
      ],
      orderStateList: [
        { index: 3, label: '全部' },
        { index: 0, label: '未生成' },
        { index: 1, label: '部分生成' },
        { index: 2, label: '全部已生成' }
      ],
      // 详情页显示
      detailShowFlage: false,
      // 缓存当前scrollTop值
      scrollTop: 0,

      buttonRole: {
        CGSQD_HBCGD: _this._filterButton(BUTTONROLE.CGSQD_HBCGD)
      }
    }
  },
  beforeRouteEnter (to, from, next) {
    // fetchData(to.params.auditStatusType, (err, post) => {
    // console.log('beforeRouteEnter', to.params.auditStatusType)
    next(vm => vm.fetchData(to.params.auditStatusType))
    // })
  },
  created () {
    this.beginTime = getXMonthFirst(-3)
    this.endTime = getNowDate().substring(0, 10)
    this.time = [this.beginTime, this.endTime]
    let args = {
      pageSize: 10000
    }
    this.getHospitalList()
    this.swsApi
      .swsPost('Data/Supplier/list', args)
      .then(res => {
        if (res.data.result) {
          this.Supplier = res.data.result
        } else {
          this.$Notice.error({
            title: '请求供应商错误',
            desc: '网络错误，请稍后再试'
          })
        }
      })
    this.swsApi
      .swsPost('Data/WarehouseCatalog/tree/0')
      .then(res => {
        if (res.data.success) {
          let list = res.data.result.filter(item => { return item.title !== '诊疗项目' })
          this.catalogList = list.map(res => {
            if (res.title !== '固定资产') {
              return {value: res.id, label: res.title, index: `${res.cataIndex}`}
            } else {
              let children = []
              children = res.children.map(re => {
                return {value: re.id, label: re.title}
              })
              return {value: res.id, label: res.title, index: `${res.cataIndex}`, children: children}
            }
          })
          // let reg = new RegExp('title', 'g')
          // let reg1 = new RegExp('id', 'g')
          // let cas = JSON.stringify(list)
          //   .replace(reg, 'label')
          //   .replace(reg1, 'value')
          // this.catalogList = JSON.parse(cas)
          // 添加一个全部
          this.catalogList.unshift({value: '', label: '全部'})
        }
      })
      .catch(e => {
        console.log(e)
      })
  },
  mounted () {
    // this.getFormList()
    this.$nextTick(() => {
      this.getFormList()
    })
  },
  mixins: [merge],
  methods: {
    fetchData (type) {
      if (type) {
        this.operateCheck = type
        // this.afterHide()
        this.getFormList()
      }
    },
    // cy 物品类型change
    cascaderChange (ids) {
      this.selectedcCataId = ids.pop() || ''
      this.mergeFlag && this.mergeCancel() // 如果是合并状态则取消合并
      this.getFormList(this.startPage)
    },
    searchKey () {
      this.getFormList()
    },
    // cy 选择供应商
    selectSupplier (id) {
      let arr = this.Supplier.filter(res => { return res.id === id })
      this.supplierInfo.supplierName = arr.length ? arr[0].name : ''
    },
    // 清空modal合并的数据
    mergeModalCancel () {
      this.totalMergeData = []
    },
    // cy 全选和取消全选时触发
    handleSelectAll (selection) {
      if (selection.length === 0) {
        // cy 若取消全选，删除保存在selectedIds里和当前table数据的id一致的数据，达到，当前页取消全选的效果
        // 当前页的table数据
        let data = this.$refs.purchaseTable.data
        data.forEach(item => {
          if (this.selectedIds.has(item.id)) {
            this.selectedIds.delete(item.id)
          }
        })
      } else {
        selection.forEach(item => {
          this.selectedIds.add(item.id)
        })
      }
      this.selectedSum = this.selectedIds.size
    },
    // cy 选中某一行
    handleSelectRow (selection, row) {
      this.selectedIds.add(row.id)
      this.selectedSum = this.selectedIds.size
    },
    // cy 取消某一行
    handleCancelRow (selection, row) {
      this.selectedIds.delete(row.id)
      this.selectedSum = this.selectedIds.size
    },
    // cy 合并,开启表单多选
    mergeBtn (type) {
      this.mergeType = type
      if (this.selectedcCataId === '' || this.selectedcCataId === undefined) {
        this.$Modal.warning({
          title: '提示',
          content: '<p>请先选择具体物品类型（非全部）！</p>'
        })
        return false
      }
      if (type === 2) {
        this.operateCheck = '2'
        this.getFormList()
      } else if (type === 1) {
        this.operateCheck = '3'
        this.getFormList()
      }
      this.mergeFlag = true
      this.table_columns.unshift({
        type: 'selection',
        width: 32,
        align: 'center'
      })
      this.setChecked(type)
    },
    // cy 取消合并
    mergeCancel () {
      this.mergeType = 0
      this.supplierInfo = {}
      this.mergeFlag = false
      this.mergeModal = false
      this.cataId = ''
      this.$refs.purchaseTable.selectAll(false)
      // 清空ids集合
      this.clearSelectedIds()
      this.table_columns.shift()
    },
    // cy 复用detail模板来显示合并采购单的数据
    showMerge () {
      let selectedIdsArr = Array.from(this.selectedIds)
      if (selectedIdsArr.length < 2) {
        this.$Modal.warning({
          title: '提示',
          content: '<p>请至少选择两项进行合并操作！</p>'
        })
        return false
      }
      // cy 更新 给合并操作赋值上合并的物品类型
      this.cataId = this.selectedcCataId
      this.detailShowFlage = true
      // cy 更新 这里的合并采购单类型（mergeType=2） 走正常的采购单流程 so 该标识设置为false
      this.mergeDetailShow = this.mergeType === 1
      // cy 清空该数据
      this.formDetail = {}
      let dom = document.querySelector('#purchase_requisition_form')
      this.scrollTop = dom.scrollTop
      this.$refs.detail.showMergeDetail()
      this.$nextTick(() => {
        dom.scrollTo(0, 0)
      })
    },
    getHospitalList () {
      this.swsApi
        .swsPost('CenterDialysis/DialysisList')
        .then(res => {
          if (res.data.success) {
            this.hospitalList = res.data.result
          }
        })
        .catch(e => {
          console.log(e)
        })
    },
    // cy 给跨页丢失的选中行重新添加选中/禁用状态
    setChecked (type) {
      // 当前页的table数据
      let objData = this.$refs.purchaseTable.objData
      for (let index in objData) {
        // 初始化禁用、已勾选状态
        objData[index]._isDisabled = false
        objData[index]._isChecked = false
        // groupAuditStatus： 2未审 3同意 4拒绝 5回退  6上级待审
        // cy 设置采购单的禁止勾选状态
        if (type === 1) {
          // 合并类型为 1（合并生成订单）
          // cy 更新：只有状态为已同意（groupAuditStatus为3）的采购单才能 被勾选
          // cy 更新：只有订单状态不为“全部已生成”（orderType！==2）的采购单才能 被勾选
          if (objData[index].groupAuditStatus !== '3' || objData[index].orderType === 2) {
            objData[index]._isDisabled = true
          }
        } else if (type === 2 && (objData[index].groupAuditStatus !== '2' || objData[index].isGroupAdd)) {
          // 合并类型为 2（合并采购单）
          // 只有审批状态为未审批 且采购单状态为未被合并（isGroupAdd=true）才能被勾选
          objData[index]._isDisabled = true
        }
        // cy 根据保存的已勾选id来设置勾选状态
        if (this.selectedIds.has(objData[index].id)) {
          // cy 弊端 每次切换select都会触发table的on-select事件
          // cy 改进
          objData[index]._isChecked = true
        }
        // console.log('000', type, objData[index].groupAuditStatus, objData[index].isGroupAdd)
      }
    },
    // 获取申请单
    getFormList (page = 1) {
      if (page === 1) this.startPage = page
      let params = {
        centerId: this.hospitalCheckedId,
        approvalState: this.operateCheck,
        beginTime: this.beginTime || null,
        endTime: this.endTime || null,
        pageSize: this.pageSize,
        pageNum: this.startPage,
        remarks: this.keySearch,
        catalogue: this.selectedcCataId || '',
        orderType: this.orderTypeCheck
      }
      this.loading = true
      this.swsApi
        // .swsPost('CenterDocking/PurchaseRequestList', params)
        .swsPost('CenterDocking/NewPurchaseRequestList', params)
        .then(res => {
          if (res.data.success) {
            this.table_data = res.data.result
            this.dataCount = res.data.dataCount
          }
          this.loading = false
        })
        .then(() => {
          // cy table数据赋值后再进行勾选状态还原
          if (this.mergeFlag) {
            this.setChecked(this.mergeType)
          }
        })
        .catch(e => {
          console.log(e)
        })
    },
    // 重新获取
    refresh () {
      // this.time = []
      this.loading = true
      this.refreshLoading = true
      let params = {
        centerId: this.hospitalCheckedId,
        approvalState: this.operateCheck,
        beginTime: this.beginTime || null,
        endTime: this.endTime || null,
        pageSize: this.pageSize,
        pageNum: this.startPage,
        remarks: this.keySearch,
        catalogue: this.selectedcCataId,
        orderType: this.orderTypeCheck
      }
      this.swsApi
        .swsPost('CenterDocking/CollectGetPurchaseRequestList', params)
        .then(res => {
          if (res.data.success) {
            this.table_data = res.data.result
            this.dataCount = res.data.dataCount
            this.$Message.success(`刷新成功！`)
            this.clearSelectedIds()
          }
          this.loading = false
          this.refreshLoading = false
        })
        .then(() => {
          // cy table数据赋值后再进行勾选状态还原
          if (this.mergeFlag) {
            this.setChecked(this.mergeType)
          }
        })
        .catch(e => {
          this.loading = false
          this.refreshLoading = false
          console.log(e)
        })
    },
    showDetail (row) {
      this.cataId = row.catalogue
      this.formDetail = row
      this.detailShowFlage = true
      let dom = document.querySelector('#purchase_requisition_form')
      this.scrollTop = dom.scrollTop
      this.$refs.detail.show()
      this.$nextTick(() => {
        dom.scrollTo(0, 0)
      })
    },
    handleChangePage (i) {
      this.getFormList(i)
    },
    saveApproval () {
      let params = {
        id: this.approvalListId,
        groupAdvice: this.approval.advice,
        groupAuditStatus: this.approval.situation
      }
      if (!this.approval.advice) {
        this.$Message.error(`请输入审批意见！`)
        return false
      }
      this.swsApi
        .swsPost('CenterDocking/ApprovalPurchaseDetail', params)
        .then(res => {
          if (res.data.success) {
            this.$Message.success(`审批完成成功`)
            this.getFormList(this.startPage)
          } else {
            this.$Message.error(`操作失败，请稍后再试！`)
          }
          this.approvalModal = false
        })
        .catch(e => {
          this.approvalModal = false
          console.log(e)
        })
    },
    hostipalChange () {
      this.getFormList(this.startPage)
      this.clearSelectedIds()
    },
    changeDate (v) {
      if (!v) return
      this.beginTime = v[0]
      this.endTime = v[1]
      this.getFormList(this.startPage)
      this.clearSelectedIds()
    },
    changeState (v) {
      this.operateCheck = v
      this.getFormList(this.startPage)
      this.clearSelectedIds()
    },
    changeOrderType (v) {
      this.orderTypeCheck = v
      this.getFormList(this.startPage)
      this.clearSelectedIds()
    },
    clearSelectedIds () {
      // 清空ids集合
      this.selectedIds.clear()
      this.selectedSum = 0
    },
    afterHide () {
      // console.log('afterHide!')
      let dom = document.querySelector('#purchase_requisition_form')
      dom.scrollTo(0, this.scrollTop)
      this.detailShowFlage = false
      this.mergeDetailShow = false
      this.getFormList(this.startPage)
      this.clearSelectedIds()
      // cy 1112
      this.cataId = ''
    }
  },
  components: {
    detail
  }
}
</script>

<style scoped lang="less">
#purchase_requisition_form {
  position: relative;
  // width: calc(~'100% - 20px');
  height: 100%;
  overflow-y: auto;
  padding: 20px 20px 0;
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
      width: 55px;
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
  .detail_table {
    margin-top: 20px;
    // cy:调大table字体
    & /deep/ .ivu-table {
      font-size: 13px;
    }
    .merge-btn-goups {
      float: left;
      margin-top: 16px;
      .ivu-select {
        position: relative !important;
      }
    }
  }
  .ivu-table-wrapper {
    border: none !important;
    & /deep/ .ivu-table-tip {
      overflow: hidden;
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
    & /deep/ .ivu-table .ivu-table-cell {
      padding-right: 0px;
      padding-left: 10px;
    }
    & /deep/ .ivu-table th {
      font-size: 14px;
      background: #f9f9f9;
      border-bottom: none;
    }
  }
}
.merge-modal {
  .items-box {
    font-size: 16px;
    .field {
      color: #666666;
      & + .red {
        color: #fc4b4b;
      }
    }
    span:not(.field) {
      font-weight: bold;
      margin-right: 50px;
    }
  }
  .ivu-table-wrapper {
    border: none !important;
    & /deep/ .ivu-table {
      tr {
        color: #666;
        border-right: none;
      }
      &::before,
      &::after {
        display: none !important;
      }
    }
  }
}
</style>
