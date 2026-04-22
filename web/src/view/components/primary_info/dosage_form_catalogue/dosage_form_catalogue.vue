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
            <span style="color: #999999; font-size: 15px; margin-right: 22px;">剂型目录</span>
            <Button type="primary" @click="handleAdd" v-permission="buttonRole.JXML_XZ">新增剂型</Button>
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
            <div class="top_right">剂型管理</div>
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
              <FormItem label="剂型名称：" prop="typeName">
                <Input v-model="formValidate.typeName" placeholder="请输入剂型名称"></Input>
              </FormItem>
              <FormItem label="编码：" prop="typeCode">
                <Input v-model="formValidate.typeCode" placeholder="请输入序号"></Input>
              </FormItem>
              <FormItem label="上级：" prop="parentId">
                <Select v-model="formValidate.parentId" placeholder filterable>
                  <Option :value="empty.value">{{empty.label}}</Option>
                  <Option v-for="item in parentList" :key="item.id" :value="item.id">{{item.title}}</Option>
                </Select>
              </FormItem>
              <FormItem label="排序：" prop="sortNo">
                <Input v-model="formValidate.sortNo" placeholder="请输入序号"></Input>
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
                  v-permission="buttonRole.JXML_BC"
                >保存</Button>
                <Button
                  style="margin-left: 20px;"
                  type="error"
                  v-permission="buttonRole.JXML_SC"
                  @click="handleDeleteMenu"
                >删除</Button>
              </FormItem>
            </Form>
          </div>
        </div>
        <!-- 新增 -->
        <div v-show="!detailShow">
          <div class="top">
            <div class="top_right">新增剂型管理</div>
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
              <FormItem label="剂型名称：" prop="typeName">
                <Input v-model="aformValidate.typeName" placeholder="请输入剂型名称"></Input>
              </FormItem>
              <FormItem label="编码：" prop="typeCode">
                <Input v-model="aformValidate.typeCode" placeholder="请输入序号"></Input>
              </FormItem>
              <FormItem label="上级：" prop="parentId">
                <Select v-model="aformValidate.parentId" placeholder="请选择选择上级" filterable>
                  <Option :value="empty.value">{{empty.label}}</Option>
                  <Option v-for="item in parentList" :key="item.id" :value="item.id">{{item.title}}</Option>
                </Select>
              </FormItem>
              <FormItem label="排序：" prop="sortNo">
                <Input v-model="aformValidate.sortNo" placeholder="请输入序号"></Input>
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
                <Button type="primary" @click="handleAddMenu" v-permission="buttonRole.JXML_QD">确定</Button>
                <Button style="margin-left: 20px" @click="handleCancleAdd">取消</Button>
              </FormItem>
            </Form>
          </div>
        </div>
      </i-col>
    </Row>
    <!-- 删除模态框 -->
    <Modal title="删除剂型" v-model="deleteModal" class-name="vertical-center-modal">
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
  JXML_XZ: 'JXML_XZ',
  JXML_BC: 'JXML_BC',
  JXML_SC: 'JXML_SC',
  JXML_QD: 'JXML_QD'
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
      empty: {
        label: '无',
        value: '0'
      },
      parentList: [],
      formValidate: {
        id: -1,
        typeName: '',
        typeCode: '',
        parentId: '',
        sortNo: 0,
        remark: ''
      },
      aformValidate: {
        typeName: '',
        typeCode: '',
        parentId: '',
        sortNo: 0,
        remark: ''
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
      aruleValidate: {
        typeName: [
          { required: true, message: '剂型名不能为空', trigger: 'blur' }
        ],
        typeCode: [{ required: true, message: '请输入编码', trigger: 'blur' }],
        parentId: [
          { required: true, message: '请选择上级剂型', trigger: 'change' }
        ]
      },
      ruleValidate: {
        typeName: [
          { required: true, message: '剂型名不能为空', trigger: 'blur' }
        ],
        typeCode: [{ required: true, message: '请输入编码', trigger: 'blur' }],
        parentId: [
          { required: true, message: '请选择上级剂型', trigger: 'change' }
        ]
      },
      buttonRole: BUTTONROLE
    }
  },
  mounted () {
    this.handleGetMenuList() // 获取剂型列表
    this.handleGetsjData() // 获取上级库房列表
  },
  methods: {
    // 点击新增
    handleAdd () {
      this.detailShow = false
    },
    // 确认新增剂型
    handleAddMenu () {
      this.$refs.aformValidate.validate(valid => {
        if (valid) {
          this.swsApi
            .swsPost('Data/DosageForm/CreateUpdate', this.aformValidate)
            .then(res => {
              if (res.data.error === null) {
                this.$Message.success('新增剂型成功')
                this.handleGetMenuList()
                this.handleGetsjData()

                this.$refs.aformValidate.resetFields()
              } else {
                this.$Notice.error({
                  title: '操作错误',
                  desc: res.data.error
                })
              }
            })
        } else {
          this.$Message.error('请完善必填信息')
        }
      })
    },
    // 删除剂型
    handleDeleteMenu () {
      this.deleteModal = true
    },
    // 确认删除
    handleConfirmDelete () {
      this.deleteModal = false
      this.swsApi.swsPost(`Data/DosageForm/del/${this.id}`).then(res => {
        if (res.data.error === null) {
          this.$Message.success('删除库房成功')
          this.$refs.formValidate.resetFields()
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
          this.swsApi
            .swsPost('Data/DosageForm/CreateUpdate', this.formValidate)
            .then(res => {
              if (res.data.error === null) {
                this.$Message.success('修改剂型成功')
                this.handleGetMenuList()
                this.$refs.formValidate.resetFields()
              } else {
                this.$Notice.error({
                  title: '操作错误',
                  desc: res.data.error
                })
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
    // 获取剂型
    handleGetMenuList () {
      this.swsApi.swsPost('Data/DosageForm/tree').then(res => {
        // console.log(res)
        if (res.data.error === null) {
          this.spinShow = false
        }
        this.treeData = res.data.result
      })
    },
    // 点击库房项
    handleClickTreeNode (e) {
      if (!e.length) return

      const { parentId, typeName, typeCode, sortNo, remark, id } = e[0]
      this.formValidate.parentId = parentId
      this.formValidate.typeName = typeName
      this.formValidate.typeCode = typeCode
      this.formValidate.sortNo = sortNo
      this.formValidate.remark = remark
      this.formValidate.id = id

      this.detailShow = true
      this.id = e[0].id
    },
    // 获取剂型列表
    handleGetsjData () {
      this.swsApi.swsPost('Data/DosageForm/list').then(res => {
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
