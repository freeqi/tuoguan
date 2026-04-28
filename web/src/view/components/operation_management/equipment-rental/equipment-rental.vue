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
                v-permission="buttonRole.SBZY_XZ"
              >添加租用申请</Button>
              <Button
                v-permission="buttonRole.SBZY_SQ"
                class="shenqing"
                type="primary"
                ghost
                @click="handleShenqing"
              >
                <i class="iconfont icon-shenqing" style="margin-right:5px;"></i>提交申请
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
    <Row style="margin-top:20px" class="equipment-rental-table">
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
    <!-- 添加租用申请 -->
    <Modal title="添加租用申请" v-model="addModal" class-name="vertical-center-modal">
      <div>
        <Form
          ref="formValidateRef"
          :model="formValidate"
          :rules="ruleValidate"
          :label-width="80"
          autocomplete="off"
          label-position="right"
        >
          <FormItem label="设备ID：" prop="equipmentId">
            <Select
              v-model="formValidate.equipmentId"
              filterable
              label-in-value
              @on-change="handleEquipmentSelect"
              transfer
              placeholder="请选择设备"
            >
              <Option v-for="item in equipmentList" :key="item.id" :value="item.id">{{item.equipmentName}}</Option>
            </Select>
          </FormItem>
          <FormItem label="租户ID：" prop="tenantId">
            <Select
              v-model="formValidate.tenantId"
              filterable
              label-in-value
              @on-change="handleTenantSelect"
              transfer
              placeholder="请选择租户"
            >
              <Option v-for="item in tenantList" :key="item.id" :value="item.id">{{item.name}}</Option>
            </Select>
          </FormItem>
          <FormItem label="租用开始日期：" prop="rentalStartDate">
            <DatePicker type="date" v-model="formValidate.rentalStartDate" placeholder="请选择开始日期"></DatePicker>
          </FormItem>
          <FormItem label="租用结束日期：" prop="rentalEndDate">
            <DatePicker type="date" v-model="formValidate.rentalEndDate" placeholder="请选择结束日期"></DatePicker>
          </FormItem>
          <FormItem label="租用用途：" prop="rentalPurpose">
            <Input type="textarea" v-model="formValidate.rentalPurpose" :rows="3" placeholder="请输入租用用途"></Input>
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
    <!-- 修改租用申请 -->
    <Modal title="修改租用申请" v-model="modifyModal" class-name="vertical-center-modal">
      <div>
        <Form
          ref="mformValidateRef"
          :model="mformValidate"
          :rules="mruleValidate"
          :label-width="80"
          label-position="right"
        >
          <FormItem label="设备ID：" prop="equipmentId">
            <Select v-model="mformValidate.equipmentId" disabled transfer>
              <Option v-for="item in equipmentList" :key="item.id" :value="item.id">{{item.equipmentName}}</Option>
            </Select>
          </FormItem>
          <FormItem label="租户ID：" prop="tenantId">
            <Select v-model="mformValidate.tenantId" disabled transfer>
              <Option v-for="item in tenantList" :key="item.id" :value="item.id">{{item.name}}</Option>
            </Select>
          </FormItem>
          <FormItem label="租用开始日期：" prop="rentalStartDate">
            <DatePicker type="date" v-model="mformValidate.rentalStartDate" placeholder="请选择开始日期"></DatePicker>
          </FormItem>
          <FormItem label="租用结束日期：" prop="rentalEndDate">
            <DatePicker type="date" v-model="mformValidate.rentalEndDate" placeholder="请选择结束日期"></DatePicker>
          </FormItem>
          <FormItem label="租用用途：" prop="rentalPurpose">
            <Input type="textarea" v-model="mformValidate.rentalPurpose" :rows="3" placeholder="请输入租用用途"></Input>
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
    <Modal title="删除租用申请" v-model="deleteModal" class-name="vertical-center-modal">
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
  SBZY_XZ: 'SBZY_XZ',
  SBZY_XG: 'SBZY_XG',
  SBZY_SC: 'SBZY_SC',
  SBZY_SQ: 'SBZY_SQ'
}
export default {
  name: 'equipmentRental',
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
      equipmentList: [],
      tenantList: [],
      equipmentId: '',
      tenantId: '',
      idAr: [],
      formValidate: {
        equipmentId: '',
        tenantId: '',
        rentalStartDate: '',
        rentalEndDate: '',
        rentalPurpose: '',
        note: ''
      },
      mformValidate: {
        equipmentId: '',
        tenantId: '',
        rentalStartDate: '',
        rentalEndDate: '',
        rentalPurpose: '',
        note: ''
      },
      ruleValidate: {
        equipmentId: [
          { required: true, message: '设备不能为空', trigger: 'blur' },
          { message: '设备不能为空', trigger: 'change' }
        ],
        tenantId: [
          { required: true, message: '租户不能为空', trigger: 'blur' },
          { message: '租户不能为空', trigger: 'change' }
        ],
        rentalStartDate: [
          { required: true, message: '租用开始日期不能为空', trigger: 'blur' }
        ],
        rentalEndDate: [
          { required: true, message: '租用结束日期不能为空', trigger: 'blur' }
        ],
        rentalPurpose: [
          { required: true, message: '租用用途不能为空', trigger: 'blur' }
        ]
      },
      mruleValidate: {
        rentalStartDate: [
          { required: true, message: '租用开始日期不能为空', trigger: 'blur' }
        ],
        rentalEndDate: [
          { required: true, message: '租用结束日期不能为空', trigger: 'blur' }
        ],
        rentalPurpose: [
          { required: true, message: '租用用途不能为空', trigger: 'blur' }
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
          title: '设备名称',
          key: 'equipmentName'
        },
        {
          title: '租户名称',
          key: 'tenantName'
        },
        {
          title: '租用开始日期',
          key: 'rentalStartDate',
          render: (h, params) => {
            return <span>{params.row.rentalStartDate ? new Date(params.row.rentalStartDate).toLocaleDateString() : '-'}</span>
          }
        },
        {
          title: '租用结束日期',
          key: 'rentalEndDate',
          render: (h, params) => {
            return <span>{params.row.rentalEndDate ? new Date(params.row.rentalEndDate).toLocaleDateString() : '-'}</span>
          }
        },
        {
          title: '租用状态',
          key: 'rentalStatus',
          width: 100,
          align: 'center',
          render: (h, params) => {
            let _text = ''
            let _color = ''
            switch (params.row.rentalStatus) {
              case 0:
                _text = '待审批'
                _color = 'warning'
                break
              case 1:
                _text = '已批准'
                _color = 'success'
                break
              case 2:
                _text = '已拒绝'
                _color = 'error'
                break
              case 3:
                _text = '已完成'
                _color = 'info'
                break
            }
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
              permissionEdit={this.buttonRole.SBZY_XG}
              permissionDelete={this.buttonRole.SBZY_SC}
            />
          )
        }
      }
      ],
      table_data: [],

      buttonRole: BUTTONROLE
    }
  },
  mounted () {
    if (this._filterButton(this.buttonRole.SBZY_XG) || this._filterButton(this.buttonRole.SBZY_SC)) {
      this.columns = [...this.table_column_default, ...this.table_column_action]
    } else {
      this.columns = this.table_column_default
    }
    this.handleGetEquipmentList()
    this.handleGetTenantList()
  },
  methods: {
    // 分页获取租用申请列表
    handleChangePage () {
      if (this.loading) return
      this.loading = true
      let pageParams = {
        pageNum: this.current,
        pageSize: this.size,
        searchValue: this.inputValue
      }
      this.swsApi.swsPost('EquipmentRental/equipmentrentalist', pageParams).then(res => {
        this.dataCount = res.data.dataCount
        this.data = res.data.result
        if (res.data.error === null) {
          this.loading = false
          this.table_data = res.data.result
        }
      })
    },
    // 获取设备列表
    handleGetEquipmentList () {
      this.swsApi.swsPost('Equipment/EquipmentList').then(res => {
        if (res.data.success) {
          this.equipmentList = res.data.result
          this.handleChangePage()
        }
      })
    },
    // 获取租户列表
    handleGetTenantList () {
      this.swsApi.swsPost('Tenant/tenantlist').then(res => {
        if (res.data.success) {
          this.tenantList = res.data.result
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
            equipmentId: this.equipmentId,
            tenantId: this.tenantId,
            rentalStartDate: this.formValidate.rentalStartDate,
            rentalEndDate: this.formValidate.rentalEndDate,
            rentalPurpose: this.formValidate.rentalPurpose,
            note: this.formValidate.note
          }
          this.swsApi.swsPost('EquipmentRental/Setequipmentrental', addParams).then(res => {
            if (res.data.error === null) {
              this.$Message.success('新增租用申请成功')
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
    // 下拉框选取设备
    handleEquipmentSelect (val) {
      if (val) {
        this.equipmentId = val.value
      }
    },
    // 下拉框选取租户
    handleTenantSelect (val) {
      if (val) {
        this.tenantId = val.value
      }
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
            rentalStartDate: this.mformValidate.rentalStartDate,
            rentalEndDate: this.mformValidate.rentalEndDate,
            rentalPurpose: this.mformValidate.rentalPurpose,
            note: this.mformValidate.note
          }
          this.swsApi.swsPost('EquipmentRental/Updateequipmentrental', modifyParams).then(res => {
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
      this.swsApi.swsGet(`EquipmentRental/equipmentrental/del/${id}`).then(res => {
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
    // 提交申请
    handleShenqing () {
      if (this.idAr.length === 0) return false
      let shenqingParams = {
        id: this.idAr
      }
      this.swsApi.swsPost('EquipmentRental/SubmitEquipmentRental', shenqingParams).then(res => {
        if (res.data.error === null) {
          this.$Message.success('提交申请成功')
          this.idAr = []
          this.handleChangePage()
        } else {
          this.$Message.error('提交申请失败，请稍后重试')
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
.equipment-rental-table {
  .ivu-table-wrapper {
    & /deep/ .ivu-table {
      font-size: 13px;
    }
  }
}
.search {
  float: right;
}
.shenqing {
  margin-left: 20px;
  font-size: 12px;
}
</style>
