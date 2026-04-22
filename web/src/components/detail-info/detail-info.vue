<template>
  <div v-if="flage">
    <div class="more-info">
      <div class="top">
        <span class="txt">{{title}}</span>
        <Icon type="md-close" class="icon" size="18" @click="hide()" color="#c3c3c3"/>
      </div>
      <div class="content">
        <slot></slot>
      </div>
    </div>
    <div class="overlay" @click="hide"></div>
  </div>
</template>

<script>
// 详情卡
export default {
  data () {
    return {
      flage: false
    }
  },
  props: {
    title: {
      type: String,
      default: '详细信息'
    }
  },
  methods: {
    show () {
      this.flage = true
    },
    hide () {
      this.flage = false
    }
  },
  components: {

  }
}
</script>

<style scoped lang="less">

// .fade-enter-active,
// .fade-leave-active {
//   transition: opacity 0.05s;
// }
// .fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
//   opacity: 0;
// }
.more-info {
  position: relative;
  display: flex;
  flex-direction: column;
  font-size: 14px;
  height: 100%;
  background: #ffffff;
  box-shadow: 0 5px 10px 0 rgba(0, 0, 0, 0.4);
  z-index: 10;
  .top {
    padding: 16px 20px;
    font-size: 16px;
    line-height: 18px;
    border-bottom: 1px solid #f1f1f1;
    &::after {
      content: "";
      display: block;
      clear: both;
      height: 0;
      visibility: hidden;
    }
    .txt {
      float: left;
    }
    .icon {
      float: right;
      vertical-align: middle;
      cursor: pointer;
    }
  }
  .content {
    position: relative;
    flex: 1;
    overflow-y: auto;
    .info-box {
      padding: 10px;
      color: #333333;
      .blue {
        color: #4f95e8;
      }
      .main-info {
        padding-left: 10px;
        display: flex;
        align-items: center;
        height: 36px;
        // justify-content: center;
        & > div {
          margin-right: 16px;
        }
        & i {
          margin-right: 4px;
        }
      }
      .detail {
        padding-left: 40px;
        p {
          display: flex;
          align-items: center;
          min-height: 28px;
          .field {
            margin-right: 24px;
            label {
              margin-right: 3px;
              color: #999;
            }
          }
        }
      }
      &:last-child {
        margin-bottom: 20px;
      }
    }
  }
}
.overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 9;
}
</style>
