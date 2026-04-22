<template>
  <div id="staff_transfer">
    <div class="button-group">
      <span>职工姓名</span>
      <Select filterable @on-change="empChange"
        placeholder="职工姓名" style="width: 160px;margin:0 10px;">
        <Option :value="item.id" :key="item.id" v-for="item in employeList.filter(res=> res.strWorkingState!=='离职')">{{item.name}}</Option>
      </Select>
      <Button type="primary" @click="transferBtn('transferFormRef')">预调动</Button>
      <Button @click="handleReset('transferFormRef')">清除</Button>

      <span style="margin-left: 30px">预调度记录查询</span>
      <Input v-model="keySearch"
        search enter-button
        @on-search="gettransferRecord"
        placeholder="请输入姓名"
        style="width: 170px;display: inline-table;"/>
    </div>
    <div class="staff_transfer_form">
      <Form ref="transferFormRef" :rules="transferFormRule"
        :model="transferForm" label-position="left" :label-width="95">
        <Row class="form" :gutter="10">
          <Col :lg="5" :md="8" :sm="8">
            <FormItem label="原部门">
              <Select v-model="transferFormPre.originalDep" disabled placeholder="原部门">
                <Option
                  :value="item.id"
                  v-for="item in sDepartment"
                  :key="item.id"
                >{{item.name}}</Option>
              </Select>
            </FormItem>
          </Col>
          <Col :lg="5" :md="8" :sm="8">
            <FormItem label="原职位">
              <Select v-model="transferFormPre.originalPosition" disabled placeholder="请选择原职位">
                <Option :value="'none'">无</Option>
                <Option
                  :value="item.id"
                  v-for="item in sPosition"
                  :key="item.id"
                >{{item.name}}</Option>
              </Select>
            </FormItem>
          </Col>
          <Col :lg="5" :md="8" :sm="8">
            <FormItem label="原机构">
              <Select v-model="transferFormPre.originalCenter" disabled placeholder="原机构">
                <Option :value="'none'">无</Option>
                <Option v-for="item in hospitalList" :value="item.id" :key="item.id">{{item.dialysisName}}</Option>
              </Select>
            </FormItem>
          </Col>
        </Row>
        <Row class="form" :gutter="10">
          <Col :lg="5" :md="8" :sm="8">
            <FormItem prop="presentDepId" label="预调部门">
              <Select v-model="transferForm.presentDepId" filterable placeholder="请选择预调部门">
                <Option
                  :value="item.id"
                  v-for="item in sDepartment"
                  :key="item.id"
                >{{item.name}}</Option>
              </Select>
            </FormItem>
          </Col>
          <Col :lg="5" :md="8" :sm="8">
            <FormItem prop="presentPositionId" label="预调职位">
              <Select v-model="transferForm.presentPositionId" filterable placeholder="请选择预调职位">
                <Option
                  :value="item.id"
                  v-for="item in sPosition"
                  :key="item.id"
                >{{item.name}}</Option>
              </Select>
            </FormItem>
          </Col>
          <Col :lg="5" :md="8" :sm="8">
            <FormItem prop="presentCenterId" label="预调机构">
              <Select v-model="transferForm.presentCenterId" filterable placeholder="请选择预调机构">
                <Option v-for="item in hospitalList" :value="item.id" :key="item.id">{{item.dialysisName}}</Option>
              </Select>
            </FormItem>
          </Col>
        </Row>
        <Row class="form" :gutter="10">
          <Col :lg="5" :md="8" :sm="8">
            <FormItem prop="transferDate" label="预调时间">
              <DatePicker
                v-model="transferForm.transferDate"
                type="date"
                :options="dateLimit"
                placement="bottom-end"
                placeholder="请选择预调时间"
              ></DatePicker>
            </FormItem>
          </Col>
          <Col :lg="5" :md="8" :sm="8">
            <FormItem prop="backDate" label="预调回时间">
              <DatePicker
                v-model="transferForm.backDate"
                type="date"
                :options="dateLimit"
                placement="bottom-end"
                placeholder="请选择预调回时间"
              ></DatePicker>
            </FormItem>
          </Col>
        </Row>
      </Form>
    </div>
    <Divider></Divider>
    <!-- <div class="button-group">
      <span>查询</span>
      <Input v-model="keySearch"
        search enter-button
        @on-search="gettransferRecord"
        placeholder="请输入姓名"
        style="width: 170px;display: inline-table;"/>
    </div> -->
    <div class="table_detail">
      <Table
        :data="table_data"
        :columns="table_columns_default"
        :loading="table_laoding">
      </Table>
      <div class="pagination" v-if="dataCount > pageSize">
        <Page
          :total="dataCount"
          :page-size="pageSize"
          :current.sync="pageNum"
          @on-change="gettransferRecord"
        />
      </div>
    </div>
    <!-- 删除/撤销 -->
    <Modal :title="title" v-model="delModal" class-name="vertical-center-modal">
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;"/>{{this.title}}后不可恢复，您确定{{this.title}}吗？
        </p>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="handleConfirmDelete">确定</Button>
        <Button class="cancelDelete" type="default" @click="delModal=false">取消</Button>
      </div>
    </Modal>
    <!-- 修改  -->
    <Modal :title="editTitle" v-model="editModal" class-name="vertical-center-modal" width="400">
      <Form ref="editModalRef"
        :rules="editModalFormRule"
        :model="editModalForm"
        :label-width="90">
        <FormItem prop="presentDepId" label="预调部门">
          <Select v-model="editModalForm.presentDepId" filterable placeholder="请选择预调部门">
            <Option
              :value="item.id"
              v-for="item in sDepartment"
              :key="item.id"
            >{{item.name}}</Option>
          </Select>
        </FormItem>
            <FormItem prop="presentPositionId" label="预调职位">
              <Select v-model="editModalForm.presentPositionId" filterable placeholder="请选择预调职位">
                <Option
                  :value="item.id"
                  v-for="item in sPosition"
                  :key="item.id"
                >{{item.name}}</Option>
              </Select>
            </FormItem>
            <FormItem prop="presentCenterId" label="预调机构">
              <Select v-model="editModalForm.presentCenterId" filterable placeholder="请选择预调机构">
                <Option v-for="item in hospitalList" :value="item.id" :key="item.id">{{item.dialysisName}}</Option>
              </Select>
            </FormItem>
            <FormItem prop="transferDate" label="预调时间">
              <DatePicker
                v-model="editModalForm.transferDate"
                format="yyyy-MM-dd"
                type="date"
                :options="dateLimit"
                placement="bottom-end"
                @on-change="dateChange"
                placeholder="请选择预调时间"
              ></DatePicker>
            </FormItem>
            <FormItem prop="backDate" label="预调时间">
              <DatePicker
                v-model="editModalForm.backDate"
                format="yyyy-MM-dd"
                type="date"
                :options="dateLimit"
                placement="bottom-end"
                @on-change="dateChange"
                placeholder="请选择预调时间"
              ></DatePicker>
            </FormItem>
      </Form>
      <div slot="footer">
        <Button type="primary" @click="transferBtn('editModalRef')" :loading="handleEditBtn">修改</Button>
        <Button type="default" @click="editModal=false">取消</Button>
      </div>
    </Modal>

  </div>
</template>

<script>
import Operate from '@/components/operate'
const [DEPARTMENT, POSITION] = [
  2,
  'cb30fac65abb48f1ba5b276d1fa76730'
]
const BUTTONROLE = {
  ZGDD_UNDO: 'ZGDD_UNDO',
  ZGDD_DEL: 'ZGDD_DEL',
  ZGDD_EDIT: 'ZGDD_EDIT'
}
export default {
  data () {
    return {
      title: '',
      delModal: false,
      buttonRole: BUTTONROLE,
      tableRowIndex: -1,

      keySearch: '',
      table_data: [],
      table_columns: [],
      table_columns_default: [
        {
          title: '姓名',
          key: 'employeeName',
          width: 70
        },
        {
          title: '原部门',
          key: 'originalDep',
          width: 70
        },
        {
          title: '原职位',
          key: 'originalPosition',
          width: 70
        },
        {
          title: '原机构',
          key: 'originalCenter',
          minWidth: 120
        },
        {
          title: '预调部门',
          width: 70,
          key: 'presentDep'
        },
        {
          title: '预调职位',
          key: 'presentPosition',
          minWidth: 65
        },
        {
          title: '预调机构',
          key: 'presentCenter',
          minWidth: 120
        },
        {
          title: '预调时间',
          key: 'transferDate',
          align: 'center',
          width: 85,
          render: (h, params) => {
            return <span>{params.row.transferDate && params.row.transferDate.slice(0, 10)}</span>
          }
        },
        {
          title: '预调回时间',
          key: 'backDate',
          align: 'center',
          width: 85,
          render: (h, params) => {
            return <span>{params.row.backDate && params.row.backDate.slice(0, 10)}</span>
          }
        },
        {
          title: '是否已执行调度',
          key: 'isTransfer',
          align: 'center',
          width: 90,
          render: (h, params) => {
            return params.row.isTransfer ? <span>是</span> : <span>否</span>
          }
        },
        {
          title: '调动状态',
          key: 'dataState',
          align: 'center',
          width: 85,
          render: (h, params) => {
            if (params.row.dataState === 1) {
              return <i-button type="primary" size="small">{this.workState[params.row.dataState]}</i-button>
            } else if (params.row.dataState === 2) {
              return <i-button type="info" size="small">{this.workState[params.row.dataState]}</i-button>
            } else if (params.row.dataState === 3) {
              return <i-button type="error" size="small">{this.workState[params.row.dataState]}</i-button>
            }
          }
        },
        {
          title: '操作',
          key: 'action',
          width: 130,
          align: 'center',
          render: (h, params) => {
            if (!params.row.isTransfer) {
              return (
                <Operate
                  showWatch={false}
                  // showEdit={false}
                  handleUndo={() => {
                    this.delModal = true
                    this.tableRowIndex = params.row.id
                    this.transferStatus = 2
                    this.title = '撤销'
                  }}
                  handleEdit={() => this.handleEdit(params.row)}
                  handleDelete={() => {
                    this.delModal = true
                    this.tableRowIndex = params.row.id
                    this.transferStatus = 3
                    this.title = '删除'
                  }}
                  permissionUndo={this.buttonRole.ZGDD_UNDO}
                  permissionDelete={this.buttonRole.ZGDD_DEL}
                  permissionEdit={this.buttonRole.ZGDD_EDIT}
                />
              )
            } else {
              return <span></span>
            }
          }
        }
      ],
      workState: ['', '有效', '撤回', '删除'],
      table_laoding: false,
      dataCount: 0,
      pageSize: 10,
      pageNum: 1,

      empInfo: {
        name: '',
        id: ''
      },
      transferFormPre: {
        originalDep: '',
        originalPosition: '',
        originalCenter: ''
      },
      transferForm: {
        presentDepId: '',
        presentPositionId: '',
        presentCenterId: '',
        transferDate: new Date(),
        backDate: new Date()
      },
      transferFormRule: {
        presentDepId: [
          { required: true, message: '部门不能为空', trigger: 'blur' }],
        presentPositionId: [
          { required: true, message: '职务不能为空', trigger: 'blur' }],
        presentCenterId: [
          { required: true, message: '机构不能为空', trigger: 'blur' }],
        transferDate: [
          { required: true, type: 'date', message: '时间不能为空', trigger: 'blur' }],
        backDate: [
          { required: true, type: 'date', message: '时间不能为空', trigger: 'blur' }]
      },
      editTitle: '',
      editModal: false,
      handleEditBtn: false,
      editModalForm: {
        empId: '',
        presentDepId: '',
        presentPositionId: '',
        presentCenterId: '',
        transferDate: new Date(),
        backDate: new Date()
      },
      editModalFormRule: {
        presentDepId: [
          { required: true, message: '部门不能为空', trigger: 'blur' }],
        presentPositionId: [
          { required: true, message: '职务不能为空', trigger: 'blur' }],
        presentCenterId: [
          { required: true, message: '机构不能为空', trigger: 'blur' }],
        transferDate: [
          { required: true, type: 'date', message: '时间不能为空', trigger: 'blur' }]
      },
      // 数据字典
      sDepartment: [],
      sPosition: [],

      hospitalList: [],
      employeList: [],
      dateLimit: {
        disabledDate (date) {
          return date && date.valueOf() < Date.now() - 86400000
        }
      }
    }
  },
  components: {Operate},
  created () {
    this.gettransferRecord()
    let types = [DEPARTMENT, POSITION]
    types = types.map(id => {
      let o = {
        url: 'SystemDictionary/DictionaryList',
        params: { typeId: `${id}` }
      }
      return o
    })
    types.push({url: 'CenterDialysis/DialysisList'}, {url: 'Employee/Employee'})
    this.swsApi
      .swsAllPost(types)
      .then(res => {
        this.sDepartment = res[0].data.result
        this.sPosition = res[1].data.result
        this.hospitalList = res[2].data.result
        this.employeList = res[3].data.result
      })
      .catch(e => {
        console.log(e)
      })
  },
  methods: {
    dateChange (date) {
      console.log(date, this.transferForm.transferDate)
    },
    handleEdit (row) {
      this.title = '修改'
      this.editTitle = `修改 ${row.employeeName}预调记录`
      let {id, empId, presentDepId, presentPositionId, presentCenterId, transferDate, backDate} = { ...row }
      this.editModalForm = {id, empId, presentDepId, presentPositionId, presentCenterId, transferDate, backDate}
      this.editModal = true
    },
    // 确认删除
    handleConfirmDelete () {
      this.delModal = false
      this.swsApi.swsGet(`Employee/Employee/UpdatePerState/${this.tableRowIndex}/${this.transferStatus}`).then(res => {
        if (res.data.error === null) {
          this.$Message.success(`${this.title}成功！`)
          this.pageNum = 1
          this.gettransferRecord()
        }
      })
    },
    empChange (id) {
      let { positionId, depId, centerDialysisId } = this.employeList.find(res => res.id === id)
      this.empInfo.id = id || ''
      this.transferFormPre.originalDep = depId || ''
      this.transferFormPre.originalPosition = positionId || 'none'
      this.transferFormPre.originalCenter = centerDialysisId || 'none'
    },
    transferBtn (refName) {
      if (!this.empInfo.id && !this.editModal) {
        this.$Message.info('请选择预调度职工！')
      } else {
        this.$refs[refName].validate(valid => {
          if (valid) {
            let params = !this.editModal
              ? {...this.transferForm, empId: this.empInfo.id}
              : {...this.editModalForm}
            this.handleEditBtn = this.editModal
            // +8hours
            params.transferDate = new Date(params.transferDate.setHours(params.transferDate.getHours() + 8))
            params.backDate = new Date(params.backDate.setHours(params.backDate.getHours() + 8))
            this.swsApi.swsPost('Employee/Employee/AddUpdatePer', params)
              .then(res => {
                if (res.data.success) {
                  this.$Message.success(`预调度${this.title}操作成功！`)
                  this.gettransferRecord()
                } else {
                  this.$Message.error('操作失败', res.data.error)
                }
              })
              .catch(e => {
                this.$Message.error(`${e}，请稍后再试！`)
              })
            if (this.editModal) {
              this.editModal = false
              this.handleEditBtn = false
            }
          }
        })
      }
    },
    gettransferRecord () {
      this.table_laoding = true
      let params = {
        empName: this.keySearch,
        pageSize: this.pageSize,
        pageNum: this.pageNum
      }
      this.swsApi.swsPost('Employee/Employee/PerTransfer', params)
        .then(res => {
          if (res.data.success) {
            this.table_data = res.data.result
            this.dataCount = res.data.dataCount
          } else {
            this.$Message.error(`请求预调度记录失败，${e}，`)
          }
          this.table_laoding = false
        })
        .catch(e => {
          this.table_laoding = false
          this.$Message.error(`${e}，请稍后再试！`)
        })
    },
    handleReset (name) {
      this.$refs[name].resetFields()
    }
  }
}
</script>

<style scoped lang="less">
#staff_transfer {
  padding: 20px 20px 0;
  /* padding: 0 0 10px; */
  height: 100%;
  background: #ffffff;
  .table_detail{
    position: relative;
    padding: 10px 0;
  }
  .button-group {
    margin-bottom: 20px;
    text-align: left;
    button + button {
      margin-left: 10px;
    }
    span {
      margin-right: 25px;
      display: inline-block;
      text-align: right;
    }
  }
  // .form {
  //   width: 80%;
  // }
  .ivu-form-item {
    margin-bottom: 10;
  }
  .ivu-divider-horizontal {
    margin-top: 0;
  }
}
</style>
