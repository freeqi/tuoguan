import Vue from 'vue'
import Router from 'vue-router'
import routes from './routers'
import store from '@/store'
// import iView from 'iview'
import iView from 'view-design'
import { getToken } from '@/libs/util'
import config from '@/config'
// import axios from 'axios'
// const CancelToken = axios.CancelToken
const { homeName } = config

Vue.use(Router)
const router = new Router({
  routes,
  mode: 'hash'
})
const LOGIN_PAGE_NAME = 'login'

const turnTo = (to, next) => {
  let _routes = store.state.user.routesList
  if (
    _routes.some(item => {
      return to.name === item
    })
  ) {
    next()
    // eslint-disable-next-line brace-style
  }
  // 有权限，可访问
  else next({ replace: true, name: 'error_401' }) // 无权限，重定向到401页面
}

router.beforeEach((to, from, next) => {
  // CancelToken.source.cancel && CancelToken.source.cancel()
  iView.LoadingBar.start()
  const token = getToken()
  // 判断账号是否在其他地方登录
  let hasAccess = store.state.user.hasAccess
  if (!hasAccess) {
    iView.Message.error({
      content: '未授权的请求或授权已过期',
      duration: 3
    })
  }
  // cy print 跳转
  // fh 修改当name中不存在print问题
  if (to.fullPath.indexOf('print') > 0) {
    next()
  } else if (!token && to.name !== LOGIN_PAGE_NAME) {
    // 未登录且要跳转的页面不是登录页
    next({
      name: LOGIN_PAGE_NAME // 跳转到登录页
    })
  } else if (!token && to.name === LOGIN_PAGE_NAME) {
    // 未登陆且要跳转的页面是登录页
    next() // 跳转
  } else if (token && to.name === LOGIN_PAGE_NAME) {
    // 已登录且要跳转的页面是登录页
    next({
      name: homeName // 跳转到homeName页
    })
  } else {
    turnTo(to, next)
  }
})

router.afterEach(to => {
  iView.LoadingBar.finish()
  window.scrollTo(0, 0)
})

export default router
