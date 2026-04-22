<template>
  <div class="inner-content">
    <div class="main">
      <!-- 侧边 -->
      <div class="ivu-card aside">
        <p class="title">透析机构列表</p>
        <div class="search-box">
          <Input v-model="searchKey" clearable placeholder="  搜索..." />
        </div>
        <!-- <slot name="aside"></slot> -->
        <Spin v-if="treeLoading" fix></Spin>
        <div class="hospital" v-else>
          <ul class="hospital-group" ref="hospitalTree" v-if="filterList.length">
            <li
              class="item"
              v-for="(item, index) in filterList"
              :title="`${item.dialysisName}`"
              :class="{'active': hospitalCheckedId === item.dialysisId}"
              @click="change(item.dialysisId)"
              :key="item.dialysisId"
            >
              <slot
                :dialysis="item"
                :index="index"
              >{{`${index !== 0 ? `${index}.` : ''}${item.dialysisName}(${item.emlyeeCount})`}}</slot>
            </li>
          </ul>
          <div v-else class="empty">暂无数据</div>
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
export default {
  data () {
    return {
      hospitalList: [],
      hospitalCheckedId: -1,
      treeLoading: true,
      searchKey: ''
    }
  },
  props: {
    api: {
      require: true,
      default: 'Employee/GetEmplyeeByDialysisList'
    }
  },
  created () {
    this.$nextTick(() => {
      this.getHospital()
    })
  },
  computed: {
    filterList: function () {
      let data = toFilterKey(this.hospitalList, 'dialysisName', this.searchKey)
      return data
    }
  },
  methods: {
    // cy:搜索机构
    // keySearch () {
    // console.log('keySearch---------------------', this.searchKey)
    // let dom = this.$refs.hospitalTree.filter(item =>item.dialysisId==id)[0]
    // console.dir(this.$refs.hospitalTree, this.keySearch)
    // toFilterKey(hospitalList,)
    // },
    // 获取机构
    getHospital () {
      this.treeLoading = true
      this.swsApi
      // 20191230 cy 机构列表新增集团总部人数统计 （仅在职工统计页面显示）
        .swsPost('Employee/GetEmplyeeByDialysisList', {'empcount': this.$route.name === 'staff_statistics' ? 1 : 0})
        .then(res => {
          if (res.data.success) {
            this.hospitalList = res.data.result
            if (sessionStorage.getItem('hospitalCheckedId') && sessionStorage.getItem('hospitalCheckedId') !== '0') {
              this.hospitalCheckedId = sessionStorage.getItem(
                'hospitalCheckedId'
              )
            } else if(this.hospitalList.length) {
              // this.hospitalCheckedId = res.data.result[0].dialysisId
              // 设置首选为康美透析中心 20200420 cy 此处写死了
              // this.hospitalCheckedId = this.hospitalList.filter(
              //   v => v.dialysisName === '康美透析中心'
              // )[0].dialysisId
              this.hospitalCheckedId = this.hospitalList[0].dialysisId
              sessionStorage.setItem('hospitalCheckedId', this.hospitalCheckedId)
            }
            // console.log('getHospital', this.hospitalCheckedId, this.hospitalList)
            this.change(this.hospitalCheckedId, this.hospitalList)
          }
          this.treeLoading = false
        })
        .catch(e => {
          this.treeLoading = false
          console.log('错误：', e)
        })
    },
    // 切换医院 list为空说明是通过点击事件触发的
    change (id, list = []) {
      // console.log('change', id, list)
      if (this.hospitalCheckedId === id && !list.length) return
      this.hospitalCheckedId = id
      this.$emit('on-change', this.hospitalCheckedId, list)
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
      padding-top: 92px;
      height: 100%;
      overflow: hidden;
      background: #ffffff;
      // cy:add border
      border: 1px solid #dcdee2;
      border-color: #e8eaec;
      & .title {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        padding: 15px 16px 14px;
        flex: 0 0 54px;
        // border-bottom: 1px solid #efefef !important;
        font-size: 16px;
        font-weight: bold;
        z-index: 9;
        &::before {
          position: relative;
          top: -4px;
          margin-right: 10px;
          content: '';
          display: inline-block;
          vertical-align: bottom;
          width: 4px;
          height: 16px;
          background: #4f95e8;
        }
      }
      & .search-box {
        position: absolute;
        top: 54px;
        left: 0;
        background: #f6f6f6;
        width: 100%;
        height: 38px;
        padding: 3px;
        /deep/ .ivu-input {
          font-size: 13px;
          border: none;
          outline: none;
          background-color: #f6f6f6;
          border: none;
          box-shadow: none;
        }
      }
    }
    .content {
      width: 0;
      overflow-y: auto;
      margin-left: 20px;
      flex: 1;
      /deep/ .ivu-table-wrapper:not(.default-table) {
        border: none !important;
        & /deep/ .ivu-table {
          &::before,
          &::after {
            display: none !important;
          }
        }
        & /deep/ .ivu-table {
          font-size: 14px;
          // cy:取消透析表头的颜色
          // background: #f2f8ff;
          border-bottom: none;
        }
      }
    }
  }
}
.hospital {
  height: 100%;
  overflow-y: scroll;
  margin-right: -7px;
  background: #ffffff;
  .hospital-group {
    li {
      margin: 0;
      line-height: 24px;
      padding: 10px 16px;
      clear: both;
      color: #333;
      font-size: 14px !important;
      list-style: none;
      cursor: pointer;
      text-overflow: ellipsis;
      overflow: hidden;
      white-space: nowrap;
      -webkit-transition: background 0.2s ease-in-out;
      transition: background 0.2s ease-in-out;
      &:hover {
        background: #c7ebfa;
      }
      &.active {
        font-weight: bold;
        color: #4f95e8;
        background: #c7ebfa !important;
      }
    }
  }
  .empty {
    margin-top: 40px;
    text-align: center;
    font-size: 16px;
  }
}
</style>
