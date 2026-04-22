<template>
  <div id="menu-manage" class="container">
    <Spin fix v-if="spinShow">
      <Icon type="ios-loading" class="demo-spin-icon-load" size="30"></Icon>
      <div class="text-loading">loading</div>
    </Spin>
    <Row class="box">
      <i-col class="menu box-left" :sm="10" :md="8" :lg="5">
        <div class="top">
          <div class="top_left">
            <span style="color: #999999; font-size: 15px; margin-right: 22px;">菜单目录</span>
            <Button type="primary" @click="handleAdd" v-permission="buttonRole.CDGL_XZCD">新增菜单</Button>
          </div>
        </div>
        <div class="menuTree">
          <Tree :data="treeData" @on-select-change="handleClickTreeNode" style="margin-left: 12px;"></Tree>
        </div>
      </i-col>
      <i-col class="operate-box box-right" :sm="14" :md="16" :lg="19">
        <!-- 详情-修改-删除 -->
        <div v-show="detailShow">
          <div class="top">
            <div class="top_right">菜单管理</div>
          </div>
          <div class="edit-box">
            <Form
              ref="formValidate"
              class="form"
              :model="formValidate"
              :rules="ruleValidate"
              :label-width="90"
              label-position="right"
            >
              <FormItem label="菜单名称：" prop="title">
                <Input v-model="formValidate.title" placeholder="请输入菜单名称"></Input>
              </FormItem>
              <FormItem label="图标：" prop="iconUrl">
                <Input v-model="formValidate.iconUrl" placeholder="请输入菜单图标"></Input>
              </FormItem>
              <FormItem label="链接：" prop="menuUrl">
                <Input v-model="formValidate.menuUrl" placeholder="请输入菜单路由"></Input>
              </FormItem>
              <FormItem label="上级：" prop="parentMenuCode">
                <Select v-model="formValidate.parentMenuCode" filterable placeholder>
                  <Option :value="'0'">无</Option>
                  <Option v-for="item in parentList" :key="item.id" :value="item.id">{{item.title}}</Option>
                </Select>
              </FormItem>
              <FormItem label="排序：" prop="menuSortNo">
                <Input v-model="formValidate.menuSortNo" placeholder="请输入序号"></Input>
              </FormItem>
              <FormItem label="状态：" prop="dataState">
                <RadioGroup v-model="formValidate.dataState">
                  <Radio
                    v-for="item in menuState"
                    :key="item.value"
                    :label="item.value"
                  >{{item.label}}</Radio>
                </RadioGroup>
              </FormItem>
              <FormItem label="是否显示：" prop="hideInMenu">
                <RadioGroup v-model="formValidate.hideInMenu">
                  <Radio
                    v-for="item in inMenuState"
                    :key="item.value"
                    :label="item.value"
                  >{{item.label}}</Radio>
                </RadioGroup>
              </FormItem>
              <FormItem label="备注：" prop="remark">
                <Input
                  v-model="formValidate.remark"
                  type="textarea"
                  :rows="4"
                  placeholder="请输入备注信息"
                />
              </FormItem>
              <FormItem style="margin-top: 45px;">
                <Button
                  type="primary"
                  @click="handleModifyMenu"
                  v-permission="buttonRole.CDGL_BC"
                >保存</Button>
                <Button
                  style="margin-left: 20px;"
                  v-permission="buttonRole.CDGL_SC"
                  type="error"
                  @click="handleDeleteMenu"
                >删除</Button>
              </FormItem>
            </Form>
          </div>
        </div>
        <!-- 新增 -->
        <div v-show="!detailShow">
          <div class="top">
            <div class="top_right">新增菜单管理</div>
          </div>
          <div class="add-box">
            <Form
              ref="aformValidate"
              class="form"
              :model="aformValidate"
              :rules="aruleValidate"
              :label-width="90"
              label-position="right"
            >
              <FormItem label="菜单名称：" prop="title">
                <Input v-model="aformValidate.title" placeholder="请输入菜单名称"></Input>
              </FormItem>
              <FormItem label="图标：" prop="iconUrl">
                <Input v-model="aformValidate.iconUrl" placeholder="请输入菜单图标"></Input>
              </FormItem>
              <FormItem label="链接：" prop="menuUrl">
                <Input v-model="aformValidate.menuUrl" placeholder="请输入菜单链接"></Input>
              </FormItem>
              <FormItem label="上级：" prop="parentMenuCode">
                <Select v-model="aformValidate.parentMenuCode" filterable placeholder="选择上级菜单">
                  <Option :value="'0'">无</Option>
                  <Option v-for="item in parentList" :key="item.id" :value="item.id">{{item.title}}</Option>
                </Select>
              </FormItem>
              <FormItem label="排序：" prop="menuSortNo">
                <Input v-model="aformValidate.menuSortNo" placeholder="请输入序号"></Input>
              </FormItem>
              <FormItem label="状态：" prop="dataState">
                <RadioGroup v-model="aformValidate.dataState">
                  <Radio
                    v-for="item in menuState"
                    :key="item.value"
                    :label="item.value"
                  >{{item.label}}</Radio>
                </RadioGroup>
              </FormItem>
              <FormItem label="是否显示：" prop="hideInMenu">
                <RadioGroup v-model="aformValidate.hideInMenu">
                  <Radio
                    v-for="item in inMenuState"
                    :key="item.value"
                    :label="item.value"
                  >{{item.label}}</Radio>
                </RadioGroup>
              </FormItem>
              <FormItem label="备注：" prop="remark">
                <Input
                  v-model="aformValidate.remark"
                  type="textarea"
                  :rows="4"
                  placeholder="请输入备注信息"
                />
              </FormItem>
              <FormItem style="margin-top:90px;">
                <Button type="primary" @click="handleAddMenu" v-permission="buttonRole.CDGL_QD">确定</Button>
                <Button style="margin-left: 20px" @click="handleCancleAdd">取消</Button>
              </FormItem>
            </Form>
          </div>
        </div>
      </i-col>
    </Row>
    <!-- 删除模态框 -->
    <Modal title="删除菜单" v-model="deleteModal" class-name="vertical-center-modal">
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;"/>删除后不可恢复，您确定删除吗？
        </p>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleConfirmDelete">确定</Button>
        <Button class="cancelDelete" type="default" @click="handleCancelDelete">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
const BUTTONROLE = {
  CDGL_XZCD: 'DAML_XZ',
  CDGL_BC: 'CDGL_BC',
  CDGL_SC: 'CDGL_SC',
  CDGL_QD: 'CDGL_QD'
}
export default {
  name: 'menuManage',
  data () {
    return {
      treeData: [],
      detailShow: true,
      deleteModal: false,
      spinShow: true,
      id: '',
      parentList: [],
      formValidate: {
        title: '',
        iconUrl: '',
        id: '',
        parentMenuCode: '',
        menuSortNo: 0,
        dataState: 1,
        remark: '',
        menuUrl: '',
        hideInMenu: 1
      },
      aformValidate: {
        title: '',
        iconUrl: '',
        id: '',
        parentMenuCode: '',
        menuSortNo: 0,
        dataState: 1,
        remark: '',
        menuUrl: '',
        hideInMenu: 1
      },
      menuState: [
        {
          label: '可用',
          value: 1
        },
        {
          label: '禁用',
          value: 2
        }
      ],
      inMenuState: [
        {
          label: '是',
          value: 1
        },
        {
          label: '否',
          value: 0
        }
      ],
      aruleValidate: {
        title: [{ required: true, message: '菜单名不能为空', trigger: 'blur' }],
        iconUrl: [
          { required: true, message: '请输入图标地址', trigger: 'blur' }
        ],
        menuUrl: [
          { required: true, message: '请输入菜单地址', trigger: 'blur' }
        ],
        parentMenuCode: [
          { required: true, message: '请选择上级菜单', trigger: 'change' }
        ],
        dataState: [
          {
            required: true,
            type: 'number',
            message: '请选择状态',
            trigger: 'change'
          }
        ]
      },
      ruleValidate: {
        title: [{ required: true, message: '菜单名不能为空', trigger: 'blur' }],
        iconUrl: [
          { required: true, message: '请输入图标地址', trigger: 'blur' }
        ],
        menuUrl: [
          { required: true, message: '请输入菜单地址', trigger: 'blur' }
        ],
        parentMenuCode: [
          { required: true, message: '请选择上级菜单', trigger: 'change' }
        ],
        dataState: [
          // { required: true, type: 'number', message: '请选择状态', trigger: 'change' }
        ]
      },
      buttonRole: BUTTONROLE
    }
  },
  mounted () {
    this.handleGetMenuList() // 获取菜单列表
    this.handleGetsjData() // 获取上级菜单列表
  },
  methods: {
    // 点击新增
    handleAdd () {
      this.detailShow = false
    },
    // 确认新增菜单
    handleAddMenu () {
      this.$refs.aformValidate.validate(valid => {
        if (valid) {
          let addParams = {
            menuName: this.aformValidate.title,
            iconUrl: this.aformValidate.iconUrl,
            parentMenuCode: this.aformValidate.parentMenuCode,
            menuSortNo: this.aformValidate.menuSortNo,
            dataState: this.aformValidate.dataState,
            remark: this.aformValidate.remark,
            hideInMenu: this.aformValidate.hideInMenu,
            menuUrl: this.aformValidate.menuUrl
          }
          // console.log(addParams)
          this.swsApi.swsPost('Menu/CreateUpdateMenu', addParams).then(res => {
            if (res.data.error === null) {
              this.$Message.success('新增菜单成功')
              this.handleGetMenuList()
              this.handleGetsjData()
            }
          })
        } else {
          this.$Message.error('请完善必填信息')
        }
      })
    },
    // 删除菜单
    handleDeleteMenu () {
      this.deleteModal = true
    },
    // 确认删除
    handleConfirmDelete () {
      this.deleteModal = false
      this.swsApi.swsPost(`Menu/del/${this.id}`).then(res => {
        // console.log(res)
        if (res.data.error === null) {
          this.$Message.success('删除菜单成功')
          this.handleGetMenuList()
        }
      })
    },
    // 取消删除
    handleCancelDelete () {
      this.deleteModal = false
    },
    // 确认修改
    handleModifyMenu () {
      this.$refs.formValidate.validate(valid => {
        if (valid) {
          let modifyParams = {
            id: this.id,
            menuName: this.formValidate.title,
            iconUrl: this.formValidate.iconUrl,
            parentMenuCode: this.formValidate.parentMenuCode,
            menuSortNo: this.formValidate.menuSortNo,
            dataState: this.formValidate.dataState,
            remark: this.formValidate.remark,
            menuUrl: this.formValidate.menuUrl,
            hideInMenu: this.formValidate.hideInMenu
          }
          this.swsApi
            .swsPost('Menu/CreateUpdateMenu', modifyParams)
            .then(res => {
              if (res.data.error === null) {
                this.$Message.success('修改菜单成功')
                this.handleGetMenuList()
              }
            })
        } else {
          this.$Message.error('请完善必填信息!')
        }
      })
    },
    // 取消新增
    handleCancleAdd () {
      this.detailShow = true
    },
    // 获取菜单
    handleGetMenuList () {
      this.swsApi.swsPost('Menu/tree').then(res => {
        // console.log(res)
        if (res.data.error === null) {
          this.spinShow = false
        }
        this.treeData = res.data.result
      })
    },
    // 点击菜单项
    handleClickTreeNode (e) {
      if (!e.length) return
      this.formValidate = e[0]
      let _parentMenuCode = e[0].parentMenuCode
      this.formValidate.parentMenuCode = _parentMenuCode
      this.detailShow = true
      this.id = e[0].id
    },
    // 获取菜单列表
    handleGetsjData () {
      this.swsApi.swsPost('Menu/arry').then(res => {
        this.parentList = res.data.result
        // console.log(this.parentList)
      })
    }
  }
}
</script>

<style scoped lang='less'>
#menu-manage {
  background-color: #fff;
  height: 100%;
  position: relative;
  overflow: hidden;
  .menu,
  .operate-box {
    height: 100%;
  }
  .operate-box {
    .top + div {
      padding: 20px;
    }
    & > div {
      height: 100%;
      display: flex;
      flex-direction: column;
    }
  }
  .form {
    max-width: 540px;
  }
}
/deep/ .menuTree .ivu-tree .ivu-tree-children li .ivu-tree-title {
  font-size: 15px;
  color: #424242;
}

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
          padding-top: 20px;
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
</style>
