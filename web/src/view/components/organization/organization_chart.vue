<template>
  <div>
    <div class="organization-operate">
      <div class="operate">
        <div>
          <Button
            class="add-btn"
            type="primary"
            @click="addModal=true;modalTitle='添加'"
            icon="md-add"
            v-permission="buttonRole.JGGL_XZJG"
          >新增机构</Button>
          <sws-upload
            v-permission="buttonRole.JGGL_DR"
            :importId="importId"
            @on-success-upload="handleUploadSuccess"
          ></sws-upload>
        </div>
        <Input
          v-model="searchKey"
          @on-search="search"
          search
          enter-button
          placeholder="请输入血透机构"
          style="width: 350px"
        />
      </div>
      <!-- 机构列表 -->
      <swsTable
        class="organization-list"
        :loading="loading"
        :columns="table_columns"
        :data="table_data"
        :pagination="pagination"
        @on-change="handleChangePage"
      >
    
    
    </swsTable>
    </div>
    <!-- 删除机构 -->
    <Modal v-model="delModal" width="400" class-name="vertical-center-modal">
      <p slot="header">
        <span>删除机构</span>
      </p>
      <div style="text-align:center; padding: 40px 0; font-size: 16px;">
        <p>
          <Icon type="ios-help-circle" color="#ff81a3" size="30" style="margin-right: 20px;" />删除后不可恢复，您确定删除吗？
        </p>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="del()">确定</Button>
        <Button type="default" @click="delModal=false">取消</Button>
      </div>
    </Modal>
    <!-- 添加机构 -->
    <Modal v-model="addModal" width="800" @on-cancel="cancel">
      <p slot="header">
        <span>{{modalTitle}}机构</span>
      </p>
      <div>
        <Form ref="form" :model="formValidate" :rules="ruleValidate" :label-width="80">
          <Row>
            <Col :lg="12" :md="12" :sm="24">
              <FormItem label="机构名称" prop="dialysisName">
                <Input v-model="formValidate.dialysisName" placeholder="请输入机构名称"></Input>
              </FormItem>
            </Col>
            <Col :lg="12" :md="12" :sm="24">
              <FormItem label="主任" prop="dialysisContactManID">
                <Select
                  :label-in-value="true"
                  filterable
                  v-model="formValidate.dialysisContactManID"
                  transfer
                >
                  <Option
                    v-for="item in ContactManList"
                    :value="item.id"
                    :key="item.id"
                  >{{ item.name }}</Option>
                </Select>
              </FormItem>
            </Col>
            <Col :lg="12" :md="12" :sm="24">
              <FormItem label="护士长" prop="headNurseId">
                <Select
                  :label-in-value="true"
                  filterable
                  v-model="formValidate.headNurseId"
                  transfer
                >
                  <Option
                    v-for="item in ContactManList"
                    :value="item.id"
                    :key="item.id"
                  >{{ item.name }}</Option>
                </Select>
              </FormItem>
            </Col>
            <Col :lg="12" :md="12" :sm="24">
              <FormItem label="联系电话" prop="dialysisPhone">
                <Input v-model="formValidate.dialysisPhone" placeholder="请输入联系电话"></Input>
              </FormItem>
            </Col>
            <Col :lg="12" :md="12" :sm="24" style="z-index: 999999">
              <FormItem label="所在地区" prop="dialysisRegionID">
                <Cascader
                  :data="DialysisRegion"
                  v-model="formValidate.dialysisRegionID"
                  placeholder="请选择地区"
                  change-on-select
                  @on-change="cascaderChange"
                ></Cascader>
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Col :lg="12" :md="12" :sm="24">
              <FormItem label="成立时间" prop="setUpDate">
                <DatePicker
                  type="date"
                  format="yyyy-MM-dd"
                  placeholder="请输入成立时间"
                  v-model="formValidate.setUpDate"
                  transfer
                ></DatePicker>
              </FormItem>
            </Col>
            <Col :lg="12" :md="12" :sm="24">
              <FormItem label="运行时间" prop="runDate">
                <DatePicker
                  type="date"
                  format="yyyy-MM-dd"
                  placeholder="请输入运行时间"
                  v-model="formValidate.runDate"
                  transfer
                ></DatePicker>
              </FormItem>
            </Col>
            <Col :lg="24" :md="24" :sm="24">
              <FormItem label="详细地址" prop="dialysisAddress">
                <Input v-model="formValidate.dialysisAddress" placeholder="请输入详细地址"></Input>
              </FormItem>
            </Col>
            <Col :lg="24" :md="24" :sm="24">
              <FormItem label="机构简介" prop="dialysisDetails">
                <editor ref="editor"></editor>
              </FormItem>
            </Col>
            <Col :lg="24" :md="24" :sm="24">
              <FormItem label="上传照片" prop>
                <span>（大小不超过2M，1366*500px）</span>
                <div id="upload">
                  <Upload
                    ref="upload"
                    :show-upload-list="false"
                    :before-upload="handleBeforeUpload"
                    multiple
                    type="drag"
                    action="//jsonplaceholder.typicode.com/posts/"
                    style="display: inline-block;width:58px;"
                  >
                    <div style="width: 58px;height:58px;line-height: 58px;">
                      <Icon type="ios-camera" size="32" color="#4f95e8"></Icon>
                    </div>
                  </Upload>
                  <div class="demo-upload-list" v-for="item in uploadList" :key="item.index">
                    <img :src="item" />
                    <div class="demo-upload-list-cover">
                      <Icon type="ios-trash-outline" @click.native="handleRemove(item)"></Icon>
                    </div>
                  </div>
                </div>
              </FormItem>
            </Col>
          </Row>
        </Form>
      </div>
      <div slot="footer" style="text-align: center">
        <Button type="primary" @click="confirm">确定</Button>
        <Button type="default" @click="cancel">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import swsUpload from '_c/sws-upload/'
import swsTable from '_c/sws-table/'
import Operate from '@/components/operate'
import editor from '_c/editor'
import FullScreen from './fullScreen'
import dingwei from '@/assets/images/dingwei.png'
import { setTimeCurrent } from '@/libs/util'
// 按鈕權限：新增，导入，详情，删除，修改
// ['JGGL_XZJG', 'JGGL_DR', 'JGGL_XQ', 'JGGL_SC', 'JGGL_XG']
const BUTTONROLE = {
  JGGL_XZJG: 'JGGL_XZJG',
  JGGL_DR: 'JGGL_DR',
  JGGL_XQ: 'JGGL_XQ',
  JGGL_SC: 'JGGL_SC',
  JGGL_XG: 'JGGL_XG'
}
const IMPORTID = 2
export default {
  data () {
    return {
      importId: IMPORTID,
      modalTitle: '添加',
      searchKey: '',
      fullscreenElement: false,
      screenObj: null,
      geocoder: '', // 地址转经纬度
      img: '', // 上传图片base64码
      imgName: '',
      uploadList: [], // 上传图片列表
      ContactManList: [], // 负责人
      total: 0, // 总机构
      current: 1, // 当前页码
      size: 8, // 每页数据条数
      loading: false,
      operationId: '', // 机构id
      delModal: false,
      addModal: false,
      modifyModal: false,
      // mapSetting: {
      //   // 地图
      //   center: [106.490232, 29.633042],
      //   zoom: 10,
      //   resizeEnable: true
      // },
      searchOption: {
        // 地图搜索
        city: '重庆市',
        citylimit: false
      },
      markers: [
        // 地图标记
      ],
      plugin: [
        {
          // 地图工具条
          pName: 'ToolBar',
          events: {
            init (instance) {
              // console.log(instance);
            }
          },
          liteStyle: true,
          position: 'RT'
        }
      ],
      searchType: [
        // 筛选类型
        {
          value: 'type1',
          label: 'type1'
        }
      ],
      table_columns: [
        {
          title: '机构名称',
          key: 'dialysisName',
          maxWidth: 220,
          slot:'dialysisName',
          render: (h, params) => {
            if(params.row.centerWebURL){
              return  h('strong',{
              style:{
                cursor:'pointer',
                color:'#2d8cf0'
              },
              domProps:{
                  // innerHTML:"<a href = '"+'http://192.168.10.22:8082/'+"?centerWebpwd="+params.row.centerWebpwd+"&centerWebuser="+params.row.centerWebuser+"'target='_blank'>"+params.row.dialysisName + '</a>'
                  innerHTML:"<a href = '"+ params.row.centerWebURL+"?centerWebpwd="+params.row.centerWebpwd+"&centerWebuser="+params.row.centerWebuser+"'target='_blank'>"+params.row.dialysisName + '</a>'
                },
            },
              params.row.dialysisName
            )
            }else{
              return  h('strong',{
              domProps:{
                  // innerHTML:"<a href = '"+'http://192.168.10.22:8082/'+"?centerWebpwd="+params.row.centerWebpwd+"&centerWebuser="+params.row.centerWebuser+"'target='_blank'>"+params.row.dialysisName + '</a>'
                  innerHTML:"<span >"+params.row.dialysisName + '</span>'
                },
            },
              params.row.dialysisName
            )
            }
         
          }
        },
        {
          title: '省份',
          key: 'dialysisProvince',
          width: 85,
          render: (h, params) => {
            return (
              <strong>{this.findProvince(params.row.dialysisRegionID)}</strong>
            )
          }
        },
        {
          title: '所在地区',
          key: 'dialysisRegion',
          width: 90
        },
        {
          title: '成立时间',
          key: 'setUpDate',
          width: 100
        },
        {
          title: '联系电话',
          key: 'dialysisPhone',
          width: 110
        },
        {
          title: '主任',
          key: 'dialysisContactMan',
          maxWidth: 100
        },
        {
          title: '护士长',
          key: 'headNurseMan',
          maxWidth: 100
        },
        {
          title: '详细地址',
          key: 'dialysisAddress'
        },
        {
          title: '操作',
          key: 'action',
          width: 130,
          align: 'center',
          render: (h, params) => {
            return (
              <Operate
                permissionEdit={this.buttonRole.JGGL_XG}
                permissionDelete={this.buttonRole.JGGL_SC}
                handleEdit={() => this.showModify(params.row)}
                handleWatch={() =>
                  this.$router.push({
                    name: 'organization_details',
                    params: { id: params.row.id }
                  })
                }
                handleDelete={() => {
                  this.delModal = true
                  this.operationId = params.row.id
                }}
              />
            )
          }
        }
      ],
      table_data: [], // 表格数据
      formValidate: {
        // 增加机构初始
        dialysisName: '',
        dialysisContactManID: '',
        // dialysisHeadNurseID: '', // 护士长ID
        headNurseId: '', // 护士长ID
        dialysisPhone: '',
        dialysisRegionID: [],
        dialysisAddress: '',
        dialysisDetails: '',
        setUpDate: '',
        runDate: '',
        dialysisLng: 0,
        dialysisLat: 0
      },
      ruleValidate: {
        // 新增框表单验证
        dialysisName: [
          { required: true, message: '请输入机构名称', trigger: 'blur' }
        ],
        dialysisAddress: [
          {
            required: true,
            trigger: 'blur',
            // validator: (rule, value, callback) => {
            //   if (!value) {
            //     return callback(new Error('请输入机构地址'))
            //   }
            //   this.geocoder.getLocation(
            //     this.formValidate.dialysisAddress,
            //     (status, result) => {
            //       if (status === 'complete' && result.info === 'OK') {
            //         // result中对应详细地理坐标信息
            //         this.formValidate.dialysisLng =
            //           result.geocodes[0].location.lng
            //         this.formValidate.dialysisLat = 
            //           result.geocodes[0].location.lat
            //         callback()
            //       } else {
            //         callback(new Error('请输入正确地址'))
            //       }
            //     }
            //   )
            // }
          }
        ],
        dialysisPhone: [
          {
            required: true,
            message: '请输入电话号码',
            trigger: 'blur'
          },
          {
            pattern: /(0\d{2,3}-?\d{7,8})|(1[3584]\d{9})/,
            message: '请正确输入电话号码',
            trigger: 'blur'
          }
        ],
        setUpDate: [
          {
            required: true,
            type: 'date',
            message: '请选择成立时间',
            trigger: 'blur'
          }
        ],
        runDate: [
          {
            required: true,
            type: 'date',
            message: '请选择运行时间',
            trigger: 'blur'
          }
        ]
      },
      DialysisRegion: [],
      format: ['jpg', 'jpeg', 'png'],

      buttonRole: BUTTONROLE
    }
  },
  components: {
    editor,
    swsUpload,
    Operate,
    swsTable
  },
  computed: {
    showFullScreenBtn () {
      return window.navigator.userAgent.indexOf('MSIE') < 0
    },
    pagination () {
      return { total: this.total, current: this.current, pageSize: this.size }
    }
  },
  mounted () {
    this.$nextTick(() => {
      this.getOrganization()
      this.handleGetEmployee() // 员工列表
      this.getAreaList() //
      this.screenObj = new FullScreen()
      this.uploadList = this.$refs.upload.fileList
    })
  },
  methods: {
    // cy 根据地区ID查询到所在省份
    findProvince (arr) {
      if (arr.length >= 1) {
        let [region] = arr
        let value = ''
        this.DialysisRegion.forEach((v, k) => {
          if (v['value'] === region) {
            value = v['regionName']
          }
        })
        return value
      }
    },
    // 上传前钩子函数
    handleBeforeUpload (file) {
      if (this.uploadList.length < 4) {
        // 判断大小
        if (file.size > 1048 * 1048 * 2) {
          this.handleMaxSize(file)
          return false
        }

        const reader = new FileReader()
        let img = null

        // 判断类型
        if (
          !this.format.some(
            item => item.toLocaleLowerCase() === file.type.split('/').pop()
          )
        ) {
          this.handleFormatError(file)
          return false
        }

        reader.readAsDataURL(file)
        reader.onloadend = () => {
          img = reader.result
          this.uploadList.push(img)
        }
      } else {
        this.handleMaxLength()
      }

      return false
    },
    // 删除
    handleRemove (file) {
      this.uploadList.splice(this.uploadList.indexOf(file), 1)
    },
    // 处理上传格式错误
    handleFormatError (file) {
      this.$Notice.warning({
        title: '文件格式错误',
        desc: '文件' + file.name + ' 错误, 请上传jpg或者png格式的图片'
      })
    },
    // 处理上传格式错误
    handleMaxLength () {
      this.$Notice.warning({
        title: '图片数量过多',
        desc: '上传图片数量过多，请控制在4张以内'
      })
    },
    // 处理上传文件过大
    handleMaxSize (file) {
      this.$Notice.warning({
        title: '文件过大',
        desc: '文件  ' + file.name + ' 太大，请上传2M以下的图片'
      })
    },
    // 删除机构事件
    del () {
      this.delModal = false
      let operationId = this.operationId
      this.swsApi.swsGet(`CenterDialysis/del/${operationId}`).then(res => {
        if (res.data.error === null) {
          this.$Message.success('删除机构成功！')
          this.getOrganization()
        } else {
          this.$Notice.error({
            title: '删除错误！',
            desc: res.data.error
          })
        }
      })
    },
    // 增加机构事件
    confirm () {
      let self = this
      // let vaildFields = ['dialysisName', 'setUpDate', 'runDate', 'dialysisPhone', 'dialysisAddress' ]
      // this.validator(vaildFields)
      this.$refs.form.validate(valid => {
        if (valid) {
          let dialysisRegionChecked = JSON.parse(
            JSON.stringify(self.formValidate)
          ).dialysisRegionID.pop()
          let id = this.formValidate.id ? this.formValidate.id : ''
          let addParams = {
            id,
            dialysisCode: '',
            dialysisName: this.formValidate.dialysisName,
            dialysisRegionID: dialysisRegionChecked,
            dialysisContactManID: this.formValidate.dialysisContactManID,
            headNurseId: this.formValidate.headNurseId, // cy 增加护士长Id
            dialysisPhone: this.formValidate.dialysisPhone,
            dialysisAddress: this.formValidate.dialysisAddress,
            dialysisLng: this.formValidate.dialysisLng,
            dialysisLat: this.formValidate.dialysisLat,
            setUpDate: setTimeCurrent(this.formValidate.setUpDate),
            runDate: setTimeCurrent(this.formValidate.runDate),
            dialysisDetails: this.$refs.editor.getHtml(),
            dialysisImg: this.uploadList,
            dataState: 0
          }
          self.swsApi
            .swsPost('CenterDialysis/CreateUpdate', addParams)
            .then(res => {
              if (res.data.error === null) {
                self.$Message.success('操作成功')
                self.addModal = false

                self.current = 1
                self.getOrganization()
              }
            })
        } else {
          self.$Message.error('请完善必填信息')
        }
      })
    },
    validator (fields, callback) {
      return new Promise(resolve => {
        let valid = true
        let count = 0
        fields.forEach(field => {
          this.$refs.form.validateField(field, error => {
            if (error) valid = false

            if (++count === fields.length) {
              // all finish
              resolve(valid)
              if (typeof callback === 'function') {
                callback(valid)
              }
            }
          })
        })
      })
    },
    cancel () {
      this.addModal = false
      this.uploadList = []
      this.$refs['form'].resetFields()
    },
    // 获取机构列表
    getOrganization (i) {
      let page = i || 1
      this.loading = true
      let pageParams = {
        dialysisName: this.searchKey,
        pageNum: page,
        pageSize: this.size
      }
      this.swsApi
        .swsPost('CenterDialysis/DialysisList', pageParams)
        .then(res => {
          if (res.data.success) {
            this.table_data = res.data.result
            this.total = res.data.dataCount
          } else {
            this.$Notice.error({
              title: '请求错误',
              desc: '网络出错，请稍后再试!'
            })
          }
          this.loading = false
        })
        .catch(e => {
          this.loading = false
        })
    },
    // 切换页码
    handleChangePage (pageNum) {
      this.current = pageNum
      this.getOrganization(pageNum)
    },
    search () {
      this.current = 1
      this.getOrganization(this.current)
    },
    // 获取人员
    handleGetEmployee () {
      this.swsApi.swsPost('Employee/Employee', {}).then(res => {
        // console.log(res)
        if (res.data.success) {
          this.ContactManList = res.data.result
        }
      })
    },
    // 获取地区
    getAreaList () {
      this.swsApi.swsPost('Data/region/list').then(res => {
        if (res.data.success) {
          let reg = new RegExp('title', 'g')
          let reg1 = new RegExp('id', 'g')
          let cas = JSON.stringify(res.data.result)
            .replace(reg, 'label')
            .replace(reg1, 'value')
          let cascaderData = JSON.parse(cas)

          this.DialysisRegion = cascaderData
        }
      })
    },
    cascaderChange (v) { },
    handleUploadSuccess () {
      this.search()
    },
    showModify ({ id }) {
      this.addModal = true
      this.modalTitle = '修改'
      let data = this.table_data.filter(v => {
        return v.id === id
      })[0]
      this.uploadList = data.dialysisImgs
      this.$refs.editor.setHtml(data.dialysisDetails)
      this.formValidate = JSON.parse(JSON.stringify(data))
      this.formValidate.setUpDate = new Date(this.formValidate.setUpDate)
      this.formValidate.runDate = new Date(this.formValidate.runDate)
      this.operationId = id
    },
  }
}
</script>

<style lang='less' scoped>
#amap-container {
  position: relative;
  padding: 20px 0;
  height: 440px;
  background: #ffffff;
  .search-box {
    position: absolute;
    top: 40px;
    left: 20px;
    /deep/ .search-btn {
      background-color: #4f95e8;
      width: 50px;
      color: #fff;
      font-size: 15px;
    }
  }
  .map {
    width: 100%;
    height: 100%;
    position: relative;
    .full {
      position: absolute;
      right: 16px;
      top: 20px;
      text-align: center;
      z-index: 500;
      width: 34px;
      height: 34px;
      line-height: 31px;
      background-color: white;
      background-color: rgba(255, 255, 255, 0.9);
      border-radius: 3px;
      border: 1px solid #ccc;
      box-shadow: 1px 1px 10px 0 #ccc;
      cursor: pointer;
    }
  }
}
.organization-operate {
  // margin-top: 20px;
  padding: 20px 0;
  background: #ffffff;
  .operate {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0 20px;
    // .operate-search .ivu-row > * {
    //   margin-top: 10px;
    // }
  }
  .organization-list {
    margin-top: 20px;
  }
}
.ivu-modal-header {
  border-radius: 6px 6px 0 0;
  background: #f6f6f6;
}

.vertical-center-modal {
  display: flex;
  align-items: center;
  justify-content: center;
  .ivu-modal {
    top: 0;
  }
}
.demo-upload-list {
  margin-left: 4px;
  display: inline-block;
  width: 60px;
  height: 60px;
  text-align: center;
  line-height: 60px;
  border: 1px solid transparent;
  border-radius: 4px;
  overflow: hidden;
  background: #fff;
  position: relative;
  box-shadow: 0 1px 1px rgba(0, 0, 0, 0.2);
}
.demo-upload-list img {
  width: 100%;
  height: 100%;
}
.demo-upload-list-cover {
  display: none;
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  background: rgba(0, 0, 0, 0.6);
}
.demo-upload-list:hover .demo-upload-list-cover {
  display: block;
}
.demo-upload-list-cover i {
  color: #fff;
  font-size: 20px;
  cursor: pointer;
  margin: 0 2px;
}

</style>
