<template>
  <medical-quality-statistics-template
    :is-year="true"
    :api="api"
    :single-api="single_api"
    :tab-data="tab_data"
    :buttonPermission="buttonRole.NDZB_DC"
    :table-column-detail="table_column_detail"
  ></medical-quality-statistics-template>
</template>

<script>
import medicalQualityStatisticsTemplate from './template.vue'
const BUTTONROLE = {
  NDZB_DC: 'NDZB_DC'
}
export default {
  data () {
    return {
      tab_data: [
        {
          id: 1,
          name: '年度观察指标'
        },
        {
          id: 2,
          name: '感染监测'
        },
        {
          id: 3,
          name: '血管通路类别'
        },
        {
          id: 4,
          name: '年度统计指标'
        }
      ],
      api: 'MedicalIndexes/Observe/IndicatorsYear',
      single_api: 'MedicalIndexes/ObserveIndicatorsYear',
      table_column_detail: [
        {
          title: '指标',
          key: 'indicatorsName',
          minWidth: 120
        },
        {
          title: '上年度',
          key: 'lastYear',
          align: 'center',
          minWidth: 60,
          sortable: true,
          sortMethod: (a, b, type) => {
            if (type === 'asc') {
              return a - b
            } else {
              return b - a
            }
          }
        },
        {
          title: '本年度（截止月底）',
          key: 'currentYear',
          width: 145,
          align: 'center',
          sortable: true,
          sortMethod: (a, b, type) => {
            if (type === 'asc') {
              return a - b
            } else {
              return b - a
            }
          },
          // cy 注意:这里写死了字段名
          render: (h, params) => {
            if (params.row.indicatorsName !== '死亡总例数') {
              return <p>{params.row.currentYear}</p>
            } else {
              if (params.row.tags.length === 0) {
                return <p>{params.row.currentYear}</p>
              } else {
                let content = params.row.tags.map(item => {
                  return h('div', `${item.name},${item.age}岁,死亡时间:${item.tagTime}`)
                })
                return h('div', [
                  h('Poptip',
                    {
                      props: {
                        wordWrap: true,
                        transfer: true,
                        trigger: 'hover',
                        placement: 'top'
                      }
                    },
                    [
                      h('span', {'class': {'death-number': params.row.currentYear !== 0}}, params.row.currentYear),
                      h('div', {slot: 'content'}, content)
                    ]
                  )
                ])
              }
            }
          }
        },
        {
          title: '备注',
          key: 'remarks',
          align: 'center',
          tooltip: true,
          minWidth: 120
        }
      ],
      buttonRole: BUTTONROLE
    }
  },
  created () {},
  mounted () {
    this.$nextTick(() => {})
  },
  methods: {},
  components: {
    medicalQualityStatisticsTemplate
  }
}
</script>

<style scoped lang="less">
/deep/ .death-number {
  color: red;
  text-decoration-line: underline;
}
</style>
