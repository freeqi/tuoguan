<template>
  <div class="approve_container message_container">
    <header class="tab_header header">
      <span
        v-for="item in tabList"
        :key="item.id"
        :class="{'active': tabActivedId == item.id}"
        @click="changeTab(item.id)"
      >{{item.txt}}</span>
    </header>
    <main class="approve_container_content">
      <div class="table-wrapper">
        <Table :data="tableData" :loading="tableLoading" :columns="columns"></Table>
        <div class="pagination" v-if="dataCount > 10">
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
    </main>

    <Modal v-model="detailModal" title="反馈消息" width="550" @on-cancel="detailModalCancel">
      <div class="feedback-content">
        <span class="header-text">反馈消息：</span>
        <div class="content">
          <p>
            反馈人：{{feedbackDetail.backUserName}}
            <span class="time">反馈时间：{{feedbackDetail.backTime}}</span>
          </p>
          <p class="detail">{{feedbackDetail.backMsg}}</p>
        </div>
      </div>
      <Divider style="margin: 10px"></Divider>
      <div class="feedback-list-wrapper">
        <span class="header-text">回复内容</span>
        <div class="content">
          <Timeline v-show="!feedbackLoading" pending class="feedback-list">
            <TimelineItem v-for="(item, index) in replyList" :key="item.id">
              <template
                v-if="index != feedbackDetail.feedbackReplyOutPuts.length && feedbackDetail.feedbackReplyOutPuts.length >= 1"
              >
                <div class="detail-content">
                  <p class="time">
                    <span>回复时间：{{item.replyTime}}</span>
                    <span>回复人：{{item.userName}}</span>
                  </p>
                  <p class="detail">{{item.replyMsg}}</p>
                </div>
              </template>
              <template v-else>
                <p v-if="item.isClose" class="closed">已解决</p>
                <p v-else class="noClosed">未解决</p>
              </template>
            </TimelineItem>
            <TimelineItem v-if="showOne && feedbackDetail.feedbackReplyOutPuts.length > 1">
              <a href="javascript:;" @click="showOne=false">查看更多</a>
            </TimelineItem>
          </Timeline>
          <Spin fix v-show="feedbackLoading"></Spin>
          <Input
            v-model="replyValue"
            v-show="!feedbackDetail.isClose"
            type="textarea"
            :rows="4"
            placeholder="请输入回复内容"
          />
        </div>
      </div>
      <div class="feedback"></div>
      <div slot="footer" style="text-align:left;padding-left: 52px;">
        <Button type="primary" :loading="btnLoading" v-show="!feedbackDetail.isClose" @click="reply">提交回复</Button>
        <Button type="default" @click="detailModalCancel">取消</Button>
      </div>
    </Modal>

    <!-- 修改回复状态 -->
    <Modal title="修改状态" width="400" v-model="changeModal" class-name="vertical-center-modal">
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;"/>修改反馈状态后不可恢复，您确定修改吗？
        </p>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="changeReplyState">确定</Button>
        <Button class="cancelDelete" type="default" @click="changeModal=false">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import { getDate } from '@/libs/tools'
export default {
  data () {
    return {
      tabList: [
        {
          id: 0,
          txt: '全部'
        },
        {
          id: 2,
          txt: '未解决反馈'
        },
        {
          id: 1,
          txt: '已解决反馈'
        }
      ],
      tabActivedId: 0,

      tableData: [],
      tableLoading: false,
      columns: [
        {
          title: '反馈人',
          width: 120,
          key: 'backUserName'
        },
        {
          title: '反馈内容',
          key: 'backMsg'
        },
        {
          title: '审核状态',
          width: 120,
          align: 'center',
          render: (h, params) => {
            let isClose = params.row.isClose
            if (isClose) {
              return <span style={{ color: '#4cba3f' }}>已解决</span>
            } else {
              return <span style={{ color: '#fc4b4b' }}>未解决</span>
            }
          }
        },
        {
          title: '通知时间',
          width: 150,
          key: 'backTime',
          render: (h, params) => {
            let timeStamp = new Date(params.row.backTime).getTime() / 1000
            return <span>{getDate(timeStamp, 'year')}</span>
          }
        },
        {
          title: '操作',
          width: 150,
          render: (h, params) => {
            return (
              <div>
                <span
                  class="color-primary"
                  style={{ cursor: 'pointer' }}
                  onClick={() => this.showDetail(params.row)}
                >
                  查看详情
                </span>
                <span
                  v-show={!params.row.isClose}
                  style={{
                    cursor: 'pointer',
                    marginLeft: '10px',
                    color: '#f17aa0'
                  }}
                  onClick={() => this.showChangeReplyModal(params.row)}
                >
                  修改状态
                </span>
              </div>
            )
          }
        }
      ],
      dataCount: 0,
      startPage: 1,
      pageSize: 10,

      detailModal: false,
      feedbackDetail: {
        backMsg: '',
        backTime: '',
        backUserName: '',
        isClose: false,
        feedbackReplyOutPuts: []
      },
      replyValue: '',
      showOne: true,
      replyId: '',
      changeModal: false,
      btnLoading: false,
      feedbackLoading: false
    }
  },
  computed: {
    replyList () {
      if (this.showOne && this.feedbackDetail.feedbackReplyOutPuts.length > 1) {
        return [this.feedbackDetail.feedbackReplyOutPuts[0]]
      } else {
        let isFinished = {
          isClose: this.feedbackDetail.isClose
        }
        return [...this.feedbackDetail.feedbackReplyOutPuts, isFinished]
      }
    }
  },
  mounted () {
    this.changeTab(this.tabActivedId)
  },
  methods: {
    changeTab (id) {
      this.tabActivedId = id
      this.getList(this.startPage)
    },
    changePage (page) {
      this.getList(page)
    },
    getList (page) {
      let params = {
        id: '',
        beginTime: '',
        endTime: '',
        pageSize: this.pageSize,
        type: this.tabActivedId,
        pageNum: page
      }
      this.tableLoading = true
      this.swsApi
        .swsPost('Information/Feedback/List', params)
        .then(res => {
          let data = res.data
          if (data.success) {
            this.tableData = data.result
            this.dataCount = data.dataCount
          }
          this.tableLoading = false
        })
        .catch(e => {
          this.tableLoading = false
        })
    },
    showDetail ({ id }) {
      this.detailModal = true
      this.replyId = id
      this.getFeedBackDetail(id)
    },
    getFeedBackDetail (id) {
      this.feedbackLoading = true
      this.swsApi
        .swsPost(`Information/GetBackDetailAsync/List/${id}`)
        .then(res => {
          let data = res.data
          this.feedbackLoading = false
          if (data.success) {
            this.feedbackDetail = data.result
          }
        })
        .catch(e => {})
    },
    reply () {
      if (!this.replyValue.trim()) {
        this.$Message.error('请输入回复')
        return
      }
      let params = {
        feedbackId: this.replyId,
        replyMsg: this.replyValue
      }
      this.btnLoading = true
      this.swsApi.swsPost('Information/FeedbackReply/Add', params)
        .then(res => {
          let data = res.data
          if (data.success) {
            this.$Message.success('回复成功')
            this.detailModal = false
          }
          this.btnLoading = false
        })
        .catch(e => { this.btnLoading = false })
    },
    showChangeReplyModal ({ id }) {
      this.replyId = id
      this.changeModal = true
    },
    changeReplyState () {
      this.swsApi
        .swsPost(`Information/BackUpdaeState/${this.replyId}`)
        .then(res => {
          let data = res.data
          if (data.success) {
            this.changeModal = false
            this.$Message.success('修改状态成功')
            this.getList(this.startPage)
          }
        })
        .catch(e => {})
    },
    detailModalCancel () {
      this.detailModal = false
      // 初始化
      setTimeout(() => {
        this.showOne = true
        for (const key in this.feedbackDetail) {
          if (this.feedbackDetail.hasOwnProperty(key)) {
            const element = Array.isArray(this.feedbackDetail[key]) ? [] : ''
            this.feedbackDetail[key] = element
          }
        }
      }, 300)
    }
  },
  components: {}
}
</script>

<style scoped lang="less">
@import '../style.less';
.feedback-content,
.feedback-list-wrapper {
  .header-text {
    font-size: 14px;
    color: #333333;
  }
  .content {
    position: relative;
    margin-left: 54px;
    font-size: 13px;
    color: #999999;
    span {
      margin-left: 16px;
    }
    & > * {
      margin-top: 6px;
    }
    .detail {
      font-size: 14px;
      color: #666;
    }
  }
}
.feedback-list {
  p {
    & > span:first-child {
      margin-left: 0;
    }
    &.detail {
      margin-top: 6px;
      font-size: 13px;
      color: #666666;
    }
  }
  .closed {
    color: #4cba3f;
  }
  .noClosed {
    color: #fc4b4b;
  }
}
</style>
