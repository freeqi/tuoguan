<template>
  <div style="padding: 20px 20px 0;background-color:#fff;height:100%;">
    <!-- header -->
    <Card>
      <Row>
        <i-col :sm="24" :md="24" :lg="24">
          <Row>
            <i-col :sm="12" :md="14" :lg="10" style="min-height: 1px">
              <Button
                icon="md-add"
                type="primary"
                @click="addModal=true"
                v-permission="buttonRole.YHGL_XZ"
              >添加账号</Button>
              <Button
                v-permission="buttonRole.YHGL_QY"
                class="qiyong"
                type="primary"
                ghost
                @click="handleQiyong"
              >
                <i class="iconfont icon-qiyong1" style="margin-right:5px;"></i>启用
              </Button>
              <Button
                v-permission="buttonRole.YHGL_JY"
                class="jinyong"
                type="error"
                ghost
                @click="handleJinyong"
              >
                <i class="iconfont icon-jinyong" style="margin-right:5px;"></i>禁用
              </Button>
            </i-col>
            <i-col :sm="12" :md="10" :lg="14" style="text-align:right">
              <Input
                class="search"
                search
                enter-button
                @on-search="handleSearch"
                v-model="inputValue"
                placeholder="搜索"
                style="width:50%;"
              />
            </i-col>
          </Row>
        </i-col>
      </Row>
    </Card>
    <!-- 表格 -->
    <Row style="margin-top:20px" class="user-detail-table">
      <i-col :sm="24" :md="24" :lg="24">
        <Table
          ref="selection"
          :columns="columns"
          :data="table_data"
          @on-select-all="handleSelectRowAll"
          @on-selection-change="handleSelectRow"
          :loading="loading"
        ></Table>
        <div style="margin:16px;text-align:right">
          <Page
            v-if="dataCount>10"
            :total="dataCount"
            :current.sync="current"
            @on-change="handleChangePage"
          />
        </div>
      </i-col>
    </Row>
    <!-- 添加账号 -->
    <Modal title="添加账号" v-model="addModal" class-name="vertical-center-modal">
      <div>
        <Form
          ref="formValidateRef"
          :model="formValidate"
          :rules="ruleValidate"
          :label-width="80"
          autocomplete="off"
          label-position="right"
        >
          <FormItem label="用户名：" prop="userName">
            <Input type="text" v-model="formValidate.userName" placeholder="请输入用户名"></Input>
          </FormItem>
          <FormItem label="姓名：" prop="name">
            <Select
              v-model="formValidate.name"
              filterable
              label-in-value
              @on-change="handleSelect"
              transfer
              placeholder="请选择用户"
            >
              <Option v-for="item in ygList" :key="item.id" :value="item.id">{{item.name}}</Option>
            </Select>
          </FormItem>
          <FormItem label="密码：" prop="password">
            <Input
              :type="inputType"
              v-model="formValidate.password"
              @on-focus="inputType='password'"
              placeholder="请输入密码"
            ></Input>
          </FormItem>
          <FormItem label="角色：" prop="role">
            <Select
              v-model="formValidate.role"
              multiple
              label-in-value
              @on-change="handleRoleSelect"
              transfer
              placeholder="请选择角色"
            >
              <Option v-for="item in roleList" :key="item.id" :value="item.id">{{item.roleName}}</Option>
            </Select>
          </FormItem>
          <FormItem label="状态" prop="isActive">
            <RadioGroup v-model="formValidate.isActive">
              <Radio v-for="item in userState" :key="item.value" :label="item.value">{{item.label}}</Radio>
            </RadioGroup>
          </FormItem>
          <FormItem label="详情：" prop="note">
            <Input type="textarea" v-model="formValidate.note" :rows="4" placeholder="请输入备注"></Input>
          </FormItem>
        </Form>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleConfirmAdd">确定</Button>
        <Button class="cancelDelete" type="default" @click="handleCancelAdd">取消</Button>
      </div>
    </Modal>
    <!-- 修改账号 -->
    <Modal title="修改账号" v-model="modifyModal" class-name="vertical-center-modal">
      <div>
        <Form
          ref="mformValidateRef"
          :model="mformValidate"
          :rules="mruleValidate"
          :label-width="80"
          label-position="right"
        >
          <FormItem label="用户名：" prop="userName">
            <Input type="text" disabled v-model="mformValidate.userName"></Input>
          </FormItem>
          <FormItem label="姓名：" prop="name">
            <Select v-model="mformValidate.name" disabled transfer>
              <Option v-for="item in ygList" :key="item.id" :value="item.name">{{item.name}}</Option>
            </Select>
          </FormItem>
          <!-- <FormItem label="密码：" prop="password">
              <Input type="password" v-model="mformValidate.password" placeholder="请输入密码"></Input>
          </FormItem>-->
          <FormItem label="角色：" prop="role">
            <Select
              v-model="mformValidate.role"
              multiple
              label-in-value
              @on-change="handleRoleSelect"
              transfer
              placeholder="请选择角色"
            >
              <Option v-for="item in roleList" :key="item.id" :value="item.id">{{item.roleName}}</Option>
            </Select>
          </FormItem>
          <FormItem label="状态" prop="isActive">
            <RadioGroup v-model="mformValidate.isActive">
              <Radio v-for="item in userState" :key="item.value" :label="item.value">{{item.label}}</Radio>
            </RadioGroup>
          </FormItem>
          <FormItem label="详情：" prop="note">
            <Input type="textarea" v-model="mformValidate.note" :rows="4" placeholder="请输入备注"></Input>
          </FormItem>
        </Form>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleConfirmModify">确定</Button>
        <Button class="cancelDelete" type="default" @click="handleCancelModify()">取消</Button>
      </div>
    </Modal>
    <!-- 删除 -->
    <Modal title="删除账号" v-model="deleteModal" class-name="vertical-center-modal">
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;"/>删除后不可恢复，您确定删除吗？
        </p>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleConfirmDelete">确定</Button>
        <Button class="cancelDelete" type="default" @click="handleCancelDelete()">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import Operate from '@/components/operate'
const BUTTONROLE = {
  YHGL_XG: 'YHGL_XG',
  YHGL_SC: 'YHGL_SC',
  YHGL_XZ: 'YHGL_XZ',
  YHGL_QY: 'YHGL_QY',
  YHGL_JY: 'YHGL_JY'
}
export default {
  name: 'userManage',
  data () {
    return {
      // 解决输入框被自动填充导致用户名下拉数据出错问题
      inputType: 'text',
      loading: false,
      addModal: false,
      modifyModal: false,
      deleteModal: false,
      inputValue: '',
      current: 1,
      size: 10,
      dataCount: '',
      id: '',
      data: [],
      ygList: [],
      ygId: '', // 员工id
      roleId: [], // 角色id
      roleList: [],
      idAr: [],
      formValidate: {
        userName: '',
        name: '',
        password: '',
        role: [],
        isActive: '',
        note: ''
      },
      mformValidate: {
        userName: '',
        name: '',
        password: '',
        role: [],
        isActive: '',
        note: ''
      },
      ruleValidate: {
        password: [
          { required: true, message: '密码不能为空', trigger: 'blur' }
        ],
        name: [
          { required: true, message: '姓名不能为空', trigger: 'blur' },
          { message: '姓名不能为空', trigger: 'change' }
        ],
        userName: [
          { required: true, message: '用户名不能为空', trigger: 'blur' }
        ],
        role: [
          {
            required: true,
            type: 'array',
            message: '角色不能为空',
            trigger: 'blur'
          },
          { type: 'array', message: '角色不能为空', trigger: 'change' }
        ],
        isActive: [
          {
            required: true,
            type: 'number',
            message: '选择账号状态',
            trigger: 'change'
          }
        ]
      },
      mruleValidate: {
        role: [
          {
            required: true,
            type: 'array',
            message: '角色不能为空',
            trigger: 'blur'
          }
        ],
        isActive: [
          {
            required: true,
            type: 'number',
            message: '选择账号状态',
            trigger: 'change'
          }
        ]
      },
      columns: [],
      table_column_default: [
        {
          type: 'selection',
          width: 30,
          align: 'left'
        },
        {
          title: '用户名',
          key: 'userName'
        },
        {
          title: '姓名',
          key: 'name'
        },
        {
          title: '机构',
          key: 'centerDialysisName',
          render: (h, params) => {
            return <span>{params.row.centerDialysisName || '-'}</span>
          }
        },
        {
          title: '职位',
          key: 'position'
        },
        {
          title: '角色',
          key: 'role',
          render: (h, params) => {
            let data = params.row.role

            let datas = data.map(v => {
              let name = this.roleList.filter(_v => {
                return _v.id === v
              })[0].roleName

              return name
            })
            datas = datas.join(',')
            return <div>{datas}</div>
          }
        },
        {
          title: '状态',
          key: 'isActive',
          width: 100,
          align: 'center',
          render: (h, params) => {
            let _text = params.row.isActive === 1 ? '可用' : '禁用'
            let _color = params.row.isActive === 1 ? 'success' : 'error'
            return <tag color={_color}>{_text}</tag>
          }
        }
      ],
      table_column_action: [{
        title: '操作',
        key: 'action',
        width: 150,
        align: 'center',
        render: (h, params) => {
          return (
            <Operate
              showWatch={false}
              handleDelete={() => {
                this.deleteModal = true
                this.id = params.row.id
              }}
              handleEdit={() => {
                this.modifyModal = true
                this.id = params.row.id
                let validate = this.data.filter(v => {
                  return v.id === params.row.id
                })[0]

                this.mformValidate = JSON.parse(JSON.stringify(validate))
              }}
              permissionEdit={this.buttonRole.YHGL_XG}
              permissionDelete={this.buttonRole.YHGL_SC}
            />
          )
        }
      }
      ],
      table_data: [],
      userState: [
        {
          value: 1,
          label: '可用'
        },
        {
          value: 0,
          label: '禁用'
        }
      ],

      buttonRole: BUTTONROLE
    }
  },
  mounted () {
    this.handleGetygList() // 获取员工列表
    this.handleGetRoleList() // 获取角色列表
    if (this._filterButton(this.buttonRole.YHGL_XG) || this._filterButton(this.buttonRole.YHGL_SC)) {
      this.columns = [...this.table_column_default, ...this.table_column_action]
    } else {
      this.columns = this.table_column_default
    }
  },
  methods: {
    // 分页获取用户列表
    handleChangePage () {
      if (this.loading) return
      this.loading = true
      let pageParams = {
        pageNum: this.current,
        pageSize: this.size,
        searchValue: this.inputValue
      }
      this.swsApi.swsPost('User/userlist', pageParams).then(res => {
        this.dataCount = res.data.dataCount
        this.data = res.data.result
        if (res.data.error === null) {
          this.loading = false
          this.table_data = res.data.result
        }
      })
    },
    // 获取员工列表
    handleGetygList () {
      this.swsApi.swsPost('Employee/Employee').then(res => {
        // console.log(res)
        this.ygList = res.data.result
      })
    },
    // 获取角色列表
    handleGetRoleList () {
      this.swsApi.swsPost('Role/RoleList').then(res => {
        // console.log(res)
        if (res.data.success) {
          this.roleList = res.data.result
          this.handleChangePage() // 获取用户列表
        }
      })
    },
    // 选中某一行
    handleSelectRow (selection) {
      this.idAr = selection.map(item => {
        return item.id
      })
    },
    // 全选
    handleSelectRowAll (selection) {
      this.idAr = selection.map(item => {
        return item.id
      })
    },
    // 确认增加
    handleConfirmAdd () {
      this.$refs.formValidateRef.validate(valid => {
        if (valid) {
          this.addModal = false
          let addParams = {
            userName: this.formValidate.userName,
            pwd: this.formValidate.password,
            name: this.formValidate.name,
            roleId: this.roleId,
            note: this.formValidate.note,
            isActive: this.formValidate.isActive,
            employeeId: this.ygId
          }
          this.swsApi.swsPost('User/Setuser', addParams).then(res => {
            console.log(res)
            if (res.data.error === null) {
              this.$Message.success('新增账号成功')
              this.handleChangePage()
            } else {
              this.$Notice.error({
                title: '新增错误',
                desc: res.data.error
              })
            }
          })
        } else {
          this.$Message.error('请完善必填信息')
        }
      })
    },
    // 下拉框选取项员工id
    handleSelect (val) {
      if (val) {
        this.ygId = val.value
      }
      // console.log(this.ygId)
    },
    // 下拉框选择项角色id
    handleRoleSelect (val) {
      let a = []
      val.forEach((item, i) => {
        a.push(item.value)
      })
      this.roleId = a
    },
    handleCancelAdd () {
      this.addModal = false
      // this.$refs.formValidateRef.resetFields()
    },
    // 确认修改
    handleConfirmModify () {
      this.$refs.mformValidateRef.validate(valid => {
        if (valid) {
          this.modifyModal = false
          let modifyParams = {
            id: this.id,
            roleId: this.roleId,
            note: this.mformValidate.note,
            isActive: this.mformValidate.isActive
          }
          this.swsApi.swsPost('User/Updateuser', modifyParams).then(res => {
            if (res.data.error === null) {
              this.$Message.success('修改成功')
              this.handleChangePage()
            }
          })
        } else {
          this.$Message.error('请完善必填信息')
        }
      })
    },
    handleCancelModify () {
      this.modifyModal = false
    },
    // 确认删除
    handleConfirmDelete () {
      this.deleteModal = false
      let id = this.id
      this.swsApi.swsGet(`User/user/del/${id}`).then(res => {
        if (res.data.error === null) {
          this.$Message.success('删除成功')
          this.handleChangePage()
        }
      })
    },
    // 取消删除
    handleCancelDelete () {
      this.deleteModal = false
    },
    // 关键字搜索
    handleSearch () {
      this.current = 1
      this.handleChangePage()
    },
    // 批量启用
    handleQiyong () {
      if (this.idAr.length === 0) return false
      let qiyongParams = {
        id: this.idAr,
        isActive: true
      }
      this.swsApi.swsPost('User/userActive', qiyongParams).then(res => {
        // console.log(res)
        if (res.data.error === null) {
          this.$Message.success('账号启用成功')
          this.idAr = []
          this.handleChangePage()
        } else {
          this.$Message.error('账号启用失败，请稍后重试')
        }
      })
    },
    // 批量禁用
    handleJinyong () {
      if (this.idAr.length === 0) return false
      let jinyongParams = {
        id: this.idAr,
        isActive: false
      }
      this.swsApi.swsPost('User/userActive', jinyongParams).then(res => {
        // console.log(res)
        if (res.data.error === null) {
          this.$Message.success('账号禁用成功')
          this.idAr = []
          this.handleChangePage()
        } else {
          this.$Message.error('账号禁用失败，请稍后重试')
        }
      })
    }
  },
  components: {
    Operate
  }
}
</script>

<style scoped lang='less'>
.user-detail-table {
  // cy:调大table的字体
  .ivu-table-wrapper {
    & /deep/ .ivu-table {
      font-size: 13px;
    }
  }
}
.search {
  float: right;
}
.jinyong,
.qiyong {
  margin-left: 20px;
  font-size: 12px;
}
</style>
