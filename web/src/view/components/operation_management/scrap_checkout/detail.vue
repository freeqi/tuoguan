<template>
  <transition name="fade_enter">
    <div class="purchase_detail" v-show="purchase_detail">
      <div class="top">
        <div class="step-wrapper" ref="step">
          <Steps :current="currentStep" :status="currentStatus">
            <Step v-for="(item, index) in scrapProcessOutPuts" :key="index" :title="item.title" :content="`${item.content}${item.time ? '，时间：': ''}${item.time ? item.time : ''}`"></Step>
          </Steps>
        </div>
        <p class="record">
          <span style="margin-left:0;">
            报废出库单号: {{dataDetail.purchaseNo}}
            <!-- <template v-for="item in stateList">
              <span :class="item.color" v-if="item.id === dataDetail.groupAuditStatus" :key="item.id">{{item.label}}</span>
            </template> -->
          </span>
          <span class="right button-group">
            <Button v-show="(formDetail.groupAuditStatus === '2' || !formDetail.groupAuditStatus) && formDetail.warehouseId == '2543420c3132403098b85cad056c577e' && !formDetail.auditorLevel" type="default" class="approval" size="large" @click="valuationConfirmation" v-permission="buttonRole.BFCK_QRGJ">确认估价</Button>
            <Button type="default" class="approval" size="large" @click="showApproval" v-show="(formDetail.groupAuditStatus === '2' || !formDetail.groupAuditStatus) && !(formDetail.warehouseId == '2543420c3132403098b85cad056c577e' && !formDetail.auditorLevel)" v-permission="buttonRole.BFCK_SHENPI">审批</Button>
            <Button type="primary" size="large" v-permission="buttonRole.BFCK_DC" @click="exportDetailTable">导出</Button>
            <Button type="default" size="large" @click="hide" icon="md-undo">返回</Button>
          </span>
        </p>
        <p style="padding: 0px 0 8px;display: flex;">
          <span style="font-weight: 700;display: inline-block;min-width: 45px;">备注：</span>
          <span style="display: inline-block;">{{ dataDetail.remark }}</span>
        </p>
        <p class="items-box">
          <span class="field">物品种类：</span>
          <span>{{dataDetail.itemType || 0 }}</span>
          <span class="field">合计总数量：</span>
          <span>{{dataDetail.totalQty || 0}}件</span>
          <span class="field">合计成本金额：</span>
          <span class="red">￥{{computedTotalPrice || 0}}</span>
          <span class="field">合计估值：</span>
          <span class="red">￥{{dataDetail.appraiseCost || 0}}</span>
          <!-- <span class="field">销售总金额：</span>
          <span class="red">￥{{dataDetail.salesTotalPrice || 0}}</span> -->
        </p>
      </div>
      <Divider class="split-line"></Divider>
      <div class="detail_table" ref="detail_table">
        <Table :height="tableHeight" :columns="table_columns" :data="table_data" :loading="loading" ref="purchaseReqTable">

          <template slot-scope="{ row }" slot="salesReturnQty">
            <span :class="{'high_light': (row.id!==null && row.salesReturnQty !== row.ySalesReturnQty)}">{{ row.salesReturnQty }}</span>
          </template>

          <template slot-scope="{ row, index }" slot="action">
            <!-- <div v-if="editIndex === index" class="button-group">
              <Button type="primary" size="small" @click="handleSave(row, index)">保存</Button>
              <Button size="small" @click="editIndex = -1">取消</Button>
            </div> -->
            <!-- cy  <div v-else v-show="row.isUpdate"> -->
            <div v-if="row.id !== null && (!formDetail.groupAuditStatus || formDetail.groupAuditStatus === '2') && formDetail.warehouseId == '2543420c3132403098b85cad056c577e' && !formDetail.auditorLevel">
              <Tooltip content="修改" placement="top" transfer v-permission="buttonRole.BFCK_EDIT">
                <Icon type="md-create" size="22" color="#4f95e8" @click="handleEdit(row, index)" style="cursor: pointer;font-size: 18px;"></Icon>
              </Tooltip>
            </div>
          </template>
        </Table>
      </div>

      <!-- 审核 -->
      <Modal v-model="approvalModal" className="vertical-center-modal" width="400">
        <p slot="header" align="center">审核报废出库单</p>
        <Form :label-width="60" style="margin-bottom: 10px;">
          <FormItem label="审核状态">
            <Select v-model="approval.groupAuditStatus" style="width: 160px" placeholder="请选择是否同意">
              <Option value="3">同意</Option>
              <Option value="4">拒绝</Option>
            </Select>
          </FormItem>
          <FormItem label="审核意见" style="margin-bottom: 10px">
            <Input type="textarea" :rows="4" placeholder="请输入审核意见..." v-model="approval.groupAdvice" />
          </FormItem>
        </Form>
        <div slot="footer" class="center">
          <Button type="primary" v-show="!saveLoading" @click="saveApproval">审核</Button>
          <Button type="primary" v-show="saveLoading" :loading="true">审核中</Button>
          <Button type="default" @click="approvalModal = false">取消</Button>
        </div>
      </Modal>

      <!-- 修改报废出库数量 -->
      <Modal v-model="editQtyModal" className="vertical-center-modal" width="400">
        <p slot="header" align="center">修改净值估价</p>
        <Form :label-width="60" style="margin-bottom: 10px;">
          <FormItem label="净值估价">
            <InputNumber :min="0" v-model="editData.Appraise" style="width: 120px"></InputNumber>
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
import { accAdd } from "@/libs/tools";

const BUTTONROLE = {
  BFCK_SHENPI: "BFCK_SHENPI",
  BFCK_DC: "BFCK_DC",
  BFCK_EDIT: "BFCK_EDIT",
  BFCK_QRGJ: "BFCK_QRGJ",
};
export default {
  data() {
    return {
      computedTotalPrice: 0,
      dataDetail: [],
      scrapProcessOutPuts: [],
      editQtyModal: false,
      editData: {
        id: "",
        Appraise: 0,
      },
      // editIndex: -1,
      tableHeight: 0,
      purchase_detail: false,
      // 表格
      table_columns: [
        {
          title: "序列",
          key: "no",
          fixed: "left",
          width: 50,
        },
        {
          title: "物品名称",
          key: "medicalItemName",
          fixed: "left",
          minWidth: 100,
        },
        {
          title: "包装",
          key: "packaging",
          minWidth: 90,
        },
        {
          title: "规格",
          key: "specifications",
          minWidth: 90,
        },
        {
          title: "单位",
          key: "speUnitCHS",
          align: "center",
          minWidth: 70,
        },
        {
          title: "批次号",
          key: "batchNo",
          minWidth: 100,
        },
        {
          title: "申请数量",
          key: "materialQuantity",
          minWidth: 80,
          align: "center",
        },
        // {
        //   title: '审核数量',
        //   key: 'salesReturnQty',
        //   slot: 'salesReturnQty',
        //   minWidth: 80,
        //   align: 'center'
        // },
        {
          title: "成本单价",
          key: "unitPrice",
          align: "center",
          minWidth: 70,
        },
        {
          title: "总价",
          key: "costTotalPrice",
          align: "center",
          minWidth: 75,
        },
        {
          title: "净值估价",
          key: "appraise",
          align: "center",
          minWidth: 75,
        },
        {
          title: "生产日期",
          key: "productionDateStr",
          align: "center",
          width: 100,
          render: (h, params) => {
            if (params.row.productionDateStr == "/") return <span>/</span>;
            else
              return (
                <span>
                  {params.row.productionDateStr !== null &&
                    new Date(params.row.productionDateStr)
                      .toLocaleDateString()
                      .replace(/\//g, "-")}
                </span>
              );
          },
        },
        {
          title: "有效期至",
          key: "qualityDateStr",
          align: "center",
          width: 100,
          render: (h, params) => {
            if (params.row.qualityDateStr == "/") return <span>/</span>;
            else
              return (
                <span>
                  {params.row.qualityDateStr !== null &&
                    new Date(params.row.qualityDateStr)
                      .toLocaleDateString()
                      .replace(/\//g, "-")}
                </span>
              );
          },
        },
        {
          title: "创建时间",
          key: "founderDate",
          align: "center",
          width: 100,
          render: (h, params) => {
            return (
              <span>
                {params.row.founderDate !== null &&
                  new Date(params.row.founderDate)
                    .toLocaleDateString()
                    .replace(/\//g, "-")}
              </span>
            );
          },
        },
        {
          title: "供应商",
          key: "suppName",
          minWidth: 120,
        },
        {
          title: "厂家",
          key: "manufacturer",
          minWidth: 120,
        },
        {
          title: "备注",
          key: "remark",
          minWidth: 120,
        },
        {
          title: "操作",
          slot: "action",
          fixed: "right",
          align: "center",
          width: 60,
        },
      ],
      table_data: [],

      // 状态
      stateList: [
        {
          id: "2",
          label: "未审批",
          color: "default",
        },
        {
          id: "3",
          label: "已同意",
          color: "success",
        },
        {
          id: "4",
          label: "已拒绝",
          color: "error",
        },
        {
          id: "5",
          label: "已回退",
          color: "primary",
        },
        {
          id: "6",
          label: "上级待审",
          color: "success",
        },
        {
          id: "7",
          label: "暂 缓",
          color: "default",
        },
      ],
      loading: false,

      approvalModal: false,
      saveLoading: false,

      approval: {
        id: null,
        groupAuditStatus: "2",
        groupAdvice: "",
      },
      buttonRole: BUTTONROLE,
    };
  },
  props: {
    formDetail: {
      require: true,
      default: () => {
        return {};
      },
    },
  },
  watch: {
    formDetail() {
      this.$nextTick(() => {
        this.tableHeight = this.$refs.detail_table.clientHeight;
      });
    },
  },
  computed: {
    currentStep() {
      if (this.scrapProcessOutPuts.length > 0) {
        return this.scrapProcessOutPuts[0].current - 1;
      } else {
        return 0;
      }
    },
    currentStatus() {
      if (this.dataDetail.groupAuditStatus === "4") {
        return "error";
      } else {
        return "process";
      }
    },
  },
  methods: {
    valuationConfirmation() {
      this.$Modal.confirm({
        title: "提示",
        content: "<p>确定要进行确认估价操作吗？</p>",
        onOk: async () => {
          const arr = [];
          for (let i = 0; i < this.table_data.length; i++) {
            this.table_data[i].id &&
              arr.push({
                id: this.table_data[i].id,
                Appraise: this.table_data[i].appraise,
              });
          }

          const res = await this.swsApi.swsPost(
            "CenterDocking/AscertainFixedAssetsScrap",
            { id: this.dataDetail.id }
          );
          if (res.data.success) {
            this.$Message.success(`确认估价成功！`);
            this.purchase_detail = false;
            this.$emit("on-save");
            this.$emit("on-hide");
          } else {
            this.$Message.error(res.data.error);
          }
          this.editQtyModal = false;
          setTimeout(() => {
            this.saveLoading = false;
          }, 100);
          this.editQtyModal = false;
          this.saveLoading = false;
        },
      });
    },
    handleEdit(row, index) {
      this.editData.Appraise = row.appraise;
      this.editData.id = row.id;
      this.editQtyModal = true;
    },
    async submitQty() {
      this.saveLoading = true;
      const res = await this.swsApi.swsPost(
        "CenterDocking/FixedAssetsScrap",
        this.editData
      );
      if (res.data.success) {
        this.editQtyModal = false;
        this.getFormDetail();
      } else {
        this.$Message.error(`错误：${res.data.error}`);
      }
      this.saveLoading = false;
      this.saveLoading = false;
    },
    exportDetailTable() {
      // let datas = JSON.parse(JSON.stringify(this.table_data))
      let datas = JSON.parse(JSON.stringify(this.table_data)).map((res) => {
        // 20200514 cy 处理导出数据中有英文逗号而导致英文逗号后面的数据换到下一列中的问题
        if (
          typeof res.manufacturer === "string" &&
          res.manufacturer.indexOf(",") !== -1
        ) {
          res.manufacturer = res.manufacturer.replace(",", " ");
        }
        return res;
      });
      let columns = JSON.parse(JSON.stringify(this.table_columns));
      columns.pop();
      let params = {
        filename: `报废出库订单${this.formDetail.returnNo}明细表`,
        columns: columns,
        data: datas,
      };
      this.$refs.purchaseReqTable.exportCsv(params);
    },
    show() {
      this.purchase_detail = true;
      this.table_data = [];
      setTimeout(() => {
        this.getFormDetail();
      }, 300);
    },
    getFormDetail() {
      this.loading = true;
      this.computedTotalPrice = 0;
      this.swsApi
        .swsGet(`CenterDocking/GetmappDetails/${this.formDetail.id}`)
        .then((res) => {
          if (res.data.success) {
            let {
              orderApproveOutPuts,
              materialApplyApproveDetailOutPuts,
              approveOutPut,
            } = res.data.result;
            this.table_data = materialApplyApproveDetailOutPuts.map(
              (item, index) => {
                if (item.id)
                  this.computedTotalPrice = accAdd(
                    this.computedTotalPrice,
                    item.costTotalPrice
                  );
                item.no = index + 1;
                return item;
              }
            );
            this.scrapProcessOutPuts = orderApproveOutPuts;
            this.dataDetail = approveOutPut;
          } else {
            this.$Message.error(`报废出库单详情查询出错，请稍后再试！`);
          }
          this.loading = false;
        })
        .catch((e) => {
          this.loading = false;
          console.log(e);
        });
    },
    hide() {
      this.purchase_detail = false;
      this.dataDetail = {};
      this.scrapProcessOutPuts = [];
      this.$emit("on-hide");
    },
    showApproval() {
      this.approvalModal = true;
      this.approval.id = this.formDetail.id;
    },
    saveApproval() {
      if (!this.approval.groupAdvice.trim()) {
        this.$Message.error(`请输入审批意见！`);
        return false;
      }
      this.saveLoading = true;
      this.swsApi
        .swsPost("CenterDocking/MaterialApplyRequest", this.approval)
        .then((res) => {
          if (res.data.success) {
            this.$Message.success(`审批完成！`);
            this.purchase_detail = false;
            this.$emit("on-save");
            this.$emit("on-hide");
          } else {
            this.$Message.error(res.data.error);
          }
          this.approvalModal = false;
          setTimeout(() => {
            this.saveLoading = false;
          }, 100);
        })
        .catch((e) => {
          this.approvalModal = false;
          this.saveLoading = false;
          console.log(e);
        });
    },
  },
  components: {},
};
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
        content: "";
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
