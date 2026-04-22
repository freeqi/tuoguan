<template>
  <div id="system-log">
    <div class="btn-groups">
      <span>时间：</span>
      <RadioGroup v-model="timeCheck" type="button" @on-change="handleOnDayChange">
        <Radio label="今天"></Radio>
        <Radio label="昨天"></Radio>
        <Radio label="最近7天"></Radio>
        <Radio label="最近30天"></Radio>
      </RadioGroup>
      <DatePicker v-model="time" :value="dateSelect" format="yyyy-MM-dd HH:mm:ss" type="daterange" placement="bottom-end" placeholder="请选择" style="width: 200px; margin-left: 20px;" @on-change="selected"></DatePicker>
    </div>
    <div class="btn-groups">
      <span>操作：</span>
      <RadioGroup v-model="operateCheck" type="button" @on-change="handleOnOperChange">
        <Radio label="增加"></Radio>
        <Radio label="删除"></Radio>
        <Radio label="修改"></Radio>
      </RadioGroup>
    </div>
    <Divider></Divider>
    <Table style="margin-top: 20px" :columns="table_columns" :data="table_data" :loading="loading"></Table>
    <div style="margin:16px; text-align:right" v-if="total>size">
      <Page
        show-elevator
        :total="total"
        :current.sync="current"
        @on-change="handleChangePage" />
    </div>
  </div>
</template>

<script>
export default {
  data () {
    return {
      type: '',
      timeCheck: '今天',
      operateCheck: '',
      dateSelect: [],
      total: 0,
      current: 1,
      size: 10,
      loading: false,
      table_columns: [
        {
          title: '访问时间',
          key: 'createTime',
          sortable: true
        },
        {
          title: '访问用户名',
          key: 'employeeName'
        },
        {
          title: '操作类型',
          key: 'logType'
        },
        {
          title: '备注',
          key: 'logContent'
        }
      ],
      table_data: [],
      statuCode: -1,
      day: 1,
      time: '',
      startTime: '',
      endTime: ''
    }
  },
  components: {

  },
  mounted () {
    this.handleChangePage()
  },
  methods: {
    // 分页
    handleChangePage () {
      if (this.loading) return
      this.loading = true
      let pageParams = {
        pageNum: this.current,
        pageSize: this.size
      }
      if (this.statuCode > 0) pageParams.logCode = this.statuCode
      if (this.day > -2) pageParams.day = this.day
      pageParams.startTime = this.startTime
      pageParams.endTime = this.endTime
      this.swsApi.swsPost('Data/log/list', pageParams).then(res => {
        // console.log(res)
        this.table_data = res.data.result
        this.loading = false
        this.total = res.data.dataCount
      })
    },
    getData (params) {
      this.loading = true
      this.swsApi.swsPost('Data/log/list', params).then(res => {
        // console.log(res)
        this.table_data = res.data.result
        this.loading = false
        this.total = res.data.dataCount
      })
    },
    // 按增加，删除，修改, 日期筛选
    handleOnOperChange (e) {
      this.current = 1

      switch (e) {
        case '增加':
          this.statuCode = 1004

          break

        case '删除':
          this.statuCode = 1003

          break

        case '修改':
          this.statuCode = 1002
          break
      }

      let pageParams = {
        day: this.day,
        logCode: this.statuCode,
        pageNum: this.current,
        pageSize: this.size,
        startTime: this.startTime,
        endTime: this.endTime
      }
      this.getData(pageParams)
    },
    handleOnDayChange (e) {
      if (this.time) {
        this.time = ''
        this.startTime = ''
        this.endTime = ''
      }
      this.current = 1

      switch (e) {
        case '今天':
          this.day = 1
          break

        case '昨天':
          this.day = -1
          break

        case '最近7天':
          this.day = 7
          break

        case '最近30天':
          this.day = 30
          break
      }

      let pageParams = {
        pageNum: this.current,
        logCode: this.statuCode,
        pageSize: this.size,
        startTime: this.startTime,
        endTime: this.endTime,
        day: this.day
      }
      this.getData(pageParams)
    },
    selected (v, d) {
      let pageParams = {
        pageNum: this.current,
        pageSize: this.size
      }
      let getDataByDatePiker = () => {
        this.swsApi.swsPost('Data/log/list', pageParams).then(res => {
          // console.log(res)
          this.table_data = res.data.result
          this.loading = false
          this.total = res.data.dataCount
        })
      }
      let startTime = v[0]
      let endTime = v[1]
      this.startTime = startTime
      this.endTime = endTime

      this.day = 0
      this.timeCheck = ''

      pageParams.startTime = startTime
      pageParams.endTime = endTime
      pageParams.logCode = this.statuCode
      pageParams.day = 0
      getDataByDatePiker()
    }
  }

}

</script>

<style scoped lang="less">
#system-log {
  padding: 20px;
  background: #ffffff;
  height: 100%;
  .btn-groups {
    color: #999;
    font-size: 13px;
    & + .btn-groups {
      margin: 20px 0 ;
    }
    /deep/ .ivu-radio-group-button .ivu-radio-wrapper-checked {
      color: #ffffff;
      background: #4f95e8;
    }
  }
    // cy:调大table的字体
  .ivu-table-wrapper {
    & /deep/ .ivu-table td {
      font-size: 13px;
    }
  }
}
</style>
