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
      _dom += `<p style='padding: 2px 10px'>${params.marker} ${params.seriesName} : ${params.value}人</p>`
      _dom += '</div>'
    } else {
      _dom = `<h4 style='padding: 5px 10px; background: #f6f6f6; color: #999; font-style: normal;'>${params[0].name}</h4><div style="padding: 6px">`
      params.map(v => {
        _dom += `<p style='padding: 2px 10px'>${v.marker} ${v.seriesName} : ${v.value}人</p>`
      })
      _dom += '</div>'
    }
    return _dom
  }
}
const option_type_1 = {
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
  tooltip: {
    trigger: 'item',
    formatter: (params) => {
      return `${params.marker}${params.name}：${params.value * 100}%`
    }
  },
  color: ['#3398DB'],
  series: [
    {
      // markLine: {
      //   symbol: ['none'],
      //   data: [{
      //     yAxis: .75
      //   }],
      //   label: {
      //     position: 'start',
      //     formatter: '标准:{c}'
      //   },
      //   lineStyle: {
      //     type: 'solid',
      //     color: '#999'
      //   },
      // },
      label: {
        show: true,
        color: '#333',
        formatter: (params) => {
          return `${params.data * 100}%`
        }
      },
      symbolSize: 8,
      data: [0.86, 0.87, 0.88, 0.85, 0.84, 0.81],
      type: 'line',
      smooth: true,
      itemStyle: {
        color: '#ee5151'
      },
      areaStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
          offset: 0,
          color: '#ee5151'
        }, {
          offset: 1,
          color: 'rgba(255, 255, 255, 0)'
        }])
      }
    }
  ]
}
// 多条折线
const option_type_line = {
  tooltip,
  color: ['#54d8ff', '#a4a1fb'],
  series: []
}
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
      start: 25,
      end: 75,
      filterMode: 'empty'
    },
    {
      type: 'inside',
      start: 25,
      end: 75
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
 * @param {是否合并柱状图} showStack
 */
const selectTypeToChange = (type, option, _data, showStack = true) => {
  let plugin
  // 柱形图圆环效果
  let itemStyle = {
    normal: {
      // 柱形图圆角，初始化效果
      barBorderRadius: [15, 15, 0, 0]
    }
  }
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
      barWidth: 30,
      data: []
    }
    showStack && (plugin.stack = '总数')
  }
  return changeOption(option, _data, plugin, (item, index, current) => {
    let {name, data} = item
    let _plugin = Object.assign({}, plugin)
    _plugin.name = name
    _plugin.data = data
    // 判断柱状图是否合并展示
    type === 'bar' && ('stack' in plugin) && current.length === (index + 1) && (_plugin = Object.assign({}, _plugin, {itemStyle}))
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
        isQualified: this.secondStateModel,
        pageSize: 10000,
        beginTime: this.beginTime,
        endTime: this.endTime,
        pageNum: 1
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
     * @param {是否选中月份} monthCheck
     */
    setChart (type, api, monthCheck = false, ...args) {
      if (!api) return false
      let params = {
        centerId: this.hospitalCheckedId,
        isQualified: this.secondStateModel,
        beginTime: this.beginTime,
        endTime: this.endTime
      }
      // if (monthCheck) {
      //   let [ date ] = args
      //   params = {}
      //   api += date
      // }
      this.swsApi
        .swsPost(api, params)
        .then(res => {
          this.loading = false
          if (res.data.success) {
            let _data = res.data
            let option
            // 治疗室消毒
            if (type === 1) {
              if (!monthCheck) {
                let names = []
                let datas = []
                option = Object.assign({}, base_type, option_type_1)
                _data.result.forEach(({name, value}) => {
                  names.push(name)
                  datas.push(value)
                })
                option.xAxis.data = names
                option.legend.data = ['消毒合格率']
                option.series[0].name = ['消毒合格率']
                option.series[0].data = datas
              } else {
                option = Object.assign({}, base_type, option_type_bar, data_zoom_plugin)
                option = selectTypeToChange('bar', option, _data)
              }
              // 新入患者传染病发病率
            } else if (type === 2) {
              option = Object.assign({}, base_type, option_type_bar, data_zoom_plugin)
              option = selectTypeToChange('bar', option, _data)
              // 新入患者传染病监测完成率检测 , 标本菌落数检验, 内毒素
            } else if ([3, 4, 5].includes(type)) {
              // if (!monthCheck) {
              //   option = Object.assign({}, base_type, option_type_line)
              //   option = selectTypeToChange('line', option, _data)
              // } else {
              if (this.hospitalCheckedId === '0') {
                option = Object.assign({}, base_type, option_type_bar, data_zoom_plugin)
                option = selectTypeToChange('bar', option, _data, true)
              } else {
                option = Object.assign({}, base_type, option_type_bar, data_zoom_plugin)
                option = selectTypeToChange('bar', option, _data, false)
              }
              // }
            }
            this.dom.clear()
            option.title.text = this.chartTitle
            this.dom.setOption(option)
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
