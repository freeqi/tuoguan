<template>
  <div id="data-dictionary">
    <Row class="data-box">
      <Col class="data-left" :lg="5" :sm="5" :md="4">
        <div class="top">
          <Input
            class="type-search"
            v-model="searchDataType"
            @on-search="searchDataTypeF"
            search
            enter-button
            placeholder="请输入字典类型关键字"
          />
        </div>
        <div class="data-type">
          <h4 class="header">
            <span>数据字典类型</span>
            <span class="add" v-permission="buttonRole.SJZD_TJLX" @click="addDicDataModal=true">
              <Icon type="md-add"/>添加类型
            </span>
          </h4>
          <div class="content">
            <spin size="large" fix v-if="searchLoading" style="margin-top: 60px;"></spin>
            <div v-else>
              <ul class="list" v-if="dictionaryType.length>0">
                <li
                  v-for="item in dictionaryType"
                  :key="item.id"
                  @click="changeType(item.id)"
                  :class="{'active': dictionaryTypeCheckedId === item.id}"
                >
                  <span>{{item.name}}</span>
                  <div>
                    <Icon
                      @click.stop="showModifyTypeModal(item.id)"
                      v-permission="buttonRole.SJZD_XGLX"
                      type="ios-create-outline"
                      class="edit"
                      size="22"
                    />
                    <Tag color="success" v-if="item.dataState === 1">可用</Tag>
                    <Tag color="error" v-else>禁用</Tag>
                  </div>
                </li>
              </ul>
              <div class="text" v-else>未查询到数据</div>
            </div>
          </div>
        </div>
      </Col>
      <Col class="data-content" :lg="19" :sm="19" :md="19" style="padding: 0 10px 20px 20px;">
        <Button
          class="add-btn"
          type="primary"
          @click="addDataModal=true"
          v-permission="buttonRole.SJZD_XZSJ"
          icon="md-add"
        >新增数据</Button>
        <Input
          style="margin-top: 20px; width: 300px;display:inline-table;"
          v-model="searchData"
          search
          enter-button
          @on-search="searchDataF"
          placeholder="请输入字典关键字"
        />
        <Table
          no-data-text="暂无数据"
          :height="522"
          style="margin-top: 20px;"
          :loading="loading"
          :columns="table_column"
          :data="dictionary_data"
        ></Table>
      </Col>
    </Row>
    <!-- 添加数据 -->
    <Modal title="新增数据" v-model="addDataModal" width="460" class-name="vertical-center-modal">
      <Form ref="addData" :model="addDataModel" :rules="addDataValidate" :label-width="80">
        <FormItem label="所属类型" prop="typeId">
          <Select ref="select" v-model="addDataModel.typeId" clearable placeholder="请选择所属类型">
            <Option
              v-for="item in dictionaryTypeSelect"
              :value="item.id"
              :key="item.id"
            >{{item.name}}</Option>
          </Select>
        </FormItem>
        <FormItem label="名称" prop="name">
          <Input type="text" v-model="addDataModel.name"></Input>
        </FormItem>
        <FormItem label="排序号" prop="showSortNo">
          <Input type="text" v-model="addDataModel.showSortNo" placeholder></Input>
        </FormItem>
        <FormItem label="值" prop="value">
          <Input type="text" v-model="addDataModel.value" placeholder></Input>
        </FormItem>
      </Form>
      <div slot="footer" style="text-align: center">
        <Button type="primary" size="large" @click="addDataF()">确定</Button>
        <Button type="default" size="large" @click="handleReset('addDataModal', 'addData')">取消</Button>
      </div>
    </Modal>
    <!-- 修改添加数据 -->
    <Modal
      title="修改数据"
      v-model="modifyDataModal"
      width="460"
      class-name="vertical-center-modal"
    >
      <Form ref="modifyData" :model="modifyDataModel" :rules="modifyDataValidate" :label-width="80">
        <FormItem label="名称" prop="name">
          <Input type="text" v-model="modifyDataModel.name" placeholder></Input>
        </FormItem>
        <FormItem label="排序号" prop="number">
          <Input type="text" v-model="modifyDataModel.showSortNo" placeholder></Input>
        </FormItem>
        <FormItem label="值" prop="value">
          <Input type="text" v-model="modifyDataModel.value" placeholder></Input>
        </FormItem>
        <FormItem label="状态" prop="dataState">
          <RadioGroup v-model="modifyDataModel.dataState">
            <Radio :label="item.label" :key="item.id" v-for="item in dataState">{{item.name}}</Radio>
          </RadioGroup>
        </FormItem>
      </Form>
      <div slot="footer" style="text-align: center">
        <Button type="primary" size="large" @click="modifyDataF()">确定</Button>
        <Button type="default" size="large" @click="handleReset('modifyDataModal', 'modifyData')">取消</Button>
      </div>
    </Modal>
    <!-- 新增数据字典类型 -->
    <Modal
      title="新增数据字典类型"
      v-model="addDicDataModal"
      width="380"
      class-name="vertical-center-modal"
    >
      <Form ref="addDicData" :model="addDicDataModel" :rules="addDicDataValidate" :label-width="80">
        <FormItem label="名称" prop="name">
          <Input type="text" v-model="addDicDataModel.name" placeholder></Input>
        </FormItem>
        <FormItem label="值" prop="value">
          <Input type="text" v-model="addDicDataModel.value" placeholder></Input>
        </FormItem>
      </Form>
      <div slot="footer" style="text-align: center">
        <Button type="primary" size="large" @click="addDicDataF()">确定</Button>
        <Button type="default" size="large" @click="handleReset('addDicDataModal', 'addDicData')">取消</Button>
      </div>
    </Modal>
    <!-- 修改数据字典类型 -->
    <Modal
      title="修改数据字典类型"
      v-model="modifyDicDataModal"
      width="380"
      class-name="vertical-center-modal"
    >
      <Form
        ref="modifyDicData"
        :model="modifyDicDataModel"
        :rules="modifyDicDataValidate"
        :label-width="80"
      >
        <FormItem label="名称" prop="name">
          <Input type="text" v-model="modifyDicDataModel.name" placeholder></Input>
        </FormItem>
        <FormItem label="值" prop="value">
          <Input type="text" v-model="modifyDicDataModel.value" placeholder></Input>
        </FormItem>
        <FormItem label="状态" prop="dataState">
          <RadioGroup v-model="modifyDicDataModel.dataState">
            <Radio :label="item.label" :key="item.id" v-for="item in dataState">{{item.name}}</Radio>
            <!-- <Radio label="0">禁用</Radio> -->
          </RadioGroup>
        </FormItem>
      </Form>
      <div slot="footer" style="text-align: center">
        <Button type="primary" size="large" @click="modifyDicDataF()">确定</Button>
        <Button
          type="default"
          size="large"
          @click="handleReset('modifyDicDataModal', 'modifyDicData')"
        >取消</Button>
      </div>
    </Modal>
    <!-- 删除人员 -->
    <Modal v-model="delModal" width="400" class-name="vertical-center-modal">
      <p slot="header">
        <span>删除人员</span>
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
  </div>
</template>

<script>
import Operate from '@/components/operate'
function translateData (result) {
  return new Promise((resolve, reject) => {
    let error = 'result参数为空'
    if (result) resolve(result)
    else reject(error)
  })
}
const BUTTONROLE = {
  SJZD_TJLX: 'SJZD_TJLX',
  SJZD_XZSJ: 'SJZD_XZSJ',
  SJZD_XGLX: 'SJZD_XGLX',
  SJZD_XGSJ: 'SJZD_XGSJ',
  SJZD_SCSJ: 'SJZD_SCSJ'
}
export default {
  data () {
    return {
      type: '',
      // 搜索
      searchDataType: '',
      searchData: '',
      searchLoading: false,
      dictionaryType: [], // {id, name}
      dictionaryTypeCheckedId: -1,
      dictionaryTypeSelect: [], // 新增数据字典类型下拉框
      // 表格
      loading: false,
      table_column: [],
      table_column_default: [
        {
          title: '所属类型',
          key: 'dictionaryTypeName'
        },
        {
          title: '名称',
          key: 'name'
        },
        {
          title: '排序号',
          key: 'showSortNo'
        },
        {
          title: '值',
          key: 'value'
        },
        {
          title: '状态',
          key: 'dataState',
          width: 100,
          align: 'center',
          render: (h, params) => {
            let _text = params.row.dataState === 1 ? '可用' : '禁用'
            let _color = params.row.dataState === 1 ? 'success' : 'error'

            return h(
              'Tag',
              {
                props: {
                  color: _color
                }
              },
              _text
            )
          }
        }
      ],
      table_column_action: [
        {
          title: '操作',
          key: 'action',
          width: 100,
          align: 'center',
          render: (h, params) => {
            return (
              <Operate
                showWatch={false}
                handleDelete={() => {
                  this.delModal = true
                  this.tableRowIndex = params.row.id
                }}
                handleEdit={() => {
                  this.modifyDataModel = this.dictionary_data.filter(v => {
                    return v.id === params.row.id
                  })[0]
                  this.modifyDataModel = Object.assign({}, this.modifyDataModel)
                  this.modifyDataModal = true
                }}
                permissionEdit={this.buttonRole.SJZD_XGSJ}
                permissionDelete={this.buttonRole.SJZD_SCSJ}
              />
            )
          }
        }
      ],
      dictionary_data: [],
      // 新增数据
      addDataModal: false,
      addDataModel: {
        name: '',
        typeId: -1,
        showSortNo: '',
        value: ''
      },
      addDataValidate: {
        name: [
          {
            required: true,
            message: '请输入名称',
            trigger: 'blur'
          }
        ],
        value: [
          {
            required: true,
            message: '请输入值',
            trigger: 'blur'
          }
        ],
        showSortNo: [
          {
            required: true,
            message: '请输入排序号',
            trigger: 'blur'
          }
        ],
        typeId: [
          {
            required: true,
            message: '请选择数据字典类型',
            trigger: 'blur'
          }
        ]
      },
      // 新增数据字典类型
      addDicDataModal: false,
      addDicDataModel: {
        name: '',
        value: ''
      },
      addDicDataValidate: {
        name: [
          {
            required: true,
            message: '请输入名称',
            trigger: 'blur'
          }
        ],
        value: [
          {
            required: true,
            message: '请输入值',
            trigger: 'blur'
          }
        ]
      },
      // 修改数据
      modifyDataModal: false,
      modifyDataModel: {
        id: -1,
        name: '',
        showSortNo: '',
        value: '',
        dataState: 1
      },
      modifyDataValidate: {
        name: [
          {
            required: true,
            message: '',
            trigger: 'blur'
          }
        ],
        value: [
          {
            required: true,
            message: '',
            trigger: 'blur'
          }
        ],
        showSortNo: [
          {
            required: true,
            message: '',
            trigger: 'blur'
          }
        ],
        dataState: [
          {
            required: true,
            type: 'number',
            message: '',
            trigger: 'change'
          }
        ]
      },
      // 修改数据字典类型
      modifyDicDataModal: false,
      dataState: [
        {
          label: 1,
          name: '可用'
        },
        {
          label: 2,
          name: '禁用'
        }
      ],
      modifyDicDataModel: {
        name: '',
        value: '',
        dataState: -1
      },
      modifyDicDataValidate: {
        name: [
          {
            required: true,
            message: '请输入名称',
            trigger: 'blur'
          }
        ],
        value: [
          {
            required: true,
            message: '请输入值',
            trigger: 'blur'
          }
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
      // 删除模态框
      delModal: false,
      // 所点数据id
      tableRowIndex: -1,

      buttonRole: BUTTONROLE
    }
  },
  mounted () {
    this.$nextTick(() => {
      this.searchLoading = true
      this.getDataTypeList().then(res => {
        console.log(res)
        this.dictionaryTypeSelect = res // 第一次进入时缓存数据字典类型
      })

      let hasEdit = this._filterButton(this.buttonRole.SJZD_XGSJ)
      let hasDelete = this._filterButton(this.buttonRole.SJZD_SCSJ)
      if (hasEdit || hasDelete) {
        this.table_column = [...this.table_column_default, ...this.table_column_action]
      } else {
        this.table_column = this.table_column_default
      }
    })
  },
  methods: {
    // 请求数据字典类型列表
    getDataTypeList (params) {
      params = params || {}
      return this.swsApi
        .swsPost('DictionaryType/DictionaryTypeList', params)
        .then(res => {
          let data = res.data
          if (data.success && data.result.length !== 0) {
            this.searchLoading = false
            this.dictionaryType = data.result
            // 每次获取列表之后默认选中第一条数据
            let id = this.dictionaryType[0].id
            this.loading = true
            this.changeType(id)

            return translateData(data.result)
          } else if (data.result.length === 0) {
            this.dictionaryType = []
            this.dictionary_data = []
            this.dictionaryTypeCheckedId = -1

            this.loading = false
          }
        })
        .catch(e => {
          this.loading = false
        })
    },
    // 请求数据字典列表
    getDataList (id, name) {
      this.loading = true
      let params = {
        typeId: `${id}`
      }
      if (name) {
        params.name = name
      }
      this.swsApi
        .swsPost('SystemDictionary/DictionaryList', params)
        .then(res => {
          let data = res.data
          if (data.success) {
            this.dictionary_data = data.result
            this.loading = false
          } else {
            this.$Message.error(data.error)
          }
        })
    },
    // 切换数据字典类型
    changeType (id) {
      this.dictionaryTypeCheckedId = id
      this.getDataList(this.dictionaryTypeCheckedId)
      this.addDataModel.typeId = id
    },
    // 添加数据字典
    addDataF () {
      this.$refs.addData.validate(valid => {
        if (valid) {
          this.swsApi
            .swsPost('SystemDictionary/CreateUpdate', this.addDataModel)
            .then(res => {
              let data = res.data
              if (data.success) {
                this.$Message.success('添加成功')

                this.getDataList(this.addDataModel.typeId) // 重新请求
                this.handleReset('addDataModal', 'addData') // 重置表单
                this.addDataModel.typeId = this.dictionaryTypeCheckedId
              } else {
                this.$Message.error(data.error)
              }
            })
          this.addDataModal = false
        } else {
          this.$Message.error('请检查是否输入完成')
        }
      })
    },
    // 添加数据字典类型
    addDicDataF () {
      this.$refs.addDicData.validate(valid => {
        if (valid) {
          this.swsApi
            .swsPost('DictionaryType/CreateUpdate', this.addDicDataModel)
            .then(res => {
              let data = res.data
              if (data.success) {
                this.$Message.success('添加成功')
                this.getDataTypeList().then(res => {
                  this.dictionaryTypeSelect = res // 添加完成后更新数据字典类型缓存
                })

                this.handleReset('addDicDataModal', 'addDicData') // 重置表单
              } else {
                this.$Message.error('添加失败')
              }
            })
          this.addDicDataModal = false
        } else {
          this.$Message.error('请检查是否输入完成')
        }
      })
    },
    // 修改数据
    modifyDataF () {
      this.$refs.modifyData.validate(valid => {
        if (valid) {
          this.swsApi
            .swsPost('SystemDictionary/CreateUpdate', this.modifyDataModel)
            .then(res => {
              let data = res.data
              if (data.success) {
                this.$Message.success('修改成功')
                this.getDataList(this.dictionaryTypeCheckedId)
              } else {
                this.$Message.error(data.error)
              }
            })
          this.modifyDataModal = false
        } else {
          this.$Message.error('请检查是否输入完成')
        }
      })
    },
    // 修改数据字典类型
    modifyDicDataF () {
      this.dictionaryTypeSelect = []
      this.addDataModel.typeId = null
      this.$refs.modifyDicData.validate(valid => {
        if (valid) {
          this.swsApi
            .swsPost('DictionaryType/CreateUpdate', this.modifyDicDataModel)
            .then(res => {
              let data = res.data
              if (data.success) {
                this.$Message.success('修改成功')
                // 更新数据字典列表
                this.getDataTypeList().then(res => {
                  this.addDataModel.typeId = this.dictionaryTypeCheckedId
                  this.dictionaryTypeSelect = res
                })
              } else {
                this.$Message.error(data.error)
              }
            })
          this.modifyDicDataModal = false
        } else {
          this.$Message.error('请检查是否输入完成')
        }
      })
    },
    // 显示修改数据字典类型模态框
    showModifyTypeModal (id) {
      this.modifyDicDataModel = Object.assign(
        {},
        this.dictionaryType.filter(v => v.id === id)[0]
      )
      this.modifyDicDataModal = true
    },
    // 重置表单v-model绑定的参数
    handleReset (Modal, name) {
      this[Modal] = false
      this.$refs[name].resetFields()
    },
    // 删除单条数据字典数据
    del () {
      this.swsApi
        .swsGet(`SystemDictionary/del/${this.tableRowIndex}`)
        .then(res => {
          if (res.data.result) {
            this.$Message.success('删除成功！')
            this.getDataList(this.dictionaryTypeCheckedId)
          } else {
            this.$Message.error(res.data.error)
          }
        })

      this.delModal = false
    },
    // 搜索
    searchDataTypeF () {
      let params = {
        name: this.searchDataType
      }

      this.getDataTypeList(params)
    },
    searchDataF () {
      if (!this.dictionaryTypeCheckedId) return
      this.loading = true
      this.getDataList(this.dictionaryTypeCheckedId, this.searchData)
    }
  },
  components: {
    Operate
  }
}
</script>

<style scoped lang="less">
#data-dictionary {
  height: 100%;
  background: #ffffff;
  .data-box {
    height: 100%;
    & > * {
      height: 100%;
    }
    .data-left {
      display: flex;
      flex-direction: column;
      // min-width: 220px;
      border-right: 1px solid #eaeaea;
      .top {
        padding: 20px;
      }
      .data-type {
        flex: 1;
        display: flex;
        flex-direction: column;
        overflow: hidden;
        .header {
          padding: 0 20px;
          display: flex;
          align-items: center;
          justify-content: space-between;
          color: #333;
          font-size: 16px;
          height: 45px;
          background: #f9f9f9;
          .add {
            color: #4f95e8;
            font-size: 14px;
            cursor: pointer;
          }
        }
        .content {
          position: relative;
          flex: 1;
          overflow-y: auto;
        }
        .list {
          li {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 0 20px;
            font-size: 14px;
            height: 45px;
            transition: all 0.3s;
            span {
              margin-top: 4px;
              flex: 1;
              width: 0;
              overflow: hidden;
              text-overflow: ellipsis;
              white-space: nowrap;
            }
            .edit {
              margin-right: 4px;
              display: none;
              cursor: pointer;
            }
            &.active,
            &:hover {
              color: #4f95e8;
              span {
                font-weight: bold;
              }
              background: #f2f8ff;
              .edit {
                display: inline-block;
              }
            }
          }
        }
        .text {
          height: 45px;
          line-height: 45px;
          text-align: center;
        }
      }
    }
    .data-content {
      min-width: 466px;
      overflow-y: auto;
      // cy：改按钮与输入框的布局
      .add-btn {
        margin: 21px 12px 0 0;
      }
      // cy:调大table的字体
      .ivu-table-wrapper {
        & /deep/ .ivu-table td {
          font-size: 13px;
        }
      }
    }
  }
}
</style>
