import _axios from './api.request'
// import iView from 'iview'
import { Message } from 'view-design'
import { getToken } from '@/libs/util'
import { Safety } from '@/assets/javascript/aes_1.js'
import store from '@/store'

// let axiosCancelFlage = false

const swsGet = (api, parameters) => {
  let res = ''
  if (!parameters) {
    parameters = ''
    res = _axios.get(api)
  } else {
    let jmparameters = JSON.stringify(parameters)
    res = _axios.get(api + '?data=' + jmparameters)
  }
  return res
}

const swsPost = (api, parameters) => {
  parameters = parameters || {}
  let res = _axios.post(api, parameters)
  // axiosCancelFlage = false
  return res
}

const swsPut = (api, parameters) => {
  // console.log(parameters)
  let jmparameters = JSON.stringify(parameters)
  let formData = new FormData()
  formData.append('data', jmparameters)
  let fheader = {
    headers: {
      Account: sessionStorage.getItem('userName'),
      token: getToken()
    }
  }
  let res = _axios.put(api, formData, fheader)
  res.then(response => {
    if (response.data.Code !== 200) {
      Message.error(response.data.Msg)
    }
  })
  return res
}

const swsLogin = (api, data) => {
  // console.log(api,data)
  for (let key in data) {
    data[key] = Safety.Encrypt(data[key])
  }
  // console.log('swsLogin1')
  // console.dir(_axios)
  let res = _axios.post(api, data)
  res.then(response => {
    if (response.data.success) {
      store.commit('setHasAccess', true)
      sessionStorage.setItem('userName', data.userName)
    }
  })
  // axiosCancelFlage = false
  return res
}
/**
 *
 * @param {string} api 退出登录
 * @param {*string} account 账号
 */
const swsLogout = (api, account) => {
  let jmaccount = account
  let res = _axios.put(api + jmaccount)
  res.then(response => {
    if (response.data.Code === 300) {
      Message.error(response.data.Msg)
    }
  })
  // axiosCancelFlage = false
  return res
}
/**
 * @description  获取数据字典
 * @param {array} datas 数组，包含接口地址和参数
 */
const swsAllPost = datas => {
  if (datas.length <= 0) return

  let requestList = datas.map(({ url, params }) => {
    return params ? swsPost(url, params) : swsPost(url)
  })

  // axiosCancelFlage = false
  return Promise.all(requestList)
}
// 可取消的请求
const swsPostCouldCancel = (api, parameters) => {
  let pending = store.state.app.pending
  pending.length && pending[pending.length - 1].f('取消上一次请求')
  parameters = parameters || {}
  let res = _axios.post(api, parameters)
  return res
}

// 下载文件
const swsDownload = (options) => {
  if (Object.prototype.toString.call(options) === '[object Object]') {
    return _axios(options)
  }
}

export default {
  swsGet,
  swsPost,
  swsPut,
  swsLogin,
  swsLogout,
  swsAllPost,
  swsPostCouldCancel,
  swsDownload
}
