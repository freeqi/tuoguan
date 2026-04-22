<template>
  <div id="medical_comparison">
    <div class="top">
      <div class="button-group">
        <span>医保等级</span>
        <Select v-model="MedType" placeholder="请选择医保等级" style="width: 160px;margin: 0 10px;">
          <Option v-for="item in Object.keys(MedTypeObj)" :value="Number(item)" :key="item">{{MedTypeObj[item]}}</Option>
        </Select>
        <Select v-model="filterType" placeholder="请选择审核状态" style="width: 160px;margin: 0 10px 0 0;" clearable>
          <Option v-for="item in filterTypeObj" :value="item.value" :key="item.label">{{item.label}}</Option>
        </Select>
        <Button type="primary" @click="search">查询</Button>
        <Button type="primary" @click="exportTb" style="margin-left:10px;">导出</Button>
      </div>
    </div>
    <div class="content">
      <Table ref="table_local" height="610" :loading="loading" highlight-row :columns="columns" :data="dataSource" no-data-text="暂无数据" :row-class-name="rowClassName"></Table>
      <div class="pagination">
        <div style="float: right;">
          <Page :total="dataCount" :page-size="pageSize" :current.sync="pageNum" @on-change="changePage" show-total></Page>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import columns from "./columns";
import { deepClone, copyValue, formateDateToString } from "@/libs/tools.js";
export default {
  data() {
    return {
      exportDatas: [], // cy 请求的导出数据
      MedType: 1,
      filterType: "",
      dataSource: [],
      MedTypeObj: {
        1: "药品",
        2: "耗材",
        3: "诊疗项目",
      },
      filterTypeObj: [
        {
          label: "医保目录",
          value: -1,
        },
        {
          label: "未审核",
          value: 0,
        },
        {
          label: "审核通过",
          value: 1,
        },
        {
          label: "未通过",
          value: 2,
        },
      ],
      medical_insurance_type: ["甲", "乙", "丙"],
      dataCount: 0, // 数据量
      pageNum: 1, // 页码
      pageSize: 16, // 每页数据条数
      compareInfo: {
        si_YpmlQueryInput: {
          ypName: "",
          brand: "",
          salePrice: null,
          packaging: "",
          manufacturer: "",
          yplsh: "",
          ylfydj: "",
        },
        medicalItemInput: {
          ypName: "",
          brand: "",
          salePrice: null,
          packaging: "",
          manufacturer: "",
          // hiCenterCode: '',
          hiLevel: "",
        },
      },
      compareInfoV: {
        ypName: [],
        brand: [],
        salePrice: [],
        packaging: [],
        manufacturer: [],
        // hiCenterCode: [],
        // hiLevel: [],
        ylfydj: [],
        yplsh: [],
      },
      loading: false,
      center_loading: false,
      compared_loading: false,

      local_data: [],
      local_data_temp: [],
      localItemId: "",
      centerItemId: "",
      // 项目对照ID
      typeID: 1,
      tabValue: "本地项目目录",
      updateLoad: false,
    };
  },
  mixins: [columns],
  methods: {
    auditSubmit(row) {
      let selectedValue = row.auditState === 2 ? "option2" : "option1";
      this.$Modal.confirm({
        title: `<p>审核【${row.medicalItemCode}-${row.medicalItemName}】</p>`,
        // content: `<Switch  />`,
        render: (h) => {
          // 定义单选按钮组
          return h(
            "div",
            {
              style: {
                paddingTop: "20px",
                display: "flex",
                justifyContent: "center",
                fontSize: "22px",
              },
            },
            [
              h(
                "RadioGroup",
                {
                  model: {
                    value: selectedValue, // 绑定选中值
                    callback: (value) => {
                      selectedValue = value;
                    },
                    expression: "selectedValue", // 对应 data 中的变量名
                  },
                },
                [
                  h("Radio", { props: { label: "option1" } }, "通过"),
                  h("Radio", { props: { label: "option2" } }, "不通过"),
                ]
              ),
            ]
          );
        },
        onOk: async () => {
          const url = `Data/SI/MedMonthAudit`;
          const jsonStr = {
            medicalId: row.medicalId,
            nationItemCode: row.nationItemCode,
            auditState: selectedValue == "option1" ? 1 : 2,
          };
          const res = await this.swsApi.swsPost(url, jsonStr);
          if (res.data.code == 200) {
            this.$Message.success("审核完成！");
            this.search();
          } else {
            this.$Message.warning(res.data.error);
          }
        },
      });
    },
    // 查询
    exportTb() {
      this.searchLocal(2);
    },
    search() {
      this.searchLocal();
    },
    isNullOrEmpty(val) {
      return val === null || val === "";
    },
    handleUploadSuccess() {
      this.searchLocal();
    },
    // 查询本地项目
    searchLocal(flag = 1) {
      this.local_data = [];
      this.loading = true;
      this.pageNum = 1;
      const url = `Data/SI/GetMedHisCatalog/${this.MedType}`;
      this.swsApi
        .swsGet(url)
        .then((res) => {
          if (res.data.success) {
            let arr = [];
            let templateList = res.data.result.filter((item) => {
              if (this.filterType === "" || this.filterType === undefined) {
                return true;
              } else {
                return item.auditState == this.filterType;
              }
            });
            this.local_data = templateList.map((item) => {
              item.isFlag = true;
              item.cellClassName = {};
              if (item.medCatalog === "本地目录") {
                arr.push(item);
              } else {
                for (let i = arr.length; i > 0; i--) {
                  const item2 = arr[i - 1];
                  // if()
                  const isNull =
                    this.isNullOrEmpty(item.medicalItemName) &&
                    this.isNullOrEmpty(item2.medicalItemName);
                  if (
                    !isNull &&
                    item.medicalItemName != item2.medicalItemName
                  ) {
                    item2.cellClassName["medicalItemName"] =
                      "demo-table-info-cell";
                  }
                  const isNull2 =
                    this.isNullOrEmpty(item.packaging) &&
                    this.isNullOrEmpty(item2.packaging);
                  if (!isNull2 && item.packaging != item2.packaging) {
                    item2.cellClassName["packaging"] = "demo-table-info-cell";
                  }
                  const isNull3 =
                    this.isNullOrEmpty(item.manufacturer) &&
                    this.isNullOrEmpty(item2.manufacturer);
                  if (!isNull3 && item.manufacturer != item2.manufacturer) {
                    item2.cellClassName["manufacturer"] =
                      "demo-table-info-cell";
                  }
                  // const flag =
                  //   item.medicalItemName === item2.medicalItemName &&
                  //   item.packaging === item2.packaging &&
                  //   item.manufacturer === item2.manufacturer;
                  // if (!flag) {
                  //   item2.isFlag = false;
                  // }
                }
                arr = [];
              }
              return item;
            });
            this.dataCount = this.local_data.length;
            this.getCompanyList(0);
            if (flag == 2) {
              this.exportDatas = this.local_data;
              let columns = deepClone(this.columns);
              this.$refs.table_local.exportCsv({
                filename: `医保月核对${formateDateToString(
                  new Date(),
                  "yyyyMMdd"
                )}`,
                columns: columns,
                data: this.exportDatas,
              });
            }
          } else {
            this.local_data = [];
            this.dataCount = this.local_data.length;
            this.$Message.error(res.data.error);
            this.getCompanyList(0);
          }
          this.loading = false;
        })
        .catch((e) => {
          this.loading = false;
        });
    },
    changePage(i) {
      this.getCompanyList(i);
    },
    getCompanyList(number) {
      this.loading = true;
      const arr = this.local_data.slice(
        (this.pageNum - 1) * this.pageSize,
        this.pageNum * this.pageSize
      );
      this.dataSource = JSON.parse(JSON.stringify(arr));
      this.$nextTick(() => {
        this.loading = false;
      });
    },
    rowClassName(row, index) {
      // if (!row.isFlag) {
      //   console.log('demo-table-error-row')
      //   return "demo-table-error-row";
      // } else {
      return "";
      // }
    },
  },
  components: {},
};
</script>

<style scoped lang="less">
#medical_comparison {
  padding: 0 0 10px;
  height: 100%;
  overflow-y: auto;
  background: #ffffff;
  .button-group {
    margin-top: 15px;
    text-align: left;
    button + button {
      margin-left: 10px;
    }
  }
  .top {
    z-index: 44;
    background: #ffffff;
    padding: 20px 40px 0;
    .ivu-form-item {
      // margin-bottom: 10px;
      margin-bottom: 0;
    }
    .ivu-form .ivu-form-item-label {
      line-height: 0.8;
    }
  }
  .bottom {
    z-index: 44;
    background: #ffffff;
    padding: 20px 40px 0;
    .ivu-form-item {
      margin-bottom: 10px;
    }
  }
  /deep/ .content {
    margin-top: 20px;
    padding: 0 40px;
    .sec_header {
      margin-top: 20px;
      font-size: 16px;
      margin-bottom: 10px;
    }
    .ivu-table .demo-table-error-row td {
      background-color: #f8b9b9;
      color: #fff;
    }
    .ivu-table .demo-table-info-cell {
      background-color: #f8b9b9;
      color: #fff;
    }
  }
  .tab {
    margin: 0 0 30px;
  }
  .clearBtn {
    margin: 33px 0 0 5px;
  }
  & /deep/ .ivu-table .table-comparsion-row td {
    background-color: #c9f3e2;
  }
  .pagination {
    padding: 10px;
    &::after {
      content: "";
      display: block;
      height: 0;
      visibility: hidden;
      clear: both;
    }
  }
}
</style>
