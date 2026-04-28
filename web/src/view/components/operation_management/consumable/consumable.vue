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
                v-permission="buttonRole.HCGK_XZ"
              >添加耗材</Button>
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
    <Row style="margin-top:20px" class="consumable-table">
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
    <!-- 添加耗材 -->
    <Modal title="添加耗材" v-model="addModal" class-name="vertical-center-modal">
      <div>
        <Form
          ref="formValidateRef"
          :model="formValidate"
          :rules="ruleValidate"
          :label-width="80"
          autocomplete="off"
          label-position="right"
        >
          <FormItem label="耗材名称：" prop="name">
            <Input type="text" v-model="formValidate.name" placeholder="请输入耗材名称"></Input>
          </FormItem>
          <FormItem label="规格：" prop="specifications">
            <Input type="text" v-model="formValidate.specifications" placeholder="请输入规格"></Input>
          </FormItem>
          <FormItem label="型号：" prop="model">
            <Input type="text" v-model="formValidate.model" placeholder="请输入型号"></Input>
          </FormItem>
          <FormItem label="生产厂家：" prop="manufacturer">
            <Input type="text" v-model="formValidate.manufacturer" placeholder="请输入生产厂家"></Input>
          </FormItem>
          <FormItem label="类型：" prop="type">
            <Select v-model="formValidate.type" placeholder="请选择类型">
              <Option value="1">低值耗材</Option>
              <Option value="2">高值耗材</Option>
              <Option value="3">试剂</Option>
            </Select>
          </FormItem>
          <FormItem label="单位：" prop="unit">
            <Input type="text" v-model="formValidate.unit" placeholder="请输入单位"></Input>
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
    <!-- 修改耗材 -->
    <Modal title="修改耗材" v-model="modifyModal" class-name="vertical-center-modal">
      <div>
        <Form
          ref="mformValidateRef"
          :model="mformValidate"
          :rules="mruleValidate"
          :label-width="80"
          label-position="right"
        >
          <FormItem label="耗材名称：" prop="name">
            <Input type="text" v-model="mformValidate.name" placeholder="请输入耗材名称"></Input>
          </FormItem>
          <FormItem label="规格：" prop="specifications">
            <Input type="text" v-model="mformValidate.specifications" placeholder="请输入规格"></Input>
          </FormItem>
          <FormItem label="型号：" prop="model">
            <Input type="text" v-model="mformValidate.model" placeholder="请输入型号"></Input>
          </FormItem>
          <FormItem label="生产厂家：" prop="manufacturer">
            <Input type="text" v-model="mformValidate.manufacturer" placeholder="请输入生产厂家"></Input>
          </FormItem>
          <FormItem label="类型：" prop="type">
            <Select v-model="mformValidate.type" placeholder="请选择类型">
              <Option value="1">低值耗材</Option>
              <Option value="2">高值耗材</Option>
              <Option value="3">试剂</Option>
            </Select>
          </FormItem>
          <FormItem label="单位：" prop="unit">
            <Input type="text" v-model="mformValidate.unit" placeholder="请输入单位"></Input>
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
    <Modal title="删除耗材" v-model="deleteModal" class-name="vertical-center-modal">
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
  HCGK_XZ: 'HCGK_XZ',
  HCGK_XG: 'HCGK_XG',
  HCGK_SC: 'HCGK_SC'
}
export default {
  name: 'consumable',
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
        specifications: '',
        model: '',
        manufacturer: '',
        type: '',
        unit: '',
        note: ''
      },
      mformValidate: {
        name: '',
        specifications: '',
        model: '',
        manufacturer: '',
        type: '',
        unit: '',
        note: ''
      },
      ruleValidate: {
        name: [
          { required: true, message: '耗材名称不能为空', trigger: 'blur' }
        ],
        specifications: [
          { required: true, message: '规格不能为空', trigger: 'blur' }
        ],
        model: [
          { required: true, message: '型号不能为空', trigger: 'blur' }
        ],
        manufacturer: [
          { required: true, message: '生产厂家不能为空', trigger: 'blur' }
        ],
        type: [
          { required: true, message: '类型不能为空', trigger: 'blur' }
        ],
        unit: [
          { required: true, message: '单位不能为空', trigger: 'blur' }
        ]
      },
      mruleValidate: {
        name: [
          { required: true, message: '耗材名称不能为空', trigger: 'blur' }
        ],
        specifications: [
          { required: true, message: '规格不能为空', trigger: 'blur' }
        ],
        model: [
          { required: true, message: '型号不能为空', trigger: 'blur' }
        ],
        manufacturer: [
          { required: true, message: '生产厂家不能为空', trigger: 'blur' }
        ],
        type: [
          { required: true, message: '类型不能为空', trigger: 'blur' }
        ],
        unit: [
          { required: true, message: '单位不能为空', trigger: 'blur' }
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
          title: '耗材名称',
          key: 'name'
        },
        {
          title: '规格',
          key: 'specifications'
        },
        {
          title: '型号',
          key: 'model'
        },
        {
          title: '生产厂家',
          key: 'manufacturer'
        },
        {
          title: '类型',
          key: 'type',
          render: (h, params) => {
            let _text = ''
            switch (params.row.type) {
              case 1:
                _text = '低值耗材'
                break
              case 2:
                _text = '高值耗材'
                break
              case 3:
                _text = '试剂'
                break
            }
            return <span>{_text}</span>
          }
        },
        {
          title: '单位',
          key: 'unit'
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
              permissionEdit={this.buttonRole.HCGK_XG}
              permissionDelete={this.buttonRole.HCGK_SC}
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
    if (this._filterButton(this.buttonRole.HCGK_XG) || this._filterButton(this.buttonRole.HCGK_SC)) {
      this.columns = [...this.table_column_default, ...this.table_column_action]
    } else {
      this.columns = this.table_column_default
    }
    this.handleChangePage()
  },
  methods: {
    // 分页获取耗材列表
    handleChangePage () {
      if (this.loading) return
      this.loading = true
      let pageParams = {
        pageNum: this.current,
        pageSize: this.size,
        searchValue: this.inputValue
      }
      this.swsApi.swsPost('Consumable/consumablelist', pageParams).then(res => {
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
            specifications: this.formValidate.specifications,
            model: this.formValidate.model,
            manufacturer: this.formValidate.manufacturer,
            type: this.formValidate.type,
            unit: this.formValidate.unit,
            note: this.formValidate.note
          }
          this.swsApi.swsPost('Consumable/Setconsumable', addParams).then(res => {
            if (res.data.error === null) {
              this.$Message.success('新增耗材成功')
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
            specifications: this.mformValidate.specifications,
            model: this.mformValidate.model,
            manufacturer: this.mformValidate.manufacturer,
            type: this.mformValidate.type,
            unit: this.mformValidate.unit,
            note: this.mformValidate.note
          }
          this.swsApi.swsPost('Consumable/Updateconsumable', modifyParams).then(res => {
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
      this.swsApi.swsGet(`Consumable/consumable/del/${id}`).then(res => {
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
    }
  },
  components: {
    Operate
  }
}
</script>

<style scoped lang='less'>
.consumable-table {
  .ivu-table-wrapper {
    & /deep/ .ivu-table {
      font-size: 13px;
    }
  }
}
.search {
  float: right;
}
</style>
