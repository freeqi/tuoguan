// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import store from './store'
import i18n from '@/locale'
import config from '@/config'
import importDirective from '@/directive'
// import installPlugin from '@/plugin'
// import base from './base'
import base from '@/libs/common_funcs'
// import iView from 'iview'
// import 'iview/dist/styles/iview.css'
import ViewUI from 'view-design';
// import style
import 'view-design/dist/styles/iview.css';
import './index.less'
// 实际打包时应该不引入mock
/* eslint-disable */

import swsApi from './libs/swsApi';

// Vue.use(ViewUI);
Vue.use(ViewUI, {
  i18n: (key, value) => i18n.t(key, value)
})

/**
 * @description  注册自定义全局事件
 */
Vue.use(base)

/**
 * @description 注册admin内置插件
 */
// installPlugin(Vue)
/**
 * @description 生产环境关掉提示
 */
Vue.config.productionTip = false
/**
 * @description 全局注册应用配置
 */
Vue.prototype.$config = config
Vue.prototype.swsApi = swsApi
/**
 * 注册指令
 */
importDirective(Vue)

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  i18n,
  store,
  render: h => h(App)
})
