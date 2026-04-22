/* eslint-disable camelcase */
import echarts from 'echarts'
import { on, off } from '@/libs/tools'
const base_type = {
  backgroundColor: '#fff',
  title: {
    text: '',
    textStyle: {
      color: '#333',
      fontStyle: 'normal',
      fontSize: 16
    },
    x: 'center',
    y: '6'
  },
  grid: {
    top: '120',
    left: '3%',
    right: '4%',
    bottom: '38',
    containLabel: true
  },
  legend: {
    left: '3%',
    bottom: '0',
    textStyle: {
      color: '#999'
    },
    data: []
  },
  xAxis: {
    type: 'category',
    data: [],
    axisLine: {
      show: false
    },
    axisTick: {
      show: false
    },
    axisLabel: {
      textStyle: {
        color: '#999',
        fontSize: 12
      }
    }
  },
  yAxis: {
    type: 'value',
    axisLine: {
      show: false
    },
    axisTick: {
      show: false
    },
    axisLabel: {
      textStyle: {
        color: '#999',
        fontSize: 12
      }
    }
  }
}
const fixed_tooltip = {
  tooltip: {
    position: function (point, params, dom, rect, size) {
      // 固定在顶部
      return [point[0] + 20, '10%']
    },
    trigger: 'axis',
    backgroundColor: 'white',
    padding: 0,
    textStyle: {
      color: '#666',
      fontSize: '13'
    },
    extraCssText: 'box-shadow: 0 0 4px rgba(0, 0, 0, 0.5);',
    formatter: (params) => {
      let _dom
      if (!params[0]) {
        _dom = `<h4 style='padding: 5px 10px; background: #f6f6f6; color: #999; font-style: normal;'>${params.name}</h4><div style="padding: 6px">`
        _dom += `<p style='padding: 2px 10px'>${params.marker} ${params.seriesName} : ${params.value}</p>`
        _dom += '</div>'
      } else {
        _dom = `<h4 style='padding: 3px 10px; background: #f6f6f6; color: #999; font-style: normal;'>${params[0].name}</h4><div style="padding: 2px 6px">`
        params.map(v => {
          _dom += `<p style='padding: 2px 10px'>${v.marker} ${v.seriesName} : ${v.value}</p>`
        })
        _dom += '</div>'
      }
      return _dom
    }
  }
}
const tooltip = {
  trigger: 'axis',
  backgroundColor: 'white',
  padding: 0,
  textStyle: {
    color: '#666',
    fontSize: '13'
  },
  extraCssText: 'box-shadow: 0 0 4px rgba(0, 0, 0, 0.5);',
  formatter: (params) => {
    let _dom
    if (!params[0]) {
      _dom = `<h4 style='padding: 5px 10px; background: #f6f6f6; color: #999; font-style: normal;'>${params.name}</h4><div style="padding: 6px">`
      _dom += `<p style='padding: 2px 10px'>${params.marker} ${params.seriesName} : ${params.value}</p>`
      _dom += '</div>'
    } else {
      _dom = `<h4 style='padding: 3px 10px; background: #f6f6f6; color: #999; font-style: normal;'>${params[0].name}</h4><div style="padding: 2px 6px">`
      params.map(v => {
        _dom += `<p style='padding: 2px 10px'>${v.marker} ${v.seriesName} : ${v.value}</p>`
      })
      _dom += '</div>'
    }
    return _dom
  }
}
// 多条折线
// const option_type_line = {
//   tooltip,
//   color: ['#54d8ff', '#a4a1fb'],
//   series: []
// }
// 柱状图
const option_type_bar = {
  tooltip,
  color: ['#56d9fe', '#a4a1fb', '#8ec8da', '#ff9901'],
  series: []
}
const data_zoom_plugin = {
  grid: {
    top: '120',
    left: '3%',
    right: '4%',
    containLabel: true
  },
  legend: {
    top: '50',
    textStyle: {
      color: '#999'
    },
    data: []
  },
  dataZoom: [
    {
      show: true,
      type: 'slider',
      start: 40,
      end: 60,
      xAxisIndex: [0],
      filterMode: 'weakFilter'
    },
    {
      type: 'inside',
      start: 40,
      end: 60,
      xAxisIndex: [0],
      filterMode: 'weakFilter'
    }
  ]
}
/**
 *
 * @param {图表option配置} option
 * @param {返回series数据} _data
 * @param {图表series的配置} plugin
 * @param {回调函数} callback
 */
const changeOption = (option, _data, plugin, callback) => {
  option.series = _data.result.serises.map(callback)
  option.legend.data = _data.result.legendData
  option.xAxis.data = _data.result.data
  return option
}
/**
 * @description 根据类型处理数据
 * @param {类型} type
 * @param {图表option配置} option
 * @param {图表series的配置} _data
 */
const selectTypeToChange = (type, option, _data) => {
  let plugin
  if (type === 'line') {
    plugin = {
      name: '',
      type: 'line',
      data: [],
      smooth: true
    }
  } else if (type === 'bar') {
    plugin = {
      name: '',
      type: 'bar',
      barWidth: 11,
      data: []
    }
  }
  return changeOption(option, _data, plugin, (item, index, current) => {
    let { name, data } = item
    let _plugin = Object.assign({}, plugin)
    _plugin.name = name
    _plugin.data = data
    return _plugin
  })
}
export default {
  name: '',
  data () {
    return {
      dom: null
    }
  },
  methods: {
    // 下载图表
    download () {
      let args = {
        centerId: this.hospitalCheckedId,
        // isQualified: this.secondStateModel,
        pageSize: 10000,
        beginTime: this.beginTime,
        // endTime: this.endTime,
        pageNum: 1,
        title: {}
      }
      this.swsApi
        .swsPost(this.table_api, args)
        .then(res => {
          if (res.data.success) {
            // let data = res.data.result.map(item => {
            //   item.contactPhone += '\t'
            //   item.receiveDate += '\t'
            //   return item
            // })
            this.$refs.table.exportCsv({
              filename: `${this.hospitalCheckedName}${this.operateTitle}`,
              columns: this.table_column,
              data: res.data.result
            })
          } else {
            this.$Message.error('网络错误，请稍后再试')
          }
        })
        .catch(e => {
          this.loading = false
        })
    },
    resize () {
      this.dom.resize()
    },
    /**
     *
     * @param {类型} type
     * @param {接口地址} api
     */
    setChart (type, api, ...args) {
      if (!api) return false
      let [date] = args
      let params = {
        centerId: this.hospitalCheckedId,
        beginTime: date
      }
      this.swsApi
        .swsPost(api, params)
        .then(res => {
          this.loading = false
          if (res.data.success) {
            this.dom.dispose()
            this.dom = echarts.init(this.$refs.dom)
            let _data = res.data
            let option
            if (this.hospitalCheckedId === '0') {
              option = Object.assign({}, base_type, option_type_bar, data_zoom_plugin, fixed_tooltip)
              // console.log('option--------------------:')
              // console.dir(option)
              option = selectTypeToChange('bar', option, _data)
              // console.log('option+plugin--------------------:')
              // console.dir(option)
            } else {
              option = Object.assign({}, base_type, option_type_bar)
              option = selectTypeToChange('bar', option, _data)
            }
            option.title.text = this.chartTitle
            this.dom.setOption(option)

            this.dom.on('legendselectchanged', (params) => {
              if (_data.result.legendData.length === 1) return false
              this.legendTitle = params.selected
              this.startPage = 1
              this.getTableList(this.startPage)
            })
          } else {
            this.$Message.error('网络错误，请稍后再试')
          }
        })
    }
  },
  mounted () {
    this.$nextTick(() => {
      this.dom = echarts.init(this.$refs.dom)
      on(window, 'resize', this.resize)
    })
  },
  beforeDestroy () {
    off(window, 'resize', this.resize)
  }
}
