<template>
  <div class="sws_container printbody">
    <div>
      <div class="header">
        <h2 style="text-align: center;">{{`${getTitle}`}}</h2>
      </div>
      <table class="border center">
        <has-header-body :datas="datas" :columns="columns" />
      </table>

      <!-- <table class="border" style="margin-top: 10px">
        <tr v-for="(item,index) in tableTotal" :key="index">
          <td>{{item.title}}</td>
          <td>{{item.desc}}</td>
        </tr>
      </table> -->
      <br/>
      <table class="border center">
        <has-header-body :datas="datas1" :columns="columns1" />
      </table>
    </div>
  </div>
</template>
<script>
import '@/view/print/print.less'
import {
  convertCurrency
} from '@/libs/tools.js'
import hasHeaderBody from '../common/tableHasHeader'
import {mapGetters} from 'vuex'
export default {
  name: '',
  data () {
    return {
      title: '',
      params: {},
      columns: [],
      datas: [],
      columns1: [],
      datas1: [],
      tableTotal: []
    }
  },
  components: {
    hasHeaderBody
  },
  async mounted () {
    let { params, title } = this.$route.params
    this.title = title
    await this.get(params)
    // await this.getDetail(params)
    // cy 改报表
    await this.getDetail1(params)

    this.print()
  },
  computed: {
    ...mapGetters(['getColumns', 'getApi', 'getTitle', 'getPParams'])},
  methods: {
    get (params) {
      let keys = Object.keys(this.getPParams)
      if (keys.includes('pageSize')) {
        this.getPParams.pageSize = 100000
      }
      return this.swsApi
        .swsPost('BusinessTargetClntroller/BusinessTarget/ALL', this.getPParams)
        .then(res => {
          if (res.data.success) {
            this.columns = res.data.result.tableHeaders
            this.datas = res.data.result.centerDayIncomeOutPuts
          }
        })
        .catch(e => {
          console.log(e)
        })
    },
    getDetail1 (params) {
      let keys = Object.keys(this.getPParams)
      this.getPParams.model = 3
      if (keys.includes('pageSize')) {
        this.getPParams.pageSize = 100000
      }
      return this.swsApi
        .swsPost('BusinessTargetClntroller/BusinessTarget/ALL', this.getPParams)
        .then(res => {
          if (res.data.success) {
            this.columns1 = res.data.result.tableHeaders
            this.datas1 = res.data.result.centerDayIncomeOutPuts
          }
        })
        .catch(e => {
          console.log(e)
        })
    },
    getDetail (params) {
      let keys = Object.keys(this.getPParams)
      if (keys.includes('pageSize')) {
        this.getPParams.pageSize = 100000
      }
      return this.swsApi
        .swsPost('BusinessTargetClntroller/BusinessTarget/DayReportAsync', this.getPParams)
        .then(res => {
          if (res.data.success) {
            let result = res.data.result
            let data = []
            for (let item in result) {
              let obj = {}
              switch (item) {
                case 'total':
                  obj.title = '收费情况'
                  obj.desc = `合计：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'individual':
                  obj.title = '个人帐户（职工）'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'jmindividual':
                  obj.title = '个人帐户（居民）'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'mzbz':
                  obj.title = '民政补助（职工）'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'jmmzbz':
                  obj.title = '民政补助（居民）'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'detc':
                  obj.title = '大额（职工）'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'jmdetc':
                  obj.title = '大额（居民）'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'ybjj':
                  obj.title = '统筹（职工）'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'jmybjj':
                  obj.title = '统筹（居民）'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'xj':
                  obj.title = '现金'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'yycb':
                  obj.title = '医院超标'
                  obj.desc = `收：￥${result[item]}/大写：${convertCurrency(result[item])}`
                  break
                case 'pjst':
                  obj.title = '票据收退'
                  obj.desc = `收费：${result[item] ? result[item] : '暂无'}`
                  break
                case 'hmfw':
                  obj.title = '号码范围'
                  obj.desc = result[item] ? result[item] : '暂无'
                  break
                case 'sjph':
                  obj.title = '实际票号'
                  obj.desc = result[item] ? result[item] : '暂无'
                  break
              }
              data.push(obj)
            }
            this.tableTotal = data
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
