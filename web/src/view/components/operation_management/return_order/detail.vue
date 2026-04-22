<template>
  <transition name="fade_enter">
    <div class="purchase_detail" v-show="purchase_detail">
      <div class="top">
        <p class="record">
          <span class="right button-group">
            <Button
              type="default"
              class="approval"
              size="large"
              @click="showApproval"
              v-if="formDetail.groupAuditStatus === '1' || !formDetail.groupAuditStatus"
              v-permission="buttonRole.THDD_SHENPI"
            >审批</Button>
            <Button
              type="primary"
              size="large"
              v-permission="buttonRole.THDD_DC"
              @click="exportDetailTable"
            >导出</Button>
            <Button type="default" size="large" @click="hide" icon="md-undo">返回</Button>
          </span>
        </p>
      </div>
      <Divider class="split-line"></Divider>
      <div class="detail_table" ref="detail_table">
        <Table
          :height="tableHeight"
          :columns="table_columns"
          :data="table_data"
          :loading="loading"
          ref="purchaseReqTable">

          <template slot-scope="{ row }" slot="salesReturnQty">
            <!-- <InputNumber :min="0" v-model="editSalePrice" v-if="editIndex === index"></InputNumber> -->
            <span :class="{'high_light': (row.id!==null && row.salesReturnQty !== row.ySalesReturnQty)}">{{ row.salesReturnQty }}</span>
          </template>
        
          <template slot-scope="{ row, index }" slot="action">
            <!-- <div v-if="editIndex === index" class="button-group">
              <Button type="primary" size="small" @click="handleSave(row, index)">保存</Button>
              <Button size="small" @click="editIndex = -1">取消</Button>
            </div> -->
            <!-- cy  <div v-else v-show="row.isUpdate"> -->
            <div v-if="row.id !== null && (!formDetail.groupAuditStatus || formDetail.groupAuditStatus === '1')">
              <Tooltip content="修改" placement="top" transfer v-permission="buttonRole.THDD_EDIT">
                <Icon
                  type="md-create"
                  size="22"
                  color="#4f95e8"
                  @click="handleEdit(row, index)"
                  style="cursor: pointer;font-size: 18px;"
                ></Icon>
              </Tooltip>
            </div>
          </template>
        </Table>
      </div>

      <!-- 审核 -->
      <Modal v-model="approvalModal" className="vertical-center-modal" width="400">
        <p slot="header" align="center">审核退货单</p>
        <Form :label-width="60" style="margin-bottom: 10px;">
          <FormItem label="审核状态">
            <Select v-model="approval.groupAuditStatus" style="width: 160px" placeholder="请选择是否同意">
              <Option :value="2">同意</Option>
              <Option :value="3">拒绝</Option>
            </Select>
          </FormItem>
          <FormItem label="审核意见" style="margin-bottom: 10px">
            <Input type="textarea" :rows="4" placeholder="请输入审核意见..." v-model="approval.groupAdvice"/>
          </FormItem>
        </Form>
        <div slot="footer" class="center">
          <Button type="primary" v-show="!saveLoading" @click="saveApproval">审核</Button>
          <Button type="primary" v-show="saveLoading" :loading="true">审核中</Button>
          <Button type="default" @click="approvalModal = false">取消</Button>
        </div>
      </Modal>

      <!-- 修改退货数量 -->
      <Modal v-model="editQtyModal" className="vertical-center-modal" width="400">
        <p slot="header" align="center">审核退货单</p>
        <Form :label-width="60" style="margin-bottom: 10px;">
          <FormItem label="退货数量">
            <InputNumber :min="0" v-model="editData.salesReturnQty" style="width: 120px"></InputNumber>
          </FormItem>
          <FormItem label="修改原因" style="margin-bottom: 10px">
            <Input type="textarea" :rows="4" placeholder="请输入修改原因..." v-model="editData.remark"/>
          </FormItem>
        </Form>
        <div slot="footer" class="center">
          <Button type="primary" @click="submitQty" :loading="saveLoading">确定</Button>
          <Button type="default" @click="editQtyModal = false">取消</Button>
        </div>
      </Modal>

    </div>
  </transition>
</template>

<script>
// import { subtract } from '@/libs/tools'

const BUTTONROLE = {
  THDD_SHENPI: 'THDD_SHENPI',
  THDD_DC: 'THDD_DC',
  THDD_EDIT: 'THDD_EDIT'
}
export default {
  data () {
    return {
      editQtyModal: false,
      editData: {
        id: '',
        remark: '',
        salesReturnQty: 0
      },
      // editIndex: -1,
      tableHeight: 0,
      purchase_detail: false,
      // 表格
      table_columns: [
        {
          title: '序列',
          key: 'no',
          width: 50
        },
        {
          title: '物品名称',
          key: 'medicalItemName',
          minWidth: 100
        },
        {
          title: '规格',
          key: 'specifications',
          minWidth: 90
        },
        {
          title: '包装单位',
          key: 'procurementUnit',
          align: 'center',
          minWidth: 70
        },
        {
          title: '批次号',
          key: 'batchNo',
          minWidth: 100
        },
        {
          title: '申请数量',
          key: 'ySalesReturnQty',
          minWidth: 80,
          align: 'center'
        },
        {
          title: '审核数量',
          key: 'salesReturnQty',
          slot: 'salesReturnQty',
          minWidth: 80,
          align: 'center'
        },
        {
          title: '采购单价',
          key: 'inPrice',
          align: 'center',
          minWidth: 70
        },
        {
          title: '总价',
          key: 'inSumMoney',
          align: 'center',
          minWidth: 75
        },
        {
          title: '生产日期',
          key: 'productionDate',
          align: 'center',
          width: 120,
          render: (h, params) => {
            return <span>{params.row.productionDate!==null && new Date(params.row.productionDate).toLocaleDateString().replace(/\//g, '-')}</span>
          }
        },
        {
          title: '有效期至',
          key: 'qualityDate',
          align: 'center',
          width: 120,
          render: (h, params) => {
            return <span>{params.row.productionDate!==null && new Date(params.row.qualityDate).toLocaleDateString().replace(/\//g, '-')}</span>
          }
        },
        {
          title: '厂家',
          key: 'manufacturer',
          minWidth: 120
        },
        {
          title: '备注',
          key: 'remark',
          minWidth: 120
        },
        {
          title: '操作',
          slot: 'action',
          fixed: 'right',
          align: 'center',
          width: 60
        }
      ],
      table_data: [],
      loading: false,

      approvalModal: false,
      saveLoading: false,

      approval: {
        id: null,
        groupAuditStatus: 2,
        groupAdvice: ''
      },
      buttonRole: BUTTONROLE
    }
  },
  props: {
    formDetail: {
      require: true,
      default: () => {
        return {}
      }
    }
  },
  watch: {
    formDetail () {
      this.$nextTick(() => {
        this.tableHeight = this.$refs.detail_table.clientHeight
      })
    }
  },
  methods: {
    handleEdit (row, index) {
      // console.log(row,index)
      // this.editIndex = index
      this.editData.salesReturnQty = row.salesReturnQty
      this.editData.id = row.id
      this.editQtyModal = true
    },
    submitQty () {
      this.saveLoading = true
      this.swsApi.swsPost('CenterDocking/ReturnDetails/UpdatereturnDetail',this.editData)
        .then(res => {
          if (res.data.success) {
            this.editQtyModal = false
            this.getFormDetail()
          } else {
            this.$Message.error(`错误：${res.data.error}`)
          }
          this.saveLoading = false
        })
        .catch(e => {
          this.saveLoading = false
          console.log(e)
        })
    },
    exportDetailTable () {
      // let datas = JSON.parse(JSON.stringify(this.table_data))
      let datas = JSON.parse(JSON.stringify(this.table_data)).map(res => {
        // 20200514 cy 处理导出数据中有英文逗号而导致英文逗号后面的数据换到下一列中的问题
        if (typeof res.manufacturer === 'string' && res.manufacturer.indexOf(',') !== -1) {
          res.manufacturer = res.manufacturer.replace(',', ' ')
        }
        return res
      })
      let columns = JSON.parse(JSON.stringify(this.table_columns))
      columns.pop()
      let params = {
        filename: `退货订单${this.formDetail.returnNo}明细表`,
        columns: columns,
        data: datas
      }
      this.$refs.purchaseReqTable.exportCsv(params)
    },
    show () {
      this.purchase_detail = true
      this.table_data = []
      setTimeout(() => {
        this.getFormDetail()
      }, 300)
    },
    getFormDetail () {
      this.loading = true
      this.swsApi
        .swsGet(`CenterDocking/ReturnDetails/list/${this.formDetail.id}`)
        .then(res => {
          if (res.data.success) {
            this.table_data = res.data.result.map((item, index) => {
              item.no = index + 1
              return item
            })
          } else {
            this.$Message.error(`退货单详情查询出错，请稍后再试！`)
          }
          this.loading = false
        })
        .catch(e => {
          this.loading = false
          console.log(e)
        })
    },
    hide () {
      this.purchase_detail = false
      this.priceDetail = {}
      this.approvalProcessOutPuts = []
      this.$emit('on-hide')
    },
    showApproval () {
      this.approvalModal = true
      this.approval.id = this.formDetail.id
    },
    saveApproval () {
      if (!this.approval.groupAdvice.trim()) {
        this.$Message.error(`请输入审批意见！`)
        return false
      }
      this.saveLoading = true
      this.swsApi
        .swsPost('CenterDocking/ReturnDetails/Examine', this.approval)
        .then(res => {
          if (res.data.success) {
            this.$Message.success(`审批完成！`)
            this.purchase_detail = false
            this.$emit('on-save')
            this.$emit('on-hide')
          } else {
            this.$Message.error(res.data.error)
          }
          this.approvalModal = false
          setTimeout(() => {
            this.saveLoading = false
          }, 100)
        })
        .catch(e => {
          this.approvalModal = false
          this.saveLoading = false
          console.log(e)
        })
    }
  },
  components: {}
}
</script>

<style scoped lang="less">
.fade_enter-enter-active {
  transition: opacity 0.3s;
}
.fade_enter-enter, .fade_enter-leave-to /* .fade-leave-active below version 2.1.8 */ {
  opacity: 0;
}
.purchase_detail {
  display: flex;
  flex-direction: column;
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 100%;
  padding: 20px;
  background: #ffffff;
  z-index: 4;
  overflow-y: auto;
  .top {
    .record {
      margin: 10px 0;
      color: #333333;
      font-size: 16px;
      span {
        margin-left: 10px;
      }
      .right {
        float: right;
        .approval {
          background: #f90;
          color: #ffffff;
          border-color: #f90;
        }
      }
      &::after {
        display: block;
        content: '';
        height: 0;
        visibility: hidden;
        clear: both;
      }
    }
  }
  .split-line {
    flex: 0 0 1px;
  }
  .items-box {
    font-size: 14px;
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
    
    & /deep/ .ivu-table .high_light {
      // background-color: #EBF7FF;
      color: red;
      font-size: 15px;
      font-weight: bold;
    }

    & /deep/ .ivu-table .ivu-table-cell {
      padding-right: 8px;
      padding-left: 8px;
    }
    & /deep/ .ivu-table th {
      font-size: 14px;
      background: #f9f9f9;
      border-bottom: none;
    }
  }
  .button-group {
    button + button {
      margin-left: 10px;
    }
  }
  .detail_table {
    flex: 1;
    overflow: hidden;
  }
}
</style>
