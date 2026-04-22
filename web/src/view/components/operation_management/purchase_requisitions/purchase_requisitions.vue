<template>
  <div id="purchase_requisition" :class="{ overflow : detailShowFlage}">
    <div class="btn-groups">
      <span>透析中心</span>
      <Select v-model="hospitalCheckedId" @on-change="hostipalChange" placeholder="请选择" filterable style="width: 200px;">
        <Option value="0">全部</Option>
        <Option v-for="item in hospitalList" :key="item.id" :value="item.id">{{item.dialysisName}}</Option>
      </Select>
      <span>供应商</span>
      <Select transfer v-model="supplierChecked" placeholder="请选择供应商" style="width: 180px;" @on-change="changeSupplier" filterable>
        <Option value="0">全部</Option>
        <Option :value="item.id" v-for="item in Supplier" :key="item.id">{{item.name}}</Option>
      </Select>
      <span>物品类型</span>
      <!-- <Select v-model="medicalItemType" @on-change="changeMedicalType" style="width: 120px;">
        <Option v-for="item in medicalItemList" :key="item.id" :value="item.id">{{item.txt}}</Option>
      </Select> -->
      <Cascader style="width:170px;display: inline-table;" :data="catalogList" @on-change="cascaderChange" trigger="hover" placeholder="请选择物品类型">
      </Cascader>
      <span>订单状态</span>
      <Select style="width: 100px;" placeholder="状态" v-model="orderStateSelected" @on-change="selectOrderState">
        <Option :value="0">全部</Option>
        <Option :value="item.index" v-for="item in orderStateList" :key="item.index">{{item.name}}</Option>
      </Select>
    </div>
    <div class="btn-groups">
      <span>时间范围</span>
      <DatePicker v-model="time" format="yyyy-MM-dd" type="daterange" placement="bottom-start" placeholder="请选择时间段" style="width: 200px;" @on-change="changeDate"></DatePicker>
      <span>关键词查询</span>
      <Input v-model="searchKey" @on-search="search" search enter-button placeholder="请输入关键字" style="width: 190px; display: inline-table;" />
      <Button type="primary" style="margin-left:10px;display: inline-table;" v-show="!multiFlag" @click="multiBtn">批量推送</Button>
    </div>
    <Divider></Divider>
    <div class="detail_table">
      <Table ref="purchaseRequestTable" :columns="table_columns" :data="table_data" :loading="loading" @on-select-all="handleSelectAll" @on-select-all-cancel="handleSelectAll" @on-select="handleSelectRow" @on-select-cancel="handleCancelRow">
        <template slot-scope="{ row, index }" slot="orderNo">
          <Input v-model="formEdit.orderNo" v-if="editIndex === index" />
          <span v-else>{{ row.orderNo }}</span>
        </template>
        <template slot-scope="{ row, index }" slot="applyDate">
          <DatePicker placeholder="请选择日期" type="date" v-model="formEdit.applyDate" v-if="editIndex === index" style="width: 140px"></DatePicker>
          <span v-else>{{ new Date(row.applyDate).toLocaleDateString().replace(/\//g, '-') }}</span>
        </template>
        <template slot-scope="{ row, index }" slot="applyMain">
          <Input v-model="formEdit.applyMain" v-if="editIndex === index"></Input>
          <span v-else>{{ row.applyMain }}</span>
        </template>
        <template slot-scope="{ row, index }" slot="remarks">
          <Input v-model="formEdit.remarks" v-if="editIndex === index"></Input>
          <span v-else>{{ row.remarks }}</span>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <!-- <template v-if="row.dataState===1"> -->
          <div v-if="editIndex === index" class="button-group">
            <Button type="primary" size="small" @click="handleSave(row, index)">保存</Button>
            <Button size="small" @click="editIndex = -1">取消</Button>
          </div>
          <div v-else>
            <Tooltip content="查看" placement="top" transfer>
              <Icon type="md-list" size="22" color="#4f95e8" @click="showDetail(row)" style="cursor: pointer"></Icon>
            </Tooltip>
            <!-- <span v-if="row.dataState===5 && row.founder === $store.state.user.userId">
              <Tooltip content="修改" placement="top" transfer v-permission="buttonRole.CGXQ_XG"> -->
            <span v-if="[5,8].includes(row.dataState) && row.founder === $store.state.user.userId">
              <Tooltip content="修改" placement="top" transfer>
                <Icon type="ios-create-outline" size="22" color="#4f95e8" @click="handleEdit(row, index)" style="cursor: pointer"></Icon>
              </Tooltip>
              <Tooltip content="删除" placement="top" transfer v-permission="buttonRole.CGXQ_SC">
                <Icon type="md-close" size="22" color="red" @click="handleDel(row)" style="cursor: pointer"></Icon>
              </Tooltip>
            </span>
            <span v-else>
              <Icon type="ios-create-outline" size="22" color="#c5c8ce" style="cursor: not-allowed"></Icon>
              <Icon type="md-close" size="22" color="c5c8ce" style="cursor: not-allowed"></Icon>
            </span>
          </div>
          <!-- </template> -->
        </template>
      </Table>
      <div class="multiCheck-btn-goups" v-show="multiFlag">
        <Button type="primary" style="margin-right:10px;" :loading="multiPushLoading" @click="pushBtn">确定推送</Button>
        <Button style="margin-left:10px;" type="default" @click="multiCheckCancel">取消</Button>
        <span style="margin-left:10px;">已经选中：{{selectedSum}} 项</span>
        <!-- <span style="margin-left:20px;color: red;">提示：合并采购单只能勾选未审批状态的采购单</span> -->
      </div>
      <div class="pagination" v-if="dataCount > pageSize">
        <Page :total="dataCount" :page-size="pageSize" :current.sync="startPage" @on-change="handleChangePage" />
      </div>
    </div>

    <Modal v-model="delModal" width="400" class-name="vertical-center-modal">
      <p slot="header">
        <span>删除订单</span>
      </p>
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;" />删除后不可恢复，您确定删除吗？
        </p>
      </div>
      <div slot="footer" class="center">
        <Button type="primary" @click="del">确定</Button>
        <Button type="default" @click="delModal=false">取消</Button>
      </div>
    </Modal>

    <!-- 关闭 -->
    <Modal v-model="closeModal" @on-cancel="formEdit.remark=''">
      <p slot="header" style="text-align: center;">确定要关闭该订单：{{formEdit.orderNo}} 吗？</p>
      <Input v-model="formEdit.remark" type="textarea" :rows="4" placeholder="请输入备注..." />
      <div slot="footer">
        <Button type="error" @click="handleClose" :loading="handleBtn">确认关闭</Button>
        <Button @click="closeModal=false;formEdit.remark='';handleBtn=false">取消</Button>
      </div>
    </Modal>

    <detail ref="detail" :form-detail="formDetail" @on-hide="afterHide" @on-save="getFormList" :supplier-list="Supplier"></detail>
  </div>
</template>

<script>
import detail from "./detail.vue";
import { getXMonthFirst, getNowDate } from "@/libs/tools.js";
const BUTTONROLE = {
  CGXQ_XG: "CGXQ_XG",
  CGXQ_ZTXG: "CGXQ_ZTXG",
  CGXQ_SC: "CGXQ_SC",
};
export default {
  name: "purchase_requisitions",
  data() {
    return {
      multiPushLoading: false,
      selectedIds: new Set(), // 选中的合并项
      selectedSum: 0,
      multiFlag: false, //批量勾选
      catalogList: [], // cy 档案目录list
      // selectedCataType: '',
      selectedCataId: "",
      selectedCataIds: [],

      orderStateSelected: 0, // 选择的订单状态
      orderStateList: [
        { index: 1, name: "未到货" },
        { index: 2, name: "已关闭" },
        { index: 3, name: "部分到货" },
        { index: 4, name: "已收货" },
        { index: 5, name: "未审批" },
        { index: 6, name: "上级待审" },
        { index: 7, name: "未定价" },
        { index: 8, name: "未推送" },
        { index: 9, name: "已拒绝" },
      ], // cy 订单状态list
      supplierChecked: "0", // cy 选中的供应商id
      Supplier: [], // cy 供应商list

      hospitalList: [],
      hospitalCheckedId: "0",
      time: [],
      handleBtn: false,
      delModal: false,
      closeModal: false,
      editIndex: -1,
      // 表格
      table_columns: [
        {
          title: "序列",
          key: "no",
          width: 50,
        },
        {
          title: "机构",
          key: "centerName",
          minWidth: 80,
        },
        {
          title: "订单号",
          // slot: 'orderNo',
          slot: "orderNo",
          minWidth: 115,
        },
        {
          title: "采购类型",
          key: "medicalItemTypeName",
          align: "center",
          minWidth: 60,
        },
        {
          title: "物品种类",
          key: "itemType",
          minWidth: 60,
          align: "center",
        },
        {
          title: "合计总数量",
          key: "totalQty",
          minWidth: 65,
          align: "center",
        },
        {
          title: "采购总金额",
          key: "actualPrice",
          align: "center",
          minWidth: 65,
        },
        {
          title: "申请时间",
          slot: "applyDate",
          minWidth: 80,
        },
        {
          title: "申请人",
          slot: "applyMain",
          minWidth: 65,
        },
        {
          title: "供应商",
          key: "supplierName",
          minWidth: 150,
        },
        {
          title: "备注",
          slot: "remarks",
          minWidth: 110,
          tooltip: true,
        },
        // {
        //   title: '推送状态',
        //   key: 'isPushCenter',
        //   width: 70,
        //   render: (h, params) => {
        //     if (params.row.isPushCenter) {
        //       return <i-button size="small" type="success">已推送</i-button>
        //     } else {
        //       return <i-button size="small" type="info">未推送</i-button>
        //     }
        //   }
        // },
        {
          title: "订单状态",
          width: 80,
          align: "center",
          render: (h, params) => {
            if (params.row.dataState === 1) {
              return (
                <i-button
                  size="small"
                  disabled={!this._filterButton(this.buttonRole.CGXQ_ZTXG)}
                  style="color: #fff;background-color: #3CADD9;border-color: #3CADD9;"
                  onClick={() => {
                    this.closeBtn(params.row);
                  }}
                >
                  未到货
                </i-button>
              );
            } else if (params.row.dataState === 2) {
              return (
                <i-button size="small" disabled>
                  已关闭
                </i-button>
              );
            } else if (params.row.dataState === 3) {
              return (
                <i-button
                  size="small"
                  style="color: #fff;background-color: #4FC0E8;border-color: #4FC0E8;"
                >
                  部分到货
                </i-button>
              );
            } else if (params.row.dataState === 4) {
              return (
                <i-button size="small" type="success">
                  已收货
                </i-button>
              );
            } else if (params.row.dataState === 5) {
              return (
                <i-button size="small" type="default">
                  未审批
                </i-button>
              );
            } else if (params.row.dataState === 6) {
              return (
                <i-button size="small" type="primary">
                  上级待审
                </i-button>
              );
            } else if (params.row.dataState === 7) {
              return (
                <i-button
                  size="small"
                  style="color: #fff;background-color: #FD8471;border-color: #FD8471;"
                >
                  未定价
                </i-button>
              );
            } else if (params.row.dataState === 8) {
              return (
                <i-button size="small" type="warning">
                  未推送
                </i-button>
              );
            } else if (params.row.dataState === 9) {
              return (
                <i-button size="small" type="error">
                  已拒绝
                </i-button>
              );
            }
          },
        },
        {
          title: "操作",
          slot: "action",
          fixed: "right",
          align: "center",
          width: 100,
        },
      ],
      table_data: [],
      dataCount: 0,
      loading: false,
      pageSize: 10,
      startPage: 1,

      // 清单id
      formDetail: "",
      // 时间
      beginTime: "",
      endTime: "",
      searchKey: "",
      medicalItemList: [
        {
          id: 0,
          txt: "全部",
        },
        {
          id: 1,
          txt: "药品",
        },
        {
          id: 2,
          txt: "耗材",
        },
        {
          id: 3,
          txt: "固定资产",
        },
        {
          id: 4,
          txt: "低值易耗品",
        },
      ],
      medicalItemType: 0,
      formEdit: {
        id: "",
        dataState: 1,
        orderNo: "",
        applyDate: "",
        remarks: "",
        applyMain: "",
      },
      // 详情页显示
      detailShowFlage: false,
      // 缓存当前scrollTop值
      scrollTop: 0,
      // 选中订单id
      checkedListId: -1,
      buttonRole: BUTTONROLE,
    };
  },
  created() {
    this.beginTime = getXMonthFirst(-3);
    this.endTime = getNowDate().substring(0, 10);
    this.time = [this.beginTime, this.endTime];
    this.swsApi
      .swsPost("Data/WarehouseCatalog/tree/0")
      .then((res) => {
        if (res.data.success) {
          let list = res.data.result.filter((item) => {
            return item.title !== "诊疗项目";
          });
          this.catalogList = list.map((res) => {
            if (res.title !== "固定资产") {
              return {
                value: res.id,
                label: res.title,
                index: `${res.cataIndex}`,
              };
            } else {
              let children = [];
              children = res.children.map((re) => {
                return { value: re.id, label: re.title };
              });
              return {
                value: res.id,
                label: res.title,
                index: `${res.cataIndex}`,
                children: children,
              };
            }
          });
          //
          this.catalogList.unshift({ value: "", label: "全部" });
        }
      })
      .catch((e) => {
        console.log(e);
      });
  },
  async mounted() {},
  mounted() {
    let args = {
      pageSize: 10000,
    };
    this.swsApi
      .swsPost("Data/Supplier/list", args)
      .then((res) => {
        if (res.data.result) {
          this.Supplier = res.data.result;
        } else {
          this.$Notice.error({
            title: "请求供应商错误",
            desc: "网络错误，请稍后再试",
          });
        }
      })
      .catch((e) => {});
    this.$nextTick(() => {
      this.getHospitalList();
      this.getFormList(1);
    });
  },
  methods: {
    pushBtn() {
      console.log(this.selectedIds);
      this.multiPushLoading = true;
      this.swsApi
        .swsPost("CenterDocking/NewPushCenter/BatchPush", {
          orderId: Array.from(this.selectedIds),
        })
        .then((res) => {
          if (res.data.success) {
            this.$Message.success(`批量推送成功！`);
            this.getFormList(1);
            this.multiCheckCancel();
          } else {
            this.$Message.warning(`批量推送异常:${res.data.error}`);
          }
          this.multiPushLoading = false;
        })
        .catch((e) => {
          this.multiPushLoading = false;
          this.$Message.error(`批量推送请求失败：${e}`);
        });
    },
    // cy 取消合并
    multiCheckCancel() {
      this.multiFlag = false;
      // this.$refs.purchaseTable.selectAll(false)
      // 清空ids集合
      this.clearSelectedIds();
      this.table_columns.shift();
    },
    clearSelectedIds() {
      // 清空ids集合
      this.selectedIds.clear();
      this.selectedSum = 0;
    },
    // cy 全选和取消全选时触发
    handleSelectAll(selection) {
      if (selection.length === 0) {
        // cy 若取消全选，删除保存在selectedIds里和当前table数据的id一致的数据，达到，当前页取消全选的效果
        // 当前页的table数据
        let data = this.$refs.purchaseRequestTable.data;
        data.forEach((item) => {
          if (this.selectedIds.has(item.id)) {
            this.selectedIds.delete(item.id);
          }
        });
      } else {
        selection.forEach((item) => {
          this.selectedIds.add(item.id);
        });
      }
      this.selectedSum = this.selectedIds.size;
    },
    // cy 选中某一行
    handleSelectRow(selection, row) {
      this.selectedIds.add(row.id);
      this.selectedSum = this.selectedIds.size;
    },
    // cy 取消某一行
    handleCancelRow(selection, row) {
      this.selectedIds.delete(row.id);
      this.selectedSum = this.selectedIds.size;
    },
    // cy 批量勾选,开启表单多选
    multiBtn() {
      this.multiFlag = true;
      this.table_columns.unshift({
        type: "selection",
        width: 32,
        align: "center",
      });
      this.setChecked();
    },
    // cy 给跨页丢失的选中行重新添加选中/禁用状态
    setChecked() {
      // 当前页的table数据
      let objData = this.$refs.purchaseRequestTable.objData;
      for (let index in objData) {
        // 初始化禁用、已勾选状态
        objData[index]._isDisabled = false;
        objData[index]._isChecked = false;
        // dataState:  8 未推送
        // cy 设置禁止勾选状态
        // cy dataState为8【未推送】状态才能 被勾选
        if (objData[index].dataState !== 8) {
          objData[index]._isDisabled = true;
        }
        // cy 根据保存的已勾选id来设置勾选状态
        if (this.selectedIds.has(objData[index].id)) {
          objData[index]._isChecked = true;
        }
      }
    },
    hostipalChange() {
      this.startPage = 1;
      this.getFormList(1);
    },
    getHospitalList() {
      this.swsApi
        .swsPost("CenterDialysis/DialysisList")
        .then((res) => {
          if (res.data.success) {
            this.hospitalList = res.data.result;
          }
        })
        .catch((e) => {
          console.log(e);
        });
    },
    // cy 物品类型change
    cascaderChange(ids, selectedData) {
      // let parentItem = selectedData[0]
      // this.selectedCataType = parentItem.value !== '' ? parentItem.index : '0'
      this.selectedCataIds = ids;
      this.selectedCataId = ids.pop() || "";
      this.getFormList();
    },
    selectOrderState() {
      this.getFormList();
    },
    changeSupplier() {
      this.getFormList();
    },
    closeBtn(row) {
      this.closeModal = true;
      this.formEdit.id = row.id;
      // this.formEdit.dataState = row.dataState
      this.formEdit.orderNo = row.orderNo;
    },
    // cy 关闭该订单
    handleClose() {
      if (!this.formEdit.remark) {
        this.$Message.info(`请填写备注！`);
        return false;
      }
      this.handleBtn = true;
      this.swsApi
        .swsPost("CenterDocking/OrderDetail/CloseOrder", {
          orderId: this.formEdit.id,
          remark: this.formEdit.remark,
        })
        .then((res) => {
          if (res.data.success) {
            this.closeModal = false;
            this.formEdit.remark = "";
            this.$Message.success(`关闭成功！`);
            this.getFormList();
          } else {
            this.$Message.error(`操作失败，请稍后再试！`);
          }
          this.handleBtn = false;
        })
        .catch((e) => {
          this.$Message.error(`请求失败，请稍后再试！`);
        });
    },
    // 修改保存
    handleSave() {
      this.swsApi
        .swsPost("CenterDocking/Order/Update", this.formEdit)
        .then((res) => {
          if (res.data.success) {
            this.$Message.success(`调整成功！`);
            this.getFormList();
          } else {
            this.$Message.error(`操作失败，请稍后再试！`);
          }
          this.editIndex = -1;
        })
        .catch((e) => {
          this.editIndex = -1;
        });
    },
    handleDel(row) {
      this.delModal = true;
      this.checkedListId = row.id;
    },
    // 获取申请单
    getFormList(pageNum) {
      pageNum = pageNum || 1;
      let params = {
        // medicalItemType: this.medicalItemType,
        supplierId: this.supplierChecked === "0" ? "" : this.supplierChecked,
        remarks: this.searchKey,
        beginApplyDate: this.beginTime,
        endApplyDate: this.endTime,
        pageSize: this.pageSize,
        pageNum: pageNum,
        orderState: this.orderStateSelected || 0,
        catalogue: this.selectedCataId || "",
        centerId: this.hospitalCheckedId || "",
      };
      this.loading = true;
      this.swsApi
        .swsPost("CenterDocking/Order/NewList", params)
        .then((res) => {
          if (res.data.success) {
            this.table_data = res.data.result;
            this.dataCount = res.data.dataCount;
          }
          this.loading = false;
        })
        .then(() => {
          // cy table数据赋值后再进行勾选状态还原
          if (this.multiFlag) {
            this.setChecked();
          }
        })
        .catch((e) => {});
    },
    showDetail(row) {
      this.formDetail = row;
      this.detailShowFlage = true;
      let dom = document.querySelector("#purchase_requisition");
      this.scrollTop = dom.scrollTop;
      this.$refs.detail.show();
      this.$nextTick(() => {
        dom.scrollTo(0, 0);
      });
    },
    handleChangePage(i) {
      this.getFormList(i);
    },
    // 编辑修改
    handleEdit(row, index) {
      let { orderNo, applyDate, remarks, applyMain, id } = row;
      this.editIndex = index;
      this.formEdit = {
        orderNo,
        applyDate,
        remarks,
        applyMain,
        id,
      };
      this.checkedListId = id;
    },
    changeMedicalType() {
      this.startPage = 1;
      this.getFormList(1);
    },
    search() {
      this.startPage = 1;
      this.getFormList(1);
    },
    changeDate(v) {
      if (!v) return;
      this.beginTime = v[0];
      this.endTime = v[1];

      this.startPage = 1;
      this.getFormList(this.startPage);
    },
    // 删除订单
    del() {
      this.delModal = false;
      this.swsApi
        .swsGet(`CenterDocking/DelOrder/${this.checkedListId}`)
        .then((res) => {
          if (res.data.success) {
            this.$Message.success("删除成功！");
            // this.startPage = 1
            this.getFormList(this.startPage);
          } else {
            this.$Message.error("删除失败，请稍后再试！");
          }
        });
    },
    afterHide() {
      let dom = document.querySelector("#purchase_requisition");
      dom.scrollTo(0, this.scrollTop);
      this.detailShowFlage = false;
      // this.startPage = 1
      this.getFormList(this.startPage);
    },
  },
  components: {
    detail,
  },
};
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
      margin: 0 10px;
      display: inline-block;
      // width: 60px;
      text-align: right;
    }
    & + .btn-groups {
      margin: 20px 0;
    }
    // & > * {
    //   margin-right: 20px;
    // }
    /deep/ .ivu-radio-group-button .ivu-radio-wrapper-checked {
      color: #ffffff;
      background: #4f95e8;
    }
  }

  .button-group {
    button + button {
      margin-left: 10px;
    }
  }
  .detail_table {
    margin-top: 20px;
    .multiCheck-btn-goups {
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
