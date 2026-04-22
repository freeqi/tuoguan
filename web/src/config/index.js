const config = {
  /**
   * @description token在Cookie中存储的天数，默认1天
   */
  cookieExpires: 1,
  /**
   * @description 是否使用国际化，默认为false
   *              如果不使用，则需要在路由中给需要在菜单中展示的路由设置meta: {title: 'xxx'}
   *              用来在菜单中显示文字
   */
  useI18n: false,
  /**
   * @description api请求基础路径
   */
  baseURL: {
    // dev: 'http://192.168.10.245:8083/api/',
    // dev: 'http://192.168.10.100:8085/api/',//测试环境
    dev: 'http://localhost:12345/api/',//本地环境
    // dev: 'http://192.168.10.121:9010/api/',
    // dev: 'http://192.168.10.245:20231/api/',
    // pro: 'http://192.168.10.245:8084/api/'
    // pro: 'http://192.168.30.252:8088/api/'
    // pro: 'http://192.168.10.22:20231/api/'
    pro: 'http://localhost:12345/api/' //集团端
    // pro: 'http://192.168.10.245:8083/api/',
    // pro: 'http://172.16.6.252:8085/api/' //涪陵
  },
  /**
   * @description 默认打开的首页的路由name值，默认为home
   */
  homeName: 'home',
  /**
   * @description 需要加载的插件
   */
  plugin: {
    'error-store': {
      showInHeader: false, // 设为false后不会在顶部显示错误日志徽标
      developmentOff: false // 设为true后在开发环境不会收集错误信息，方便开发中排查错误
    }
  },
  /**
   * @description 在首页中对不在左侧菜单里的路由需要进行的选中
   */
  filterDatas: {
    organization_chart: [
      'organization_chart',
      'organization_details'
    ],
    message_center: [
      'approve',
      'notify',
      'publish',
      'feedback'
    ],
    regulatory_log: [
      'regulatory_log_notify',
      'regulatory_log_publish'
    ]
  },
  defaultRoutes: [
    'home',
    '_home',
    'login',
    'error_404',
    'error_401',
    'error_500',
    'organization_details',
    'message_center'
  ]
}
export default config
