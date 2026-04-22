<template>
  <div id="purchase_requisition" :class="{ overflow: detailShowFlage }">
    <div class="btn-groups">
      <span>机构</span>
      <Select
        v-model="hospitalCheckedId"
        @on-change="changeHospital"
        style="width: 160px;"
      >
        <Option
          v-for="item in hospitalList"
          :key="item.id"
          :value="item.dialysisId"
          >{{ item.dialysisName }}</Option
        >
      </Select>
      <span>患者</span>
      <Select v-model="patientId" style="width: 160px;">
        <Option
          v-for="item in patientList"
          :key="item.value"
          :value="item.value"
          >{{ item.label }}</Option
        >
      </Select>
      <span>透析模式</span>
      <Input
        v-model="dialysisType"
        placeholder="请输入透析模式"
        style="width: 200px"
      ></Input>
      <span>透析器型号</span>
      <Input
        v-model="dialyzer"
        placeholder="请输入透析器型号"
        style="width: 200px"
      ></Input>
      <span>时间范围</span>
      <DatePicker
        v-model="time"
        format="yyyy-MM-dd"
        type="daterange"
        placement="bottom-end"
        placeholder="请选择时间段"
        style="width: 200px;"
        @on-change="changeDate"
      >
      </DatePicker>
      <Button type="primary" @click="getFormList">查询</Button>
      <Button type="primary" @click="exportData">导出</Button>
    </div>
    <Divider></Divider>
    <div class="detail_table">
      <Table
        ref="table"
        :columns="table_columns"
        :data="table_data"
        :loading="loading"
        height="700"
      >
      </Table>

      <!-- <div class="pagination" v-if="dataCount > pageSize">
        <Page
          :total="dataCount"
          :page-size="pageSize"
          :current.sync="startPage"
          @on-change="handleChangePage"
        />
      </div> -->
    </div>
  </div>
</template>

<script>

export default {
  name: "dialysis",
  data() {
    return {
      time: [],
      // 表格
      table_columns: [
        {
          title: "患者姓名",
          key: "name",
          align: "center",
          render(h, params) {
            return <span>{params.row.patientDar.name}</span>;
          }
        },
        // {
        //   title: '患者编号',
        //   key: 'patientId',
        //   align: 'center',
        //   width:280,
        // },

        {
          title: "记录时间",
          width: 150,
          key: "recordDate",
          render(h, params) {
            return <span>{params.row.recordDate.substring(0, 10)}</span>;
          }
        },
        {
          title: "透析模式",
          key: "dialysisType",
          width: 100,
          render(h, params) {
            return <span>{params.row.dialysisType || "- -"}</span>;
          }
        },
        {
          title: "透析器型号",
          key: "dialyzer",
          render(h, params) {
            return <span>{params.row.dialyzer || "- -"}</span>;
          }
        },

        {
          title: "透析前尿素氮(mmol/L)",
          key: "c01"
        },
        {
          title: "透析后尿素氮(mmol/L)",
          key: "c02"
        },
        {
          title: "第二次透析前尿素氮(mmol/L)",
          key: "c03",
          render(h, params) {
            return <span>{params.row.c03 || "- -"}</span>;
          }
        },
        {
          title: "透析前体重(kg)",
          key: "w01"
        },
        {
          title: "透析后体重(kg)",
          key: "w02"
        },
        {
          title: "透析时间(h)",
          key: "t"
        },
        {
          title: "透析间期",
          key: "q",
          render(h, params) {
            return <span>{params.row.q || "- -"}</span>;
          }
        },

        // {
        //   title: '血流量',
        //   key: 'BloodFlow',
        // },
        {
          title: "URR(%)",
          key: "urr"
        },
        {
          title: "Kt/V",
          key: "ktV"
        },
        {
          title: "TACurea",
          key: "taCurea"
        },
        {
          title: "PCR",
          key: "pcr"
        },
        {
          title: "NPCR",
          key: "npcr"
        }
      ],
      table_data: [],
      loading: false,

      // 时间
      beginTime: "",
      endTime: "",

      // 详情页显示
      detailShowFlage: false,
      // 缓存当前scrollTop值
      scrollTop: 0,

      // 血透中心
      hospitalList: [],
      hospitalCheckedId: "0",

      // 患者列表
      patientList: [],
      patientId: "",
      // 透析器型号
      dialyzerList: [],
      dialyzer: "",
      // 透析模式
      dialysisTypeList: [],
      dialysisType: "",

      dataCount: 0,
      pageSize: 10,
      startPage: 1
    };
  },
  created() {
    this.getHospital();
    this.getPatientId();
    this.getEquipment();
    this.getFormList();
  },

  methods: {
    // 获取数据
    getFormList() {
      let inPut = {
        centerId: this.hospitalCheckedId,
        patientId: this.patientId,
        dialyzer: this.dialyzer,
        dialysisType: this.dialysisType,
        beginRecordDate: this.beginTime,
        endRecordDate: this.endTime

        // 用以前的时间 有数据 方便显示数据 一会儿删
        // beginRecordDate: "2021-07-31T16:00:00.000Z",
        // endRecordDate: "2021-09-27T16:00:00.000Z",
      };
      this.loading = true;
      this.swsApi
        .swsPost("Patients/GetDialysisAdequacyRecord", inPut)
        .then(res => {
          if (res.data.success) {
            this.table_data = res.data.result;
          }
          this.loading = false;
        })
        .catch(e => {
          console.log(e);
        });
    },

    changeHospital() {
      this.startPage = 1;
      this.getPatientId();
      // this.getFormList()
    },
    handleChangePage(i) {
      this.startPage = i;
      this.getFormList();
    },

    changeDate(v) {
      if (!v) return;
      this.beginTime = this.time[0];
      this.endTime = this.time[1];

      // this.startPage = 1
      // this.getFormList()
    },
    // 获取机构
    getHospital() {
      this.swsApi
        .swsPost("Employee/GetEmplyeeByDialysisList")
        .then(res => {
          if (res.data.success) {
            this.hospitalList = res.data.result;
          }
        })
        .catch(e => {});
    },
    // 解决小屏幕样式问题
    afterHide() {
      let dom = document.querySelector("#purchase_requisition");
      dom.scrollTo(0, this.scrollTop);
      this.detailShowFlage = false;
    },

    // 获取患者id和名字
    getPatientId() {
      this.patientList = [];
      let jsonStr = {
        centerId: this.hospitalCheckedId,
        hospitalStateId: "0",
        name: ""
      };
      this.swsApi.swsPost("Patients/Patients", jsonStr).then(res => {
        if (res.data.code == 200) {
          console.log(res.data.result);
          res.data.result.forEach((item, index) => {
            let obj = { value: item.id, label: item.name };
            this.patientList.push(obj);
          });
        }
      });
    },
    // 导出
    exportData() {
      this.$refs.table.exportCsv({
        filename: "患者透析充分性表",
        columns: this.table_columns,
        data: this.table_data.forEach((item,index)=>{
          item.name = item.patientDar.name;
          item.recordDate = item.recordDate.substring(0,10)
        })
      });
    },
    //获取设备型号，透析模式
    getEquipment() {
      // 设备型号
      this.swsApi
        .swsPost("SystemDictionary/DictionaryList", { typeId: "14" })
        .then(res => {
          if (res.data.code == 200) {
            console.log(res.data.result);
            res.data.result.forEach((item, index) => {
              let obj = { value: item.id, label: item.name };
              this.dialyzerList.push(obj);
            });
          }
        });

      // 透析模式
      this.swsApi
        .swsPost("SystemDictionary/DictionaryList", { typeId: "11" })
        .then(res => {
          if (res.data.code == 200) {
            console.log(res.data.result);
            res.data.result.forEach((item, index) => {
              let obj = { value: item.id, label: item.name };
              this.dialysisTypeList.push(obj);
            });
          }
        });
    }
  }
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
