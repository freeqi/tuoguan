import echarts from 'echarts'
import { on, off } from '@/libs/tools'
export default {
  name: 'serviceRequests',
  data () {
    return {
      dom: null
    }
  },
  methods: {
    resize () {
      this.dom.resize()
    }
  },
  mounted () {
    const option = {
      tooltip: {
        trigger: 'item',
        backgroundColor: 'white',
        padding: 0,
        textStyle: {
          color: '#666',
          fontSize: 13
        },
        formatter: params => {
          return `<h4 style='padding: 5px 10px; background: #f6f6f6; color: #999; font-style: normal;border-radius:5px'>${params.name}年机构数量 </h4>
            <p style='padding: 8px 10px;border-radius:5px'>${params.marker}机构量 :&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ${params.value}家</p >`
        },
        extraCssText: 'box-shadow: 0 0 4px rgba(0, 0, 0, 0.5);'
      },
      xAxis: {
        type: 'category',
        boundaryGap: false,
        axisLine: {
          show: false
        },
        axisLabel: {
          textStyle: {
            color: '#999',
            fontSize: 16
          }
        },
        axisTick: {
          show: false
        },
        data: [
          '2008',
          '2009',
          '2010',
          '2011',
          '2012',
          '2013',
          '2014',
          '2015',
          '2016',
          '2017',
          '2018'
        ]
      },
      yAxis: {
        type: 'value',
        name: '单位（家）',
        nameTextStyle: {
          color: '#999999',
          fontSize: 16
        },
        axisLine: {
          show: false
        },
        axisLabel: {
          textStyle: {
            color: '#999',
            fontSize: 16
          }
        },
        axisTick: {
          show: false
        }
      },
      color: '#4f95e8',
      series: [
        {
          data: [0, 2, 3, 3, 4, 5, 6, 6, 8, 8, 10],
          type: 'line',
          areaStyle: {
            color: '#dceafa'
          },
          lineStyle: {
            color: '#4f95e8'
          },
          emphasis: {
            itemStyle: {}
          }
        }
      ]
    }
    this.$nextTick(() => {
      this.dom = echarts.init(this.$refs.line_chart)
      this.dom.setOption(option)
      on(window, 'resize', this.resize)
    })
  },
  beforeDestroy () {
    off(window, 'resize', this.resize)
  }
}
