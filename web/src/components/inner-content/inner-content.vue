<template>
  <div class="inner-content">
    <div class="main">
      <!-- 侧边 -->
      <div class="aside">
        <Card :bordered="false" :padding="0">
          <p slot="title">透析机构列表</p>
          <ul class="hosiptal-group">
              <li class="item" v-for="item in hospitalList" :class="{'active': hospitalCheckedId === item.id}" @click="changeHospital(item.id)" :key="item.id">{{item.name+"(30人)"}}</li>
          </ul>
        </Card>
      </div>
      <!-- 主内容 -->
      <div class="content">
      </div>
    </div>

  </div>
</template>

<script>
export default {
  data () {
    return {
      hospitalCheckedId: 1
    }
  },
  props: {
    hospitalList: {
      type: Object
    }
  },
  methods: {
    // 切换医院
    changeHospital (id) {
      id = id - 0
      this.hospitalCheckedId = id
    }
  },
  components: {

  }
}
</script>

<style scoped lang="less">
.inner-content {
  position: relative;
  width: 100%;
  height: 100%;
  & /deep/ .ivu-card-head {
    padding: 15px 16px 14px;
    border-bottom: 1px solid #efefef !important;
    p {
      font-size: 16px;
      &::before {
        position: relative;
        top: -3px;
        margin-right: 10px;
        content: '';
        display: inline-block;
        vertical-align: bottom;
        width: 4px;
        height: 16px;
        background: #4f95e8;
      }
    }
  }
  & .main {
    display: flex;
    width: 100%;
    height: 100%;
    &>* {
      height: 100%;
      .ivu-card {
        height: 100%;
      }
    }
    .aside {
      flex: 0 0  250px;
    }
    .content {
      width: 0;
      overflow-y: auto;
      margin-left: 20px;
      flex: 1;
    }
  }
}
.hosiptal-group {
  height: 100%;
  li {
    margin: 0;
    line-height: 24px;
    padding: 10px 20px;
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
      background: #f2f8ff;
    }
    &.active {
      font-weight: bold;
      color: #4f95e8;
      background: #f2f8ff !important;
    }
  }
}
</style>
