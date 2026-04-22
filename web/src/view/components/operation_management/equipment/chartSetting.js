/* eslint-disable no-unused-vars */
/* eslint-disable */
import echarts from 'echarts'
import { on, off } from '@/libs/tools'
const pieData = [
  {
    name: 'SWS-3000',
    value: 6
  },
  {
    name: 'SWS-3000A',
    value: 4
  },
  {
    name: 'SWS-4000',
    value: 6
  },
  {
    name: 'SWS-4000A',
    value: 5
  },
  {
    name: 'SWS-6000',
    value: 8
  },
  {
    name: 'SWS-6000A',
    value: 3
  }
]
const lineData = [
  {
    name: 'SWS-3000',
    data: [1, 2, 4, 6, 8, 7, 9, 16]
  },
  {
    name: 'SWS-3000A',
    data: [3, 4, 6, 7, 8, 9, 12, 10]
  },
  {
    name: 'SWS-4000',
    data: [4, 6, 8, 7, 5, 6, 9, 8]
  },
  {
    name: 'SWS-4000A',
    data: [8, 7, 8, 5, 7, 4, 6, 5]
  },
  {
    name: 'SWS-6000',
    data: [16, 13, 14, 14, 12, 13, 15, 14]
  },
  {
    name: 'SWS-6000A',
    data: [15, 14, 11, 16, 16, 12, 14, 16]
  },
  {
    name: '其他设备',
    data: [14, 12, 16, 14, 12, 11, 16, 8]
  }
]
const barData = [
  {
    name: '康美透析中心',
    value: 6
  },
  {
    name: '弹子石透析中心',
    value: 4
  },
  {
    name: '天府透析中心',
    value: 6
  },
  {
    name: '涪陵透析中心',
    value: 5
  },
  {
    name: '射洪透析中心',
    value: 8
  },
  {
    name: '忠县透析中心',
    value: 3
  }
]
/*
* 转换数据
@params d 数组
*/
const translateData = (d) => {
  return d.reduce((s, v, i) => {
    let o = {
      name: v.name,
      type: 'line',
      data: v.data
    }
    if (i === 0) {
      s = []
    }
    s.push(o)

    return s
  }, '')
}

let optionPie = {
  backgroundColor: '#fff',
  title: {
    text: '设备型号数量占比',
    textStyle: {
      color: '#333',
      fontStyle: 'normal',
      fontSize: 16
    },
    x: '20'
  },
  tooltip: {
    trigger: 'item',
    formatter: '{a} <br/>{b} : {c}台({d}%)'
  },
  legend: {
    orient: 'vertical',
    top: '20',
    left: '80%',
    data: pieData.map(v => { return v.name })
  },
  color: ['#37a2da', '#32c5e9', '#67e0e3', '#ffdb5c', '#ff9f7f', '#e7bcf3'],
  series: [
    {
      name: '设备型号数量占比',
      type: 'pie',
      radius: '55%',
      center: ['50%', '60%'],
      data: pieData,
      itemStyle: {
        emphasis: {
          shadowBlur: 10,
          shadowOffsetX: 10,
          shadowColor: 'rgba(0, 0, 0, 0.5)'
        }
      }
    }
  ]
}
let optionLine = {
  backgroundColor: '#fff',
  title: {
    text: '设备购入情况年限统计',
    textStyle: {
      color: '#333',
      fontStyle: 'normal',
      fontSize: 16
    },
    x: 'center'
  },
  tooltip: {
    trigger: 'axis',
    backgroundColor: 'white',
    padding: 0,
    textStyle: {
      color: '#666',
      fontSize: '13'
    },
    extraCssText: 'box-shadow: 0 0 4px rgba(0, 0, 0, 0.5);',
    formatter: (params) => {
      let _dom = `<h4 style='padding: 5px 10px; background: #f6f6f6; color: #999; font-style: normal;'>${params[0].name}年购入设备统计 </h4><div style="padding: 6px">`
      params.map(v => {
        _dom += `<p style='padding: 2px 10px'>${v.marker}购入${v.seriesName} : ${v.value}台</p>`
      })
      _dom += '</div>'
      return _dom
      // return `<h4 style='padding: 5px 10px; background: #f6f6f6; color: #999; font-style: normal;'>${params.name}年购入设备统计 </h4>
      // <p style='padding: 8px 10px'>${params.marker}购入${params.seriesName} : ${params.value}台</p>`
    }
  },
  legend: {
    orient: 'vertical',
    right: '20',
    top: '50',
    data: lineData.map(v => { return v.name })
  },
  grid: {
    left: '3%',
    right: '150',
    bottom: '3%',
    containLabel: true
  },
  xAxis: {
    type: 'category',
    boundaryGap: false,
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
    },
    data: ['2012', '2013', '2014', '2015', '2016', '2017', '2018']
  },
  yAxis: {
    type: 'value',
    name: '单位（台）',
    nameTextStyle: {
      color: '#999',
      align: 'left'
    },
    axisLine: {
      show: false
    },
    axisLabel: {
      textStyle: {
        color: '#999',
        fontSize: 12
      }
    },
    axisTick: {
      show: false
    }
  },
  color: ['#e17c78', '#759aa0', '#eedd78', '#73a373', '#f49f42', '#7289ab', '#e69d87'], // "#4f95e8"
  series: translateData(lineData)
}
let optionBar = {
  backgroundColor: '#fff',
  title: {
    text: '设备型号分布统计',
    textStyle: {
      color: '#333',
      fontStyle: 'normal',
      fontSize: 16
    },
    x: 'center'
  },
  legend: {
    top: '26',
    data: ['血透中心']
  },
  color: ['#3398DB'],
  tooltip: {
    trigger: 'item',
    // formatter: '{b}<br />SWS-2000A: {c}台'，
    formatter: (params) => {
      return `${params.name}<br />${params.marker}SWS-2000A:${params.value}台`
    },
    backgroundColor: 'rgba(0, 0, 0, .4)',
    position: function (point, params, dom, rect, size) {
      // 固定在顶部
      let x, y
      x = point[0] - size.contentSize[0] / 2
      y = size.viewSize[1] - rect.height - 60
      return [x, y]
    }
  },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '3%',
    containLabel: true
  },
  xAxis: [
    {
      type: 'category',
      data: barData.map(v => { return v.name }),
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
  ],
  yAxis: {
    type: 'value',
    name: '单位（台）',
    nameTextStyle: {
      color: '#999',
      align: 'left'
    },
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
  series: [
    {
      name: '血透中心',
      type: 'bar',
      barWidth: '30',
      itemStyle: {
        color: '#97ccfb'
      },
      emphasis: {
        itemStyle: {
          color: '#4f95e8'
        }
      },
      data: barData.map(v => { return v.value })
    }
  ]
}
export default {
  data () {
    return {
      dom: null
    }
  },
  methods: {
    resize () {
      this.dom.resize()
    },
    setPieChart (api) {
      if (!api) {
        this.$Notice.error({
          title: `网络错误`,
          desc: '请求错误，请稍后再试！'
        })
        return false
      }
      this.dom.clear()

      this.swsApi.swsGet(api)
        .then(res => {
          let data = res.data
          let option = null
          if (data.result && !data.result.data) {
            // 当返回类型超过5时，展现为柱状图
            if (data.result.length < 5) {
              option = {
                tooltip: {
                  trigger: 'item',
                  formatter: '{a} <br/>{b}: {c}台'
                },
                title: {
                  text: this.chartTitle,
                  textStyle: {
                    color: '#333',
                    fontStyle: 'normal',
                    fontSize: 16
                  },
                  x: 'center',
                  y: '50'
                },
                legend: {
                  orient: 'vertical',
                  top: 'middle',
                  left: '80%',
                  data: []
                },
                backgroundColor: '#fff',
                color: ['#5eaffe', '#ff807d', '#67e0e3', '#ffdb5c', '#ff9f7f', '#e7bcf3'], // ['#37a2da', '#32c5e9', '#67e0e3', '#ffdb5c', '#ff9f7f', '#e7bcf3']
                series: [
                  {
                    name: this.hospitalCheckedName,
                    type: 'pie',
                    radius: ['40%', '60%'],
                    center: ['50%', '60%'],
                    avoidLabelOverlap: false,
                    itemStyle: {
                      emphasis: {
                        shadowBlur: 10,
                        shadowOffsetX: 10,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                      }
                    },
                    labelLine: {
                      normal: {
                        show: true
                      }
                    },
                    label: {
                      normal: {
                        formatter: '{b}{c}台',
                        textStyle: {
                          fontSize: '14',
                          fontWeight: 'bold'
                        }
                      },
                      emphasis: {
                        show: true,
                        textStyle: {
                          fontSize: '20',
                          fontWeight: 'bold'
                        }
                      }
                    },
                    data: []
                  }
                ]
              }
            } else {
              option = {
                backgroundColor: '#fff',
                title: {
                  text: this.chartTitle,
                  textStyle: {
                    color: '#333',
                    fontStyle: 'normal',
                    fontSize: 16
                  },
                  x: 'center',
                  y: '50'
                },
                tooltip: {
                  trigger: 'item',
                  formatter: '{a} <br/>{b}: {c}台'
                },
                color: ['#3398DB'],
                legend: {
                  right: 0
                },
                grid: {
                  top: '120',
                  left: '3%',
                  right: '4%',
                  bottom: '3%',
                  containLabel: true
                },
                xAxis: [
                  {
                    type: 'category',
                    data: data.result.map(v => { return v.name }),
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
                ],
                yAxis: {
                  type: 'value',
                  name: '单位（台）',
                  nameTextStyle: {
                    color: '#999',
                    align: 'left'
                  },
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
                series: [
                  {
                    name: this.hospitalCheckedName,
                    type: 'bar',
                    barWidth: '30',
                    itemStyle: {
                      color: '#97ccfb'
                    },
                    emphasis: {
                      itemStyle: {
                        color: '#4f95e8'
                      }
                    },
                    label: {
                      normal: {
                        show: true,
                        textStyle: {
                          color: '#999',
                          fontSize: 12
                        },
                        position: 'top'
                      }
                    },
                    data: data.result.map(v => { return v.value })
                  }
                ]
              }
            }
            option.series[0].data = data.result
            option.legend.data = data.result.map(v => v.name)
            this.dom.setOption(option)
          } else if (data.result.data) {
            option = {
              title: {
                text: '全部设备分布统计',
                textStyle: {
                  color: '#333',
                  fontStyle: 'normal',
                  fontSize: 16
                },
                x: 'center',
                y: '40'
              },
              legend: {
                orient: 'vertical',
                top: 'middle',
                right: '20',
                data: data.result.legendData
              },
              backgroundColor: '#fff',
              tooltip: {
                trigger: 'axis',
                backgroundColor: 'white',
                padding: 0,
                textStyle: {
                  color: '#666',
                  fontSize: '13'
                },
                extraCssText: 'box-shadow: 0 0 4px rgba(0, 0, 0, 0.5);',
                formatter: (params) => {
                  let _dom = `<h4 style='padding: 5px 10px; background: #f6f6f6; color: #999; font-style: normal;'>${params[0].name} </h4><div style="padding: 6px">`
                  params.map(v => {
                    _dom += `<p style='padding: 2px 10px'>${v.marker}${v.seriesName} : ${v.value}台</p>`
                  })
                  _dom += '</div>'
                  return _dom
                }
              },
              grid: {
                top: '120',
                left: '3%',
                right: '160',
                bottom: '10%',
                containLabel: true
              },
              xAxis: {
                type: 'category',
                data: data.result.data.map(v => {
                  v = v.slice(0, 7) + '\n' + v.slice(7)
                  return v
                }),
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
                  // rotate: '90'
                }
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
              ],
              color: ['#5eaffe', '#ff807d', '#67e0e3'],
              yAxis: {
                type: 'value',
                name: '单位（台）',
                nameTextStyle: {
                  color: '#999',
                  align: 'left'
                },
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
              series: data.result.serises.map(v => {
                v.type = 'bar'
                v.stack = '总数'
                v.barWidth = 30
                return v
              })
            }
            this.dom.setOption(option)
          }
        })
        .catch(e => {
          console.log(e)
        })
    }
  },
  mounted () {
    this.$nextTick(() => {
      this.dom = echarts.init(this.$refs.chart)
      on(window, 'resize', this.resize)
    })
  },
  beforeDestroy () {
    off(window, 'resize', this.resize)
  }
}
