<template>
  <div :class="['message_container', {'overflowHidden': isOverflowHidden}]">
    <header class="message_container_header header">
      <span class="left">全部公告</span>
      <div class="right operate-box">
        <!-- <span>
          <Icon type="ios-done-all" class="color-primary" size="26"/>一键已读
        </span> -->

        <Dropdown class="all-news" placement="bottom-end" @on-click="changeNewsType">
          <div>
            {{newTypeCheckedName}}
            <Icon type="md-arrow-dropdown" size="14"/>
          </div>
          <DropdownMenu slot="list">
            <DropdownItem :name="item.id" v-for="item in newsTypeList" :key="item.id">{{item.name}}</DropdownItem>
          </DropdownMenu>
        </Dropdown>
      </div>
    </header>
    <div class="message_container_content">
      <div class="table-wrapper">
        <Table
          :data="tableData"
          :row-class-name="rowClassName"
          :loading="tableLoading"
          :columns="columns"
        ></Table>
        <div class="pagination"  v-if="dataCount > 10">
          <Page
            class="page"
            v-show="dataCount"
            :total="dataCount"
            show-total
            :current="startPage"
            :page-size="pageSize"
            @on-change="changePage"
          ></Page>
        </div>
      </div>
    </div>
    <div class="message_detail" v-show="isOverflowHidden">
      <Button type="default" icon="md-undo" @click="handleBack">返回</Button>
      <div class="article-header">
        <div class="badge">日志</div>
        <div class="article-info">
          <h3>{{atricleDetail.msgTitle}}</h3>
          <span class="time">{{atricleDetail.sendTime}}</span>
        </div>
      </div>
      <div class="article-content" v-html="atricleDetail.msgContent"></div>
    </div>
  </div>
</template>

<script>
import { getDate } from '@/libs/tools'
export default {
  data () {
    return {
      tableData: [],
      tableLoading: false,
      columns: [
        {
          title: '标题',
          key: 'msgTitle'
        },
        {
          title: '发布人',
          key: 'userName',
          width: 120
        },
        {
          title: '通知时间',
          key: 'sendTime',
          width: 220
        },
        {
          title: '操作',
          width: 120,
          render: (h, params) => {
            return (
              <span class="color-primary" style={{'cursor': 'pointer'}} onClick={() => this.showDetail(params.row)}>
                查看详情
              </span>
            )
          }
        }
      ],
      dataCount: 0,
      startPage: 1,
      pageSize: 10,
      isOverflowHidden: false,
      newsTypeCheckedId: 0,
      newsTypeList: [{ id: 0, name: '全部日志' }, { id: 1, name: '我的发布' }],
      atricleDetail: {
        msgContent: '',
        msgTitle: '',
        sendTime: '',
        adjunct: '',
        id: ''
      }
    }
  },
  computed: {
    newTypeCheckedName () {
      return this.newsTypeList.filter(
        item => item.id === this.newsTypeCheckedId
      )[0].name
    }
  },
  mounted () {
    this.getList(this.startPage)
  },
  methods: {
    changePage (page) {
      this.getList(page)
    },
    showDetail (detail) {
      this.atricleDetail = detail
      this.isOverflowHidden = true
    },
    // 已读置灰
    rowClassName (row, index) {
      // console.log(row)
    },
    handleBack () {
      this.isOverflowHidden = false
    },
    // 获取公告列表
    getList (page) {
      let params = {
        allOrSelf: this.newsTypeCheckedId,
        beginTime: '',
        endTime: '',
        pageSize: this.pageSize,
        type: 2,
        pageNum: page
      }
      this.tableLoading = true
      this.swsApi.swsPost('Information/Information/List', params)
        .then(res => {
          let data = res.data
          if (data.success) {
            this.tableData = data.result.map(item => {
              let timeStamp = new Date(item.sendTime).getTime() / 1000
              item.sendTime = getDate(timeStamp, 'year')
              return item
            })
            this.dataCount = data.dataCount
          }
          this.tableLoading = false
        })
        .catch(e => { this.tableLoading = false })
    },
    changeNewsType (id) {
      this.newsTypeCheckedId = id
      this.startPage = 1
      this.getList(this.startPage)
    }
  },
  components: {}
}
</script>

<style scoped lang="less">
@import '../style.less';
.message_container {
  position: relative;
  .all-news {
    margin-left: 10px;
    padding-left: 10px;
    // border-left: 1px solid #8d9ea6;
    color: #666 !important;
  }
  .message_detail {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    padding: 25px 10px 25px 25px;
    background: #ffffff;
    overflow-y: auto;
    z-index: 999;
    .article-header {
      display: flex;
      padding: 20px 0 10px;
      border-bottom: 1px solid #eaeaea;
      .badge {
        margin-right: 10px;
        width: 32px;
        height: 32px;
        text-align: center;
        line-height: 32px;
        color: #ffffff;
        border-radius: 32px;
        background-color: #f17aa0;
      }
      h3 {
        font-size: 22px;
        font-weight: normal;
        line-height: 32px;
      }
      .time {
        font-size: 14px;
        color: #999999;
      }
    }
    .article-content {
      padding: 20px;
    }
  }
}
</style>
