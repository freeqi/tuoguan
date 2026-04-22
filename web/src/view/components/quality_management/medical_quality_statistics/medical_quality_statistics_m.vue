<template>
  <medical-quality-statistics-template
    :tab-data="tab_data"
    :api="api"
    :buttonPermission="buttonRole.YDZB_DC"
    :single-api="single_api"
    :table-column-detail="table_column_detail">
  </medical-quality-statistics-template>
</template>

<script>
import medicalQualityStatisticsTemplate from './template.vue'
const BUTTONROLE = {
  YDZB_DC: 'YDZB_DC'
}
export default {
  data () {
    return {
      tab_data: [
        {
          id: 1,
          name: '月度观察指标'
        },
        {
          id: 2,
          name: '患者人数'
        },
        {
          id: 3,
          name: '转归人数'
        },
        {
          id: 4,
          name: '月度统计指标'
        }
      ],
      api: 'MedicalIndexes/Observe/Indicators',
      single_api: 'MedicalIndexes/ObserveIndicators',
      table_column_detail: [
        {
          title: '指标',
          key: 'indicatorsName',
          align: 'left',
          minWidth: 120
          // width: 250
        },
        {
          title: '上月',
          key: 'lastMonth',
          align: 'center',
          sortable: true,
          minWidth: 40
        },
        {
          title: '本月',
          key: 'currentMonth',
          align: 'center',
          sortable: true,
          minWidth: 40,
          render: (h, params) => {
            if (params.row.indicatorsName !== '死亡') {
              return <p>{params.row.currentMonth}</p>
            } else {
              if (params.row.tags.length === 0) {
                return <p>{params.row.currentMonth}</p>
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
                      h('p', {'class': {'death-number': params.row.currentMonth !== 0}}, params.row.currentMonth),
                      h('div', {slot: 'content'}, content)
                    ]
                  )
                ])
              }
            }
          }
        },
        {
          title: '同比(-/+)',
          key: 'sameCompared',
          align: 'center',
          sortable: true,
          minWidth: 60
        },
        {
          title: '环比(-/+)',
          key: 'sequential',
          align: 'center',
          sortable: true,
          minWidth: 60
        },
        {
          title: '备注',
          key: 'remarks',
          align: 'center',
          tooltip: true,
          minWidth: 150,
          maxWidth: 300
          // maxWidth: 140
        }
      ],
      buttonRole: BUTTONROLE
    }
  },
  created () {
  },
  mounted () {
    this.$nextTick(() => {
    })
  },
  methods: {
  },
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
