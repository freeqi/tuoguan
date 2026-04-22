<template>
  <div id="medical">
    <div class="content">
      <!-- top -->
      <div class="header">
        <div class="btn-groups">
          <Button type="primary" v-permission="buttonRole.GYS_XZ" @click="showOperate('添加')">新增供应商</Button>
          <sws-upload
            :importId="importId"
            v-permission="buttonRole.GYS_DR"
            @on-success-upload="handleUploadSuccess"
          ></sws-upload>
        </div>
        <div class="filter">
          <Input
            v-model="searchKey"
            search
            enter-button
            @on-search="searchCompany"
            placeholder="请输入关键字"
            style="width: 300px;"
          />
          <span class="text">主营类别</span>
          <Select
            v-model="businessCheckedId"
            multiple
            placeholder="请选择"
            @on-change="filterList"
            transfer
            style="width:200px"
          >
            <Option :value="0">全部</Option>
            <Option v-for="item in businessList" :value="item.id" :key="item.id">{{ item.name }}</Option>
          </Select>
        </div>
      </div>
      <!-- 供应商列表 -->
      <div class="table">
        <Table
          :loading="loading"
          style="margin-top: 20px;"
          size="large"
          :columns="table_column"
          :data="company_data"
        ></Table>
        <div class="pagination" v-if="dataCount > 10">
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
        <span>删除供应商</span>
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

    <Modal
      v-model="operateModel"
      width="900"
      class-name="vertical-center-modal"
      :mask-closable="false"
    >
      <p slot="header">
        <span>{{operateFlage}}供应商</span>
      </p>
      <Form ref="supplier" :model="supplierInfo" :rules="supplierValidate" :label-width="100">
        <Row class="form">
          <Col :lg="12" :md="12" :sm="24">
            <FormItem label="供应商编码" prop="supCode">
              <Input type="text" v-model="supplierInfo.supCode" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="12" :md="12" :sm="24">
            <FormItem label="供应商名称" prop="name">
              <Input type="text" v-model="supplierInfo.name" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="12" :md="12" :sm="24">
            <FormItem label="供应商地址" prop="address">
              <Input type="text" v-model="supplierInfo.address" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="12" :md="12" :sm="24">
            <FormItem label="联系人" prop="linkMan">
              <Input type="text" v-model="supplierInfo.linkMan" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="12" :md="12" :sm="24">
            <FormItem label="联系电话" prop="phone">
              <Input type="text" v-model="supplierInfo.phone" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="12" :md="12" :sm="24">
            <FormItem label="法人代表" prop="legalPerson">
              <Input type="text" v-model="supplierInfo.legalPerson" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="12" :md="12" :sm="24">
            <FormItem label="主营业务" prop="mainBusiness">
              <Input type="text" v-model="supplierInfo.mainBusiness" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="12" :md="12" :sm="24">
            <FormItem label="备注" prop="remark">
              <Input type="text" v-model="supplierInfo.remark" placeholder="请输入"></Input>
            </FormItem>
          </Col>
          <Col :lg="24" :md="24" :sm="24">
            <FormItem label="主营类别" prop="mainCategories">
              <CheckboxGroup v-model="supplierInfo.mainCategories">
                <Checkbox v-for="item in businessList" :label="item.name" :key="item.id"></Checkbox>
              </CheckboxGroup>
            </FormItem>
          </Col>
          <Col :lg="24" :md="24" :sm="24">
            <FormItem label="证件照" prop="mainCategories">
              <Upload
                ref="upload"
                :before-upload="handleUpload"
                multiple
                type="drag"
                action="//jsonplaceholder.typicode.com/posts/"
                style="display: inline-block;width:58px;margin-right: 20px;"
              >
                <div style="width: 58px;height:58px;line-height: 58px;">
                  <Icon type="ios-camera" size="20"></Icon>
                </div>
              </Upload>
              <div
                class="demo-upload-list"
                v-for="(item, index) in uploadList"
                :key="item.url+ index"
              >
                <img :src="item.url" />
                <div class="demo-upload-list-cover">
                  <Icon type="ios-trash-outline" @click.native="handleRemove(item)"></Icon>
                </div>
              </div>
            </FormItem>
          </Col>
        </Row>
      </Form>
      <div slot="footer" style="text-align: center">
        <Button type="primary" v-if="operateFlage=='添加'" @click="submit('添加')">确定</Button>
        <Button type="primary" v-else @click="confirmChangeBtn()">保存</Button>
        <Button type="default" @click="handleReset">取消</Button>
      </div>
    </Modal>
    <transition name="fade">
      <supplier-detail id="supplier-detail" ref="supplierDetail" :company="supplierDetail"></supplier-detail>
    </transition>
  </div>
</template>

<script>
import supplierDetail from './supplier-detail.vue'
import swsUpload from '_c/sws-upload/'
import Operate from '@/components/operate'
// const BusinessType = 27
const BusinessType = '27'
const IMPORTID = 3
const BUTTONROLE = {
  GYS_XZ: 'GYS_XZ',
  GYS_DR: 'GYS_DR',
  GYS_SC: 'GYS_SC',
  GYS_XG: 'GYS_XG'
}
export default {
  data () {
    return {
      importId: IMPORTID,
      loading: false,
      pageSize: 10,
      startPage: 1,
      searchKey: '',
      dataCount: 0,
      company_data: [],
      businessList: [],
      businessCheckedId: 0,
      table_column: [
        {
          title: '供应商编码',
          key: 'supCode',
          width: 120
        },
        {
          title: '供应商名称',
          key: 'name'
        },
        {
          title: '地址',
          key: 'address'
        },
        {
          title: '主营类别',
          key: 'mainCategories'
        },
        {
          title: '联系人',
          key: 'linkMan',
          width: 110
        },
        {
          title: '联系电话',
          key: 'phone',
          width: 130
        },
        {
          title: '操作',
          key: 'action',
          width: 120,
          align: 'center',
          render: (h, params) => {
            return (
              <Operate
                handleDelete={() => {
                  this.delModal = true
                  this.supplierId = params.row.id
                }}
                handleEdit={() => {
                  this.operateFlage = '修改'
                  this.operateModel = true
                  this.supplierInfo = JSON.parse(JSON.stringify(params.row))
                  this.uploadList = []
                  this.supplierInfo.image1 &&
                    this.uploadList.push({ url: this.supplierInfo.image1 })
                  this.supplierInfo.image2 &&
                    this.uploadList.push({ url: this.supplierInfo.image2 })

                  this.supplierInfo.mainCategories = params.row.mainCategories
                    ? params.row.mainCategories.split(',')
                    : []
                }}
                handleWatch={() => {
                  this.supplierDetail = params.row
                  this.$refs.supplierDetail.show()
                }}
                permissionEdit={this.buttonRole.GYS_XG}
                permissionDelete={this.buttonRole.GYS_SC}
              />
            )
          }
        }
      ],
      supplierInfo: {
        address: '',
        name: '',
        supCode: '',
        legalPerson: '',
        mainBusiness: '',
        mainCategories: [],
        linkMan: '',
        phone: '',
        dataState: 0,
        remark: '',
        image1: '',
        image2: ''
      },
      supplierValidate: {
        // supCode: [
        //   {
        //     required: true,
        //     type: 'string',
        //     message: '请输入供应商编码',
        //     trigger: 'blur'
        //   }
        // ],
        name: [
          {
            required: true,
            type: 'string',
            message: '请输入供应商名称',
            trigger: 'blur'
          }
        ]
      },
      supplierDetail: {},
      delModal: false,
      supplierId: -1,
      operateModel: false,
      operateFlage: '添加',
      uploadList: [],
      file: null,
      format: ['jpg', 'jpeg', 'png'],

      buttonRole: BUTTONROLE
    }
  },
  props: {},
  mounted () {
    this.$nextTick(() => {
      // 加载机构
      let params = {
        typeId: BusinessType
      }
      this.swsApi
        .swsPost('SystemDictionary/DictionaryList', params)
        .then(res => {
          if (res.data.success) {
            this.businessList = res.data.result
            this.getCompanyList()
          } else {
            this.$Notice.error({
              title: '请求错误',
              desc: '网络错误，请稍后再试！'
            })
          }
        })
        .catch(e => {
          console.log(e)
        })
    })
  },
  computed: {
    mainCategoriesCheckedList () {
      let arr = []
      if (
        this.supplierInfo.mainCategories.length &&
        this.supplierInfo.mainCategories[0] !== false
      ) {
        arr = this.supplierInfo.mainCategories.map(v => {
          let id = -1
          id = this.businessList.filter(_v => {
            return _v.name === v
          })[0].id

          return id
        })
      }
      return arr
    }
  },
  methods: {
    changePage (i) {
      this.getCompanyList(i)
    },
    // 获取供应商列表
    getCompanyList (i) {
      i = i || 1
      let args = {
        name: this.searchKey,
        mainCategories: this.businessCheckedId,
        pageSize: this.pageSize,
        pageNum: i
      }
      this.loading = true
      this.swsApi
        .swsPost('Data/Supplier/list', args)
        .then(res => {
          this.loading = false
          if (res.data.result) {
            this.company_data = res.data.result
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
    searchCompany () {
      this.startPage = 1
      this.getCompanyList()
    },
    filterList (ids) {
      this.searchKey = ''
      this.startPage = 1
      this.getCompanyList()
    },
    del () {
      this.delModal = false
      this.swsApi
        .swsPost(`Data/Supplier/del/${this.supplierId}`)
        .then(res => {
          if (res.data.result) {
            this.$Message.success('删除成功！')
            this.getCompanyList(this.startPage)
          } else {
            this.$Message.error(res.data.error)
          }
        })
        .catch(e => {
          this.loading = false
        })
    },
    showOperate (name) {
      if (name === '添加') {
        this.$refs.supplier.resetFields()
      }
      this.operateFlage = name
      this.operateModel = true
    },
    handleReset () {
      this.uploadList = []
      this.supplierInfo.image1 = ''
      this.supplierInfo.image2 = ''
      this.operateModel = false
      this.$refs.supplier.resetFields()
    },
    // 20200507
    confirmChangeBtn () {
      this.$Modal.confirm({
        title: '提示',
        content: '<p>供应商名称修改后可能会影响历史单据信息，确定要进行修改操作吗？</p>',
        onOk: () => this.submit('修改')
      })
    },
    submit (text) {
      this.$refs['supplier'].validate(valid => {
        if (valid) {
          this.supplierInfo.mainCategories = this.mainCategoriesCheckedList

          if (this.uploadList.length === 1) {
            this.supplierInfo.image2 = ''
          }
          this.uploadList.forEach((v, i) => {
            let url = this.uploadList[i].url
            if (i === 0) {
              this.supplierInfo.image1 = url
            } else if (i === 1) {
              this.supplierInfo.image2 = url
              // eslint-disable-next-line no-useless-return
            } else return
          })

          this.loading = true
          this.swsApi
            .swsPost('Data/Supplier/CreateUpdate', this.supplierInfo)
            .then(res => {
              this.loading = false
              if (res.data.result) {
                this.$Message.success(`${text}成功`)
                this.getCompanyList()
              } else {
                this.$Notice.error({
                  title: '请求错误',
                  desc: '网络错误，请稍后再试'
                })
              }
              this.operateModel = false
              this.handleReset()
            })
            .catch(e => {
              console.log(e)
              this.loading = false

              this.operateModel = false
              this.handleReset()
            })
        } else {
          this.$Message.error('请检查是否输入完成')
        }
      })
    },
    handleRemove (obj) {
      this.uploadList.splice(this.uploadList.indexOf(obj), 1)
    },
    handleUpload (file) {
      const check = this.uploadList.length < 2
      if (!check) {
        this.$Notice.warning({
          title: '上传图片不得超过两张，请重新选择！'
        })
      } else {
        let reader = new FileReader()
        // readAsDataURL 方法用于读取指定 Blob 或 File 的内容
        // 当读操作完成，readyState 变为 DONE，loadend 被触发，此时 result 属性包含数据：URL（以 base64 编码的字符串表示文件的数据）
        // 读取文件作为 URL 可访问地址
        reader.readAsDataURL(file)
        // 判断大小
        if (file.size > 1048 * 1048 * 2) {
          this.handleMaxSize(file)
          return false
        }
        // 判断类型
        if (
          !this.format.some(
            item => item.toLocaleLowerCase() === file.type.split('/').pop()
          )
        ) {
          this.handleFormatError(file)
          return false
        }
        reader.onloadend = e => {
          file.url = reader.result
          this.uploadList.push(file)
        }
      }
      return false
    },
    handleFormatError (file) {
      this.$Notice.warning({
        title: '所选文件格式不正确，请重新选择'
      })
    },
    handleMaxSize (file) {
      this.$Notice.warning({
        title: '图片内存太大，请重新选择！'
      })
    },
    handleUploadSuccess () {
      this.searchCompany()
    }
  },
  components: {
    supplierDetail,
    swsUpload
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
    height: 100%;
    overflow-y: auto;
    .header {
      padding: 20px;
      background: #ffffff;
      .btn-groups {
        & > * {
          margin-bottom: 20px;
        }
      }
      .filter {
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
    .table {
      margin-bottom: 20px;
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
    .pagination {
      padding: 10px;
      &::after {
        content: '';
        display: block;
        height: 0;
        visibility: hidden;
        clear: both;
      }
    }
  }
}
.color-gray {
  color: #999;
}
.demo-upload-list {
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
  margin-right: 4px;
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
#supplier-detail {
  position: absolute;
  top: 0;
  right: 0;
  width: 500px;
  height: 100%;
  z-index: 10;
}
</style>
