<template>
  <div id="home">
    <div class="home-banner">
      <div class="announce">
        <div class="top">
          <div class="name">您好！<span>{{this.empName}}</span></div>
          <div class="location">
          </div>
        </div>
        <div class="time">当前时间：{{currentTime}}</div>
      </div>
    </div>
    <div class="home_data">
      <Row :gutter="20">
        <i-col :md="24" :lg="16" class="home_col">
          <Card shadow>
            <h3>实时数据统计</h3>
            <Row :gutter="20" class="real-time-data">
              <Col class="item" :sm="12" :md="6" :lg="6">
              <div class="top green">
                <common-icon :size="iconSize" type="md-home" />
                <p>透析机构数(家)</p>
              </div>
              <div class="bottom">
                <count-to :end="primaryData.centerCount" count-class="count-style" />
              </div>
              </Col>
              <i-col class="item" :sm="12" :md="6" :lg="6">
                <div class="top blue">
                  <common-icon :size="iconSize" type="ios-person" />
                  <p>全部职工数(人)</p>
                </div>
                <div class="bottom">
                  <count-to :end="primaryData.empCount" count-class="count-style" />
                </div>
              </i-col>
              <i-col class="item" :sm="12" :md="6" :lg="6">
                <div class="top red">
                  <common-icon :size="iconSize" type="ios-easel" />
                  <p>全部血透机数量(台)</p>
                </div>
                <div class="bottom">
                  <count-to :end="primaryData.machineCount" count-class="count-style" />
                </div>
              </i-col>
              <i-col class="item" :sm="12" :md="6" :lg="6">
                <div class="top yellow">
                  <common-icon :size="27.8" type="_iconfont icon-huanzheguanli" />
                  <p>全部患者数(人)</p>
                </div>
                <div class="bottom">
                  <count-to :end="primaryData.patientCount" count-class="count-style" />
                </div>
              </i-col>
            </Row>
          </Card>
        </i-col>
        <i-col :md="24" :lg="8" class="home_col">
          <Card shadow>
            <h3>常用操作</h3>
            <Row :gutter="20" class="original-operate">
              <i-col class="item" :xs="12" :sm="8" :md="8" :lg="6" v-for="item in operateList" :key="item.url" @click.native="goto(item.url)">
                <div class="icon" :class="[item.bgStyle]">
                  <common-icon :size="18" :type="item.type" />
                </div>
                <p>{{item.text}}</p>
              </i-col>
            </Row>
          </Card>
        </i-col>
      </Row>
    </div>

    <div class="home_data">
      <Row :gutter="20">
        <!-- <i-col :md="24" :lg="8" class="home_col">
        </i-col> -->
        <i-col :md="24" :lg="16" class="home_col">
          <div>
            <Card shadow style="position:relative;">
              <h3>医保码信息核对提醒</h3>
              <Icon class="iconRefresh" size="20" @click="handleRefresh" color="#2d8cf0" type="md-refresh" />
              <Table :loading="refreshLoading" :columns="table_column2" :data="medicalItem_data" height="230" style="margin-top:12px;"></Table>
            </Card>
          </div>
        </i-col>
        <i-col :md="24" :lg="8" class="home_col">
          <div v-if="company_data.length>0">
            <Card shadow style="position:relative;">
              <Icon class="iconClose" size="20" @click="isClose=true" type="md-close" />
              <Table :loading="loading" :columns="table_column" :data="company_data" height="230" style="margin-top:18px;"></Table>
              <div style="text-align:right;position:relative;">
                <span style="position:absolute;left:0;top:15px;color:red;">注意：7天内有目录更新</span>
                <Button size="small" type="primary" style="margin-top:15px;" @click="go">
                  查看更多
                  <Icon type="ios-arrow-forward" />
                </Button>
              </div>
              </Button>
            </Card>
          </div>
        </i-col>
      </Row>
    </div>
  </div>
</template>

<script>
import CommonIcon from "_c/common-icon";
import CountTo from "_c/count-to";
import { mapState } from "vuex";
export default {
  name: "home",
  components: {
    CountTo,
    CommonIcon,
  },
  data() {
    return {
      // 常规操作数组链接
      operateList: [
        {
          url: "staff",
          type: "_iconfont icon-zhigongzhuangtaishezhi",
          text: "职工管理",
          bgStyle: "bg-style-1",
        },
        {
          url: "medical",
          type: "_iconfont icon-binganguanli",
          text: "病案管理",
          bgStyle: "bg-style-2",
        },
        {
          url: "purchase_requisition_form",
          type: "_iconfont icon-caigoudan",
          text: "采购申请单",
          bgStyle: "bg-style-3",
        },
        {
          url: "item_file_drug",
          type: "_iconfont icon-record",
          text: "物品档案",
          bgStyle: "bg-style-4",
        },
        {
          url: "incoming_outgoings",
          type: "_iconfont icon-caiwuguanli1",
          text: "财务管理",
          bgStyle: "bg-style-5",
        },
        {
          url: "material",
          type: "_iconfont icon-wuziguanli",
          text: "物资管理",
          bgStyle: "bg-style-1",
        },
      ],
      cityData: {
        name: "获取中",
        temperature: "",
        weather: "",
        windDirection: "",
        windPower: "",
      },
      weendays: [
        "星期日",
        "星期一",
        "星期二",
        "星期三",
        "星期四",
        "星期五",
        "星期六",
      ],
      currentTime: "",
      iconSize: 36,

      timer: null,
      primaryData: {
        centerCount: 0,
        empCount: 0,
        machineCount: 0,
        patientCount: 0,
      },
      refreshLoading: false,
      medicalItem_data: [],
      table_column2: [
        {
          title: "编码",
          key: "medicalItemCode",
          minWidth: 100,
        },
        {
          title: "名称",
          key: "medicalItemName",
          minWidth: 140,
        },
        {
          title: "规格",
          key: "packaging",
          minWidth: 140,
        },
        {
          title: "厂家",
          key: "manufacturer",
          minWidth: 140,
        },
        {
          title: "国家医保码",
          key: "nationItemCode",
          minWidth: 140,
        },
      ],
      company_data: [],
      table_column: [
        {
          title: "名称",
          key: "名称",
          minWidth: 140,
        },
        {
          title: "国家医保编码",
          key: "国家医保编码",
          minWidth: 140,
        },
      ],
      loading: false,
      isClose: false,
    };
  },
  computed: mapState({
    empName: (state) => state.user.empName,
  }),
  mounted() {
    this.getPrimaryData();
    this.setTime();
    this.getData();
    this.handleRefresh();
  },
  methods: {
    getData() {
      let args = {
        queryName: "",
        pageSize: 4,
        pageNum: 1,
      };
      this.loading = true;
      this.swsApi
        .swsPost("Data/SI/UpdateReminder", args)
        .then((res) => {
          this.loading = false;
          if (res.data.result) {
            this.company_data = res.data.result;
          } else {
            this.$Notice.error({
              title: "请求错误",
              desc: "网络错误，请稍后再试",
            });
          }
        })
        .catch((e) => {
          this.loading = false;
        });
    },
    async handleRefresh() {
      this.refreshLoading = true;
      const res = await this.swsApi.swsGet("Data/SI/MedicalSICodeOut");
      if (res.data.result) {
        this.medicalItem_data = res.data.result;
      }
      this.refreshLoading = false;
    },
    jumpToSQ() {
      this.$router.push({
        // path: '/operation_management/purchase_requisition_form',
        // query: { diy: 'cy' },
        name: "purchase_requisition_form",
        params: { auditStatusType: "2" },
      });
    },
    setTime() {
      let date = new Date();
      let timeString = date.toLocaleString().replace(/\//g, "-").split(" ");
      let weenday = this.weendays[date.getDay()];
      timeString.splice(1, 0, weenday);

      this.currentTime = timeString.join("  ");

      this.timer = setTimeout(this.setTime, 1000);
    },
    goto(name) {
      this.$router.push({ name });
    },
    getPrimaryData() {
      this.swsApi
        .swsGet("CenterDialysis/AllTotal")
        .then((res) => {
          if (res.data.success) {
            this.primaryData = res.data.result;
          }
        })
        .catch((e) => {
          console.log(e);
        });
    },
    go() {
      this.$router.push(
        "primary_info/medical_comparsion/directory_update_list"
      );
    },
  },
  beforeDestroy() {
    clearTimeout(this.timer);
  },
};
</script>

<style lang="less">
.home-banner {
  position: relative;
  height: 187px;
  background: #fff url(../../assets/images/home-banner-bg.jpg) center 86%
    no-repeat;
  background-size: 100% auto;
  .announce {
    position: absolute;
    top: 0;
    right: 0;
    padding: 30px 20px;
    width: 386px;
    height: 100%;
    font-size: 14px;
    color: #ffffff;
    background: rgba(74, 120, 138, 0.88);
    .top {
      display: flex;
      align-items: center;
      justify-content: space-between;
      .name {
        font-size: 18px;
        span {
          font-size: 14px;
        }
      }
      .location {
        /deep/ .ivu-icon {
          margin-top: -4px;
        }
      }
    }
    .time {
      margin: 14px 0;
    }
    .weather {
      span {
        display: inline-block;
        line-height: 1;
        vertical-align: bottom;
        & + span {
          margin-left: 15px;
        }
      }
      .temperature {
        font-size: 54px;
      }
    }
  }
}
.home_data {
  padding: 20px 20px 0;
  .home_col {
    margin-bottom: 20px;
  }
}
.dataList {
  padding: 0 20px 0;
}
.real-time-data {
  margin: 10px 0 0;
  padding: 0 10px;
  text-align: center;
  .item {
    margin-bottom: 20px;
    height: 140px;
    border-radius: 3px;
    .top {
      display: flex;
      align-items: center;
      flex-direction: column;
      justify-content: center;
      color: #ffffff;
      height: 82px;
      border-radius: 3px 3px 0px 0px;
      &.green {
        background: #53c9b2;
      }
      &.blue {
        background: #519bf1;
      }
      &.yellow {
        background: #f8be5f;
      }
      &.red {
        background: #f77c75;
      }
      p {
        margin-top: 6px;
      }
    }
    .bottom {
      font-size: 32px;
      height: 64px;
      line-height: 64px;
      color: #333;
      background-color: #f7f7f8;
      border-radius: 0 0 3px 3px;
    }
  }
}
.original-operate {
  .item {
    margin: 4px 0;
    font-size: 12px;
    color: #999999;
    text-align: center;
    cursor: pointer;
    .icon {
      margin: 8px auto 7px;
      width: 44px;
      height: 44px;
      color: #ffffff;
      display: flex;
      align-items: center;
      justify-content: center;
      overflow: hidden;
      border-radius: 44px;
      &.bg-style-1 {
        background-color: #00c4d2;
        box-shadow: 0px 2px 4px 0px rgba(0, 196, 210, 0.3);
      }
      &.bg-style-2 {
        background-color: #7b8dfa;
        box-shadow: 0px 2px 4px 0px rgba(123, 141, 250, 0.3);
      }
      &.bg-style-3 {
        background-color: #f8be5f;
        box-shadow: 0px 2px 4px 0px rgba(248, 190, 95, 0.3);
      }
      &.bg-style-4 {
        background-color: #519bf1;
        box-shadow: 0px 2px 4px 0px rgba(81, 155, 241, 0.3);
      }
      &.bg-style-5 {
        background-color: #f77c75;
        box-shadow: 0px 2px 4px 0px rgba(247, 124, 117, 0.3);
      }
    }
    p {
      white-space: nowrap;
      text-overflow: ellipsis;
      overflow: hidden;
    }
  }
}
.iconClose {
  position: absolute;
  right: 5px;
  top: 6px;
  cursor: pointer;
}
.iconRefresh {
  position: absolute;
  right: 20px;
  top: 18px;
  cursor: pointer;
}
</style>
