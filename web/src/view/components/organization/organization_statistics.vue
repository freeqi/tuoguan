<template>
  <div>
    <Card>
      <Row>
        <Col
          :sm="24"
          :md="24"
          :lg="24"
          style="text-align:center;font-size:18px;color:#333333"
        >机构分布统计</Col>
      </Row>
      <Row>
        <Col :sm="15" :md="15" :lg="15">
          <div :style="{height:'500px'}" ref="myEchart"></div>
        </Col>
        <Col :sm="6" :md="6" :lg="6" style="margin-top:105px">
          <Table stripe :columns="columns" :data="data"></Table>
        </Col>
      </Row>
    </Card>
    <Card style="margin-top:20px">
      <Row>
        <Col
          :sm="24"
          :md="24"
          :lg="24"
          style="text-align:center;font-size:18px;color:#333333"
        >机构增长统计</Col>
        <Col
          :sm="24"
          :md="24"
          :lg="24">
          <div ref="line_chart" style="width:100%;height:320px" class="echarts-parent"></div>
        </Col>
      </Row>
      <!-- <span style="margin-right:18px;color:#666;font-size:16px;padding-left:35px;">选择日期</span><DatePicker type="daterange" transfer placement="bottom-end" placeholder="请选择日期" style="width: 300px"></DatePicker> -->
    </Card>
  </div>
</template>

<script>
import echarts from 'echarts'
import { on, off } from '@/libs/tools'
import '../../../../node_modules/echarts/map/js/china.js'

export default {
  name: 'echarts',
  data () {
    return {
      myChart: null,
      line_chart: null,
      // 表头
      columns: [
        {
          title: '地区',
          key: 'retionName',
          className: 'demo-table-info-row'
        },
        {
          title: '机构数',
          key: 'dialysisCount',
          className: 'demo-table-info-row'
        },
        {
          title: '占比',
          key: 'percentage',
          className: 'demo-table-info-row'
        }
      ],
      // 表格数据
      data: [],
      // 中国地图相关配置
      chinaOption: {
        // 进行相关配置
        backgroundColor: '#fff',
        tooltip: {
          // 鼠标移到图里面的浮动提示框
          trigger: 'item',
          backgroundColor: 'rgba(0, 0, 0, .7)',
          padding: 0,
          textStyle: {
            color: '#666',
            fontSize: 13
          },
          formatter: params => {
            // console.log(params.data)
            let fragment = `<h4 style='padding: 5px 10px; color: #fff; font-style: normal;font-size:16px'>${
              params.name
            }</h4>
                            <p style='padding: 8px 10px;color:#fff;'>${
  params.marker
}机构数量 : &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style='font-size:16px'>${params.value ||
              0}家</span></p >`

            if (params.data) {
              return `<h4 style='padding: 5px 10px; color: #fff; font-style: normal;font-size:16px'>${params.name}</h4>
                      <p style='padding: 8px 10px;color:#fff;'>${params.marker}机构数量 : &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style='font-size:16px'>${params.value}家</span></p >
                      <p style='padding: 8px 10px;color:#fff;'>${params.marker}占比 : &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style='font-size:16px'>${params.data.percent}</span></p >`
            } else {
              return fragment
            }
          },
          extraCssText: 'box-shadow: 0 0 4px rgba(0, 0, 0, 0.5);'
        },
        visualMap: {
          type: 'continuous',
          min: 0,
          max: 20,
          left: 'center',
          top: 'bottom',
          text: ['高', '低'],
          calculable: false,
          orient: 'horizontal',
          inRange: {
            color: ['#dceafa', '#78a3c6']
          }
        },
        geo: {
          map: 'china', // 表示中国地图
          roam: true,
          label: {
            normal: {
              show: true, // 是否显示对应地名
              textStyle: {
                color: 'rgba(0,0,0,0.6)'
              }
            }
          },
          itemStyle: {
            normal: {
              borderColor: '#f3f3f3',
              areaColor: '#d7e3ef'
            },
            emphasis: {
              areaColor: null,
              shadowOffsetX: 0,
              shadowOffsetY: 0,
              shadowBlur: 20,
              borderWidth: 0,
              shadowColor: 'rgba(0, 0, 0, 0.5)'
            }
          }
        },
        series: [
          {
            type: 'scatter',
            coordinateSystem: 'geo' // 对应上方配置
          },
          {
            // name: '机构统计', // 浮动框的标题
            type: 'map',
            mapType: 'china',
            roam: true,
            label: {
              normal: {
                show: true // 省份名称
              },
              emphasis: {
                areaColor: 'lightsteelblue',
                show: false
              }
            },
            geoIndex: 0,
            data: [
            ]
          }
        ]
      },
      // 折线图配置
      lineChartOption: {
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
          data: []
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
            data: [],
            symbolSize: 10,
            smooth: true,
            type: 'line',
            areaStyle: {
              color: '#dceafa'
            },
            lineStyle: {
              color: '#4f95e8'
            }
          }
        ]
      },

      // domChina
      domChina: null,
      domLine: null
    }
  },
  mounted () {
    this.handleGetDataByCity() // 根据地区查询
    this.handleGetDataByTime() // 根据时间查询
    on(window, 'resize', this.resize)
  },
  // mixins: [chartSetting],
  beforeDestroy () {
    off(window, 'resize', this.resize)
  },
  methods: {
    resize () {
      this.domLine.resize()
      this.domChina.resize()
    },
    // 中国地图
    chinaConfigure () {
      this.domChina = echarts.init(this.$refs.myEchart)
      this.domChina.setOption(this.chinaOption)
    },
    // 折线图
    lineChartConfigure () {
      this.domLine = echarts.init(this.$refs.line_chart)
      this.domLine.setOption(this.lineChartOption)
    },
    // 根据城市获取
    handleGetDataByCity () {
      this.swsApi.swsGet('CenterDialysis/StatisticalByCity').then(res => {
        // console.log(res.data.result)
        const data = res.data.result
        let array = []
        data.forEach(item => {
          let o = {
            name: item.retionName,
            value: item.dialysisCount,
            percent: item.percentage
          }
          if (item.retionName === '重庆市') {
            o.name = '重庆'
          }
          array.push(o)
        })
        this.data = res.data.result // 表格数据
        this.chinaOption.series[1].data = array
        this.chinaConfigure() // 中国地图
      })
    },
    // 根据时间获取
    handleGetDataByTime () {
      this.swsApi
        .swsGet('CenterDialysis/DialysisStatisticalByYear')
        .then(res => {
          // console.log(res.data.result)
          const data = res.data.result
          let yearArray = []
          let countArray = []
          data.forEach(item => {
            // console.log(item)
            let year = item.year
            let count = item.dialysisCount
            yearArray.push(year)
            countArray.push(count)
          })
          // console.log(yearArray, countArray)
          this.lineChartOption.xAxis.data = yearArray
          this.lineChartOption.series[0].data = countArray
          this.lineChartConfigure()
        })
    }
  }
}
</script>

<style lang='less'>
.ivu-table-header thead tr .demo-table-info-row {
  background-color: #4f95e8;
  color: #fff;
}
</style>
