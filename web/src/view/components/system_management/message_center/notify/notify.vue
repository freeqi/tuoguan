<template>
  <div :class="['message_container', {'overflowHidden': isOverflowHidden}]">
    <header class="message_container_header header">
      <span class="left">全部公告</span>
      <div class="left searchBox">
        <span>数据状态</span>
        <Select v-model="searchOption.dataState" style="width: 100px;" @on-change="getList(1)">
          <Option v-for="item in dataStateList" :key='item.value' :value='item.value'>{{item.label}}</Option>
        </Select>
        <span>审核状态</span>
        <Select v-model="searchOption.reviewPush" style="width: 100px;" @on-change="getList(1)">
          <Option v-for="item in reviewPushList" :key='item.value' :value='item.value'>{{item.label}}</Option>
        </Select>
      </div>
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
      <div class="article-header">
        <div class="header_left">
          <div class="badge">公告</div>
          <div class="article-info">
            <h3>{{atricleDetail.msgTitle}}</h3>
            <span class="time">{{atricleDetail.sendTime}}</span>
          </div>
        </div>
        <div class="header_right">
          <Button type="default" icon="md-undo" @click="handleBack">返回</Button>
          <!-- <Button type="info" icon="md-checkbox" @click="approvalModal = true" v-permission="buttonRole.SHENPI">审批</Button> -->
          <Button type="info" icon="md-checkbox" @click="approvalModal = true" v-permission="buttonRole.SHENPI" v-if="atricleDetail.reviewPush===0">审批</Button>
          <Button type="success" icon="md-send" @click="pushBtn" v-permission="buttonRole.PUSH" v-if="atricleDetail.reviewPush === 1 && atricleDetail.dataState === 1">推送</Button>
          <Button type="error" icon="md-close" v-show="atricleDetail.dataState !== 2" @click="handleClose(atricleDetail.id)">关闭</Button>
        </div>
      </div>
      <div class="article-content" v-html="atricleDetail.msgContent"></div>
      <Divider></Divider>
      <div style="position:relative;" v-if="atricleDetail.reviewPush === 1 && atricleDetail.dataState !== 2">
        <Button type="success" class="pushBox" 
          v-show="atricleDetail.usersMsgs.some(res=>{ return !res.isSend})"
          v-permission="buttonRole.PUSH"
          @click="pushBtn(true)">一键推送</Button>
      </div>
      <div class="feedback">
        <template v-for="item in atricleDetail.usersMsgs">
          <div class="feedback_box" :key="item.id">
            <div class="feedback_info">
                <p class="text">{{item.feedbackContent}}</p>
              <div style="width: 50%;">
                <span>{{item.centerName}}</span>
                <span>{{item.backUserName}}</span>
                <p>{{item.backTime && formateDate(item.backTime, 'yyyy-MM-dd hh:mm:ss')}}</p>
              </div>
            </div>
            <div class="isSendBox" v-if="atricleDetail.dataState===3">
              <Icon type="md-checkmark-circle" size="20" color="green" v-if="item.isSend"/>
              <Icon type="md-close-circle" size="20" color="red" v-else/>
            </div>
          </div>
        </template>
      </div>
    </div>
    <Modal v-model="approvalModal" className="vertical-center-modal" width="600">
      <p slot="header" align="center">审批公告</p>
        <Form :label-width="60" style="margin: 5px;">
          <div class="checkTable">
            <Table class="checkTable" max-height="200" :data="approvalInfo.notice" :columns="checkColumn">
              <!-- <template slot-scope="{ row }" slot="salePrice">
                <span :class="{'high_light': (row.salePrice< row.purchasingPrice || row.salePrice > row.socialSecurityPrice)}"
                >{{ row.salePrice }}</span>
              </template> -->
              <template slot-scope="{ row }" slot="salePrice">
                <!-- <span class="high_light"
                >{{ row.salePrice }}</span> -->
                <span :style="`${(row.salePrice < row.purchasingPrice || row.salePrice > row.socialSecurityPrice) ? 'color:red;' : ''}`">{{ row.salePrice }}</span>
              </template>
            </Table>
          </div>
          <FormItem label="审核状态" style="margin: 10px 0;">
            <Select v-model="approvalInfo.approvalState" style="width: 210px" placeholder="请选择">
              <Option :value="1">同意</Option>
              <Option :value="2">拒绝</Option>
            </Select>
          </FormItem>
        </Form>
        <div slot="footer" class="right">
          <Button type="primary" :loading="saveLoading" @click="saveApproval">审批</Button>
          <Button type="default" @click="approvalModal = false">取消</Button>
        </div>
    </Modal>
  </div>
</template>

<script>
import { getDate, formateDateToString } from '@/libs/tools'
const BUTTONROLE = {
  SHENPI: 'SHENPI_XIANSHI',
  PUSH: 'PUSH_XIAOXI'
}
export default {
  data () {
    return {
      dataStateList: [
        {label: '全部', value: 0},
        {label: '有效', value: 1},
        {label: '已关闭', value: 2},
        {label: '已推送', value: 3},
      ],
      reviewPushList: [
        {label: '全部', value: 3},
        {label: '未审核', value: 0},
        {label: '已同意', value: 1},
        {label: '已拒绝', value: 2},
      ],
      searchOption: {
        dataState: 0,
        reviewPush: 3
      },
      buttonRole: BUTTONROLE,
      saveLoading: false,
      approvalModal: false,
      approvalInfo: {
        approvalState: 0,
        notice: []
      }, //审批
      checkColumn: [
        {title: '物品名', key: 'medicalName',  align: 'left'},
        {title: '成本价', key: 'purchasingPrice', width: 90, align: 'center'},
        {title: '原售价', key: 'upSalePrice', width: 90, align: 'center'},
        {title: '医保限价', key: 'socialSecurityPrice', width: 90, align: 'center'},
        {title: '现售价', key: 'salePrice', width: 90, slot: 'salePrice', align: 'center'},
      ],
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
          title: '审批状态',
          key: 'reviewPush',
          align: 'center',
          width: 80,
          render: (h, params) => {
            return params.row.reviewPush === 0
            ? <span style="color:#FAA74F;">未审批</span>
            : params.row.reviewPush === 1
              ? <span style="color:green;">已同意</span> 
              : <span style="color:red;">已拒绝</span>
          }
        },
        {
          title: '公告状态',
          key: 'dataState',
          align: 'center',
          width: 80,
          render: (h, params) => {
            return params.row.dataState === 1 
              ? <span style="color:green;">有效</span> 
              : params.row.dataState === 2
                ? <span style="color:red;">关闭</span>
                : params.row.usersMsgs.some(res=> !res.isSend)
                  ? <span style="color:#FAA74F;">部分推送</span>
                  : <span style="color:blue;">已推送</span>
          }
        },
        {
          title: '操作',
          align: 'center',
          width: 120,
          render: (h, params) => {
            return (
              <span class="color-primary" style={{'cursor': 'pointer'}} onClick={() => this.showDetail(params.row)}>查看详情</span>
            )
          }
        }
      ],
      dataCount: 0,
      startPage: 1,
      pageSize: 10,
      isOverflowHidden: false,
      newsTypeCheckedId: 0,
      newsTypeList: [{ id: 0, name: '全部消息' }, { id: 1, name: '我的发布' }],
      atricleDetail: {
        msgContent: '',
        msgTitle: '',
        sendTime: '',
        adjunct: '',
        id: '',
        usersMsgs: [],
        dataState: '',
        reviewPush: 0,
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
    pushBtn (flag=false) {
      this.$Modal.confirm({
        title: '提示',
        content: `<p>公告将${flag?' <strong>发布失败</strong> 的机构':''}进行推送发布，确定要进行此操作吗？</p>`,
        loading: true,
        onOk: () => {this.handlePush()}
      })
    },
    handlePush () {
      // this.saveLoading = true
      this.swsApi.swsGet(`/Information/Information/MesgSend/${this.atricleDetail.id}`)
       .then((res)=>{
         if (res.data.success) {
            this.$Message.success('推送操作成功！')
            this.approvalModal = false
            this.handleBack()
            this.getList(this.startPage)
          } else {
            this.$Message.warning({
              content: `操作失败，${res.data.error}`,
              duration: 3
            })
          }
          this.$Modal.remove()
       })
      .catch(e => {
        this.$Modal.remove()
        this.$Message.error({
          content: '请求失败，请稍后再试！' + e,
          duration: 3
        })
      })
    },
    saveApproval () {
      this.saveLoading = true
      this.swsApi.swsGet(`/Information/Information/MesgAudit/${this.atricleDetail.id}/${this.approvalInfo.approvalState}`)
       .then((res)=>{
         if (res.data.success) {
            this.$Message.success('审核操作成功！')
            this.approvalModal  = false
            this.handleBack()
            this.getList(this.startPage)
          } else {
            this.$Message.warning({
              content: `操作失败，${res.data.error}`,
              duration: 3
            })
          }
          this.saveLoading = false
       })
      .catch(e => {
        this.saveLoading = false
        this.$Message.error({
          content: '请求失败，请稍后再试！' + e,
          duration: 3
        })
      })
    },
    formateDate (da, style) {
      return formateDateToString(new Date(da), style)
    },
    changePage (page) {
      this.startPage = page
      this.getList(page)
    },
    showDetail (detail) {
      this.atricleDetail = detail
      this.approvalInfo.notice = detail.noticeMedicalOutPuts
      this.isOverflowHidden = true
    },
    // 已读置灰
    rowClassName (row, index) {
      // console.log(row)
    },
    handleBack () {
      this.isOverflowHidden = false
    },
    handleClose (id) {
      this.$Modal.confirm({
        title: '关闭',
        content: '确定要关闭该条公告吗？',
        onOk: () => {
          this.swsApi.swsPost(`Information/Information/close/${id}`)
            .then(res => {
              if (res.data.success) {
                this.$Message.success('关闭成功！')
                this.handleBack()
                this.getList(this.startPage)
              } else {
                this.$Message.warning({
                  content: `关闭失败，${res.data.error}`,
                  duration: 3
                })
              }
            })
            .catch(e => {
              this.$Message.error({
                content: '请求失败，请稍后再试！' + e,
                duration: 3
              })
            })
        }
      })
    },
    // 获取公告列表
    getList (page) {
      let params = {
        ...this.searchOption,
        allOrSelf: this.newsTypeCheckedId,
        beginTime: '',
        endTime: '',
        pageSize: this.pageSize,
        type: 1,
        pageNum: page,
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
  .searchBox {
    width: 80%;
    margin: 0 10px;
    span {
      margin: 0 10px;
    }
  }
  .checkTable {
    .ivu-table-wrapper {
      & /deep/ .ivu-table .high_light {
        // background-color: #EBF7FF;
        color: red;
        font-size: 15px;
        font-weight: bold;
      }
    }
  }
  .message_detail {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    padding: 25px;
    background: #ffffff;
    overflow-y: auto;
    z-index: 999;
    .article-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding-bottom: 10px;
      // border-bottom: 1px solid #eaeaea;
      .header_left,.header_right {
        display: flex;
        button + button {
          margin-left: 10px;
        }
      }
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
    .pushBox {
      position: absolute;
      top: -15px;
      right: 30px;
    }
    .feedback {
      // height: 330px;
      height: 65%;
      overflow-y: scroll;
      margin: 50px 15px 0;
      position: relative;
      .feedback_box {    
        display: flex;
        justify-content: flex-end;
        align-items: center;
        padding: 10px 15px;
        margin-bottom: 10px;
        background: #f6f6f6;
        border-radius: 8px;
        font-size: 13px;
        .feedback_info {
          width: 100%;
          font-size: 12px;
          text-align: right;
          display: flex;
          justify-content: flex-end;
          .text {
            text-align: left;
            text-indent: 2em;
          }
          span + span {
            padding-left: 5px;
          }
        }
        .isSendBox {
          margin-left: 2%;
        }
      }
    }
  }
}
</style>
