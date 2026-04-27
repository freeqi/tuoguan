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
                v-permission="buttonRole.ZHGL_XZ"
              >添加租户</Button>
              <Button
                v-permission="buttonRole.ZHGL_QY"
                class="qiyong"
                type="primary"
                ghost
                @click="handleQiyong"
              >
                <i class="iconfont icon-qiyong1" style="margin-right:5px;"></i>启用
              </Button>
              <Button
                v-permission="buttonRole.ZHGL_JY"
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
    <Row style="margin-top:20px" class="tenant-detail-table">
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
    <!-- 添加租户 -->
    <Modal title="添加租户" v-model="addModal" class-name="vertical-center-modal">
      <div>
        <Form
          ref="formValidateRef"
          :model="formValidate"
          :rules="ruleValidate"
          :label-width="80"
          autocomplete="off"
          label-position="right"
        >
          <FormItem label="租户名称：" prop="name">
            <Input type="text" v-model="formValidate.name" placeholder="请输入租户名称"></Input>
          </FormItem>
          <FormItem label="联系人：" prop="contactPerson">
            <Input type="text" v-model="formValidate.contactPerson" placeholder="请输入联系人"></Input>
          </FormItem>
          <FormItem label="联系电话：" prop="contactPhone">
            <Input type="text" v-model="formValidate.contactPhone" placeholder="请输入联系电话"></Input>
          </FormItem>
          <FormItem label="联系地址：" prop="address">
            <Input type="textarea" v-model="formValidate.address" :rows="3" placeholder="请输入联系地址"></Input>
          </FormItem>
          <FormItem label="状态" prop="isActive">
            <RadioGroup v-model="formValidate.isActive">
              <Radio v-for="item in tenantState" :key="item.value" :label="item.value">{{item.label}}</Radio>
            </RadioGroup>
          </FormItem>
          <FormItem label="备注：" prop="note">
            <Input type="textarea" v-model="formValidate.note" :rows="3" placeholder="请输入备注"></Input>
          </FormItem>
        </Form>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleConfirmAdd">确定</Button>
        <Button class="cancelDelete" type="default" @click="handleCancelAdd">取消</Button>
      </div>
    </Modal>
    <!-- 修改租户 -->
    <Modal title="修改租户" v-model="modifyModal" class-name="vertical-center-modal">
      <div>
        <Form
          ref="mformValidateRef"
          :model="mformValidate"
          :rules="mruleValidate"
          :label-width="80"
          label-position="right"
        >
          <FormItem label="租户名称：" prop="name">
            <Input type="text" v-model="mformValidate.name" placeholder="请输入租户名称"></Input>
          </FormItem>
          <FormItem label="联系人：" prop="contactPerson">
            <Input type="text" v-model="mformValidate.contactPerson" placeholder="请输入联系人"></Input>
          </FormItem>
          <FormItem label="联系电话：" prop="contactPhone">
            <Input type="text" v-model="mformValidate.contactPhone" placeholder="请输入联系电话"></Input>
          </FormItem>
          <FormItem label="联系地址：" prop="address">
            <Input type="textarea" v-model="mformValidate.address" :rows="3" placeholder="请输入联系地址"></Input>
          </FormItem>
          <FormItem label="状态" prop="isActive">
            <RadioGroup v-model="mformValidate.isActive">
              <Radio v-for="item in tenantState" :key="item.value" :label="item.value">{{item.label}}</Radio>
            </RadioGroup>
          </FormItem>
          <FormItem label="备注：" prop="note">
            <Input type="textarea" v-model="mformValidate.note" :rows="3" placeholder="请输入备注"></Input>
          </FormItem>
        </Form>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleConfirmModify">确定</Button>
        <Button class="cancelDelete" type="default" @click="handleCancelModify">取消</Button>
      </div>
    </Modal>
    <!-- 删除 -->
    <Modal title="删除租户" v-model="deleteModal" class-name="vertical-center-modal">
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
import Operate from '@/components/operate'
const BUTTONROLE = {
  ZHGL_XG: 'ZHGL_XG',
  ZHGL_SC: 'ZHGL_SC',
  ZHGL_XZ: 'ZHGL_XZ',
  ZHGL_QY: 'ZHGL_QY',
  ZHGL_JY: 'ZHGL_JY'
}
export default {
  name: 'tenantManage',
  data () {
    return {
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
      idAr: [],
      formValidate: {
        name: '',
        contactPerson: '',
        contactPhone: '',
        address: '',
        isActive: 1,
        note: ''
      },
      mformValidate: {
        name: '',
        contactPerson: '',
        contactPhone: '',
        address: '',
        isActive: 1,
        note: ''
      },
      ruleValidate: {
        name: [
          { required: true, message: '租户名称不能为空', trigger: 'blur' }
        ],
        contactPerson: [
          { required: true, message: '联系人不能为空', trigger: 'blur' }
        ],
        contactPhone: [
          { required: true, message: '联系电话不能为空', trigger: 'blur' }
        ],
        isActive: [
          {
            required: true,
            type: 'number',
            message: '选择租户状态',
            trigger: 'change'
          }
        ]
      },
      mruleValidate: {
        name: [
          { required: true, message: '租户名称不能为空', trigger: 'blur' }
        ],
        contactPerson: [
          { required: true, message: '联系人不能为空', trigger: 'blur' }
        ],
        contactPhone: [
          { required: true, message: '联系电话不能为空', trigger: 'blur' }
        ],
        isActive: [
          {
            required: true,
            type: 'number',
            message: '选择租户状态',
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
          title: '租户名称',
          key: 'name'
        },
        {
          title: '联系人',
          key: 'contactPerson'
        },
        {
          title: '联系电话',
          key: 'contactPhone'
        },
        {
          title: '联系地址',
          key: 'address',
          render: (h, params) => {
            return <span>{params.row.address || '-'}</span>
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
        },
        {
          title: '创建时间',
          key: 'createDate',
          render: (h, params) => {
            return <span>{params.row.createDate ? new Date(params.row.createDate).toLocaleString() : '-'}</span>
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
              permissionEdit={this.buttonRole.ZHGL_XG}
              permissionDelete={this.buttonRole.ZHGL_SC}
            />
          )
        }
      }
      ],
      table_data: [],
      tenantState: [
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
    if (this._filterButton(this.buttonRole.ZHGL_XG) || this._filterButton(this.buttonRole.ZHGL_SC)) {
      this.columns = [...this.table_column_default, ...this.table_column_action]
    } else {
      this.columns = this.table_column_default
    }
    this.handleChangePage()
  },
  methods: {
    // 分页获取租户列表
    handleChangePage () {
      if (this.loading) return
      this.loading = true
      let pageParams = {
        pageNum: this.current,
        pageSize: this.size,
        searchValue: this.inputValue
      }
      this.swsApi.swsPost('Tenant/tenantlist', pageParams).then(res => {
        this.dataCount = res.data.dataCount
        this.data = res.data.result
        if (res.data.error === null) {
          this.loading = false
          this.table_data = res.data.result
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
            name: this.formValidate.name,
            contactPerson: this.formValidate.contactPerson,
            contactPhone: this.formValidate.contactPhone,
            address: this.formValidate.address,
            isActive: this.formValidate.isActive,
            note: this.formValidate.note
          }
          this.swsApi.swsPost('Tenant/Settenant', addParams).then(res => {
            if (res.data.error === null) {
              this.$Message.success('新增租户成功')
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
    handleCancelAdd () {
      this.addModal = false
    },
    // 确认修改
    handleConfirmModify () {
      this.$refs.mformValidateRef.validate(valid => {
        if (valid) {
          this.modifyModal = false
          let modifyParams = {
            id: this.id,
            name: this.mformValidate.name,
            contactPerson: this.mformValidate.contactPerson,
            contactPhone: this.mformValidate.contactPhone,
            address: this.mformValidate.address,
            isActive: this.mformValidate.isActive,
            note: this.mformValidate.note
          }
          this.swsApi.swsPost('Tenant/Updatetenant', modifyParams).then(res => {
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
      this.swsApi.swsGet(`Tenant/tenant/del/${id}`).then(res => {
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
      this.swsApi.swsPost('Tenant/tenantActive', qiyongParams).then(res => {
        if (res.data.error === null) {
          this.$Message.success('租户启用成功')
          this.idAr = []
          this.handleChangePage()
        } else {
          this.$Message.error('租户启用失败，请稍后重试')
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
      this.swsApi.swsPost('Tenant/tenantActive', jinyongParams).then(res => {
        if (res.data.error === null) {
          this.$Message.success('租户禁用成功')
          this.idAr = []
          this.handleChangePage()
        } else {
          this.$Message.error('租户禁用失败，请稍后重试')
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
.tenant-detail-table {
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
