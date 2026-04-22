<template>
  <div class="message-center-dropdown-wrapper">
    <div class="message-center-dropdow" @mouseenter="showDropDown" @mouseleave="hideDropDown">
      <Badge dot :count="dataList.length" :offset="[20, 0]">
        <Icon type="md-notifications-outline" size="26"></Icon>
      </Badge>
    </div>
    <transition name="slide">
      <div v-show="toggleDropDown" @mouseleave="hideDropDown" @mouseenter="showDropDown" class="dropdown-wrapper">
        <header class="dropdown-wrapper-header">
          <span class="left">消息中心</span>
          <span class="right">
            未读消息
            <span class="dataCount">({{dataList.length}})</span>
          </span>
        </header>
        <div class="dropdown-wrapper-content">
          <div class="message-list" v-if="dataList.length">
            <ul>
              <li v-for="item in dataList" :key="item.id">
                <div class="badge type-1" v-if="item.type == 1">公告</div>
                <div class="badge type-2" v-else-if="item.type == 2">审核</div>
                <div class="badge type-3" v-else>反馈</div>
                <div class="message-info">
                  <p class="content">{{item.content}}</p>
                  <span class="time">{{item.time}}</span>
                </div>
              </li>
            </ul>
          </div>
          <div class="empty" v-else>暂无未读消息</div>
        </div>
        <footer class="dropdown-wrapper-footer">
          <span class="left"><Icon type="ios-done-all" class="color-primary" size="26" />一键已读</span>
          <span class="right color-primary watch-all">查看全部</span>
        </footer>
      </div>
    </transition>
  </div>
</template>

<script>
export default {
  name: 'MessageCenter',
  data () {
    return {
      dataList: [
        {
          id: '1',
          type: '1',
          content:
            '这是消息中心这是消息中心这是消息中心这是消息中心这是下水线这是消息中心这是消息中心这是消息中心这是消息中心这是下水线',
          time: '2019-04-26 12:10'
        },
        {
          id: '2',
          type: '2',
          content: '111',
          time: '2019-04-26 12:10'
        },
        {
          id: '3',
          type: '3',
          content: '这是消息中心这是消息中心这是消息中心这是消息中心这是下水线',
          time: '2019-04-26 12:10'
        },
        {
          id: '4',
          type: '1',
          content: '这是消息中心这是消息中心这是消息中心这是消息中心这是下水线',
          time: '2019-04-26 12:10'
        },
        {
          id: '5',
          type: '2',
          content: '这是消息中心这是消息中心这是消息中心这是消息中心这是下水线',
          time: '2019-04-26 12:10'
        },
        {
          id: '6',
          type: '1',
          content: '这是消息中心这是消息中心这是消息中心这是消息中心这是下水线',
          time: '2019-04-26 12:10'
        }
      ],
      toggleDropDown: false,
      timeout: null
    }
  },
  methods: {
    showDropDown () {
      if (this.timeout) clearTimeout(this.timeout)
      this.timeout = setTimeout(() => {
        this.toggleDropDown = true
      }, 50)
    },
    hideDropDown () {
      if (this.timeout) {
        clearTimeout(this.timeout)
        this.timeout = setTimeout(() => {
          this.toggleDropDown = false
        }, 50)
      }
    }
  },
  components: {}
}
</script>

<style scoped lang="less">
.message-center-dropdown-wrapper {
  position: relative;
  margin: 0 15px 0 0;
  cursor: pointer;
  .dropdown-wrapper {
    display: flex;
    flex-direction: column;
    position: absolute;
    top: 60px;
    width: 320px;
    height: 480px;
    max-height: 480px;
    font-size: 14px;
    line-height: 26px;
    background: #ffffff;
    z-index: 100001;
    transform: translateX(-156px);
    border-radius: 4px;
    box-shadow: 0 5px 10px 0 rgba(0, 0, 0, 0.4);
    overflow: hidden;
    cursor: default;
    opacity: 1;
    transform-origin: top left;
    backface-visibility: visible;
    .left {
      float: left;
      cursor: pointer;
    }
    .right {
      float: right;
    }
    &-header,
    &-footer {
      color: #333;
      padding: 18px 15px;
      border-bottom: solid 1px #eaeaea;
      &::before,
      &::after {
        display: block;
        content: "";
        height: 0;
        visibility: hidden;
        clear: both;
      }
      .dataCount {
        color: #fc4b4b;
      }
    }
    &-content {
      flex: 1;
      overflow-y: auto;
      .empty {
        margin-top: 160px;
        text-align: center;
      }
      .message-list {
        ul li {
          display: flex;
          padding: 10px 15px;
          font-size: 12px;
          .badge {
            margin-right: 10px;
            // flex: 0 0 32px;
            width: 32px;
            height: 32px;
            text-align: center;
            line-height: 32px;
            color: #ffffff;
            border-radius: 32px;
            &.type-1 {
              background-color: #f17aa0;
            }
            &.type-2 {
              background-color: #ffb10a;
            }
            &.type-3 {
              background-color: #68b8f6;
            }
          }
          .message-info {
            flex: 1;
            .content {
              display: -webkit-box;
              -webkit-box-orient: vertical;
              -webkit-line-clamp: 2;
              overflow: hidden;
              line-height: 1.6;
              cursor: pointer;
              &:hover {
                text-decoration: underline;
              }
            }
          }
          .time {
            font-size: 14px;
            color: #999999;
          }
        }
      }
    }
    &-footer {
      color: #333;
      padding: 18px 15px;
      border-top: solid 1px #eaeaea;
      .watch-all {
        cursor: pointer;
      }
    }
  }
}

.slide-enter-active {
  animation-name: slide;
  animation-timing-function: ease-in-out;
  animation-duration: 0.6s;
  animation-fill-mode: backwards
}
// .slide-enter, .slide-leave-to /* .slide-leave-active in below version 2.1.8 */ {
//   opacity: 0;
//   height: 100px;
// }
@keyframes slide {
  0% {
    opacity: 0;
    // height: 100px;
    transform: translateX(-156px) rotateX(180deg);
    backface-visibility: hidden;
  }
  50% {
    opacity: 0;
    // height: 100px;
    transform: translateX(-156px) rotateX(180deg);
    backface-visibility: hidden;
  }
  100% {
    opacity: 1;
    // height: 480px;
    transform: translateX(-156px) rotateX(0);
    backface-visibility: hidden;
  }
}
</style>
