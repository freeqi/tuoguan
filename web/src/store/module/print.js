/* eslint-disable no-unused-vars */

// import config from '@/config'
// import swsApi from '@/libs/swsApi'
import { printUtils } from '@/libs/util'

export default {
  state: {
    columns: printUtils.getColumns(),
    title: printUtils.getTitle(),
    params: printUtils.getPParams(),
    api: printUtils.getApi()
  },
  getters: {
    getColumns: state => state.columns,
    getApi: state => state.api,
    getTitle: state => state.title,
    getPParams: state => state.params
  },
  mutations: {
    setColumns (state, columns) {
      state.columns = columns
      printUtils.setColumns(columns)
    },
    setTitle (state, title) {
      state.title = title
      printUtils.setTitle(title)
    },
    setParams (state, params) {
      state.params = params
      printUtils.setParams(params)
    },
    setApi (state, api) {
      state.api = api
      printUtils.setApi(api)
    }
  },
  actions: {}
}
