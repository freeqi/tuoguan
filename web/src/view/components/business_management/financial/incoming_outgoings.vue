<template>
  <div id="incoming_outgoings">
    <financial-template :tabData="tabData" @on-change="changeTab" @on-initial-hospital="getHospital" ref="template">
      <div class="tab-content" slot="content">
        <div class="table">
          <div style="margin: 10px;">
            机构
            <Select v-show="!hasPage" v-model="hospitalCheckedIds" multiple :max-tag-count="1" :max-tag-placeholder="maxTagPlaceholder" filterable placeholder="请选择一或多个机构" @on-change="changeHospitals" style="width: 222px;margin-left:6px;">
              <Option v-for="item in hospitalList" :key="item.dialysisId" :value="item.dialysisId">{{item.dialysisName}}</Option>
            </Select>
            <Select v-show="hasPage" v-model="hospitalCheckedId" filterable clearable placeholder="请选择机构" @on-change="changeHospital" style="width: 187px;margin-left:6px;">
              <Option v-for="item in hospitalList.filter(item => item.dialysisId !== '0')" :key="item.dialysisId" :value="item.dialysisId">{{item.dialysisName}}</Option>
            </Select>
            <span style="padding: 0 19px;font-size: 18px;" v-show="tabCheckedId == 1">
              <RadioGroup v-model="typeCheckedId" @on-change="changeType" type="button">
                <Radio v-for="item of type" :key="item.index" :label="item.index">{{item.title}}</Radio>
              </RadioGroup>
            </span>
          </div>
          <div class="table-operate">
            <div class="button-group">
              <span>
                <span class="span_title">日期</span>
                <DatePicker :value="formData.date" @on-change="changeDate" type="daterange" :options="options" placeholder="请选择时间段"></DatePicker>
              </span>
              <span v-show="tabCheckedId != 8 && hasPage && tabCheckedId != 9">
                <span v-show="hasPage" class="search_box">
                  <span class="box_item">查询条件</span>
                  <Input v-model="searchKey" placeholder="请输入姓名或单号查询" />
                </span>
              </span>
              <!-- <span v-show="hasPage" class="search_box">
                <span class="box_item">查询条件</span>
                <Input
                  v-model="searchKey"
                  placeholder="请输入姓名或单号查询"/>
              </span> -->
            </div>
            <div class="operate-right">
              <Button @click="keySearch" type="primary">查询</Button>
              <Button @click="exportTable" type="info" :loading="tbDownLoading" v-permission="buttonRole.SZBB_DC">导出</Button>
              <Button @click="print('table')" type="primary" ghost v-permission="buttonRole.SZBB_DC">打印</Button>
            </div>
          </div>
          <div class="table_title">
            <p class="p" v-if="!hasPage">
              {{hospitalCheckedName}}{{title}}
              <span v-show="tabCheckedId === 1">（{{this.type[this.typeCheckedId-1].title}}）</span>
            </p>
            <p class="p" v-else>
              {{hospitalCheckedId === '0' ? '透析中心' : hospitalCheckedName}}{{title}}
              <span v-show="tabCheckedId ==2">（按收费项目）</span>
            </p>
          </div>
        </div>
        <div class="table">
          <Table id="table_box" ref="incoming_table" class="default-table" @on-row-dblclick="dbClick" :height="tableHeight" :highlight-row="[4,5].includes(tabCheckedId)" :data="tableData" :row-class-name="rowClassName" :loading="tableLoading" :columns="columns"></Table>
          <div class="pagination">
            <Page class="page" v-show="hasPage && tabCheckedId != 8 && tabCheckedId != 9" :total="dataCount" show-total :current.sync="current" :page-size="pageSize" @on-change="changePage"></Page>
          </div>
          <p class="p" v-show="detailTableFlag">{{detailTableCheckedInfo.name}}{{title}}({{detailTableCheckedInfo.prescriptionNo}})明细表</p>
          <Table v-show="detailTableFlag" class="default-table" ref="detailTable" height="300" :data="detailTableData" :row-class-name="rowClassName" :loading="detailTableLoading" :columns="detailTableColumns"></Table>
          <div v-show="isTab3">
            <div class="time">
              <span>总汇表 </span>
              <span>
                时间：
                <label v-show="isTab3">{{this.monthFirst}} 至 {{this.monthLast}}</label>
                <!-- <label v-show="isTab3">{{this.formData.singleDate}}</label> -->
              </span>
            </div>
            <Table id="other_table_box" ref="other_table" class="default-table" :height="tableHeight" :data="otherTableData" :row-class-name="rowClassName" :loading="tableLoading" :columns="otherColumns"></Table>
          </div>
          <!-- <div class="button">
              <Button @click="exportTable()" type="primary" v-permission="buttonRole.SZBB_DC">导出</Button>
              <Button @click="print" type="primary" ghost v-permission="buttonRole.SZBB_DC">打印</Button>
          </div>-->
        </div>
      </div>
    </financial-template>
  </div>
</template>

<script>
import financialTemplate from "@/components/financial-template";
import { mapMutations } from "vuex";
import {
  getCurrentMonthFirst,
  getNowDate,
  getNowFormatDate,
  convertCurrency,
  formateDateToString,
  // _debounce
} from "@/libs/tools.js";
const BUTTONROLE = {
  SZBB_DC: "SZBB_DC",
  SZBB_DY: "SZBB_DY",
};
export default {
  name: "incoming_outgoings",
  components: {
    financialTemplate,
  },
  data() {
    return {
      tbDownLoading: false, //
      searchKey: "", // cy 查询关键字

      detailTableColumns: [], // cy 双击指定table的行显示的明细表单
      detailTableData: [],
      detailTableFlag: false, // 是否显示明细表单
      detailTableCheckedInfo: {
        name: "",
        prescriptionNo: "",
      },
      detailTableLoading: false,
      // headerFlag: false
      tableHeight: 0, // table高度
      pageSize: 10,
      current: 1,
      options: {
        disabledDate(date) {
          return date && date.valueOf() > Date.now();
        },
      },
      type: [
        { title: "按收费项目", index: 1 },
        { title: "按结算方式", index: 2 },
        { title: "按回款方式", index: 3 },
      ],
      hospitalCheckedIds: ["0"],
      hospitalCheckedId: "0",
      hospitalCheckedName: "",
      hospitalList: [],
      data: {
        outPrice: 5000,
        price: 4000,
        name: "张三",
      },
      tabData: [],
      tabCheckedId: 1,
      typeCheckedId: 1,
      formData: {
        type: "1",
        date: [],
        singleDate: getNowFormatDate(false),
      },
      title: "收入总汇表",
      monthFirst: "",
      monthLast: "",
      columns: [],
      allColumns: [],
      otherColumns: [], // 存列表三 附表 表头
      otherTableData: [], // 列表三 收入日报表的 附表
      tableData: [],
      feeColumns: [
        { title: "序号", key: "no", width: 65, align: "center" },
        { title: "项目", key: "itemName", align: "center" },
        // { title: '门诊内科', key: 'totalMoney' },
        { title: "合计", key: "totalMoney", align: "center" },
      ],
      allFeeColumns: [
        { title: "透析中心", key: "itemName" },
        { title: "药品费", key: "yp" },
        { title: "检查费", key: "jc" },
        { title: "治疗费", key: "zl" },
        { title: "透析费", key: "tx" },
        { title: "护理费", key: "hl" },
        { title: "氧气费", key: "yq" },
        { title: "材料费", key: "cl" },
        { title: "其他费", key: "qt" },
        { title: "合计", key: "totalMoney" },
      ],
      allCostColumns: [
        { title: "透析中心", key: "itemName" },
        { title: "药品费", key: "yp" },
        { title: "耗材费", key: "cl" },
        { title: "透析费", key: "tx" },
        { title: "其他费", key: "qt" },
        { title: "合计", key: "totalMoney" },
      ],
      tableTotal: [],
      listColumns: [
        { title: "序号", key: "no", align: "center", width: 65 },
        { title: "姓名", key: "pName", width: 65 },
        { title: "性别", key: "sex", align: "center", width: 50 },
        { title: "年龄", key: "age", align: "center", width: 50 },
        { title: "医保类别", key: "healthCareType", width: 70 },
        { title: "类别", key: "mzType", align: "left", width: 75 },
        {
          title: "结算单号",
          key: "prescriptionNo",
          align: "center",
          minWidth: 110,
        },
        { title: "总计", key: "sumPrice", width: 90 },
        { title: "统筹", key: "detc", align: "center", width: 70 },
        { title: "大额", key: "ybjj", align: "center", width: 90 },
        { title: "民政救助", key: "mzjz", align: "center", width: 70 },
        { title: "公务员补助", key: "gwybz", width: 70 },
        { title: "企业补充基金", key: "hifes_pay", width: 75 },
        { title: "其他", key: "oth_pay", width: 70 },
        { title: "帐户支付", key: "zhzf", align: "center", width: 70 },
        { title: "现金", key: "xj", width: 90 },
        {
          title: "医院超标",
          key: "yycb",
          align: "center",
          minWidth: 70,
          render: (h, params) => {
            return (
              <div
                style={params.row.yycb > 0 ? "color:red;font-size:18px;" : ""}
              >
                {params.row.yycb}
              </div>
            );
          },
        },
        { title: "误差", key: "wx", width: 65 },
        {
          title: "退费时间",
          key: "cancelDate",
          width: 130,
          // renderHeader: (h, params) => {
          //   return h('div', (params.columns = this.timeTitle))
          // }
        },
        {
          title: "收费时间",
          key: "balanceDate",
          width: 130,
          // renderHeader: (h, params) => {
          //   return h('div', (params.columns = this.timeTitle))
          // }
        },
      ],
      listDetailColumns: [
        { title: "序号", key: "no", align: "center", minWidth: 65 },
        // { title: '机构', key: 'centerName', minWidth: 65 },
        { title: "姓名", key: "pName", minWidth: 65 },
        { title: "门诊号", key: "mzh", minWidth: 90 },
        {
          title: "项目开立时间",
          width: 150,
          key: "openDateTime",
        },
        {
          title: "收费时间",
          width: 150,
          key: "openDate",
        },
        {
          title: "参保类别",
          width: 100,
          key: "healthCareType",
        },
        { title: "处方号", key: "prescriptionNo", minWidth: 110 },
        { title: "医保结算流水号", key: "siBalanceSerialNo", minWidth: 110 },
        { title: "名称", key: "itemName", minWidth: 110 },
        { title: "规格型号", key: "specifications", minWidth: 110 },
        { title: "医保码", key: "compareCode", minWidth: 200 },
        { title: "项目类型", key: "categoryName", minWidth: 70 },
        { title: "单价", key: "unitPrice", minWidth: 70 },
        { title: "数量", key: "qty", minWidth: 70 },
        { title: "金额", key: "totalPrice", minWidth: 80 },
        { title: "等级", key: "level", minWidth: 60 },
        {
          title: "支付比例",
          key: "payProportion",
          align: "center",
          minWidth: 70,
        },
        // { title: '自费金额', key: 'payAmount', align: 'center', minWidth: 70 },
        // {
        //   title: '医保支付金额',
        //   key: 'isPayAmount',
        //   align: 'center',
        //   minWidth: 90
        // },
        {
          title: "医保上传数量",
          key: "isUpQty",
          align: "center",
          minWidth: 90,
        },
        { title: "操作员", key: "operationMan", minWidth: 65 },
      ],
      outListDetailColumns: [
        { title: "序号", key: "no", minWidth: 65, align: "center" },
        // { title: '机构', key: 'centerName', minWidth: 65 },
        { title: "姓名", key: "pName", minWidth: 65 },
        { title: "门诊号", key: "mzh", minWidth: 110 },
        {
          title: "退费时间",
          key: "cancelDate",
          minWidth: 130,
        },
        { title: "处方号", key: "prescriptionNo", minWidth: 110 },
        { title: "名称", key: "itemName", minWidth: 110 },
        { title: "单价", key: "unitPrice", minWidth: 65 },
        { title: "数量", key: "qty", minWidth: 65 },
        { title: "金额", key: "totalPrice", minWidth: 80 },
        { title: "操作员", key: "operationMan", minWidth: 65 },
      ],
      //渝快保赔付
      compensateColumns: [
        {
          title: "序号",
          key: "serialNo",
          minWidth: 20,
          align: "center",
          sortable: true,
        },
        { title: "机构名称", key: "shortName", minWidth: 65, align: "center" },
        { title: "就诊流水号", key: "mdtrt_id", minWidth: 65, align: "center" },
        {
          title: "医保结算流水号",
          key: "jsjylsh",
          minWidth: 70,
          align: "center",
        },
        { title: "姓名", key: "xm", minWidth: 40, align: "center" },
        { title: "证件号码", key: "cardNum", minWidth: 110, align: "center" },
        {
          title: "结算时间",
          key: "jsrq",
          minWidth: 100,
          align: "center",
          sortable: true,
          render(h, params) {
            return (
              <span>
                {formateDateToString(
                  new Date(params.row.jsrq),
                  "yyyy-MM-dd hh:mm:ss"
                )}
              </span>
            );
          },
        },
        { title: "确诊疾病", key: "ryzdmc", minWidth: 65, align: "center" },
        {
          title: "医疗费总金额",
          key: "zje",
          minWidth: 80,
          align: "center",
          sortable: true,
        },
        {
          title: "渝快保赔付金额",
          key: "hifes_pay",
          minWidth: 90,
          align: "center",
          sortable: true,
        },
      ],
      //医保物资上传数据汇总统计表
      medicalMaterialsColumns: [
        { title: "项目类型", key: "itemType", align: "center" },
        { title: "项目名称", key: "xmmc", align: "center" },
        { title: "规格", key: "specifications", align: "center" },
        { title: "医保编码", key: "yblsh", align: "center", minWidth: 100 },
        { title: "医院项目编码", key: "yynm", align: "center" },
        { title: "等级", key: "strHiLevel", align: "center" },
        { title: "厂家", key: "manufacturer", align: "center" },
        { title: "总数", key: "totalCount", align: "center", sortable: true },
      ],
      timeTitle: "收费时间",
      tableLoading: false,
      table3Loading: false,
      dataCount: 0,
      buttonRole: BUTTONROLE,

      printJson: {},
      printDatas: {
        api: "",
        title: "",
        columns: [],
        params: {},
      },
    };
  },
  computed: {
    // cy 收入总汇表和收入日报表的选择全部机构时的标志
    headerFlag() {
      if (
        this.hospitalCheckedIds.length === 1 &&
        this.hospitalCheckedIds[0] !== "0"
      ) {
        return false;
      } else if (this.tabCheckedId === 2) {
        return false;
      } else if ([4, 5, 6, 7, 8, 9].includes(this.tabCheckedId)) {
        return false;
      } else {
        return true;
      }
    },
    hasPage() {
      if (this.tabCheckedId === 8 || 9) {
        return true;
      } else {
        return [4, 5, 6, 7].includes(this.tabCheckedId);
      }
    },
    isTab3() {
      return this.tabCheckedId === 3;
    },
  },
  created() {
    let data = this.loadMenu();
    data.then(() => {
      let d = getNowDate().substring(0, 10);
      this.formData.date.push(getCurrentMonthFirst(), d);
      this.monthFirst = this.formData.date[0];
      this.monthLast = this.formData.date[1];
      // this.changeDate(this.formData.date)
    });
  },
  // mounted () {
  //   this.$nextTick(_ => {
  //     // console.log(this.$refs.incoming_table.clientHeight)
  //     // this.tableHeight = this.$refs.template.$el.clientHeight - 226
  //   })
  // },
  methods: {
    ...mapMutations(["setColumns", "setTitle", "setParams", "setApi"]),
    keySearch() {
      this.current = 1;
      let { actionCode, jsonStr } = this.getTableDataByKey(this.tabCheckedId);
      if (this.tabCheckedId == 8 || 9) {
        console.log(jsonStr, actionCode);
      }
      this.loadTable(actionCode, jsonStr);
    },
    maxTagPlaceholder(num) {
      return `+${num}机构`;
    },
    // cy 双击table某一行触发【用于某些统计表单查看明细】
    dbClick(row, index) {
      if (!row.prescriptionNo) {
        return false;
      }
      if (this.tabCheckedId === 4) {
        this.detailTableCheckedInfo.name = row.pName;
        this.detailTableCheckedInfo.prescriptionNo = row.prescriptionNo;
        this.detailTableFlag = true;
        this.detailTableColumns = this.listDetailColumns;
      } else if (this.tabCheckedId === 5) {
        this.detailTableCheckedInfo.name = row.pName;
        this.detailTableCheckedInfo.prescriptionNo = row.prescriptionNo;
        this.detailTableFlag = true;
        this.detailTableColumns = this.outListDetailColumns;
      } else {
        return false;
      }
      this.swsApi
        // cy 订单prescriptionNo改为id
        .swsGet(
          `BusinessTargetClntroller/BusinessTarget/ChargeDetailByNo/${row.id}`
        )
        .then((res) => {
          this.detailTableData = res.data.result;
          this.$nextTick(() => {
            let dom = document.querySelector(".financial-content .content");
            dom.scrollTo({
              top: dom.scrollHeight,
              left: 0,
              behavior: "smooth",
            });
          });
        })
        .catch((e) => {
          this.$Message.error(e);
        });
    },
    // 选择日期
    changeDate(e) {
      if (typeof e === "object" && e[0]) {
        this.monthFirst = e[0];
        this.monthLast = e[1];
        this.formData.date = e;
        // this.changeTab(this.tabCheckedId)
      } else if (e && typeof e === "string") {
        this.formData.singleDate = e;
        // this.changeTab(this.tabCheckedId)
      } else {
        this.formData.singleDate = getNowFormatDate(false);
      }
    },
    // 多选机构
    changeHospitals(ids) {
      if (ids.length === 0) this.hospitalCheckedName = "透析机构";
      // cy 非全选 且 只选择了一个机构时 获取该机构名称
      if (ids.length === 1 && ids[0] !== "0") {
        this.hospitalCheckedName = this.hospitalList.filter(
          (v) => v.dialysisId === ids[0]
        )[0].dialysisName;
      } else {
        this.hospitalCheckedName = "透析机构";
      }
      // cy 如果多选了全选+其他机构，则去掉全部
      if (ids.length > 1 && ids[0] === "0") {
        this.hospitalCheckedIds.splice(ids.indexOf("0"), 1);
      }
      // 如果多选，再选中了全选，则清空ids，并显示全选
      if (ids.length > 1 && ids[ids.length - 1] === "0") {
        this.hospitalCheckedIds = ["0"];
      }
      this.columns = [];
      this.tableData = [];
      if (this.tabCheckedId == 3) this.otherTableData = [];
      this.current = 1;
      this.dataCount = 0;
    },
    // 切换医院
    changeHospital(id) {
      this.hospitalCheckedId = id;
      if (id !== undefined) {
        this.hospitalCheckedName = this.hospitalList.filter(
          (v) => v.dialysisId === id
        )[0].dialysisName;
      } else {
        this.hospitalCheckedName = "透析机构";
      }

      this.columns = [];
      this.tableData = [];
      if (this.tabCheckedId == 3) this.otherTableData = [];
      // 解决机构切换，请求页码不对
      this.current = 1;
      this.dataCount = 0;
    },
    // 获取机构
    getHospital(hospitalList) {
      this.hospitalList = hospitalList;
      this.hospitalCheckedId = hospitalList[1].dialysisId;
    },
    // 20200616 处理医保流水号长数字字符串导出变成科学计数法，字符串前加英文单引号
    handleLongNumStr(dataList, keyList = []) {
      let rawData = JSON.parse(JSON.stringify(dataList));
      let data = [];
      if (dataList.length > 0) {
        keyList.forEach((key, index) => {
          data = rawData.map((res) => {
            if (res[key] !== "" && res[key] !== null)
              res[key] = `\t${res[key]}`;
            return res;
          });
        });
        return data;
      } else {
        return dataList;
      }
    },
    // 导出
    exportTable() {
      if (this.tableData.length <= 1) {
        this.$Message.warning("暂无数据，无法导出。");
        return false;
      }
      let filename = "";
      let pDom = document.querySelector(".table_title .p");
      filename = pDom.innerText;

      // 特殊表格
      if ([1, 2, 3].includes(this.tabCheckedId)) {
        // tableExport('table_box', filename, 'xlsx')
        // 列表三 需要将两个表合并在一起导出
        // 制作一份第二个表单的表头

        let tbData =
          this.tabCheckedId !== 3
            ? this.tableData
            : [...this.tableData, { no: "收费情况" }, ...this.otherTableData];
        this.$refs.incoming_table.exportCsv({
          filename: `${filename}（${this.formData.date[0]}~${this.formData.date[1]}）`,
          columns: this.columns,
          data: tbData,
        });
      } else if (this.tabCheckedId === 7) {
        // 3/7这两个报表需要后台返回文件流进行导出
        let url = "BusinessTargetClntroller/ExportExcel/filedownload";
        let data = {
          pageNum: this.current,
          pageSize: 99999,
          centerId: [this.hospitalCheckedId],
          beginTime: this.formData.date[0],
          endTime: this.formData.date[1],
          keyword: this.searchKey,
        };
        this.tbDownLoading = true;
        this.swsApi
          .swsDownload({
            method: "post",
            url: url,
            responseType: "blob",
            data: data,
          })
          .then((res) => {
            let blob = new Blob([res.data], {
              type: "application/x-zip-compressed",
            });
            let elink = document.createElement("a");
            elink.download = `${filename}（${this.formData.date[0]}~${this.formData.date[1]}）.xlsx`;
            elink.style.display = "none";
            let href = URL.createObjectURL(blob);
            elink.href = href;
            document.body.appendChild(elink);
            elink.click();
            URL.revokeObjectURL(href);
            document.body.removeChild(elink);
            this.tbDownLoading = false;
          })
          .catch(() => {
            this.tbDownLoading = false;
          });
      } else if (this.tabCheckedId === 8) {
        let url = "Document/ExportExcel/ykbfiledownload";
        let data = {
          pageNum: this.current,
          pageSize: 99999,
          centerId: this.hospitalCheckedId,
          beginTime: this.formData.date[0],
          endTime: this.formData.date[1],
          keyword: this.searchKey,
        };
        this.tbDownLoading = true;
        this.swsApi
          .swsDownload({
            method: "post",
            url: url,
            responseType: "blob",
            data: data,
          })
          .then((res) => {
            let blob = new Blob([res.data], {
              type: "application/x-zip-compressed",
            });
            let elink = document.createElement("a");
            elink.download = `${filename}（${this.formData.date[0]}~${this.formData.date[1]}）.xlsx`;
            elink.style.display = "none";
            let href = URL.createObjectURL(blob);
            elink.href = href;
            document.body.appendChild(elink);
            elink.click();
            URL.revokeObjectURL(href);
            document.body.removeChild(elink);
            this.tbDownLoading = false;
          })
          .catch(() => {
            this.tbDownLoading = false;
          });
      } else {
        let tableData = [];
        // if (this.tabCheckedId === 7) {
        let { actionCode, jsonStr } = this.getTableDataByKey(
          this.tabCheckedId,
          99999
        );
        // cy api接口的centerId类型更改为Array 在这里统一处理
        if (
          typeof jsonStr.centerId === "string" &&
          this.tabCheckedId != 8 &&
          this.tabCheckedId != 9
        ) {
          jsonStr.centerId = [jsonStr.centerId];
        } else if (typeof jsonStr.centerId === "undefined") {
          delete jsonStr.centerId;
        } else if (
          typeof jsonStr.centerId === "object" &&
          jsonStr.centerId.length === 0
        ) {
          jsonStr.centerId = ["0"];
        }
        this.swsApi
          .swsPost(actionCode, jsonStr)
          .then((res) => {
            if (res.data.success) {
              // 处理导出 收费明细 医保结算流水号 ：siBalanceSerialNo 问题
              // if (this.tabCheckedId === 7) tableData = this.handleLongNumStr(res.data.result, ['siBalanceSerialNo'])
              // else tableData = res.data.result
              tableData = res.data.result;
              tableData.forEach((item) => {
                if (item.jsrq) {
                  item.jsrq = formateDateToString(
                    new Date(item.jsrq),
                    "yyyy-MM-dd hh:mm:ss"
                  );
                }
                if (item.cardNum) {
                  item.cardNum = Number(item.cardNum);
                }
              });
            } else {
              this.$Message.error(
                `请求全部数据失败(${res.data.error})，导出失败`
              );
            }
          })
          .then(() => {
            this.$refs["incoming_table"].exportCsv({
              filename: `${filename}（${this.formData.date[0]}~${this.formData.date[1]}）`,
              columns: this.columns,
              data: tableData.length === 0 ? this.tableData : tableData,
            });
          });
        // }
      }
    },
    // 打印
    print() {
      if (this.tableData.length <= 1) {
        this.$Message.warning("暂无数据，无法打印。");
        return false;
      }

      let routers = [
        "cost_summary",
        "cost_table",
        "income_daily",
        "cost_summary",
        "cost_summary",
        "cost_summary",
        "cost_summary",
        //8.9
        "cost_summary",
        "cost_summary",
      ];
      let url = window.location.href.split("#")[0];
      let pDom = document.querySelector(".p");
      let filename = `${pDom.innerText}（${this.formData.date[0]}~${this.formData.date[1]}）`;

      // 缓存打印数据
      this.printDatas = Object.assign(this.printDatas, {
        columns: JSON.parse(JSON.stringify(this.columns)),
        title: filename,
      });

      console.log(this.printDatas);
      // 针对收费清单和退费清单收费时间退费时间问题
      if ([4, 5].includes(this.tabCheckedId)) {
        let dateHeader = {};
        if (this.tabCheckedId === 4) {
          dateHeader = {
            key: "cancelDate",
            title: "收费清单",
          };
        }
        if (this.tabCheckedId === 5) {
          dateHeader = {
            key: "cancelDate",
            title: "退费清单",
          };
        }
        this.printDatas.columns.splice(-1, 1, dateHeader);
      }

      const { api, columns, title, params } = this.printDatas;
      this.setTitle(title);
      this.setApi(api);
      this.setColumns(columns);
      this.setParams(params);

      window.open(`${url}#/print/${routers[this.tabCheckedId - 1]}`);
    },
    // 加载表格类型
    loadMenu() {
      return new Promise((resolve, reject) => {
        this.swsApi
          .swsPost(`BusinessTargetClntroller/BusinessTarget/1/1`)
          .then((res) => {
            if (res.data.success) {
              this.tabData = res.data.result;
              resolve("成功");
            }
          })
          .catch((e) => {
            reject(e);
            this.$Message.error(e);
          });
      });
    },
    // 选择表格类型
    changeTab(index) {
      // console.log(index)
      this.tabCheckedId = index;
      this.title = this.tabData.filter(
        (item) => item.childKey === index
      )[0].childValue;
      this.columns = [];
      this.tableData = [];
      if (this.tableTotal.length > 0) {
        this.tableTotal = [];
      }
      this.searchKey = ""; // 清空搜索关键词
      this.detailTableFlag = false; // cy 重置显隐明细表单的flag
      this.current = 1;
      this.dataCount = 0;
    },
    // 选择收入总汇表类型
    changeType() {
      this.tableData = [];
    },
    // cy 获取总汇表
    getAllReportData() {
      let jsonStr = {
        centerId: this.hospitalCheckedIds,
        beginTime: this.formData.date[0],
        endTime: this.formData.date[1],
        model: 3,
      };
      this.otherTbLoading = true;
      this.swsApi
        .swsPost("BusinessTargetClntroller/BusinessTarget/ALL", jsonStr)
        .then((res) => {
          if (res.data.success) {
            if (this.headerFlag) {
              let header = res.data.result.tableHeaders;
              for (let item of header) {
                if (item.key === null) {
                  delete item.key;
                }
                if (item.children === null) {
                  delete item.children;
                } else {
                  for (let fitem of item.children) {
                    if (fitem.children === null) {
                      delete fitem.children;
                    }
                    if (item.key === null) {
                      delete item.key;
                    }
                  }
                }
              }
              this.otherColumns = header;
              this.otherTableData = res.data.result.centerDayIncomeOutPuts;
            } else {
              this.otherColumns = this.feeColumns;
              this.otherTableData = res.data.result;
            }
            this.otherTbLoading = false;
          }
        })
        .catch((e) => {
          this.otherTbLoading = false;
          this.$Message.error(e);
        });
    },
    // cy 获取日报表
    getDayReportData() {
      let params = {
        centerId: this.hospitalCheckedIds,
        beginTime: this.formData.date[0],
        endTime: this.formData.date[1],
        model: 1,
      };
      /**
       * @param {String} key _api对应key值
       * @description 策略模式，转换大写
       * @returns {Object} 返回对象，包含title，desc，addApi
       */
      const lowToUpper = (key) => {
        // 大小写
        const upper = (text, count) =>
          `${text}：￥${count}/大写：${convertCurrency(count)}`;
        // 合计
        const totalAndUpper = (count) => upper("合计", count);
        // 收
        const getAndUpper = (count) => upper("收", count);
        const addApi = (key, api) => {
          let { title, desc } = api;
          !_api[key] && (_api[key] = { title, desc });
        };
        let _api = {
          total: {
            title: "收费情况",
            desc: totalAndUpper,
          },
          xj: {
            title: "现金",
            desc: getAndUpper,
          },
          individual: {
            title: "个人帐户（职工）",
            desc: getAndUpper,
          },
          jmindividual: {
            title: "个人帐户（居民）",
            desc: getAndUpper,
          },
          mzbz: {
            title: "民政补助（职工）",
            desc: getAndUpper,
          },
          jmmzbz: {
            title: "民政补助（居民）",
            desc: getAndUpper,
          },
          detc: {
            title: "大额（职工）",
            desc: getAndUpper,
          },
          jmdetc: {
            title: "大额（居民）",
            desc: getAndUpper,
          },
          yycb: {
            title: "医院超标",
            desc: getAndUpper,
          },
          ybjj: {
            title: "统筹（职工）",
            desc: getAndUpper,
          },
          jmybjj: {
            title: "统筹（居民）",
            desc: getAndUpper,
          },
          pjst: {
            title: "票据收退",
            desc(count) {
              return `收费：${count || "暂无"}`;
            },
          },
          hmfw: {
            title: "号码范围",
            desc(count) {
              return `收费：${count || "暂无"}`;
            },
          },
          sjph: {
            title: "实际票号",
            desc(count) {
              return `收费：${count || "暂无"}`;
            },
          },
        };
        return { ..._api[key], addApi };
      };
      this.swsApi
        .swsPost(
          "/BusinessTargetClntroller/BusinessTarget/DayReportAsync",
          params
        )
        .then((res) => {
          if (res.data.success) {
            let { result } = res.data;
            let data = [];
            for (let item in result) {
              let { title, desc } = lowToUpper(item);
              let obj = {
                title,
                desc: desc(result[item]),
              };
              if (JSON.stringify(obj) !== "{}") {
                data.push(obj);
              }
            }
            this.tableTotal = data;
          }
        })
        .catch((e) => {
          this.$Message.error(e);
        });
    },
    // 按值请求表格数据
    getTableDataByKey(index, page = 0) {
      let actionCode = "";
      let jsonStr = {};
      switch (index) {
        case 1:
          if (!this.headerFlag) {
            this.columns = this.feeColumns;
          }
          actionCode = "BusinessTargetClntroller/BusinessTarget/ALL";
          jsonStr = {
            centerId: this.hospitalCheckedIds,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            model: this.typeCheckedId,
          };
          break;
        case 2:
          // this.headerFlag = false
          actionCode = "BusinessTargetClntroller/BusinessTarget/CostsAsync";
          jsonStr = {
            centerId: this.hospitalCheckedIds,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            model: 1,
          };
          break;
        case 3:
          if (!this.headerFlag) {
            this.columns = this.feeColumns;
          }
          actionCode = "BusinessTargetClntroller/BusinessTarget/ALL";
          jsonStr = {
            centerId: this.hospitalCheckedIds,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            model: 1,
          };
          // cy 获取日报表
          // this.getDayReportData()
          // cy 获取总汇表
          this.getAllReportData();
          break;
        // 收费清单
        case 4:
          // this.timeTitle = '收费时间'
          this.columns = this.listColumns.map((res) => {
            if (res.key === "cancelDate") {
              res.title = "退费时间";
            }
            return res;
          });
          actionCode = "BusinessTargetClntroller/BusinessTarget/Charge";
          jsonStr = {
            pageNum: page === 0 ? this.current : 1,
            pageSize: page === 0 ? this.pageSize : page,
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            keyword: this.searchKey,
          };
          break;
        // 退费清单
        case 5:
          // this.timeTitle = '退费时间'
          this.columns = this.listColumns.map((res) => {
            if (res.key === "cancelDate") {
              res.title = "退费时间";
            }
            return res;
          });
          actionCode = "BusinessTargetClntroller/BusinessTarget/Refund";
          jsonStr = {
            pageNum: page === 0 ? this.current : 1,
            pageSize: page === 0 ? this.pageSize : page,
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            keyword: this.searchKey,
          };
          break;
        // 退费明细
        case 6:
          this.columns = this.outListDetailColumns;
          actionCode = "BusinessTargetClntroller/BusinessTarget/RefundDetail";
          jsonStr = {
            pageNum: page === 0 ? this.current : 1,
            pageSize: page === 0 ? this.pageSize : page,
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            keyword: this.searchKey,
          };
          break;
        // 收费明细
        case 7:
          let col = [...this.listDetailColumns];
          col.splice(4, 0, {
            title: "结算单号",
            key: "balanceNo",
            minWidth: 110,
          });
          this.columns = col;
          actionCode = "BusinessTargetClntroller/BusinessTarget/ChargeDetail";
          jsonStr = {
            // pageNum: this.current,
            // pageSize: this.pageSize,
            pageNum: page === 0 ? this.current : 1,
            pageSize: page === 0 ? this.pageSize : page,
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
            keyword: this.searchKey,
          };
          break;
        // 渝快保赔付明细统计
        case 8:
          this.columns = this.compensateColumns;
          actionCode = "BusinessTargetClntroller/BusinessTarget/HifesPay";
          jsonStr = {
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
          };
          break;
        //医保物资上传数据汇总统计表
        case 9:
          this.columns = this.medicalMaterialsColumns;
          actionCode =
            "BusinessTargetClntroller/BusinessTarget/SIItemUploadInfo";
          jsonStr = {
            centerId: this.hospitalCheckedId,
            beginTime: this.formData.date[0],
            endTime: this.formData.date[1],
          };
          break;
      }
      // console.log('组装', page, actionCode, jsonStr)
      return { actionCode, jsonStr };
    },
    loadTable(actionCode, jsonStr) {
      this.printJson = jsonStr;
      this.tableLoading = true;
      this.tableData = [];
      if (this.tabCheckedId == 3) this.otherTableData = [];
      this.dataCount = 0;
      this.printDatas.api = actionCode;
      // cy api接口的centerId类型更改为Array 在这里统一处理
      if (
        typeof jsonStr.centerId === "string" &&
        this.tabCheckedId != 8 &&
        this.tabCheckedId != 9
      ) {
        jsonStr.centerId = [jsonStr.centerId];
      } else if (typeof jsonStr.centerId === "undefined") {
        delete jsonStr.centerId;
      } else if (
        typeof jsonStr.centerId === "object" &&
        jsonStr.centerId.length === 0
      ) {
        jsonStr.centerId = ["0"];
      }
      // console.log('请求', jsonStr)
      this.printDatas.params = jsonStr;
      this.swsApi
        .swsPostCouldCancel(actionCode, jsonStr)
        .then((res) => {
          if (res.data.success) {
            if (this.headerFlag) {
              let tableData = res.data.result.centerDayIncomeOutPuts;
              let header = res.data.result.tableHeaders;
              for (let item of header) {
                if (item.key === null) {
                  delete item.key;
                }
                if (item.children === null) {
                  delete item.children;
                } else {
                  for (let fitem of item.children) {
                    if (fitem.children === null) {
                      delete fitem.children;
                    }
                    if (item.key === null) {
                      delete item.key;
                    }
                  }
                }
              }
              this.columns = header;
              this.tableData = tableData;
            } else {
              if (this.tabCheckedId === 2) {
                this.handleTable2Data(res.data.result);
              } else {
                this.tableData = res.data.result;
                this.dataCount = res.data.dataCount;
              }
            }
          } else {
            this.$Notice.error({
              title: "网络出错！",
              desc: res.data.error,
            });
          }
          this.tableLoading = false;
        })
        .catch((e) => {
          this.$Message.error("请求错误", e);
          if (!e.message.includes("取消上一次请求")) {
            this.tableLoading = false;
          }
        });
    },
    handleTable2Data(data) {
      let columnData = data.shift().title.split(",");
      // columnData.unshift('序号')
      var columns = [];
      for (let index in columnData) {
        // if (columnData[index] === '序号') {
        //   columns.push({ title: columnData[index], key: 0, width: '65' })
        // } else {
        columns.push({ title: columnData[index], key: index, align: "center" });
        // }
      }
      this.columns = columns;
      let tableData = data.map((item) => {
        let arr = item.title.split(",");
        let da = {};
        for (let i in arr) {
          da[`${i}`] = arr[i];
        }
        return da;
      });
      // console.log(columns, tableData)
      this.tableData = tableData;
    },
    // 分页
    changePage(page) {
      this.current = page;
      let { actionCode, jsonStr } = this.getTableDataByKey(this.tabCheckedId);
      this.loadTable(actionCode, jsonStr);
    },
    // 表格合计行加粗
    rowClassName(row, index) {
      // cy 成本总汇的表头特殊需row[1] === '合计'来判断 收费清单pName
      if (
        row.itemName === "合计" ||
        row[1] === "合计" ||
        row.pName === "合计"
      ) {
        return "total";
      }
      if (this.tabCheckedId == 4 && row.sumPrice < 0) {
        return "sumPrice_warning";
      }
    },
  },
};
</script>

<style scope="scoped" lang="less">
#incoming_outgoings {
  height: 100%;
  .ivu-table .total td {
    font-weight: bold;
  }
  .type {
    position: sticky;
    top: 0;
    z-index: 999;
    background: #fff;
    // margin: 20px 0;
    padding: 14px 0;
    border-bottom: 2px solid #f6f6f6;
    li {
      display: inline-block;
      color: #a8a8a8;
      font-size: 16px;
      width: 110px;
      text-align: center;
      transition: color 0.3s;
      &::before {
        position: absolute;
        display: block;
        content: "";
        top: -1px;
        left: 0;
        width: 100%;
        height: 2px;
        background-color: transparent;
        transition: all 0.3s;
      }
      &.typeActive {
        color: #4f95e8 !important;
        border-bottom: 2px solid #4f95e8;
      }
      &:hover {
        color: #4f95e8 !important;
      }
    }
  }
  .typeActive {
    color: #4f95e8 !important;
    border-bottom: 2px solid #4f95e8;
  }
  .content {
    background: #ffffff;
    .tab-wrapper {
      display: flex;
      flex-direction: column;
      height: 100%;
    }
    .tab-panel {
      height: 32px;
      // position: sticky;
      // top: 0;
      // z-index: 100;
    }
    .tab-content {
      flex: 1;
      overflow-y: auto;
      // margin-top: 10px;
    }
  }
  .ivu-form .ivu-form-item-label {
    color: #a3a3a3;
  }
  .p {
    text-align: center;
    font-size: 18px;
    color: #515a6e;
    line-height: 37px;
  }
  form {
    padding-top: 14px;
  }
  .ivu-form .ivu-form-item-label {
    color: #a3a3a3;
    font-size: 14px;
  }
  .ivu-form-item {
    margin-bottom: 10px;
  }
  .table {
    font-size: 13px;
    padding: 10px 10px 0;
    background: #ffffff;
    .table-operate {
      padding: 10px 0;
      display: flex;
      justify-content: space-between;
      // .operate-right {
      //   margin-right: 20px;
      // }
      .button-group {
        display: flex;
        justify-content: flex-start;
        .search_box {
          display: flex;
          align-items: center;
          .box_item {
            width: 85px;
          }
        }
        & > span .span_title {
          margin: 0 10px;
        }
        & > span + span {
          margin-left: 10px;
        }
      }
      button + button {
        margin-left: 10px;
      }
    }
  }
  .time {
    margin-top: 10px;
    font-size: 13px;
    color: #333333;
    margin-bottom: 10px;
    span {
      margin-right: 20px;
    }
  }
  .time-right {
    float: right;
  }
  .button {
    text-align: right;
    button {
      margin: 2px 10px;
    }
  }
  .total-table {
    width: 100%;
    border-collapse: collapse;
    margin-bottom: 10px;
    td {
      border: 1px solid #aaaaaa;
      padding: 10px 20px;
    }
  }
  .default-table {
    .ivu-table-wrapper {
      & /deep/ .ivu-table .ivu-table-cell {
        padding-right: 3px;
        padding-left: 3px;
      }
      & /deep/ .ivu-table .sumPrice_warning td {
        // color: #4f95e8;
        background-color: red !important;
      }
    }
  }
  .page {
    float: right;
    margin-top: 10px;
  }
  .ivu-table .total td {
    font-weight: bold;
  }
}
</style>
