<template>
  <transition name="fade_enter">
    <div class="purchase_detail" v-show="purchase_detail">
      <div class="top">
        <div class="step-wrapper" ref="step" >
          <Steps :current="currentStep" :status="currentStatus">
            <Step
              v-for="(item, index) in orderApproveOutPuts"
              :key="index"
              :title="item.title"
              :content="`${item.content}${item.time ? '，时间：': ''}${item.time ? item.time : ''}`"
            ></Step>
          </Steps>
        </div>
        <p class="record">
          采购订单号: {{formDetail.orderNo}}
          <span class="right button-group">
            <!-- <Poptip
              v-if="!formDetail.isPushCenter"
              confirm transfer
              title="确定要进行推送操作吗？"
              @on-ok="pushOrder"
              style="margin: 0 10px;">
              <Button type="success" size="large" v-permission="buttonRole.CGXQ_DC">推送</Button>
            </Poptip> -->
            <Button
              type="default"
              class="approval" v-permission="buttonRole.CGXQ_SHENPI"
              v-show="formDetail.groupAuditStatus === '2'"
              @click="showApproval"
            >审核</Button>
            <Button type="success" v-permission="buttonRole.CGXQ_DJ" v-show="orderDetail.dataState ===7" :loading="cspLoading" @click="confirmSalePriceBtn">定价</Button>
            <Button type="success" v-permission="buttonRole.CGXQ_TS" v-show="isShowPushCenterBtn" @click="pushBtn">推送</Button>
            <Button type="info" v-permission="buttonRole.CGXQ_CD" @click="splitOrder" v-show="supplierSets.size>1">拆单</Button>
            <Button type="primary" v-permission="buttonRole.CGXQ_DC" @click="exportDetailTable">导出</Button>
            <Button type="default" @click="hide" icon="md-undo">返回</Button></span>
        </p>
        <p class="items-box">
          <span class="field">物品种类：</span>
          <span>{{formDetail.itemType || '0'}}</span>
          <span class="field">合计总数量：</span>
          <span>{{formDetail.totalQty || '0'}}件</span>
          <span class="field">采购总金额：</span>
          <span class="red">￥{{formDetail.actualPrice || '-'}}</span>
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
          <template slot-scope="{ row, index }" slot="supplierName">
            <Select transfer
              v-model="editSupplierId"
              placeholder="请选择供应商"
              filterable
              v-if="editIndex === index && !yiBaoAccessType"
            >
              <Option :value="item.id" v-for="item in supplierList" :key="item.id">{{item.name}}</Option>
            </Select>
            <span v-else>{{ row.supplierName}}</span>
          </template>

          <template slot-scope="{ row, index }" slot="purchPrice">
            <InputNumber
              :min="0"
              style="width: 80px"
              v-model="editPurchPrice"
              v-if="editIndex === index  && !yiBaoAccessType"
            ></InputNumber>
            <span v-else>{{ row.purchPrice }}</span>
          </template>

          <template slot-scope="{ row, index }" slot="salePrice">
            <InputNumber
              :min="0"
              style="width: 80px"
              v-model="editSalePrice"
              v-if="editIndex === index"
            ></InputNumber>
            <span v-else-if="row.id">
              <template>
                <span
                  v-if="row.socialSecurityPrice !== null && row.dualPrice !== null"
                  :class="(row.salePrice > row.dualPrice || row.salePrice > row.socialSecurityPrice) ? 'red-color' : ''">
                  {{row.salePrice}}
                </span>
                <span v-else>
                  {{row.salePrice}}
                </span>
                <sup v-show="row.isEdit" style="color:#19BE6B">改</sup>
              </template>
            </span>
          </template>

          <template slot-scope="{ row, index }" slot="sumPrice">
            <InputNumber
              :min="0"
              style="width: 80px"
              v-model="editSumPrice"
              v-if="editIndex === index  && !yiBaoAccessType"
            ></InputNumber>
            <span v-else :class="{'emphasize': !row.id}">{{ row.sumPrice }}</span>
          </template>

          <template slot-scope="{ row, index }" slot="monthAverage">
            <InputNumber
              :min="0"
              style="width: 80px"
              v-model="editMonthAverage"
              v-if="editIndex === index && !isFixedType  && !yiBaoAccessType"
            ></InputNumber>
            <span v-else>{{ isFixedType ? '-' : row.monthAverage }}</span>
          </template>

          <template slot-scope="{ row, index }" slot="specifications">
            <Input style="width: 70px" v-model="editSpecifications" v-if="editIndex === index  && !yiBaoAccessType"/>
            <span v-else>{{ row.specifications }}</span>
          </template>

          <template slot-scope="{ row, index }" slot="currentInventory">
            <InputNumber
              :min="0"
              style="width: 80px"
              v-model="editCurrentInventory"
              v-if="editIndex === index && !isFixedType  && !yiBaoAccessType"
            ></InputNumber>
            <span v-else>{{ isFixedType ? '-' : row.currentInventory }}</span>
          </template>

          <!-- <template slot-scope="{ row, index }" slot="hurrySlowly">
            <Input style="width: 60px" v-model="editHurrySlowly" v-if="editIndex === index"></Input>
            <span v-else>{{ row.hurrySlowly }}</span>
          </template> -->

          <template slot-scope="{ row, index }" slot="remark">
            <Input
              type="textarea" :rows="3"
              v-model="editRemark" placeholder="请输入备注"
              v-if="editIndex === index"
            />
            <div v-else>
              <span v-if="row.remark===''"></span>
              <Tooltip :content="row.remark" placement="left" transfer max-width="200" v-else>
                <span class="ivu-table-cell-tooltip-content" >{{ row.remark}}</span>
              </Tooltip>
            </div>
          </template>

          <template slot-scope="{ row, index }" slot="action">
            <template v-if="row.id && (row.dataState===1 || row.dataState===3)">
              <div v-if="editIndex === index" class="button-group">
                <Button type="primary" size="small" @click="handleSave(row, index)">保存</Button>
                <Button size="small" @click="editIndex = -1">取消</Button>
              </div>
              <div v-else>
                <!-- v-permission="buttonRole.CGXQ_XG2" -->
                <Tooltip content="修改" placement="top" transfer>
                  <Icon
                    type="ios-create-outline"
                    size="22"
                    color="#4f95e8"
                    @click="handleEdit(row, index)"
                    style="cursor: pointer"
                  ></Icon>
                </Tooltip>
                <!-- <Tooltip content="关闭" placement="top" transfer>
                  <Icon
                    type="md-power"
                    size="19"
                    color="#fc4b4b"
                    @click="closeBtn(row)"
                    style="cursor: pointer;"
                  ></Icon>
                </Tooltip> -->
              </div>
            </template>
            <template v-else></template>
          </template>
        </Table>
      </div>

      <!-- 审核 -->
      <Modal v-model="approvalModal" className="vertical-center-modal" width="400">
        <p slot="header" align="center">审核申请单</p>
        <Form :label-width="60" style="margin-bottom: 10px;">
          <FormItem label="审核状态">
            <Select v-model="approval.situation" style="width: 160px" placeholder="请选择是否同意">
              <Option :value="3">同意</Option>
              <Option :value="4">拒绝</Option>
              <Option :value="5">回退</Option>
            </Select>
          </FormItem>
          <FormItem label="审核意见" style="margin-bottom: 10px">
            <Input type="textarea" :rows="4" placeholder="请输入审核意见..." v-model="approval.advice"/>
          </FormItem>
        </Form>
        <div slot="footer" class="center">
          <Button type="primary" v-show="!saveLoading" @click="saveApproval">审核</Button>
          <Button type="primary" v-show="saveLoading" :loading="true">审核中</Button>
          <Button type="default" @click="approvalModal = false">取消</Button>
        </div>
      </Modal>
      <!-- 关闭 -->
      <Modal v-model="closeModal" @on-cancel="formEdit.remark=''">
        <p slot="header" style="text-align: center;">确定要关闭该条明细：{{formEdit.medicalName}} 吗？</p>
        <Input v-model="formEdit.remark" type="textarea" :rows="4" placeholder="请输入备注..." />
        <div slot="footer">
          <Button type="error" @click="handleClose" :loading="handleBtn">确认关闭</Button>
          <Button @click="closeModal=false;formEdit.remark='';handleBtn=false">取消</Button>
        </div>
      </Modal>
    </div>
  </transition>
</template>

<script>
import { subtract } from '@/libs/tools'
const BUTTONROLE = {
  CGXQ_DC: 'CGXQ_DC',
  CGXQ_CD: 'CGXQ_CD',
  CGXQ_TS: 'CGXQ_TS',
  CGXQ_XG2: 'CGXQ_XG2',
  CGXQ_SHENPI: 'CGXQ_SHENPI',
  CGXQ_DJ: 'CGXQ_DJ'
}
export default {
  data () {
    return {
      cspLoading: false, // cy 定价loading
      saveLoading: false,
      approvalModal: false, // cy 审核modal
      approval: { // cy 审核的数据
        situation: 3,
        advice: ''
      },
      orderApproveOutPuts: [], // cy 采购订单的审批流数据
      tableHeight: 0,
      handleBtn: false,
      closeModal: false,
      formEdit: {
        id: '',
        medicalName: '',
        remark: ''
      },
      purchase_detail: false,
      // 表格
      table_base: [
        {
          title: '序列',
          key: 'no',
          width: 45
        },
        {
          title: '机构名称',
          key: 'centerName',
          minWidth: 110,
          render (h, params) {
            if (!params.row.id) {
              return <span class="emphasize">{params.row.centerName}</span>
            } else {
              return <span>{params.row.centerName}</span>
            }
          }
        },
        {
          title: '物品名称',
          key: 'medicalName',
          minWidth: 100,
          tooltip: true
        },
        {
          title: '件比',
          key: 'medicalThan',
          minWidth: 90
        },
        {
          title: '规格型号',
          slot: 'specifications',
          key: 'specifications',
          minWidth: 90
        },
        {
          title: '单位',
          key: 'uint',
          align: 'center',
          minWidth: 70
        },
        {
          title: '供应商',
          slot: 'supplierName',
          key: 'supplierName',
          minWidth: 100,
          tooltip: true
        },
        {
          title: '生产厂家',
          key: 'manufacturer',
          minWidth: 100
          // tooltip: true
        },
        {
          title: '采购定价',
          slot: 'purchPrice',
          key: 'purchPrice',
          align: 'center',
          minWidth: 88
        },
        {
          title: '销售定价',
          slot: 'salePrice',
          key: 'salePrice',
          align: 'left',
          minWidth: 88
        },
        {
          title: '双控价',
          key: 'dualPrice',
          align: 'center',
          minWidth: 75,
          render: (h, params) => {
            return params.row.id ? <span>{params.row.dualPrice || 0}</span> : <span></span>
          }
        },
        {
          title: '医保限价',
          key: 'socialSecurityPrice',
          align: 'center',
          minWidth: 75,
          render: (h, params) => {
            if (!(this.isYpHc && !params.row.isHIS)) {
              return params.row.id ? <span>{params.row.socialSecurityPrice || '/'}</span> : <span></span>
            } else {
              // return params.row.isHIS ? <span>{params.row.socialSecurityPrice || 0}</span> : <span>{params.row.socialSecurityPrice}<sup style="color:red">未</sup></span>
              return params.row.id ? <span>{params.row.socialSecurityPrice || '/'}<sup style="color:red"> 未</sup></span> : <span></span>
            }
          }
        },
        {
          title: '采购总额',
          slot: 'sumPrice',
          key: 'sumPrice',
          align: 'center',
          minWidth: 88
          // render (h, params) {
          //   if (!params.row.id) {
          //     return <span class="emphasize">{params.row.sumPrice}</span>
          //   } else {
          //     return <span>{params.row.sumPrice}</span>
          //   }
          // }
        },
        {
          title: '上月用量',
          key: 'monthAverage',
          slot: 'monthAverage',
          align: 'center',
          minWidth: 88
        },
        {
          title: '库存量',
          key: 'currentInventory',
          slot: 'currentInventory',
          align: 'center',
          minWidth: 88
        },
        {
          title: '采购数量',
          key: 'purchasePuantity',
          align: 'center',
          minWidth: 75
        },
        {
          title: '已到货量',
          key: 'actualQty',
          align: 'center',
          minWidth: 75,
          render: (h, params) => {
            if (params.row.id) {
              return <span>{params.row.actualQty}</span>
            } else {
              return ''
            }
          }
        },
        {
          title: '未到货量',
          key: 'unreceivedQty',
          align: 'center',
          minWidth: 75,
          render: (h, params) => {
            if (params.row.id) {
              let num = subtract(params.row.purchasePuantity, params.row.actualQty)
              let redStyle = {color: 'red'}
              return <span style={num < 0 ? redStyle : ''}>{num}</span>
            }
            return ''
          }
        },
        {
          title: '备注',
          key: 'remark',
          slot: 'remark',
          minWidth: 75
        },
        // {
        //   title: '急缓程度',
        //   key: 'hurrySlowly',
        //   slot: 'hurrySlowly',
        //   align: 'center',
        //   minWidth: 75
        // },
        {
          title: '到货状态',
          minWidth: 80,
          key: 'dataState',
          align: 'center',
          fixed: 'right',
          render: (h, params) => {
            if (params.row.dataState === 1) {
              return (
                <i-button
                  size="small"
                  type="info"
                  onClick={() => {
                    this.closeBtn(params.row)
                  }}>
                  {this.dataStateList[params.row.dataState]}
                </i-button>
              )
            } else if (params.row.dataState === 2) {
              return (
                <i-button
                  size="small"
                  style="background:#c5c8ce;cursor:not-allowed;border-color:#c5c8ce;"
                >
                  {this.dataStateList[params.row.dataState]}
                </i-button>
              )
            } else if (params.row.dataState === 3) {
              return (
                <i-button size="small" type="warning"
                  onClick={() => {
                    this.closeBtn(params.row)
                  }}>
                  {this.dataStateList[params.row.dataState]}
                </i-button>
              )
            } else if (params.row.dataState === 4) {
              return (
                <i-button size="small" type="success">
                  {this.dataStateList[params.row.dataState]}
                </i-button>
              )
            } else {
              return ''
            }
          }
        }
      ],
      dataStateList: ['', '未到货', '已关闭', '部分到货', '已收货'],
      table_operate: [
        {
          title: '操作',
          slot: 'action',
          align: 'center',
          width: 115,
          fixed: 'right'
        }
      ],
      table_columns: [],
      table_data: [],
      loading: false,

      editIndex: -1,
      editMonthAverage: 0,
      editCurrentInventory: 0,
      // editHurrySlowly: 0,
      editSupplierId: '0', // cy 改供应商
      editPurchPrice: 0, // cy 改新采购价
      editSumPrice: 0, // cy 改采购总额
      editSalePrice: 0, // cy 销售
      editSpecifications: '', // cy 规格型号
      editRemark: '', // cy 备注

      supplierSets: new Set(),
      emptyPrice: false,
      buttonRole: BUTTONROLE,

      // formDetail: [] // cy 改机制
      orderDetail: {}
    }
  },
  props: {
    supplierList: {
      require: true,
      default: []
    },
    formDetail: {
      require: true,
      default: () => {
        return {}
      }
    }
  },
  computed: {
    yiBaoAccessType () {
      return this._filterButton(this.buttonRole.CGXQ_DJ) && this.formDetail.dataState === 7
    },
    editAllAccessType(){
      return this._filterButton(this.buttonRole.CGXQ_XG2) && this.formDetail.dataState === 5
    },
    editSomeAccessType(){
      return this._filterButton(this.buttonRole.CGXQ_XG2) && this.formDetail.dataState === 8
    },
    // 固定资产类型？
    isFixedType () {
      return [3, 5, 6, 7].includes(this.formDetail.medicalItemType)
    },
    isYpHc () {
      return this.formDetail.medicalItemType === 1 || this.formDetail.medicalItemType === 2
    },
    currentStep () {
      if (this.orderApproveOutPuts.length > 0) {
        return this.orderApproveOutPuts[0].current
      } else {
        return 0
      }
    },
    currentStatus () {
      if (this.formDetail.groupAuditStatus === '4') {
        return 'error'
      } else {
        return 'process'
      }
    },
    // cy 是否展示推送按钮
    isShowPushCenterBtn () {
      // cy 更新 只有在未推送时（dataState=8） 才显示推送
      // if (!this.formDetail.isPushCenter && this.formDetail.dataState !==2 && this.formDetail.groupAuditStatus === 3) {
      if (this.formDetail.dataState === 8) {
        return true
      } else {
        return false
      }
    }
  },
  watch: {
    formDetail () {
      // cy 更新 只有在未审批状态（dataState=5）和 有修改权限时 才能修改
      // cy 再更新 只要有医保人员权限 也可以进行定价操作
      // let hasEdit = this._filterButton(this.buttonRole.CGXQ_XG2) && this.formDetail.dataState !== 2 && !this.formDetail.isPushCenter
      // let hasEdit = (this._filterButton(this.buttonRole.CGXQ_XG2) && this.formDetail.dataState === 5) || this._filterButton(this.buttonRole.CGXQ_DJ)
      // 20220322 cy 可以修改的情况
      // 未审批  5 且 修改权限  都可以修改
      // 未定价  7 且 医保权限  只改销售价
      // 未推送  8 且 修改权限  只改采购定价，供应商
      let hasEdit = [5,7,8].includes(this.formDetail.dataState)
      // 20191231 cy 增加对表单列的过滤显示
      let fliterColumns = this.table_base.filter(res => {
        // 如果订单物品类型为2,3(5,6,7),4 过滤掉双控价
        if (res.key === 'dualPrice') {
          return ![2, 3, 4, 5, 6, 7].includes(this.formDetail.medicalItemType)
        } else if (res.key === 'socialSecurityPrice') {
          // 如果订单物品类型为3(5,6,7),4 过滤掉医保限价
          return ![3, 4, 5, 6, 7].includes(this.formDetail.medicalItemType)
        } else {
          return true
        }
      })
      if (hasEdit) {
        this.table_columns = [...fliterColumns, ...this.table_operate]
      } else {
        this.table_columns = fliterColumns
      }
      this.$nextTick(() => {
        this.tableHeight = this.$refs.detail_table.clientHeight - 70
      })
    }
  },
  methods: {
    // cy 增加对销售定价在定价操作时的非空判断
    salePriceVerify () {
      let sum = 0
      let data = this.table_data.map(item => {
        if (item.salePrice === 0) {
          item.cellClassName = {salePrice: 'supplier-empty'}
          sum += 1
        }
        return item
      })
      this.table_data = data
      if (sum !== 0) {
        this.$Modal.warning({
          title: '提示',
          content: `<p>含有销售定价为 0 的数据，无法执行定价操作</p><p>共有 <span style="color:red;">${sum}处</span> </p>`
        })
        return false
      }
      return true
    },
    // cy 定价
    confirmSalePriceBtn () {
      this.salePriceVerify() && this.$Modal.confirm({
        title: '提示',
        content: '<p>确定要进行定价操作吗？</p>',
        onOk: () => this.confirmSalePrice()
      })
    },
    // cy 定价
    confirmSalePrice () {
      this.cspLoading = true
      this.swsApi.swsGet('CenterDocking/OrderPricing/' + this.formDetail.id)
        .then(res => {
          if (res.data.success) {
            this.$Message.success(`定价成功！`)
            this.getFormDetail()
          } else {
            this.$Message.error(`操作失败，请稍后再试！`)
          }
          this.cspLoading = false
        })
        .catch(e => {
          this.cspLoading = false
          this.$Message.error(`请求失败，请稍后再试！`)
        })
    },
    // 审批
    showApproval () {
      let sum = this.purchPriceVerify()
      if (sum !== 0) {
        this.$Modal.warning({
          title: '提示',
          content: `<p>请先完善表格中空缺的<strong>采购定价</strong>数据</p><p>共有 <span style="color:red;">${sum}处</span></p>`
        })
        return false
      }
      this.approvalListId = this.formDetail.id
      this.approvalModal = true
      this.emptyPrice = false
      this.table_data.forEach(obj => {
        obj.salePrice === 0 && (this.emptyPrice = true)
      })
    },
    // 审核请求
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
      this.saveLoading = true
      this.swsApi
        .swsPost('CenterDocking/OrderApproval', params)
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
        })
    },
    purchPriceVerify () {
      let sum = 0
      // console.log(this.table_data)
      this.table_data.map(item => {
        if (item.id && (item.purchPrice === 0 || item.purchPrice === null)) {
          sum += 1
        }
      })
      return sum
    },
    // cy 增加对药品、卫生耗材的供应商非空判断
    supplierVerify () {
      let sum = 0
      let data = this.table_data.map(item => {
        if (item.supplierName === '无') {
          item.cellClassName = {supplierName: 'supplier-empty'}
          sum += 1
        }
        return item
      })
      this.table_data = data
      if (sum !== 0) {
        this.$Modal.warning({
          title: '提示',
          content: `<p>请先完善表格中标红的供应商数据</p><p>共有 <span style="color:red;">${sum}处</span> 药品/卫生耗材类型明细</p>`
        })
        return false
      } else if (this.supplierSets.size > 1) {
        this.$Modal.warning({
          title: '提示',
          content: `<p>该订单包含多个供应商明细，请先进行拆分操作</p><p>共有 <span style="color:red;">${this.supplierSets.size}家</span> 不同供应商采购明细</p>`
        })
        return false
      } else {
        return true
      }
    },
    pushBtn () {
      let num = this.table_data.find(item => item.salePrice === 0)
      // console.log(num)
      this.supplierVerify() && this.$Modal.confirm({
        title: '提示',
        content: `<p>${num !== undefined ? '含有销售价为 <strong>零</strong> 的物品，' : ''}确定要进行推送操作吗？</p>`,
        onOk: () => {
          this.pushOrder()
        }
      })
    },
    pushOrder () {
      this.swsApi
        .swsGet('/CenterDocking/OrderDetail/NewPushCenter/' + this.formDetail.id)
        .then(res => {
          if (res.data.success) {
            this.$Message.success('推送成功')
            this.hide()
          } else {
            this.$Message.error(`操作失败，请稍后再试！`)
          }
        })
        .catch(e => {
          this.$Message.error(`请求失败，请稍后再试！`)
        })
    },
    closeBtn (row) {
      this.closeModal = true
      this.formEdit.id = row.id
      this.formEdit.medicalName = row.medicalName
    },
    // cy 关闭该条明细
    handleClose () {
      if (!this.formEdit.remark) {
        this.$Message.info(`请填写备注！`)
        return false
      }
      this.handleBtn = true
      this.swsApi
        .swsPost('CenterDocking/OrderDetail/CloseOrderDetail', {
          orderDetailId: this.formEdit.id,
          remark: this.formEdit.remark
        })
        .then(res => {
          if (res.data.success) {
            this.closeModal = false
            this.formEdit.remark = ''
            this.$Message.success(`关闭成功！`)
            this.getFormDetail()
          } else {
            this.$Message.error(`操作失败，${res.data.error}`)
          }
          this.handleBtn = false
        })
        .catch(e => {
          this.$Message.error(`请求失败，请稍后再试！`)
        })
    },
    splitOrder () {
      this.swsApi
        .swsGet('CenterDocking/OrderDetail/SplitOrder/' + this.formDetail.id)
        .then(res => {
          if (res.data.success) {
            this.$Message.success('拆分成功')
            this.hide()
          } else {
            this.$Message.error(`操作失败，请稍后再试！`)
          }
        })
        .catch(e => {
          this.$Message.error(`请求失败，请稍后再试！`)
        })
    },
    exportDetailTable () {
      let datas = JSON.parse(JSON.stringify(this.table_data)).map(res => {
        // 20200514 cy 处理导出数据中有英文逗号而导致英文逗号后面的数据换到下一列中的问题
        if (typeof res.manufacturer === 'string' && res.manufacturer.indexOf(',') !== -1) {
          res.manufacturer = res.manufacturer.replace(',', ' ')
        }
        return res
      })
      let params = {
        filename: `采购订单${this.formDetail.orderNo}明细表`,
        columns: this.table_base,
        data: datas.map(res => {
          if (res.id) {
            res.dataState = this.dataStateList[res.dataState]
            res.unreceivedQty = subtract(res.purchasePuantity, res.actualQty)
          } else {
            delete res.actualQty
          }
          return res
        })
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
      // cy 清空供应商sets
      this.supplierSets.clear()
      this.swsApi
        .swsGet(`CenterDocking/NewOrderDetail/${this.formDetail.id}`)
        .then(res => {
          if (res.data.success) {
            let { orderData, orderDetailData, orderApproveOutPuts } = res.data.result
            this.orderDetail = orderData
            this.orderApproveOutPuts = orderApproveOutPuts
            this.table_data = orderDetailData.map(res => {
              if (res.id !== null) {
                if (res.supplierName === null) {
                  res.supplierName = '无'
                } else {
                  this.supplierSets.add(res.supplierName)
                }
              }
              return res
            })
          } else {
            this.$Message.error(`采购单详情查询出错，请稍后再试！`)
          }
          this.loading = false
        })
        .catch(e => {
          this.loading = false
        })
    },
    handleEdit (row, index) {
      this.editIndex = index
      this.editMonthAverage = row.monthAverage || 0
      this.editCurrentInventory = row.currentInventory || 0
      this.editRemark = row.remark || ''
      this.editPurchPrice = row.purchPrice || 0
      this.editSumPrice = row.sumPrice || 0
      this.editSupplierId = row.supplierId || '0'
      this.editSalePrice = row.salePrice || 0
      this.editSpecifications = row.specifications || ''
    },
    handleSave (row, index) {
      let params = {
        id: row.id,
        monthAverage: this.editMonthAverage,
        currentInventory: this.editCurrentInventory,
        remark: this.editRemark,
        purchPrice: this.editPurchPrice,
        sumPrice: this.editSumPrice,
        supplierId: this.editSupplierId,
        salePrice: this.editSalePrice,
        specifications: this.editSpecifications
      }
      this.swsApi
        .swsPost('CenterDocking/OrderDetail/Update', params)
        .then(res => {
          if (res.data.success) {
            this.$Message.success(`调整成功！`)
            this.getFormDetail()
          } else {
            this.$Message.error(`操作失败，请稍后再试！`)
          }
          this.editIndex = -1
        })
        .catch(e => {
          this.editIndex = -1
        })
    },
    hide () {
      this.purchase_detail = false
      this.editIndex = -1
      // this.orderDetail = {}
      this.orderApproveOutPuts = []
      this.$emit('on-hide')
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
      // margin: 10px 0;
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
      line-height: 2;
      td {
        color: #666;
      }
      &::before,
      &::after {
        display: none !important;
      }
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
    & /deep/ .ivu-table .supplier-empty {
      color: red;
      font-size: 15px;
      font-weight: bold;
    }
  }
  span {
    &.error {
      color: #ed4014;
    }
    &.success {
      color: #19be6b;
    }
    &.default {
      color: #f90;
    }
  }
  .step-wrapper {
    padding-bottom: 10px;
  }
  .button-group {
    button + button {
      margin-left: 10px;
    }
  }
  .detail_table {
    flex: 1;
    // overflow: hidden;
  }
}
.red-color {
  color: red;
}
.emptyPrice {
  color: red;
  margin-left: 60px;
}
// cy 分割线
.ivu-divider-horizontal {
  margin: 10px 0;
}
</style>
