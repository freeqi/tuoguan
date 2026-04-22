<template>
  <div class="sws-aside">
    <h4 class="title">{{name}}</h4>
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: 'sws-aside',
  data () {
    return {

    }
  },
  props: {
    name: {
      type: String,
      default: '标题'
    }
  },
  components: {

  }
}
</script>

<style scoped lang="less">
.sws-aside {
  width: 265px;
  border-right: 1px solid #eaeaea;
  .title {
    padding-left: 20px;
    color: #ffffff;
    font-size: 16px;
    line-height: 36px;
    background: #ff7b9e;
  }
  ul>li {
    display: block;
    padding-left: 20px;
    font-size: 14px;
    line-height: 48px;
    color: #515a6e;
    cursor: pointer;
    transition: all .1s;
    &:hover, &.active {
      background: #f4f3f3;
    }
  }
}

</style>
