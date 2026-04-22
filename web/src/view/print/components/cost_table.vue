<template>
  <div class="sws_container printbody">
    <div>
      <div class="header">
        <h2 style="text-align: center;">{{getTitle}}</h2>
      </div>

      <table class="border" style="margin-top: 10px">
        <tr v-for="(item,index) in datas" :key="index">
          <template>
            <td v-for="_item in item" :key="_item">{{_item}}</td>
          </template>
        </tr>
      </table>
    </div>
  </div>
</template>
<script>
import '@/view/print/print.less'
import { mapGetters } from 'vuex'
export default {
  name: '',
  data () {
    return {
      title: '',
      params: {},
      columns: [],
      datas: [],
      tableTotal: []
    }
  },
  async mounted () {
    await this.get()

    this.print()
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
      return this.swsApi
        .swsPost(this.getApi, this.getPParams)
        .then(res => {
          if (res.data.success) {
            this.datas = res.data.result.reduce((_res, current, index) => {
              let arr = current.title.split(',')
              _res.push(arr)
              return _res
            }, [])
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
