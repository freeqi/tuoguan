<template>
  <div class="sws_container printbody">
    <div>
      <div class="header">
        <h2 style="text-align: center;">{{getTitle}}</h2>
      </div>
      <table class="border center">
        <has-header-body :datas="datas" :columns="getColumns"/>
      </table>
    </div>
  </div>
</template>
<script>
import '@/view/print/print.less'
import hasHeaderBody from '../common/tableHasHeader'
import { mapGetters } from 'vuex'
export default {
  name: '',
  data () {
    return {
      title: '',
      params: {},
      columns: [],
      datas: []
    }
  },
  components: {
    hasHeaderBody
  },
  async mounted () {
    // let { params, title } = this.$route.params
    await this.get()
    this.print()
  },
  computed: {
    ...mapGetters(['getColumns', 'getApi', 'getTitle', 'getPParams'])
  },
  methods: {
    get () {
      this.swsApi
        .swsPost(this.getApi, this.getPParams)
        .then(res => {
          if (res.data.success) {
            let { result } = res.data
            this.datas = Array.isArray(result) ? result : result.costChangeData
          }
        })
        .catch(e => {
          console.log(e)
        })
    },
    print () {
      window.print()
    }
  }
}
</script>

<style lang="less" scoped="scoped">
.header {
  margin-bottom: 10px;
}
</style>
