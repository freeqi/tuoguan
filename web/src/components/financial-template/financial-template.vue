<template>
  <div class="financial-content">
    <div class="main">
      <!-- 侧边 -->
      <div class="ivu-card aside">
        <p class="title">
          <slot name="title">报表</slot>
        </p>
        <ul class="aside-list" v-if="tabData.length">
          <li
            class="aside-list-item"
            v-for="item in tabData"
            :key="item.childKey"
            :title="item.childValue"
            @click="changeTab(item.childKey)"
            :class="tabCheckedId == item.childKey ? 'active' : ''"
          >{{item.childValue}}<Badge v-if="item.childKey==5" :dot="stockExcessiveCount>0" :offset="[-10,-5]"></Badge></li>
        </ul>
        <div v-else class="empty">暂无数据</div>
      </div>
      <!-- 主内容 -->
      <div class="content">
        <slot name="content"></slot>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data () {
    return {
      isGetRoute: false,
      stockExcessiveCount: 0,
      tabCheckedId: '',
      firstLoad: true
    }
  },
  props: {
    tabData: {
      type: Array,
      default: () => {
        return []
      }
    },
    loadHospital: {
      type: Boolean,
      default: true
    },
    defaultTabCheckedId: {
      default: '1'
    }
  },
  mounted () {
    this.loadHospital && this.getHospital()
    this.changeTab(this.defaultTabCheckedId)
  },
  watch: {
    // $route: {
    //   handler (newRoute) {
    //     if (newRoute.name == "statistics") {
    //       console.log(newRoute);
    //       this.getStockExcessive()
    //     }
    //   },
    // },
    tabCheckedId (newVal,oldVal){
      if(this.$route.name==='statistics'){
        console.log(111);
        this.getStockExcessive()
      }
    }
  },
  methods: {
    getStockExcessive(){
      this.swsApi.swsGet('/CenterDocking/StockExcessive')
        .then(res=>{
          if(res.data.success){
            this.stockExcessiveCount = res.data.result.stockExcessiveCount
          }
        })
        .catch(e=>{
          console.log(e);
        })
    },
    // 左側tab切換
    changeTab (key) {
      if (key === this.tabCheckedId) return
      this.tabCheckedId = key
      if (this.firstLoad) {
        this.firstLoad = false
      } else {
        this.$emit('on-change', key)
      }
    },
    // 获取机构
    getHospital () {
      return this.swsApi
        .swsPost('Employee/GetEmplyeeByDialysisList')
        .then(res => {
          if (res.data.success) {
            this.$emit('on-initial-hospital', res.data.result)
          }
        })
        .catch(e => console.log(e))
    }
  },
  components: {}
}
</script>

<style scoped lang="less">
.financial-content {
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
      padding-top: 54px;
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
        z-index: 99;
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
      .aside-list {
        height: 100%;
        overflow-y: auto;
        .aside-list-item {
          margin: 0;
          line-height: 24px;
          padding: 10px 15px;
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
    .content {
      background: #ffffff;
      width: 0;
      overflow-y: auto;
      margin-left: 20px;
      flex: 1;
      .ivu-table-wrapper:not(.default-table) {
        border: none !important;
        & /deep/ .ivu-table {
          font-size: 14px;
          // cy:取消透析表头的颜色
          // background: #f2f8ff;
          border-bottom: none;
          &::before,
          &::after {
            display: none !important;
          }
        }
        & /deep/ .ivu-table .ivu-table-cell {
          padding-right: 2px;
          padding-left: 2px;
        }
      }
      & /deep/ .ivu-table th,
      & /deep/ .ivu-table td {
        height: 39px;
      }
    }
  }
}
</style>
