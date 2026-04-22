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
          <RadioGroup v-model="article.msgTypeId" @on-change="messageTypeChange">
            <Radio v-for="item in messageTypeList" :key="item.id" :label="item.id">{{item.txt}}</Radio>
          </RadioGroup>
        </div>
        <div class="content_box">
          <div class="priceChange_box" v-show="article.msgTypeId !== 1">
            <div class="add_btn_group">
              <Button type="primary" @click="loadGoods">新增调价物品</Button>
              <Button type="info" @click="creatText">生成文本</Button>
              <!-- <Button type="info" @click="getText">获取文本</Button> -->
            </div>
            <Table
              :columns="table_column_price"
              :data="table_data_price"
              :height="540"
              no-data-text="请添加调价物品数据">
              <template slot-scope="{ row, index }" slot="action">
                <div>
                  <Tooltip content="修改" placement="top" transfer>
                    <Icon
                      type="md-create"
                      size="22"
                      color="#4f95e8"
                      @click="handleEdit(row, index)"
                      style="cursor: pointer;font-size: 18px;"
                    ></Icon>
                  </Tooltip>
                  <Tooltip content="删除" placement="top" transfer>
                    <Icon
                      type="md-close"
                      size="19"
                      color="red"
                      @click="delBtn(row, index)"
                      style="cursor: pointer"
                    ></Icon>
                  </Tooltip>
                </div>
              </template>
            </Table>
            <div class="dispose_time">
              <span>生效日期:</span>
              <DatePicker v-model="disposeTime" type="date" placeholder="请选择生效日期"></DatePicker>
            </div>
          </div>
          <div class="message_box">
            <div class="message_title">
              <input
                type="text"
                ref="msgTitle"
                @focus="hasError = false"
                :class="{'error': hasError}"
                v-model="article.msgTitle"
                placeholder="请输入标题(最多50个字)">
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
          </div>
        </div>
        <div class="publish_button">
          <Button type="primary" @click="publish">发&nbsp;&nbsp;&nbsp;&nbsp;布</Button>
        </div>
      </div>
    </main>
    <Modal v-model="selectModal" class="people_modal" width="660" :closable="false" @on-cancel="searchKey=''">
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

    <!-- 新增物品 -->
    <Modal v-model="goodsFlag" class="goods_data" :mask-closable="false" width="1000">
      <p slot="header" class="text-center">{{title}}物品</p>
      <Form
        ref="formValidateRef"
        :model="formValidate"
        :rules="ruleValidate"
        :label-width="90"
        style="margin-right: 30px;">
        <FormItem label="物品" prop="itemName">
          <Input v-model="formValidate.itemName" placeholder="请选择" readonly="readonly"/>
        </FormItem>
        <div
          style="width: 83%;height: 32px;position: absolute;top: 67px;left: 120px;"
          @click="loadGoods"
        ></div>
        <Row>
          <Col span="8">
            <FormItem label="成本价" prop="purchasingPrice">
              <InputNumber :min="0" v-model="formValidate.purchasingPrice" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="8">
            <FormItem label="销售价" prop="salePrice">
              <InputNumber :min="0" v-model="formValidate.salePrice" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="8">
            <FormItem label="调整售价" prop="upSalePrice">
              <InputNumber :min="0" v-model="formValidate.upSalePrice" style="width:100%"></InputNumber>
            </FormItem>
          </Col>
          <Col span="16">
            <FormItem label="备注" prop="remark">
              <Input
                v-model="formValidate.remark"
                type="textarea"
                :rows="3"
                placeholder="请输入备注"
              />
            </FormItem>
          </Col>
        </Row>
      </Form>
      <div slot="footer">
        <Button type="primary" @click="saveAddGoods('formValidateRef')">新增</Button>
        <Button type="default" @click="goodsFlag=false;handleReset('formValidateRef')">取消</Button>
      </div>
    </Modal>

    <!-- 加载所有物品 -->
    <Modal v-model="selectGoodsModal" width="800">
      <p slot="header" style="text-align: center;">选择项目</p>
      <Row>
        <Col span="24">
          <Input placeholder="检索..." v-model="modalSearch" ref="searchInput"/>
          <Spin size="large" fix v-if="listShow || !goodsFilter"></Spin>
          <ul class="liList">
            <li v-for="item in goodsFilter" :key="item.Id" @click="selectOneInfo(item)">
              <div>{{item.itemName}}</div>
            </li>
            <!-- <template v-for="item in goodsFilter">
              <li v-if="!item.isDisabled" :key="item.Id" @click="selectOneInfo(item)"><div>{{item.itemName}}</div></li>
              <li v-else :key="item.Id" class="disabledClick"><div>{{item.itemName}}</div></li>
            </template> -->
          </ul>
        </Col>
      </Row>
      <div slot="footer">
        <span style="float: left">
          <span>共{{goodsFilter.length}}项 </span>
          <span> 物品数据</span>
        </span>
        <Button @click="selectGoodsModal=false;modalSearch=''">关闭</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
// this.$refs.editor.getHtml() 获取编辑器内容
import editor from '_c/editor'
import { toFilterKey, formateDateToString } from '@/libs/tools.js'
const BUTTONROLE = {
  FABU_ADDP: 'FABU_ADDP'
}
export default {
  data () {
    return {
      table_column_price: [
        {title: '序号', type: 'index', align: 'center', width: 45},
        {title: '物品名称', key: 'itemName', minWidth: 75, tooltip: true},
        {title: '成本价', key: 'purchasingPrice', width: 65},
        {title: '销售价', key: 'salePrice', width: 65},
        {title: '调整售价', key: 'upSalePrice', width: 65},
        {title: '备注', key: 'remark', width: 65, tooltip: true},
        {
          title: '操作',
          slot: 'action',
          align: 'center',
          fixed: 'right',
          width: 65
        }
      ],
      table_data_price: [],
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
        },
        {
          id: 2,
          txt: '调价公告'
        }
      ],
      // 文章信息
      article: {
        msgTypeId: 1,
        msgTitle: '',
        msgContent: ''
      },
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
      hasError: false,

      buttonRole: BUTTONROLE,
      selectModal: false,
      selectGoodsModal: false,
      goodsList: [],
      listShow: false,
      modalSearch: '', // cy 物品查询词
      goodsFlag: false, // cy  新增调价物品
      formValidate: {
        id: '',
        itemName: '',
        medicalId: '',
        purchasingPrice: 0,
        salePrice: 0,
        upSalePrice: 0,
        remark: ''
      },
      ruleValidate: {
        purchasingPrice: [
          {required: true, type: 'number', message: '成本价不能为空', trigger: 'blur'}
        ],
        salePrice: [
          {required: true, type: 'number', message: '销售价不能为空', trigger: 'blur'}
        ],
        upSalePrice: [
          {required: true, type: 'number', message: '调整售价不能为空', trigger: 'blur'}
        ]
      },
      disposeTime: '', // cy 生效时间
      title: '' // cy modal title
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
    },
    goodsFilter () {
      let data = []
      if (this.goodsList) {
        data = toFilterKey(
          this.goodsList,
          'medicalItemName,aliasName,manufacturer,packaging,mnemonic',
          this.modalSearch
        )
      } else {
        data = []
      }
      return data
    }
  },
  methods: {
    getText () {
      console.log(this.$refs.editor.getHtml())
    },
    creatText () {
      if (this.table_data_price.length === 0) {
        this.$Message.error('请先添加调整物品')
        return false
      }
      if (!this.disposeTime) {
        this.$Message.error('请选择失效时间！')
        return
      }
      let title = `<h1>调整公告</h1>`
      let txtLi = this.table_data_price.map((res, index) => {
        return `<li>${index + 1}.物品<span style="font-style: italic;">${res.itemName}</span>&nbsp; &nbsp;` +
          `<span style="font-weight: bold;">成本价为</span><span style="font-weight: bold;">：${res.purchasingPrice}元</span>，` +
          `<span style="font-weight: bold;">原销售价格</span><span style="font-weight: bold;">：${res.salePrice}元</span>，` +
          `<span style="font-weight: bold;">调整为：${res.upSalePrice} 元</span>。</li>`
      }).join('')
      let time = `<h3 style="text-align: right;">生效日期：${formateDateToString(this.disposeTime, 'yyyy-MM-dd')}</h3>`
      this.$refs.editor.setHtml(`${title}<ul>${txtLi}</ul>${time}`)
    },
    handleEdit (row, index) {
      this.title = '修改'
      this.selectOneInfo(row)
    },
    delBtn (row, index) {
      console.log(row, index)
      this.$Modal.confirm({
        title: '提示',
        content: '<p>确定要进行删除操作吗？</p>',
        onOk: () => this.table_data_price.splice(index, 1)
      })
    },
    saveAddGoods (name) {
      // this.goodsFlag = false
      let params = { ...this.formValidate }
      this.$refs[name].validate(valid => {
        if (valid) {
          this.goodsFlag = false
          this.$Message.success(`添加成功！`)
          this.table_data_price.push(params)
          this.handleReset(name)
        } else {
          this.$Message.error('请仔细填写表格')
        }
      })
    },
    handleReset (name) {
      this.$refs[name].resetFields()
    },
    // cy 选中一个物品
    selectOneInfo (item) {
      this.formValidate.medicalId = item.id
      this.formValidate.itemName = item.itemName

      this.modalSearch = ''
      this.goodsFlag = true
      this.selectGoodsModal = false
    },
    // cy 加载所有物品
    loadGoods () {
      this.title = '新增'
      let that = this
      this.selectGoodsModal = true
      let params = {
        pageSize: 9999,
        pageNum: 1
      }
      // 查询物品
      // cy 减少请求次数
      if (!this.goodsList.length) {
        this.listShow = true
        this.swsApi
          .swsPost('Data/MedicalItemRecord/list', params)
          .then(function (res) {
            if (res.data.success) {
              let arr = []
              // 排除诊疗项目
              let result = res.data.result.filter(res => {
                return res.medicalItemType !== 5
              })
              // cy 新增过滤采购单里已经存在的物品
              // let tableDataIds = that.table_data.map(res => res.medicalItemRecordOutPut.id)
              for (let i in result) {
                let item = {}
                // cy 新增过滤采购单里已经存在的物品
                // item.isDisabled = tableDataIds.includes(result[i].id)
                // 物品名 + 别名 + 产品规格 + 生产厂家 + 包装规格 +助记码
                item.itemName = `${result[i].medicalItemName} ${result[i].brand
                  ? '(' + result[i].brand + ')' : ''} ${result[i].procurementPackage
                  ? '(' + result[i].procurementPackage + ')' : ''} ${result[i].aliasName
                  ? '(' + result[i].aliasName + ')' : ''} ${result[i].manufacturer
                  ? '(' + result[i].manufacturer + ')' : ''} ${result[i].packaging
                  ? result[i].packaging : ''} [${result[i].mnemonic}]`
                item.id = result[i].id
                // cy 用于模糊搜素
                item.medicalItemName = result[i].medicalItemName // 物品名称
                item.aliasName = result[i].aliasName // 别名
                item.manufacturer = result[i].manufacturer // 生产厂家
                item.packaging = result[i].packaging // 包装规格
                item.packageUnit = result[i].packageUnit // 包装单位ID
                item.mnemonic = result[i].mnemonic // 助记码
                item.medicalItemType = result[i].medicalItemType // 物品类型

                arr.push(item)
              }
              that.goodsList = arr
              that.listShow = false
            }
          })
      } else {
        that.listShow = false
      }
    },
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
        userIds,
        noticeMedicalInputs: this.table_data_price,
        outTime: this.disposeTime
      }
      // console.log(params)
      this.swsApi
        .swsPost(`Information/Information/Add`, params)
        .then(res => {
          let data = res.data
          if (data.success) {
            this.$Message.success('发布成功！')
            this.$refs.editor.setHtml(' ')
            this.article.msgTitle = ''
          } else {
            this.$Message.error('发布失败', data.error)
          }
        })
        .catch(e => {
          this.$Message.error('请求错误', e)
        })
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
    padding: 20px;
    .message_type {
      span {
        display: inline-block;
        margin-right: 10px;
        line-height: 2;
      }
    }
    .content_box {
      display: flex;
      justify-content: flex-start;
      .priceChange_box {
        width: 90%;
        margin: 22px 20px 0 0;
        .add_btn_group {
          margin-bottom: 20px;
          button + button {
            margin-left: 10px;
          }
        }
        .dispose_time {
          margin-top: 15px;
          font-size: 15px;
          span {
            margin-right: 15px;
          }
        }
      }
      .message_box {
        width: 100%;
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
      margin-top: 12px;
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
.people_modal {

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
}
.editor-wrapper {
  /deep/ .w-e-text-container {
    z-index: 1000 !important;
  }
  /deep/ .w-e-menu {
    z-index: 1001 !important;
  }
  /deep/ .w-e-toolbar {
    padding: 5px;
  }
}

.liList {
  max-height: 450px;
  overflow-y: scroll;
  li {
    height: 30px;
    line-height: 30px;
    font-size: 12px;
    border-bottom: 1px solid #eaeaea;
    cursor: pointer;
    white-space: nowrap;
    text-overflow: ellipsis;
    overflow: hidden;
    i {
      color: red;
    }
  }
  li:hover {
    background: #E8EAEC;
  }
  .disabledClick {
    cursor: not-allowed;
    color: #aaa;
  }
}
.goods_data {
  /deep/ .ivu-input-prefix i {
    font-size: 12px;
  }
}
</style>
