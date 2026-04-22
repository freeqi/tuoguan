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
import {
  formateDateToString
} from '@/libs/tools.js'
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
  // mounted() {
  //   let { params, title } = this.$route.params
  //   this.get(params)
  //   this.title = title
  // },
  async mounted () {
    await this.get()
    // this.print()
  },
  computed: {
    ...mapGetters(['getColumns', 'getApi', 'getTitle', 'getPParams'])
  },
  methods: {
    get (params) {
      let keys = Object.keys(this.getPParams)
      if (keys.includes('pageSize')) {
        this.getPParams.pageSize = 100000
      }
      this.swsApi
        .swsPost(this.getApi, this.getPParams)
        .then(res => {
          if (res.data.success) {
            let result = res.data.result
            if (Array.isArray(result)) {
              this.datas = result

              if(this.datas[0].jsrq){
                this.datas.forEach(item=>{
                  item.jsrq = formateDateToString(new Date(item.jsrq),'yyyy-MM-dd hh:mm:ss')
                })
              }

            } else {
              this.datas = res.data.result.centerDayIncomeOutPuts
            }
            this.$nextTick(_ => {
              this.print()
            })
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
