<template>
  <div class="message_container publish_container overflowHidden">
    <header class="publish_container_header tab_header header">
      <span
        v-for="item in tabList"
        :key="item.id"
        :class="{'active': tabActivedId == item.id}"
        @click="changeTab(item.id)"
      >{{item.txt}}</span>
    </header>
    <main class="publish_container_content">
      <div v-show="tabActivedId == 1">
        <div class="message_type">
          <span>请选择类目：</span>
          <RadioGroup v-model="article.msgTypeId">
            <Radio v-for="item in messageTypeList" :key="item.id" :label="item.id">{{item.txt}}</Radio>
          </RadioGroup>
        </div>
        <div class="message_title">
          <input
            type="text"
            ref="msgTitle"
            @focus="hasError = false"
            :class="{'error': hasError}"
            v-model="article.msgTitle"
            placeholder="请输入标题(最多50个字)"
          >
          <span></span>
        </div>

        <div class="message_c">
          <editor ref="editor" :cache="false"></editor>
        </div>
        <div class="message_select_people">
          <span style="margin-top: 6px;">选择查看人：</span>
          <div class="select_list">
            <div class="add" @click="showSelectModal">
              <Icon size="22" type="ios-add"/>
            </div>
            <button v-for="item in selectedPeople" :key="item.id">
              {{item.title}}
              <Icon
                type="md-close"
                @click="deleteNode(item, true)"
                size="16"
                style="cursor: pointer"
              />
            </button>
          </div>
        </div>
        <div class="publish_button">
          <Button type="primary" @click="publish">发&nbsp;&nbsp;&nbsp;&nbsp;布</Button>
        </div>
      </div>
    </main>
    <Modal v-model="selectModal" width="660" :closable="false" @on-cancel="searchKey=''">
      <header slot="header" class="modal_header">选择人员</header>
      <div class="modal_content">
        <aside class="tree_wrapper">
          <Input prefix="ios-search" v-model="searchKey" placeholder="搜索透析中心/职务"/>
          <div class="text">选择：</div>
          <div class="people_select_wrapp">
            <div class="tab_select_header">
              <span
                v-for="item in selectHeaderList"
                :class="[selectHeaderCheckedId === item.id ? 'active': '']"
                :key="item.id"
                @click="selectPeople(item.id)"
              >{{item.txt}}</span>
            </div>
            <div class="tab_select_content">
              <div v-show="selectHeaderCheckedId === 1">
                <Tree
                  ref="tree1"
                  :data="filterTreeDataO"
                  show-checkbox
                  multiple
                  @on-check-change="getNode"
                ></Tree>
                <Spin v-if="treeLoading1" fix></Spin>
              </div>
              <div v-show="selectHeaderCheckedId === 2">
                <Tree
                  ref="tree2"
                  :data="filterTreeDataJ"
                  show-checkbox
                  multiple
                  @on-check-change="getNode"
                ></Tree>
                <Spin v-if="treeLoading2" fix></Spin>
              </div>
            </div>
          </div>
        </aside>
        <div class="selected_list">
          <div class="content">
            <div class="text">已选：</div>
            <ul class="checked_list">
              <li class v-for="item in nodes" :key="item.id">
                <div class="left">
                  <div class="icon" v-if="item.children.length && item.groupType === 1">
                    <Icon type="md-home" size="18" color="#fff"/>
                  </div>
                  <div class="icon people" v-else>
                    <Icon type="md-person" size="18" color="#fff"/>
                  </div>
                  {{item.title}}
                </div>
                <Icon color="#dadada" @click="deleteNode(item)" type="md-close-circle" size="20"/>
              </li>
            </ul>
          </div>
          <div class="footer">
            <Button type="primary" style="margin-right: 10px" @click="confirmPeople">确定</Button>
            <Button type="default" @click="selectModal=false;searchKey=''">取消</Button>
          </div>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script>
// this.$refs.editor.getHtml() 获取编辑器内容
import editor from '_c/editor'
export default {
  data () {
    return {
      tabList: [
        {
          id: 1,
          txt: '发布内容'
        }
        // {
        //   id: 2,
        //   txt: '我的发布'
        // }
      ],
      tabActivedId: 1,

      messageTypeList: [
        {
          id: 1,
          txt: '通知公告'
        }
      ],
      // 文章信息
      article: {
        msgTypeId: 1,
        msgTitle: '',
        msgContent: ''
      },
      selectModal: false,
      searchKey: '',

      selectHeaderList: [
        {
          txt: '组织架构',
          id: 1
        },
        {
          txt: '职务',
          id: 2
        }
      ],
      selectHeaderCheckedId: 1,
      // 按机构划分
      treeData1: [],
      // 按职务划分
      treeData2: [],
      nodes: [],
      tree1Nodes: [],
      tree2Nodes: [],

      selectedPeople: [],
      // 是否第一次加载人员信息
      firstLoad: true,
      treeLoading1: false,
      treeLoading2: false,
      // 标题为空时class样式
      hasError: false
    }
  },
  computed: {
    filterTreeDataO () {
      if (this.selectHeaderCheckedId === 1) {
        let key = this.searchKey.trim()
        return this.handleFilterTree(key, this.treeData1)
      } else {
        return this.treeData1
      }
    },
    filterTreeDataJ () {
      if (this.selectHeaderCheckedId === 2) {
        let key = this.searchKey.trim()
        return this.handleFilterTree(key, this.treeData2)
      } else {
        return this.treeData2
      }
    }
  },
  methods: {
    messageTypeChange (typeId) {
      console.log(typeId)
    },
    changeTab (id) {
      this.tabActivedId = id
    },
    selectPeople (id) {
      this.searchKey = ''
      // 无数据是才加载树形菜单
      ;(!this.treeData2.length || !this.treeData1.length) &&
        this.getTreeDate(id)
      this.selectHeaderCheckedId = id
    },
    // 选择节点时触发
    getNode (nodes, { groupType }) {
      // 尾遍历解决splice删除元素问题
      for (let index = nodes.length - 1; index >= 0; index--) {
        const element = nodes[index]
        element.children.length && nodes.splice(index + 1, element.children.length)
      }
      if (nodes.length) {
        this[`tree${groupType}Nodes`] = nodes
        // 同步源数据
        this.mapAndCompareDatas(nodes, this[`treeData${groupType}`])
      } else {
        this[`tree${groupType}Nodes`] = nodes
      }
      // tree1Node,tree2Node为中间变量，为了合并节点做准备
      this.nodes = [...this.tree1Nodes, ...this.tree2Nodes]
    },
    // 删除节点
    deleteNode ({ id, nodeKey, groupType }, outside) {
      // this.data4 = this.changeTree(JSON.parse(JSON.stringify(this.data4)), id)
      this.$refs[`tree${groupType}`].handleCheck({ checked: false, nodeKey })
      if (outside) {
        this.selectedPeople = JSON.parse(JSON.stringify(this.nodes))
      }
    },
    confirmPeople () {
      this.selectModal = false
      this.searchKey = ''
      this.selectedPeople = JSON.parse(JSON.stringify(this.nodes))
    },
    // 按机构/职务获取人员
    getTreeDate (id) {
      id === 1 ? (this.treeLoading1 = true) : (this.treeLoading2 = true)
      this.swsApi
        .swsGet(`Information/Information/Userlist/${id}`)
        .then(res => {
          let data = res.data
          if (data.success) {
            id === 1
              ? (this.treeData1 = data.result)
              : (this.treeData2 = data.result)
            id === 1 ? (this.treeLoading1 = false) : (this.treeLoading2 = false)
          }
        })
        .catch(e => {})
    },
    // 显示联系人弹窗
    showSelectModal () {
      this.firstLoad && this.getTreeDate(this.selectHeaderCheckedId)
      this.selectModal = true
      this.firstLoad = false
    },
    // 发布消息
    publish () {
      let { msgTitle, msgTypeId } = this.article
      let userIds = this.selectedPeople.map(item => {
        return { userId: item.id, groupType: item.userType }
      })
      if (!msgTitle) {
        this.$Message.error('请输入标题！')
        this.hasError = true
        return
      }
      if (!userIds.length) {
        this.$Message.error('请选择查看人员！')
        return
      }
      let params = {
        msgContent: this.$refs.editor.getHtml(),
        msgTitle,
        msgTypeId,
        adjunct: '',
        userIds
      }
      this.swsApi
        .swsPost(`Information/Information/Add`, params)
        .then(res => {
          let data = res.data
          if (data.success) {
            this.$Message.success('发布成功！')
            this.$refs.editor.setHtml(' ')
            this.article.msgTitle = ''
          }
        })
        .catch(e => {})
    },
    handleFilterTree (title, datas) {
      if (!title) return datas
      let targets = []
      for (let i = 0; i < datas.length; i++) {
        const element = JSON.parse(JSON.stringify(datas[i]))
        if (element.children && element.children.length) {
          if (element.title.indexOf(title) > -1 || element.checked) {
            targets.push(element)
          }
        }
      }
      return targets
    },
    // 同步源数据
    mapAndCompareDatas (compareDatas, originalDatas) {
      if (!originalDatas.length) return []
      for (let i = 0; i < compareDatas.length; i++) {
        const element = compareDatas[i]
        this.mapDatas(element, originalDatas)
      }
    },
    mapDatas (target, datas) {
      const {id} = target
      for (let i = 0; i < datas.length; i++) {
        let element = datas[i]
        // id匹配
        if (element.id === id) {
          datas[i] = target
          continue
        }
        // id不匹配时， 且element为父节点则继续匹配
        if (element.id !== id && element.children && element.children.length) {
          this.mapDatas(target, element.children)
        }
      }
    }
  },
  components: {
    editor
  }
}
</script>

<style scoped lang="less">
@import '../style.less';
.publish_container {
  &_content {
    padding: 30px 40px;
    .message_type {
      span {
        display: inline-block;
        margin-right: 10px;
        line-height: 2;
      }
    }
    .message_title {
      position: relative;
      margin-top: 20px;
      width: 100%;
      font-size: 20px;
      line-height: 2;
      input {
        width: 100%;
        font-size: inherit;
        font-family: inherit;
        background-color: transparent;
        border: 1px solid transparent;
        outline: none;
        &::placehoder {
          color: #999999;
        }
        &:focus {
          & ~ span {
            transform-origin: bottom left;
            transform: scaleX(1);
          }
        }
        &.error {
          & ~ span {
            background-color: #ed4014;
            transform-origin: bottom left;
            transform: scaleX(1);
          }
        }
      }
      span {
        position: absolute;
        bottom: 0;
        left: 0;
        right: 0;
        height: 1px;
        background-color: @blue;
        transform-origin: bottom right;
        transform: scaleX(0);
        transition: transform 0.5s ease, background 0.3s ease;
      }
    }
    .message_c {
      margin-top: 20px;
      /deep/ .w-e-text-container {
        height: 500px !important;
      }
    }
    .message_select_people {
      margin-top: 10px;
      display: flex;
      font-size: 15px;
      .add {
        margin: 0 10px 10px;
        display: inline-block;
        vertical-align: middle;
        background: #fff;
        width: 36px;
        height: 36px;
        /deep/ .ivu-icon {
          line-height: 1.5 !important;
        }
        border: 1px dashed #dcdee2;
        border-radius: 4px;
        text-align: center;
        cursor: pointer;
        position: relative;
        overflow: hidden;
        -webkit-transition: border-color 0.2s ease;
        transition: border-color 0.2s ease;
        &:hover {
          border-color: @blue;
          color: @blue;
        }
      }
      .select_list {
        flex: 1;
        button {
          margin-bottom: 10px;
          vertical-align: middle;
          padding: 0 10px;
          display: inline-block;
          height: 36px;
          line-height: 34px;
          font-size: 14px;
          color: #333333;
          border: solid 1px #eaeaea;
          background-color: #f6f6f6;
          & + button {
            margin-left: 10px;
          }
        }
      }
    }
    .publish_button {
      text-align: center;
      margin-top: 20px;
    }
  }
}
/deep/ .ivu-modal {
  .ivu-modal-footer {
    padding: 0;
    display: none;
  }
  .ivu-modal-header {
    padding: 0;
    overflow: hidden;
    border-radius: 6px;
  }
  .modal_header {
    text-align: center;
    font-size: 18px;
    color: #333333;
    height: 50px;
    line-height: 50px;
    background-color: #eeeff4;
  }
  .modal_content {
    display: flex;
    & > * {
      height: 475px;
      padding: 20px 20px 0;
      flex: 1;
      overflow-y: auto;
    }
    .tree_wrapper {
      border-right: 1px solid #eaeaea;
      .text {
        margin: 10px 0;
        display: block;
        font-size: 14px;
        color: #999999;
      }
      .tab_select_header {
        display: flex;
        height: 32px;
        align-items: center;
        cursor: pointer;
        span {
          flex: 1;
          font-size: 14px;
          height: 32px;
          line-height: 32px;
          text-align: center;
          color: #333;
          background: #eeeff4;
          &.active {
            background: @blue;
            color: #ffffff;
          }
        }
      }
      .tab_select_content {
        position: relative;
        // margin: 16px 0;
        height: 318px;
        overflow-y: auto;
        /deep/ .ivu-tree-empty {
          text-align: center;
          padding-top: 40px;
        }
      }
    }
    .selected_list {
      padding: 0;
      display: flex;
      flex-direction: column;
      div.content {
        padding: 20px 20px 0;
        flex: 1;
        overflow-y: auto;
        .text {
          font-size: 16px;
          color: #333333;
          margin-bottom: 10px;
        }
        .checked_list {
          li {
            margin-bottom: 10px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            font-size: 13px;
            height: 32px;
            & > div {
              line-height: 32px;
            }
            .icon {
              display: inline-block;
              vertical-align: middle;
              margin-right: 10px;
              width: 28px;
              height: 28px;
              line-height: 28px;
              text-align: center;
              border-radius: 28px;
              background: #00b4de;
              &.people {
                background: #c7cb6f;
              }
            }
            /deep/ .ivu-icon-md-close-circle {
              cursor: pointer;
            }
          }
        }
      }
      .footer {
        text-align: right;
        padding: 20px;
        background: #ffffff;
      }
    }
  }
  .ivu-modal-body {
    padding: 0;
  }
}
.editor-wrapper {
  /deep/ .w-e-text-container {
    z-index: 1000 !important;
  }
  /deep/ .w-e-menu {
    z-index: 1001 !important;
  }
}
</style>
