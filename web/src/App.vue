<template>
  <div id="app">
    <router-view/>
  </div>
</template>

<script>
export default {
  name: 'App'
}
</script>

<style lang="less">
.size{
  width: 100%;
  height: 100%;
}
html,body{
  .size;
  overflow: hidden;
  margin: 0;
  padding: 0;
}
// cy新增两个质量管理的月度指标统计的特殊显示行样式
.ivu-table .big-font-cell{
  font-size: 15px;
  font-weight: 600;
}
.ivu-table .small-font-cell {
  text-align: right;
  font-size: 13px;
  font-weight: 400;
}
// cy 调整table的cell内padding
.ivu-table-wrapper {
  & /deep/ .ivu-table .ivu-table-cell {
    padding-right: 4px;
    padding-left: 5px;
  }
}
#app {
  .size;
}
</style>
