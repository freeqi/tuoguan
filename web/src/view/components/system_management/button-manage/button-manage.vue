<template>
  <div class="container">
    <Spin fix v-if="spinShow">
      <Icon type="ios-loading" class="demo-spin-icon-load" size="30"></Icon>
      <div class="text-loading">loading</div>
    </Spin>
    <Row class="box">
      <i-col class="box-left" :sm="6" :md="6" :lg="5">
        <div class="top">
          <div class="top_left">
            <span style="color: #999999; font-size: 15px; margin-right: 22px;">菜单目录</span>
          </div>
        </div>
        <div class="menuTree">
          <Tree :data="treeData" @on-select-change="handleClickTreeNode" style="margin-left: 12px;"></Tree>
        </div>
      </i-col>
      <i-col class="box-right" :sm="18" :md="18" :lg="19">
        <div class="top">
          <div class="top_left">
            <span style="color: #999999; font-size: 15px; margin-right: 22px;">{{title}}</span>
          </div>
        </div>
        <div class="content">
          <Button type="primary" @click="addModal=true" v-permission="buttonRole.ANGL_XZAN">新增按钮</Button>
          <div>
            <Row
              style="margin-top: 10px;background-color:#f9f9f9;height:40px;line-height:40px;font-size:15px;color:#363636;"
            >
              <i-col :sm="6" :md="6" :lg="6">按钮名称</i-col>
              <i-col :sm="6" :md="6" :lg="6">按钮状态</i-col>
              <i-col :sm="6" :md="6" :lg="6">排序</i-col>
              <i-col :sm="6" :md="6" :lg="6" v-show="hasRole">操作</i-col>
            </Row>
            <Row
              style="margin-top:17px;margin-left:17px;"
              v-for="item in buttonList"
              :key="item.id"
            >
              <i-col :sm="6" :md="6" :lg="6">
                <Button type="info">{{item.buttonName}}</Button>
              </i-col>
              <i-col :sm="6" :md="6" :lg="6">
                <Tag color="success" v-if="item.dataState == 1">启用</Tag>
                <Tag color="error" v-else>禁用</Tag>
              </i-col>
              <i-col :sm="6" :md="6" :lg="6">{{item.sortNo}}</i-col>
              <i-col :sm="6" :md="6" :lg="6" style="font-size:14px; cursor: pointer" v-show="hasRole">
                <span
                  v-permission="buttonRole.ANGL_XG"
                  style="margin-right:20px;color:#6badfd"
                  @click="handleModify(item.id)"
                >修改</span>
                <span
                  v-permission="buttonRole.ANGL_SC"
                  style="color:#fc6969;"
                  @click="handleDelete(item.id)"
                >删除</span>
              </i-col>
            </Row>
          </div>
        </div>
      </i-col>
    </Row>
    <!-- 新增模态框 -->
    <Modal v-model="addModal" width="360" :mask-closable="false">
      <p slot="header" style="text-align:center">
        <span>新增按钮</span>
      </p>
      <div style>
        <Form inline :label-width="80" :model="addData">
          <FormItem label="按钮名称">
            <Input v-model="addData.buttonName" placeholder="请输入"></Input>
          </FormItem>
          <FormItem label="CODE">
            <Input v-model="addData.buttonCode" placeholder="请输入"></Input>
          </FormItem>
          <FormItem label="排序">
            <InputNumber :max="999" :min="0" v-model="addData.sortNo"></InputNumber>
          </FormItem>
          <FormItem label="是否可用">
            <RadioGroup v-model="addData.dataState">
              <Radio v-for="item in state" :key="item.value" :label="item.value">{{item.label}}</Radio>
            </RadioGroup>
          </FormItem>
        </Form>
      </div>
      <div slot="footer">
        <Button type="primary" @click="handleAddButton">确定</Button>
        <Button type="default" @click="addModal=false">取消</Button>
      </div>
    </Modal>
    <!-- 修改模态框 -->
    <Modal v-model="modifyModal" width="360" :mask-closable="false">
      <p slot="header" style="text-align:center">
        <span>修改按钮</span>
      </p>
      <div style>
        <Form inline :label-width="80" :model="modifyData">
          <FormItem label="按钮名称">
            <Input v-model="modifyData.buttonName" placeholder="请输入"></Input>
          </FormItem>
          <FormItem label="CODE">
            <Input v-model="modifyData.buttonCode" placeholder="请输入"></Input>
          </FormItem>
          <FormItem label="排序">
            <InputNumber :max="999" :min="0" v-model="modifyData.sortNo"></InputNumber>
          </FormItem>
          <FormItem label="是否可用">
            <RadioGroup v-model="modifyData.dataState">
              <Radio v-for="item in state" :key="item.value" :label="item.value">{{item.label}}</Radio>
            </RadioGroup>
          </FormItem>
        </Form>
      </div>
      <div slot="footer">
        <Button type="primary" @click="handleConfirmModify">保存</Button>
        <Button type="default" @click="modifyModal=false">取消</Button>
      </div>
    </Modal>
    <!-- 删除模态框 -->
    <Modal
      title="删除按钮"
      v-model="deleteModal"
      class-name="vertical-center-modal"
      :mask-closable="false"
      width="400"
    >
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;"/>删除后不可恢复，您确定删除吗？
        </p>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleDeleteButton">确定</Button>
        <Button class="cancelDelete" type="default" @click="deleteModal=false">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
const BUTTONROLE = {
  ANGL_XZAN: 'ANGL_XZAN',
  ANGL_SC: 'ANGL_SC',
  ANGL_XG: 'ANGL_XG'
}
export default {
  name: 'buttonManage',
  data () {
    return {
      spinShow: true,
      addModal: false,
      modifyModal: false,
      deleteModal: false,
      menuId: '',
      currentMenuId: '-1',
      treeData: [],
      addData: {
        buttonName: '',
        buttonCode: '',
        sortNo: -1,
        dataState: ''
      },
      modifyData: {
        buttonName: '',
        buttonCode: '',
        sortNo: -1,
        dataState: ''
      },
      state: [
        {
          value: 1,
          label: '启用'
        },
        {
          value: 2,
          label: '禁用'
        }
      ],
      id: 1,
      title: '按钮管理',
      buttonList: [],
      buttonRole: BUTTONROLE,
      hasRole: true
    }
  },
  mounted () {
    this.handleGetMenuList() // 获取菜单列表
    this.handleGetButton() // 获取按钮列表

    this.hasRole = this._filterButton(this.buttonRole.ANGL_SC) || this._filterButton(this.buttonRole.ANGL_XG)
  },
  methods: {
    // 点击菜单
    handleClickTreeNode (e) {
      if (!e.length) return
      console.log(e)
      this.id = e[0].id
      this.currentMenuId = e[0].id
      this.title = e[0].title
      this.handleGetButton()
    },
    // 获取菜单列表
    handleGetMenuList () {
      this.swsApi.swsPost('Menu/tree').then(res => {
        // console.log(res)
        if (res.data.error === null) {
          this.spinShow = false
          this.treeData = res.data.result
        }
      })
    },
    // 获取按钮
    handleGetButton () {
      this.swsApi
        .swsPost('Menu/ButtonArry', { menuId: this.currentMenuId })
        .then(res => {
          // console.log(res)
          if (res.data.error === null) {
            this.buttonList = res.data.result
            // console.log(this.buttonList)
            if (this.buttonList.length !== 0) {
              this.menuId = this.buttonList[0].menuId
            }
          }
        })
    },
    // 点击修改
    handleModify (id) {
      this.id = id
      this.modifyModal = true
      this.modifyData = this.buttonList.filter(v => {
        return id === v.id
      })[0]
      this.menuId = this.modifyData.menuId
    },
    // 确认修改
    handleConfirmModify () {
      this.modifyModal = false
      let modifyParams = {
        id: this.id,
        buttonName: this.modifyData.buttonName,
        buttonCode: this.modifyData.buttonCode,
        menuId: this.menuId,
        sortNo: this.modifyData.sortNo,
        dataState: this.modifyData.dataState
      }
      this.swsApi
        .swsPost('Menu/CreateUpdateMenuButton', modifyParams)
        .then(res => {
          if (res.data.error === null) {
            this.$Message.success('修改按钮成功')
            this.handleGetButton()
          }
        })
    },
    // 确认增加按钮
    handleAddButton () {
      this.addModal = false
      let addParams = {
        buttonName: this.addData.buttonName,
        buttonCode: this.addData.buttonCode,
        menuId: this.currentMenuId,
        sortNo: this.addData.sortNo,
        dataState: this.addData.dataState
      }
      this.swsApi
        .swsPost('Menu/CreateUpdateMenuButton', addParams)
        .then(res => {
          if (res.data.error === null) {
            this.$Message.success('增加按钮成功')
            this.handleGetButton()
          } else {
            this.$Message.error('添加出错，请稍后！')
          }
        })
    },
    // 点击删除
    handleDelete (id) {
      this.deleteModal = true
      this.id = id
    },
    // 确认删除按钮
    handleDeleteButton () {
      this.deleteModal = false
      this.swsApi.swsPost(`Menu/delbutton/${this.id}`).then(res => {
        if (res.data.error === null) {
          this.$Message.success('删除成功！')
          this.handleGetButton()
        } else {
          this.$Message.error('操作错误，请稍后！')
        }
      })
    }
  }
}
</script>

<style scoped lang='less'>
.container {
  background-color: #fff;
  height: 100%;
  position: relative;
  .box {
    height: 100%;
    & > * {
      height: 100%;
      display: flex;
      flex-direction: column;
      .top {
        width: 100%;
        height: 50px;
        line-height: 50px;
        border-bottom: 1px solid #eaeaea;
        .top_left {
          padding-left: 20px;
        }
        .top_right {
          color: #999999;
          font-size: 15px;
          padding-left: 20px;
        }
        & + * {
          padding-top: 10px;
          flex: 1;
          overflow-y: auto;
        }
      }
    }
    &-left {
      height: 100%;
      border-right: 1px solid #eaeaea;
      .menuTree {
        margin-left: 30px;
      }
    }
  }
}

/deep/ .menuTree .ivu-tree .ivu-tree-children li .ivu-tree-title {
  font-size: 15px;
  color: #424242;
}
.content {
  width: 95%;
  margin-left: 20px;
  // margin-top: px;
}
</style>
