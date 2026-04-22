<template>
  <div style="padding:0 20px; position: relative">
    <Spin fix v-if="spinShow">
      <Icon type="ios-loading" size="18" class="demo-spin-icon-load"></Icon>
      <div>Loading</div>
    </Spin>
    <Row>
      <i-col :sm="24" :md="24" :lg="24">
        <Card>
          <Row>
            <i-col :sm="6" :md="6" :lg="6" style="min-height: 1px">

              <Button
                icon="md-add"
                type="primary"
                @click="addModal=true"
                v-permission="buttonRole.JSGL_XZ"
              >新增角色</Button>
            </i-col>
            <i-col :sm="18" :md="18" :lg="18">
              <Input
                class="search"
                v-model="inputValue"
                @on-search="handleSearch"
                search
                enter-button
                placeholder="搜索"
                style="width:35%;"
              />
            </i-col>
          </Row>
        </Card>
      </i-col>
    </Row>
    <Row :gutter="16">
      <i-col
        :sm="12"
        :md="8"
        :lg="6"
        style="margin-top:16px"
        v-for="item in roleList"
        :key="item.id"
      >
        <Card>
          <Row>
            <!-- <i-col :sm="12" :md="12" :lg="12" style="font-size:16px;">{{item.roleName}}</i-col> -->
            <i-col :sm="18" :md="18" :lg="18" style="font-size:16px;">{{item.roleName}}</i-col>
            <i-col :sm="6" :md="6" :lg="6" style="text-align:right">
              <Tag color="success" v-if="item.dataState === 1">可用</Tag>
              <Tag color="error" v-else>禁用</Tag>
            </i-col>
          </Row>
          <Row>
            <i-col
              :sm="24"
              :md="24"
              :lg="24"
              style="color:#9b9b9b; font-size:14px; margin-top:20px;"
            >{{item.roleCode}}</i-col>
          </Row>
          <div class="btn-group" style="margin-top:50px">
            <div>
              <Button
                :disabled="!buttonRole.JSGL_XG"
                long
                style="color:#5a96f6;font-size:16px;"
                @click="handleModifyRole(item.id)"
              >
                <Icon type="ios-create-outline" size="23" style="margin-right:10px"/>
                <span>修改</span>
              </Button>
            </div>
            <div>
              <Button
                :disabled="!buttonRole.JSGL_SC"
                class="del-btn"
                @click="handleDeleteRole(item.id)"
              >
                <Icon type="ios-trash" size="24" color="#ff3e47"/>
              </Button>
            </div>
          </div>
        </Card>
      </i-col>
    </Row>
    <!-- 增加模态框 -->
    <Modal title="新增角色信息" width="400" v-model="addModal" class-name="vertical-center-modal">
      <div>
        <Form
          ref="formValidate"
          :model="formValidate"
          :rules="ruleValidate"
          :label-width="90"
          label-position="right"
        >
          <FormItem label="角色名称：" prop="roleName">
            <Input type="text" v-model="formValidate.roleName"></Input>
          </FormItem>
          <FormItem label="描述：" prop="remark">
            <Input type="text" v-model="formValidate.remark"></Input>
          </FormItem>
          <FormItem label="状态" prop="dataState">
            <RadioGroup v-model="formValidate.dataState">
              <Radio :label="item.value" v-for="item in roleState" :key="item.value">{{item.label}}</Radio>
            </RadioGroup>
          </FormItem>
        </Form>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleConfirmAdd()">确定</Button>
        <Button type="default" @click="handleCancelAdd()">取消</Button>
      </div>
    </Modal>
    <!-- 修改模态框 -->
    <Modal title="修改角色信息" width="400" v-model="modifyModal" class-name="vertical-center-modal">
      <div>
        <Form
          ref="mformValidate"
          :model="mformValidate"
          :rules="rulemValidate"
          :label-width="90"
          label-position="right"
        >
          <FormItem label="角色名称：" prop="roleName">
            <Input type="text" v-model="mformValidate.roleName"></Input>
          </FormItem>
          <FormItem label="描述：" prop="remark">
            <Input type="text" v-model="mformValidate.remark" number></Input>
          </FormItem>
          <FormItem label="状态" prop="dataState">
            <RadioGroup v-model="mformValidate.dataState">
              <Radio :label="item.value" v-for="item in roleState" :key="item.value">{{item.label}}</Radio>
            </RadioGroup>
          </FormItem>
        </Form>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleConfirmModify()">确定</Button>
        <Button type="default" @click="handleCancelModify()">取消</Button>
      </div>
    </Modal>
    <!-- 删除模态框 -->
    <Modal title="删除角色" v-model="deleteModal" width="400" class-name="vertical-center-modal">
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;"/>删除后不可恢复，您确定删除吗？
        </p>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleConfirmDelete()">确定</Button>
        <Button class="cancelDelete" type="default" @click="handleCancelDelete()">取消</Button>
      </div>
    </Modal>
  </div>
</template>
<script>
const BUTTONROLE = {
  JSGL_XZ: 'JSGL_XZ',
  JSGL_XG: 'JSGL_XG',
  JSGL_SC: 'JSGL_SC'
}
export default {
  name: 'roleManage',
  data () {
    let _this = this
    return {
      spinShow: true,
      addModal: false,
      modifyModal: false,
      deleteModal: false,
      id: '',
      inputValue: '',
      roleList: [], // 角色列表
      roleName: '', // 角色名
      roleCode: '', // 角色编码
      dataState: '', // 角色状态
      roleState: [
        {
          label: '可用',
          value: 1
        },
        {
          label: '禁用',
          value: 2
        }
      ],
      formValidate: {
        roleName: '',
        remark: '',
        dataState: ''
      },
      mformValidate: {
        roleName: '',
        remark: '',
        dataState: ''
      },
      // 新增角色表单验证
      ruleValidate: {
        roleName: [
          {
            required: true,
            type: 'string',
            message: '角色名不能为空',
            trigger: 'blur'
          }
        ],
        remark: [
          {
            required: true,
            type: 'string',
            message: '描述信息不能为空',
            trigger: 'blur'
          }
        ],
        dataState: [
          {
            required: true,
            type: 'number',
            message: '类型状态不能为空',
            trigger: 'blur'
          }
        ]
      },
      // 修改角色表单验证
      rulemValidate: {
        roleName: [
          {
            required: true,
            type: 'string',
            message: '角色名不能为空',
            trigger: 'blur'
          }
        ],
        remark: [
          {
            required: true,
            type: 'string',
            message: '描述信息不能为空',
            trigger: 'blur'
          }
        ],
        dataState: [
          {
            required: true,
            type: 'number',
            message: '类型状态不能为空',
            trigger: 'blur'
          }
        ]
      },
      buttonRole: {
        JSGL_XZ: BUTTONROLE.JSGL_XZ,
        JSGL_SC: _this._filterButton(BUTTONROLE.JSGL_SC),
        JSGL_XG: _this._filterButton(BUTTONROLE.JSGL_XG)
      }
    }
  },
  mounted () {
    this.handleGetRoleList() // 获取角色列表
  },
  methods: {
    // 添加角色
    handleConfirmAdd () {
      this.$refs.formValidate.validate(valid => {
        if (valid) {
          let addParams = {
            roleName: this.formValidate.roleName,
            remark: this.formValidate.remark,
            dataState: this.formValidate.dataState
          }
          this.swsApi.swsPost('Role/CreateUpdate', addParams).then(res => {
            if (res.data.success) {
              this.$Message.success('操作成功！')
              this.handleGetRoleList()
            }
            this.addModal = false
          })
        } else {
          this.$Message.error('请完善必填信息！')
        }
      })
    },
    // 取消添加角色
    handleCancelAdd () {
      this.addModal = false
    },
    // 修改角色
    handleConfirmModify () {
      this.modifyModal = false
      let modifyParams = {
        roleName: this.mformValidate.roleName,
        remark: this.mformValidate.remark,
        dataState: this.mformValidate.dataState,
        id: this.id
      }
      this.swsApi.swsPost('Role/CreateUpdate', modifyParams).then(res => {
        if (res.data.success) {
          this.$Message.success('操作成功！')
          this.handleGetRoleList()
        }
      })
    },
    // 取消修改角色
    handleCancelModify () {
      this.modifyModal = false
      this.$refs.mformValidate.resetFields()
    },
    // 删除角色
    handleConfirmDelete (id) {
      this.deleteModal = false
      this.swsApi.swsGet(`Role/del/${this.id}`).then(res => {
        // console.log(res)
        this.handleGetRoleList()
      })
    },
    // 取消删除角色
    handleCancelDelete () {
      this.deleteModal = false
    },
    // 获取角色列表
    handleGetRoleList () {
      this.spinShow = true
      this.swsApi.swsPost('Role/RoleList').then(res => {
        // console.log(res)
        if (res.data.error === null) {
          this.spinShow = false
        }
        this.roleList = res.data.result
        // console.log(this.roleList)
      })
    },
    handleModifyRole (id) {
      this.id = id
      this.modifyModal = true
      let data = this.roleList.filter(v => {
        return id === v.id
      })[0]

      this.mformValidate = Object.assign({}, data)
    },
    handleDeleteRole (id) {
      this.deleteModal = true
      this.id = id
    },
    handleSearch () {
      this.swsApi
        .swsPost('Role/RoleList', { roleName: this.inputValue })
        .then(res => {
          this.roleList = res.data.result
        })
    }
  }
}
</script>

<style scoped lang='less'>
.demo-spin-icon-load {
  animation: ani-demo-spin 1s linear infinite;
}
.search {
  float: right;
}
.del-btn {
  background-color: #ffebec;
}
.ivu-btn-long {
  background-color: #f3f8fe;
}
.vertical-center-modal {
  display: flex;
  align-items: center;
  justify-content: center;
}
.btn-group {
  display: flex;
  align-items: center;
  div:first-child {
    flex: 1;
    margin-right: 20px;
  }
}
</style>
