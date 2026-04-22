<template>
  <div class="inner-content">
    <div class="main">
      <div class="ivu-card aside pat_list">
        <div class="pat_cont">
          <h3 class="title">机构患者列表</h3>
          <!-- <div class="hos-selection" > -->
          <Select
            v-model="hospitalCheckedId"
            @on-change="changeHospital"
            transfer
            filterable
            placeholder="请选择透析中心"
          >
            <Option
              v-for="item in hospitalList"
              :value="item.dialysisId"
              :key="item.dialysisId"
            >{{ item.dialysisName }}</Option>
          </Select>
          <!-- </div> -->

          <div class="search-box">
            <Input v-model="searchKey" clearable placeholder="搜索患者..."/>
          </div>
          <!--<input placeholder="搜索..." class="ivu-input" type="text" style="opacity: 0;height: 0;position: fixed;z-index: -99;">-->
          <div class="sta-list">
            <div class="pat_hos">
              <span class="w-35" :class="{isActive1:staType==1}" @click="changeStaType(1)">
                在院
                <span>({{inHospitalNum}})</span>
                <!-- <span>({{filterList.length}})</span> -->
              </span>
              <span class="w-35" :class="{isActive1:staType==2}" @click="changeStaType(2)">
                其他
                <span>({{dataCount-inHospitalNum}})</span>
              </span>
              <span class="w-35" :class="{isActive1:staType==0}" @click="changeStaType(0)">
                全部
                <span>({{dataCount}})</span>
              </span>
            </div>
            <div class="pat_hos">
              <span class="w-35">
                编号
              </span>
              <span class="w-20">
                年龄
              </span>
              <span class="w-45">
                姓名
              </span>
            </div>

            <ul class="pat_msg" :loading="treeLoading">
              <li
                @click="changePatient(item)"
                :key="index"
                v-for="(item,index) in filterList"
                :class="{isOdd:index%2==1,isActive:item.id==patientCheckedInfo.id}"
              >
                <span class="w-35">{{item.patientNo}}</span>
                <span class="w-20">{{item.age}}</span>
                <span class="w-45 name">
                  <Icon class="sex nv" type="md-female" v-show="item.sex=='女'"/>
                  <Icon class="sex nan" type="md-male" v-show="item.sex=='男'"/>
                  {{item.name}}
                  <!--<Tooltip content="Right Center text" placement="right">-->
                  <!-- <strong class="red_12" v-show="item.BloodBorneDisease!='正常'">({{item.BloodBorneDisease}})</strong> -->
                  <!--</Tooltip>-->
                </span>
              </li>
              <li v-show="filterList.length==0">
                <span>未查询到数据</span>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <!-- 主内容 -->
      <div class="content">
        <slot name="content"></slot>
      </div>
    </div>
  </div>
</template>

<script>
import { toFilterKey } from '@/libs/tools.js'
// import head_img from '@/assets/images/headimg.png'
export default {
  name: 'medical_record_list',
  data () {
    return {
      staType: 1,
      // now: '',
      dataCount: 0,
      inHospitalNum: 0,
      patientCheckedInfo: {
        id: '',
        name: ''
      },
      // headImg: head_img,

      pageSize: 20,
      startPage: 1,
      patientListData: [],

      hospitalList: [],
      hospitalCheckedId: '0',
      treeLoading: false,
      searchKey: ''
    }
  },
  props: {
    api: {
      require: true,
      default: ''
    }
  },
  created () {
    this.$nextTick(() => {
      // 加载机构
      this.swsApi
        .swsPost('Employee/GetEmplyeeByDialysisList', {})
        .then(res => {
          if (res.data.success) {
            this.hospitalList = res.data.result
            // cy:默认机构选择康美透析中心
            this.hospitalCheckedId = this.hospitalList.filter(
              item => item.dialysisName === '康美透析中心'
            )[0].dialysisId

            this.getPatientList()
          } else {
            this.$Notice.error({
              title: '请求错误',
              desc: '网络错误，请稍后再试！'
            })
          }
        })
        .catch(e => {
          this.$Notice.error({
            title: '请求失败',
            desc: '网络请求失败，请稍后再试！'
          })
        })
    })
  },
  computed: {
    filterList: function () {
      let data = []
      if (this.staType === 0) {
        // 全部
        data = this.patientListData
      } else if (this.staType === 1) {
        // 在院就是转入
        data = toFilterKey(this.patientListData, 'sHospitalState', '转入')
      } else {
        // 其他
        data = toFilterKey(this.patientListData, 'sHospitalState', '转出')
        data = data.concat(toFilterKey(this.patientListData, 'sHospitalState', '死亡'))
        data = data.concat(toFilterKey(this.patientListData, 'sHospitalState', '离院'))
      }
      let fileData = toFilterKey(data, 'patientNo,sex,name,age', this.searchKey)
      return fileData
    }
  },
  methods: {
    changeHospital (id) {
      this.searchKey = ''
      this.hospitalCheckedId = id
      this.startPage = 1
      this.getPatientList()
    },
    changePatient (item) {
      this.patientCheckedInfo = {
        id: item.id,
        name: item.name
      }
      let hospitalName = this.hospitalList.filter(
        item => item.dialysisId == this.hospitalCheckedId
      )[0].dialysisName
      this.$emit('on-change', {
        patientId: item.id,
        patientName: item.name,
        hospitalId: this.hospitalCheckedId,
        hospitalName: hospitalName
      })
    },
    changeStaType (index) {
      this.staType = index
    },
    // 获取病人列表
    getPatientList () {
      this.inHospitalNum = 0
      this.treeLoading = true
      let args = {
        name: this.searchKey,
        centerId: this.hospitalCheckedId
        // cy 取消分页请求，
        // pageSize: this.pageSize,
        // pageNum: 1
      }
      this.swsApi
        .swsPost(this.api, args)
        .then(res => {
          this.treeLoading = false
          if (res.data.success) {
            this.patientListData = res.data.result
            this.dataCount = res.data.dataCount
            if (this.patientListData.length !== 0) {
              // cy 如果机构患者不为零，则默认选取第一个患者数据
              let { name, id } = this.patientListData[0]
              this.patientCheckedInfo = {
                name,
                id
              }
              this.changePatient(this.patientCheckedInfo)
              this.inHospitalNum = this.patientListData.filter(
                item => {
                  return item.sHospitalState === '转入'
                }
              ).length
            }
          } else {
            this.$Notice.error({
              title: '请求错误',
              desc: '网络错误，请稍后再试'
            })
          }
        })
        .catch(e => {
          this.treeLoading = false
          this.$Notice.error({
            title: '请求失败',
            desc: '后台人员正在紧急修复中，请稍后再试'
          })
        })
    }
  },
  components: {}
}
</script>

<style scoped lang="less">
.inner-content {
  position: relative;
  width: 100%;
  height: 100%;
  & .main {
    display: flex;
    width: 100%;
    height: 100%;
    & > * {
      height: 100%;
    }
    .aside {
      position: relative;
      flex: 0 0 200px;
      // padding-top: 54px;
      height: 100%;
      overflow: hidden;
      background: #ffffff;
      // cy:add border
      border: 1px solid #dcdee2;
      border-color: #e8eaec;
      & .title {
        position: relative;
        top: 0;
        left: 0;
        width: 100%;
        padding: 10px 20px 10px 11px;
        flex: 0 0 54px;
        font-size: 16px;
        font-weight: 500;
        z-index: 99;
        // background: #ffffff;
        border: 1px solid #eaeaea;
        color: #ffffff;
        background: #178fff;
        &::before {
          position: relative;
          top: -4px;
          margin-right: 10px;
          content: '';
          display: inline-block;
          vertical-align: bottom;
          width: 4px;
          height: 16px;
          // background: #178FFF;
          background: #ffffff;
        }
      }
      & .search-box {
        background: #f6f6f6;
        width: 100%;
        padding: 5px;
        /deep/ .ivu-input {
          width: 99%;
          font-size: 13px;
          border: none;
          outline: none;
          background-color: #f6f6f6;
          border: none;
        }
      }
    }
    .content {
      width: 100%;
      height: 100%;
      // overflow-y: auto;
      margin-left: 20px;
      .ivu-table-wrapper:not(.default-table) {
        border: none !important;
        & /deep/ .ivu-table {
          &::before,
          &::after {
            display: none !important;
          }
        }
        & /deep/ .ivu-table {
          font-size: 14px;
          border-bottom: none;
        }
      }
    }
    .pat_list {
      flex: 0 0 200px;
      width: 210px;
      height: 99.5%;
      position: relative;
      // top:2px;
      background: #f9f9f9;
    }

    .pat_cont {
      // margin:-16px;
      background: #ffffff;
      .pat_cont-t,
      .sta-list div,
      .sta-list li {
        display: flex;
        justify-content: space-around;
        list-style-type: none;
        span {
          height: 36px;
          line-height: 36px;
          /*color: #666666;*/
          text-align: center;
          overflow: hidden;
          white-space: nowrap;
          text-overflow: ellipsis;
          font-size: 13px;
        }
      }
      .red_12 {
        color: red !important;
        font-weight: normal;
        font-size: 10px !important;
      }
      .w-20 {
        width: 20%;
      }
      .w-45 {
        width: 45%;
      }
      .w-30 {
        width: 30%;
      }
      .w-35 {
        width: 35%;
      }
      .pat_hos {
        background: #ffffff;
        font-size: 12px;
        span:hover {
          cursor: pointer;
        }
      }
      /deep/ .ivu-select-default {
        // margin: 7px 4px 7px;
        // width: 96%;
        margin: 7px 0px;
        width: 100%;
        background: #ffffff;
        border: 1px solid #eaeaea;
        border-radius: 6px;
        // outline: none;
        // border: none;
        .ivu-select-selection {
          line-height: 34px;
          border: none;
          &:hover {
            outline: none;
          }
          .ivu-select-selected-value {
            height: 34px;
            font-size: 14px;
          }
        }
      }
      input {
        width: 98%;
        margin: 5px 2px;
        outline: none;
        border: none;
      }
      .sta-list {
        width: 100%;
        .pat_msg {
          width: 100%;
          top: 205px;
          bottom: 0;
          left: 0;
          position: absolute;
          z-index: 4;
          overflow-y: scroll;
          li:hover {
            cursor: pointer;
            /* background: #c7ebfa; */
            background-color: rgba(200, 235, 250, 0.5);
          }
          // .name {
          //   text-align: left !important;
          // }
        }
      }
    }
    .isOdd {
      background: #ffffff;
    }
    .isActive {
      background: #c7ebfa !important;
    }
    .isActive1 {
      background: #01a9db !important;
      color: #fff;
    }
    .sex {
      font-weight: bold;
    }
    .sex.nv {
      color: red;
    }
    .sex.nan {
      color: blue;
    }
  }
}
</style>
