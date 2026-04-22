<template>
  <div id="version_content">
    <div class="button-group">
      <span>时间段</span>
      <DatePicker
        v-model="time"
        format="yyyy-MM-dd"
        type="daterange"
        placeholder="请选择时间段"
        style="width:190px;"
        @on-change="changeDate">
      </DatePicker>
      <!-- <span>查询</span>
      <Input v-model="keySearch"
        search enter-button
        @on-search="getVersionRecord"
        placeholder="请输入版本/机构"
        style="width: 170px;display: inline-table;"/> -->
      <span class="rightBtn-group">
        <Button type="success" @click="addVersionRecord">新增</Button>
      </span>
    </div>
    <Divider></Divider>
    <Tabs type="card" v-model="tabName" @on-click="tabsChange">
        <TabPane label="PC版本" name="pc">
          <div class="table_detail">
            <Table
              :data="table_data_pc"
              :columns="table_columns_pc"
              :loading="table_laoding">
            </Table>
            <div class="pagination" v-if="dataCount > pageSize">
              <Page
                :total="dataCount"
                :page-size="pageSize"
                :current.sync="pageNum"
                @on-change="getVersionRecord"
              />
            </div>
          </div>
        </TabPane>
        <TabPane label="APP版本" name="app">
          <div class="table_detail">
            <Table
              :data="table_data_app"
              :columns="table_columns_app"
              :loading="table_laoding">
            </Table>
            <div class="pagination" v-if="dataCount > pageSize">
              <Page
                :total="dataCount"
                :page-size="pageSize"
                :current.sync="pageNum"
                @on-change="getVersionRecord"
              />
            </div>
          </div>
        </TabPane>
    </Tabs>
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
    <!-- 修改 / 新增 -->
    <Modal :title="editTitle" v-model="editModal" class-name="vertical-center-modal" width="600">
      <div class="modalContentPC" style="display: flex;height: 550px;" v-show="tabName === 'pc'">
        <div class="leftContent">
          <h3>选择机构</h3>
          <div style="border-bottom: 1px solid #e9e9e9;padding-bottom:6px;margin-bottom:6px;">
            <Checkbox
                :indeterminate="indeterminate"
                :value="checkAll"
                @click.prevent.native="handleCheckAll">全选</Checkbox>
          </div>
          <CheckboxGroup v-model="checkHospitalGroup" @on-change="checkAllGroupChange"
            style="display: flex;flex-direction: column;height: 300px;overflow-y: auto;width: 230px;">
            <Checkbox :label="item.id" v-for="item in hospitalList" style="margin:5px 0;" :key="item.id">{{item.dialysisName}}</Checkbox>
          </CheckboxGroup>
        </div>
        <Divider type="vertical" style="height:100%;width:5px;"/>
        <div class="rightContent">
          <Form
            ref="editModalRef"
            :rules="editModalFormRule"
            :model="editModalForm"
            :label-width="110">
              <Row>
                <FormItem label="id" v-show="false">
                  <Input placeholder="请输入" v-model="editModalForm.id"/>
                </FormItem>
                <!-- <Col span="24">
                  <FormItem prop="sysCHSName" label="系统中文名称">
                    <Input placeholder="请输入" v-model="editModalForm.sysCHSName"/>
                  </FormItem>
                </Col>
                <Col span="24">
                  <FormItem prop="sysUSName" label="系统英文名称">
                    <Input placeholder="请输入" v-model="editModalForm.sysUSName"/>
                  </FormItem>
                </Col> -->
                <Col span="24">
                  <FormItem prop="sysType" label="系统类型">
                    <Select v-model="editModalForm.sysType" placeholder="请选择系统类型" @on-change="sysTypeChange">
                      <Option :value="index + 1" v-for="(item,index) in sysState" :key="index">{{item}}</Option>
                    </Select>
                  </FormItem>
                </Col>
                <Col span="24">
                  <FormItem prop="devTagsVersion" label="开发版本">
                    <Input placeholder="请输入" v-model="editModalForm.devTagsVersion"/>
                  </FormItem>
                </Col>
                <Col span="24">
                  <FormItem prop="sysVersion" label="系统发布版本">
                    <Input placeholder="请输入" v-model="editModalForm.sysVersion"/>
                  </FormItem>
                </Col>
                <Col span="24">
                  <FormItem prop="publishDate" label="系统发布时间">
                    <DatePicker
                      v-model="editModalForm.publishDate"
                      format="yyyy-MM-dd"
                      type="date" transfer
                      placeholder="请选择时间阶段"
                    ></DatePicker>
                  </FormItem>
                </Col>
                <Col span="24">
                  <FormItem label="更新/修改详情" prop="updateInfo">
                    <Input
                      placeholder="请输入"
                      type="textarea" :rows="6"
                      v-model="editModalForm.updateInfo"
                    ></Input>
                  </FormItem>
                </Col>
                <Col span="24">
                  <FormItem label="备注" prop="remark">
                    <Input
                      placeholder="请输入"
                      type="textarea"
                      v-model="editModalForm.remark"
                    ></Input>
                  </FormItem>
                </Col>
              </Row>
            </Form>
        </div>
      </div>
      <div class="modalContentAPP" v-show="tabName === 'app'">
        <Form ref="editModalAPPRef"
          :rules="editModalFormAPPRule"
          :model="editModalForm_app"
          :label-width="110">
          <Row>
            <FormItem prop="id" label="id" v-show="false">
              <Input placeholder="请输入" v-model="editModalForm_app.id"/>
            </FormItem>
            <Col span="12">
              <FormItem prop="appCHSName" label="APP中文名称">
                <Input placeholder="请输入" v-model="editModalForm_app.appCHSName"/>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem prop="appUSName" label="APP英文名称">
                <Input placeholder="请输入" v-model="editModalForm_app.appUSName"/>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem prop="appVersion" label="APP版本号">
                <Input placeholder="请输入APP版本号" v-model="editModalForm_app.appVersion"/>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem prop="appUpdateType" label="APP更新类型">
                <Select v-model="editModalForm_app.appUpdateType" placeholder="请选择APP更新类型">
                  <Option :value="index + 1" v-for="(item,index) in appUpdateType" :key="index">{{item}}</Option>
                </Select>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem prop="appDownUrl" label="APP下载地址1">
                <Input type="textarea" placeholder="请输入" v-model="editModalForm_app.appDownUrl"/>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem prop="appWgtDownUrl" label="APP下载地址2">
                <Input type="textarea" placeholder="请输入" v-model="editModalForm_app.appWgtDownUrl"/>
              </FormItem>
            </Col>
            <Col span="12">
              <FormItem prop="isForcedUpdate" label="是否强制更新">
                <Select v-model="editModalForm_app.isForcedUpdate" placeholder="请选择是否强制更新">
                  <Option :value="1" :key="1">是</Option>
                  <Option :value="2" :key="2">否</Option>
                </Select>
              </FormItem>
            </Col>
            <Col span="10" offset="1">
              <Upload
                ref="uploadRef"
                :before-upload="beforeUpload"
                :on-success="handleSuccess"
                :show-upload-list="false"
                :format="['apk','wgt']" accept=".apk,.wgt"
                :on-format-error="formatError"
                :action="`${url}`"
                style="display: inline-block;margin-right: 10px;"
              >
                <Button icon="ios-cloud-upload-outline">上传文件</Button>
              </Upload>
              <Button v-show="file.name&&!uploadFlag" :loading="uploading" style="display:inline-block;" type="primary" @click="sureUpload">确定</Button>
              <Button v-show="uploadFlag" style="display:inline-block;" @click="file='';uploadFlag=false">取消</Button>
              <div class="uploaded-file" v-if="file !== ''">
                <span>{{uploadFlag?'上传成功':'已选文件'}}: {{ file.name }}</span>
                <Icon v-show="uploadFlag" class="close" type="md-checkmark" color="green" size="14" />
                <Icon v-show="!uploadFlag" class="close" type="md-close" style="cursor:pointer;" color="red" size="14" @click="file=''" />
              </div>
            </Col>
            <Col span="24">
              <FormItem label="备注" prop="remark">
                <Input
                  placeholder="请输入"
                  type="textarea" :rows="3"
                  v-model="editModalForm_app.remark"
                ></Input>
              </FormItem>
            </Col>
          </Row>
        </Form>
      </div>
      <div slot="footer">
        <Button type="primary" :loading="handleEditBtn" @click="handleChangeRecord">确定</Button>
        <Button type="default" @click="editModal=false;file='';">取消</Button>
      </div>
    </Modal>

  </div>
</template>

<script>
import Operate from '@/components/operate'
import {formateDateToString} from '@/libs/tools.js'
const BUTTONROLE = {
  VS_ADD: 'VS_ADD',
  VS_EDIT: 'VS_EDIT',
  VS_DEL: 'VS_DEL'
}
export default {
  data () {
    return {
      uploading: false,//upload btn loading
      uploadFlag: false,//is upload successed
      file: '',//upload file
      URL:
        process.env.NODE_ENV === 'development'
          ? this.$config.baseURL.dev
          : this.$config.baseURL.pro,
      indeterminate: false,
      checkAll: false,
      pushBtnLoadingId: '',
      tabName: 'pc', // cy 默认值为pc
      checkHospitalGroup: [], // cy 选中的机构
      // pushCenterBtnLoading: false,
      time: [],
      beginTime: '',
      endTime: '',
      title: '',
      delModal: false,
      buttonRole: BUTTONROLE,
      tableRowIndex: -1,

      keySearch: '',
      table_data_app: [],
      table_data_pc: [],
      table_columns_app: [
        {
          title: '序号',
          type: 'index',
          width: 70,
          align: 'center'
        },
        {
          title: 'APP中文名称',
          key: 'appCHSName',
          width: 110,
        },
        {
          title: 'APP英文名称',
          key: 'appUSName',
          width: 110,
        },
        {
          title: 'APP版本号',
          key: 'appVersion',
          width: 110,
        },
        {
          title: 'APP下载地址1',
          key: 'appDownUrl',
          minWidth: 140
        },
        {
          title: 'APP下载地址2',
          key: 'appWgtDownUrl',
          minWidth: 140
        },
        {
          title: 'APP更新类型',
          key: 'appUpdateType',
          width: 110,
          render: (h, params) => {
            return [1, 2].includes(params.row.appUpdateType) ? <span>{this.appUpdateType[params.row.appUpdateType - 1]}</span> : <span>/</span>
          }
        },
        {
          title: '是否强制更新',
          key: 'isForcedUpdate',
          width: 110,
          render: (h, params) => {
            return params.row.isForcedUpdate === 1 ? <span>是</span> : <span>否</span>
          }
        },
        {
          title: '创建时间',
          key: 'founderDate',
          align: 'center',
          width: 100,
          render: (h, params) => {
            return <span>{params.row.founderDate && params.row.founderDate.slice(0, 10)}</span>
          }
        },
        {
          title: '修改时间',
          key: 'modifierDate',
          align: 'center',
          width: 100,
          render: (h, params) => {
            return <span>{params.row.modifierDate && params.row.modifierDate.slice(0, 10)}</span>
          }
        },
        {
          title: '备注',
          key: 'remark',
          tooltip: true,
          transfer: true,
          minWidth: 140,
        },
        {
          title: '操作',
          key: 'action',
          width: 100,
          align: 'center',
          fixed: 'right',
          render: (h, params) => {
            return (
              <Operate
                showWatch={false}
                // showEdit={false}
                showUndo={false}
                handleEdit={() => this.handleEdit(params.row, 'app')}
                handleDelete={() => {
                  this.delModal = true
                  this.tableRowIndex = params.row.id
                  this.title = '删除'
                }}
                permissionDelete={this.buttonRole.VS_DEL}
                permissionEdit={this.buttonRole.VS_EDIT}
              />
            )
          }
        }
      ],
      table_columns_pc: [
        // {
        //   title: '序列',
        //   key: 'no',
        //   align: 'center',
        //   width: 50,
        //   render: (h, {index}) => {
        //     let No = (index + 1) + this.pageSize * (this.pageNum - 1)
        //     return <span>{No}</span>
        //   }
        // },
        {
          title: '序号',
          type: 'index',
          align: 'center',
          width: 60
          // indexMethod: (row) => {
          //   return <span>{row.index}</span>
          // }
        },
        {
          title: '机构',
          // key: 'publishCenterName',
          key: 'id',
          tooltip: true,
          width: 150,
          render: (h, params) => {
            let arr = this.handleIdtoHospitalName(params.row.updateCenter)
            let cont = arr.map(res => {
              return h('div', `${res}`)
            })
            return h('div', [
              h(
                'Poptip',
                {
                  props: {
                    wordWrap: true, transfer: true, trigger: 'hover', placement: 'right'
                  }
                },
                [
                  h('div',
                    {
                      // props: {
                      style: {
                        width: '150px',
                        overflow: 'hidden',
                        whiteSpace: 'nowrap',
                        textOverflow: 'ellipsis'
                      }
                      // }
                    },
                    arr.join('')
                  ),
                  h(
                    'div',
                    {
                      slot: 'content'
                    },
                    cont
                  )
                ]
              )
            ])
          }
        },
        // {
        //   title: '系统中文名称',
        //   key: 'sysCHSName',
        //   minWidth: 82
        // },
        // {
        //   title: '系统英文名称',
        //   key: 'sysUSName',
        //   minWidth: 82
        // },
        {
          title: '开发版本',
          key: 'devTagsVersion',
          width: 160,
        },
        {
          title: '系统发布版本',
          width: 120,
          key: 'sysVersion'
        },
        {
          title: '系统发布时间',
          key: 'publishDate',
          width: 110,
          render: (h, params) => {
            return <span>{params.row.publishDate && params.row.publishDate.slice(0, 10)}</span>
          }
        },
        {
          title: '系统类型',
          key: 'sysType',
          align: 'center',
          width: 100,
          render: (h, params) => {
            if (params.row.sysType === 1) {
              return <span>{this.sysState[params.row.sysType - 1]}</span>
            } else if (params.row.sysType === 2) {
              return <span>{this.sysState[params.row.sysType - 1]}</span>
            }
          }
        },
        {
          title: '更新修改详情',
          key: 'updateInfo',
          tooltip: true,
          minWidth: 130
        },
        // {
        //   title: '创建人',
        //   key: 'founder',
        //   minWidth: 65
        // },
        {
          title: '创建时间',
          key: 'founderDate',
          align: 'center',
          width: 100,
          render: (h, params) => {
            return <span>{params.row.founderDate && params.row.founderDate.slice(0, 10)}</span>
          }
        },
        {
          title: '修改时间',
          key: 'modifierDate',
          align: 'center',
          width: 100,
          render: (h, params) => {
            return <span>{params.row.modifierDate && params.row.modifierDate.slice(0, 10)}</span>
          }
        },
        {
          title: '备注',
          key: 'remark',
          tooltip: true,
          transfer: true,
          width: 100
        },
        {
          title: '是否推送',
          key: 'isPush',
          width: 90,
          fixed: 'right',
          render: (h, params) => {
            return params.row.isPush
              ? <i-button type="success" size="small" >已推送</i-button>
              : <i-button type="error" size="small" loading={params.row.id === this.pushBtnLoadingId} onClick={() => { this.pushCenter(params.row) }}>未推送</i-button>
          }
        },
        {
          title: '操作',
          key: 'action',
          width: 90,
          align: 'center',
          fixed: 'right',
          render: (h, params) => {
            return (
              <Operate
                showWatch={false}
                showUndo={false}
                showDelete={!params.row.isPush}
                handleEdit={() => {
                  this.indeterminate =false;
                  this.checkAll = false
                  this.handleEdit(params.row, 'pc')
                }}
                handleDelete={() => {
                  this.delModal = true
                  this.tableRowIndex = params.row.id
                  this.title = '删除'
                }}
                permissionDelete={this.buttonRole.VS_DEL}
                permissionEdit={this.buttonRole.VS_EDIT}
              />
            )
          }
        }
      ],
      appUpdateType: ['app更新', 'wgt热更新'],
      sysState: ['web', 'server/api'],
      table_laoding: false,
      dataCount: 0,
      pageSize: 10,
      pageNum: 1,

      editTitle: '',
      editModal: false,
      handleEditBtn: false,
      editModalForm_app: {
        appCHSName: '',
        appUSName: '',
        appVersion: '',
        appDownUrl: '',
        appWgtDownUrl: '',
        appUpdateType: '',
        isForcedUpdate: '',
        remark: ''
      },
      editModalFormAPPRule: {
        appCHSName: [
          { required: true, message: 'app中文名称不能为空', trigger: 'blur' }],
        appUSName: [
          { required: true, message: 'app英文名称不能为空', trigger: 'blur' }],
        appDownUrl: [
          { required: true, message: '下载地址1不能为空', trigger: 'blur' }],
        appWgtDownUrl: [
          { required: true, message: '下载地址2不能为空', trigger: 'blur' }],
        appVersion: [
          { required: true, message: 'app版本号不能为空', trigger: 'blur' }],
        appUpdateType: [
          { required: true, type: 'number', message: 'app更新类型不能为空', trigger: 'blur' }],
        isForcedUpdate: [
          { required: true, type: 'number', message: '是否强制更新不能为空', trigger: 'blur' }]
      },
      editModalForm: {
        sysCHSName: '',
        sysUSName: '',
        sysType: 1,
        devTagsVersion: '',
        sysVersion: '',
        publishDate: '',
        updateInfo: '',
        remark: ''
      },
      editModalFormRule: {
        sysCHSName: [
          { required: true, message: '系统中文名称不能为空', trigger: 'blur' }],
        sysUSName: [
          { required: true, message: '系统英文名称不能为空', trigger: 'blur' }],
        devTagsVersion: [
          { required: true, message: '开发版本不能为空', trigger: 'blur' }],
        sysType: [
          { required: true, type: 'number', message: '系统类型不能为空', trigger: 'blur' }],
        sysVersion: [
          { required: true, message: '系统发布版本不能为空', trigger: 'blur' }],
        updateInfo: [
          { required: true, message: '更新/修改详情不能为空', trigger: 'blur' }],
        publishDate: [
          { required: true, type: 'date', message: '系统发布时间不能为空', trigger: 'blur' }]
      },
      hospitalList: []
    }
  },
  components: {Operate},
  computed: {
    urlType () {
      if (this.tabName === 'pc') {
        return 'Publish'
      } else if (this.tabName === 'app') {
        return 'APPPublish'
      }
    },
    url () {
      return this.URL + '/data/packagefileup/' +this.file.name
    }
  },
  created () {
    this.swsApi
      .swsPost('CenterDialysis/DialysisList')
      .then(res => {
        if (res.data.success) {
          this.hospitalList = res.data.result
        }
      })
      .catch(e => {
        this.$Message.success(`请求报错，请联系管理员。${e}`)
      })
  },
  mounted () {
    this.$nextTick(() => {
      this.getVersionRecord()
    })
  },
  methods: {
    sureUpload () {
      // 防止重复点击
      if (this.uploading) return false
      this.uploading = true
      this.$refs.uploadRef.post(this.file)
    },
    beforeUpload (file) {
      this.file = file
      return false
    },
    handleSuccess (res) {
      this.uploading = false
      // 文件上传出错
      if (!res.success) {
        this.uploadFlag = false
        this.$Notice.error({
          title: '上传失败',
          desc: res.error
        })
      } else {
        this.uploadFlag = true
        this.$Message.success('上传成功！')
        let len = this.file.name.length
        let name = this.file.name.slice(0,-4)
        let type = this.file.name.slice(len-3,len)
        if (this.title=='新增') {
          this.editModalForm_app = {
            appCHSName: name,
            appUSName: name,
            appDownUrl: res.result,
            appWgtDownUrl: res.result,
            appVersion: name,
            appUpdateType: type=='wgt'?2:1,
            isForcedUpdate: 2
          }
        } else {
          this.editModalForm_app.appDownUrl = res.result
          this.editModalForm_app.appWgtDownUrl = res.result
        }
      }
    },
    // 上传格式出错
    formatError () {
      this.$Notice.error({
        title: '格式错误',
        desc: '文件格式错误，请重新选择'
      })
    },
    handleCheckAll () {
        if (this.indeterminate) {
            this.checkAll = false;
        } else {
            this.checkAll = !this.checkAll;
        }
        this.indeterminate = false;

        if (this.checkAll) {
            this.checkHospitalGroup = this.hospitalList.map(res=> res.id);
        } else {
            this.checkHospitalGroup = [];
        }
    },
    checkAllGroupChange (data) {
        if (data.length === this.hospitalList.length) {
            this.indeterminate = false;
            this.checkAll = true;
        } else if (data.length > 0) {
            this.indeterminate = true;
            this.checkAll = false;
        } else {
            this.indeterminate = false;
            this.checkAll = false;
        }
    },
    sysTypeChange (v) {
      let timeStr = formateDateToString(new Date(), 'yyyyMMdd')
      this.editModalForm.devTagsVersion = v === 1 ? `web_${timeStr}_` : `api_${timeStr}_`
    },
    tabsChange () {
      this.pageNum = 1
      this.getVersionRecord()
    },
    handleIdtoHospitalName (data) {
      let idList = data.split(',')
      let arr = idList.map(id => {
        let arr1 = this.hospitalList.filter(res => {
          return res.id === id.trim()
        })
        return arr1.length > 0 ? arr1[0].dialysisName : '未知'
      })
      return arr
    },
    pushCenter (row) {
      this.$Modal.confirm({
        title: '提示',
        content: '<p>确定要进行推送操作吗？</p>',
        onOk: () => this.handlePushCenter(row.id)
      })
    },
    handlePushCenter (id) {
      this.pushBtnLoadingId = id
      this.swsApi.swsGet(`Data/Publish/SendPublish/${id}`)
        .then(res => {
          if (res.data.success) {
            this.$Message.success(`推送成功！`)
            this.getVersionRecord()
          } else {
            this.$Message.success(`推送失败，请稍后再试。`)
          }
          this.pushBtnLoadingId = ''
        })
        .catch(e => {
          this.pushBtnLoadingId = ''
          this.$Message.success(`推送请求报错，请联系管理员。`)
        })
    },
    addVersionRecord () {
      this.indeterminate =false;
      this.checkAll = false;
      let refName = ''
      this.title = '新增'
      if (this.tabName === 'pc') {
        refName = 'editModalRef'
        this.checkHospitalGroup = [] // 清空选中机构id
        this.editTitle = `${this.title} 系统版本记录`
        let timeStr = formateDateToString(new Date(), 'yyyyMMdd')
        this.editModalForm = {
          sysType: 1,
          devTagsVersion: `web_${timeStr}_`,
          sysVersion: `v_${timeStr}_`,
          publishDate: new Date()
        }
      } else if (this.tabName === 'app') {
        refName = 'editModalAPPRef'
        this.editTitle = `${this.title} APP版本记录`
      }
      this.handleReset(refName) // 清空表单数据
      this.editModal = true
    },
    changeDate (date) {
      this.beginTime = date[0]
      this.endTime = date[1]
      this.getVersionRecord()
    },
    handleChangeRecord () {
      let refName = ''
      let params = {}
      if (this.tabName === 'app') {
        refName = 'editModalAPPRef'
        params = {
          ...this.editModalForm_app
        }
        if (this.editModalForm_app.id === '') {
          delete params.id
        }
      } else if (this.tabName === 'pc') {
        if (this.checkHospitalGroup.length <= 0) {
          this.$Message.info('请至少选择一个机构！')
          return false
        }
        refName = 'editModalRef'
        params = {
          ...this.editModalForm,
          updateCenter: this.checkHospitalGroup.join(',')
        }

        params.publishDate = new Date(params.publishDate.setHours(params.publishDate.getHours() + 8))
        if (this.editModalForm.id === '') {
          delete params.id
          delete params.isPush
        }
      }
      // console.log('refName:', refName, this.$refs[refName])
      this.$refs[refName].validate(valid => {
        if (valid) {
          this.handelPost(params, refName)
        } else {
          this.$Message.info('请完善填写信息！')
        }
      }).catch(e => {
        console.log(e)
        this.$Message.info('form error', e)
      })
    },
    handelPost (params, refName) {
      this.swsApi.swsPost(`Data/${this.urlType}/AddOrUpdate`, params)
        .then(res => {
          if (res.data.success) {
            this.$Message.success(`版本记录 ${this.title}操作成功！`)
            this.getVersionRecord()
            // 清空上传文件的数据
            if (this.tabName === 'app') {
              this.uploadFlag = false
              this.file = ""
            }
          } else {
            this.$Message.error('操作失败', res.data.error)
          }
        })
        .catch(e => {
          this.$Message.error(`${e}，请稍后再试！`)
        })
      if (this.editModal) {
        this.handleReset(refName)
        // 
        this.$refs.uploadRef.clearFiles()
        this.editModal = false
        this.handleEditBtn = false
      }
    },
    handleEdit (row, type) {
      this.title = '修改'
      if (type === 'app') {
        // this.handleReset('editModalAPPRef')
        this.editTitle = `${this.title} ${row.appCHSName} 版本记录`
        let {id, appCHSName, appUSName, appVersion, appDownUrl, appWgtDownUrl, appUpdateType, isForcedUpdate, remark} = {...row}
        this.editModalForm_app = {id, appCHSName, appUSName, appVersion, appDownUrl, appWgtDownUrl, appUpdateType, isForcedUpdate, remark}
      } else {
        // this.handleReset('editModalRef')
        this.editTitle = `${this.title} ${row.sysCHSName} 版本记录`
        let {id, sysCHSName, sysUSName, devTagsVersion, sysVersion, sysType, publishDate, updateInfo, isPush, remark} = { ...row }
        this.editModalForm = {id, sysCHSName, sysUSName, devTagsVersion, sysVersion, sysType, publishDate, updateInfo, isPush, remark}
        // cy
        this.checkHospitalGroup = row.updateCenter.split(',')
      }
      this.editModal = true
    },
    // 确认删除
    handleConfirmDelete () {
      this.delModal = false
      this.swsApi.swsGet(`Data/${this.urlType}/Delete/${this.tableRowIndex}`).then(res => {
        if (res.data.success) {
          this.$Message.success(`删除成功！`)
          this.pageNum = 1
          this.getVersionRecord()
        } else {
          this.$Message.error(`删除失败，${res.data.error}`)
        }
      })
    },
    getVersionRecord () {
      this.table_laoding = true
      let params = {
        beginRunDate: this.beginTime,
        endRunDate: this.endTime,
        pageSize: this.pageSize,
        pageNum: this.pageNum
      }
      this.swsApi.swsPost(`Data/${this.urlType}/list`, params)
        .then(res => {
          if (res.data.success) {
            if (this.tabName === 'pc') {
              this.table_data_pc = res.data.result
            } else {
              this.table_data_app = res.data.result
            }
            this.dataCount = res.data.dataCount
          } else {
            this.$Message.error(`请求版本发布记录失败，${res.data.error}`)
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
#version_content {
  padding: 20px 40px 0;
  /* padding: 0 0 10px; */
  height: 100%;
  background: #ffffff;
  .button-group {
    margin-bottom: 26px;
    text-align: left;
    button + button {
      margin-left: 10px;
    }
    span {
      margin: 0 15px;
      display: inline-block;
      text-align: right;
    }
    span:nth-child(1) {
      margin-left: 0;
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
  .modalContentPC {
    width: 100%;
    height: 800px;
    display: flex;
    .leftContent {
      // .vertical_box{
      /deep/ .ivu-checkbox-group .ivu-checkbox-default{
        display: flex;
        flex-direction: column;
        height: 100px;
        overflow-y: auto;
      }
    }
    .rightContent {
      display: block;
      width: 570px;
      height: 500px;
    }

  }
}
</style>
