<template>
  <div id="message_center">
    <aside class="menu-wrapper">
      <ul class="menu">
        <li
          v-for="item in menuList"
          :key="item.id"
          :class="[item.url === menuActived? 'active': '']"
          @click="changeMenuActived(item.url)"
        >
          <common-icon class="icon" :size="size" :type="item.icon || ''"/>
          <Badge :count="item.count" :offset="[18, -18]">
            <span>{{item.txt}}</span>
          </Badge>
        </li>
      </ul>
    </aside>
    <main class="content-box">
      <router-view></router-view>
    </main>
  </div>
</template>

<script>
import CommonIcon from '_c/common-icon'
export default {
  data () {
    return {
      size: 18,
      menuList: [
        {
          id: 1,
          txt: '监管日志',
          icon: 'ios-megaphone-outline',
          url: 'regulatory_log_notify'
          // count: 15
        },
        {
          id: 4,
          txt: '发布日志',
          icon: 'md-add',
          url: 'regulatory_log_publish'
        }
      ],
      menuActived: 'notify'
    }
  },
  created () {
    // this.$router.push({name: this.menuActived})
    let { name } = this.$router.currentRoute
    this.changeMenuActived(name)
  },
  watch: {
    $route (newRoute) {
      const { name } = newRoute
      this.changeMenuActived(name)
    }
  },
  methods: {
    changeMenuActived (url) {
      this.menuActived = url
      this.$router.push({ name: url })
    }
  },
  components: {
    CommonIcon
  }
}
</script>

<style scoped lang="less">
@blue: #2d8cf0;
#message_center {
  display: flex;
  height: 100%;
  & > * {
    background: #ffffff;
    overflow-y: auto;
  }
  .menu-wrapper {
    flex: 0 0 178px;
    .menu {
      li {
        padding-left: 20px;
        height: 48px;
        line-height: 48px;
        font-size: 14px;
        border-right: 2px solid transparent;
        transition: all 0.3s;
        cursor: pointer;
        /deep/ .ivu-badge-count {
          padding: 0 4px;
          min-width: 16px;
          height: 16px;
          line-height: 14px;
        }
        .icon {
          margin-right: 10px;
        }
        &:hover,
        &.active {
          color: @blue;
          border-color: @blue;
          background-color: #f8f8f9;
        }
      }
    }
  }
  .content-box {
    margin: 0 20px;
    flex: 1;
  }
}
</style>
