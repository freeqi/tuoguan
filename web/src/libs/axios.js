import axios from 'axios'
// import Cookies from 'js-cookie'
// import { Notice } from 'iview'
import { Notice } from 'view-design'
import store from '@/store'
import route from '@/router'
import { getToken } from '@/libs/util'
import config from '@/config'
const { baseURL } = config
const TIMEOUT = 300000
// 请求错误提示显示
let isRequestError = false
// 请求超时显示
let isTimeouteError = false

class HttpRequest {
  constructor (baseUrl = baseURL) {
    this.baseUrl = baseUrl
    this.queue = {}
    this.instance = null

    this.pending = []
    this.cancelToken = null

    this.request()
    return this.instance
  }
  getInsideConfig () {
    const config = {
      baseURL: this.baseUrl,
      headers: {}
    }
    return config
  }
  // 销毁请求实例
  destroy (url) {
    delete this.queue[url]
    if (!Object.keys(this.queue).length) {
      // Spin.hide()
    }
  }

  removePending (config, isResponse) {
    // console.log('this pending', this.pending)
    // 结果相应拦截返回的data为string类型
    let data = config.data
      ? isResponse
        ? JSON.stringify(JSON.parse(config.data))
        : config.data
      : ''
    for (let p in this.pending) {
      let url = isResponse ? config.url.split(config.baseURL)[1] : config.url
      if (this.pending[p].u === url + '&' + config.method + '&' + data) {
        // 当当前请求在数组中存在时执行函数体
        this.pending[p].f('重复请求') // 执行取消操作
        this.pending.splice(p, 1) // 把这条记录从数组中移除
      }
    }
    store.commit('setAxiosPending', this.pending)
  }

  interceptors (instance, url) {
    // 请求拦截
    instance.interceptors.request.use(
      config => {
        this.queue[url] = true
        // 每次发送请求之前,统一在http请求的header都加上token
        config.headers.token = getToken()
        this.removePending(config, false) // 在一个ajax发送前执行一下取消操作
        // eslint-disable-next-line new-cap
        config.cancelToken = new this.cancelToken(c => {
          let data = config.data ? JSON.stringify(config.data) : ''
          // 这里的ajax标识我是用请求地址&请求方式拼接的字符串，当然你可以选择其他的一些方式
          this.pending.push({
            u: config.url + '&' + config.method + '&' + data,
            f: c
          })
        })

        return config
      },
      error => {
        console.log(error)
        return Promise.reject(error)
      }
    )

    // 响应拦截
    instance.interceptors.response.use(
      res => {
        // 在一个ajax响应后再执行一下取消操作，把已经完成的请求从pending中移除
        this.removePending(res.config, true)
        if (res.data.error === '未授权的请求或授权已过期') {
          store.commit('setHasAccess', false)

          store.dispatch('handleLogOut')

          route.push({
            name: 'login' // 跳转到登录页
          })
        } else {
          this.destroy(url)
          const { data, status } = res
          return { data, status }
        }
      },
      error => {
        this.destroy(url)
        // 请求超时状态
        if (error.message.includes('timeout')) {
          console.log('超时了')
          if (!isTimeouteError) {
            isTimeouteError = true
            Notice.error({
              title: '请求超时',
              desc: '请求超时，请稍后再试！',
              onClose: () => {
                isTimeouteError = false
              }
            })
          }
          // 重复请求
        } else if (
          error.message.includes('重复请求') ||
          error.message.includes('取消上一次请求')
        ) {
          console.log(error.message)
        } else {
          // 可以展示断网组件
          // Notice.error({
          //   title: '请求错误',
          //   desc: '网络出错，请稍后再试!'
          // })

          if (!isRequestError) {
            isRequestError = true
            Notice.error({
              title: '请求错误',
              desc: '网络出错，请稍后再试',
              onClose: () => {
                isRequestError = false
              }
            })
          }
          console.log(error)
        }
        // addErrorLog(error.response)
          return Promise.reject(error)
      }
    )
  }

  request (options = {}) {
    const instance = axios.create({
      timeout: TIMEOUT,
      withCredentials: true,
      baseURL: this.baseUrl
    })
    this.cancelToken = axios.CancelToken
    // instance.baseURL = this.baseUrl.dev/
    options = Object.assign(this.getInsideConfig(), options)
    this.interceptors(instance, options.url)
    this.instance = instance
  }
}
export default HttpRequest
