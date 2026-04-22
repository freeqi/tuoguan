import Vue from 'vue'
import Vuex from 'vuex'

import user from './module/user'
import app from './module/app'
import patient from './module/patient'
import print from './module/print'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    //
  },
  mutations: {
    //
  },
  actions: {
    //
  },
  modules: {
    user,
    app,
    patient,
    print
  }
})
