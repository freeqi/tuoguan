import echarts from 'echarts'
import { on, off } from '@/libs/tools'
export default {
  name: '',
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
                  formatter: '{a} <br/>{b}: {c}人'
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
                        formatter: '{b}{c}人',
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
                  formatter: '{a} <br/>{b}: {c}人'
                },
                color: ['#3398DB'],
                legend: {
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
                  name: '单位（人）',
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
                text: this.chartTitle,
                textStyle: {
                  color: '#333',
                  fontStyle: 'normal',
                  fontSize: 16
                },
                x: 'center',
                y: '40'
              },
              legend: {
                top: '70',
                right: '0',
                data: data.result.legendData
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
                  let _dom = `<h4 style='padding: 5px 10px; background: #f6f6f6; color: #999; font-style: normal;'>${params[0].name} </h4><div style="padding: 6px">`
                  params.map(v => {
                    _dom += `<p style='padding: 2px 10px'>${v.marker}${v.seriesName} : ${v.value}人</p>`
                  })
                  _dom += '</div>'
                  return _dom
                }
              },
              grid: {
                top: '120',
                left: '3%',
                right: '4%',
                bottom: '3%',
                containLabel: true
              },
              xAxis: {
                type: 'category',
                boundaryGap: false,
                data: data.result.data,
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
              color: ['#5eaffe', '#ff807d', '#67e0e3'],
              yAxis: {
                type: 'value',
                name: '单位（人）',
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
              series: data.result.serises.map(v => { v.type = 'line'; return v })
            }
            this.dom.setOption(option)
          } else {

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
