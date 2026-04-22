<template>
  <div id="medical">
    <div class="content">
      <!-- top -->
      <div class="header">
        <div class="type-change">
          <RadioGroup v-model="typeChecked" type="button" @on-change="handleOnOperChange">
            <Radio label="单位"></Radio>
            <Radio label="用法用量"></Radio>
          </RadioGroup>
          <div class="inline-block">
            <Button
              type="primary"
              v-if="typeChecked==='单位'"
              v-permission="buttonRole.DWML_XZDW"
              @click="showUnitOperate('添加')"
            >新增单位</Button>
            <Button
              type="primary"
              v-if="typeChecked==='用法用量'"
              v-permission="buttonRole.DWML_XZYF"
              @click="showUseOperate('添加')"
            >新增用法用量</Button>
          </div>
        </div>

        <div class="filter">
          <div class="inline-block" v-if="typeChecked=='单位'">
            <!-- <Form>
              <FormItem label="单位类型"> -->
                <span>单位类型</span>
                <Select
                  v-model="selectedUintType"
                  @on-change="changeItemType"
                  placeholder="请选择单位类型"
                  style="width: 200px;margin: 0 10px;"
                >
                  <Option value="0">全部</Option>
                  <Option :value="item.id" v-for="item in UnitType" :key="item.id">{{item.name}}</Option>
                </Select>
              <!-- </FormItem>
            </Form> -->
          </div>
          <div class="inline-block" v-else>
            <!-- <Form :label-width="60"> -->
              <!-- <FormItem label="用法类型"> -->
                <span>用法类型</span>
                <Select
                  v-model="selectedWayType"
                  @on-change="changeItemType"
                  placeholder="请选择类型"
                  style="width: 200px;margin: 0 10px;"
                >
                  <Option value="0">全部</Option>
                  <Option :value="item.id" v-for="item in wayType" :key="item.id">{{item.name}}</Option>
                </Select>
              <!-- </FormItem>
            </Form> -->
          </div>
          <!-- <div> -->
            <Input
              v-model="searchKey"
              search
              enter-button
              @on-search="search"
              placeholder="请输入关键词"
              style="width: 220px;"
            />
          <!-- </div> -->
        </div>
      </div>
      <!-- 单位列表 -->
      <div class="table">
        <Table
          :loading="loading"
          style="margin-top: 20px;"
          no-filtered-data-text="暂无查到所需数据"
          size="large"
          :columns="table_column"
          :data="table_data"
        ></Table>
        <div style="margin: 10px;overflow: hidden" v-if="dataCount > 10">
          <div style="float: right;">
            <Page
              :total="dataCount"
              :page-size="pageSize"
              :current.sync="startPage"
              @on-change="changePage"
            ></Page>
          </div>
        </div>
      </div>
    </div>

    <Modal v-model="delModal" width="400" class-name="vertical-center-modal">
      <p slot="header">
        <span>删除单位</span>
      </p>
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;"/>删除后不可恢复，您确定删除吗？
        </p>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="del()">确定</Button>
        <Button type="default" @click="delModal=false">取消</Button>
      </div>
    </Modal>
    <Modal
      v-model="operateUnitModel"
      width="400"
      class-name="vertical-center-modal"
      :mask-closable="false"
    >
      <p slot="header">
        <span>{{operateUnitFlage}}单位</span>
      </p>
      <Form ref="unit" :model="unitInfo" :rules="unitValidate" :label-width="100">
        <Row class="form">
          <Col :lg="24">
            <FormItem label="单位类型" prop="unitType">
              <Select v-model="unitInfo.unitType" placeholder="请选择单位类型">
                <Option v-for="item in UnitType" :value="item.id" :key="item.id">{{ item.name }}</Option>
              </Select>
            </FormItem>
          </Col>
          <Col :lg="24">
            <FormItem label="代码值" prop="unitCode">
              <Input type="text" v-model="unitInfo.unitCode" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="24">
            <FormItem label="中文单位" prop="speUnitCHS">
              <Input type="text" v-model="unitInfo.speUnitCHS" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="24">
            <FormItem label="英文单位" prop="speUnitUS">
              <Input type="text" v-model="unitInfo.speUnitUS" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="24">
            <FormItem label="排序" prop="sortNum">
              <InputNumber placeholder="请输入" style="width:100%" v-model="unitInfo.sortNum"></InputNumber>
            </FormItem>
          </Col>
        </Row>
      </Form>
      <div slot="footer" style="text-align: center">
        <Button type="primary" v-if="operateUnitFlage=='添加'" @click="submitUnit('添加')">确定</Button>
        <Button type="primary" v-else @click="submitUnit('修改')">保存</Button>
        <Button type="default" @click="handleReset">取消</Button>
      </div>
    </Modal>
    <Modal
      v-model="operateUseModel"
      width="400"
      class-name="vertical-center-modal"
      :mask-closable="false"
    >
      <p slot="header">
        <span>{{operateUseFlage}}用法用量</span>
      </p>
      <Form ref="use" :model="useInfo" :rules="useValidate" :label-width="100">
        <Row class="form">
          <Col :lg="24">
            <FormItem label="代码值" prop="wayCode">
              <Input type="text" v-model="useInfo.wayCode" placeholder="请输入代码值"></Input>
            </FormItem>
          </Col>
          <Col :lg="24">
            <FormItem label="类别" prop="wayType">
              <Select v-model="useInfo.wayType" placeholder="请选择用法用量类型">
                <Option v-for="item in UseType" :value="item.id" :key="item.id">{{ item.name }}</Option>
              </Select>
            </FormItem>
          </Col>
          <Col :lg="24">
            <FormItem label="描述" prop="WayDescribe">
              <Input type="text" v-model="useInfo.WayDescribe" placeholder="请输入"></Input>
            </FormItem>
          </Col>
        </Row>
      </Form>
      <div slot="footer" style="text-align: center">
        <Button type="primary" v-if="operateUnitFlage=='添加'" @click="submitUse('添加')">确定</Button>
        <Button type="primary" v-else @click="submitUse('修改')">保存</Button>
        <Button type="default" @click="handleReset">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import Operate from '@/components/operate'
const BUTTONROLE = {
  DWML_XZDW: 'DWML_XZDW',
  DWML_XZYF: 'DWML_XZYF',
  DWML_XG: 'DWML_XG',
  DWML_XGZT: 'DWML_XGZT'
}
export default {
  data () {
    return {
      loading: false,
      // cy:selectedUintType
      selectedUintType: 0,
      selectedWayType: 0,
      UnitType: [
        {
          id: 1,
          name: '制剂单位'
        },
        {
          id: 2,
          name: '包装单位'
        },
        {
          id: 3,
          name: '剂量单位'
        },
        {
          id: 4,
          name: '其他通用单位'
        }
      ],
      wayType: [
        {
          id: 1,
          name: '用药途径'
        },
        {
          id: 2,
          name: '使用频率'
        }
      ],
      pageSize: 10,
      startPage: 1,
      searchKey: '',
      dataCount: 0,

      table_column: [],
      table_data: [],
      // 单位
      unit_data: [],
      table_column_unit: [
        {
          title: '代码值',
          key: 'unitCode'
        },
        {
          title: '中文单位',
          key: 'speUnitCHS',
          align: 'center'
        },
        {
          title: '英文单位',
          key: 'speUnitUS',
          align: 'center'
        },
        {
          title: '单位类型',
          key: 'unitType',
          align: 'center',
          render: (h, params) => {
            let text = this.UnitType[params.row.unitType - 1].name
            return <span>{text}</span>
          }
        },
        {
          title: '数据状态',
          key: 'dataState',
          width: 120,
          align: 'center',
          render: (h, params) => {
            let flage = params.row.dataState === 1
            return (
              <i-switch
                size="large"
                value={flage}
                disabled={!this.buttonRole.DWML_XGZT}
                onOn-change={() => {
                  this.changeState(params.row)
                }}
              >
                <span slot="open">可用</span>
                <span slot="close">禁用</span>
              </i-switch>
            )
          }
        },
        {
          title: '操作',
          key: 'action',
          width: 120,
          align: 'center',
          render: (h, params) => {
            return (
              <Operate
                permissionEdit={this.buttonRole.DWML_XG}
                showWatch={false}
                showDelete={false}
                handleEdit={() => {
                  this.showEdit(params.row)
                }}
              />
            )
          }
        }
      ],
      unitInfo: {
        unitType: '',
        unitCode: '',
        speUnitCHS: '',
        speUnitUS: '',
        sortNum: 1
      },
      unitValidate: {
        unitType: [
          {
            required: true,
            type: 'number',
            message: '请选择单位类型',
            trigger: 'blur'
          },
          {
            type: 'number',
            message: '请选择单位类型',
            trigger: 'change'
          }
        ],
        unitCode: [
          {
            required: true,
            type: 'string',
            message: '请输入代码值',
            trigger: 'blur'
          }
        ],
        speUnitCHS: [
          {
            required: true,
            type: 'string',
            message: '请输入中文单位',
            trigger: 'blur'
          }
        ]
      },

      delModal: false,
      unitId: -1,
      operateUnitModel: false,
      operateUnitFlage: '添加',

      typeChecked: '单位',
      // 用法用量
      operateUseModel: false,
      operateUseFlage: '添加',
      UseType: [
        {
          id: 1,
          name: '用药途径'
        },
        {
          id: 2,
          name: '使用频次'
        }
      ],
      table_column_use: [
        {
          title: '代码值',
          key: 'wayCode'
        },
        {
          title: '描述',
          key: 'wayDescribe'
        },
        {
          title: '类别',
          key: 'wayType',
          align: 'center',
          render: (h, params) => {
            let text = this.UseType[params.row.wayType - 1].name
            return <span>{text}</span>
          }
        },
        {
          title: '数据状态',
          key: 'dataState',
          width: 120,
          align: 'center',
          render: (h, params) => {
            let flage = params.row.dataState === 1
            return (
              <i-switch
                size="large"
                disabled={!this.buttonRole.DWML_XGZT}
                value={flage}
                onOn-change={() => {
                  this.changeState(params.row)
                }}
              >
                <span slot="open">可用</span>
                <span slot="close">禁用</span>
              </i-switch>
            )
          }
        },
        {
          title: '操作',
          key: 'action',
          width: 120,
          align: 'center',
          render: (h, params) => {
            return (
              <Operate
                showWatch={false}
                showDelete={false}
                permissionEdit={this.buttonRole.DWML_XG}
                handleEdit={() => {
                  this.showUseEdit(params.row)
                }}
              />
            )
          }
        }
      ],
      useInfo: {
        wayCode: '',
        wayDescribe: '',
        wayType: 0
      },
      useValidate: {
        wayType: [
          {
            required: true,
            type: 'number',
            message: '请选择单位类型',
            trigger: 'blur'
          },
          {
            type: 'number',
            message: '请选择单位类型',
            trigger: 'change'
          }
        ],
        wayCode: [
          {
            required: true,
            type: 'string',
            message: '请输入代码值',
            trigger: 'blur'
          }
        ],
        wayDescribe: [
          {
            required: true,
            type: 'string',
            message: '字段单位',
            trigger: 'blur'
          }
        ]
      },

      buttonRole: BUTTONROLE
    }
  },
  props: {},
  mounted () {
    this.$nextTick(() => {
      this.changeType(this.typeChecked)
    })
    this.startPage = 1
    // this.getUnitList(this.startPage)
    this.getList()
  },
  computed: {},
  methods: {
    // cy改变查询类型
    changeItemType () {
      // 清空前搜索记录
      this.searchKey = ''
      // 修复切换查询类型时未重置页数造成搜索无数据的问题
      this.startPage = 1
      this.getList()
    },
    // cy：直接合并到一起了
    getList () {
      let baseArgs = {
        name: this.searchKey ? this.searchKey : '',
        pageSize: this.pageSize,
        pageNum: this.startPage
      }
      let otherArgs =
        this.typeChecked === '单位'
          ? { unitType: this.selectedUintType }
          : { wayType: this.selectedWayType }
      let api =
        this.typeChecked === '单位'
          ? 'Data/MedicalUnit/list'
          : 'Data/UseWay/list'
      let allArgs = { ...otherArgs, ...baseArgs }
      this.loading = true
      this.swsApi
        .swsPost(api, allArgs)
        .then(res => {
          this.loading = false
          if (res.data.success) {
            this.unit_data = res.data.result
            this.table_data = this.unit_data
            this.dataCount = res.data.dataCount
          } else {
            this.$Notice.error({
              title: '请求错误',
              desc: '网络错误，请稍后再试'
            })
          }
        })
        .catch(e => {
          this.loading = false
          console.log(e)
        })
    },
    search () {
      this.startPage = 1
      // this.typeChecked == '单位'? this.getUnitList(1): this.getUseList(1)
      this.getList()
    },
    // 切换
    changeType (type) {
      this.table_data = []
      this.startPage = 1
      if (type === '单位') {
        this.table_column = this.table_column_unit
        // this.getUnitList(this.startPage)
        this.getList()
      } else {
        this.table_column = this.table_column_use
        // this.getUseList(this.startPage)
        this.getList()
      }
    },
    changePage () {
      // this.getUnitList()
      this.getList()
    },

    // 获取单位列表
    getUnitList (i) {
      i = i || 1
      let args = {
        name: this.searchKey ? this.searchKey : '',
        pageSize: this.pageSize,
        pageNum: this.startPage
      }
      this.loading = true
      this.swsApi
        .swsPost('Data/MedicalUnit/list', args)
        .then(res => {
          this.loading = false
          if (res.data.success) {
            this.unit_data = res.data.result
            this.table_data = this.unit_data
            this.dataCount = res.data.dataCount
          } else {
            this.$Notice.error({
              title: '请求错误',
              desc: '网络错误，请稍后再试'
            })
          }
        })
        .catch(e => {
          this.loading = false
        })
    },
    // 获取用法用量列表
    getUseList (i) {
      i = i || 1
      let args = {
        name: this.searchKey
      }
      this.loading = true
      this.swsApi
        .swsPost('Data/UseWay/list', args)
        .then(res => {
          this.loading = false
          if (res.data.success) {
            this.use_data = res.data.result
            this.table_data = this.use_data
            this.dataCount = res.data.dataCount
          } else {
            this.$Notice.error({
              title: '请求错误',
              desc: '网络错误，请稍后再试'
            })
          }
        })
        .catch(e => {
          this.loading = false
        })
    },
    // filterList (ids) {
    //   this.searchKey = ''
    //   this.startPage = 1
    //   this.getUnitList(this.startPage)
    // },
    showUnitOperate (name) {
      if (name === '添加') {
        this.$refs.unit.resetFields()
        delete this.unitInfo.id
      }
      this.operateUnitFlage = name
      this.operateUnitModel = true
    },
    showUseOperate (name) {
      if (name === '添加') {
        this.$refs.use.resetFields()
        delete this.useInfo.id
      }
      this.operateUseFlage = name
      this.operateUseModel = true
    },
    handleReset () {
      this.operateUnitModel = false
      this.$refs.unit.resetFields()

      this.operateUseModel = false
      this.$refs.use.resetFields()
    },
    submitUnit (text) {
      this.$refs['unit'].validate(valid => {
        if (valid) {
          this.loading = true
          this.swsApi
            .swsPost('Data/MedicalUnit/CreateUpdate', this.unitInfo)
            .then(res => {
              this.loading = false
              if (res.data.result) {
                this.$Message.success(`${text}成功`)
                // this.getUnitList(1)

                this.getList()
              } else {
                this.$Notice.error({
                  title: '请求错误',
                  desc: '网络错误，请稍后再试'
                })
              }
              this.operateUnitModel = false
              this.handleReset()
            })
            .catch(e => {
              console.log(e)
              this.loading = false

              this.operateUnitModel = false
              this.handleReset()
            })
        } else {
          this.$Message.error('请检查是否输入完成')
        }
      })
    },
    submitUse (text) {
      this.$refs['use'].validate(valid => {
        if (valid) {
          this.loading = true
          this.swsApi
            .swsPost('Data/UseWay/CreateUpdate', this.useInfo)
            .then(res => {
              this.loading = false
              if (res.data.result) {
                this.$Message.success(`${text}成功`)
                //  this.getUseList(this.startPage)
                this.getList()
              } else {
                this.$Notice.error({
                  title: '请求错误',
                  desc: '网络错误，请稍后再试'
                })
              }
              this.operateUseModel = false
              this.handleReset()
            })
            .catch(e => {
              console.log(e)
              this.loading = false

              this.operateUseModel = false
              this.handleReset()
            })
        } else {
          this.$Message.error('请检查是否输入完成')
        }
      })
    },
    // 改变状态
    changeState (row) {
      let isActive = row.dataState === 1 ? 2 : 1
      let args = {
        id: row.id,
        isActive
      }
      this.loading = true
      let api =
        this.typeChecked === '单位'
          ? 'Data/MedicalUnit/Active'
          : 'Data/UseWay/Active'
      this.swsApi
        .swsPost(api, args)
        .then(res => {
          this.loading = false
          if (res.data.success) {
            this.getList()
            // this.typeChecked == '单位' ? this.getUnitList(this.startPage) : this.getUseList(this.startPage)
          }
        })
        .catch(e => {
          this.loading = false
        })
    },
    showEdit (row) {
      this.operateUnitFlage = '修改'
      this.operateUnitModel = true
      this.unitInfo = JSON.parse(JSON.stringify(row))
      delete this.unitInfo._index
      delete this.unitInfo._rowKey
    },
    showUseEdit (row) {
      this.operateUseFlage = '修改'
      this.operateUseModel = true
      this.useInfo = JSON.parse(JSON.stringify(row))
      delete this.useInfo._index
      delete this.useInfo._rowKey
    },
    handleOnOperChange (v) {
      // cy清空前搜索记录
      this.searchKey = ''
      this.changeType(v)
    }
  },
  components: {
    Operate
  }
}
</script>

<style scoped lang="less">
#medical {
  position: relative;
  height: 100%;
  .content {
    padding: 0 20px 20px;
    width: 100%;
    .header {
      padding: 20px;
      background: #ffffff;
      .inline-block {
        display: inline-block;
      }
      .type-change {
        margin-bottom: 20px;
        /deep/ .ivu-radio-group-button {
          & + .inline-block {
            margin-left: 20px;
          }
          .ivu-radio-wrapper-checked {
            color: #ffffff;
            background: #4f95e8;
          }
        }
      }
      .filter {
        margin-top: 20px;
        display: flex;
        justify-content: flex-start;
        align-items: center;
        .text {
          margin: 0 10px 0 20px;
          font-size: 14px;
          color: #999;
        }
      }
    }
    .ivu-table-wrapper {
      border: none !important;
      /deep/ .ivu-table-default,
      /deep/ .ivu-table-large {
        background: transparent;
      }
      & /deep/ .ivu-table-header {
        margin-bottom: 20px;
      }
      & /deep/ .ivu-table {
        &::before,
        &::after {
          display: none !important;
        }
      }
      & /deep/ .ivu-table th {
        font-size: 14px;
        background: #fff;
        border-bottom: none;
      }
    }
  }
}
.tag-box {
  display: inline-block;
}
.color-gray {
  color: #999;
}
</style>
