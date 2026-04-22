<template>
  <Upload
    ref="upload"
    class="upload"
    :format="format"
    :headers="headers"
    :show-upload-list='false'
    :on-success="handleSuccess"
    :before-upload="handleBeforeUpload"
    :action="uploadAction">
    <Button icon="ios-cloud-upload-outline" :loading="loading">{{loading ? '上传中...' : '导入'}}</Button>
  </Upload>
</template>

<script>
import { getToken } from '@/libs/util'
export default {
  data () {
    return {
      file: null,
      format: ['xls', 'xlsx'],
      loading: false,
      headers: {
        token: getToken()
      },
      URL: process.env.NODE_ENV === 'development' ? this.$config.baseURL.dev : this.$config.baseURL.pro
    }
  },
  props: {
    importId: {
      required: true,
      type: Number
    }
  },
  computed: {
    uploadAction () {
      if (this.file && this.file.name) return `${this.URL}Document/ImportExcel/${this.importId}/${this.file.name}`
      else return `${this.URL}Document/ImportExcel/${this.importId}`
    }
  },
  methods: {

    handleBeforeUpload (file) {
      // cy 修复导入xxx.XLS等文件时因为大写后缀导致导入失败的问题
      let check = this.format.some(item => item.toLocaleLowerCase() === file.name.split('.').pop().toLocaleLowerCase())
      if (!check) {
        this.handleFormatError(file)
        return false
      }
      this.file = file
      this.loading = true
      let promise = new Promise((resolve) => {
        this.$nextTick(function () {
          resolve(true)
        })
      })
      return promise // 通过返回一个promis对象解决
    },
    // 处理上传格式错误
    handleFormatError (file) {
      this.$Notice.warning({
        title: '文件格式错误',
        desc: '文件' + file.name + ' 错误, 请上传xls或xlsx后缀文件'
      })
    },
    handleSuccess (res) {
      if (res.success) {
        this.$Message.success('文件上传成功！')
        this.$emit('on-success-upload')
      } else {
        this.$Message.error('文件上传失败！请检查网络或者表格内容！')
      }
      this.loading = false
    }
  },
  components: {

  }
}
</script>

<style scoped lang="less">
.upload {
  display: inline-block;
  vertical-align: top;
  margin-left: 10px;
}
</style>
