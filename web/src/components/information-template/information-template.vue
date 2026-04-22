<template>
  <div id="info_template">
    <div class="content">
      <!-- <div class="left menu" v-if="!hideSideMenu">
        <h4 class="title">知识分类</h4>
        <Menu ref="menu" :open-names="['1']" @on-select="changeMenuItem" :active-name="activeType">
          <Submenu name="1">
            <template slot="title">全部({{totalDoc}})</template>
            <MenuItem :name="item.value" v-for="item in knowledgeTypeS" :key="item.value">{{ item.label }}({{item.count}})</MenuItem>
          </Submenu>
        </Menu>
      </div>-->
      <sws-aside class="left menu" name="知识分类" v-if="!hideSideMenu">
        <Menu ref="menu" :open-names="['1']" @on-select="changeMenuItem" :active-name="activeType">
          <Submenu name="1">
            <template slot="title">全部({{totalDoc}})</template>
            <MenuItem
              :name="item.value"
              v-for="item in knowledgeTypeS"
              :key="item.value"
            >{{ item.label }}({{item.count}})</MenuItem>
          </Submenu>
        </Menu>
      </sws-aside>
      <div class="main">
        <Row class="operate">
          <Col :lg="8" :md="18">
            <Input
              class="search"
              v-model="searchKey"
              @on-search="keySearch"
              search
              enter-button
              placeholder="请输入关键字查询"
            />
          </Col>
          <Col :lg="16" :md="6" style="text-align: right">
            <Button
              icon="ios-cloud-upload-outline"
              v-permission="buttonRole.XXGL_SCWJ"
              @click="addFile=true"
              type="primary"
            >上传文件</Button>
          </Col>
        </Row>
        <div class="table">
          <Table :loading="loading" :columns="table_column" :data="doc_data"></Table>
          <div style="margin: 10px;" class="pagination" v-if="dataCount > 10">
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

      <Modal title="上传文件" v-model="addFile" width="380" class-name="vertical-center-modal">
        <Form ref="addFile" :model="addFileModel" :rules="addFileModelValidate" :label-width="80">
          <FormItem label="别名" prop="title">
            <Input type="text" v-model="addFileModel.title" placeholder></Input>
          </FormItem>
          <FormItem label="类别" prop="type" v-if="!hideSideMenu">
            <Select v-model="addFileModel.type" placeholder="请选择文档类别">
              <Option
                v-for="item in knowledgeTypeS"
                :value="item.value"
                :key="item.value"
              >{{ item.label }}</Option>
            </Select>
          </FormItem>
          <FormItem label="备注" prop="note">
            <Input type="text" v-model="addFileModel.note" placeholder></Input>
          </FormItem>
          <FormItem style="margin-bottom: 0;">
            <Upload
              ref="upload"
              :before-upload="handleUpload"
              :on-success="handleSuccess"
              :show-upload-list="false"
              :format="['pdf','pptx','docx','DOC','xlsx','png','jpg','jpeg','gif','txt', 'xls']"
              :on-format-error="formatError"
              :action="`${url}`"
            >
              <Button icon="ios-cloud-upload-outline">上传</Button>
            </Upload>
            <div class="uploaded-file" v-if="file !== ''">
              <span>已选文件: {{ file.name }}</span>
              <Icon class="close" type="md-close" size="14" @click="file=''" />
            </div>
          </FormItem>
        </Form>
        <div slot="footer" style="text-align: center">
          <Button type="primary" size="large" @click="uploadF">确定</Button>
          <Button type="default" size="large" @click="handleReset('addFile', 'addFile')">取消</Button>
        </div>
      </Modal>
      <Modal title="修改文档" v-model="modifyFile" width="380" class-name="vertical-center-modal">
        <Form
          ref="modifyFile"
          :model="modifyFileModel"
          :rules="modifyFileModelValidate"
          :label-width="80"
        >
          <FormItem label="别名" prop="title">
            <Input type="text" v-model="modifyFileModel.title" placeholder></Input>
          </FormItem>
          <FormItem label="备注" prop="note">
            <Input type="text" v-model="modifyFileModel.note" placeholder></Input>
          </FormItem>
        </Form>
        <div slot="footer" style="text-align: center">
          <Button type="primary" size="large" @click="updateF">确定</Button>
          <Button type="default" size="large" @click="modifyFile=false">取消</Button>
        </div>
      </Modal>
    </div>
    <Modal v-model="watch" class-name="vertical-center-modal" :footer-hide="true" title="查看图片">
      <div class="loading" v-if="imgLoading">
        <Spin fix>
          <Icon type="ios-loading" size="18" class="demo-spin-icon-load"></Icon>
          <div>图片加载中，请稍后</div>
        </Spin>
      </div>
      <img v-else style="width: 100%" :src="watchImg" alt srcset />
    </Modal>
  </div>
</template>

<script>
import swsAside from '@/components/sws-aside'
import { getNowFormatDate, on } from '@/libs/tools.js'
import EXCEL from '@/assets/images/system_icon_excel.png'
import IMG from '@/assets/images/system_icon_img.png'
import PDF from '@/assets/images/system_icon_pdf.png'
import PPT from '@/assets/images/system_icon_ppt.png'
import TXT from '@/assets/images/system_icon_txt.png'
import WORD from '@/assets/images/system_icon_word.png'
import OTHER from '@/assets/images/system_icon_other.png'

/**
 * 根据文档类型选择相应icon
 * @param type 文档类型
 */
const chooseType = type => {
  // eslint-disable-next-line no-unused-vars
  let imgUrl = ''
  switch (type) {
    case 1:
      return (imgUrl = WORD)
    case 2:
      return (imgUrl = EXCEL)
    case 3:
      return (imgUrl = PPT)
    case 4:
      return (imgUrl = TXT)
    case 5:
      return (imgUrl = IMG)
    case 6:
      return (imgUrl = PDF)
    default:
      return (imgUrl = OTHER)
  }
}
/**
 * 图片加载
 * @param src 图片地址
 */
const imgLoad = src => {
  let img = new Image()
  img.src = src
  return new Promise((resolve, reject) => {
    on(img, 'load', () => {
      resolve('图片加载成功')
    })

    on(img, 'error', () => {
      reject(new Error('图片加载失败，请稍后再试！'))
    })
  })
}
const BUTTONROLE = {
  XXGL_SCWJ: 'XXGL_SCWJ',
  XXGL_XG: 'XXGL_XG',
  XXGL_YL: 'XXGL_YL',
  XXGL_XZ: 'XXGL_XZ'
}
export default {
  data () {
    return {
      buttonRole: BUTTONROLE,
      URL:
        process.env.NODE_ENV === 'development'
          ? this.$config.baseURL.dev
          : this.$config.baseURL.pro,
      addFile: false,
      modifyFile: false,
      searchKey: '',
      file: '',
      loading: false, // 表格加载效果
      startPage: 1,
      pageSize: 10,
      dataCount: -1, // 文档数量
      knowledgeType: -1, // 缓存所选文档库类型
      addFileModel: {
        title: '',
        note: '',
        type: this.knowledgeType
      },
      // 预览框
      watch: false,
      watchImg: '',
      knowledgeTypeS: [
        {
          label: '',
          value: '',
          count: 0
        }
      ],
      modifyFileModel: {
        title: '',
        note: ''
      },
      addFileModelValidate: {
        title: [
          {
            required: true,
            message: '请输入别名',
            trigger: 'blur'
          }
        ],
        type: [
          {
            required: true,
            type: 'number',
            message: '请选择文档类别',
            trigger: 'change'
          }
        ]
      },
      modifyFileModelValidate: {
        title: [
          {
            required: true,
            message: '请输入别名',
            trigger: 'blur'
          }
        ]
      },

      // 表格
      doc_data: [],
      table_column: [],
      table_column_default: [
        {
          title: '文件名',
          key: 'title',
          minWidth: 200,
          render: (h, params) => {
            let type = params.row.fileType
            let url = chooseType(type)
            return (
              <div style="display: flex; alignItems: center;">
                <img
                  src={url}
                  title="icon"
                  alt="icon"
                  style="marginRight: 5px"
                />
                <strong>{params.row.title}</strong>
              </div>
            )
          }
        },
        {
          title: '上传时间',
          minWidth: 120,
          key: 'publishTime'
        },
        {
          title: '下载次数',
          minWidth: 120,
          align: 'center',
          render: (h, params) => {
            return <span>{params.row.downloadCount}次</span>
          }
        },
        {
          title: '文件大小',
          width: 130,
          align: 'center',
          render: (h, params) => {
            return <span>{params.row.fileSize}KB</span>
          }
        }
      ],
      table_column_action: [
        {
          title: '操作',
          key: 'action',
          width: 150,
          align: 'center',
          render: (h, params) => {
            return (
              <div>
                <tooltip content="预览" v-permission={this.buttonRole.XXGL_YL} placement="top">
                  <a
                    target="blank"
                    onClick={() => this.preview(params.row, event)}
                    style="cursor: pointer; marginRight: 10px; color: #ff7b9e"
                    href={`${this.URL}Document/fileview/${params.row.id}`}
                  >
                    预览
                  </a>
                </tooltip>
                <tooltip content="修改" v-permission={this.buttonRole.XXGL_XG} placement="top">
                  <span
                    onClick={() => this.showModify(params.row)}
                    style="cursor: pointer; marginRight: 10px; color: #74bce9"
                  >
                    修改
                  </span>
                </tooltip>
                <tooltip content="下载" v-permission={this.buttonRole.XXGL_XZ} placement="top">
                  <span
                    onClick={() => {
                      let aDom = document.createElement('a')
                      aDom.href = `${this.URL}Document/filedownload/${
                        params.row.id
                      }`
                      aDom.click()
                      aDom.remove()
                      aDom = null
                      this.keySearch()
                    }}
                    style="cursor: pointer; color: #4a9dfd"
                  >
                    下载
                  </span>
                </tooltip>
              </div>
            )
          }
        }
      ],
      // 图片加载
      imgLoading: true
    }
  },
  props: {
    api: {
      type: String
      // default: 'Document/doc/DocKnowledgeList/'
    },
    hideSideMenu: {
      type: Boolean,
      default: false
    },
    CATALOG: {
      type: Number,
      required: true
    }
  },
  created () {
    if (this.hideSideMenu) {
      delete this.addFileModelValidate.type
      // console.log(this.addFileModelValidate)
    }
  },
  mounted () {
    this.$nextTick(() => {
      if (this.hideSideMenu) {
        this.getDocList(this.startPage)
      } else {
        this.getKnowledgeTypeList()
      }

      let hasPreview = this._filterButton(this.buttonRole.XXGL_YL)
      let hasEdit = this._filterButton(this.buttonRole.XXGL_XG)
      let hasDownload = this._filterButton(this.buttonRole.XXGL_XZ)
      if (hasPreview || hasEdit || hasDownload) {
        this.table_column = [...this.table_column_default, ...this.table_column_action]
      } else {
        this.table_column = this.table_column_default
      }
    })
  },
  computed: {
    // 全部文档数量
    totalDoc () {
      return this.knowledgeTypeS.reduce((s, v) => {
        return s + v.count
      }, 0)
    },
    activeType () {
      return this.knowledgeTypeS[0].value
    },
    url () {
      let url =
        this.URL + 'Document/fileup/' + this.CATALOG + '/' + this.file.name
      return url
    }
  },
  methods: {
    // 获取知识库文档类型列表
    getKnowledgeTypeList () {
      this.swsApi
        .swsPost(`Document/doc/DocKnowledgeList/${this.CATALOG}`)
        .then(res => {
          let data = res.data
          let flage = this.knowledgeType <= -1
          this.knowledgeTypeS = data.result
          this.knowledgeType =
            this.knowledgeType <= -1
              ? this.knowledgeTypeS[0].value
              : this.knowledgeType
          flage && this.getDocList(this.startPage)

          this.$nextTick(() => {
            this.$refs.menu.updateOpened()
            this.$refs.menu.updateActiveName()
          })
        })
        .catch(e => {
          console.log(e)
        })
    },
    handleUpload (file) {
      // let fileReader = new FileReader()
      // fileReader.onload = function () {
      //   console.log(this.result)
      // }
      // fileReader.readAsBinaryString(file)
      this.file = file
      return false
    },
    // 上传文件
    uploadF () {
      this.$refs['addFile'].validate(valid => {
        this.addFile = false
        if (valid && this.file) {
          this.$refs.upload.post(this.file)
        } else {
          this.$Message.error('请检查是否输入完成')
        }
      })
    },
    handleSuccess (res) {
      // 文件上传出错
      if (!res.success) {
        this.error = true
        this.$Notice.error({
          title: '上传失败',
          desc: res.error
        })
      } else {
        let params = {
          fileName: res.result,
          title: this.addFileModel.title,
          fileSize: this.file.size,
          note: this.addFileModel.note,
          publishTime: getNowFormatDate(true),
          catalog: this.CATALOG
        }

        if (!this.hideSideMenu) {
          params.knowledgeType = this.addFileModel.type
        }
        // 图片上传成功后继续上传字段
        this.swsApi
          .swsPost('Document/doc/new', params)
          .then(res => {
            if (res.data.success) {
              this.$Message.success('文件上传成功！')
              this.file = ''
              this.$refs.addFile.resetFields()
              this.addFile = false
              // 上传完成同时更新左侧菜单和表格数据
              this.startPage = 1
              this.getDocList(this.startPage)
              this.getKnowledgeTypeList()
            } else {
              this.$Notice.error({
                title: '上传失败',
                desc: '文件上传失败，请联系管理员!'
              })
            }
          })
          .catch(e => {
            console.log(e)
          })
      }
    },
    // 改变文档库类型
    changeMenuItem (name) {
      if (name === this.knowledgeType) return
      this.knowledgeType = name
      this.startPage = 1
      this.searchKey = ''
      this.getDocList(this.startPage)
    },
    // 分页
    changePage (pageNum) {
      this.getDocList(pageNum)
    },
    // 查询文档
    getDocList (pageNum) {
      let params = {
        fileName: this.searchKey, // 别名
        catalog: this.CATALOG,
        pageSize: this.pageSize,
        pageNum: pageNum
      }
      if (!this.hideSideMenu) {
        params.knowledgeType = this.knowledgeType
      }

      this.loading = true
      this.swsApi
        .swsPost('Document/doc/list', params)
        .then(res => {
          let data = res.data
          this.dataCount = data.dataCount
          this.doc_data = data.result
          this.loading = false
        })
        .catch(e => {
          this.loading = false
        })
    },
    // 关键字搜索
    keySearch () {
      this.startPage = 1
      this.getDocList(this.startPage)
    },
    // 更新
    updateF () {
      this.loading = true
      this.modifyFile = false
      this.swsApi
        .swsPost('Document/doc/update', this.modifyFileModel)
        .then(res => {
          let data = res.data
          if (data.success) {
            this.$Message.success('更新成功')
            this.startPage = 1
            this.getDocList(this.startPage)
          }

          this.loading = false
        })
        .catch(e => {
          this.loading = false
        })
    },
    handleReset (name, modal) {
      this[modal] = false
      this.$refs[name].resetFields()
    },
    // 上传格式出错
    formatError () {
      this.$Notice.error({
        title: '格式错误',
        desc: '文件格式错误，请重新选择'
      })
    },
    // 预览
    preview ({ fileType, id }, e) {
      if (fileType === 5) {
        e.preventDefault()
        let _src = `${this.URL}Document/filedownload/${id}`
        this.watch = true
        this.imgLoading = true

        let loadImg = imgLoad(_src)

        loadImg
          .then(res => {
            this.imgLoading = false
            this.watchImg = _src
          })
          .catch(e => {
            this.$Notice.error({
              title: '请求错误',
              desc: e
            })
          })
      }
    },
    // 显示修改框
    showModify ({ id }) {
      let doc = this.doc_data.filter(v => {
        return v.id === id
      })[0]

      this.modifyFileModel = Object.assign(this.modifyFileModel, doc)
      this.modifyFile = true
    }
  },
  components: {
    swsAside
  }
}
</script>

<style scoped lang="less">
#info_template {
  position: relative;
  height: 100%;
  background: #ffffff;
  .content {
    display: flex;
    width: 100%;
    height: 100%;
    & > * {
      height: 100%;
    }
    .left {
      /deep/ .ivu-menu {
        width: 100% !important;
        font-size: 14px;
        background: #ffffff;
        &::after {
          display: none !important;
        }
        .ivu-menu-item-active:not(.ivu-menu-submenu) {
          color: #333;
          background: #f4f3f3;
          z-index: 2;
          &::after {
            display: none;
          }
        }
      }
    }
    .main {
      flex: 1;
      width: 0;
      overflow-y: auto;
      .operate {
        padding: 20px;
        .search {
          width: 230px;
        }
      }
      .table {
        padding: 20px;
        background: #ffffff;
        .pagination {
          &::after {
            content: '';
            display: block;
            height: 0;
            clear: both;
          }
        }
      }
      .ivu-table-wrapper {
        border: none !important;
        & /deep/ .ivu-table {
          font-size: 14px !important;
          &::before,
          &::after {
            display: none !important;
          }
        }
        & /deep/ .ivu-table th {
          font-size: 14px;
          border-bottom: none;
        }
      }
    }
  }
}

/deep/ .ivu-menu-submenu-title i.ivu-menu-submenu-title-icon {
  top: 23px !important;
}
/deep/ .ivu-modal .ivu-form-item-content .uploaded-file {
  margin-top: 10px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 10px;
  line-height: 26px;
  &:hover {
    color: #2d8cf0;
    background: #f3f3f3;
    transition: all 0.2s ease-in-out;
  }
  .close {
    cursor: pointer;
  }
}
.loading {
  position: relative;
  height: 200px;
  .demo-spin-icon-load {
    animation: ani-demo-spin 1s linear infinite;
  }
  @keyframes ani-demo-spin {
    from {
      transform: rotate(0deg);
    }
    50% {
      transform: rotate(180deg);
    }
    to {
      transform: rotate(360deg);
    }
  }
}
</style>
