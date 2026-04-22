<template>
  <div id="material">
    <financial-template :tabData="ListArr" @on-change="chooseArr" @on-initial-hospital="getHospital" ref="template">
      <template slot="content">
        <div class="chart">
          <div>
            <p>
              <span>
                机构
                <Select v-model="centerId" filterable placeholder multiple @on-change="changeHospitals" :max-tag-count="1" :max-tag-placeholder="maxTagPlaceholder" style="width: 222px;margin: 0 8px;">
                  <Option v-for="item in hospitalList" :key="item.dialysisId" :value="item.dialysisId">{{item.dialysisName}}</Option>
                </Select>
                <span v-if="![1,2,7,12].includes(recLi)">
                  筛选
                  <Input v-model="search" style="width: 130px;margin:0 8px;display:inline-table;" placeholder="搜索..." />
                </span>
                <span v-if="recLi === 4">
                  状态
                  <Select v-model="isSettlementFlag" style="width:120px;margin-left:8px;">
                    <Option :value="2">全部</Option>
                    <Option :value="0">未结算</Option>
                    <Option :value="1">已结算</Option>
                    <Option :value="3">未勾稽</Option>
                    <Option :value="4">已勾稽</Option>
                  </Select>
                </span>
                <span v-if="recLi === 7">
                  查询
                  <Input v-model="searchBackend" style="width: 130px;margin-left:8px;display:inline-table;" placeholder="搜索..." />
                </span>
                <span class="out">
                  <Button type="primary" :loading="searchBtnLoading" @click="chooseArr(recLi,true)">查询</Button>
                  <Button type="info" v-permission="buttonRole.WZBB_DC" @click="exportTab()">导出</Button>
                  <Button type="primary" ghost v-permission="buttonRole.WZBB_DY" @click="print">打印</Button>
                </span>
              </span>
              <span v-if="recLi === 12">
                物品
                <Input v-model="goodsSearchKey" style="width: 180px;margin-left:8px;" placeholder="请选择物品..." @on-focus="loadGoods" />
              </span>
            </p>
            <p class="operate-box">
              <span class="data-box">
                <span v-if="recLi !== 3">
                  日期
                  <DatePicker :value="dateData" type="daterange" placeholder="请选择日期" style="width: 180px;margin-left: 8px;" @on-change="chooseDateRange"></DatePicker>
                </span>
                <span v-else>
                  近效期时间段
                  <Select v-model="month" style="width:120px;margin-left:8px;">
                    <Option value="1">1个月</Option>
                    <Option value="3">3个月</Option>
                    <Option value="6">6个月</Option>
                  </Select>
                </span>
                <span style="margin:0 8px;" class="medical_type" v-show="recLi !== 12">
                  类别
                  <Select v-model="modelType1_M" style="width:150px;margin-left:8px;" @on-change="chooseType_M" :max-tag-count="1" multiple>
                    <Option :value="'0'">全部</Option>
                    <Option :value="'1'">药品</Option>
                    <Option :value="'2'">耗材</Option>
                    <Option :value="'3'" v-if="recLi!=3">固定资产</Option>
                    <Option :value="'4'" v-if="recLi!=3">低值易耗品</Option>
                  </Select>
                </span>
                <span v-show="[4,13,17].includes(recLi)">
                  供应商
                  <Select v-model="selectedSupplierId" filterable style="width:120px;margin: 0 8px;">
                    <Option v-for="item in supplierList" :value="item.id" :key="item.id">{{ item.name }}</Option>
                  </Select>
                </span>
              </span>
              <span class="out">
                <Button type="info" @click="multiSettleBtn(1)" v-permission="buttonRole.WZBB_PLGJ" v-show="[4,13].includes(this.recLi) && multiSettleFlag">批量勾稽</Button>
                <Button type="primary" @click="multiSettleBtn(2)" v-permission="buttonRole.WZBB_PLJS" v-show="[4,13].includes(this.recLi) && multiSettleFlag">批量结算</Button>
              </span>
            </p>
            <div class="cont">
              <h2 class="title">{{hospitalCheckedName}}{{hTitle}}</h2>
              <div style="position:relative;">
                <Spin fix v-show="showTzTable && recLi==12">请先选择物品...</Spin>

                <!-- :span-method="handleSpan" -->
                <Table id="material_table" class="default-table" ref="Table" border highlight-row :columns="columnsTop" :data="filterData" style="margin-top: 10px;" :loading="tabLoad" :height="tableHeight" size="small" @on-row-dblclick="getTableDetailData" @on-selection-change="selectChange" :row-class-name="rowClassName"></Table>
                <div class="settle-btn-goups" v-show="!multiSettleFlag">
                  <Button type="primary" style="margin-right:10px;" @click="settleOk">确定</Button>
                  <Button style="margin-left:10px;" type="default" @click="settleCancel">取消</Button>
                  <span style="margin-left:10px;">已选中：{{selectedSum}} 项</span>
                </div>
                <Page v-if="recLi==7" :total="dataCount" show-total style="float: right;margin-top: 10px;" @on-change="changePage" :page-size="pageSize" :current.sync="startPage" />
              </div>
            </div>
          </div>
        </div>
        <div style="padding: 0 15px 20px;background:#fff;" v-show="detailTableFlag">
          <div class="cont">
            <h2>{{hTitle}}单({{detailTableTitle}})明细</h2>
            <Table class="default-table" ref="detai_table" border size="small" height="300" :columns="columnsDetail" :data="detailTableData" :row-class-name="rowClassName" style="margin-top: 10px;"></Table>
          </div>
        </div>

        <!-- 加载所有物品 -->
        <Modal v-model="selectModal" width="800">
          <p slot="header" style="text-align: center;">选择项目</p>
          <Row>
            <Col span="24">
            <Input placeholder="检索..." v-model="modalSearchKey" ref="searchInput" />
            <Spin size="large" fix v-if="listShow || !goodsFilter"></Spin>
            <ul class="liList">
              <li v-for="item in goodsFilter" :key="item.id" @click="selectOneInfo(item)">
                <div>{{item.itemName}}</div>
              </li>
            </ul>
            </Col>
          </Row>
          <div slot="footer">
            <Button @click="selectModal=false;modalSearchKey=''">关闭</Button>
          </div>
        </Modal>

        <!-- 勾稽、结算modal +时间 -->
        <Modal v-model="settleModal" :title="`提示：确定要进行${handleIndex===1?'勾稽':'结算'}吗？`" width="380">
          <!-- <p slot="header" style="text-align: center;"></p> -->
          <Form ref="settleForm" :model="settleData" :rules="settleValidate" :label-width="80">
            <FormItem label="日期" prop="settlementDate">
              <DatePicker :value="settleData.settlementDate" @on-change="changeSettleDate" placeholder="请选择日期"></DatePicker>
            </FormItem>
          </Form>
          <div slot="footer">
            <Button @click="settleModal=false;">关闭</Button>
            <Button type="primary" @click="settleBtn">确定</Button>
          </div>
        </Modal>

        <!-- 勾稽、结算modal +时间 -->
        <Modal v-model="exportModal" title="导出" width="380">
          <p style="text-align: center;">若导出全部数据需再次发送请求，请耐心等待。</p>
          <div slot="footer">
            <Button @click="exportModal=false;">关闭</Button>
            <Button type="primary" :loading="tbBtnLoading" @click="toExportNormalTab(true)">导出全部</Button>
            <Button type="primary" :loading="tbBtnLoading" @click="toExportNormalTab()">导出当前数据</Button>
          </div>
        </Modal>
      </template>
    </financial-template>
  </div>
</template>

<script>
import financialTemplate from "@/components/financial-template";
import {
  toFilterKey,
  getNowDate,
  getCurrentMonthFirst,
  accAdd,
} from "@/libs/tools";
import { mapMutations } from "vuex";
import column from "./column";
const BUTTONROLE = {
  WZBB_DC: "WZBB_DC",
  WZBB_DY: "WZBB_DY",
  WZBB_PLGJ: "WZBB_PLGJ",
  WZBB_PLJS: "WZBB_PLJS",
};
export default {
  name: "material",
  components: {
    financialTemplate,
  },
  data() {
    return {
      isSetTotalCol: true, //是否设置最后一行为 合计项（独立展示）
      colCross: 3, //表示跨3行合并
      colStart: 1, //表示【跨行合并操作】从第一行开始

      searchBtnLoading: false,
      exportModal: false, //导出提示modal
      tbBtnLoading: false, // loading
      handleIndex: 0, // 批量操作的操作码
      isSettlementFlag: 2, // 结算状态
      selectedSupplierId: "0", // 供应商id
      supplierList: [], // 供应商列表
      selectedSum: 0,
      multiSettleFlag: true, // cy 批量结算按钮
      settleValidate: {
        settlementDate: [
          {
            required: true,
            type: "string",
            message: "请选择日期",
            trigger: "blur",
          },
        ],
      },
      settleModal: false, // cy 结算modal
      settleData: {
        // cy 结算数据
        id: [],
        settlementDate: "",
      },
      showTzTable: false, // cy 物品台账查询时未选择物品显示加载中的标识
      listShow: false,
      medicalItemId: "", // 选中的物品id
      modalSearchKey: "", // cy 搜索模态框的绑定关键词
      selectModal: false, // cy 加载物品
      goodsList: [], //
      goodsSearchKey: "", // cy 台账物品搜索的绑定关键词

      columnsDetail: [],
      detailTableTitle: "", // cy 明细表名（单号）
      detailTableData: [], // cy 明细table
      detailTableFlag: false, // cy 显示明细table
      tableHeight: 0,
      modelType1_M: ["1"],
      hospitalCheckedName: "",

      hTitle: "",
      houseName: "全部",
      ListArr: [],
      recLi: 0,
      hospitalList: [],
      tabLoad: false,
      columnsTop: [],
      dateData: [],
      tableData: [],
      searchBackend: "", // cy 后台查询
      search: "",
      startPage: 1,

      month: "1",
      centerId: ["0"],
      actionCode: "",
      pageSize: 15,
      pageIndex: 1,
      jsonStr: {},
      dataCount: 0,
      SPTabData: [],
      buttonRole: BUTTONROLE,
      printDatas: {
        api: "",
        title: "",
        columns: [],
        params: {},
      },
      typeList: {
        0: "全部",
        1: "药品",
        2: "耗材",
        3: "固定资产",
        4: "低值易耗品",
      },
    };
  },
  created() {
    let nowDate = getNowDate().substring(0, 10);
    this.dateData.push(getCurrentMonthFirst(), nowDate);
  },
  async mounted() {
    this.getSupplierList();
    await this.getList();
    await this.chooseArr();
    this.tableHeight = this.$refs.template.$el.clientHeight - 190;
  },
  mixins: [column],
  computed: {
    tabDataLen() {
      return this.tableData.length;
    },
    filterData() {
      console.log(this.recLi);
      let mdata = [];
      let tableData = [...this.tableData];
      if (!tableData.length) {
        return [];
      } else if (this.search) {
        mdata = toFilterKey(
          tableData,
          "medicalItemName,manufacturer,supplierName,inStorageNo,batchNo,outboundNo",
          this.search
        );
        if (this.recLi === 5 && tableData.length !== 0) {
          let costAmount = 0;
          let inQty = 0;
          let salesAmount = 0;
          mdata.forEach((res) => {
            costAmount = accAdd(costAmount, res.costAmount).toFixed(2);
            inQty = accAdd(inQty, res.inQty);
            salesAmount = accAdd(salesAmount, res.salesAmount).toFixed(2);
          });
          let amountData = {
            costAmount,
            inQty,
            salesAmount,
            rowNumber: "合计",
          };
          // console.log(amountData)
          mdata.push(amountData);
        } else {
          // console.log('asdasd')
          mdata = tableData;
        }
      } else {
        mdata = tableData;
      }
      if (this.recLi == 7) {
        let begin = (this.startPage - 1) * this.pageSize;
        return mdata.slice(begin, begin + this.pageSize);
      } else {
        return mdata;
      }
    },
    isChart1() {
      return this.recLi <= 5;
    },
    // 接口汇总
    UISetting() {
      return {
        1: {
          // 进销存汇总表
          columnsTop: this.columns1,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/EntersSellsSaves",
        },
        2: {
          // 入出库汇总表
          columnsTop: this.columns2,
          actionCode: "MaterialsStatistica/MaterialsStatistica/Confluence",
        },
        3: {
          // 近效期查询表
          columnsTop: this.columns3,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/RecentValidityStatistics",
        },
        4: {
          // 外购入库统计
          columnsTop: this.columns4,
          actionCode: "MaterialsStatistica/MaterialsStatistica/PutInStorage",
        },
        5: {
          // 外购入库明细
          columnsTop: this.columns5,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/PutInStorageDetail",
        },
        6: {
          // 划价出库统计
          columnsTop: this.columns6,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/AccuratelyOutbound",
        },
        7: {
          // 划价出库明细
          columnsTop: this.columns7,
          // actionCode: 'MaterialsStatistica/MaterialsStatistica/AccuratelyOutboundDetail'
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/AccuratelyOutboundDetailById",
        },
        8: {
          // 领用出库统计
          columnsTop: this.columns8,
          actionCode: "MaterialsStatistica/MaterialsStatistica/Recipients",
        },
        9: {
          // 领用出库明细
          columnsTop: this.columns9,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/RecipientsDetail",
        },
        10: {
          // 其他出库统计
          columnsTop: this.columns10,
          actionCode: "MaterialsStatistica/MaterialsStatistica/OtherOutbound",
        },
        11: {
          // 其他出库明细统计
          columnsTop: this.columns11,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/OtherOutboundDtail",
        },
        12: {
          // 物品台账
          columnsTop: this.columns12,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/ItemStandingBook",
        },
        13: {
          // 退货出库统计
          columnsTop: this.columns13,
          actionCode: "MaterialsStatistica/MaterialsStatistica/ReturnOutbound",
        },
        14: {
          // 退货出库明细
          columnsTop: this.columns14,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/ReturnOutboundDetail",
        },
        15: {
          // 报废出库统计
          columnsTop: this.columns15,
          actionCode: "MaterialsStatistica/MaterialsStatistica/ScrapOutbound",
        },
        16: {
          // 报废出库明细
          columnsTop: this.columns16,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/ScrapOutboundDetail",
        },
        17: {
          // 库存统计
          columnsTop: this.columns17,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/EntersSellsSavesBySupplier",
        },
        18: {
          // 库存统计
          columnsTop: this.columns18,
          actionCode: "MaterialsStatistica/MaterialsStatistica/ItemForthe",
        },
        19: {
          // 进销存汇总表（简）
          columnsTop: this.columns19,
          actionCode:
            "MaterialsStatistica/MaterialsStatistica/EntersSellsSavesSimplify",
        },
      };
    },
    // typeMultiple () {
    //   return [1, 2, 4, 5].includes(this.recLi)
    // },
    goodsFilter() {
      let data = [];
      if (this.goodsList) {
        data = toFilterKey(
          this.goodsList,
          "medicalItemName,aliasName,manufacturer,packaging,mnemonic",
          this.modalSearchKey
        );
      } else {
        data = [];
      }
      return data;
    },
  },
  watch: {
    recLi() {
      if (this.ListArr.length !== 0 && this.recLi) {
        this.hTitle = this.ListArr.filter(
          (item) => item.childKey === this.recLi
        )[0].childValue;
      }
    },
  },
  methods: {
    ...mapMutations(["setColumns", "setTitle", "setParams", "setApi"]),
    handleSpan({ row, column, rowIndex, columnIndex }) {
      //将index从0开始变为从1开始,方便进行取余计算
      return this.colCross != 1
        ? this.handleSpanData(rowIndex + 1, columnIndex + 1)
        : "";
    },
    handleSpanData(rowIndex, columnIndex) {
      if (this.isSetTotalCol) {
        //diff表示当前处理行与最后一行的差值
        let diff = this.tabDataLen - rowIndex;
        if (!diff) {
          //diff=0 表示当前行为最后一行，需要阻止【跨行合并操作】、【删除操作】
          return { rowspan: diff == 0 ? 1 : 0, colspan: diff == 0 ? 1 : 0 };
        } else if (
          rowIndex % this.colCross == 1 &&
          columnIndex == this.colStart
        ) {
          //当前行数%跨行数 等于1的 时候：进行【跨行合并操作】
          //同时不能让它跨行到最后一行
          return {
            rowspan: diff <= this.colCross - 1 ? diff : this.colCross,
            colspan: 1,
          };
        } else if (
          rowIndex % this.colCross != 1 &&
          columnIndex == this.colStart
        ) {
          //当前行数%跨行数 不等于1的 时候：进行【删除操作】
          //同时需要阻止【跨行合并操作】、【删除操作】
          return { rowspan: diff == 0 ? 1 : 0, colspan: diff == 0 ? 1 : 0 };
        }
      } else {
        if (rowIndex % this.colCross == 1 && columnIndex == this.colStart) {
          //当前行数%跨行数 等于1的 时候：进行【跨行合并操作】
          return { rowspan: this.colCross, colspan: 1 };
        } else if (
          rowIndex % this.colCross != 1 &&
          columnIndex == this.colStart
        ) {
          //当前行数%跨行数 不等于1的 时候：进行【删除操作】
          return { rowspan: 0, colspan: 0 };
        }
      }
    },
    getSupplierList() {
      this.swsApi
        .swsPost("Data/Supplier/list", { pageSize: 10000 })
        .then((res) => {
          if (res.data.success) {
            this.supplierList = [{ name: "全部", id: "0" }, ...res.data.result];
          } else {
            this.$Notice.error({
              title: "请求错误",
              desc: "获取供应商列表错误，请稍后再试",
            });
          }
        })
        .catch((e) => {
          this.$Notice.error({
            title: "请求错误",
            desc: "获取供应商列表错误，请稍后再试",
          });
        });
    },
    multiSettleBtn(index) {
      if (this.centerId[0] === "0" || this.centerId.length > 1) {
        return this.$Message.warning("批量操作时，请选择单个透析中心");
      }
      this.handleIndex = index;
      this.columnsTop.unshift({
        type: "selection",
        width: 32,
        align: "center",
        fixed: "left",
      });
      this.multiSettleFlag = false;
      this.initChecked(index);
    },
    initChecked(index) {
      // cy 清空id数据 和计数器
      this.settleData.id = [];
      this.selectedSum = 0;
      // 当前页的table数据
      let objData = JSON.parse(JSON.stringify(this.$refs.Table.objData));
      for (let i in objData) {
        // 初始化禁用、已勾选状态
        objData[i]._isDisabled = false;
        objData[i]._isChecked = false;
        if (objData[i].id === null) {
          objData[i]._isDisabled = true;
        } else if (index === 1) {
          // 勾稽
          // isChecked: 0未勾稽 1已勾稽
          // cy 合并统计行（id为空）、已勾稽行默认勾选状态为 禁止
          if (objData[i].isChecked === 1) {
            objData[i]._isDisabled = true;
          }
        } else if (index === 2) {
          // 结算
          // isSettlement: 0未结算 1已结算
          // cy 合并统计行（id为空）、还未勾稽、已结算行默认勾选状态为 禁止
          if (objData[i].isChecked === 0 || objData[i].isSettlement === 1) {
            objData[i]._isDisabled = true;
          }
        }
      }
      this.$refs.Table.objData = objData;
    },
    selectChange(selection) {
      this.settleData.id = selection.map((_) => _.id);
      this.selectedSum = this.settleData.id.length;
    },
    changeSettleDate(date) {
      this.settleData.settlementDate = date;
    },
    settleOk() {
      if (this.selectedSum < 2) {
        this.$Modal.warning({
          title: "提示",
          content: "<p>请至少选择两项进行批量操作！</p>",
        });
        return false;
      }
      this.settleModal = true;
    },
    settleCancel() {
      if (this.columnsTop[0].type === "selection") this.columnsTop.shift();
      this.multiSettleFlag = true;
    },
    settleBtn() {
      let api = "MaterialsStatistica/MaterialsStatistica/";
      if (this.recLi === 4) {
        api =
          this.handleIndex === 1
            ? api + "PutInStorageBillChecked"
            : api + "PutInStorageBillSettlement";
      } else if (this.recLi === 13) {
        api =
          this.handleIndex === 1
            ? api + "ReturnSettlementChecked"
            : api + "ReturnSettlement";
      }
      this.$refs.settleForm.validate((valid) => {
        if (valid) {
          this.settleBtnLoading = this.settleData.id;
          this.settleModal = false;
          this.swsApi
            .swsPost(api, this.settleData)
            .then((res) => {
              if (res.data.success) {
                this.$Message.success(
                  `${this.handleIndex === 1 ? "勾稽" : "结算"}操作成功`
                );
                this.chooseArr(this.recLi, true);
              } else {
                this.$Notice.error({
                  title: "请求失败",
                  desc: "请稍后再试",
                });
              }
            })
            .catch((e) => {
              this.$Notice.error({
                title: "请求错误",
                desc: "网络错误，请稍后再试," + e,
              });
            });
          this.settleBtnLoading = "";
        }
      });
    },
    // 表格合计行加粗
    rowClassName(row, index) {
      if (row.serialNumber === "合计" || row.rowNumber === "合计") {
        return "total";
      }
    },
    selectOneInfo(item) {
      this.selectModal = false;
      this.goodsSearchKey = item.medicalItemName;
      this.medicalItemId = item.id;
      this.chooseArr(this.recLi);
    },
    // cy 加载所有物品
    loadGoods() {
      let that = this;
      this.selectModal = true;
      let params = {
        pageSize: 9999,
        pageNum: 1,
      };
      // 查询物品
      if (!this.goodsList.length) {
        this.listShow = true;
        this.swsApi
          .swsPost("Data/MedicalItemRecord/list", params)
          .then(function (res) {
            if (res.data.success) {
              let arr = [];
              // 过滤非药品、耗材
              let result = res.data.result.filter((res) => {
                return [1, 2].includes(res.medicalItemType);
              });
              for (let i in result) {
                let item = {};
                // 物品名 + 别名 + 生产厂家 + 包装规格 +助记码
                item.itemName = `${result[i].medicalItemName} ${
                  result[i].brand ? "(" + result[i].brand + ")" : ""
                } ${
                  result[i].aliasName ? "(" + result[i].aliasName + ")" : ""
                } ${
                  result[i].manufacturer
                    ? "(" + result[i].manufacturer + ")"
                    : ""
                } ${result[i].packaging ? result[i].packaging : ""} [${
                  result[i].mnemonic
                }]`;
                item.id = result[i].id;
                // cy 用于模糊搜素
                item.medicalItemName = result[i].medicalItemName; // 物品名称
                item.aliasName = result[i].aliasName; // 别名
                item.manufacturer = result[i].manufacturer; // 生产厂家
                item.packaging = result[i].packaging; // 包装规格
                item.packageUnit = result[i].packageUnit; // 包装单位ID
                item.mnemonic = result[i].mnemonic; // 助记码
                item.medicalItemType = result[i].medicalItemType; // 类型
                arr.push(item);
              }
              that.goodsList = arr;
              that.listShow = false;
            }
          });
      } else {
        that.listShow = false;
      }
    },
    // 获取机构
    getHospital(hospitalList) {
      this.hospitalList = hospitalList;
      // this.centerId = hospitalList[0].dialysisId
    },
    maxTagPlaceholder(num) {
      return `+${num}机构`;
    },
    // 切换医院
    changeHospitals(ids) {
      if (ids.length === 0) return false;
      if (ids.length === 1 && ids[0] !== "0") {
        this.hospitalCheckedName = this.hospitalList.filter(
          (v) => v.dialysisId === ids[0]
        )[0].dialysisName;
      } else {
        this.hospitalCheckedName = "透析机构";
      }
      // cy 如果多选了全选+其他机构，则去掉全部
      if (ids.length > 1 && ids[0] === "0") {
        this.centerId.splice(ids.indexOf("0"), 1);
      }
      // 如果多选，再选中了全选，则清空ids，并显示全选
      if (ids.length > 1 && ids[ids.length - 1] === "0") {
        this.centerId = ["0"];
      }

      this.tableData = [];
      // _debounce(this.chooseArr, this.recLi, 500)
    },
    getList() {
      let that = this;
      return this.swsApi
        .swsPost("BusinessTargetClntroller/BusinessTarget/2/1")
        .then(function (response) {
          // 获取列表
          if (response.data.code === 200) {
            that.ListArr = response.data.result;
          }
        });
    },
    // 多选
    chooseType_M(value) {
      if (value.length === 0) return false;
      // cy 如果多选了全选+其他类型，则去掉全部
      if (value.length > 1 && value[0] === "0") {
        this.modelType1_M.splice(value.indexOf("0"), 1);
      }
      // 如果多选，再选中了全选，则清空ids，并显示全选
      if (value.length > 1 && value[value.length - 1] === "0") {
        this.modelType1_M = ["0"];
      }
      // _debounce(this.chooseArr, this.recLi, 1000)
    },
    // 近效期
    // chooseMonth () {
    //   this.chooseArr(this.recLi)
    // },
    getTableDetailData(row, index) {
      if (!row.id) {
        return false;
      }
      // cy 根据入库、出库单的id查询明细表单
      if ([4, 6, 8, 10, 13, 15].includes(this.recLi)) {
        this.detailTableTitle =
          this.recLi === 4 ? row.inStorageNo : row.outboundNo;
        this.detailTableFlag = true;
        let { columnsTop, actionCode } = this.UISetting[this.recLi + 1];
        this.columnsDetail = columnsTop;
        this.swsApi
          .swsPost(actionCode, { id: row.id })
          .then((res) => {
            if (res.data.success) {
              this.detailTableData = res.data.result;
              this.$nextTick(() => {
                let dom = document.querySelector(".financial-content .content");
                dom.scrollTo({
                  top: dom.scrollHeight,
                  left: 0,
                  behavior: "smooth",
                });
              });
            }
          })
          .catch((e) => {});
      }
    },
    changePage(value) {
      this.pageIndex = value;
      // this.jsonStr.pageIndex = value
      // this.getTabData()
    },
    chooseDateRange(date) {
      this.dateData = date;
      // this.chooseArr(this.recLi)
    },
    // 处理第一次进入页面时取消跟报表不相关的请求
    // chooseArr (id) {
    chooseArr(id = 1, flag = false) {
      if (!this.multiSettleFlag) this.settleCancel();
      this.recLi = id;
      this.pageIndex = 1;
      this.search = "";
      this.detailTableFlag = false;
      if (!this.ListArr.length) return;
      this.actionCode = "";
      this.columnsTop = [];
      this.tableData = [];
      this.jsonStr = {
        medicalItemType: this.modelType1_M.join(","), // 多选
        centerId: this.centerId,
        beginTime: this.dateData[0],
        endTime: this.dateData[1],
      };
      let { columnsTop, actionCode } = this.UISetting[this.recLi];
      this.columnsTop = columnsTop;
      this.actionCode = actionCode;
      // 3、7、12特殊需要
      if (this.recLi === 3) {
        this.jsonStr.month = this.month;
      } else if (this.recLi === 4) {
        this.jsonStr.supplierId = this.selectedSupplierId;
        this.jsonStr.isSettlement = this.isSettlementFlag;
      } else if (this.recLi === 7) {
        this.actionCode =
          "MaterialsStatistica/MaterialsStatistica/AccuratelyOutboundDetail";
        this.jsonStr.searchBackend = this.searchBackend;
        this.jsonStr.pageIndex = this.pageIndex;
        this.jsonStr.pageSize = this.pageSize;
      } else if (this.recLi === 12) {
        if (this.goodsSearchKey === "") {
          this.showTzTable = true;
          this.$Message.warning("请先选择物品");
          return false;
        } else {
          this.showTzTable = false;
        }
        this.jsonStr = {
          id: this.medicalItemId,
          centerId: this.centerId,
          beginTime: this.dateData[0],
          endTime: this.dateData[1],
        };
      } else if ([13, 17].includes(this.recLi)) {
        this.jsonStr.supplierId = this.selectedSupplierId;
      }
      this.printDatas = Object.assign(this.printDatas, {
        params: JSON.parse(JSON.stringify(this.jsonStr)),
        api: this.actionCode,
      });
      if (flag) this.getTabData();
      // _debounce(this.getTabData, id, 500)
    },
    // 请求表格数据
    getTabData() {
      this.searchBtnLoading = true;
      this.tabLoad = true;
      this.swsApi
        .swsPostCouldCancel(this.actionCode, this.jsonStr)
        .then((res) => {
          if (res.data.success) {
            this.tableData = res.data.result;
            this.dataCount = this.recLi === 7 ? res.data.dataCount : 0;
            if (!this.multiSettleFlag) {
              this.$nextTick(() => {
                this.initChecked();
              });
            }
          }
          this.tabLoad = false;
          this.searchBtnLoading = false;
        })
        .catch((e) => {
          if (!e.message.includes("取消上一次请求")) {
            this.tabLoad = false;
            this.searchBtnLoading = false;
          }
        });
    },
    exportTab() {
      if (this.filterData.length === 0) {
        this.$Message.warning("表单无数据");
        return false;
      }
      let filename = this.hospitalCheckedName + this.hTitle;
      if ([1, 12, 17].includes(this.recLi)) {
        tableExport("material_table", filename, "xlsx");
      } else {
        this.exportModal = true;
      }
    },
    async toExportNormalTab(exportAll = false) {
      let RawData = [];
      let tabData = [];
      let exportColumn = [];
      let filename = this.hospitalCheckedName + this.hTitle;
      let ColumnOne = {};
      let date = "";
      let houseName = "";
      // cy 目前只有报表7做了分页
      if (exportAll && this.recLi === 7) {
        let jsonStr = JSON.parse(JSON.stringify(this.jsonStr));
        jsonStr.pageSize = 9999;
        this.tbBtnLoading = true;
        await this.swsApi.swsPost(this.actionCode, jsonStr).then((res) => {
          if (res.data.success) {
            RawData = res.data.result;
          }
        });
      } else {
        RawData = JSON.parse(JSON.stringify(this.tableData));
      }

      exportColumn = this.handleMultiTableHeader(this.columnsTop, false);
      exportColumn.forEach((item) => {
        ColumnOne[item.key] = item.title;
      });
      if (this.dateData.length !== 0) {
        date = "日期：" + this.dateData[0] + "至" + this.dateData[1];
      }
      if (this.recLi === 3) {
        date = this.month ? `近效期：近${this.month}个月` : "";
      }
      houseName =
        "库房：" +
        this.modelType1_M.map((index) => this.typeList[index]).join("+");
      tabData = RawData.map((res) => {
        if (res.isSettlement !== null) {
          res.isSettlement = res.isSettlement ? "已结算" : "未结算";
        }
        // 20200514 cy 处理导出数据中有英文逗号而导致英文逗号后面的数据换到下一列中的问题
        if (
          typeof res.manufacturer === "string" &&
          res.manufacturer.indexOf(",") !== -1
        ) {
          res.manufacturer = res.manufacturer.replace(",", " ");
        }
        return res;
      });
      let tableTitle = {};
      // 模拟iview表单格式，将自定义的【表标题和表头】添加到数据里，再将noHeader配置为ture，
      tableTitle[
        `${exportColumn[0].key}`
      ] = `${filename},,${houseName},,${date},,`;
      tabData.unshift(tableTitle, ColumnOne);
      this.$refs.Table.exportCsv({
        filename: filename,
        columns: exportColumn,
        data: tabData.filter((data, index) => {
          for (let i in data) {
            // 解决日期格式问题
            if (typeof data[i] === "string" && data[i].indexOf("T") === 10) {
              data[i] =
                data[i].substring(0, 10) + " " + data[i].substring(11, 16);
            }
          }
          return index < tabData.length;
        }),
        noHeader: true,
      });
      this.tbBtnLoading = false;
      this.exportModal = false;
    },
    // cy 处理多表头导出问题
    handleMultiTableHeader(columns, noSubTitle) {
      noSubTitle = noSubTitle || false;
      return columns
        .map((res) => {
          if (res.hasOwnProperty("children")) {
            return res.children.map((item) => {
              let colData = Object.assign({}, item);
              colData.title = `${
                noSubTitle
                  ? ""
                  : `${res.title.substring(res.title.indexOf("_") + 1)}_`
              }${colData.title}`;
              return colData;
            });
          } else {
            return res;
          }
        })
        .flat();
    },
    print() {
      let url = window.location.href.split("#")[0];
      // 缓存打印数据
      this.printDatas = Object.assign(this.printDatas, {
        columns: JSON.parse(JSON.stringify(this.columnsTop)),
        // title: this.hospitalCheckedName + this.title
        title: this.hospitalCheckedName + this.hTitle,
      });
      const { api, columns, title, params } = this.printDatas;
      this.setTitle(title);
      this.setApi(api);
      this.setColumns(columns);
      this.setParams(params);
      window.open(`${url}#/print/material_statement`);
    },
  },
};
</script>

<style scoped lang="less">
#material {
  position: relative;
  width: 100%;
  height: 100%;
  font-size: 14px;
  .chart {
    height: 100%;
    padding: 20px 15px;
    background: #ffffff;
    position: relative;
  }
  .operate-box {
    & > span {
      display: inline-block;
      margin-top: 10px;
    }
  }
  /deep/ .ivu-table .total td {
    font-weight: bold;
  }
}
.inner-content .main .content {
  background: #ffffff !important;
}
.tab-panel {
  position: absolute;
  top: -32px;
  left: 0;
  font-size: 14px;
}
.out {
  float: right;
  button {
    margin-right: 10px;
  }
}
.cont {
  h2 {
    padding-top: 10px;
    text-align: center;
    font-weight: normal;
    font-size: 18px;
  }
  span {
    margin-right: 30px;
  }
  .settle-btn-goups {
    float: left;
    margin-top: 16px;
    .ivu-select {
      position: relative !important;
    }
  }
}
.liList {
  max-height: 450px;
  overflow-y: scroll;
  li {
    height: 30px;
    line-height: 30px;
    font-size: 12px;
    border-bottom: 1px solid #eaeaea;
    cursor: pointer;
    white-space: nowrap;
    text-overflow: ellipsis;
    overflow: hidden;
    i {
      color: red;
    }
  }
}
</style>
