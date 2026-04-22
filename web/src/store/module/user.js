/* eslint-disable camelcase */
import swsApi from '@/libs/swsApi'
import {
  setToken,
  getToken,
  getRouters,
  setRouters,
  getRouteList,
  getButtons,
  setButtons,
  getMenuByRouter,
  getMenu,
  setMenu,
  getUserId,
  setUserId
} from '@/libs/util'
import config from '@/config'

export default {
  state: {
    userName: '',
    empName: sessionStorage.getItem('empName'),
    userId: getUserId(),
    avatorImgPath: '',
    token: getToken(),
    access: '',
    hasGetInfo: false,
    hasAccess: true, // 判断用户账号是否在别处登录
    routesList: getRouters(),
    buttons: getButtons(),
    menuList: getMenu()
  },
  getters: {
    getButtons: state => state.buttons,
    getMenuList: (state, getters, rootState) => {
      return getMenuByRouter(state.menuList)
    }
  },
  mutations: {
    setAvator (state, avatorPath) {
      state.avatorImgPath = avatorPath
    },
    setUserId (state, id) {
      state.userId = id
      setUserId(id)
    },
    setUserName (state, name) {
      state.userName = name
    },
    setEmpName (state, name) {
      state.empName = name
    },
    setAccess (state, access) {
      state.access = access
    },
    setHasAccess (state, access) {
      state.hasAccess = access
    },
    setToken (state, token) {
      state.token = token
      setToken(token)
    },
    setHasGetInfo (state, status) {
      state.hasGetInfo = status
    },
    setRoutesList (state, routes) {
      state.routesList = routes
      setRouters(routes)
    },
    setButtons (state, buttons) {
      state.buttons = buttons
      setButtons(buttons)
    },
    setMenu (state, list) {
      state.menuList = list
      setMenu(list)
    }
  },
  actions: {
    // 登录
    handleLogin ({ commit }, { userName, pwd }) {
      return new Promise((resolve, reject) => {
        swsApi
          .swsLogin('User/login', { userName, pwd })
          .then(res => {
            let data = res.data
            if (data.success) {
              const { menu, button, account } = data.result
              const {
                token: TOKEN,
                empName: EMPNAME,
                userName: USERNASME,
                id: USERID
              } = account
              // const TOKEN = account.token
              // const EMPNAME = account.empName
              const MENU = JSON.stringify(menu)
              let router_default = config.defaultRoutes
              let router_user = getRouteList(menu)
              let routers = router_default.concat(router_user)
              const BUTTON = JSON.stringify(button)
              // const USERNASME = account.userName
              // const USERID = account.employeeId

              commit('setToken', TOKEN)
              commit('setUserName', USERNASME)
              commit('setEmpName', EMPNAME)
              commit('setUserId', USERID)
              commit('setButtons', BUTTON)
              commit('setRoutesList', routers)
              commit('setMenu', menu)

              // 缓存用户信息及相关权限
              setMenu(MENU)
              sessionStorage.setItem('empName', EMPNAME)
              sessionStorage.setItem('userName', USERNASME)
            }
            resolve(res)
          })
          .catch(e => reject(e))
      })
    },
    // 退出登录
    handleLogOut ({ state, commit }) {
      return new Promise((resolve, reject) => {
        /*   logout(state.token).then(() => {
            commit('setToken', '')
            commit('setAccess', [])
            resolve()
          }).catch(err => {
            reject(err)
          }) */
        // 如果你的退出登录无需请求接口，则可以直接使用下面三行代码而无需使用logout调用接口
        commit('setToken', '')
        commit('setUserId', '')
        commit('setTagNavList', []) // 清空缓存列表
        commit('setAccess', [])
        commit('setRoutesList', config.defaultRoutes)
        sessionStorage.setItem('Button', JSON.stringify('[]'))
        sessionStorage.setItem('Menu', '')
        sessionStorage.setItem('empName', '')
        setRouters([])
        resolve()
      })
    }
  }
}
