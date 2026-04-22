<template>
  <div class="statistical">
    <financial-template :tabData="tabData" @on-change="changeTab" @on-initial-hospital="getHospital">
      <div class="table-box" slot="content" ref="tables">
        <div class="table">
          <Form ref="formData" :label-width="75" :model="formData">
            <Row>
              <Col :lg="12" :md="12">
              <Row>
                <Col :lg="24" :md="24" v-show="[1,10,11,12,13,15].includes(tabCheckedId)">
                <FormItem label="机构选择">
                  <Select v-model="hospitalCheckedId" :multiple="![11,15].includes(tabCheckedId)" :max-tag-count="1" :max-tag-placeholder="maxTagPlaceholder" filterable clearable placeholder="请选择机构" @on-change="changeHospital" style="width: 222px">
                    <Option v-for="item in hospitalList" :key="item.dialysisId" :value="item.dialysisId">{{item.dialysisName}}</Option>
                  </Select>
                </FormItem>
                </Col>
                <Col :lg="12" :md="12">
                <FormItem label="日期范围">
                  <DatePicker :value="formData.date" @on-change="changeDate" type="daterange" :options="options"></DatePicker>
                </FormItem>
                </Col>
                <Col :lg="12" :md="12" v-show="[13].includes(tabCheckedId)">
                <FormItem label="统计方式">
                  <Space wrap>
                    <RadioGroup v-model="MonthModel" type="button" button-style="solid">
                      <Radio :label="1">按月统计</Radio>
                      <Radio :label="2">合并统计</Radio>
                    </RadioGroup>
                  </Space>
                </FormItem>
                </Col>
                <Col :lg="12" :md="12" v-show="[10].includes(tabCheckedId)">
                <FormItem label="物品类型">
                  <Select v-model="modelType" style="width:150px;">
                    <Option :value="1">药品</Option>
                    <Option :value="2">耗材</Option>
                    <Option :value="3">诊疗项目</Option>
                  </Select>
                </FormItem>
                </Col>
                <Col :lg="12" :md="12" v-show="[14].includes(tabCheckedId)">
                <FormItem label="统计方式">
                  <Select v-model="staType" style="width:150px;">
                    <Option :value="1">每日统计</Option>
                    <Option :value="2">综合统计</Option>
                  </Select>
                </FormItem>
                </Col>
              </Row>
              </Col>
              <Col :lg="12" :md="12" class="button">
              <FormItem>
                <!-- <Row type="flex" justify="end">
                    <Col> -->
                <Button v-permission="buttonRole.TJFX_DC" @click="getTableDataByKey()" type="primary">查询</Button>
                <Button @click="exportTable" type="info">导出</Button>
                <Button v-permission="buttonRole.TJFX_DY" @click="print" type="primary" ghost v-if="tabCheckedId!=15">打印</Button>
                <!-- </Col>
                  </Row> -->
              </FormItem>
              </Col>
            </Row>
          </Form>
          <p class="title">{{title}}</p>
          <Table v-show="![8,11].includes(tabCheckedId)" id="statistical_table" class="default-table" :data="tableData" border ref="table" :loading="tableLoading" :columns="columns" :row-class-name="rowClassName" :height="tableHeight"></Table>
          <div style="position:relative;" v-if="tabCheckedId===8">
            <Spin fix v-show="showCustomerTable">加载中...</Spin>
            <table id="customer_table">
              <tr>
                <td style="height: 60px;font-size: 20px;display: none;" v-bind:colspan="columnLength">{{`${this.title} - ${this.monthFirst}-${this.monthLast}`}}</td>
              </tr>
              <thead>
                <tr class="tablehead" v-for="theadDatals in patientOncePayColumns">
                  <td v-for="(val) in theadDatals" v-if="(val.split('#'))[0] && (val.split('#'))[0]!='empty'" v-bind:colspan="(val.split('#'))[1]" v-bind:rowspan="(val.split('#'))[2]">
                    {{val?(val.split('#'))[0]:1}}
                  </td>
                </tr>
              </thead>
              <tbody>
                <template v-for="(item) in customerTableData">
                  <tr>
                    <td :rowspan="item.perCapitaDialysisDetail.length">{{item.centerName}}</td>
                    <td v-for="val in item.perCapitaDialysisDetail[0]" v-html="val"></td>
                  </tr>
                  <tr v-for="(_item, _index) in item.perCapitaDialysisDetail" v-if="_index !== 0">
                    <td v-for="val in _item" v-html="val"></td>
                  </tr>
                </template>
                <!-- <tr v-if="customerText">
                  <td v-bind:colspan="columnLength">暂无数据</td>
                </tr> -->
              </tbody>
            </table>
          </div>
          <div style="position:relative;" v-if="tabCheckedId===11">
            <Spin fix v-show="showCustomerTable">加载中...</Spin>
            <table id="customer_table">
              <tr>
                <td style="height: 60px;font-size: 20px;display: none;" v-bind:colspan="columnLength">{{`${this.title} - ${this.monthFirst}-${this.monthLast}`}}</td>
              </tr>
              <thead>
                <tr class="tablehead" v-for="theadDatals in PatientDialysisColumns">
                  <td v-for="(val) in theadDatals" v-if="(val.split('#'))[0] && (val.split('#'))[0]!='empty'" v-bind:colspan="(val.split('#'))[1]" v-bind:rowspan="(val.split('#'))[2]">
                    {{val?(val.split('#'))[0]:1}}
                  </td>
                </tr>
              </thead>
              <tbody>
                <template v-for="(item) in customerTableData">
                  <tr>
                    <td v-for="(_item, _index) in item" v-html="_item"></td>
                  </tr>
                </template>
                <!-- <tr v-if="customerText">
                  <td colspan="9">暂无数据</td>
                </tr> -->
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </financial-template>
  </div>
</template>

<script>
import financialTemplate from "@/components/financial-template";
import { getCurrentMonthFirst, getNowDate } from "@/libs/tools.js";
import { mapMutations } from "vuex";
const BUTTONROLE = {
  TJFX_DY: "TJFX_DY",
  TJFX_DC: "TJFX_DC",
};
export default {
  name: "statistical",
  data() {
    return {
      staType: 1, //统计方式，营业额成本统计
      customerText: false, // 自定义表单 显示暂无数据
      modelType: 1, // 物品收入占比的物品查询类型
      MonthModel: 1, // 1按月统计 2 合并统计
      showCustomerTable: false, // 显示自定义表单
      columnLength: 0, // 自定义表头的长度

      hospitalCheckedId: ["0"],
      hospitalCheckedName: "",
      hospitalList: [],

      selectedBxType: 1, // cy 医保报销类型
      tableHeight: 0,
      buttonRole: BUTTONROLE,
      options: {
        disabledDate(date) {
          return date && date.valueOf() > Date.now();
        },
      },
      tableLoading: false,
      title: "",
      monthFirst: "",
      search: "",
      monthLast: "",
      tabData: [],
      tabCheckedId: 1,
      formData: {
        date: [],
      },
      tableData: [],
      columns: [],
      // 患者人数统计
      patienyColumns: [
        { title: "序号", key: "no", width: 70 },
        { title: "机构名称", key: "centerName" },
        { title: "全部患者人数", key: "total" },
        { title: "职工医保人数", key: "workerHealthCount" },
        { title: "居民医保人数", key: "residentsHealthCount" },
        { title: "新增患者人数", key: "newCount" },
        { title: "流失患者人数", key: "lossCount" },
        { title: "流失率", key: "turnoverRate" },
      ],
      // 医护人数统计
      docNurColumns: [
        { title: "序号", key: "no", width: 70 },
        { title: "机构名称", key: "centerName" },
        { title: "医生人员数", key: "doctorCount" },
        { title: "护士人员数", key: "rnurseCount" },
        { title: "其他人员数", key: "otherCount" },
        { title: "合计", key: "total" },
      ],
      // 血透机器数量统计
      dialysisMachineColumns: [
        { title: "序号", key: "no", width: 70 },
        { title: "机构名称", key: "centerName" },
        { title: "SWS-4000", key: "swS4000Count" },
        { title: "SWS-4000A", key: "swS4000ACount" },
        { title: "SWS-6000", key: "swS6000Count" },
        { title: "SWS-6000A", key: "swS6000ACount" },
        {
          title: "4000+6000",
          key: "total1",
          render: (h, params) => {
            return h("div", params.row.swS4000Count + params.row.swS6000Count);
          },
        },
        {
          title: "4000A+6000A",
          key: "total2",
          render: (h, params) => {
            return h(
              "div",
              params.row.swS4000ACount + params.row.swS6000ACount
            );
          },
        },
        { title: "合计", key: "total" },
      ],
      // 床位日均使用次数
      bedColumns: [
        { title: "序号", key: "no", width: 70 },
        { title: "机构名称", key: "centerName" },
        { title: "床位总数", key: "bedsCount" },
        { title: "透析总次数", key: "curePattern" },
        { title: "床位日均使用次数", key: "dayCout" },
        { title: "床位日均使用率", key: "usageRate" },
      ],
      // 患者人均月收费及成本统计表
      patientFeeColumns: [
        { title: "序号", key: "no", width: 70 },
        { title: "机构名称", key: "centerName" },
        { title: "耗材收入(元)", key: "materialCharge" },
        { title: "药品收入", key: "drugCharge" },
        { title: "诊疗收入", key: "treatmentCharge" },
        { title: "耗材成本", key: "materialEarnings" },
        { title: "药品成本", key: "drugEarnings" },
        { title: "诊疗成本", key: "treatmentEarnings" },
        {
          title: "合计",
          align: "center",
          children: [
            { title: "收入", key: "chargeTotal", align: "center" },
            { title: "成本", key: "earningsTotal", align: "center" },
          ],
        },
      ],
      // 医护人员人均月创收表
      docNurInColumns: [
        { title: "序号", key: "no", width: 70 },
        { title: "机构名称", key: "centerName" },
        { title: "总创收", key: "revenueTotal" },
        { title: "医护人员数量", key: "medicalCount" },
        { title: "人均创收", key: "perCapita" },
      ],
      // 医护人员人均月创利表
      docNurProColumns: [
        { title: "序号", key: "no", width: 70 },
        { title: "机构名称", key: "centerName" },
        { title: "总创收（万元）", key: "revenueTotal" },
        { title: "总成本（万元）", key: "earningsTotal" },
        { title: "医护人员数量", key: "medicalCount" },
        { title: "人均创利", key: "perProfit" },
      ],
      // 单台机器月均收入表
      MachInColumns: [
        { title: "序号", key: "no", width: 70 },
        { title: "机构名称", key: "centerName" },
        { title: "透析机总创收（万元）", key: "machineCapita" },
        { title: "透析机数量", key: "machineCount" },
        { title: "单台透析机创收", key: "machineCapita" },
        { title: "月透析次数", key: "curePattern" },
      ],
      // 医护人员利用率
      docNorUseRaColumns: [
        { title: "序号", key: "no", width: 70 },
        { title: "机构名称", key: "centerName" },
        { title: "医护数量", key: "machineCount" },
        { title: "透析次数", key: "curePattern" },
        { title: "理论工作时间", key: "machineCapita" },
        { title: "透析工作时间", key: "revenueTotal" },
        { title: "饱和度", key: "saturation" },
      ],
      //透析模式、例次统计
      modelColumns: [
        { title: "患者姓名", key: "patientName", width: 80 },
        { title: "处方单号", key: "recipelNo" },
        { title: "结算单号", key: "balanceNo" },
        { title: "项目名称", key: "itemName" },
        { title: "规格", key: "specifications", width: 120 },
        { title: "数量", key: "qty", width: 60 },
        { title: "销售单价", key: "unitPrice" },
        { title: "销售总价", key: "totalPrice" },
        {
          title: "结算时间",
          key: "balanceDate",
          // render:(h,params)=>{
          //   return h('div',params.row.balanceDate.substring(0,10)+' '+params.row.balanceDate.substring(11,19))
          // }
        },
        {
          title: "项目开立时间",
          key: "prescriptionDetailFounderDate",
          // render:(h,params)=>{
          //   return h('div',params.row.prescriptionDetailFounderDate.substring(0,10)+' '+params.row.prescriptionDetailFounderDate.substring(11,19))
          // }
        },
        { title: "结算类别", key: "balanceType", width: 80 },
        { title: "透析模式", key: "dialysisType", width: 80 },
        { title: "例次", key: "dialysisQty", width: 60 },
      ],
      customerTableData: [],
      // cy 患者人均每例收费及成本
      patientOncePayColumns: [
        {
          projectName: "项目#2#2",
          HD: "HD#2#1",
          HDF: "HDF#2#1",
          HFHD: "HFHD#2#1",
          "HD+HP": "HD+HP#2#1",
          SUCF: "SUCF#2#1",
          // 'average': '平均#2#1',
          hdCost: "",
          hdIncome: "",
          hdfCost: "",
          hdfIncome: "",
          hfhdfCost: "",
          hfhdfIncome: "",
          hdhpfCost: "",
          hdhpfIncome: "",
          sucffCost: "",
          sucffIncome: "",
        },
        {
          projectName: "empty",
          HD: "empty",
          HDF: "empty",
          HFHD: "empty",
          "HD+HP": "empty",
          SUCF: "empty",
          // 'average': 'empty',
          hdCost: "成本",
          hdIncome: "收入",
          hdfCost: "成本",
          hdfIncome: "收入",
          hfhdfCost: "成本",
          hfhdfIncome: "收入",
          hdhpfCost: "成本",
          hdhpfIncome: "收入",
          sucffCost: "成本",
          sucffIncome: "收入",
          // 'averageCost': '成本平均',
          // 'averageIncome': '收入平均'
        },
      ],
      PatientDialysisColumns: [
        {
          no: "序号#1#2",
          prantName: "姓名#1#2",
          dialysisType: "透析方式#6#1",
          dt: "empty",
          gt: "empty",
          HDF: "empty",
          "HD+HP": "empty",
          SCUF: "empty",
          HF: "empty",
          monthCount: "月合计次数#1#2",
        },
        {
          no: "empty",
          prantName: "empty",
          dialysisType: "empty",
          dt: "低通",
          gt: "高通",
          HDF: "HDF",
          "HD+HP": "HD+HP",
          SCUF: "SCUF",
          HF: "HF",
          monthCount: "empty",
        },
      ],
      exportColumns: [],
      printDatas: {
        api: "",
        title: "",
        columns: [],
        params: {},
      },
    };
  },
  created() {
    this.loadTabData().then((_) => {
      let d = getNowDate().substring(0, 10);
      this.formData.date.push(getCurrentMonthFirst(), d);
      this.monthFirst = getCurrentMonthFirst();
      this.monthLast = d;
      this.changeTab(this.tabCheckedId);
    });
  },
  mounted() {
    this.$nextTick((_) => {
      this.tableHeight = this.$refs.tables.clientHeight - 115;
    });
  },
  computed: {
    // 接口，表格汇总
    UISettings() {
      return {
        1: {
          columns: [],
          actionCode: "BusinessTargetClntroller/BusinessTarget/GetSIRatioAsync",
        },
        2: {
          columns: this.patienyColumns,
          actionCode: "BusinessTargetClntroller/BusinessTarget/PatientsReport",
        },
        3: {
          columns: this.docNurColumns,
          actionCode: "BusinessTargetClntroller/BusinessTarget/Employee",
        },
        4: {
          columns: this.dialysisMachineColumns,
          actionCode: "BusinessTargetClntroller/BusinessTarget/Equipment",
        },
        5: {
          columns: [],
          actionCode:
            "BusinessTargetClntroller/BusinessTarget/CurePatternCheck",
        },
        6: {
          columns: this.bedColumns,
          actionCode: "BusinessTargetClntroller/BusinessTarget/BedsUseReport",
        },
        7: {
          columns: this.patientFeeColumns,
          actionCode: "BusinessTargetClntroller/BusinessTarget/PatientSaverage",
        },
        8: {
          columns: [],
          actionCode:
            "BusinessTargetClntroller/BusinessTarget/PerCapitaDialysis",
        },
        9: {
          columns: this.docNorUseRaColumns,
          actionCode:
            "BusinessTargetClntroller/BusinessTarget/SaturatedMedical",
        },
        10: {
          columns: [],
          actionCode:
            "BusinessTargetClntroller/BusinessTarget/GetItemSIRatioAsync",
        },
        11: {
          columns: [],
          actionCode:
            "BusinessTargetClntroller/BusinessTarget/GetPatientDialysis",
        },
        12: {
          columns: [],
          actionCode:
            "BusinessTargetClntroller/BusinessTarget/MedicalProportion",
        },
        13: {
          columns: [],
          actionCode: "BusinessTargetClntroller/BusinessTarget/ChargeCases",
        },
        14: {
          columns: [],
          actionCode: "BusinessTargetClntroller/BusinessTarget/IncomeAdnCost",
        },
        15: {
          columns: this.modelColumns,
          actionCode: "BusinessTargetClntroller/BusinessTarget/SIDealQuery",
        },
      };
    },
    exportAndPrintTitle() {
      if (this.tabCheckedId !== 7) {
        // if ([1, 2, 3, 4, 5, 6].includes(this.tabCheckedId)) {
        return `${this.title}（${this.monthFirst}~${this.monthLast}）`;
      } else {
        return this.title;
      }
    },
  },
  methods: {
    ...mapMutations(["setColumns", "setTitle", "setParams", "setApi"]),
    // 获取机构
    getHospital(hospitalList) {
      this.hospitalList = hospitalList;
      this.hospitalCheckedId.push(hospitalList[0].dialysisId);
    },
    // 切换医院
    changeHospital(id) {
      // console.log(id)
      if (this.tabCheckedId !== 11) {
        // cy 多选只有一个时则需要获取该机构名称
        if (id.length === 1) {
          this.hospitalCheckedName = this.hospitalList.filter(
            (v) => v.dialysisId === id[0]
          )[0].dialysisName;
        }
        // cy 如果多选了全选+其他机构，则去掉全部
        if (id.length > 1 && id.includes("0")) {
          id.splice(id.indexOf("0"), 1);
        }
      }
      this.hospitalCheckedId = id;
    },
    maxTagPlaceholder(num) {
      return `+${num}机构`;
    },
    // 医保报销类型选择
    changeBxType() {},
    // 加载表格名字
    loadTabData() {
      return this.swsApi
        .swsPost(`BusinessTargetClntroller/BusinessTarget/1/2`)
        .then((res) => {
          if (res.data.success) {
            this.tabData = res.data.result;
          } else {
            this.tabData = [];
          }
        })
        .catch((e) => {});
    },
    // 选择第一个表格日期
    changeDate(e) {
      if (e[0]) {
        this.monthFirst = e[0];
        this.monthLast = e[1];
        this.formData.date = e;
        // this.changeTab(this.tabCheckedId)
      }
    },
    // 点击表格类别
    changeTab(index) {
      console.log(index, "index");
      this.hospitalCheckedId = index !== 11 ? ["0"] : "0";
      if (index == 15) this.hospitalCheckedId = "0";
      this.customerText = false;
      this.customerTableData = [];
      this.columns = [];
      this.tableData = [];
      this.tabCheckedId = index;
      this.title = this.tabData[index - 1].childValue;
      // this.getTableDataByKey(index)
    },
    getTableDataByKey(index) {
      let id = index || this.tabCheckedId;
      let jsonStr = {};
      let baseJsonStr = {
        beginReportDate: this.monthFirst,
        endReportDate: this.monthLast,
      };
      // cy 医保报销表单增加 类型筛选
      if (id === 1) {
        jsonStr = {
          selectedBxType: this.selectedBxType,
          centerId: this.hospitalCheckedId,
          beginTime: this.monthFirst,
          endTime: this.monthLast,
          model: 0,
        };
      } else if (id === 10) {
        jsonStr = {
          centerId: this.hospitalCheckedId,
          beginTime: this.monthFirst,
          endTime: this.monthLast,
          model: this.modelType,
        };
      } else if ([11, 12, 15].includes(id)) {
        jsonStr = {
          centerId: this.hospitalCheckedId,
          beginTime: this.monthFirst,
          endTime: this.monthLast,
        };
      } else if ([13].includes(id)) {
        jsonStr = {
          centerId: this.hospitalCheckedId,
          beginTime: this.monthFirst,
          endTime: this.monthLast,
          MonthModel: this.MonthModel,
        };
      } else if (id === 14) {
        jsonStr = {
          ...baseJsonStr,
          staType: this.staType,
        };
      } else {
        jsonStr = baseJsonStr;
      }
      let { actionCode, columns } = this.UISettings[id];
      this.columns = columns;
      this.loadTable(actionCode, jsonStr, id);
    },
    // 处理多表头：
    // 1、去掉空children，column如有 空的 children 会报错
    // 2、add minWidth
    handleColumns(data) {
      if (!data.length) return [];
      for (let item of data) {
        item["minWidth"] = 70;
        if (item.key === null) {
          delete item.key;
        }
        if (item.children == null || !item.children.length) {
          delete item.children;
        } else item.children = this.handleColumns(item.children);
      }
      return data;
    },
    // 加载表格
    loadTable(actionCode, jsonStr, index) {
      this.tableLoading = true;
      if (this.tabCheckedId === 8) this.showCustomerTable = true;
      this.printDatas.api = actionCode;
      this.printDatas.params = JSON.parse(JSON.stringify(jsonStr));
      this.swsApi
        .swsPostCouldCancel(`${actionCode}`, jsonStr)
        .then((res) => {
          // console.log(res);
          if (res.data.success) {
            this.customerText = res.data.result.length === 0;
            if ([1, 5, 10, 12, 13, 14].includes(index)) {
              let data = [...res.data.result.tableHeaders];
              // 处理表头
              data = this.handleColumns(data);
              // this.columns = JSON.parse(JSON.stringify(data))
              this.columns = [...data];
              if ([1, 5, 10, 13, 14].includes(index)) {
                this.tableData = res.data.result.siRatioOutPuts;
              }
              // else if (index === 5) {
              //   this.tableData = res.data.result.curePatternDatas
              // }
              else if (index === 12) {
                this.tableData = res.data.result.centerDayIncomeOutPuts;
              }
            } else if (index === 8) {
              this.customerTableData = res.data.result;
              if (
                this.customerTableData.length !== 0 &&
                this.customerTableData[0].perCapitaDialysisDetail.length !== 0
              ) {
                this.columnLength =
                  Object.keys(
                    this.customerTableData[0].perCapitaDialysisDetail[0]
                  ).length + 1;
              }
              this.showCustomerTable = false;
            } else if (index === 11) {
              this.customerTableData = res.data.result;
            } else if (index === 15) {
              console.log(111);
              for (let item of res.data.result) {
                item.balanceDate =
                  item.balanceDate.substring(0, 10) +
                  " " +
                  item.balanceDate.substring(11, 19);
                item.prescriptionDetailFounderDate =
                  item.prescriptionDetailFounderDate.substring(0, 10) +
                  " " +
                  item.prescriptionDetailFounderDate.substring(11, 19);
              }
              this.tableData = res.data.result;
            } else {
              this.tableData = res.data.result;
            }
          } else {
            this.$Message.warning(`提示：后台错误（${res.data.error}）`);
          }
          this.tableLoading = false;
        })
        .catch((e) => {
          this.tableLoading = false;
          this.$Message.error(`请求错误:（${e}）`);
          // this.showCustomerTable = false
        });
    },
    // 表格合计行加粗
    rowClassName(row, index) {
      if (row.centerName === "合计" || row.itemName === "合计") {
        return "total";
      }
    },
    // 打印
    print() {
      let url = window.location.href.split("#")[0];
      // 缓存打印数据
      this.printDatas = Object.assign(this.printDatas, {
        columns: JSON.parse(JSON.stringify(this.columns)),
        title: this.exportAndPrintTitle,
      });

      const { api, columns, title, params } = this.printDatas;
      this.setTitle(title);
      this.setApi(api);
      this.setColumns(columns);
      this.setParams(params);

      window.open(`${url}#/print/statistic_analysis`);
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
    // 导出
    exportTable() {
      // cy 多表头有不同的导出方式
      if ([1, 5, 7, 8, 11, 13, 14].includes(this.tabCheckedId)) {
        let tableId = [8, 11].includes(this.tabCheckedId)
          ? "customer_table"
          : "statistical_table";
        let name = this.exportAndPrintTitle;
        let Obj = document.getElementById(`#${tableId}`);
        // if()
        tableExport(tableId, name, "xlsx");
      } else {
        this.$refs.table.exportCsv({
          filename: this.exportAndPrintTitle,
          columns: this.columns,
          data: this.tableData,
        });
      }
    },
  },
  components: {
    financialTemplate,
  },
};
</script>

<style lang="less" scope="scoped">
.statistical {
  height: 100%;
  .table-box {
    background: #fff;
    height: 100%;
    .title {
      text-align: center;
      font-size: 18px;
      color: #515a6e;
      line-height: 37px;
    }
  }
  form {
    padding-top: 14px;
  }
  .ivu-form .ivu-form-item-label {
    color: #a3a3a3;
    font-size: 14px;
  }
  .ivu-form .ivu-form-item {
    margin-bottom: 10px;
  }
  .table {
    padding: 0 10px;
    padding-bottom: 20px;
    background: #ffffff;
  }
  .time {
    font-size: 13px;
    color: #333333;
    margin-bottom: 10px;
  }
  .ivu-table .total td {
    font-weight: bold;
  }
  .button {
    text-align: right !important;
    button {
      margin-left: 10px;
    }
  }
  #customer_table {
    border-color: #e8eaec;
    width: 100%;
    border-collapse: collapse;
    margin-bottom: 10px;
    thead {
      background-color: #f8f8f9;
    }
    td {
      text-align: center;
      border: 1px solid #e8eaec;
      padding: 10px 20px;
    }
  }
}
</style>
