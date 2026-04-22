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
          desc: '图表统计出错，请稍后再试！'
        })
        return false
      }
      this.dom.clear()
      this.swsApi.swsGet(api).then(res => {
        let data = res.data
        let option = null
        if (data.result) {
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
              color: [
                '#5eaffe',
                '#ff807d',
                '#67e0e3',
                '#ffdb5c',
                '#ff9f7f',
                '#e7bcf3'
              ], // ['#37a2da', '#32c5e9', '#67e0e3', '#ffdb5c', '#ff9f7f', '#e7bcf3']
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
                  data: data.result
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
              legend: {},
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
                  data: data.result.map(v => {
                    return v.name
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
                  data: data.result.map(v => {
                    return v.value
                  })
                }
              ]
            }
          }
          option.legend.data = data.result.map(v => v.name)
          let newPlugin = {
            grid: {
              top: '120',
              left: '3%',
              right: '4%',
              containLabel: true
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
          if (data.result.length > 10) {
            let newOption = Object.assign({}, option, newPlugin)
            newOption.series[0].barWidth = 'auto'

            this.dom.setOption(newOption)
            return
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
