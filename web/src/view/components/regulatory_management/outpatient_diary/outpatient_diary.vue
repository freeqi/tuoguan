<template>
  <div id="diary">
    <parent-view2
      @on-change="changeHospital"
      ref="tree">
      <template slot-scope="slotProps">{{`${slotProps.dialysis.dialysisName}`}}</template>
      <div slot="content">
        <div class="diary-operate">
          <div class="operate">
            <div class="operate-search">
              <Row :gutter=20>
                <Col :lg="24" :md="24" class="btn-groups">
                  <span>时间：</span>
                  <DatePicker format="yyyy-MM-dd" v-model="beginTime" placement="bottom-end" placeholder="请选择日期" style="width: 260px;" @on-change="selecteDate"></DatePicker>
                </Col>
              </Row>
            </div>
          </div>
          <Table :loading="loading" class="default-table" :columns="table_column" :data="diary_data"></Table>
          <div style="margin: 10px;overflow: hidden" v-if="diaryDataCount > 10">
            <div style="float: right;">
              <Page :total="diaryDataCount" :page-size="pageSize" :current.sync="startPage" @on-change="changePage"></Page>
            </div>
          </div>
        </div>
      </div>
    </parent-view2>
  </div>
</template>

<script>
import parentView2 from '@/components/parent-view/parent-view2.vue'
export default {
  data () {
    return {
      loading: true,
      // 透析机构列表
      hospitalCheckedId: '0',
      hospitalCheckedName: '',
      hospitalList: [],

      // 表格
      table_column: [
        {
          title: '姓名',
          key: 'patientName',
          fixed: 'left',
          width: 70
        },
        {
          title: '就诊日期',
          key: 'diagnosisDate',
          width: 150
        },
        {
          title: '发病时间',
          key: 'morbidityDate',
          width: 150
        },
        {
          title: '性别',
          key: 'sex',
          align: 'center',
          width: 60
        },
        {
          title: '年龄',
          key: 'age',
          align: 'center',
          width: 60
        },
        {
          title: '职业',
          key: 'professional',
          width: 80,
          align: 'center'
        },
        {
          title: '现住详细地址',
          key: 'address',
          minWidth: 120
        },
        {
          title: '主诉',
          key: 'symptoms',
          minWidth: 140
        },
        {
          title: '诊断',
          key: 'diagnosis',
          minWidth: 240
        },
        {
          title: '复诊',
          key: 'subsequentVisit',
          minWidth: 80,
          align: 'center'
        },
        {
          title: '血压',
          key: 'bloodPressure',
          minWidth: 100,
          align: 'center'

        },
        {
          title: '疫情报告',
          align: 'center',
          children: [
            {
              title: '报告人',
              key: 'reportUser',
              align: 'center',
              width: 100
            },
            {
              title: '报告日',
              key: 'reportDate',
              align: 'center',
              minWidth: 160
              // tooltip: true,
              // width: 120
            }
          ]
        }
      ],
      diary_data: [],
      pageSize: 10,
      diaryDataCount: 0,
      startPage: 1,
      // 时间状态类型，范围
      diaryState: '',
      beginTime: new Date()
    }
  },
  created () {
    // 获取机构列表
    this.$nextTick(() => {

    })
  },
  computed: {
  },
  mounted () {

  },
  // mixins: [chartSetting],
  methods: {
    // 切换医院
    changeHospital (id, list) {
      this.searchKey = ''
      list.length && (this.hospitalList = list)
      this.hospitalCheckedId = id

      this.hospitalCheckedName = this.hospitalList.filter(v => v.dialysisId === id)[0].dialysisName
      sessionStorage.setItem('hospitalCheckedId', id)
      // 重置筛选条件
      this.startPage = 1
      this.getDiaryList(this.startPage)
    },
    // 分页
    changePage (index) {
      this.getDiaryList(index)
    },
    // 获取日志列表
    getDiaryList (i) {
      i = i || 1
      let args = {
        pageSize: this.pageSize,
        pageNum: i,
        beginTime: this.beginTime,
        centerId: this.hospitalCheckedId
      }
      this.loading = true
      this.swsApi.swsPost('HospitalFeeling/OutpatientDetailsLog/List', args)
        .then(res => {
          this.loading = false
          if (res.data.success) {
            this.diary_data = res.data.result
            this.diaryDataCount = res.data.dataCount
          } else {
            this.$Message.info(res.data.error)
          }
        })
        .catch(e => {
          this.loading = false
        })
    },
    selecteDate (v) {
      this.beginTime = v
      this.startPage = 1
      this.getDiaryList(this.startPage)
    }
  },
  components: {
    parentView2
  }
}
</script>

<style scoped lang="less">
#diary {
  /deep/ .content {
    background: #fff;
  }
  position: relative;
  width: 100%;
  height: 100%;
  .chart {
    height: 480px;
    padding: 20px;
    background: #ffffff;
    position: relative;
    .btn-group {
      position: absolute;
      right: 40px;
      z-index: 10;
      /deep/ .ivu-btn-primary {
        background: #4f95e8;
        border-color: #4f95e8;
        box-shadow: 2px 2px 6px rgba(79, 149, 232, .35);
      }
    }
  }
  .diary-operate {
    padding: 20px 0;
    background: #ffffff;
    .operate {
      padding: 0 20px;
      margin-bottom: 20px;
      // .operate-search .ivu-row > * {
      //    margin-top: 10px;
      // }
    }
    .organization-list {
      margin-top: 20px;
    }
  }
}
.btn-groups {
  color: #999;
  font-size: 13px;
  margin-bottom: 10px;
  /deep/ .ivu-radio-group-button .ivu-radio-wrapper-checked {
    color: #ffffff;
    background: #4f95e8;
  }
}
.ivu-table-wrapper {
  /deep/ .ivu-table th, .ivu-table td {
    border-right: 1px solid #e8eaec;
  }
}
</style>
